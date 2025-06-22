using System.ComponentModel.DataAnnotations;

namespace YCInterview.Models;

public class User
{
    public int Id { get; set; }

    [Required]
    [Display(Name = "使用者名稱")]
    public string Username { get; set; }

    [Required]
    [Display(Name = "密碼")]
    public byte[] Password { get; set; }
}
