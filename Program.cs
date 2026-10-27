using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PortfolioDb>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PortfolioDb")));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

// projects

// CREATE / POST 
app.MapPost("/api/projects", (PortfolioDb db, [FromBody] CreateProjectRequest request) =>
{
    var newProject = new Project
    {
        Title = request.Title,
        Description = request.Description,
        Category = request.Category,
        GitHubUrl = request.GitHubUrl,
        Date = request.Date,
        ImageUrl = request.ImageUrl
    };

    db.Projects.Add(newProject);
    db.SaveChanges();

    var response = new ProjectResponse
    {
        Id = newProject.Id,
        Title = newProject.Title,
        Description = newProject.Description,
        Category = newProject.Category,
        GitHubUrl = newProject.GitHubUrl,
        Date = newProject.Date,
        ImageUrl = newProject.ImageUrl
    };

    return Results.Created($"/api/projects/{response.Id}", response);
})
.WithName("CreateProject")
.WithTags("Projects");

// READ / GET 
app.MapGet("/api/projects", (PortfolioDb db) =>
{
    var projects = db.Projects.Select(p => new ProjectResponse
    {
        Id = p.Id,
        Title = p.Title,
        Description = p.Description,
        Category = p.Category,
        GitHubUrl = p.GitHubUrl,
        Date = p.Date,
        ImageUrl = p.ImageUrl
    }).ToList();

    return Results.Ok(projects);
})
.WithName("GetProjects")
.WithTags("Projects");

// UPDATE / PUT 
app.MapPut("/api/projects/{id}", (PortfolioDb db, int id, [FromBody] CreateProjectRequest request) =>
{
    var project = db.Projects.Find(id);
    if (project == null) return Results.NotFound();

    project.Title = request.Title;
    project.Description = request.Description;
    project.Category = request.Category;
    project.GitHubUrl = request.GitHubUrl;
    project.Date = request.Date;
    project.ImageUrl = request.ImageUrl;

    db.SaveChanges();
    return Results.NoContent();
})
.WithName("UpdateProject")
.WithTags("Projects");

// DELETE 
app.MapDelete("/api/projects/{id}", (PortfolioDb db, int id) =>
{
    var project = db.Projects.Find(id);
    if (project == null) return Results.NotFound();

    db.Projects.Remove(project);
    db.SaveChanges();
    return Results.NoContent();
})
.WithName("DeleteProject")
.WithTags("Projects");

//blog post

// CREATE / POST 
app.MapPost("/api/blogposts", (PortfolioDb db, [FromBody] CreateBlogpostRequest request) =>
{
    var newBlogpost = new Blogpost 
    { 
        Title = request.Title, 
        Content = request.Content, 
        Date = request.Date,
        ImageUrl = request.ImageUrl,
        LinkedInUrl = request.LinkedInUrl
    };
    
    db.Blogposts.Add(newBlogpost);
    db.SaveChanges();
    
    var response = new BlogpostResponse 
    { 
        Id = newBlogpost.Id, 
        Title = newBlogpost.Title, 
        Content = newBlogpost.Content, 
        Date = newBlogpost.Date,
        ImageUrl = newBlogpost.ImageUrl,
        LinkedInUrl = newBlogpost.LinkedInUrl
    };
    return Results.Created($"/api/blogposts/{response.Id}", response);
})
.WithName("CreateBlogpost")
.WithTags("Blogposts");

// READ / GET 
app.MapGet("/api/blogposts", (PortfolioDb db) =>
{
    var blogposts = db.Blogposts.Select(b => new BlogpostResponse
    {
        Id = b.Id,
        Title = b.Title,
        Content = b.Content,
        Date = b.Date,
        ImageUrl = b.ImageUrl,
        LinkedInUrl = b.LinkedInUrl
    }).ToList();
    return Results.Ok(blogposts);
})
.WithName("GetBlogposts")
.WithTags("Blogposts");

// UPDATE / PUT 
app.MapPut("/api/blogposts/{id}", (PortfolioDb db, int id, [FromBody] CreateBlogpostRequest request) =>
{
    var blogpost = db.Blogposts.Find(id);
    if (blogpost == null) return Results.NotFound();

    blogpost.Title = request.Title;
    blogpost.Content = request.Content;
    blogpost.Date = request.Date;
    blogpost.ImageUrl = request.ImageUrl;
    blogpost.LinkedInUrl = request.LinkedInUrl;

    db.SaveChanges();
    return Results.NoContent();
})
.WithName("UpdateBlogpost")
.WithTags("Blogposts");

// DELETE 
app.MapDelete("/api/blogposts/{id}", (PortfolioDb db, int id) =>
{
    var blogpost = db.Blogposts.Find(id);
    if (blogpost == null) return Results.NotFound();

    db.Blogposts.Remove(blogpost);
    db.SaveChanges();
    return Results.NoContent();
})
.WithName("DeleteBlogpost")
.WithTags("Blogposts");

app.Run();