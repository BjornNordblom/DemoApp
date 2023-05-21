namespace DemoApp.Routes;

using AutoMapper;
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
            async (AppDbContext context, IMapper mapper, CreateUserRequest user) =>
            {
                if (await context.Users.AnyAsync(u => u.Email == user.Email))
                {
                    return Results.Conflict($"User with email {user.Email} already exists");
                }
                var newUser = User.Create(user.Name, user.Email);
                await context.Users.AddAsync(newUser);
                await context.SaveChangesAsync();
                return Results.Created($"/users/{newUser.Id}", mapper.Map<UserDto>(newUser));
            }
        );
        group.MapGet(
            "/",
            async (AppDbContext context, IMapper mapper) =>
            {
                var users = await context.Users
                    .Include(up => up.UserPosts)
                    .Select(
                        x =>
                            new UserResponseDto()
                            {
                                Id = x.Id,
                                Name = x.Name,
                                Email = x.Email,
                                CreatedAt = x.CreatedAt,
                                UpdatedAt = x.UpdatedAt,
                                Author = x.UserPosts
                                    .Where(up => up.Relation == UserPostRelation.Author)
                                    .Select(up => up.PostId),
                                Watcher = x.UserPosts
                                    .Where(up => up.Relation == UserPostRelation.Watcher)
                                    .Select(up => up.PostId),
                            }
                    )
                    .ToListAsync();
                return Results.Ok(users);
            }
        );
        group.MapGet(
            "/{id}",
            async (AppDbContext context, IMapper mapper, Guid id) =>
            {
                var user = await context.Users
                    .Include(u => u.Posts)
                    .FirstOrDefaultAsync(u => u.Id == id);

                if (user is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(mapper.Map<UserDto>(user));
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
                context.UserPosts.Add(UserPost.Create(user.Id, post.Id, UserPostRelation.Watcher));
                await context.SaveChangesAsync();
                return Results.Ok();
            }
        );

        group.MapDelete(
            "/{userId}",
            async (AppDbContext context, Guid userId) =>
            {
                var user = await context.Users.FindAsync(userId);
                if (user is null)
                {
                    return Results.NotFound($"User {userId} does not exist");
                }
                context.Users.Remove(user);
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

        return group;
    }
}
