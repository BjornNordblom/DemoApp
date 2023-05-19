namespace DemoApp.Entities;

public class Post
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;

    public IReadOnlyCollection<UserPost> UserAuthoredPosts { get; private set; }
    public IReadOnlyCollection<UserPost> UserWatchedPosts { get; private set; }

    protected Post(string title, string body)
    {
        Id = Guid.NewGuid();
        Title = title;
        Body = body;
        UserAuthoredPosts = new List<UserPost>();
        UserWatchedPosts = new List<UserPost>();
        //UserAuthoredPosts.Append(UserPost.Create(userId, Id));
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
