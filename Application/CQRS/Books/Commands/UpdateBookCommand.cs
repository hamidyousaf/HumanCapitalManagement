using Domain.DTOs.Requests;
using Domain.DTOs.Responces;
using MediatR;

namespace Domain.CQRS.Books.Commands;

public sealed class UpdateBookCommand : IRequest<Result<bool>>
{
    public UpdateBookRequest Book { get; }
    public UpdateBookCommand(UpdateBookRequest book)
    {
        Book = book;
    }
}
