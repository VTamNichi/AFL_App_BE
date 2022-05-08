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
    [Route("api/v1/football-players")]
    [ApiController]
    public class FootballPlayerController : ControllerBase
    {
        private readonly IFootballPlayerService _footballPlayerService;
        private readonly IUploadFileService _uploadFileService;
        private readonly IMapper _mapper;

        public FootballPlayerController(IFootballPlayerService footballPlayerService, IUploadFileService uploadFileService, IMapper mapper)
        {
            _footballPlayerService = footballPlayerService;
            _uploadFileService = uploadFileService;
            _mapper = mapper;
        }

        /// <summary>Get list football player</summary>
        /// <returns>List football player</returns>
        /// <response code="200">Returns list football player</response>
        /// <response code="404">Not found football player</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<FootballPlayerListVM> GetListFootballPlayer(
            [FromQuery(Name = "football-player-name")] string? name,
            [FromQuery(Name = "order-by")] FootballPlayerFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<FootballPlayer> footballPlayerList = _footballPlayerService.GetList();
                if (!String.IsNullOrEmpty(name))
                {
                    footballPlayerList = footballPlayerList.Where(s => s.PlayerName.ToUpper().Contains(name.Trim().ToUpper()));
                }

                var footballPlayerListPaging = footballPlayerList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var footballPlayerListOrder = new List<FootballPlayer>();
                if (orderBy == FootballPlayerFieldEnum.Id)
                {
                    footballPlayerListOrder = footballPlayerListPaging.OrderBy(tnm => tnm.Id).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        footballPlayerListOrder = footballPlayerListPaging.OrderByDescending(tnm => tnm.Id).ToList();
                    }
                }
                if (orderBy == FootballPlayerFieldEnum.Email)
                {
                    footballPlayerListOrder = footballPlayerListPaging.OrderBy(tnm => tnm.Email).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        footballPlayerListOrder = footballPlayerListPaging.OrderByDescending(tnm => tnm.Email).ToList();
                    }
                }
                if (orderBy == FootballPlayerFieldEnum.PlayerName)
                {
                    footballPlayerListOrder = footballPlayerListPaging.OrderBy(tnm => tnm.PlayerName).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        footballPlayerListOrder = footballPlayerListPaging.OrderByDescending(tnm => tnm.PlayerName).ToList();
                    }
                }
                if (orderBy == FootballPlayerFieldEnum.DateOfBirth)
                {
                    footballPlayerListOrder = footballPlayerListPaging.OrderBy(tnm => tnm.DateOfBirth).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        footballPlayerListOrder = footballPlayerListPaging.OrderByDescending(tnm => tnm.DateOfBirth).ToList();
                    }
                }
                if (orderBy == FootballPlayerFieldEnum.ClothersNumber)
                {
                    footballPlayerListOrder = footballPlayerListPaging.OrderBy(tnm => tnm.ClothersNumber).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        footballPlayerListOrder = footballPlayerListPaging.OrderByDescending(tnm => tnm.ClothersNumber).ToList();
                    }
                }

                var footballPlayerListResponse = new FootballPlayerListVM
                {
                    FootballPlayers = _mapper.Map<List<FootballPlayer>, List<FootballPlayerVM>>(footballPlayerListOrder),
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(footballPlayerListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get football player by id</summary>
        /// <returns>Return the football player with the corresponding id</returns>
        /// <response code="200">Returns the football player with the specified id</response>
        /// <response code="404">No football player found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<FootballPlayerVM>> GetFootballPlayerById(int id)
        {
            try
            {
                FootballPlayer currentFootballPlayer = await _footballPlayerService.GetByIdAsync(id);
                if (currentFootballPlayer != null)
                {
                    return Ok(_mapper.Map<FootballPlayerVM>(currentFootballPlayer));
                }
                return NotFound("Can not found football player by id: " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new football player</summary>
        /// <response code="201">Created new football player successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<FootballPlayerVM>> CreateFootballPlayer([FromForm] FootballPlayerCM model)
        {
            FootballPlayer footballPlayer = new FootballPlayer();
            try
            {
                footballPlayer.Email = model.Email;
                footballPlayer.PlayerName = model.PlayerName;
                if (!String.IsNullOrEmpty(model.PlayerAvatar.ToString()))
                {
                    string fileUrl = await _uploadFileService.UploadFile(model.PlayerAvatar, "service", "service-detail");
                    footballPlayer.PlayerAvatar = fileUrl;
                }
                else
                {
                    footballPlayer.PlayerAvatar = "data:image/jpeg;base64,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";
                }
                if (!String.IsNullOrEmpty(model.DateOfBirth.ToString()))
                {
                    footballPlayer.DateOfBirth = model.DateOfBirth;
                }
                footballPlayer.Gender = model.Gender == FootballPlayerGenderEnum.Male ? "Male" : model.Gender == FootballPlayerGenderEnum.Female ? "Female" : "Other";
                footballPlayer.DateCreate = DateTime.Now;
                footballPlayer.Status = true;

                FootballPlayer footballPlayerCreated = await _footballPlayerService.AddAsync(footballPlayer);
                if (footballPlayerCreated != null)
                {
                    return CreatedAtAction("GetFootballPlayerById", new { id = footballPlayerCreated.Id }, _mapper.Map<FootballPlayerVM>(footballPlayerCreated));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a football player</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<FootballPlayerVM>> UpdateFootballPlayer([FromForm] FootballPlayerUM model)
        {
            FootballPlayer currentFootballPlayer = await _footballPlayerService.GetByIdAsync(model.Id);
            if (currentFootballPlayer == null)
            {
                return NotFound("Can not found football player");
            }
            try
            {
                if (!String.IsNullOrEmpty(model.PlayerAvatar.ToString()))
                {
                    string fileUrl = await _uploadFileService.UploadFile(model.PlayerAvatar, "service", "service-detail");
                    currentFootballPlayer.PlayerAvatar = fileUrl;
                }
                currentFootballPlayer.Email = String.IsNullOrEmpty(model.Email) ? currentFootballPlayer.Email : model.Email.Trim();
                currentFootballPlayer.PlayerName = String.IsNullOrEmpty(model.PlayerName) ? currentFootballPlayer.PlayerName : model.PlayerName.Trim();
                currentFootballPlayer.Gender = model.Gender == FootballPlayerGenderEnum.Male ? "Male" : model.Gender == FootballPlayerGenderEnum.Female ? "Female" : model.Gender == FootballPlayerGenderEnum.Other ? "Other" : currentFootballPlayer.Gender;
                currentFootballPlayer.DateOfBirth = String.IsNullOrEmpty(model.DateOfBirth.ToString()) ? currentFootballPlayer.DateOfBirth : model.DateOfBirth;
                currentFootballPlayer.DateUpdate = DateTime.Now;

                bool isUpdated = await _footballPlayerService.UpdateAsync(currentFootballPlayer);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<FootballPlayerVM>(currentFootballPlayer));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Change status football player By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpPatch("{id}")]
        public async Task<ActionResult> ChangeStatusFootballPlayer(int id)
        {
            FootballPlayer currentFootballPlayer = await _footballPlayerService.GetByIdAsync(id);
            if (currentFootballPlayer == null)
            {
                return NotFound(new
                {
                    message = "Can not found football player by id: " + id
                });
            }
            try
            {
                currentFootballPlayer.Status = !currentFootballPlayer.Status;
                bool isDeleted = await _footballPlayerService.UpdateAsync(currentFootballPlayer);
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

        /// <summary>Delete football player By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            FootballPlayer currentFootballPlayer = await _footballPlayerService.GetByIdAsync(id);
            if (currentFootballPlayer == null)
            {
                return NotFound(new
                {
                    message = "Can not found football player by id: " + id
                });
            }
            try
            {
                currentFootballPlayer.Status = false;
                currentFootballPlayer.DateDelete = DateTime.Now;
                bool isDeleted = await _footballPlayerService.UpdateAsync(currentFootballPlayer);
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
