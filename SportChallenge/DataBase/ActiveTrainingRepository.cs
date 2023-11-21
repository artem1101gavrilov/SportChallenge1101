using SportChallenge.Models.Sport;
using System.Linq.Expressions;

namespace SportChallenge.DataBase;

public class ActiveTrainingRepository : IRepository<ActiveTraining>
{
    private ApplicationContext _context;

    public ActiveTrainingRepository(ApplicationContext context)
    {
        _context = context;
    }

    public void Create(ActiveTraining item)
    {
        throw new NotImplementedException();
    }

    public ActiveTraining Get(Expression<Func<ActiveTraining, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<ActiveTraining> GetAll()
        => _context.ActiveTraining;

    public IEnumerable<ActiveTraining> GetCollection(Expression<Func<ActiveTraining, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public bool TryUpdate(long id, SportType sportType, int level)
    {
        var activeTraining = _context.ActiveTraining.FirstOrDefault(_ => _.UserId == id
                                            && _.SportType == sportType);

        if (activeTraining is null)
        {
            _context.ActiveTraining.Add(new ActiveTraining()
            {
                UserId = id,
                Date = DateTime.Now,
                SportType = sportType,
                Level = level
            });
        }
        else
        {
            if (activeTraining.Level != level)
            {
                activeTraining.Date = DateTime.Now;
            }
            activeTraining.Level = level;
        }
        return true;
    }
}