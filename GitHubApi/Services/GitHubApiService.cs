namespace GitHubApi.Services
{
    using GitHubApi.Models;
    using Microsoft.AspNetCore.Mvc;
    using Octokit;

    public class GitHubApiService
        : IGitHubApiService
    {
        private readonly GitHubClient gitHubClient;

        public GitHubApiService(GitHubClient gitHubClient)
        {
            this.gitHubClient = gitHubClient;
        }

        public async Task<User> GetCurrentUser()
        {
            try
            {
                var currentUser = await gitHubClient.User.Current();
                return currentUser;
            }
            catch (ApiException)
            {

                throw;
            }
        }

        public async Task<IEnumerable<Repository>> GetReposForUser(string username)
        {
            try
            {
                var repos = await gitHubClient.Repository.GetAllForUser(username);
                return repos.ToArray();
            }
            catch (ApiException)
            {
                throw; // Rethrow the exception or return an error response
            }
        }

        public IEnumerable<RepoViewModel> GetReposForView(IEnumerable<Repository> repos)
        {
            return repos.Select(r => new RepoViewModel
            {
               Id = r.Id,
               Name = r.Name,
               OwnerName = r.Owner.Login,
               CreatedAt = r.CreatedAt.ToString("yyyy-MM-dd"),
               PublicAddress = r.Url
            })
            .ToList();
        }

        public async Task<Repository> RepositoryDetails(string owner, string repoName)
        {
            Repository repository = await this.gitHubClient.Repository.Get(owner, repoName);
            return repository;
        }
    }
}
