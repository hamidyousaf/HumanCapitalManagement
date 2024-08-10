using Domain.Abstractions.UnitOfWork;
using Domain.CQRS.Books.Commands;
using Domain.DTOs.Responces;
using MediatR;

namespace Domain.CQRS.Books.Handlers;

public sealed class UpdateBookHandler : IRequestHandler<UpdateBookCommand, Result<bool>>
{
    private readonly IUnitOfWork _unitOfWork;

    public UpdateBookHandler(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> Handle(UpdateBookCommand request, CancellationToken cancellationToken)
    {
        // Get book by id.
        var book = await _unitOfWork.BookRepository.GetById(request.Book.Id);

        if (book is null)
        {
            return Result<bool>.Fail($"There is no book found with id: {request.Book.Id}");
        }

        book.Title = request.Book.Title;
        book.Description = request.Book.Description;
        book.Author = request.Book.Author;

        _unitOfWork.BookRepository.Change(book);
        await _unitOfWork.SaveChangesAsync();

        return Result<bool>.Success(true, "Book updated successfully");
    }
}
