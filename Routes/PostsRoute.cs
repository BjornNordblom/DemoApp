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
                var posts = await context.Posts
                    .Include(up => up.UserPosts)
                    .Select(
                        x =>
                            new PostResponseDto()
                            {
                                Id = x.Id,
                                Title = x.Title,
                                Body = x.Body,
                                Authors = x.UserPosts
                                    .Where(up => up.Relation == UserPostRelation.Author)
                                    .Select(up => up.UserId),
                                Watchers = x.UserPosts
                                    .Where(up => up.Relation == UserPostRelation.Watcher)
                                    .Select(up => up.UserId)
                            }
                    )
                    .ToListAsync();
                //var posts = await context.Posts.ToListAsync();
                // var postDtos = mapper.Map<List<PostDto>>(posts);
                return Results.Ok(posts);
            }
        );
        group.MapGet(
            "/{id}",
            async (AppDbContext context, IMapper mapper, Guid id) =>
            {
                var post = await context.Posts
                    .Include(up => up.UserPosts)
                    .Select(
                        x =>
                            new PostResponseDto()
                            {
                                Id = x.Id,
                                Title = x.Title,
                                Body = x.Body,
                                Authors = x.UserPosts
                                    .Where(up => up.Relation == UserPostRelation.Author)
                                    .Select(up => up.UserId),
                                Watchers = x.UserPosts
                                    .Where(up => up.Relation == UserPostRelation.Watcher)
                                    .Select(up => up.UserId)
                            }
                    )
                    .FirstOrDefaultAsync(p => p.Id == id);
                if (post is null)
                {
                    return Results.NotFound();
                }
                return Results.Ok(post);
            }
        );
        group.MapPatch(
            "/{postId}/addwatcher/{userId}",
            async (AppDbContext context, IMapper mapper, Guid postId, Guid userId) =>
            {
                var post = await context.Posts
                    .Include(up => up.UserPosts)
                    .Select(
                        x =>
                            new PostResponseDto()
                            {
                                Id = x.Id,
                                Title = x.Title,
                                Body = x.Body,
                                Authors = x.UserPosts
                                    .Where(up => up.Relation == UserPostRelation.Author)
                                    .Select(up => up.UserId),
                                Watchers = x.UserPosts
                                    .Where(up => up.Relation == UserPostRelation.Watcher)
                                    .Select(up => up.UserId)
                            }
                    )
                    .FirstOrDefaultAsync(p => p.Id == postId);
                if (post is null)
                {
                    return Results.NotFound();
                }
                var user = await context.Users.FindAsync(userId);
                if (user is null)
                {
                    return Results.NotFound($"User {userId} does not exist");
                }
                if (
                    context.UserPosts.Any(
                        up =>
                            up.PostId == postId
                            && up.UserId == userId
                            && up.Relation == UserPostRelation.Watcher
                    )
                )
                {
                    return Results.BadRequest($"User {userId} is already watching post {postId}");
                }
                context.UserPosts.Add(UserPost.Create(user.Id, post.Id, UserPostRelation.Watcher));
                post.Watchers.Append(user.Id);
                await context.SaveChangesAsync();
                return Results.Ok(post);
            }
        );

        return group;
    }
}
