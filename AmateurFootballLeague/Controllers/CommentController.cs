using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
//using Microsoft.AspNetCore.Authentication.JwtBearer;
//using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentService _commentService;
        private readonly IMapper _mapper;
        private readonly ITournamentService _tournamentService;
        private readonly ITeamService _teamService;
        private readonly IUserService _userService;

        public CommentController(ICommentService commentService, IMapper mapper, ITournamentService tournamentService,
            ITeamService teamService, IUserService userService)
        {
            _commentService = commentService;
            _mapper = mapper;
            _tournamentService = tournamentService;
            _teamService = teamService;
            _userService = userService;
        }

        [HttpGet]
        public ActionResult<CommentVM> GetAllCommentIntournament(int? tounamentID, int? teamID, SortTypeEnum orderType, int pageIndex = 1, int limit = 5)
        {
            try
            {
                IQueryable<Comment> listComment = _commentService.GetList().Join(_userService.GetList(), c => c.User , u => u , (c, u) => new Comment
                {
                    Id = c.Id,
                    Content = c.Content,
                    DateCreate = c.DateCreate,
                    DateDelete = c.DateDelete,
                    DateUpdate = c.DateUpdate,
                    Status =c.Status,
                    TeamId = c.TeamId,
                    TournamentId = c.TournamentId,
                    UserId = u.Id,
                    User = new User
                    {
                        Id = u.Id,
                        Username = u.Username,
                        Avatar = u.Avatar 

                    }
                });
                if (tounamentID > 0)
                {
                    listComment = listComment.Where(c => c.TournamentId == tounamentID);
                }
                if (teamID > 0)
                {
                    listComment = listComment.Where(c => c.TeamId == teamID);
                }
                if (orderType == SortTypeEnum.DESC)
                {
                    listComment = listComment.OrderByDescending(c => c.Id);
                }
                    var commentListPagging = listComment.Skip((pageIndex - 1) * limit).Take(limit).ToList();
                int CountList = listComment.Count();

                var commentList = new CommentLV
                {
                    Comments = _mapper.Map<List<Comment>, List<CommentVM>>(commentListPagging),
                    CountList = CountList,
                    CurrentPage = pageIndex,
                    Size = limit
                };
                return Ok(commentList);

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<CommentVM>> CreateComment(int? tournamentID, int? teamID, CommentCM model)
        {
            Comment comment = new();
            try
            {
                User user = await _userService.GetByIdAsync(model.UserId);
                if (user == null)
                {
                    return BadRequest("Người dùng không tồn tại");
                }

                if (tournamentID == 0 && teamID == 0)
                {
                    return BadRequest();
                }
                if (tournamentID > 0)
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync((int)tournamentID);
                    if (tournament == null)
                    {
                        return BadRequest("Giải đấu không tồn tại");
                    }
                    comment.TournamentId = tournamentID;
                }
                if (teamID > 0)
                {
                    Team team = await _teamService.GetByIdAsync((int)teamID);
                    if (team == null)
                    {
                        return BadRequest("Đội bóng không tồn tại");
                    }
                    comment.TeamId = teamID;

                }


                comment.Content = model.Content!.Trim();
                comment.DateCreate = DateTime.Now.AddHours(7);
                comment.Status = "";
                comment.UserId = model.UserId;
                Comment commentCreated = await _commentService.AddAsync(comment);
                if (commentCreated != null)
                {
                    return Ok(_mapper.Map<CommentVM>(commentCreated));
                }
                return BadRequest("Bình luận thất bại");
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPut]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<CommentVM>> UpdateComment(CommentUM model)
        {
            try
            {
                Comment currentComment = await _commentService.GetByIdAsync(model.Id);
                if (currentComment == null)
                {
                    return NotFound("Bình luận không tồn tại");
                }
                currentComment.Content = model.Content!.Trim();
                currentComment.DateUpdate = DateTime.Now.AddHours(7);
                bool isUpdated = await _commentService.UpdateAsync(currentComment);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<CommentVM>(currentComment));
                }
                return BadRequest("Cập nhật bình luận thất bại");

            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpDelete]
        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<CommentVM>> DeleteComment(int id)
        {
            try
            {
                Comment currentComment = await _commentService.GetByIdAsync(id);
                if (currentComment != null)
                {
                    bool isDeleted = await _commentService.DeleteAsync(currentComment);
                    if (isDeleted)
                    {
                        return Ok("Xóa bình luận thành công");
                    }
                    return BadRequest();
                }
                return NotFound();
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
