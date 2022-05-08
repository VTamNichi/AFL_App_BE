using AmateurFootballLeague.ExternalService;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/matchs")]
    [ApiController]
    public class MatchController : ControllerBase
    {
        private readonly IMatchService _matchService;
        private readonly ITournamentService _tournamentService;
        private readonly IMapper _mapper;

        public MatchController(IMatchService matchService, ITournamentService tournamentService, IMapper mapper)
        {
            _matchService = matchService;
            _tournamentService = tournamentService;
            _mapper = mapper;
        }

        /// <summary>Get list match</summary>
        /// <returns>List match</returns>
        /// <response code="200">Returns list match</response>
        /// <response code="404">Not found match</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<MatchListVM> GetListMatch(
            [FromQuery(Name = "match-date")] string? matchDate,
            [FromQuery(Name = "order-by")] MatchFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<Match> matchList = _matchService.GetList();
                if (!String.IsNullOrEmpty(matchDate))
                {
                    matchList = matchList.Where(s => s.MatchDate.Equals(matchDate));
                }

                var matchListPaging = matchList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var matchListOrder = new List<Match>();
                if (orderBy == MatchFieldEnum.Id)
                {
                    matchListOrder = matchListPaging.OrderBy(tnm => tnm.Id).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        matchListOrder = matchListPaging.OrderByDescending(tnm => tnm.Id).ToList();
                    }
                }
                if (orderBy == MatchFieldEnum.MatchDate)
                {
                    matchListOrder = matchListPaging.OrderBy(tnm => tnm.MatchDate).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        matchListOrder = matchListPaging.OrderByDescending(tnm => tnm.MatchDate).ToList();
                    }
                }
                if (orderBy == MatchFieldEnum.Status)
                {
                    matchListOrder = matchListPaging.OrderBy(tnm => tnm.Status).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        matchListOrder = matchListPaging.OrderByDescending(tnm => tnm.Status).ToList();
                    }
                }

                var matchListResponse = new MatchListVM
                {
                    Matchs = _mapper.Map<List<Match>, List<MatchVM>>(matchListOrder),
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(matchListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get match by id</summary>
        /// <returns>Return the match with the corresponding id</returns>
        /// <response code="200">Returns the match with the specified id</response>
        /// <response code="404">No match found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<MatchVM>> GetMatchById(int id)
        {
            try
            {
                Match currentMatch = await _matchService.GetByIdAsync(id);
                if (currentMatch != null)
                {
                    return Ok(_mapper.Map<MatchVM>(currentMatch));
                }
                return NotFound("Can not found match by id: " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new match</summary>
        /// <response code="201">Created new match successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<MatchVM>> CreateMatch(
                [FromQuery(Name = "match-date")] DateTime matchDate,
                [FromQuery(Name = "status")] MatchStatusEnum status,
                [FromQuery(Name = "tournament-id")] int tournamentID
            )
        {
            Match match = new Match();
            try
            {
                Tournament tournament = await _tournamentService.GetByIdAsync(tournamentID);
                if(tournament == null)
                {
                    return BadRequest("Tournament not exist");
                }
                match.TournamentId = tournamentID;
                match.MatchDate = matchDate;
                match.Status = status == MatchStatusEnum.NotStart ? "Not start" : status == MatchStatusEnum.Processing ? "Processing" : "Finished";

                Match matchCreated = await _matchService.AddAsync(match);
                if (matchCreated != null)
                {
                    return CreatedAtAction("GetMatchById", new { id = matchCreated.Id }, _mapper.Map<MatchVM>(matchCreated));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a match</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<MatchVM>> UpdateMatch(
                [FromQuery(Name = "match-date")] DateTime? matchDate,
                [FromQuery(Name = "status")] MatchStatusEnum? status,
                [FromQuery(Name = "tournament-id")] int? tournamentID
            )
        {
            Match match = new Match();
            try
            {
                if (!String.IsNullOrEmpty(tournamentID.ToString()))
                {
                    Tournament tournament = await _tournamentService.GetByIdAsync((int)tournamentID);
                    if (tournament == null)
                    {
                        return BadRequest("Tournament not exist");
                    } else
                    {
                        match.TournamentId = (int)tournamentID;
                    }
                }
                match.MatchDate = String.IsNullOrEmpty(matchDate.ToString()) ? match.MatchDate : matchDate;
                match.Status = status == MatchStatusEnum.NotStart ? "Not start" : status == MatchStatusEnum.Processing ? "Processing" : status == MatchStatusEnum.Finished ? "Finished" : match.Status;

                bool isUpdated = await _matchService.UpdateAsync(match);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<MatchVM>(match));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete match By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            Match currentMatch = await _matchService.GetByIdAsync(id);
            if (currentMatch == null)
            {
                return NotFound(new
                {
                    message = "Can not found match by id: " + id
                });
            }
            try
            {
                bool isDeleted = await _matchService.DeleteAsync(currentMatch);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Success"
                    });
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
