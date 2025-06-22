using System.ComponentModel.DataAnnotations;

namespace YCInterview.Models;

public class TodoItem
{
    public int Id { get; set; }

    [Required]
    [StringLength(100)]
    [Display(Name = "事項名稱")]
    public string Title { get; set; }

    [Display(Name = "事項狀態")]
    public bool IsDone { get; set; }

    public int UserId { get; set; }
}
