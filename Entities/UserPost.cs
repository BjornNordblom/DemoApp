namespace DemoApp.Entities;

public enum UserPostRelation
{
    Author,
    Watcher
}

public class UserPost
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }

    public User User { get; set; }
    public Post Post { get; set; }

    public UserPostRelation Relation { get; set; }

    protected UserPost() { }

    public static UserPost Create(Guid userId, Guid postId, UserPostRelation relation)
    {
        return new UserPost()
        {
            UserId = userId,
            PostId = postId,
            Relation = relation
        };
    }
}
