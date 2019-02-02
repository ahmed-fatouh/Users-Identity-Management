using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersIdentityManagement.Models.AppUserModels;

namespace UsersIdentityManagement.Infrastructure
{
    public class CustomPasswordValidator : PasswordValidator<AppUser>
    {

        public override async Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            IdentityResult result = await base.ValidateAsync(manager, user, password);

            List<IdentityError> errors = result.Succeeded ?
                                         new List<IdentityError>() : result.Errors.ToList();
            
            if (password.Contains("12345"))
                errors.Add(new IdentityError()
                {
                    Code = "PasswordContainsNumerical Sequence",
                    Description = "Password can't contain numerical sequences."
                });
            if (password.ToLower().Contains(user.UserName.ToLower()))
            {
                errors.Add(new IdentityError
                {
                    Code = "PasswordContainsUserName",
                    Description = "Password can't contain user name."
                });
            }
            
            result = errors.Count == 0 ?
                     IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
            
            return result;
        }

        
    }
}
