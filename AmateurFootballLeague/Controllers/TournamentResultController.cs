using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/tournament-results")]
    [ApiController]
    public class TournamentResultController : ControllerBase
    {
        private readonly ITournamentResultService _tournamentResultService;
        private readonly ITeamInTournamentService _teamInTournamentService;
        private readonly ITournamentService _tournamentService;
        private readonly IMapper _mapper;
        public TournamentResultController(ITournamentResultService tournamentResultService, ITeamInTournamentService teamInTournamentService, ITournamentService tournamentService, IMapper mapper)
        {
            _tournamentResultService = tournamentResultService;
            _teamInTournamentService = teamInTournamentService;
            _tournamentService = tournamentService;
            _mapper = mapper;
    }

        /// <summary>Get list tournament result</summary>
        /// <returns>List tournamentResult</returns>
        /// <response code="200">Returns list tournamentResult</response>
        /// <response code="404">Not found tournamentResult</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<TournamentResultListVM> GetListTournamentResult(
            [FromQuery(Name = "prize")] string? prize,
            [FromQuery(Name = "order-by")] TournamentResultFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<TournamentResult> tournamentResultList = _tournamentResultService.GetList();
                if (!String.IsNullOrEmpty(prize))
                {
                    tournamentResultList = tournamentResultList.Where(s => s.Prize!.ToUpper().Contains(prize.Trim().ToUpper()));
                }

                var tournamentResultListPaging = tournamentResultList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var tournamentResultListOrder = new List<TournamentResult>();
                if (orderBy == TournamentResultFieldEnum.Id)
                {
                    tournamentResultListOrder = tournamentResultListPaging.OrderBy(tnm => tnm.Id).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        tournamentResultListOrder = tournamentResultListPaging.OrderByDescending(tnm => tnm.Id).ToList();
                    }
                }
                if (orderBy == TournamentResultFieldEnum.Prize)
                {
                    tournamentResultListOrder = tournamentResultListPaging.OrderBy(tnm => tnm.Prize).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        tournamentResultListOrder = tournamentResultListPaging.OrderByDescending(tnm => tnm.Prize).ToList();
                    }
                }

                var tournamentResultListResponse = new TournamentResultListVM
                {
                    TournamentResults = _mapper.Map<List<TournamentResult>, List<TournamentResultVM>>(tournamentResultListOrder),
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(tournamentResultListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get tournament result by id</summary>
        /// <returns>Return the tournament result with the corresponding id</returns>
        /// <response code="200">Returns the tournament result with the specified id</response>
        /// <response code="404">No tournament result found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentResultVM>> GetTournamentResultById(int id)
        {
            try
            {
                TournamentResult currentTournamentResultVM = await _tournamentResultService.GetByIdAsync(id);
                if (currentTournamentResultVM != null)
                {
                    return Ok(_mapper.Map<TournamentResultVM>(currentTournamentResultVM));
                }
                return NotFound("Không tìm thấy kết quả giải đấu với id là " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new tournament result</summary>
        /// <response code="201">Created new tournament result successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentResultVM>> CreateTournamentResult([FromBody] TournamentResultCM model)
        {
            TournamentResult tournamentResult = _mapper.Map<TournamentResult>(model);
            try
            {
                Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId);
                if (tournament == null)
                {
                    return BadRequest("Giải đấu không tồn tại");
                }
                TeamInTournament teamInTournament = await _teamInTournamentService.GetByIdAsync(model.TeamInTournamentId);
                if (teamInTournament == null)
                {
                    return BadRequest("Đội bóng trong giải đấu không tồn tại");
                }

                tournamentResult.Prize = model.Prize!.Trim();
                tournamentResult.Description = String.IsNullOrEmpty(model.Description) ? "" : model.Description.Trim();
                tournamentResult.TeamInTournamentId = model.TeamInTournamentId;
                tournamentResult.TournamentId = model.TournamentId;

                TournamentResult tournamentResultCreated = await _tournamentResultService.AddAsync(tournamentResult);
                if (tournamentResultCreated != null)
                {
                    return CreatedAtAction("GetTournamentResultById", new { id = tournamentResultCreated.Id }, _mapper.Map<TournamentResultVM>(tournamentResultCreated));
                }
                return BadRequest("Tạo kết quả giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a tournament result</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentResultVM>> UpdateTournamentResult([FromBody] TournamentResultUM model)
        {
            TournamentResult currentTournamentResult = await _tournamentResultService.GetByIdAsync(model.Id);
            if (currentTournamentResult == null)
            {
                return NotFound("Không tìm thấy kết quả giải đấu với id là " + model.Id);
            }
            try
            {
                if(!String.IsNullOrEmpty(model.TournamentId.ToString()))
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync(model.TournamentId!.Value);
                    if (tournament == null)
                    {
                        return BadRequest("Giải đấu không tồn tại");
                    }
                    currentTournamentResult.TournamentId = model.TournamentId;
                }
                if(!String.IsNullOrEmpty(model.TeamInTournamentId.ToString()))
                {
                    TeamInTournament teamInTournament = await _teamInTournamentService.GetByIdAsync(model.TeamInTournamentId!.Value);
                    if (teamInTournament == null)
                    {
                        return BadRequest("Đội bóng trong giải đấu không tồn tại");
                    }
                    currentTournamentResult.TeamInTournamentId = model.TeamInTournamentId;
                }

                currentTournamentResult.Prize = String.IsNullOrEmpty(model.Prize) ? currentTournamentResult.Prize : model.Prize.Trim();
                currentTournamentResult.Description = String.IsNullOrEmpty(model.Description) ? currentTournamentResult.Description : model.Description.Trim();

                bool isUpdated = await _tournamentResultService.UpdateAsync(currentTournamentResult);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<TournamentResultVM>(currentTournamentResult));
                }
                return BadRequest("Cập nhật kết quả giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete tournament result By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            TournamentResult currentTournamentResultVM = await _tournamentResultService.GetByIdAsync(id);
            if (currentTournamentResultVM == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy kết quả giải đấu với id là " + id
                });
            }
            try
            {
                bool isDeleted = await _tournamentResultService.DeleteAsync(currentTournamentResultVM);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Xóa kết quả giải đấu thành công"
                    });
                }
                return BadRequest("Xóa kết quả giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
