using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ServerApp.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ServerApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AccountController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        [HttpGet]
        public IActionResult Login(string returnUrl)
        {
            ViewBag.ReturnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel creds, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                if (await DoLogin(creds))
                {
                    return Redirect(returnUrl ?? "/");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password");
                }
            }
            return View(creds);
        }

        [HttpPost]
        public async Task<IActionResult> Logout(string returnUrl)
        {
            await _signInManager.SignOutAsync();
            return Redirect(returnUrl ?? "/");
        }
        [HttpGet("/api/account/test")]
        public IActionResult Test()
        {
            return Ok("test passes");
        }
        [HttpPost("/api/account/login")]
        public async Task<IActionResult> ApiLogin([FromBody]LoginViewModel creds)
        {
            Console.WriteLine("Api Login");
            Console.WriteLine($"name: {creds.Name}, password: {creds.Password}");
            if (ModelState.IsValid && await DoLogin(creds))
            {
                return Ok("true");
            }
            return BadRequest();
        }

        [HttpPost("/api/account/logout")]
        public async Task<IActionResult> ApiLogout()
        {
            await _signInManager.SignOutAsync();
            return Ok();
        }

        private async Task<bool> DoLogin(LoginViewModel creds)
        {
            var user = await _userManager.FindByNameAsync(creds.Name);
            if (user == null)
            {
                return false;
            }
            await _signInManager.SignOutAsync();
            var result = await _signInManager.PasswordSignInAsync(user, creds.Password, false, false);
            return result.Succeeded;
        }

    }
}
