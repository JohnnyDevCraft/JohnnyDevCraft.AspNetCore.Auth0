using System.Security.Claims;

namespace JohnnyDevCraft.AspNetCore.Auth0.Abstractions
{
    public interface IPermissionValidator
    {
        bool CurrentUserHasPermission(ClaimsIdentity user, string permission);
    }
}