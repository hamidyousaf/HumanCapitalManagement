using Domain.DTOs.ProjectToDTOs;
using Domain.DTOs.Responces;
using MediatR;

namespace Domain.CQRS.Books.Queries;
public sealed class GetBookByIdQuery : IRequest<Result<BookProjectTo_V1>>
{
    public int BookId { get; }
    public GetBookByIdQuery(int bookId)
    {
        BookId = bookId;
    }
}
