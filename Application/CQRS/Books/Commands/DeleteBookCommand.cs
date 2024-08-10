using Domain.DTOs.Responces;
using MediatR;

namespace Domain.CQRS.Books.Commands;

public sealed class DeleteBookCommand : IRequest<Result<bool>>
{
    public int BookId { get; }
    public DeleteBookCommand(int bookId)
    {
        BookId = bookId;
    }
}
