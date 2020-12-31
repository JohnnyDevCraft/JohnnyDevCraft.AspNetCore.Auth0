using System.Security.Claims;

namespace JohnnyDevCraft.AspNetCore.Auth0.Abstractions
{
    public interface IRoleValidator
    {
        bool CurrentUserHasRole(ClaimsIdentity user, string name);
    }
}