# AspDotNetCore-Authentication-Authorization
 AspDotNetCore-Authentication-Authorization Demo

This application is an example of  Authentication and Authorization in Asp.Net Core  using idenity.

Secure your Dot.Net Core Application using Asp.Net Core  Authentication and Authorization demo.


# Login Manager configuration 

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
             if (_userIdentity.UserName == FakeDBContext().UserName && _userIdentity.UserRole == FakeDBContext().UserRole &&                             FakeDBContext().Password == _userIdentity.Password)
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
        else if (_userIdentity.UserName == FakeDBContext().UserName && _userIdentity.UserRole == FakeDBContext().UserRole &&                             FakeDBContext().Password == _userIdentity.Password)
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

                    
                    
# startup File Configuration
  
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace AspDotNetCoreAuthentication
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<CookiePolicyOptions>(options =>
            {
               
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });


            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);


            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
            .AddCookie(Options =>
              {
                  Options.AccessDeniedPath = "/Home/Error";
                  Options.LoginPath = "/AspDotNetCoreSecurityManager/ClaimUserIdentity";
                  Options.LogoutPath = "/AspDotNetCoreSecurityManager/RemoveUserIdentity";
              }
              );

            services.AddAuthorization(Options =>
            {
                Options.AddPolicy("RequiredAdminAccess", _policy => _policy.RequireAuthenticatedUser().RequireRole("admin"));
            });

            services.AddAuthorization(Options =>
            {
                Options.AddPolicy("UserAccessible", _policy => _policy.RequireAuthenticatedUser().RequireRole("user"));
            });

        }

      
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error"); 
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCookiePolicy();
            app.UseAuthentication();
          

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}

        
  # controller Configuration
   
    [Authorize]
    public class HomeController : Controller
    {
        
        public IActionResult Index()
        {
            return View();
        }

        [Authorize(policy: "UserAccessible")]
        public IActionResult Index1()
        {
            return View();
        }

        [Authorize(policy: "RequiredAdminAccess")]
        public IActionResult Privacy()
        {
            return View();
        }
    }
    
    
  # Thank you.
