using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using yildiz.entities.Abstract;

namespace yildiz.entities.Concrete;

public class PasswordResetToken : IEntity
{
    [Key]
    public int PasswordResetTokenId { get; set; }

    public int UserId { get; set; }

    [Required]
    public string TokenHash { get; set; } = string.Empty;

    public DateTime ExpiresAt { get; set; }

    public bool IsUsed { get; set; }

    [ForeignKey(nameof(UserId))]
    public AppUser? User { get; set; }
}