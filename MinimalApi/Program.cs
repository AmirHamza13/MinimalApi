using Microsoft.EntityFrameworkCore;
using MinimalApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<ApplicationDbContext>(options =>
{
    // options.UseInMemoryDatabase("TestDB");
    options.UseSqlServer(builder.Configuration.GetConnectionString("MinimalDbConnection"));
});
var app = builder.Build();
app.MapGet("/", () =>
{
    return"Assalamu alaikum.";
});

app.MapPost("/api/post", (Post post) =>
{
    return Results.Ok(post);
});

app.MapPut("/api/put", () =>
{
    return Results.Ok("Call put action");
});

app.MapDelete("/api/delete", () =>
{
    return Results.Ok("Call delete action");
});

app.MapGet("/posts", (ApplicationDbContext db) =>
{
    var posts= db.Posts.ToList();
    return Results.Ok(posts);
});

app.MapPost("/posts", (Post post, ApplicationDbContext db) =>
{
    db.Posts.Add(post);
    bool isSaved = db.SaveChanges() > 0;
    if (isSaved)
    {
        return Results.Ok("Post has been saved.");
    }
    return Results.BadRequest("Post saved failed.");
});

app.MapPut("/posts",(int id,Post post,ApplicationDbContext db)=>
{
    var data=db.Posts.FirstOrDefault(c=>c.Id==id);
    if(data==null)
    {
        return Results.NotFound();
    }
    if(data.Id!=post.Id)
    {
        return Results.BadRequest("Id is not valid.");
    }
    data.Title  = post.Title;
    data.Description = post.Description;
    bool isUpdated = db.SaveChanges() > 0;
    if (isUpdated)
    {
        return Results.Ok("Post has been modified.");
    }
    return Results.BadRequest("Post modified failed.");
});

app.MapDelete("/posts",(int id,ApplicationDbContext db) =>
{
    var post =db.Posts.FirstOrDefault(c=>c.Id==id);
    if (post==null) 
    {
        return Results.NotFound();
    }
    db.Posts.Remove(post);
    if(db.SaveChanges() > 0)
    {
        return Results.Ok("Post has deleted.");
    }
    return Results.BadRequest("post delete failed.");
});
app.Run();