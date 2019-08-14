using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using AspDotNetCoreAuthentication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AspDotNetCoreAuthentication.Controllers
{
    public class AspDotNetCoreSecurityManagerController : Controller
    {
        [HttpGet]
        [RequireHttps]
        [AllowAnonymous]
        public IActionResult ClaimUserIdentity() => View();

        [HttpPost]
        [RequireHttps]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        [AutoValidateAntiforgeryToken] 
        public IActionResult ClaimUserIdentity(UserIdentity _userIdentity)
        {
            if (ModelState.IsValid)
            {
                if (_userIdentity.UserName == FakeDBContext().UserName && _userIdentity.UserRole == FakeDBContext().UserRole && FakeDBContext().Password == _userIdentity.Password)
                {
                    var UserIdentity = new ClaimsIdentity(new[]
                    {
                       new Claim(ClaimTypes.Name,_userIdentity.UserName),
                       new Claim(ClaimTypes.Role ,_userIdentity.UserRole)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var UserPrinciple = new ClaimsPrincipal(UserIdentity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, UserPrinciple);

                    return RedirectToAction("Index", "Home");
                }
                else if (_userIdentity.UserName == FakeDBContext().UserName && _userIdentity.UserRole == FakeDBContext().UserRole && FakeDBContext().Password == _userIdentity.Password)
                {
                    var AdminIdentity = new ClaimsIdentity(new[]
                       {
                       new Claim(ClaimTypes.Name,_userIdentity.UserName),
                       new Claim(ClaimTypes.Role ,_userIdentity.UserRole)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var AdminPrinciple = new ClaimsPrincipal(AdminIdentity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, AdminPrinciple);

                    return RedirectToAction("Index", "Home");
                }
                ViewBag.reponse = "Login attempt was unsuccessful!";
                return View();
            }
            ViewBag.reponse = "Login attempt was unsuccessful!";
            return View();
        }

        public IActionResult RemoveUserIdentity()
        {
            HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction(nameof(ClaimUserIdentity));
        }


        public UserIdentity FakeDBContext()
        {
            return new UserIdentity { UserName = "Admin", Password = "123456", UserRole = "admin" };
        }

         
    }
}
