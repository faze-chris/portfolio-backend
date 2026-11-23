using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PortfolioDb>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("PortfolioDb")));

builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddRoles<IdentityRole>()
    .AddEntityFrameworkStores<PortfolioDb>();

builder.Services.AddAuthentication();
builder.Services.AddAuthorizationBuilder()
    .AddPolicy("CanEdit", policy => policy.RequireRole( "Admin"));
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.MapOpenApi();
}
await DbInitializer.SeedAsync(app.Services);

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapIdentityApi<IdentityUser>();

app.MapProjectEndpoints();
app.MapBlogpostEndpoints();

app.Run();