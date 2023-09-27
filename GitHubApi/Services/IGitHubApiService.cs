namespace GitHubApi.Services
{
    using GitHubApi.Models;
    using Octokit;

    public interface IGitHubApiService
    {
        Task<IEnumerable<Repository>> GetReposForUser(string username);
        Task<User> GetCurrentUser();
        Task<Repository> RepositoryDetails(string owner, string repoName);
        Task<IEnumerable<RepoViewModel>> GetReposForView(IEnumerable<Repository> repos);
    }
}
