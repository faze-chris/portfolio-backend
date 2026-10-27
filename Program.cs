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
        Date = p.Date
    }).ToList();

    return Results.Ok(projects);
})
.WithName("GetProjects")
.WithTags("Projects");

// CREATE / POST
app.MapPost("/api/projects", (PortfolioDb db, [FromBody] CreateProjectRequest request) =>
{
    var newProject = new Project
    {
        Title = request.Title,
        Description = request.Description,
        Category = request.Category,
        GitHubUrl = request.GitHubUrl,
        Date = request.Date
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
        Date = newProject.Date
    };

    return Results.Created($"/api/projects/{response.Id}", response);
})
.WithName("CreateProject")
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

app.Run();