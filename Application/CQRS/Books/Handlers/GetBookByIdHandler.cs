using Domain.Abstractions.UnitOfWork;
using Domain.CQRS.Books.Queries;
using Domain.DTOs.ProjectToDTOs;
using Domain.DTOs.Responces;
using Domain.Entities;
using Domain.Extensions.ProjectTo;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Domain.CQRS.Books.Handlers;

public sealed class GetBookByIdHandler : IRequestHandler<GetBookByIdQuery, Result<BookProjectTo_V1>>
{
    private readonly IUnitOfWork _unitOfWork;

    public GetBookByIdHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<BookProjectTo_V1>> Handle(GetBookByIdQuery request, CancellationToken cancellationToken)
    {
        var book = await _unitOfWork.BookRepository
            .GetAllReadOnly()
            .ProjectTo_V1()
            .FirstOrDefaultAsync(x => x.Id == request.BookId, cancellationToken);

        if (book is null)
        {
            return Result<BookProjectTo_V1>.Fail($"There is no book found with id: {request.BookId}");
        }

        return Result<BookProjectTo_V1>.Success(book);
    }
}
