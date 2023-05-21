public class UserResponseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string Email { get; set; } = null!;
    public IEnumerable<Guid> Author { get; set; } = null!;
    public IEnumerable<Guid> Watcher { get; set; } = null!;
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
}
