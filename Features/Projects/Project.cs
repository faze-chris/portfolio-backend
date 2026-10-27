using System.ComponentModel.DataAnnotations;

public class Project
{
    public int Id { get; set; } 
    
    [Required]
    public string Titel { get; set; } = string.Empty;
    
    [Required]
    public string Beschrijving { get; set; } = string.Empty;
    
    [Required]
    public string Categorie { get; set; } = string.Empty;
    
    public string GitHubUrl { get; set; } = string.Empty;
    
    public DateTime Datum { get; set; }
}