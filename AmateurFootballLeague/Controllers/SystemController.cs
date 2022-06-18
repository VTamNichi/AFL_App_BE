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
                            model.Message = "<html><head></head><body><p style='font-size: 18px'>Xin chào quản lý đội bóng " + team.TeamName + ",</p><p style='font-size: 18px'>Cầu thủ " + footballPlayer.PlayerName + " đã chấp nhận tham gia đội bóng của bạn. Hãy vào phần <b>đội bóng của bạn</b> tại trang <a href='https://afl-app-fe.vercel.app/'>https://afl-app-fe.vercel.app/</a> để biết thêm chi tiết về cầu thủ.</p><p style='font-size: 18px'>Xin cảm ơn,<br>A-Football-League</p>";
                        }
                        else
                        {
                            model.ToEmail = userFP.Email!;
                            model.Subject = "Yêu Cầu Tham Gia Đội Bóng Thành Công";
                            model.Message = "<html><head></head><body><p style='font-size: 18px'>Xin chào cầu thủ " + footballPlayer.PlayerName + ",</p><p style='font-size: 18px'>Đội bóng " + team.TeamName + " đã chấp nhận yêu cầu tham gia của bạn. Hãy vào phần <b>thông tin cầu thủ</b> tại trang <a href='https://afl-app-fe.vercel.app/'>https://afl-app-fe.vercel.app/</a> để biết thêm chi tiết về đội bóng.</p><p style='font-size: 18px'>Xin cảm ơn,<br>A-Football-League</p>";
                        }
                    }
                    else
                    {
                        if (modelRR.Type == TypeRegisterOrRecruit.Register)
                        {
                            model.ToEmail = userTeam.Email!;
                            model.Subject = "Yêu Cầu Tham Gia Từ Cầu Thủ " + footballPlayer.PlayerName;
                            model.Message = "<html><head></head><body><p style='font-size: 18px'>Xin chào quản lý đội bóng " + team.TeamName + ",</p><p style='font-size: 18px'>Cầu thủ " + footballPlayer.PlayerName + " đã gửi lời mời tham gia đội bóng mà bạn quản lý. Hãy vào phần <b>đội bóng của bạn</b> tại trang <a href='https://afl-app-fe.vercel.app/'>https://afl-app-fe.vercel.app/</a> để xét duyệt cầu thủ.</p><p style='font-size: 18px'>Xin cảm ơn,<br>A-Football-League</p>";
                        }
                        else
                        {
                            model.ToEmail = userFP.Email!;
                            model.Subject = "Yêu Cầu Chiêu Mộ Từ Đội Bóng " + team.TeamName;
                            model.Message = "<html><head></head><body><p style='font-size: 18px'>Xin chào cầu thủ " + footballPlayer.PlayerName + ",</p><p style='font-size: 18px'>Đội bóng " + team.TeamName + " gửi lời mời chiêu mộ bạn vào đội. Hãy vào phần <b>thông tin cầu thủ</b> tại trang <a href='https://afl-app-fe.vercel.app/'>https://afl-app-fe.vercel.app/</a> để xét duyệt đội bóng.</p><p style='font-size: 18px'>Xin cảm ơn,<br>A-Football-League</p>";
                        }
                    }
                }
                else
                {
                    model.ToEmail = userFP.Email!;
                    model.Subject = "Thông báo tham gia giải đấu";
                    model.Message = "<html><head></head><body><p style='font-size: 18px'>Xin chào cầu thủ " + footballPlayer.PlayerName + ",</p><p style='font-size: 18px'>Quản lý đội bóng " + userTeam.Username + " đã đăng ký bạn vào giải đấu " + tournament.TournamentName + ". Hãy vào phần <b>thông tin cầu thủ</b> tại trang <a href='https://afl-app-fe.vercel.app/'>https://afl-app-fe.vercel.app/</a> để biết thêm chi tiết về giải đấu.</p><p style='font-size: 18px'>Xin cảm ơn,<br>A-Football-League</p>";

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
                    model.Message = "<html><head></head><body><p style='font-size: 18px'>Xin chào quản lý đội bóng " + team.TeamName + ",</p><p style='font-size: 18px'>Giải đấu " + tournament.TournamentName + " đã chấp nhận yêu cầu tham gia của đội bạn. Hãy vào phần <b>đội bóng của bạn</b> tại trang <a href='https://afl-app-fe.vercel.app/'>https://afl-app-fe.vercel.app/</a> để biết thêm chi tiết về giải đấu.</p><p style='font-size: 18px'>Xin cảm ơn,<br>A-Football-League</p>";

                }
                else
                {
                    string reason = "";
                    if(!String.IsNullOrEmpty(modelRR.Reason))
                    {
                        reason = "<p>Lý do: " + modelRR.Reason + "</p>";

                    }
                    model.ToEmail = userTeam.Email!;
                    model.Subject = "Thông Báo Từ Chối Tham Gia Giải Đấu";
                    model.Message = "<html><head></head><body><p style='font-size: 18px'>Xin chào quản lý đội bóng " + team.TeamName + ",</p><p style='font-size: 18px'>Giải đấu " + tournament.TournamentName + " đã từ chối yêu cầu tham gia của đội bạn.</p>" + reason + "<p style='font-size: 18px'>Xin cảm ơn,<br>A-Football-League</p>";
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
    }
}
