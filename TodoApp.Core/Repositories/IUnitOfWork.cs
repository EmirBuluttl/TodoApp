namespace TodoApp.Core.Repositories;

public interface IUnitOfWork : IAsyncDisposable, IDisposable
{
    Task CommitAsync();
    void Commit();
}
