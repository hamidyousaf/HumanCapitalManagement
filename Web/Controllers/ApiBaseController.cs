using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ApiBaseController : ControllerBase
    {
        protected readonly IMediator _mediator;
        public ApiBaseController(IMediator mediator)
        {
            _mediator = mediator;
        }
    }
}
