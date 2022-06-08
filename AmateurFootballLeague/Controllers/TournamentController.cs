﻿using AmateurFootballLeague.ExternalService;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/tournaments")]
    [ApiController]
    //[Authorize(Roles = "ADMIN")]
    public class TournamentController : ControllerBase
    {
        private readonly ITournamentService _tournamentService;
        private readonly IUserService _userService;
        private readonly ITeamInTournamentService _teamInTournamentService;
        private readonly IUploadFileService _uploadFileService;
        private readonly IMapper _mapper;
        private readonly IPlayerInTeamService _playerInTeamService;
        private readonly IPlayerInTournamentService _playerInTournamentService;
        private readonly IFootballPlayerService _footballPlayerService;

        public TournamentController(ITournamentService tournamentService, IUserService userService, ITeamInTournamentService teamInTournamentService, IUploadFileService uploadFileService, IMapper mapper,
            IPlayerInTeamService playerInTeamService, IPlayerInTournamentService playerInTournamentService, IFootballPlayerService footballPlayerService)
        {
            _tournamentService = tournamentService;
            _userService = userService;
            _teamInTournamentService = teamInTournamentService;
            _uploadFileService = uploadFileService;
            _mapper = mapper;
            _playerInTeamService = playerInTeamService;
            _playerInTournamentService = playerInTournamentService;
            _footballPlayerService = footballPlayerService;
        }

        /// <summary>Get list tournament</summary>
        /// <returns>List tournament</returns>
        /// <response code="200">Returns list tournament</response>
        /// <response code="404">Not found tournament</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<TournamentListVM> GetListTournament(
            [FromQuery(Name = "tournament-name")] string? name,
            [FromQuery(Name = "userId")] int? userId,
            [FromQuery(Name = "tournament-area")] string? area,
            [FromQuery(Name = "tournament-mode")] TournamentModeEnum? mode,
            [FromQuery(Name = "tournament-type")] TournamentTypeEnum? type,
            [FromQuery(Name = "tournament-gender")] TournamentGenderEnum? gender,
            [FromQuery(Name = "tournament-football-type")] TournamentFootballFieldTypeEnum? footballType,
            [FromQuery(Name = "order-by")] TournamentFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<Tournament> tournamentList = _tournamentService.GetList();
                if (!String.IsNullOrEmpty(name))
                {
                    tournamentList = tournamentList.Where(s => s.TournamentName.ToUpper().Contains(name.Trim().ToUpper()));
                }
                if (userId != null)
                {
                    tournamentList = tournamentList.Where(s => s.UserId == userId);
                }
                if (!String.IsNullOrEmpty(area))
                {
                    tournamentList = tournamentList.Where(s => s.FootballFieldAddress.ToUpper().Contains(area.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(mode.ToString()))
                {
                    tournamentList = tournamentList.Where(s => s.Mode.ToUpper().Equals(mode.ToString().Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(type.ToString()))
                {
                    tournamentList = tournamentList.Where(s => s.TournamentType.TournamentTypeName.ToUpper().Contains(type.ToString().Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(gender.ToString()))
                {
                    tournamentList = tournamentList.Where(s => s.TournamentGender.ToUpper().Equals(gender.ToString().Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(footballType.ToString()))
                {
                    tournamentList = tournamentList.Where(s => s.FootballFieldType.FootballFieldTypeName.ToUpper().Contains(footballType.ToString().Trim().ToUpper()));
                }
                tournamentList = tournamentList.Where(s => s.Status == true);

                int countList = tournamentList.Count();

                if (orderBy == TournamentFieldEnum.TournamentName)
                {
                    tournamentList = tournamentList.OrderBy(tnm => tnm.TournamentName);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        tournamentList = tournamentList.OrderByDescending(tnm => tnm.TournamentName);
                    }
                }
                if (orderBy == TournamentFieldEnum.Mode)
                {
                    tournamentList = tournamentList.OrderBy(tnm => tnm.Mode);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        tournamentList = tournamentList.OrderByDescending(tnm => tnm.Mode);
                    }
                }
                if (orderBy == TournamentFieldEnum.DateCreate)
                {
                    tournamentList = tournamentList.OrderBy(tnm => tnm.DateCreate);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        tournamentList = tournamentList.OrderByDescending(tnm => tnm.DateCreate);
                    }
                }

                var tournamentListVM = tournamentList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                List<TournamentVM> listTournamentVM = new List<TournamentVM>();
                listTournamentVM = _mapper.Map<List<TournamentVM>>(tournamentListVM);
                foreach (var tournamentVM in listTournamentVM)
                {
                    tournamentVM.NumberTeamInTournament = _teamInTournamentService.CountTeamInATournament(tournamentVM.Id);
                }

                var tournamentListResponse = new TournamentListVM
                {
                    Tournaments = listTournamentVM,
                    CurrentPage = pageIndex,
                    CountList = countList,
                    Size = limit
                };

                return Ok(tournamentListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpGet("FootballPlayerID")]
        public async Task<ActionResult<TournamentListVM>> getAllTournamentOfFootballPlayer(int footballPlayerID, SortTypeEnum sort, int pageIndex = 1, int limit = 5)
        {
            try
            {
                IQueryable<Tournament> listTournament = _tournamentService.GetList().Join(_teamInTournamentService.GetList(), t => t.Id, tit => tit.TournamentId
                , (t, tit) => new { t, tit }).Join(_playerInTournamentService.GetList(), tpit => tpit.tit.Id, ptour => ptour.TeamInTournamentId, (tpit, ptour) =>
                     new { tpit, ptour }).Join(_playerInTeamService.GetList(), pitour => pitour.ptour.PlayerInTeam, pit => pit, (pitour, pit) =>
                          new { pitour, pit }).Where(pit => pit.pit.FootballPlayerId == footballPlayerID).Join(_footballPlayerService.GetList(), fpit => fpit.pit.FootballPlayerId, f => f.Id, (fpit, f) => new Tournament
                          {
                              Id = fpit.pitour.tpit.t.Id,
                              TournamentName = fpit.pitour.tpit.t.TournamentName,
                              Mode = fpit.pitour.tpit.t.Mode,
                              TournamentPhone = fpit.pitour.tpit.t.TournamentPhone,
                              TournamentGender = fpit.pitour.tpit.t.TournamentGender,
                              RegisterEndDate = fpit.pitour.tpit.t.RegisterEndDate,
                              TournamentStartDate= fpit.pitour.tpit.t.TournamentStartDate,
                              TournamentEndDate= fpit.pitour.tpit.t.TournamentEndDate,
                              FootballFieldAddress = fpit.pitour.tpit.t.FootballFieldAddress,
                              TournamentAvatar = fpit.pitour.tpit.t.TournamentAvatar,
                              Description = fpit.pitour.tpit.t.Description,
                              MatchMinutes= fpit.pitour.tpit.t.MatchMinutes,
                              FootballTeamNumber= fpit.pitour.tpit.t.FootballTeamNumber,
                              FootballPlayerMaxNumber= fpit.pitour.tpit.t.FootballPlayerMaxNumber,
                              GroupNumber= fpit.pitour.tpit.t.GroupNumber,
                              DateCreate= fpit.pitour.tpit.t.DateCreate,
                              Status= fpit.pitour.tpit.t.Status
                              
                          });
                if (sort == SortTypeEnum.DESC)
                {
                    listTournament = listTournament.OrderByDescending(t => t.Id);
                }
                var tournamentListPagging = listTournament.Skip((pageIndex - 1) * limit).Take(limit).ToList();
                var tournamentListResponse = new TournamentListVM
                {
                    Tournaments = _mapper.Map<List<Tournament>, List<TournamentVM>>(tournamentListPagging),
                    CurrentPage = pageIndex,
                    Size = limit
                };
                return Ok(tournamentListResponse);
            }
            catch
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>Get tournament by id</summary>
        /// <returns>Return the tournament with the corresponding id</returns>
        /// <response code="200">Returns the tournament with the specified id</response>
        /// <response code="404">No tournament found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentVM>> GetTournamentById(int id)
        {
            try
            {
                Tournament currentTournament = await _tournamentService.GetByIdAsync(id);
                if (currentTournament == null)
                {
                    return NotFound("Không tìm thấy giải đấu với id là " + id);
                }
                TournamentVM tournamentVM = _mapper.Map<TournamentVM>(currentTournament);
                tournamentVM.NumberTeamInTournament = _teamInTournamentService.CountTeamInATournament(id);
                if (currentTournament != null)
                {
                    return Ok(tournamentVM);
                }
                return NotFound("Không tìm thấy giải đấu với id là " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


        /// <summary>Create a new tournament</summary>
        /// <response code="201">Created new tournament successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentVM>> CreateTournament([FromForm] TournamentCM model)
        {
            Tournament tournament = new Tournament();
            try
            {

                User user = await _userService.GetByIdAsync(model.UserId);
                if(user.RoleId != 2)
                {
                    return BadRequest(new
                    {
                        message = "Người dùng không có vai trò chủ giải đấu"
                    });
                }

                bool isDuplicated = _tournamentService.GetList().Where(s => s.TournamentName.Trim().ToUpper().Equals(model.TournamentName.Trim().ToUpper())).FirstOrDefault() != null;
                if (isDuplicated)
                {
                    return BadRequest(new
                    {
                        message = "Tên giải đấu đã tồn tại"
                    });
                }
                tournament.TournamentName = model.TournamentName;
                tournament.Mode = "PUBLIC";
                if (model.Mode == TournamentModeEnum.PRIVATE)
                {
                    tournament.Mode = "PRIVATE";
                }
                try
                {
                    if (!String.IsNullOrEmpty(model.TournamentAvatar.ToString()))
                    {
                        string fileUrl = await _uploadFileService.UploadFile(model.TournamentAvatar, "images", "image-url");
                        tournament.TournamentAvatar = fileUrl;
                    }
                }
                catch (Exception) 
                {
                    tournament.TournamentAvatar = "https://t3.ftcdn.net/jpg/03/46/83/96/360_F_346839683_6nAPzbhpSkIpb8pmAwufkC7c5eD7wYws.jpg";
                }
                tournament.TournamentPhone = String.IsNullOrEmpty(model.TournamentPhone) ? "" : model.TournamentPhone.Trim();
                tournament.TournamentGender = model.TournamentGender == TournamentGenderEnum.Male ? "Male" : model.TournamentGender == TournamentGenderEnum.Female ? "Female" : "Other";
                tournament.RegisterEndDate = String.IsNullOrEmpty(model.RegisterEndDate.ToString()) ? DateTime.Now : model.RegisterEndDate;
                tournament.TournamentStartDate = String.IsNullOrEmpty(model.TournamentStartDate.ToString()) ? DateTime.Now : model.TournamentStartDate;
                tournament.TournamentEndDate = String.IsNullOrEmpty(model.TournamentEndDate.ToString()) ? DateTime.Now : model.TournamentEndDate;
                tournament.FootballFieldAddress = String.IsNullOrEmpty(model.FootballFieldAddress) ? "" : model.FootballFieldAddress;
                tournament.Description = String.IsNullOrEmpty(model.Description) ? "" : model.Description.Trim();
                tournament.MatchMinutes = model.MatchMinutes;
                tournament.FootballTeamNumber = model.FootballTeamNumber;
                tournament.FootballPlayerMaxNumber = model.FootballPlayerMaxNumber;
                tournament.GroupNumber = String.IsNullOrEmpty(model.GroupNumber.ToString()) ? 0 : model.GroupNumber;
                tournament.UserId = model.UserId;
                tournament.TournamentTypeId = model.TournamentTypeEnum == TournamentTypeEnum.KnockoutStage ? 1 : model.TournamentTypeEnum == TournamentTypeEnum.CircleStage ? 2 : 3;
                tournament.FootballFieldTypeId = model.TournamentFootballFieldTypeEnum == TournamentFootballFieldTypeEnum.Field5 ? 1 : model.TournamentFootballFieldTypeEnum == TournamentFootballFieldTypeEnum.Field7 ? 2 : 3;
                tournament.DateCreate = DateTime.Now;
                tournament.Status = true;
                tournament.StatusTnm = "";

                Tournament tournamentCreated = await _tournamentService.AddAsync(tournament);
                if (tournamentCreated != null)
                {
                    return CreatedAtAction("GetTournamentById", new { id = tournamentCreated.Id }, _mapper.Map<TournamentVM>(tournamentCreated));
                }
                return BadRequest("Tạo giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a tournament</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentVM>> UpdateTournament([FromForm] TournamentUM model)
        {
            Tournament currentTournament = await _tournamentService.GetByIdAsync(model.Id);
            if (currentTournament == null)
            {
                return NotFound("Không tìm thấy giải đấu với id là " + model.Id);
            }
            if (!String.IsNullOrEmpty(model.TournamentName))
            {
                if (!currentTournament.TournamentName.ToUpper().Equals(model.TournamentName.ToUpper()) && _tournamentService.GetList().Where(s => s.TournamentName.Trim().ToUpper().Equals(model.TournamentName.Trim().ToUpper())).FirstOrDefault() != null)
                {
                    return BadRequest(new
                    {
                        message = "Tên giải đấu đã tồn tại"
                    });
                }
            }
            try
            {
                try
                {
                    if (!String.IsNullOrEmpty(model.TournamentAvatar.ToString()))
                    {
                        string fileUrl = await _uploadFileService.UploadFile(model.TournamentAvatar, "images", "image-url");
                        currentTournament.TournamentAvatar = fileUrl;
                    }
                }
                catch (Exception) { }

                currentTournament.TournamentPhone = String.IsNullOrEmpty(model.TournamentPhone) ? currentTournament.TournamentPhone : model.TournamentPhone.Trim();
                currentTournament.TournamentGender = model.TournamentGender == TournamentGenderEnum.Male ? "Male" : model.TournamentGender == TournamentGenderEnum.Female ? "Female" : currentTournament.TournamentGender;
                currentTournament.TournamentName = String.IsNullOrEmpty(model.TournamentName) ? currentTournament.TournamentName : model.TournamentName.Trim();
                currentTournament.Mode = model.Mode == TournamentModeEnum.PUBLIC ? "PUBLIC" : model.Mode == TournamentModeEnum.PRIVATE ? "PRIVATE" : currentTournament.Mode;
                currentTournament.RegisterEndDate = String.IsNullOrEmpty(model.RegisterEndDate.ToString()) ? currentTournament.RegisterEndDate : model.RegisterEndDate;
                currentTournament.TournamentStartDate = String.IsNullOrEmpty(model.TournamentStartDate.ToString()) ? currentTournament.TournamentStartDate : model.TournamentStartDate;
                currentTournament.TournamentEndDate = String.IsNullOrEmpty(model.TournamentEndDate.ToString()) ? currentTournament.TournamentEndDate : model.TournamentEndDate;
                currentTournament.FootballFieldAddress = String.IsNullOrEmpty(model.FootballFieldAddress) ? currentTournament.FootballFieldAddress : model.FootballFieldAddress.Trim();
                currentTournament.GroupNumber = String.IsNullOrEmpty(model.GroupNumber.ToString()) ? currentTournament.GroupNumber : model.GroupNumber;
                currentTournament.Description = String.IsNullOrEmpty(model.Description) ? currentTournament.Description : model.Description.Trim();
                currentTournament.MatchMinutes = String.IsNullOrEmpty(model.MatchMinutes.ToString()) ? currentTournament.MatchMinutes : model.MatchMinutes;
                currentTournament.FootballTeamNumber = String.IsNullOrEmpty(model.FootballTeamNumber.ToString()) ? currentTournament.FootballTeamNumber : model.FootballTeamNumber;
                currentTournament.FootballPlayerMaxNumber = String.IsNullOrEmpty(model.FootballPlayerMaxNumber.ToString()) ? currentTournament.FootballPlayerMaxNumber : model.FootballPlayerMaxNumber;
                currentTournament.TournamentTypeId = model.TournamentTypeEnum == TournamentTypeEnum.KnockoutStage ? 1 : model.TournamentTypeEnum == TournamentTypeEnum.CircleStage ? 2 : model.TournamentTypeEnum == TournamentTypeEnum.GroupStage ? 3 : currentTournament.TournamentTypeId;
                currentTournament.FootballFieldTypeId = model.TournamentFootballFieldTypeEnum == TournamentFootballFieldTypeEnum.Field5 ? 1 : model.TournamentFootballFieldTypeEnum == TournamentFootballFieldTypeEnum.Field7 ? 2 : model.TournamentFootballFieldTypeEnum == TournamentFootballFieldTypeEnum.Field11 ? 3 : currentTournament.FootballFieldTypeId;
                currentTournament.DateUpdate = DateTime.Now;

                bool isUpdated = await _tournamentService.UpdateAsync(currentTournament);
                if (isUpdated)
                {
                    return CreatedAtAction("GetTournamentById", new { id = currentTournament.Id }, _mapper.Map<TournamentVM>(currentTournament));
                }
                return BadRequest("Cập nhật giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Change status tournament By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("{id}")]
        public async Task<ActionResult> ChangeStatusTournament(int id)
        {
            Tournament currentTournament = await _tournamentService.GetByIdAsync(id);
            if (currentTournament == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy giải đấu với id là " + id
                });
            }
            try
            {
                currentTournament.Status = !currentTournament.Status;
                bool isDeleted = await _tournamentService.UpdateAsync(currentTournament);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Thay đổi trạng thái giải đấu thành công"
                    });
                }
                return BadRequest("Thay đổi trạng thái giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete tournament By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            Tournament currentTournament = await _tournamentService.GetByIdAsync(id);
            if (currentTournament == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy giải đấu với id là " + id
                });
            }
            try
            {
                currentTournament.Status = false;
                currentTournament.DateDelete = DateTime.Now;
                bool isDeleted = await _tournamentService.UpdateAsync(currentTournament);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Xóa giải đấu thành công"
                    });
                }
                return BadRequest("Xóa giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get count all tournament</summary>
        /// <returns>Return the count of tournament with the corresponding id</returns>
        /// <response code="200">Returns the count of tournament with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet("count")]
        [Produces("application/json")]
        public async Task<ActionResult<int>> GetCountAllTournament()
        {
            try
            {
                return Ok(_tournamentService.CountAllTournament());
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
