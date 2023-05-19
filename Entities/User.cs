namespace DemoApp.Entities;

public class User : IAuditable
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;
    public IReadOnlyCollection<UserPost> UserAuthoredPosts { get; private set; }
    public IReadOnlyCollection<UserPost> UserWatchedPosts { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    protected User()
    {
        UserAuthoredPosts = new List<UserPost>();
        UserWatchedPosts = new List<UserPost>();
    }

    public User Update(string name, string email)
    {
        Name = name;
        Email = email;
        return this;
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
