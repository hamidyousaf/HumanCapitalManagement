using Domain.Abstractions.GenericRepository;
using Domain.Entities;

namespace Domain.Abstractions;

public interface IBookRepository : IGenericRepository<Book>
{
}
