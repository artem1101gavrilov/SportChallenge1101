using SportChallenge.Models.Sport;
using System.Linq.Expressions;

namespace SportChallenge.DataBase;

public class SportsExerciseRepository : IRepository<SportsExercise>
{
    private ApplicationContext _context;

    public SportsExerciseRepository(ApplicationContext context)
    {
        _context = context;
    }

    public void Create(SportsExercise item)
    {
        throw new NotImplementedException();
    }

    public SportsExercise Get(Expression<Func<SportsExercise, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<SportsExercise> GetAll()
        => _context.SportsExercises;

    public IEnumerable<SportsExercise> GetCollection(Expression<Func<SportsExercise, bool>> filter)
    {
        IQueryable<SportsExercise> query = _context.SportsExercises;

        if (filter != null)
        {
            return query.Where(filter);
        }
        return Enumerable.Empty<SportsExercise>();
    }

    public bool TryUpdate(long id, SportType sportType, int count)
    {
        var sportsExercise = _context.SportsExercises.FirstOrDefault(_ => _.UserId == id
                                            && _.SportType == sportType
                                            && _.Date.Date == DateTime.Now.Date);

        if (sportsExercise is null)
        {
            _context.SportsExercises.Add(new SportsExercise()
            {
                UserId = id,
                Date = DateTime.Now,
                SportType = sportType,
                Count = count
            });
        }
        else
        {
            sportsExercise.Count += count;
        }
        return true;
    }
}
