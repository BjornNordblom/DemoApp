using Newtonsoft.Json;

namespace DemoApp.Entities;

public class Post : IAuditable
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public List<User> Users { get; private set; } = new();
    public List<UserPost> UserPosts { get; private set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    protected Post(string title, string body)
    {
        Id = Guid.NewGuid();
        Title = title;
        Body = body;
    }

    public static Post Create(string title, string body)
    {
        var newPost = new Post(title, body);
        return newPost;
    }
}
