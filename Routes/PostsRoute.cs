using AutoMapper;
using DemoApp.Contracts;
using DemoApp.Entities;
using DemoApp.Persistence;
using Microsoft.EntityFrameworkCore;

namespace DemoApp.Routes;

public static partial class PostsRouteExtension
{
    public static RouteGroupBuilder MapPostsApiRoutes(this RouteGroupBuilder group)
    {
        group.MapPost(
            "/",
            async (AppDbContext context, IMapper mapper, CreatePostRequest post) =>
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Id == post.UserId);
                if (user is null)
                {
                    return Results.NotFound();
                }
                var newPost = Post.Create(post.Title, post.Body);
                //var newUserPost = UserPost.Create(user.Id, newPost.Id, UserPostRelation.Author);
                newPost.Users.Add(user);
                await context.Posts.AddAsync(newPost);
                //await context.UserPosts.AddAsync(newUserPost);
                await context.SaveChangesAsync();
                return Results.Created($"/posts/{newPost.Id}", mapper.Map<PostDto>(newPost));
            }
        );
        group.MapGet(
            "/",
            async (AppDbContext context, IMapper mapper) =>
            {
                var posts = await context.Posts.Include(p => p.Users).ToListAsync();
                //var posts = await context.Posts.ToListAsync();
                var postDtos = mapper.Map<List<PostDto>>(posts);
                return Results.Ok(postDtos);
            }
        );
        group.MapGet(
            "/{id}",
            async (AppDbContext context, IMapper mapper, Guid id) =>
            {
                var post = await context.Posts
                    .Include(p => p.Users)
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (post is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(mapper.Map<PostDto>(post));
            }
        );
        group.MapPut(
            "/{postId}/addwatcher/{userId}",
            async (AppDbContext context, IMapper mapper, Guid postId, Guid userId) =>
            {
                var post = await context.Posts
                    .Include(p => p.UserPosts)
                    .Include(p => p.Users)
                    .FirstOrDefaultAsync(p => p.Id == postId);
                if (post is null)
                {
                    return Results.NotFound($"Post {postId} does not exist");
                }
                var user = await context.Users.FindAsync(userId);
                if (user is null)
                {
                    return Results.NotFound($"User {postId} does not exist");
                }
                context.UserPosts.Add(UserPost.Create(user.Id, post.Id, UserPostRelation.Watcher));
                await context.SaveChangesAsync();
                return Results.Ok(mapper.Map<PostDto>(post));
            }
        );

        return group;
    }
}
