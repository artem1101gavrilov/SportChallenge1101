using FitnessTelegramBot.Models.Sport;
using System.Linq.Expressions;

namespace FitnessTelegramBot.DataBase;

interface IRepository<T> where T : class
{
    T Get(Expression<Func<T, bool>> filter);
    IEnumerable<T> GetCollection(Expression<Func<T, bool>> filter);
    IEnumerable<T> GetAll();
    void Create(T item);
    bool TryUpdate(long id, SportType sportType, int count);
}