using System;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Movie.Client.ApiServices;
using Movie.Client.HttpHandler;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http.Headers;
using Microsoft.Net.Http.Headers;
using IdentityModel.Client;
using Microsoft.AspNetCore.Authentication;

namespace Movie.Client
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllersWithViews();
            services.AddScoped<IMovieService, MovieService>();
            services.AddAuthentication(opt =>
            {
                opt.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                opt.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
            })
                .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddOpenIdConnect(OpenIdConnectDefaults.AuthenticationScheme, opt =>
                {
                    opt.Authority = "https://localhost:5005";

                    opt.ClientId = "movies_mvc_client";
                    opt.ResponseType = OpenIdConnectResponseType.Code;
                    opt.ClientSecret = "secrets";

                    opt.Scope.Add("openid");
                    opt.Scope.Add("profile");
                    opt.Scope.Add("roles");
                    opt.ClaimActions.MapJsonKey("role","role");
                    opt.SaveTokens = true;
                    opt.GetClaimsFromUserInfoEndpoint = true;
                    opt.Events = new OpenIdConnectEvents
                    {
                        OnRemoteFailure = (context) =>
                        {
                            if (context.Failure.Message.Contains("access_denied"))
                            {
                                // do something
                            }
                            context.Response.Redirect("/");
                            context.HandleResponse();

                            return Task.CompletedTask;
                        },
                        OnAccessDenied = (context) =>
                        {
                            context.Response.Redirect("Home/Error");
                            context.HandleResponse();
                            return Task.CompletedTask;
                        },
                        OnMessageReceived = (context) =>
                        {
                            return Task.CompletedTask;
                        }
                        //do something
                    };
                });
            services.ConfigureApplicationCookie(options =>
            {
                options.AccessDeniedPath = "Account/AccessDenied";
            });
            services.AddHttpContextAccessor();
            services.AddTransient<AuthenticationDelegatingHandler>();
            services.AddHttpClient("MovieAPIClient", options =>
            {
                options.BaseAddress = new Uri("https://localhost:5001");
                options.DefaultRequestHeaders.Clear();
                options.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            }).AddHttpMessageHandler<AuthenticationDelegatingHandler>();

            // create http client to access IDP
            services.AddHttpClient("IDPClient", options =>
            {
                options.BaseAddress = new Uri("https://localhost:5005");
                options.DefaultRequestHeaders.Clear();
                options.DefaultRequestHeaders.Add(HeaderNames.Accept, "application/json");
            });

            services.AddSingleton(new ClientCredentialsTokenRequest()
            {
                Address = "https://localhost:5005/connect/token",
                ClientId = "MoviesAPI",
                ClientSecret = "MoviesAPI_secrets",
                Scope = "MoviesAPI"
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });
        }
    }
}
