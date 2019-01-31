using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersIdentityManagement.Models.AppUserModels;

namespace UsersIdentityManagement.Infrastructure
{
    public class CustomUserValidator : UserValidator<AppUser>
    {
        public override async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user)
        {
            IdentityResult result = await base.ValidateAsync(manager, user);
            List<IdentityError> errors = result.Succeeded ?
                new List<IdentityError>() : result.Errors.ToList();
            errors.ForEach(e => {
                if (e.Code == "InvalidUserName")
                    e.Description = $"Invalid name '{user.UserName}', can only contain lower case letters.";
            });

            if (!user.Email.EndsWith("@example.com"))
            {
                errors.Add(new IdentityError()
                {
                    Code = "InvalidDomainName",
                    Description = "Only 'example.com' domain name is allowed"
                });
            }

            return errors.Count == 0 ? 
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
        }
    }
}
