using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using JohnnyDevCraft.AspNetCore.Auth0.Abstractions;

namespace JohnnyDevCraft.AspNetCore.Auth0.Harness.Services
{
    public class CurrentUserService: IRoleValidator, IPermissionValidator
    {
        public CurrentUserService()
        {

        }

        public bool CurrentUserHasRole(ClaimsIdentity user, string name)
        {
            if (user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email)?.Value == "john@johnnydevcraft.com")
            {
                return true;
            }

            if (name == "User")
            {
                return true;
            }

            return false;
        }

        public bool CurrentUserHasPermission(ClaimsIdentity user, string permission)
        {
            if (user.Claims.SingleOrDefault(c => c.Type == ClaimTypes.Email)?.Value == "john@johnnydevcraft.com")
            {
                return true;
            }

            if (permission == "weather:Can-Read")
            {
                return true;
            }

            return false;
        }

        public List<string> GetPermissions()
        {
            return new List<string>()
            {
                "Weather:Can-Edit",
                "Weather:Can-Read",
            };
        }

        public List<string> GetRoles()
        {
            return new List<string>()
            {
                "Administrator",
                "User"
            };
        }

        public Dictionary<string, List<string>> GetRolePolicies()
        {
            return new Dictionary<string, List<string>>()
            {
                {"EveryRole", new List<string>(){"Administrator", "User"}},
                {"Administrator", new List<string>(){"Administrator"}},
                {"User", new List<string>(){"User"}}
            };
        }
    }
}