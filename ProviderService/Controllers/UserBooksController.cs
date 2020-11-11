using AutoMapper;
using BusinessLogic.DTOModels;
using BusinessLogic.Models;
using BusinessLogic.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProviderService.Middleware;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ProviderService.Controllers
{
    [Route("api/userbooks")]
    [AuthApiKey]
    [ApiController]
    public class UserBooksController : ControllerBase
    {
        private readonly ILogger<UserBooksController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UserBooksController(ILogger<UserBooksController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
         
        [HttpGet("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            try
            {
                _logger.LogInformation("UserBooksController:: Get(int id) called");
                var user = await _unitOfWork.UserBooks.GetUserBooksAsync(id);

                if (user == null)
                    return NotFound(new { message = "User not subscribed to any books" });

                return Ok(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError("UserBooksController:: GET error", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }
        }

        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]

        public async Task<IActionResult> Post([FromBody] UserBookDto userbook)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("UserBooksController:: POST called");

                var exists = await _unitOfWork.UserBooks.FindAsync(x => x.BookId == userbook.BookId && x.UserId == userbook.UserId);

                if (exists.Count() > 0)
                    return BadRequest(new { message = "User already subscribed to selected book"});

                var ub = _mapper.Map<UserBook>(userbook);
                await _unitOfWork.UserBooks.AddAsync(ub);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction(nameof(GetById), new { id = ub.UserId }, new { message = "Subscribed successfully..." });
            }
            catch (Exception ex)
            {
                _logger.LogError("UserBooksController:: POST error", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }
        }

       
        [HttpDelete()]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete([FromBody] UserBookDto userbook)
        {
            try
            {
                _logger.LogInformation("UserBooksController:: Delete called");
                var ub = await _unitOfWork.UserBooks.FindAsync(x => x.BookId == userbook.BookId && x.UserId == userbook.UserId);


                if (ub.Count() < 0) 
                    return BadRequest(new { message = "Specified record not in database" });

                _unitOfWork.UserBooks.Remove(ub.First());

                await _unitOfWork.CompleteAsync();

                //return NoContent(); // Best practice but for an api to be consumed externally, message responses are good
                return Ok(new { message = "Book subscription removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("UserBooksController:: DELETE error", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }
        }
    }
}
