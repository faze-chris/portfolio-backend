using System.ComponentModel.DataAnnotations;

public class CreateProjectRequest
{
    [Required]
    [StringLength(100, MinimumLength = 2)]
    public string Titel { get; set; } = string.Empty;
    
    [Required]
    public string Beschrijving { get; set; } = string.Empty;
    
    [Required]
    public string Categorie { get; set; } = string.Empty;
    
    public string GitHubUrl { get; set; } = string.Empty;
    
    [Required]
    public DateTime Datum { get; set; }
}