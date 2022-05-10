using AmateurFootballLeague.ExternalService;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJWTProvider _jwtProvider;
        private readonly ISendEmailService _sendEmailService;
        private readonly IUploadFileService _uploadFileService;
        private readonly IMapper _mapper;
        public AuthController(IUserService userService, IJWTProvider jwtProvider, ISendEmailService sendEmailService, IUploadFileService uploadFileService, IMapper mapper)
        {
            _userService = userService;
            _jwtProvider = jwtProvider;
            _sendEmailService = sendEmailService;
            _uploadFileService = uploadFileService;
            _mapper = mapper;
        }

        /// <summary>Login with email and password</summary>
        /// <returns>Return user and token token</returns>
        /// <response code="200">Returns user and token</response>
        /// <response code="404">Not found users</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("login-email-password")]
        public async Task<ActionResult<UserLVM>> LoginWithEmailAndPassword(UserLEPM model)
        {
            try
            {
                User user = _userService.GetUserByEmail(model.Email);
                if (user == null)
                {
                    return NotFound("User is not exist");
                }
                if (!_userService.CheckPassword(model.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return BadRequest("Wrong password");
                }
                if (!user.Status.HasValue)
                {
                    return BadRequest("User is ban");
                }
                UserVM userVM = _mapper.Map<UserVM>(user);
                UserLVM userLEPVM = new UserLVM
                {
                    UserVM = userVM,
                    AccessToken = await _jwtProvider.GenerationToken(user)
                };

                return Ok(userLEPVM);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Login with google</summary>
        /// <returns>Return user and token token</returns>
        /// <response code="200">Returns user and token</response>
        /// <response code="404">Not found users</response>
        /// <response code="500">Internal server error</response>
        [HttpPost("login-with-google")]
        [Produces("application/json")]
        public async Task<ActionResult<UserLVM>> Login([FromBody] UserLM model)
        {
            var auth = FirebaseAdmin.Auth.FirebaseAuth.DefaultInstance;
            string email;
            try
            {
                var token = await auth.VerifyIdTokenAsync(model.TokenId);
                email = (string)token.Claims["email"];
            }
            catch (Exception e)
            {
                return Unauthorized(e.Message);
            }

            try
            {
                User user = _userService.GetUserByEmail(email);
                if (user == null)
                {
                    return NotFound("User is not exist");
                }

                if (!user.Status.HasValue)
                {
                    return BadRequest("User is ban");
                }

                UserVM userVM = _mapper.Map<UserVM>(user);
                UserLVM userLEPVM = new UserLVM
                {
                    UserVM = userVM,
                    AccessToken = await _jwtProvider.GenerationToken(user)
                };

                return Ok(userLEPVM);
            }
            catch (Exception e)
            {
                Response.StatusCode = StatusCodes.Status500InternalServerError;
                return new JsonResult(e.Message);
            }
        }

        /// <summary>Logout the system</summary>
        [HttpPost("logout")]
        public async Task<ActionResult> Logout([FromBody] UserLOM model)
        {
            User user = _userService.GetUserByEmail(model.Email);
            if (user == null)
            {
                return BadRequest("User is not exist.");
            }
            try
            {
                var response = await FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(new List<string>() { model.Token }, "/topics/" + user.Id);
                if (response.SuccessCount > 0)
                {
                    return Ok(new { Message = "Logout success!" });
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Upload image to firebase</summary>
        [HttpPost("upload-image")]
        [Produces("application/json")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> uploadFileImage([FromForm] ImageCM model)
        {
            try
            {
                string fileUrl = await _uploadFileService.UploadFile(model.File, "service", "service-detail");
                return Ok(new
                {
                    url = fileUrl
                });
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }

        /// <summary>Send mail from system</summary>
        [HttpPost("send-mail")]
        [Produces("application/json")]
        public async Task<ActionResult> SendEmail([FromBody] EmailForm model)
        {
            try
            {
                if ( !await _sendEmailService.SendEmail(model))
                {
                    return BadRequest();
                }
                
                return Ok("Success");
            }
            catch (Exception e)
            {
                return Ok(e.Message);
            }
        }
    }
}
