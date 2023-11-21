using SportChallenge.Models.Sport;
using SportChallenge.Models.User;
using Microsoft.EntityFrameworkCore;

namespace SportChallenge.DataBase;

//dotnet ef migrations add NameMigration

public class ApplicationContext : DbContext
{
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<SportsExercise> SportsExercises { get; set; } = null!;
    public DbSet<RecordsOneTime> RecordsOneTime { get; set; } = null!;
    public DbSet<ActiveTraining> ActiveTraining { get; set; } = null!;


    public ApplicationContext()
    {
    }

    public ApplicationContext(DbContextOptions<ApplicationContext> options)
            : base(options)
    {
    }
}