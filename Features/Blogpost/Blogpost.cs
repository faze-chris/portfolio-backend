using System.ComponentModel.DataAnnotations;

public class Blogpost
{
    public int Id { get; set; }
    
    [Required]
    public string Title { get; set; } = string.Empty;
    
    [Required]
    public string Content { get; set; } = string.Empty;
    
    public DateTime Date { get; set; }
    
    public string ImageUrl { get; set; } = string.Empty; 
    public string LinkedInUrl { get; set; } = string.Empty; 
}