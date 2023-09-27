namespace GitHubApi.Controllers
{
    using GitHubApi.Models;
    using GitHubApi.Services;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Octokit;

    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IGitHubApiService gitHubApiService;
        private readonly IAuthenticationSchemeProvider _authenticationSchemeProvider;


        public HomeController(ILogger<HomeController> logger,
            IGitHubApiService gitHubApiService,
            IAuthenticationSchemeProvider authenticationSchemeProvider)
        {
            _logger = logger;
            this.gitHubApiService = gitHubApiService;
            _authenticationSchemeProvider = authenticationSchemeProvider;
        }

        [Authorize(Policy = "GitHubAuth")]
        public async Task<IActionResult> Index()
        {
            User currentUser = await this.gitHubApiService.GetCurrentUser();
            string username = currentUser.Login;

            IEnumerable<Repository> repos =
                await this.gitHubApiService.GetReposForUser(username);

            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var userClaims = User.Claims.Select(x => new { x.Type, x.Value }).ToList();

            var result = this.gitHubApiService.GetReposForView(repos);

            return View(result);
        }

        public IActionResult Login()
        {
            var schemes = _authenticationSchemeProvider.GetAllSchemesAsync().Result;
            var githubScheme = schemes.FirstOrDefault(x => x.Name == "github");

            if (githubScheme != null)
            {
                return Challenge(new AuthenticationProperties
                { RedirectUri = "/oauth/github-cb" },
                githubScheme.Name);
            }

            return BadRequest();
        }

        public async Task<IActionResult> Details(string owner, string repoName)
        {
            // Call your service method to retrieve repository details
            Repository repository = await this.gitHubApiService.RepositoryDetails(owner, repoName);

            if (repository != null)
            {
                return View(repository); // Pass the repository details to a view
            }

            // Handle the case where the repository doesn't exist or there's an error
            return View("RepositoryNotFound");
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = System.Diagnostics.Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}