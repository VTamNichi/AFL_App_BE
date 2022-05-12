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
                return NotFound("Không thể tìm thấy cầu thủ với id là " + id);
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
                    string fileUrl = await _uploadFileService.UploadFile(model.PlayerAvatar, "images", "image-url");
                    footballPlayer.PlayerAvatar = fileUrl;
                }
                else
                {
                    footballPlayer.PlayerAvatar = "https://t3.ftcdn.net/jpg/03/46/83/96/360_F_346839683_6nAPzbhpSkIpb8pmAwufkC7c5eD7wYws.jpg";
                }
                if (!String.IsNullOrEmpty(model.DateOfBirth.ToString()))
                {
                    footballPlayer.DateOfBirth = model.DateOfBirth;
                }
                footballPlayer.Gender = model.Gender == FootballPlayerGenderEnum.Male ? "Male" : "Female";
                footballPlayer.DateCreate = DateTime.Now;
                footballPlayer.Status = true;

                FootballPlayer footballPlayerCreated = await _footballPlayerService.AddAsync(footballPlayer);
                if (footballPlayerCreated != null)
                {
                    return CreatedAtAction("GetFootballPlayerById", new { id = footballPlayerCreated.Id }, _mapper.Map<FootballPlayerVM>(footballPlayerCreated));
                }
                return BadRequest("Tạo cầu thủ thất bại");
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
                return NotFound("Không thể tìm thấy cầu thủ với id là " + model.Id);
            }
            try
            {
                if (!String.IsNullOrEmpty(model.PlayerAvatar.ToString()))
                {
                    string fileUrl = await _uploadFileService.UploadFile(model.PlayerAvatar, "images", "image-url");
                    currentFootballPlayer.PlayerAvatar = fileUrl;
                }
                currentFootballPlayer.Email = String.IsNullOrEmpty(model.Email) ? currentFootballPlayer.Email : model.Email.Trim();
                currentFootballPlayer.PlayerName = String.IsNullOrEmpty(model.PlayerName) ? currentFootballPlayer.PlayerName : model.PlayerName.Trim();
                currentFootballPlayer.Gender = model.Gender == FootballPlayerGenderEnum.Male ? "Male" : model.Gender == FootballPlayerGenderEnum.Female ? "Female" : currentFootballPlayer.Gender;
                currentFootballPlayer.DateOfBirth = String.IsNullOrEmpty(model.DateOfBirth.ToString()) ? currentFootballPlayer.DateOfBirth : model.DateOfBirth;
                currentFootballPlayer.DateUpdate = DateTime.Now;

                bool isUpdated = await _footballPlayerService.UpdateAsync(currentFootballPlayer);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<FootballPlayerVM>(currentFootballPlayer));
                }
                return BadRequest("Cập nhật cầu thủ thất bại");
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
                    message = "Không thể tìm thấy cầu thủ với id là " + id
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
                        message = "Thay đổi trạng thái cầu thủ thành công"
                    });
                }
                return BadRequest("Thay đổi trạng thái cầu thủ thất bại");
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
                    message = "Không thể tìm thấy cầu thủ với id là " + id
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
                        message = "Xóa  cầu thủ thành công"
                    });
                }
                return BadRequest("Xóa  cầu thủ thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
