public class PostResponseDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = null!;
    public string Body { get; set; } = null!;
    public IEnumerable<Guid> Authors { get; set; } = null!;
    public IEnumerable<Guid> Watchers { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public DateTime? DeletedAt { get; set; }
}
