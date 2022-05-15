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
    [Route("api/v1/auth")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IJWTProvider _jwtProvider;
        private readonly ISendEmailService _sendEmailService;
        private readonly IMapper _mapper;
        public AuthController(IUserService userService, IJWTProvider jwtProvider, ISendEmailService sendEmailService, IMapper mapper)
        {
            _userService = userService;
            _jwtProvider = jwtProvider;
            _sendEmailService = sendEmailService;
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
                    return NotFound("Tài khoản không tồn tại");
                }
                if (!_userService.CheckPassword(model.Password, user.PasswordHash, user.PasswordSalt))
                {
                    return BadRequest("Sai mật khẩu");
                }
                if (!user.Status.HasValue)
                {
                    return BadRequest("Tài khoản đã bị khóa");
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
                    return NotFound("Tài khoản không tồn tại");
                }

                if (!user.Status.HasValue)
                {
                    return BadRequest("Tài khoản đã bị khóa");
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
                return BadRequest("Tài khoản không tồn tại");
            }
            try
            {
                var response = await FirebaseAdmin.Messaging.FirebaseMessaging.DefaultInstance.UnsubscribeFromTopicAsync(new List<string>() { model.Token }, "/topics/" + user.Id);
                if (response.SuccessCount > 0)
                {
                    return Ok(new { Message = "Đăng xuất thành công" });
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Send verify code from system</summary>
        /// <returns>Return verify code</returns>
        /// <response code="200">Returns verify code</response>
        /// <response code="400">Send mail fail</response>
        /// <response code="500">Internal Server</response>
        [HttpPost("send-verify-code")]
        [Produces("application/json")]
        public async Task<ActionResult<String>> SendVerifyCode(String email, int toDo)
        {
            try
            {
                User user = _userService.GetUserByEmail(email);
                if(toDo == 1)
                {
                    if (user != null)
                    {
                        return BadRequest("Tài khoản đã tồn tại");
                    }
                } else
                {
                    if (user == null)
                    {
                        return NotFound("Tài khoản không tồn tại");
                    }
                }
                Random rd = new Random();
                int code = rd.Next(100000, 999999);

                EmailForm model = new EmailForm();
                model.ToEmail = email;
                model.Subject = "Mã xác nhận tài khoản A-Football-League";
                model.Message = "<html><head></head><body><p style='font-size: 18px'>Xin chào <a href='mailto:" + email + "'>" + email + "</a>,</p><p style='font-size: 18px'>Chúng tôi đã nhận yêu cầu gửi mã xác thực cho tài khoản A-Football-League của bạn.</p><p style='font-size: 18px'>Mã xác thực của bạn là: " + code.ToString() + "</p><p style='font-size: 18px'>Nếu không yêu cầu mã này thì bạn có thể bỏ qua email này một cách an toàn. Có thể ai đó khác đã nhập địa chỉ email của bạn do nhầm lẫn.</p><p style='font-size: 18px'>Xin cảm ơn,<br>A-Football-League</p>";

                if (!await _sendEmailService.SendEmail(model))
                {
                    return BadRequest("Gửi thất bại");
                }

                return Ok(code.ToString());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
