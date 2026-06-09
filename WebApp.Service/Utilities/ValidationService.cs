using WebApp.Dal.Entities;
using WebApp.Service.Exceptions;
using WebApp.Service.Interfaces;

namespace WebApp.Service.Utilities;

public class ValidationService() : IValidationService
{
	public void ValidateEmail(string email)
	{
		var notValidMsg = $"Email '{email}' is not valid!";
		if(string.IsNullOrWhiteSpace(email))
		{
			throw new BadRequestException(exceptionMsg: "Failed to validate email!", reasonMsg: "Email cannot be empty!");
		}
		try
		{
			var addr = new System.Net.Mail.MailAddress(email);
			if(addr.Address != email)
			{
				throw new BadRequestException(exceptionMsg: "Failed to validate email!", reasonMsg: notValidMsg);
			}
		}
		catch
		{
			throw new BadRequestException(exceptionMsg: "Failed to validate email!", reasonMsg: notValidMsg);
		}
	}
	public void ValidateWallet(Wallet wallet)
	{
		if(wallet == null)
		{
			throw new BadRequestException("Failed to validate wallet!", "Wallet cannot be null!");
		}
		if(string.IsNullOrWhiteSpace(wallet.Name))
		{
			throw new BadRequestException(Messages.FailedToValidate(wallet), "Wallet name cannot be empty!");
		}
		if(wallet.Balance < 0)
		{
			throw new BadRequestException(Messages.FailedToValidate(wallet), "Wallet balance cannot be negative!");
		}
	}
	public void ValidateTransaction(Transaction transaction)
	{
		if(transaction == null)
		{
			throw new BadRequestException("Failed to validate transaction!", "Transaction cannot be null!");
		}
		if(string.IsNullOrWhiteSpace(transaction.Name))
		{
			throw new BadRequestException(Messages.FailedToValidate(transaction), "Transaction name cannot be null or empty!");
		}
		if(transaction.Amount == 0)
		{
			throw new BadRequestException(Messages.FailedToValidate(transaction), "Transaction amount cannot be zero!");
		}
	}
	public void ValidateCategory(Category category)
	{
		if(category == null)
		{
			throw new BadRequestException("Failed to validate category!", "Category cannot be null!");
		}
		if(string.IsNullOrWhiteSpace(category.Name))
		{
			throw new BadRequestException(Messages.FailedToValidate(category), "Category name cannot be empty!");
		}
		if(category.Description != null && string.IsNullOrWhiteSpace(category.Description))
		{
			throw new BadRequestException(Messages.FailedToValidate(category), "Category description cannot be empty!");
		}
		if(category.Icon != null && string.IsNullOrWhiteSpace(category.Icon))
		{
			throw new BadRequestException(Messages.FailedToValidate(category), "Category icon value cannot be empty!");
		}
		if(category.Color != null)
		{
			ValidateColor(category.Color.ToString());
		}
	}
	public void ValidateLimit(Limit limit)
	{
		if(limit == null)
		{
			throw new BadRequestException("Failed to validate limit!", "Limit cannot be null!");
		}
		if(limit.MaxAmount <= 0)
		{
			throw new BadRequestException(Messages.FailedToValidate(limit), "Limit max amount must be greater than zero!");
		}
		if(limit.StartDate < DateOnly.FromDateTime(DateTime.UtcNow))
		{
			throw new BadRequestException(Messages.FailedToValidate(limit), "Limit start date cannot be in the past!");
		}
		if(limit.EndDate <= limit.StartDate)
		{
			throw new BadRequestException(Messages.FailedToValidate(limit), "Limit end date must be after start date!");
		}
		if(limit.CurrentAmount < 0)
		{
			throw new BadRequestException(Messages.FailedToValidate(limit), "Limit current amount must be greater than zero!");
		}
		if(limit.CurrentAmount > limit.MaxAmount)
		{
			throw new BadRequestException(Messages.FailedToValidate(limit), "Limit exceeded!");
		}
	}
	public void ValidateColor(string color)
	{
		if(string.IsNullOrWhiteSpace(color))
		{
			throw new BadRequestException("Failed to validate color!", "Color cannot be empty!");
		}
		if(!System.Text.RegularExpressions.Regex.IsMatch(color, @"^#[0-9a-fA-F]{6}$"))
		{
			throw new BadRequestException("Failed to validate color!", "Invalid color format!");
		}
	}
}