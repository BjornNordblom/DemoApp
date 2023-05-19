namespace DemoApp.Contracts;

public record CreatePostRequest(Guid UserId, string Title, string Body);
