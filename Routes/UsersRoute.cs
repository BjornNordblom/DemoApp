namespace DemoApp.Routes;

using DemoApp.Contracts;
using DemoApp.Entities;
using DemoApp.Persistence;
using Microsoft.EntityFrameworkCore;

public static partial class UsersRoute
{
    public static RouteGroupBuilder MapUsersApiRoutes(this RouteGroupBuilder group)
    {
        group.MapPost(
            "/",
            async (AppDbContext context, CreateUserRequest user) =>
            {
                if (await context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    return Results.Conflict($"User with email {user.Email} already exists");
                }
                var newUser = User.Create(user.Name, user.Email);
                await context.Users.AddAsync(newUser);
                await context.SaveChangesAsync();
                return Results.Created($"/users/{newUser.Id}", newUser);
            }
        );
        group.MapGet(
            "/",
            async (AppDbContext context) =>
            {
                var users = await context.Users.ToListAsync();
                return Results.Ok(users);
            }
        );
        group.MapGet(
            "/{id}",
            async (AppDbContext context, Guid id) =>
            {
                var user = await context.Users
                    .Include(u => u.UserAuthoredPosts)
                    .Include(u => u.UserWatchedPosts)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(user);
            }
        );
        // group.MapGet("/", GetAllTodos);
        // group.MapGet("/{id}", GetTodo);
        // group.MapPost("/", CreateTodo);
        // group.MapPut("/{id}", UpdateTodo);
        // group.MapDelete("/{id}", DeleteTodo);

        return group;
    }
}
