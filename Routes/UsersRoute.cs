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

        group.MapPut(
            "/{userId}/addwatchedpost/{postId}",
            async (AppDbContext context, Guid userId, Guid postId) =>
            {
                var user = await context.Users.FindAsync(userId);
                if (user is null)
                {
                    return Results.NotFound($"User {userId} does not exist");
                }
                var post = await context.Posts.FindAsync(postId);
                if (post is null)
                {
                    return Results.NotFound($"Post {postId} does not exist");
                }
                if (
                    user.UserWatchedPosts.Any(
                        up =>
                            up.UserId == userId
                            && up.PostId == postId
                            && up.Relation == UserPostRelation.Watcher
                    )
                )
                {
                    return Results.Conflict($"User {userId} already watches post {postId}");
                }
                user.UserWatchedPosts.Append(
                    UserPost.Create(userId, postId, UserPostRelation.Watcher)
                );
                await context.SaveChangesAsync();
                return Results.Ok();
            }
        );

        group.MapPut(
            "/",
            async (AppDbContext context, UpdateUserRequest user) =>
            {
                var userToUpdate = await context.Users.FindAsync(user.Id);
                if (userToUpdate is null)
                {
                    return Results.NotFound($"User {user.Id} does not exist");
                }
                userToUpdate.Update(user.Name, user.Email);
                await context.SaveChangesAsync();
                return Results.Ok();
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
