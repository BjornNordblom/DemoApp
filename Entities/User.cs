namespace DemoApp.Entities;

public class User
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;

    public IReadOnlyCollection<UserPost> UserAuthoredPosts { get; private set; }
    public IReadOnlyCollection<UserPost> UserWatchedPosts { get; private set; }

    protected User()
    {
        UserAuthoredPosts = new List<UserPost>();
        UserWatchedPosts = new List<UserPost>();
    }

    public static User Create(string name, string email)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Name = name,
            Email = email
        };
    }
}
