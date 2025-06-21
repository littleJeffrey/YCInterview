using System.ComponentModel.DataAnnotations;

namespace YCInterview.Models;

public class TodoItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    public string Title { get; set; }

    public bool IsDone { get; set; }
}
