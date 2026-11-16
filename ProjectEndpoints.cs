using Microsoft.AspNetCore.Mvc;

public static class ProjectEndpoints
{
    public static void MapProjectEndpoints(this WebApplication app)
    {
        // map group for projects with path /api/projects
        var group = app.MapGroup("/api/projects").WithTags("Projects");

        // CREATE / POST 
        group.MapPost("/", (PortfolioDb db, [FromBody] CreateProjectRequest request) =>
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
        }).WithName("CreateProject");

        // READ / GET 
        group.MapGet("/", (PortfolioDb db) =>
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
        }).WithName("GetProjects");

        // UPDATE / PUT 
        group.MapPut("/{id}", (PortfolioDb db, int id, [FromBody] CreateProjectRequest request) =>
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
        }).WithName("UpdateProject");

        // DELETE 
        group.MapDelete("/{id}", (PortfolioDb db, int id) =>
        {
            var project = db.Projects.Find(id);
            if (project == null) return Results.NotFound();

            db.Projects.Remove(project);
            db.SaveChanges();
            return Results.NoContent();
        }).WithName("DeleteProject");
    }
}