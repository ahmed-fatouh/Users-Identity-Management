using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersIdentityManagement.Models.AppUserModels;

namespace UsersIdentityManagement.Infrastructure
{
    public class CustomPasswordValidator : IPasswordValidator<AppUser>
    {

        public Task<IdentityResult> ValidateAsync(UserManager<AppUser> manager, AppUser user, string password)
        {
            List<IdentityError> errors = new List<IdentityError>();
            
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
            
            IdentityResult result = errors.Count == 0 ?
                IdentityResult.Success : IdentityResult.Failed(errors.ToArray());
            
            return Task.FromResult(result);
        }
    }
}
