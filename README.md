# AspDotNetCore-Authentication-Authorization
 AspDotNetCore-Authentication-Authorization Demo

This application is an example of  Authentication and Authorization in Asp.Net Core  using idenity.

Secure your Dot.Net Core Application using Asp.Net Core  Authentication and Authorization demo.


# Login Manager configuration 
                  Login Class
                  
                   var UserIdentity = new ClaimsIdentity(new[]
                    {
                       new Claim(ClaimTypes.Name,_userIdentity.UserName),
                       new Claim(ClaimTypes.Role ,_userIdentity.UserRole)
                    }, CookieAuthenticationDefaults.AuthenticationScheme);

                    var UserPrinciple = new ClaimsPrincipal(UserIdentity);

                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, UserPrinciple);
                    
                    
  # startup File Configuration
  
    public void ConfigureServices(IServiceCollection services)
    {
            services.Configure<CookiePolicyOptions>(options =>
            {
                
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            //Registering Employee service in dependency container here.

            services.AddSingleton<IEmployeeService, Employeeservice>();
            //Registering session
            services.AddSession();

            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(Options => 
                 { 
                     Options.AccessDeniedPath = "/Employee/EmployeeBoard";
                     Options.LoginPath = "/AspDotNetCoreSecurityManager/ClaimUserIdentity";
                     Options.LogoutPath= "/AspDotNetCoreSecurityManager/ClaimUserIdentity";
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
            //using session
            app.UseSession();


            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Employee}/{action=Employeeboard}/{id?}");
            });
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
