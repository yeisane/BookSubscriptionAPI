using AutoMapper;
using BusinessLogic.Models;
using BusinessLogic.Repositories;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using ProviderService.Middleware;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ProviderService.Controllers
{
    /// <summary>
    /// Manage subscribers
    /// </summary>
    [Route("api/apiusers")]
    [AuthApiKey]
    [ApiController]
    public class ApiUsersController : ControllerBase
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public ApiUsersController( IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }


        /// <summary>
        /// Retrieve all users subscribed to this service
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                var apiusers = await _unitOfWork.RegisteredApiUsers.GetAllAsync();

                if (apiusers.Count() <= 0) return NotFound(new { message = "No registered api users in the database" });

                return Ok(apiusers);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Oops, server error");
            }
           
        }

        [HttpGet("{apikey}")]
        public async Task<IActionResult> GetById(string apikey)
        {
            try
            {
                var apiusers = await _unitOfWork.RegisteredApiUsers.FindAsync(x => x.ApiKey == apikey);

                if (apiusers.Count() <= 0) return NotFound(new { message = "No registered api users with that KEY in the database" });

                return Ok(apiusers.SingleOrDefault());
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, "Oops, server error");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] RegisteredApiUser newUser)
        {
            try
            {

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                //small controller thats only used internally, no need to over-engineer it with DTO

                var generatedApiKey = Guid.NewGuid();

                var apiuser = new RegisteredApiUser { ApiKey = generatedApiKey.ToString(), Name = newUser.Name,  Dns=newUser.Dns };

                await _unitOfWork.RegisteredApiUsers.AddAsync(apiuser);

                await _unitOfWork.CompleteAsync();

                return CreatedAtAction(nameof(GetById), new { apikey = apiuser.ApiKey }, new { message = "Api User added successfully..." });
            }
            catch (Exception ex)
            {
                //controller used internally, we can reveal what the hack happened :)
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpPut("{apikey}")]
        public async Task<IActionResult> Put(string apikey, [FromBody] RegisteredApiUser oldUser)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var apiuser = (await _unitOfWork.RegisteredApiUsers.FindAsync(x => x.ApiKey == apikey)).FirstOrDefault();

                if (apiuser == null) return NotFound(new { message = "Please check the API Key and submit again"});

                apiuser.Name = oldUser.Name;

                await _unitOfWork.CompleteAsync();

                return Ok(new { message = "Api User updated successfully..." });
            }
            catch (Exception ex)
            {
                //controller used internally, we can reveal what the hack happened :)
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }

        [HttpDelete("{apikey}")]
        public async Task<IActionResult> Delete(string apikey)
        {
            try
            {
                var apiuser = (await _unitOfWork.RegisteredApiUsers.FindAsync(x => x.ApiKey == apikey)).FirstOrDefault();

                if (apiuser == null) return NotFound(new { message = "Please check the API Key and submit again" });

                _unitOfWork.RegisteredApiUsers.Remove(apiuser);

                await _unitOfWork.CompleteAsync();

                //we could return NoContent but a message is more friendly. We are still returning a 20? code anyway
                return Ok(new { message = "Api User deleted successfully..." });
            }
            catch (Exception ex)
            {
                //controller used internally, we can reveal what the hack happened :)
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
            }
        }
    }
}
