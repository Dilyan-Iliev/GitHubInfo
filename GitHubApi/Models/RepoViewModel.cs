namespace GitHubApi.Models
{
    public class RepoViewModel
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string OwnerName { get; set; } = null!;
        public string CreatedAt { get; set; } = null!;
        public string PublicAddress { get; set; } = null!;
    }
}
