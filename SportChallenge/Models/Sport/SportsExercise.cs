using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace FitnessTelegramBot.Models.Sport;

/// <summary>
/// Спортивное упражнение
/// </summary>
public class SportsExercise
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }
    public long UserId { get; set; }
    public DateTime Date { get; set; }
    public SportType SportType { get; set; }
    public int Count { get; set; }
}