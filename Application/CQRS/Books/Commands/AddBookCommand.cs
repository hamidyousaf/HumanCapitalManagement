using Domain.DTOs.Requests;
using Domain.DTOs.Responces;
using MediatR;

namespace Domain.CQRS.Books.Commands;

public sealed class AddBookCommand : IRequest<Result<int>>
{
    public AddBookRequest Book { get; }
    public AddBookCommand(AddBookRequest book)
    {
        Book = book;
    }
}
