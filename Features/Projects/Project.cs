using System.ComponentModel.DataAnnotations;

public class Project
{
    public int Id { get; set; }

    [Required]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    public string GitHubUrl { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string ImageUrl { get; set; } = string.Empty;
}