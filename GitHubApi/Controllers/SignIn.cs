namespace GitHubApi.Controllers
{
    using Microsoft.AspNetCore.Mvc;

    public class SignIn : Controller
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public SignIn(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        public async Task<IActionResult> GitHub(string code, string state)
        {
            try
            {
                var client = _httpClientFactory.CreateClient("GitHubClient");

                // Configure the HTTP client to include the client ID, client secret, and other required parameters.
                var clientId = "9db96f05c486cc6885c9";
                var clientSecret = "d13fdfa7dd9c2995c363f68101f6de3e9d143d39";
                var redirectUri = "https://localhost:7137/SignIn/GitHub";
                var content = new FormUrlEncodedContent(new[]
                {
                    new KeyValuePair<string, string>("code", code),
                    new KeyValuePair<string, string>("client_id", clientId),
                    new KeyValuePair<string, string>("client_secret", clientSecret),
                    new KeyValuePair<string, string>("redirect_uri", redirectUri)
                });

                var tokenResponse = await client.PostAsync("https://github.com/login/oauth/access_token", content);
                tokenResponse.EnsureSuccessStatusCode();
                var responseContent = await tokenResponse.Content.ReadAsStringAsync();

                // Parse the response content to extract the access token (and other information if needed).
                var accessToken = responseContent.Split('&')[0].Split('=')[1];

                // Store the access token securely, e.g., in a database, and proceed with user authentication.

                return RedirectToAction("LoginSuccess");
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
