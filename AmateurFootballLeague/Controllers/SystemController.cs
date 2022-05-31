using AmateurFootballLeague.ExternalService;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.ViewModels.Requests;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/systems")]
    [ApiController]
    public class SystemController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPromoteRequestService _promoteRequestService;
        private readonly ISendEmailService _sendEmailService;
        private readonly IMapper _mapper;
        public SystemController(IUserService userService, IPromoteRequestService promoteRequestService, ISendEmailService sendEmailService, IMapper mapper)
        {
            _userService = userService;
            _promoteRequestService = promoteRequestService;
            _sendEmailService = sendEmailService;
            _mapper = mapper;
        }

        /// <summary>Send mail promote</summary>
        /// <returns>Return success</returns>
        /// <response code="200">Returns success</response>
        /// <response code="400">Send mail fail</response>
        /// <response code="500">Internal Server</response>
        [HttpPost("send-mail-promote")]
        [Produces("application/json")]
        public async Task<ActionResult> SendMailPromote(
            [FromQuery(Name = "promote-request-id")] int promoteRequestId,
            [FromQuery(Name = "approve")] bool approve
            )
        {
            try
            {
                PromoteRequest promoteRequest = await _promoteRequestService.GetByIdAsync(promoteRequestId);
                if (promoteRequest == null)
                {
                    return NotFound("Yêu cầu nâng cấp tài khoản không tồn tại");
                }
                User user = await _userService.GetByIdAsync((int)promoteRequest.UserId);

                EmailForm model = new EmailForm();
                model.ToEmail = user.Email;
                model.Subject = "Xác nhận nâng cấp tài khoản tài khoản A-Football-League";
                string approved = approve ? "được chấp thuận" : "bị từ chối </p><p style='font-size: 18px'>Lý do từ chối là: " + promoteRequest.Reason + "</p>";
                model.Message = "<html><head></head><body><p style='font-size: 18px'>Xin chào <a href='mailto:" + user.Email + "'>" + user.Email + "</a>,</p><p style='font-size: 18px'>Yêu cầu nâng cấp tài khoản lên chủ giải đấu của bạn đã " + approved + "<p style='font-size: 18px'>Xin cảm ơn,<br>A-Football-League</p>";

                if (!await _sendEmailService.SendEmail(model))
                {
                    return BadRequest("Gửi thất bại");
                }
                return Ok("Gửi thành công");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
