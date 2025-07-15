using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using IdentityModel;
using IdentityService.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace IdentityService.Pages.Register
{
    [SecurityHeaders]
    [AllowAnonymous]
    public class Index : PageModel
    {
        private readonly UserManager<ApplicationUser> _userManager;

        public Index(UserManager<ApplicationUser> userManager)
        {
            _userManager = userManager;
        }

        [BindProperty]
        public RegisterViewModel Input { get; set; }

        [BindProperty]
        public bool RegisterSuccess { get; set; }

        public IActionResult OnGet(string? returnUrl)
        {
            Console.WriteLine("Return URL FROM Params: " + returnUrl);
            Input = new RegisterViewModel()
            {
                ReturnUrl = returnUrl
            };

            Console.WriteLine("Return URL From GET REGISTER: " + Input.ReturnUrl);

            return Page();
        }

        public async Task<IActionResult> OnPost()
        {
             Console.WriteLine("Return URL from Register POST: " + Input.ReturnUrl);

            // User clicked cancel button
            if (Input.Button != "Register") return Redirect("~/");

            // If cause any mistakes or errors -> present to Validation Summary
            if (ModelState.IsValid)
            {
                // Mapping request properties to ApplicationUser (IdentityUser)
                var user = new ApplicationUser()
                {
                    UserName = Input.Username,
                    Email = Input.Email,
                    EmailConfirmed = true
                };

                // Create new user 
                var result = await _userManager.CreateAsync(user, Input.Password);

                if (result.Succeeded)
                {
                    await _userManager.AddClaimsAsync(user, new[]
                    {
                        new Claim(JwtClaimTypes.Name, Input.Fullname)
                    });

                    RegisterSuccess = true;
                }
            }

            return Page();
        }
    }
}