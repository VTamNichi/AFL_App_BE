using AmateurFootballLeague.ExternalService;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.ViewModels.Requests;
using AutoMapper;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/systems")]
    [ApiController]
    //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SystemController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IPromoteRequestService _promoteRequestService;
        private readonly IFootballPlayerService _footballPlayerService;
        private readonly ITeamService _teamService;
        private readonly ITournamentService _tournamentService;
        private readonly ISendEmailService _sendEmailService;
        public SystemController(IUserService userService, IPromoteRequestService promoteRequestService, IFootballPlayerService footballPlayerService, ITeamService teamService, ITournamentService tournamentService, ISendEmailService sendEmailService)
        {
            _userService = userService;
            _promoteRequestService = promoteRequestService;
            _footballPlayerService = footballPlayerService;
            _teamService = teamService;
            _tournamentService = tournamentService;
            _sendEmailService = sendEmailService;
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
                User user = await _userService.GetByIdAsync(promoteRequest.UserId!.Value);

                EmailForm model = new();
                model.ToEmail = user.Email!;
                model.Subject = "Xác nhận nâng cấp tài khoản tài khoản A-Football-League";
                string approved = approve ? "được chấp thuận.</p>" : "bị từ chối.</p>" +
                    "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                    "Lý do từ chối là: " + promoteRequest.Reason + "</p>";
                model.Message = "<html><head></head><body>" +
                    "<header style='margin: 0px auto; margin-top: 20px; max-width: 590px; padding: 10px 50px; background-color: #00afab;'>" +
                    "<img src='https://afl-app-fe.vercel.app/assets/img/homepage/logo.png' alt='' class='logo' style='display: inline-flex; color: #fff; width: 60px;' width='60'/>" +
                    "</header>" +
                    "<div class='form_mail' style='font-family: Arial, Helvetica, sans-serif; padding: 50px; background-color: #f4f7f6; max-width: 590px; margin: 0px auto; line-height: 24px;'>" +

                    "<div class='title' style='display: block; font-size: 1.5em; margin-block-start: 0.83em; margin-block-end: 0.83em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Nâng cấp tài khoản</div>" +
                    "<p class='hello' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin chào <a href='mailto:" + user.Email + "'>" + user.Email + "</a>,</p>" +
                    "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                    "Yêu cầu nâng cấp tài khoản lên chủ giải đấu của bạn đã " + approved +
                    "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                    " Hãy truy cập vào trang chủ AFL để bắt đầu tạo giải.</p>" +

                    "<a href='https://afl-app-fe.vercel.app/' style='background-color: #00afab; border-radius: 50px; padding: 10px 20px; display: inline-flex; color: #fff; font-weight: bold; text-decoration: none; text-transform: uppercase; font-size: 14px; margin-top: 10px;'>Trang chủ AFL</a>" +
                    "<p class='thank' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin cảm ơn,</p>" +
                    "<p class='thanks' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Amateur Football League</p>" +
                    "</div>" +
                    "<footer style='padding: 30px 50px; max-width: 590px; margin: 0px auto; font-size: 13px; background: #ddd; color: #767676; font-family: Arial, Helvetica, sans-serif; line-height: 24px;'>" +
                    "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Đây là email gửi từ hệ thống AFL</p>" +
                    "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Vui lòng không trả lời trực tiếp qua email này</p>" +
                    "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>" +
                    "Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh" +
                    "</p>" +
                    "</footer></body></html>";

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

        /// <summary>Send mail register to team or recruit football player</summary>
        /// <returns>Return success</returns>
        /// <response code="200">Returns success</response>
        /// <response code="400">Send mail fail</response>
        /// <response code="500">Internal Server</response>
        [HttpPost("send-mail-register-recruit")]
        [Produces("application/json")]
        public async Task<ActionResult> SendMailRegisterOrRecruit([FromBody] SendMailRegisterOrRecruitCM modelRR)
        {
            try
            {
                FootballPlayer footballPlayer = await _footballPlayerService.GetByIdAsync(modelRR.FootballPlayerId);
                if (footballPlayer == null)
                {
                    return NotFound("Cầu thủ không tồn tại");
                }
                Team team = await _teamService.GetByIdAsync(modelRR.TeamId);
                if (team == null)
                {
                    return NotFound("Đội bóng không tồn tại");
                }
                User userFP = await _userService.GetByIdAsync(modelRR.FootballPlayerId);
                User userTeam = await _userService.GetByIdAsync(modelRR.TeamId);

                EmailForm model = new();

                Tournament tournament = await _tournamentService.GetByIdAsync(modelRR.TournamentId!.Value);
                if (tournament == null)
                {
                    if (modelRR.Status)
                    {
                        if (modelRR.Type == TypeRegisterOrRecruit.Recruit)
                        {
                            model.ToEmail = userTeam.Email!;
                            model.Subject = "Yêu Cầu Chiêu Mộ Cầu Thủ Thành Công";
                            model.Message = "<html><head></head><body>" +
                                "<header style='margin: 0px auto; margin-top: 20px; max-width: 590px; padding: 10px 50px; background-color: #00afab;'>" +
                                "<img src='https://afl-app-fe.vercel.app/assets/img/homepage/logo.png' alt='' class='logo' style='display: inline-flex; color: #fff; width: 60px;' width='60'/>" +
                                "</header>" +
                                "<div class='form_mail' style='font-family: Arial, Helvetica, sans-serif; padding: 50px; background-color: #f4f7f6; max-width: 590px; margin: 0px auto; line-height: 24px;'>" +

                                "<div class='title' style='display: block; font-size: 1.5em; margin-block-start: 0.83em; margin-block-end: 0.83em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Chiêu mộ cầu thủ thành công</div>" +
                                "<p class='hello' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin chào quản lý đội bóng <b>" + team.TeamName + "</b>,</p>" +
                                "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                                "Cầu thủ <b>" + footballPlayer.PlayerName + "</b> đã chấp nhận tham gia đội bóng của bạn. Hãy vào phần <b>Đội bóng của bạn</b> tại trang chủ AFL để biết thêm chi tiết về cầu thủ." +
                                "</p>" +

                                "<a href='https://afl-app-fe.vercel.app/' style='background-color: #00afab; border-radius: 50px; padding: 10px 20px; display: inline-flex; color: #fff; font-weight: bold; text-decoration: none; text-transform: uppercase; font-size: 14px; margin-top: 10px;'>Trang chủ AFL</a>" +
                                "<p class='thank' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin cảm ơn,</p>" +
                                "<p class='thanks' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Amateur Football League</p>" +
                                "</div>" +
                                "<footer style='padding: 30px 50px; max-width: 590px; margin: 0px auto; font-size: 13px; background: #ddd; color: #767676; font-family: Arial, Helvetica, sans-serif; line-height: 24px;'>" +
                                "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Đây là email gửi từ hệ thống AFL</p>" +
                                "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Vui lòng không trả lời trực tiếp qua email này</p>" +
                                "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>" +
                                "Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh" +
                                "</p>" +
                                "</footer></body></html>";
                        }
                        else
                        {
                            model.ToEmail = userFP.Email!;
                            model.Subject = "Yêu Cầu Tham Gia Đội Bóng Thành Công";
                            model.Message = "<html><head></head><body>" +
                                "<header style='margin: 0px auto; margin-top: 20px; max-width: 590px; padding: 10px 50px; background-color: #00afab;'>" +
                                "<img src='https://afl-app-fe.vercel.app/assets/img/homepage/logo.png' alt='' class='logo' style='display: inline-flex; color: #fff; width: 60px;' width='60'/>" +
                                "</header>" +
                                "<div class='form_mail' style='font-family: Arial, Helvetica, sans-serif; padding: 50px; background-color: #f4f7f6; max-width: 590px; margin: 0px auto; line-height: 24px;'>" +

                                "<div class='title' style='display: block; font-size: 1.5em; margin-block-start: 0.83em; margin-block-end: 0.83em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Tham gia đội bóng thành công</div>" +
                                "<p class='hello' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin chào cầu thủ <b>" + footballPlayer.PlayerName + "</b>,</p>" +
                                "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                                "Đội bóng <b>" + team.TeamName + "</b> đã chấp nhận yêu cầu tham gia của bạn. Hãy vào phần <b>Thông tin cầu thủ</b> tại trang chủ AFL để biết thêm chi tiết về đội bóng." +
                                "</p>" +

                                "<a href='https://afl-app-fe.vercel.app/' style='background-color: #00afab; border-radius: 50px; padding: 10px 20px; display: inline-flex; color: #fff; font-weight: bold; text-decoration: none; text-transform: uppercase; font-size: 14px; margin-top: 10px;'>Trang chủ AFL</a>" +
                                "<p class='thank' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin cảm ơn,</p>" +
                                "<p class='thanks' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Amateur Football League</p>" +
                                "</div>" +
                                "<footer style='padding: 30px 50px; max-width: 590px; margin: 0px auto; font-size: 13px; background: #ddd; color: #767676; font-family: Arial, Helvetica, sans-serif; line-height: 24px;'>" +
                                "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Đây là email gửi từ hệ thống AFL</p>" +
                                "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Vui lòng không trả lời trực tiếp qua email này</p>" +
                                "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>" +
                                "Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh" +
                                "</p>" +
                                "</footer></body></html>";
                        }
                    }
                    else
                    {
                        if (modelRR.Type == TypeRegisterOrRecruit.Register)
                        {
                            model.ToEmail = userTeam.Email!;
                            model.Subject = "Yêu Cầu Tham Gia Từ Cầu Thủ " + footballPlayer.PlayerName;
                            model.Message = "<html><head></head><body>" +
                                "<header style='margin: 0px auto; margin-top: 20px; max-width: 590px; padding: 10px 50px; background-color: #00afab;'>" +
                                "<img src='https://afl-app-fe.vercel.app/assets/img/homepage/logo.png' alt='' class='logo' style='display: inline-flex; color: #fff; width: 60px;' width='60'/>" +
                                "</header>" +
                                "<div class='form_mail' style='font-family: Arial, Helvetica, sans-serif; padding: 50px; background-color: #f4f7f6; max-width: 590px; margin: 0px auto; line-height: 24px;'>" +

                                "<div class='title' style='display: block; font-size: 1.5em; margin-block-start: 0.83em; margin-block-end: 0.83em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Đăng ký tham gia đội bóng</div>" +
                                "<p class='hello' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin chào quản lý đội bóng <b>" + team.TeamName + "</b>,</p>" +
                                "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                                "Cầu thủ <b>" + footballPlayer.PlayerName + "</b> đã gửi lời mời tham gia đội bóng mà bạn quản lý. Hãy vào phần <b>Đội bóng của bạn</b> tại trang chủ AFL để xét duyệt cầu thủ." +
                                "</p>" +
                                
                                "<a href='https://afl-app-fe.vercel.app/' style='background-color: #00afab; border-radius: 50px; padding: 10px 20px; display: inline-flex; color: #fff; font-weight: bold; text-decoration: none; text-transform: uppercase; font-size: 14px; margin-top: 10px;'>Trang chủ AFL</a>" +
                                "<p class='thank' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin cảm ơn,</p>" +
                                "<p class='thanks' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Amateur Football League</p>" +
                                "</div>" +
                                "<footer style='padding: 30px 50px; max-width: 590px; margin: 0px auto; font-size: 13px; background: #ddd; color: #767676; font-family: Arial, Helvetica, sans-serif; line-height: 24px;'>" +
                                "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Đây là email gửi từ hệ thống AFL</p>" +
                                "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Vui lòng không trả lời trực tiếp qua email này</p>" +
                                "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>" +
                                "Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh" +
                                "</p>" +
                                "</footer></body></html>";
                        }
                        else
                        {
                            model.ToEmail = userFP.Email!;
                            model.Subject = "Yêu Cầu Chiêu Mộ Từ Đội Bóng " + team.TeamName;
                            model.Message = "<html><head></head><body>" +
                                "<header style='margin: 0px auto; margin-top: 20px; max-width: 590px; padding: 10px 50px; background-color: #00afab;'>" +
                                "<img src='https://afl-app-fe.vercel.app/assets/img/homepage/logo.png' alt='' class='logo' style='display: inline-flex; color: #fff; width: 60px;' width='60'/>" +
                                "</header>" +
                                "<div class='form_mail' style='font-family: Arial, Helvetica, sans-serif; padding: 50px; background-color: #f4f7f6; max-width: 590px; margin: 0px auto; line-height: 24px;'>" +

                                "<div class='title' style='display: block; font-size: 1.5em; margin-block-start: 0.83em; margin-block-end: 0.83em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Chiêu mộ từ đội bóng</div>" +
                                "<p class='hello' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin chào cầu thủ <b>" + footballPlayer.PlayerName + "</b>,</p>" +
                                "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                                "Đội bóng <b>" + team.TeamName + "</b> gửi lời mời chiêu mộ bạn vào đội. Hãy vào phần <b>Thông tin cầu thủ</b> tại trang chủ AFL để xét duyệt tham gia đội bóng." +
                                "</p>" +
                                
                                "<a href='https://afl-app-fe.vercel.app/' style='background-color: #00afab; border-radius: 50px; padding: 10px 20px; display: inline-flex; color: #fff; font-weight: bold; text-decoration: none; text-transform: uppercase; font-size: 14px; margin-top: 10px;'>Trang chủ AFL</a>" +
                                "<p class='thank' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin cảm ơn,</p>" +
                                "<p class='thanks' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Amateur Football League</p>" +
                                "</div>" +
                                "<footer style='padding: 30px 50px; max-width: 590px; margin: 0px auto; font-size: 13px; background: #ddd; color: #767676; font-family: Arial, Helvetica, sans-serif; line-height: 24px;'>" +
                                "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Đây là email gửi từ hệ thống AFL</p>" +
                                "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Vui lòng không trả lời trực tiếp qua email này</p>" +
                                "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>" +
                                "Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh" +
                                "</p>" +
                                "</footer></body></html>";
                        }
                    }
                }
                else
                {
                    model.ToEmail = userFP.Email!;
                    model.Subject = "Thông báo tham gia giải đấu";
                    model.Message = "<html><head></head><body>" +
                        "<header style='margin: 0px auto; margin-top: 20px; max-width: 590px; padding: 10px 50px; background-color: #00afab;'>" +
                        "<img src='https://afl-app-fe.vercel.app/assets/img/homepage/logo.png' alt='' class='logo' style='display: inline-flex; color: #fff; width: 60px;' width='60'/>" +
                        "</header>" +
                        "<div class='form_mail' style='font-family: Arial, Helvetica, sans-serif; padding: 50px; background-color: #f4f7f6; max-width: 590px; margin: 0px auto; line-height: 24px;'>" +

                        "<div class='title' style='display: block; font-size: 1.5em; margin-block-start: 0.83em; margin-block-end: 0.83em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Tham gia giải đấu</div>" +
                        "<p class='hello' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin chào cầu thủ <b>" + footballPlayer.PlayerName + "</b>,</p>" +
                        "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                        "Quản lý đội bóng <b>" + userTeam.Username + "</b> đã đăng ký bạn vào giải đấu <b>" + tournament.TournamentName + "</b>. Hãy vào phần <b>Thông tin cầu thủ</b> tại trang chủ AFL để biết thêm chi tiết về giải đấu." +
                        "</p>" +
                        
                        "<a href='https://afl-app-fe.vercel.app/' style='background-color: #00afab; border-radius: 50px; padding: 10px 20px; display: inline-flex; color: #fff; font-weight: bold; text-decoration: none; text-transform: uppercase; font-size: 14px; margin-top: 10px;'>Trang chủ AFL</a>" +
                        "<p class='thank' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin cảm ơn,</p>" +
                        "<p class='thanks' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Amateur Football League</p>" +
                        "</div>" +
                        "<footer style='padding: 30px 50px; max-width: 590px; margin: 0px auto; font-size: 13px; background: #ddd; color: #767676; font-family: Arial, Helvetica, sans-serif; line-height: 24px;'>" +
                        "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Đây là email gửi từ hệ thống AFL</p>" +
                        "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Vui lòng không trả lời trực tiếp qua email này</p>" +
                        "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>" +
                        "Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh" +
                        "</p>" +
                        "</footer></body></html>";

                }

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

        /// <summary>Send mail accept team to tournament</summary>
        /// <returns>Return success</returns>
        /// <response code="200">Returns success</response>
        /// <response code="400">Send mail fail</response>
        /// <response code="500">Internal Server</response>
        [HttpPost("send-mail-accept-team-to-tournament")]
        [Produces("application/json")]
        public async Task<ActionResult> SendMailAcceptTeamToTournament([FromBody] SendMailAcceptTeamToTournament modelRR)
        {
            try
            {
                Team team = await _teamService.GetByIdAsync(modelRR.TeamId);
                if (team == null)
                {
                    return NotFound("Đội bóng không tồn tại");
                }
                Tournament tournament = await _tournamentService.GetByIdAsync(modelRR.TournamentId);
                if(tournament == null)
                {
                    return NotFound("Giải đấu không tồn tại");
                }
                User userTeam = await _userService.GetByIdAsync(modelRR.TeamId);

                EmailForm model = new();

                
                if (modelRR.Status)
                {
                    model.ToEmail = userTeam.Email!;
                    model.Subject = "Thông Báo Chấp Nhận Tham Gia Giải Đấu";
                    model.Message = "<html><head></head><body>" +
                        "<header style='margin: 0px auto; margin-top: 20px; max-width: 590px; padding: 10px 50px; background-color: #00afab;'>" +
                        "<img src='https://afl-app-fe.vercel.app/assets/img/homepage/logo.png' alt='' class='logo' style='display: inline-flex; color: #fff; width: 60px;' width='60'/>" +
                        "</header>" +
                        "<div class='form_mail' style='font-family: Arial, Helvetica, sans-serif; padding: 50px; background-color: #f4f7f6; max-width: 590px; margin: 0px auto; line-height: 24px;'>" +

                        "<div class='title' style='display: block; font-size: 1.5em; margin-block-start: 0.83em; margin-block-end: 0.83em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Chấp nhận tham gia giải đấu</div>" +
                        "<p class='hello' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin chào quản lý đội bóng <b>" + team.TeamName + "</b>,</p>" +
                        "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                        "Giải đấu <b>" + tournament.TournamentName + "</b> đã chấp nhận yêu cầu tham gia của đội bạn. Hãy vào phần <b>Đội bóng của bạn</b> tại trang chủ AFL để biết thêm chi tiết về giải đấu." +
                        "</p>" +

                        "<a href='https://afl-app-fe.vercel.app/' style='background-color: #00afab; border-radius: 50px; padding: 10px 20px; display: inline-flex; color: #fff; font-weight: bold; text-decoration: none; text-transform: uppercase; font-size: 14px; margin-top: 10px;'>Trang chủ AFL</a>" +
                        "<p class='thank' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin cảm ơn,</p>" +
                        "<p class='thanks' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Amateur Football League</p>" +
                        "</div>" +
                        "<footer style='padding: 30px 50px; max-width: 590px; margin: 0px auto; font-size: 13px; background: #ddd; color: #767676; font-family: Arial, Helvetica, sans-serif; line-height: 24px;'>" +
                        "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Đây là email gửi từ hệ thống AFL</p>" +
                        "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Vui lòng không trả lời trực tiếp qua email này</p>" +
                        "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>" +
                        "Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh" +
                        "</p>" +
                        "</footer></body></html>";

                }
                else
                {
                    string reason = "";
                    if(!String.IsNullOrEmpty(modelRR.Reason))
                    {
                        reason = "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                            "Lý do: " + modelRR.Reason + "</p>";
                    }
                    model.ToEmail = userTeam.Email!;
                    model.Subject = "Thông Báo Từ Chối Tham Gia Giải Đấu";
                    model.Message = "<html><head></head><body>" +
                        "<header style='margin: 0px auto; margin-top: 20px; max-width: 590px; padding: 10px 50px; background-color: #00afab;'>" +
                        "<img src='https://afl-app-fe.vercel.app/assets/img/homepage/logo.png' alt='' class='logo' style='display: inline-flex; color: #fff; width: 60px;' width='60'/>" +
                        "</header>" +
                        "<div class='form_mail' style='font-family: Arial, Helvetica, sans-serif; padding: 50px; background-color: #f4f7f6; max-width: 590px; margin: 0px auto; line-height: 24px;'>" +

                        "<div class='title' style='display: block; font-size: 1.5em; margin-block-start: 0.83em; margin-block-end: 0.83em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Từ chối tham gia giải đấu</div>" +
                        "<p class='hello' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin chào quản lý đội bóng <b>" + team.TeamName + "</b>,</p>" +
                        "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                        "Giải đấu <b>" + tournament.TournamentName + "</b> đã từ chối yêu cầu tham gia của đội bạn." +
                        "</p>" + reason +
                        
                        "<a href='https://afl-app-fe.vercel.app/' style='background-color: #00afab; border-radius: 50px; padding: 10px 20px; display: inline-flex; color: #fff; font-weight: bold; text-decoration: none; text-transform: uppercase; font-size: 14px; margin-top: 10px;'>Trang chủ AFL</a>" +
                        "<p class='thank' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin cảm ơn,</p>" +
                        "<p class='thanks' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Amateur Football League</p>" +
                        "</div>" +
                        "<footer style='padding: 30px 50px; max-width: 590px; margin: 0px auto; font-size: 13px; background: #ddd; color: #767676; font-family: Arial, Helvetica, sans-serif; line-height: 24px;'>" +
                        "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Đây là email gửi từ hệ thống AFL</p>" +
                        "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Vui lòng không trả lời trực tiếp qua email này</p>" +
                        "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>" +
                        "Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh" +
                        "</p>" +
                        "</footer></body></html>";
                }

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

        /// <summary>Send mail ban</summary>
        /// <returns>Return success</returns>
        /// <response code="200">Returns success</response>
        /// <response code="400">Send mail fail</response>
        /// <response code="500">Internal Server</response>
        [HttpPost("send-mail-ban")]
        [Produces("application/json")]
        public async Task<ActionResult> SendMailBan(
            [FromQuery(Name = "user-id")] int userId
            )
        {
            try
            {
                User user = await _userService.GetByIdAsync(userId);

                EmailForm model = new();
                model.ToEmail = user.Email!;
                model.Subject = "Thông báo khóa tài khoản tài khoản A-Football-League";
                model.Message = "<html><head></head><body>" +
                    "<header style='margin: 0px auto; margin-top: 20px; max-width: 590px; padding: 10px 50px; background-color: #00afab;'>" +
                    "<img src='https://afl-app-fe.vercel.app/assets/img/homepage/logo.png' alt='' class='logo' style='display: inline-flex; color: #fff; width: 60px;' width='60'/>" +
                    "</header>" +
                    "<div class='form_mail' style='font-family: Arial, Helvetica, sans-serif; padding: 50px; background-color: #f4f7f6; max-width: 590px; margin: 0px auto; line-height: 24px;'>" +

                    "<div class='title' style='display: block; font-size: 1.5em; margin-block-start: 0.83em; margin-block-end: 0.83em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Khóa tài khoản</div>" +
                    "<p class='hello' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin chào <a href='mailto:" + user.Email + "'>" + user.Email + "</a>,</p>" +
                    "<p class='content' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>" +
                    "Tài khoản của bạn đã bị khóa <b>" + (user.DateUnban!.Value - user.DateBan!.Value).TotalDays.ToString() + " ngày</b> từ ngày <b>" + user.DateBan!.Value.ToString("dd/MM/yyyy") + "</b> đến hết ngày <b>" + user.DateUnban!.Value.ToString("dd/MM/yyyy") + "</b> do nhận quá nhiều báo cáo từ người dùng khác.</p>" +
                   
                    "<a href='https://afl-app-fe.vercel.app/' style='background-color: #00afab; border-radius: 50px; padding: 10px 20px; display: inline-flex; color: #fff; font-weight: bold; text-decoration: none; text-transform: uppercase; font-size: 14px; margin-top: 10px;'>Trang chủ AFL</a>" +
                    "<p class='thank' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px;'>Xin cảm ơn,</p>" +
                    "<p class='thanks' style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; font-weight: bold;'>Amateur Football League</p>" +
                    "</div>" +
                    "<footer style='padding: 30px 50px; max-width: 590px; margin: 0px auto; font-size: 13px; background: #ddd; color: #767676; font-family: Arial, Helvetica, sans-serif; line-height: 24px;'>" +
                    "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Đây là email gửi từ hệ thống AFL</p>" +
                    "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>Vui lòng không trả lời trực tiếp qua email này</p>" +
                    "<p style='display: block; margin-block-start: 1em; margin-block-end: 1em; margin-inline-start: 0px; margin-inline-end: 0px; margin: 0 0 2px 0;'>" +
                    "Lô E2a-7, Đường D1, Đ. D1, Long Thạnh Mỹ, Thành Phố Thủ Đức, Thành phố Hồ Chí Minh" +
                    "</p>" +
                    "</footer></body></html>";

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
