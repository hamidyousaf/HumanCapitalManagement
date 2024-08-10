using Domain.DTOs.ProjectToDTOs;
using Domain.DTOs.Responces;
using Domain.Entities;
using MediatR;

namespace Domain.CQRS.Books.Queries;

public sealed class GetAllBooksQuery : IRequest<Result<List<BookProjectTo_V1>>>
{
}
