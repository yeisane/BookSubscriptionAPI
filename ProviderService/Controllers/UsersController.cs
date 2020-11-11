using AutoMapper;
using BusinessLogic.DTOModels;
using BusinessLogic.Models;
using BusinessLogic.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using ProviderService.Helpers;
using ProviderService.Middleware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

namespace ProviderService.Controllers
{
    [Route("api/users")]
    [AuthApiKey]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly ILogger<UsersController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public UsersController(ILogger<UsersController> logger, IUnitOfWork unitOfWork, IMapper mapper)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }         

        [HttpGet]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Get([FromQuery] PagedResults paging, bool includebooks = false)
        {
            _logger.LogInformation("UsersController:: GET called");

            try
            { 
                var users = await _unitOfWork.Users.GetUsersPagedAsync(paging, includebooks);
                var totalRecords = await _unitOfWork.Users.GetAllAsync();

                if (users.Count() <= 0)
                    return NotFound(new { message = "Database empty, please register some users" });

                   var response = PagedResultsHelper<UserDto>.PagingResponse(_mapper.Map<UserDto[]>(users), paging.PageIndex, paging.PageSize, totalRecords.Count());

                return Ok(response);

            }
            catch (Exception ex)
            {
                _logger.LogError("UsersController:: GET error", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }

        }

        [HttpGet("{id}")]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> GetById(int id, [FromQuery] bool includebooks=false)
        {
            _logger.LogDebug("UsersController:: GETBYID called");

            try
            {
                var user = await _unitOfWork.Users.GetAsync(id);

                if (includebooks)
                    user = await _unitOfWork.Users.GetSingleUserAsync(id);

                if (user == null)
                    return NotFound(new { message = "User not found" });

                return Ok(_mapper.Map<UserDto>(user));
            }
            catch (Exception ex)
            {
                _logger.LogError("UsersController:: GETBYID error", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }

        }


        [HttpPost]
        [Consumes(MediaTypeNames.Application.Json)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> Post([FromBody] UserAddDto newUser)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("UsersController:: POST called");

                Request.Headers.TryGetValue("ApiKey", out var apikey);

                var userExists = await _unitOfWork.Users.FindAsync(u => u.Email.ToLower().Trim() == newUser.Email.ToLower().Trim());

                newUser.ServiceProvider = apikey;

                if (userExists.Count() > 0)
                    return BadRequest(new { message = "User with specified email already registered" });

                var user = _mapper.Map<User>(newUser);
                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.CompleteAsync();

                return CreatedAtAction(nameof(GetById), new { id = user.Id }, new { message = "User added successfully..." });
            }
            catch (Exception ex)
            {
                _logger.LogError("UsersController:: POST error", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, [FromBody] UserAddDto updatedUser)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                _logger.LogInformation("UsersController:: PUT called");

                Request.Headers.TryGetValue("ApiKey", out var apikey);

                var user = await _unitOfWork.Users.GetAsync(id);

                if (user == null)
                    return BadRequest(new { message = "User not found. Update failed." });

                //can be improved by adding change tracking in unit of 
                //work but since this has few properties, this will surfice without any performance knock
                user.Email = updatedUser.Email ?? user.Email ;
                user.Firstname = updatedUser.Firstname ?? user.Firstname;
                user.Lastname = updatedUser.Lastname ?? user.Lastname;
                user.Password = updatedUser.Password ?? user.Password;
                user.Role = updatedUser.Role ?? user.Role;
                user.ServiceProvider = apikey;

                await _unitOfWork.CompleteAsync();

                return Ok(new { message = "User updated successfully..." });
            }
            catch (Exception ex)
            {
                _logger.LogError("UsersController:: PUT error", ex.StackTrace);
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
            //In realworld api, don't delete, just add a deleted column and set it to TRUE
            try
            {
                _logger.LogInformation("UsersController:: Delete called");
                var user = await _unitOfWork.Users.GetAsync(id);


                if (user == null)
                    return BadRequest(new { message = "Specified record not in database" });

                _unitOfWork.Users.Remove(user);

                await _unitOfWork.CompleteAsync();

                //return NoContent(); // Best practice but for an api to be consumed externally, message responses are good
                return Ok(new { message = "User removed successfully" });
            }
            catch (Exception ex)
            {
                _logger.LogError("UsersController:: DELETE error", ex.StackTrace);
                return StatusCode(StatusCodes.Status500InternalServerError, "Server error");
            }
        }
    }
}
