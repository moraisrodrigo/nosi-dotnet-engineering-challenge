namespace NOS.Engineering.Challenge.Interfaces;

public interface ICacheService<T>
{
  Task<T?> Get(Guid id);
  Task Set(Guid id, T item);
  Task Remove(Guid id);
}
