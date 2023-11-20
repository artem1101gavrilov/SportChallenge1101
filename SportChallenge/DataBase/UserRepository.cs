using FitnessTelegramBot.Models.Sport;
using FitnessTelegramBot.Models.User;
using System.Linq.Expressions;

namespace FitnessTelegramBot.DataBase;

public class UserRepository : IRepository<User>
{
    private ApplicationContext _context;

    public UserRepository(ApplicationContext context)
    {
        _context = context;
    }

    public void Create(User user)
    {
        _context.Users.Add(user);
    }

    public User Get(Expression<Func<User, bool>> filter)
    {
        IQueryable<User> query = _context.Users;

        if (filter != null)
        {
            return query.First(filter);
        }
        throw new Exception($"{nameof(UserRepository)} {nameof(Get)}");
    }

    public IEnumerable<User> GetAll()
        => _context.Users;

    public IEnumerable<User> GetCollection(Expression<Func<User, bool>> filter)
    {
        IQueryable<User> query = _context.Users;

        if (filter != null)
        {
            return query.Where(filter);
        }
        return Enumerable.Empty<User>();
    }

    public bool TryUpdate(long id, SportType sportType, int count)
    {
        throw new NotImplementedException();
    }
}
