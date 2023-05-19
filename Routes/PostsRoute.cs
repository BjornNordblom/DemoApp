namespace DemoApp.Routes;

using DemoApp.Contracts;
using DemoApp.Entities;
using DemoApp.Persistence;
using Microsoft.EntityFrameworkCore;

public static partial class PostsRouteExtension
{
    public static RouteGroupBuilder MapPostsApiRoutes(this RouteGroupBuilder group)
    {
        group.MapPost(
            "/",
            async (AppDbContext context, CreatePostRequest post) =>
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == post.UserId);
                if (user is null)
                {
                    return Results.NotFound();
                }
                var newPost = Post.Create(post.UserId, post.Title, post.Body);
                await context.Posts.AddAsync(newPost);
                await context.SaveChangesAsync();
                return Results.Created($"/posts/{newPost.Id}", newPost);
            }
        );
        group.MapGet(
            "/",
            async (AppDbContext context) =>
            {
                var posts = await context.Posts.ToListAsync();
                return Results.Ok(posts);
            }
        );
        group.MapGet(
            "/{id}",
            async (AppDbContext context, Guid id) =>
            {
                var post = await context.Posts
                    .Include(p => p.UserAuthoredPosts)
                    .Include(p => p.UserWatchedPosts)
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (post is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(post);
            }
        );
        group.MapPut(
            "/{postId}/addwatcher/{userId}",
            async (AppDbContext context, Guid postId, Guid userId) =>
            {
                var post = await context.Posts.FindAsync(postId);
                if (post is null)
                {
                    return Results.NotFound($"User {userId} does not exist");
                }
                var user = await context.Users.FindAsync(userId);
                if (user is null)
                {
                    return Results.NotFound($"Post {postId} does not exist");
                }
                if (
                    post.UserWatchedPosts.Any(
                        up =>
                            up.UserId == userId
                            && up.PostId == postId
                            && up.Relation == UserPostRelation.Watcher
                    )
                )
                {
                    return Results.Conflict($"User {userId} already watches post {postId}");
                }
                post.UserWatchedPosts.Append(
                    UserPost.Create(userId, postId, UserPostRelation.Watcher)
                );
                await context.SaveChangesAsync();
                return Results.Ok(post);
            }
        );

        return group;
    }
}
