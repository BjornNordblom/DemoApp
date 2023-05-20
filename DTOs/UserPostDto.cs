public class UserPostDto
{
    public Guid UserId { get; set; }
    public Guid PostId { get; set; }

    public UserPostRelationDto Relation { get; set; }
}
