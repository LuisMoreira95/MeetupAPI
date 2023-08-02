using FluentValidation;
using MMeetupAPI.Entities;
using MMeetupAPI.Models;

namespace MMeetupAPI.Validators
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserValidator(MeetupContext meetupContext)
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).MinimumLength(6);
            RuleFor(x => x.ConfirmPassword).Equal(x => x.Password);
            RuleFor(x => x.Email).Custom((value, context) =>
            {
                var userAlreadyExists = meetupContext.Users.Any(user => user.Email == value);
                if (userAlreadyExists)
                {
                    context.AddFailure("Email", "That email address is taken");
                }
            });

        }
    }
}
