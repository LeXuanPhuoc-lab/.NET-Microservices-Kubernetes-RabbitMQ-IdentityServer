using System.Security.Claims;
using System.Text.Json;
using System.Xml.Linq;
using Duende.IdentityServer.Hosting.LocalApiAuthentication;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using Duende.IdentityServer.Validation;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Identity;

namespace IdentityService.Services;

public class CustomProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;

    public CustomProfileService(UserManager<ApplicationUser> userManager)
    {
        _userManager = userManager;
    }

    // Request and Add addtional information to our token
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        var user = await _userManager.GetUserAsync(context.Subject);

        // if(user is null) return;

        var existingClaims = await _userManager.GetClaimsAsync(user!);

        var claims = new List<Claim>()
        {
            new Claim("username", user?.UserName!)
        };

        context.IssuedClaims.AddRange(claims);
        context.IssuedClaims.Add(existingClaims.FirstOrDefault(x => x.Type == JwtClaimTypes.Name)!);
    }


    public Task IsActiveAsync(IsActiveContext context)
    {
        return Task.CompletedTask;
    }
}