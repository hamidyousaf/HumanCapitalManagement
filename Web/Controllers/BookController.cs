using Domain.CQRS.Books.Commands;
using Domain.CQRS.Books.Queries;
using Domain.DTOs.Requests;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    public class BookController : ApiBaseController
    {
        public BookController(IMediator mediator) : base(mediator){}

        [AllowAnonymous]
        [HttpGet("GetAllBooks")]
        public async Task<IActionResult> GetAllBooks()
        {
            var result = await _mediator.Send(new GetAllBooksQuery());

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpGet("GetBookById/{bookId}")]
        public async Task<IActionResult> GetBookById(int bookId)
        {
            var result = await _mediator.Send(new GetBookByIdQuery(bookId));

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPost("AddBook")]
        public async Task<IActionResult> AddBook([FromBody] AddBookRequest request)
        {
            var result = await _mediator.Send(new AddBookCommand(request));

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpPut("UpdateBook")]
        public async Task<IActionResult> UpdateBook([FromBody] UpdateBookRequest request)
        {
            var result = await _mediator.Send(new UpdateBookCommand(request));

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }

        [AllowAnonymous]
        [HttpDelete("DeleteBook/{bookId}")]
        public async Task<IActionResult> DeleteBook(int bookId)
        {
            var result = await _mediator.Send(new DeleteBookCommand(bookId));

            if (result.Succeeded)
            {
                return Ok(result);
            }

            return BadRequest(result);
        }
    }
}
