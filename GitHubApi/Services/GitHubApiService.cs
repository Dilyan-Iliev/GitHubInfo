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
                var repos = await gitHubClient.Repository.GetAllForCurrent();
                return repos.ToArray();
            }
            catch (ApiException)
            {
                throw; // Rethrow the exception or return an error response
            }
        }

        public async Task<IEnumerable<RepoViewModel>> GetReposForView(IEnumerable<Repository> repos)
        {
            var user = await this.GetCurrentUser();
            var avatarUrl = user.AvatarUrl;

            return repos.Select(r => new RepoViewModel
            {
               Id = r.Id,
               Name = r.Name,
               OwnerName = r.Owner.Login,
               CreatedAt = r.CreatedAt.ToString("yyyy-MM-dd"),
               PublicAddress = r.HtmlUrl,
               AvatarUrl = avatarUrl,
               IsRepoPrivate = r.Private
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
