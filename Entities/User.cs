using System.Text.Json.Serialization;

namespace DemoApp.Entities;

public class User : IAuditable
{
    public Guid Id { get; private set; }
    public string Name { get; private set; } = null!;
    public string Email { get; private set; } = null!;

    [JsonIgnore]
    public ICollection<Post> Posts { get; private set; } = null!;

    [JsonIgnore]
    public List<UserPost> UserPosts { get; private set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    protected User(string name, string email)
    {
        Id = Guid.NewGuid();
        Name = name;
        Email = email;
    }

    public User Update(string name, string email)
    {
        Name = name;
        Email = email;
        return this;
    }

    public static User Create(string name, string email)
    {
        return new User(name, email);
    }
}
