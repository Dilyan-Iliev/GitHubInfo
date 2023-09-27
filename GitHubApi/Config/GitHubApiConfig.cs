namespace GitHubApi.Config
{
    using Octokit;

    public class GitHubApiConfig
    {
        public static GitHubClient GetInstance(IConfiguration config)
        {
            var productHeaderValue = new ProductHeaderValue("GitHubAPI");
            GitHubClient gitHubClient = new(productHeaderValue);
            var token = config.GetValue<string>("GitHub:AccessToken");
            var tokenAuth = new Credentials(token);
            gitHubClient.Credentials = tokenAuth;

            return gitHubClient;
        }
    }
}
