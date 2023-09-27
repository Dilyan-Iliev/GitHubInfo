namespace GitHubApi
{
    using GitHubApi.Config;
    using GitHubApi.Services;
    using Microsoft.AspNetCore.Authentication;
    using Octokit;
    using System.Net.Http.Headers;
    using System.Security.Claims;
    using System.Text.Json;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var config = builder.Configuration;

            builder.Services.AddAuthentication("cookie")
            .AddCookie("cookie")
            .AddOAuth("github", o =>
            {
                o.SignInScheme = "cookie";

                o.ClientId = config.GetValue<string>("GitHub:ClientId");
                o.ClientSecret = config.GetValue<string>("GitHub:ClientSecret");
                o.CallbackPath = "/oauth/github-cb";

                o.SaveTokens = true;

                o.AuthorizationEndpoint = "https://github.com/login/oauth/authorize";
                o.TokenEndpoint = "https://github.com/login/oauth/access_token";
                o.UserInformationEndpoint = "https://api.github.com/user";

                o.ClaimActions.MapJsonKey("sub", "id");
                o.ClaimActions.MapJsonKey(ClaimTypes.Name, "login");

                o.Events.OnCreatingTicket = async ctx =>
                {
                    //ctx.HttpContext.RequestServices.
                    using var request =
                        new HttpRequestMessage(HttpMethod.Get, ctx.Options.UserInformationEndpoint);
                    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", ctx.AccessToken);

                    using var result = await ctx.Backchannel.SendAsync(request);
                    var user = await result?.Content?.ReadFromJsonAsync<JsonElement>();

                    ctx.RunClaimActions(user);
                };
            });

            builder.Services.ConfigureApplicationCookie(cfg =>
            {
                cfg.LoginPath = "/Home/Login";
            });

            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("GitHubAuth", policy =>
                {
                    policy.AuthenticationSchemes.Add("github");
                    policy.RequireAuthenticatedUser();
                });
            });

            builder.Services.AddControllersWithViews();

            GitHubClient gitHubClient = GitHubApiConfig.GetInstance(config);
            builder.Services.AddSingleton(gitHubClient);
            builder.Services.AddSingleton(typeof(GitHubApiConfig));
            builder.Services.AddScoped<IGitHubApiService, GitHubApiService>();

            var app = builder.Build();

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            //app.Map("/", (HttpContext ctx) =>
            //{
            //    ctx.GetTokenAsync("access_token");
            //    return ctx.User.Claims.Select(x => new { x.Type, x.Value }).ToList();
            //});

            //app.Map("/login", (HttpContext ctx) =>
            //{
            //    return Results.Challenge(new Microsoft.AspNetCore.Authentication.AuthenticationProperties()
            //    {
            //        RedirectUri = "https://localhost:7137/"
            //    } ,authenticationSchemes: new List<string>() { "github" });
            //});

            app.Run();
        }
    }
}