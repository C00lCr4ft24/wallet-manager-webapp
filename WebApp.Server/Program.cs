using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NSwag;
using NSwag.Generation.Processors.Security;
using WebApp.Dal;
using WebApp.Dal.Entities;
using WebApp.Server.Handlers;
using WebApp.Service.Contexts;
using WebApp.Service.Exceptions;
using WebApp.Service.Interfaces;
using WebApp.Service.Mappings;
using WebApp.Service.Services;
using WebApp.Service.Utilities;

var config = new ConfigurationBuilder().AddUserSecrets<Program>().Build();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AppDbConnection")));

// Identity configuration
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<User>()
				.AddEntityFrameworkStores<AppDbContext>()
				.AddDefaultTokenProviders();
builder.Services.Configure<IdentityOptions>(options =>
{
	options.Password.RequireDigit = false;
	options.Password.RequiredLength = 1;
	options.Password.RequireNonAlphanumeric = false;
	options.Password.RequireUppercase = false;
	options.Password.RequireLowercase = false;
	options.Password.RequiredUniqueChars = 0;
});
builder.Services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
builder.Services.AddScoped<IUserService, UserService>();                            //User
builder.Services.AddScoped<IWalletService, WalletService>();                        //Wallet
builder.Services.AddScoped<ITransactionService, TransactionService>();              //Transaction
builder.Services.AddScoped<ICategoryService, CategoryService>();                    //Category
builder.Services.AddScoped<IFamilyService, FamilyService>();                        //Family
builder.Services.AddScoped<ILimitService, LimitService>();                          //Limit
builder.Services.AddScoped<ICommonService, CommonService>();                        //Common
builder.Services.AddScoped<IAuthorizationService, AuthorizationService>();          //Authorization
builder.Services.AddScoped<IValidationService, ValidationService>();                //Entity Validation

//Request context provider
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IRequestContextProvider, RequestContextProvider>();
//AutoMapper
builder.Services.AddAutoMapper(cfg =>
{
	cfg.LicenseKey = config["AutoMapper:License"];

}, typeof(CategoryMappingProfile),
	typeof(FamilyMappingProfile),
	typeof(InviteMappingProfile),
	typeof(LimitMappingProfile),
	typeof(TransactionMappingProfile),
	typeof(UserMappingProfile),
	typeof(WalletMappingProfile)
);
//Data protection
builder.Services.AddDataProtection();
//Controllers
builder.Services.AddControllers();

builder.Services.AddCors(options =>
{
	options.AddPolicy("Frontend", policy =>
	{
		policy.WithOrigins("http://localhost:5002", "https://localhost:5003")
			  .AllowAnyMethod()
			  .AllowAnyHeader()
			  .AllowCredentials();
	});
});

builder.Services.ConfigureApplicationCookie(options =>
{
	options.Cookie.SameSite = SameSiteMode.None;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

builder.Services.AddProblemDetails();
//Exception handling
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddExceptionHandler<GeneralExceptionHandler>();

builder.Services.AddRazorComponents()
	.AddInteractiveWebAssemblyComponents()
	.AddAuthenticationStateSerialization(options => options.SerializeAllClaims = true);

builder.Services.AddOpenApiDocument(doc =>
{
	doc.Title = "API";
	doc.Version = "v1";
	doc.Description = "WebApp API with Cookie authentication";

	doc.AddSecurity("cookie", [], new OpenApiSecurityScheme
	{
		Type = OpenApiSecuritySchemeType.ApiKey,
		Name = "Cookie",
		In = OpenApiSecurityApiKeyLocation.Header,
		Scheme = "cookie",
		Description = "Cookie-based authentication"
	});		

	doc.OperationProcessors.Add(new AspNetCoreOperationSecurityScopeProcessor());
});

builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();

app.UseCors("Frontend");

app.UseExceptionHandler();

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

var usersApi = app.MapGroup("/user");
usersApi.MapIdentityApi<User>();

usersApi.MapPost("logout", async (SignInManager<User> signInManager) =>
{
	await signInManager.SignOutAsync();
	return Results.Ok();
});

if(app.Environment.IsDevelopment())
{
	app.UseOpenApi();
	app.UseSwaggerUi(options => { options.EnableTryItOut = true; });
	app.UseReDoc();

	app.MapPost("/reset-database/{secret}", async ([FromRoute] string secret, AppDbContext db, SignInManager<User> signInManager) =>
	{
		if(secret != "supersecret") { throw new BadRequestException("Failed to reset database!", "Invalid secret"); }

		await signInManager.SignOutAsync();
		await db.Database.EnsureDeletedAsync();
		await db.Categories.AddRangeAsync(DbSeedValues.DefaultCategories);
		await db.Database.MigrateAsync();
		return Results.Ok();
	});
}

await app.RunAsync();