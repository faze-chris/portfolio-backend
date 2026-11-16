using Microsoft.AspNetCore.Mvc;

public static class BlogpostEndpoints
{
    public static void MapBlogpostEndpoints(this WebApplication app)
    {
        // map group for blogposts with path /api/blogposts
        var group = app.MapGroup("/api/blogposts").WithTags("Blogposts");

        // CREATE / POST 
        group.MapPost("/", (PortfolioDb db, [FromBody] CreateBlogpostRequest request) =>
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
        }).WithName("CreateBlogpost");

        // READ / GET 
        group.MapGet("/", (PortfolioDb db) =>
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
        }).WithName("GetBlogposts");

        // UPDATE / PUT 
        group.MapPut("/{id}", (PortfolioDb db, int id, [FromBody] CreateBlogpostRequest request) =>
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
        }).WithName("UpdateBlogpost");

        // DELETE 
        group.MapDelete("/{id}", (PortfolioDb db, int id) =>
        {
            var blogpost = db.Blogposts.Find(id);
            if (blogpost == null) return Results.NotFound();

            db.Blogposts.Remove(blogpost);
            db.SaveChanges();
            return Results.NoContent();
        }).WithName("DeleteBlogpost");
    }
}