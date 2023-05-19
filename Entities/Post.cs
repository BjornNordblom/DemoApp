namespace DemoApp.Entities;

public class Post : IAuditable
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public IReadOnlyCollection<UserPost> UserAuthoredPosts { get; private set; }
    public IReadOnlyCollection<UserPost> UserWatchedPosts { get; private set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }

    protected Post(string title, string body)
    {
        Id = Guid.NewGuid();
        Title = title;
        Body = body;
        UserAuthoredPosts = new List<UserPost>();
        UserWatchedPosts = new List<UserPost>();
    }

    public static Post Create(Guid userId, string title, string body)
    {
        var newPost = new Post(title, body);
        newPost.UserAuthoredPosts.Append(
            UserPost.Create(userId, newPost.Id, UserPostRelation.Author)
        );
        return newPost;
    }
}
