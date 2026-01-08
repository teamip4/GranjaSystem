using System.ComponentModel.DataAnnotations;

namespace GranjaSystemProject.Models.Farm;
public class Race
{
    [Key]
    public int Id { get; set; }
    [Required, MaxLength(50)]
    public string Name { get; set; }
}
