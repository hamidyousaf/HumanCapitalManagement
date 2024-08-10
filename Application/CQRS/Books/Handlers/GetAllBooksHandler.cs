using Domain.Abstractions.UnitOfWork;
using Domain.CQRS.Books.Queries;
using Domain.DTOs.ProjectToDTOs;
using Domain.DTOs.Responces;
using Domain.Extensions.ProjectTo;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.CQRS.Books.Handlers;

public sealed class GetAllBooksHandler : IRequestHandler<GetAllBooksQuery, Result<List<BookProjectTo_V1>>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetAllBooksHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    async Task<Result<List<BookProjectTo_V1>>> IRequestHandler<GetAllBooksQuery, Result<List<BookProjectTo_V1>>>.Handle(GetAllBooksQuery request, CancellationToken cancellationToken)
    {
        var result = await _unitOfWork.BookRepository
            .GetAllReadOnly()
            .ProjectTo_V1()
            .ToListAsync(cancellationToken);

        return Result<List<BookProjectTo_V1>>.Success(result);
    }
}
