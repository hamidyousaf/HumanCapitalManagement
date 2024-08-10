using Domain.DTOs.ProjectToDTOs;
using Domain.Entities;

namespace Domain.Extensions.ProjectTo;

internal static class BookExtensions
{
    internal static IQueryable<BookProjectTo_V1> ProjectTo_V1(this IQueryable<Book> book)
    {
        var result = book.Select(x => new BookProjectTo_V1
        {
            Id = x.Id,
            Title = x.Title,
            Author = x.Author,
            Description = x.Description
        });
        return result;
    }
}
