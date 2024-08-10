namespace Domain.Abstractions.UnitOfWork;

public interface IUnitOfWork
{
    #region Add repositories here
    IBookRepository BookRepository { get; }
    #endregion

    Task<bool> IsCompleted();
    Task SaveChangesAsync();
    bool HasChanges();
}
