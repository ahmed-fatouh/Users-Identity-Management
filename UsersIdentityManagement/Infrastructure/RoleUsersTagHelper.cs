using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UsersIdentityManagement.Models.AppUserModels;

namespace UsersIdentityManagement.Infrastructure
{
    [HtmlTargetElement("td", Attributes ="identity-role")]
    public class RoleUsersTagHelper : TagHelper
    {
        private UserManager<AppUser> userManger;
        private RoleManager<IdentityRole> roleManager;

        public RoleUsersTagHelper(UserManager<AppUser> usrMgr, RoleManager<IdentityRole> roleMgr)
        {
            userManger = usrMgr;
            roleManager = roleMgr;
        }

        [HtmlAttributeName("identity-role")]
        public string RoleId { get; set; }

        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            List<AppUser> usersInRole = new List<AppUser>();
            IdentityRole role = await roleManager.FindByIdAsync(RoleId);
            if(role != null)
            {
                usersInRole = (await userManger.GetUsersInRoleAsync(role.Name)).ToList();
            }
            string outputContent = usersInRole.Count == 0 ?
                                    "No users" : string.Join(", ", usersInRole);
            output.Content.SetContent(outputContent);
        }
    }
}
