using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PetStore.Domain;

namespace PetStore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IUser _user;
        private readonly ILogger<UserController> _logger;

        public UserController(IUser user, ILogger<UserController> logger)
        {
            _user = user;
            _logger = logger;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status405MethodNotAllowed)]
        public ActionResult<Models.User> AddUser(Models.User user)
        {
            return _user.SetUser(user);
        }

        [HttpGet("{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.User> GetUserByUsername(string username)
        {
            var user = _user.GetUserByUsername(username);
            if (user == null)
            {
                return NotFound();
            }

            return user;
        }
        
        [HttpPut("{username}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<Models.User> UpdateUser(string username, Models.User updatedUser)
        {
            try
            {
                var user = _user.GetUserByUsername(username);
                if (user == null)
                {
                    return NotFound();
                }

                return _user.UpdateUser(username, updatedUser);
            }
            catch (Exception e)
            {
                _logger.LogError("Error", e);
                throw;
            }
        }
        

    }
}