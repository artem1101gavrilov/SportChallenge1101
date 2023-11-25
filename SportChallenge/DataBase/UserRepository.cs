using SportChallenge.Models.Sport;
using SportChallenge.Models.User;
using System.Linq.Expressions;

namespace SportChallenge.DataBase;

public class UserRepository : IRepository<User>
{
    private ApplicationContext _context;
    public IDictionary<long, User> Users { get; } = new Dictionary<long, User>();

    public UserRepository(ApplicationContext context)
    {
        _context = context;
        Users = _context.Users.ToDictionary(user => user.Id);
    }

    public void Create(User user)
    {
        _context.Users.Add(user);
        Users.Add(user.Id, user);
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
