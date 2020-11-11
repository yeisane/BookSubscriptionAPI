using AutoMapper;
using BusinessLogic.DTOModels;
using BusinessLogic.Models;
using BusinessLogic.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProviderService.Helpers;
using ProviderService.Middleware;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ProviderService.Controllers
{
    [ApiController]
    [AuthApiKey]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
         
        private readonly ILogger<BooksController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public BooksController(ILogger<BooksController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        /// <summary>
        /// Gets all books in the database. Default page size is 10 records per page
        /// </summary>
        /// <param name="paging">Holds the pageIndex and pageSize params</param>
        /// <returns>All books paged using specified page size and if not defaults to 10 record per page</returns>
        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] PagedResults paging)
        {
            _logger.LogInformation("UserBooksController:: GETPAGED called");

            try
            {
                var books = await _unitOfWork.Books.GetBooksPagedAsync(paging);
                var totalRecords = await _unitOfWork.Books.GetAllAsync(); 

                if (books.Count() <= 0)
                    return NotFound(new { message = "Database empty, please add some books to the library" });

                var response = PagedResultsHelper<BookDto>.PagingResponse(_mapper.Map<BookDto[]>(books), paging.PageIndex, paging.PageSize, totalRecords.Count());
                
                return Ok (response);
            }
            catch (Exception ex)
            {
                _logger.LogError("BooksController:: GETPAGED error", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }
           
        }

        /// <summary>
        /// Gets one book by its ID
        /// </summary>
        /// <param name="id">Id of the book</param>
        /// <returns>One books object</returns>
        [HttpGet("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id)
        {
            _logger.LogDebug("BooksController:: GETBYID called");

            try
            {
                var book = await _unitOfWork.Books.GetAsync(id);
                if (book == null)
                    return NotFound(new { message = "Book not found" });

                return Ok(_mapper.Map<BookDto>(book));
            }
            catch (Exception ex)
            {
                _logger.LogError("BooksController:: GETBYID error", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }
           
        }

        /// <summary>
        /// Creates a book in the database
        /// </summary>
        /// <param name="newBook">Book object transfer object</param>
        /// <returns>The path to the created book in the header</returns>
        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] BookDto newBook)
        { 
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("BooksController:: POST called");

                var book = _mapper.Map<Book>(newBook);
                await _unitOfWork.Books.AddAsync(book);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction(nameof(GetById), new { id = book.Id }, new { message = "Book added successfully..." });
            }
            catch (Exception ex)
            {
                _logger.LogError("UserBooksController:: POST error", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }
        }

        /// <summary>
        /// Take in an id and a book object and updates the info on the book
        /// </summary>
        /// <param name="id">ID of book to update</param>
        /// <param name="updatedBook">Book object with new info</param>
        /// <returns>Status 200 on success or 500 or error</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] BookDto updatedBook)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("BooksController:: PUT called");


                var book = await _unitOfWork.Books.GetAsync(id);

                if (book == null)
                    return BadRequest(new { message = "Book not found. Update failed." });

                //can be improved by adding change tracking in unit of 
                //work but since this has few properties, this will surfice without any performance knock
                book.Name = updatedBook.Name;
                book.Price = updatedBook.Price;
                book.Text = updatedBook.Text;

                await _unitOfWork.CompleteAsync();

                return Ok(new { message = "Book updated successfully..." });
            }
            catch (Exception ex)
            {
                _logger.LogError("UserBooksController:: PUT error", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }
        }

        [HttpDelete("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _logger.LogInformation("BooksController:: Delete called");
                var book = await _unitOfWork.Books.GetAsync(id);


                if (book == null)
                    return BadRequest(new { message = "Specified record not in database" });

                _unitOfWork.Books.Remove(book);

                await _unitOfWork.CompleteAsync();

                //return NoContent(); // Best practice but for an api to be consumed externally, message responses are good
                return Ok(new { message = "Book removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("BooksController:: DELETE error", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }
        }
    }
}
