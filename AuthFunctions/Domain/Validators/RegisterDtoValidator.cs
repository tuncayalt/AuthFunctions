using AuthFunctions.Domain.Dtos;
using FluentValidation.Results;
using FluentValidation;

namespace AuthFunctions.Domain.Validators
{
    public class RegisterDtoValidator : AbstractValidator<RegisterDto>
    {
        protected override bool PreValidate(ValidationContext<RegisterDto> context, ValidationResult result)
        {
            if (context.InstanceToValidate == null)
            {
                result.Errors.Add(new ValidationFailure(nameof(RegisterDto), "Please ensure a model was supplied."));
                return false;
            }
            return true;
        }

        public RegisterDtoValidator()
        {
            RuleFor(dto => dto.UserName)
                .NotEmpty().WithName("User Name");

            RuleFor(dto => dto.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(dto => dto.Password)
                .NotEmpty()
                .MinimumLength(8).WithMessage("{PropertyName} must have a minimum length of 8 characters.")
                .Must(MeetPasswordRules).WithMessage("{PropertyName} must have at least one uppercase letter, one lowercase letter, one number and one special character.");

            RuleFor(dto => dto.ConfirmPassword)
                .Matches(dto => dto.Password).WithMessage($"{nameof(RegisterDto.Password)} and {nameof(RegisterDto.ConfirmPassword)} do not match.");
        }

        private bool MeetPasswordRules(string pass)
        {
            bool hasUppercase = false, hasLowercase = false, hasNumber = false, hasSpecialChar = false;

            foreach (char ch in pass)
            {
                if (char.IsUpper(ch)) hasUppercase = true;
                else if (char.IsLower(ch)) hasLowercase = true;
                else if (char.IsDigit(ch)) hasNumber = true;
                else hasSpecialChar = true;
            }

            return hasUppercase && hasLowercase && hasNumber && hasSpecialChar;
        }
    }
}
