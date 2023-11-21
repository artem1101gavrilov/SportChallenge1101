using SportChallenge.Models.Sport;
using System.Linq.Expressions;

namespace SportChallenge.DataBase;

public class RecordsOneTimeRepository : IRepository<RecordsOneTime>
{
    private ApplicationContext _context;

    public RecordsOneTimeRepository(ApplicationContext context)
    {
        _context = context;
    }

    public void Create(RecordsOneTime item)
    {
        throw new NotImplementedException();
    }

    public RecordsOneTime Get(Expression<Func<RecordsOneTime, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<RecordsOneTime> GetAll()
        => _context.RecordsOneTime;

    public IEnumerable<RecordsOneTime> GetCollection(Expression<Func<RecordsOneTime, bool>> filter)
    {
        throw new NotImplementedException();
    }

    public bool TryUpdate(long id, SportType sportType, int count)
    {
        var record = _context.RecordsOneTime.FirstOrDefault(_ => _.UserId == id
                                            && _.SportType == sportType);

        if (record is null)
        {
            _context.RecordsOneTime.Add(new RecordsOneTime()
            {
                Count = count,
                Date = DateTime.Now,
                SportType = sportType,
                UserId = id
            });
            return true;
        }
        else if (record.Count < count)
        {
            record.Count = count;
            record.Date = DateTime.Now;
            return true;
        }
        return false;
    }
}
