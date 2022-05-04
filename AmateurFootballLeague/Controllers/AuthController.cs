using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJWTProvider _jwtProvider;
        public AuthController(IUserService userService, IJWTProvider jwtProvider)
        {
            _userService = userService;
            _jwtProvider = jwtProvider;
        }

        /// <summary>Login with email and password</summary>
        /// <returns>Token JWT</returns>
        /// <response code="200">Returns token jwt</response>
        /// <response code="404">Not found users</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("login")]
        public async Task<ActionResult<UserLVM>> LoginWithEmailAndPassword(UserLM model)
        {
            try
            {
                User user = _userService.GetUserByEmail(model.Email);
                if(user == null)
                {
                    return NotFound("Account is not exist");
                }
                if(!_userService.CheckPassword(model.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return BadRequest("Wrong password");
                }
                
                UserLVM userLVM = new UserLVM();
                userLVM.Email = model.Email;
                userLVM.Token = await _jwtProvider.GenerationToken(user);

                return Ok(userLVM);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
