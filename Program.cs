using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/projects", () =>
{
    var fakeProjects = new List<ProjectResponse>
    {
        new ProjectResponse 
        { 
            Id = 1, 
            Titel = "Noughts and Crosses", 
            Beschrijving = "Boter, kaas en eieren game", 
            Categorie = "Game",
            Datum = new DateTime(2026, 9, 15) // Vul hier een datum in
        },
        new ProjectResponse 
        { 
            Id = 2, 
            Titel = "Weather App", 
            Beschrijving = "Een weer-applicatie gekoppeld aan een API", 
            Categorie = "App",
            Datum = new DateTime(2026, 9, 30) // Vul hier een datum in
        }
    };
    return Results.Ok(fakeProjects);
})
.WithName("GetProjects")
.WithTags("Projects");


app.MapPost("/api/projects", ([FromBody] CreateProjectRequest request) =>
{
    
    var createdProject = new ProjectResponse 
    { 
        Id = 3, 
        Titel = request.Titel,
        Beschrijving = request.Beschrijving,
        Categorie = request.Categorie,
        GitHubUrl = request.GitHubUrl,
        Datum = request.Datum
    };
    
    return Results.Created($"/api/projects/{createdProject.Id}", createdProject);
})
.WithName("CreateProject")
.WithTags("Projects");


app.Run();