using System.ComponentModel.DataAnnotations;

public class CreateProjectRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Title { get; set; } = string.Empty;

    [Required]
    public string Description { get; set; } = string.Empty;

    [Required]
    public string Category { get; set; } = string.Empty;

    public string GitHubUrl { get; set; } = string.Empty;

    [Required]
    public DateTime Date { get; set; }
}