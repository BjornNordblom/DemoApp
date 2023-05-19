namespace DemoApp.Contracts;

public record UpdateUserRequest(Guid Id, string Name, string Email);
