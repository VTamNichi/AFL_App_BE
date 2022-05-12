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
    [Route("api/v1/teams")]
    [ApiController]
    public class TeamController : ControllerBase
    {
        private readonly ITeamService _teamService;
        private readonly IUserService _userService;
        private readonly ITeamInTournamentService _teamInTournamentService;
        private readonly ITournamentResultService _tournamentResultService;
        private readonly IUploadFileService _uploadFileService;
        private readonly IMapper _mapper;

        public TeamController(ITeamService teamService, IUserService userService, ITeamInTournamentService teamInTournamentService, ITournamentResultService tournamentResultService, IUploadFileService uploadFileService, IMapper mapper)
        {
            _teamService = teamService;
            _userService = userService;
            _teamInTournamentService = teamInTournamentService;
            _tournamentResultService = tournamentResultService;
            _uploadFileService = uploadFileService;
            _mapper = mapper;
        }

        /// <summary>Get list team</summary>
        /// <returns>List team</returns>
        /// <response code="200">Returns list team</response>
        /// <response code="404">Not found team</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<TeamListVM> GetListTeam(
            [FromQuery(Name = "team-name")] string? name,
            [FromQuery(Name = "team-area")] string? area,
            [FromQuery(Name = "team-gender")] TeamGenderEnum? gender,
            [FromQuery(Name = "order-by")] TeamFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<Team> teamList = _teamService.GetList();
                if (!String.IsNullOrEmpty(name))
                {
                    teamList = teamList.Where(s => s.TeamName.ToUpper().Contains(name.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(area))
                {
                    teamList = teamList.Where(s => s.TeamArea.ToUpper().Contains(area.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(gender.ToString()))
                {
                    teamList = teamList.Where(s => s.TeamGender.ToUpper().Contains(gender.ToString().Trim().ToUpper()));
                }
                var teamListPaging = teamList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var teamListOrder = new List<Team>();
                if (orderBy == TeamFieldEnum.Id)
                {
                    teamListOrder = teamListPaging.OrderBy(tnm => tnm.Id).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        teamListOrder = teamListPaging.OrderByDescending(tnm => tnm.Id).ToList();
                    }
                }
                if (orderBy == TeamFieldEnum.TeamName)
                {
                    teamListOrder = teamListPaging.OrderBy(tnm => tnm.TeamName).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        teamListOrder = teamListPaging.OrderByDescending(tnm => tnm.TeamName).ToList();
                    }
                }

                var teamListResponse = new TeamListVM
                {
                    Teams = _mapper.Map<List<TeamVM>>(teamListOrder),
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(teamListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get team by id</summary>
        /// <returns>Return the team with the corresponding id</returns>
        /// <response code="200">Returns the team with the specified id</response>
        /// <response code="404">No team found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<TeamVM>> GetTeamById(int id)
        {
            try
            {
                Team currentTeam = await _teamService.GetByIdAsync(id);
                if (currentTeam != null)
                {
                    return Ok(_mapper.Map<TeamVM>(currentTeam));
                }
                return NotFound("Can not found team by id: " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new team</summary>
        /// <response code="201">Created new team successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<TeamVM>> CreateTeam([FromForm] TeamCM model)
        {
            Team team = new Team();
            try
            {
                Team teamExist = await _teamService.GetByIdAsync(model.Id);
                if (teamExist != null)
                {
                    return BadRequest(new
                    {
                        message = "User has a team"
                    });
                }
                User user = await _userService.GetByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound(new
                    {
                        message = "User is not exist"
                    });
                }
                if (user.RoleId != 3)
                {
                    return BadRequest(new
                    {
                        message = "User is not team manager role"
                    });
                }

                bool isDuplicated = _teamService.GetList().Where(s => s.TeamName.Trim().ToUpper().Equals(model.TeamName.Trim().ToUpper())).FirstOrDefault() != null;
                if (isDuplicated)
                {
                    return BadRequest(new
                    {
                        message = "Team Name is duplicated"
                    });
                }
                try
                {
                    if (!String.IsNullOrEmpty(model.TeamAvatar.ToString()))
                    {
                        string fileUrl = await _uploadFileService.UploadFile(model.TeamAvatar, "images", "image-url");
                        team.TeamAvatar = fileUrl;
                    }
                }
                catch (Exception)
                {
                    team.TeamAvatar = "https://t3.ftcdn.net/jpg/03/46/83/96/360_F_346839683_6nAPzbhpSkIpb8pmAwufkC7c5eD7wYws.jpg";
                }
                team.TeamName = model.TeamName;
                team.TeamArea = String.IsNullOrEmpty(model.TeamArea) ? "" : model.TeamArea.Trim();
                team.TeamPhone = String.IsNullOrEmpty(model.TeamPhone) ? "" : model.TeamPhone.Trim();
                team.TeamGender = model.TeamGender == TeamGenderEnum.Male ? "Male" : model.TeamGender == TeamGenderEnum.Female ? "Female" : "Other";
                team.Description = String.IsNullOrEmpty(model.Description) ? "" : model.Description.Trim();
                team.DateCreate = DateTime.Now;
                team.Status = true;
                team.Id = model.Id;

                Team teamCreated = await _teamService.AddAsync(team);
                if (teamCreated != null)
                {
                    return CreatedAtAction("GetTeamById", new { id = teamCreated.Id }, _mapper.Map<TeamVM>(teamCreated));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a team</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<TeamVM>> UpdateTeam([FromForm] TeamUM model)
        {
            Team currentTeam = await _teamService.GetByIdAsync(model.Id);
            if (currentTeam == null)
            {
                return NotFound("Can not found team");
            }
            if (!String.IsNullOrEmpty(model.TeamName))
            {
                if (!currentTeam.TeamName.ToUpper().Equals(model.TeamName.ToUpper()) && _teamService.GetList().Where(s => s.TeamName.Trim().ToUpper().Equals(model.TeamName.Trim().ToUpper())).FirstOrDefault() != null)
                {
                    return BadRequest(new
                    {
                        message = "Team Name is duplicated!"
                    });
                }
            }
            try
            {
                try
                {
                    if (!String.IsNullOrEmpty(model.TeamAvatar.ToString()))
                    {
                        string fileUrl = await _uploadFileService.UploadFile(model.TeamAvatar, "images", "image-url");
                        currentTeam.TeamAvatar = fileUrl;
                    }
                }
                catch (Exception) { }
                
                currentTeam.TeamName = String.IsNullOrEmpty(model.TeamName) ? currentTeam.TeamName : model.TeamName.Trim();
                currentTeam.TeamArea = String.IsNullOrEmpty(model.TeamArea) ? currentTeam.TeamArea : model.TeamArea.Trim();
                currentTeam.TeamPhone = String.IsNullOrEmpty(model.TeamPhone) ? currentTeam.TeamPhone : model.TeamPhone.Trim();
                currentTeam.TeamGender = model.TeamGender == TeamGenderEnum.Male ? "Male" : model.TeamGender == TeamGenderEnum.Female ? "Female" : model.TeamGender == TeamGenderEnum.Other ? "Other" : currentTeam.TeamGender;
                currentTeam.Description = String.IsNullOrEmpty(model.Description) ? currentTeam.Description : model.Description.Trim();
                currentTeam.DateUpdate = DateTime.Now;

                bool isUpdated = await _teamService.UpdateAsync(currentTeam);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<TeamVM>(currentTeam));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Change status team By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("{id}")]
        public async Task<ActionResult> ChangeStatusTeam(int id)
        {
            Team currentTeam = await _teamService.GetByIdAsync(id);
            if (currentTeam == null)
            {
                return NotFound(new
                {
                    message = "Can not found team by id: " + id
                });
            }
            try
            {
                currentTeam.Status = !currentTeam.Status;
                bool isDeleted = await _teamService.UpdateAsync(currentTeam);
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

        /// <summary>Delete team By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            Team currentTeam = await _teamService.GetByIdAsync(id);
            if (currentTeam == null)
            {
                return NotFound(new
                {
                    message = "Can not found team by id: " + id
                });
            }
            try
            {
                currentTeam.Status = false;
                currentTeam.DateDelete = DateTime.Now;
                bool isDeleted = await _teamService.UpdateAsync(currentTeam);
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

        /// <summary>Get count all team</summary>
        /// <returns>Return the count of team with the corresponding id</returns>
        /// <response code="200">Returns the count of team with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("count")]
        [Produces("application/json")]
        public async Task<ActionResult<int>> GetCountAllTeam()
        {
            try
            {
                return Ok(_teamService.CountAllTeam());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get top 4 teams by achievement</summary>
        /// <returns>Return top 4 teams by achievement</returns>
        /// <response code="200">Returns top 4 teams by achievement</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("top4")]
        [Produces("application/json")]
        public async Task<ActionResult<List<TeamVM>>> GetTop4Team()
        {
            try
            {
                var top4ID = _teamInTournamentService.GetList().Join(_tournamentResultService.GetList(), tit => tit.Id, ts => ts.TeamInTournamentId, (tit, ts) => new { tit.TeamId })
                          .GroupBy(t => t.TeamId).Select(g => new { teamId = g.Key, count = g.Count()}).OrderByDescending(t => t.count).Take(4).ToList();
                List<TeamVM> listTeamVM = new List<TeamVM>();
                foreach (var tid in top4ID)
                {
                    TeamVM teamVM = _mapper.Map<TeamVM>(await _teamService.GetByIdAsync((int) tid.teamId));
                    listTeamVM.Add(teamVM);
                }

                return Ok(listTeamVM);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
