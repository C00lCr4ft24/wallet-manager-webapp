using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WebApp.Dal.Entities;

namespace WebApp.Dal
{
	public class AppDbContext : IdentityDbContext<User, IdentityRole<int>, int>
	{
		public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

		//FAMILY
		public DbSet<Family> Families { get; set; }
		public DbSet<FamilyUser> FamilyUsers { get; set; }
		public DbSet<FamilyInvite> FamilyInvites { get; set; }
		//////////////////////////////////////////////////////
		public DbSet<Limit> Limits { get; set; }
		public DbSet<Transaction> Transactions { get; set; }
		public DbSet<Category> Categories { get; set; }
		//WALLET
		public DbSet<Wallet> Wallets { get; set; }
		public DbSet<WalletUser> WalletUsers { get; set; }
		public DbSet<WalletInvite> WalletInvites { get; set; }
		//////////////////////////////////////////////////////

		protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
		{
			base.OnConfiguring(optionsBuilder);
			optionsBuilder.ConfigureWarnings(warnings => warnings.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.PendingModelChangesWarning));
		}

		protected override void OnModelCreating(ModelBuilder builder)
		{
			base.OnModelCreating(builder);

			builder.Entity<Family>(entity =>
			{
				entity.HasMany(e => e.Categories)
					.WithOne(e => e.Family)
					.HasForeignKey(e => e.FamilyId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			builder.Entity<Category>(entity =>
			{
				entity.HasOne(e => e.Family)
					.WithMany(e => e.Categories)
					.HasForeignKey(e => e.FamilyId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.Property(e => e.Color).HasMaxLength(7);

				entity.HasIndex(e => new { e.FamilyId, e.Name, e.Icon, e.Color }).IsUnique();

				SeedDefaultCategories(entity);
			});

			builder.Entity<Limit>(entity =>
			{
				entity.Property(e => e.MaxAmount)
					.HasPrecision(18, 2);

				entity.HasOne(e => e.User)
					.WithMany(e => e.Limits)
					.HasForeignKey(e => e.UserId)
					.IsRequired()
					.OnDelete(DeleteBehavior.Cascade);
			});

			builder.Entity<Transaction>(entity =>
			{
				entity.Property(e => e.Amount)
					.HasPrecision(18, 2);

				entity.HasOne(e => e.Wallet)
					.WithMany(e => e.Transactions)
					.HasForeignKey(e => e.WalletId)
					.IsRequired()
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasOne(e => e.Category)
					.WithMany(e => e.Transactions)
					.HasForeignKey(e => e.CategoryId)
					.OnDelete(DeleteBehavior.SetNull);
			});

			builder.Entity<Wallet>(entity =>
			{
				entity.Property(e => e.Balance)
					.HasPrecision(18, 2);

				entity.HasMany(e => e.Transactions)
					.WithOne(e => e.Wallet)
					.HasForeignKey(e => e.WalletId)
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasMany(e => e.WalletUsers)
					.WithOne(e => e.Wallet)
					.HasForeignKey(e => e.WalletId)
					.IsRequired()
					.OnDelete(DeleteBehavior.Cascade);
			});

			builder.Entity<WalletUser>(entity =>
			{
				entity.HasOne(e => e.User)
					.WithMany(e => e.WalletUsers)
					.HasForeignKey(e => e.UserId)
					.IsRequired()
					.OnDelete(DeleteBehavior.Cascade);
				entity.HasOne(e => e.Wallet)
					.WithMany(e => e.WalletUsers)
					.HasForeignKey(e => e.WalletId)
					.IsRequired()
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasIndex(e => new { e.UserId, e.WalletId }).IsUnique();
			});

			builder.Entity<FamilyUser>(entity =>
			{
				entity.HasOne(e => e.User)
					.WithOne(e => e.FamilyUser)
					.HasForeignKey<FamilyUser>(e => e.UserId)
					.IsRequired()
					.OnDelete(DeleteBehavior.Cascade);
				entity.HasOne(e => e.Family)
					.WithMany(e => e.FamilyUsers)
					.HasForeignKey(e => e.FamilyId)
					.IsRequired()
					.OnDelete(DeleteBehavior.Cascade);

				entity.HasIndex(e => new { e.UserId, e.FamilyId }).IsUnique();
			});

			builder.Entity<FamilyInvite>(entity =>
			{
				entity.HasOne(e => e.Family)
					.WithMany(e => e.Invites)
					.HasForeignKey(e => e.FamilyId)
					.IsRequired()
					.OnDelete(DeleteBehavior.NoAction);
				entity.HasOne(e => e.Inviter)
					.WithMany(e => e.SentFamilyInvites)
					.HasForeignKey(e => e.InviterId)
					.IsRequired()
					.OnDelete(DeleteBehavior.NoAction);
				entity.HasOne(e => e.Invitee)
					.WithMany(e => e.ReceivedFamilyInvites)
					.HasForeignKey(e => e.InviteeId)
					.IsRequired()
					.OnDelete(DeleteBehavior.NoAction);

				entity.HasIndex(e => new { e.FamilyId, e.InviteeId, e.InviterId }).IsUnique();
			});

			builder.Entity<WalletInvite>(entity =>
			{
				entity.HasOne(e => e.Wallet)
					.WithMany(e => e.Invites)
					.HasForeignKey(e => e.WalletId)
					.IsRequired()
					.OnDelete(DeleteBehavior.NoAction);
				entity.HasOne(e => e.Inviter)
					.WithMany(e => e.SentWalletInvites)
					.HasForeignKey(e => e.InviterId)
					.IsRequired()
					.OnDelete(DeleteBehavior.NoAction);
				entity.HasOne(e => e.Invitee)
					.WithMany(e => e.ReceivedWalletInvites)
					.HasForeignKey(e => e.InviteeId)
					.IsRequired()
					.OnDelete(DeleteBehavior.NoAction);
				entity.HasIndex(e => new { e.WalletId, e.InviteeId, e.InviterId }).IsUnique();
			});
		}

		private static void SeedDefaultCategories(EntityTypeBuilder<Category> entity) { entity.HasData(DbSeedValues.DefaultCategories); }

	}
	public static class DbSeedValues
	{
		public static readonly Category[] DefaultCategories =
			[
				new Category
				{
					Id = 1,
					Name = "Wages & Salaries",
					Description = "Income from employment",
					Icon = "💵",
					Color = "#4CAF50",
					IsDefault = true,
				},
				new Category
				{
					Id = 2,
					Name = "Housing & Utilities",
					Description = "Rent, mortgage, electricity, water, etc.",
					Icon = "🏠",
					Color = "#FF0000",
					IsDefault = true,
				},
				new Category
				{
					Id = 3,
					Name = "Food & Dining",
					Description = "Restaurants, groceries, dining out",
					Icon = "🍕",
					Color = "#FF7F00",
					IsDefault = true,
				},
				new Category
				{
					Id = 4,
					Name = "Transportation",
					Description = "Gas, public transit, maintenance",
					Icon = "🚕",
					Color = "#FFFF00",
					IsDefault = true,
				},
				new Category
				{
					Id = 5,
					Name = "Health",
					Description = "Medical, fitness, wellness",
					Icon = "💪",
					Color = "#00FFFF",
					IsDefault = true,
				},
				new Category
				{
					Id = 6,
					Name = "Shopping & Retail",
					Description = "Clothing, electronics, other purchases",
					Icon = "👜",
					Color = "#0000FF",
					IsDefault = true,
				},
				new Category
				{
					Id = 7,
					Name = "Entertainment",
					Description = "Movies, games, hobbies",
					Icon = "🎮",
					Color = "#8B00FF",
					IsDefault = true,
				},
				new Category
				{
					Id = 8,
					Name = "Subscriptions & Memberships",
					Description = "Streaming services, gym memberships, etc.",
					Icon = "📺",
					Color = "#FF1493",
					IsDefault = true,
				}
			];
	}
}
