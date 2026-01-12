using System.ComponentModel.DataAnnotations;
using GranjaSystemProject.Models.Users;
using System.ComponentModel.DataAnnotations.Schema;

namespace GranjaSystemProject.Models.Farm;
public class Farm
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(100)]
    public string Name { get; set; }
    [Required]
    public int OwnerId { get; set; }
    [ForeignKey("OwnerId")]
    public User Owner { get; set; }
}
