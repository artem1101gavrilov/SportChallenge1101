using FitnessTelegramBot.Models.Sport;

namespace FitnessTelegramBot.DataBase;

public class UnitOfWork
{
    private readonly ApplicationContext _context;
    private ActiveTrainingRepository _activeTrainingRepository = null!;
    private UserRepository _userRepository = null!;
    private RecordsOneTimeRepository _recordsOneTimeRepository = null!;
    private SportsExerciseRepository _sportsExerciseRepository = null!;

    public ActiveTrainingRepository ActiveTraining => _activeTrainingRepository ??= new ActiveTrainingRepository(_context);
    public UserRepository Users => _userRepository ??= new UserRepository(_context);
    public RecordsOneTimeRepository RecordsOneTime => _recordsOneTimeRepository ??= new RecordsOneTimeRepository(_context);
    public SportsExerciseRepository SportsExercises => _sportsExerciseRepository ??= new SportsExerciseRepository(_context);

    public UnitOfWork(ApplicationContext context)
    {
        _context = context;
    }

    public async Task SaveChangesAsync()
    {
        await _context.SaveChangesAsync();
    }
}