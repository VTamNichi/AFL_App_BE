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
        private readonly IUserService _userService;
        private readonly IUploadFileService _uploadFileService;
        private readonly IMapper _mapper;

        public FootballPlayerController(IFootballPlayerService footballPlayerService, IUserService userService, IUploadFileService uploadFileService, IMapper mapper)
        {
            _footballPlayerService = footballPlayerService;
            _userService = userService;
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
        public async Task<ActionResult<FootballPlayerListVM>> GetListFootballPlayer(
            [FromQuery(Name = "football-player-name")] string? name,
            [FromQuery(Name = "gender")] string? gender,
            [FromQuery(Name = "position")] string? position,
            [FromQuery(Name = "status")] bool? status,
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
                if (!String.IsNullOrEmpty(gender))
                {
                    footballPlayerList = footballPlayerList.Where(s => s.IdNavigation.Gender.ToUpper() == gender.ToUpper());
                }
                if (!String.IsNullOrEmpty(position))
                {
                    footballPlayerList = footballPlayerList.Where(s => s.Position.ToUpper().Contains(position.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(status.ToString()))
                {
                    footballPlayerList = footballPlayerList.Where(s => s.Status == status);
                }

                int countList = footballPlayerList.Count();

                var footballPlayerListOrder = new List<FootballPlayer>();
                if (orderBy == FootballPlayerFieldEnum.Id)
                {
                    footballPlayerList = footballPlayerList.OrderBy(tnm => tnm.Id);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        footballPlayerList = footballPlayerList.OrderByDescending(tnm => tnm.Id);
                    }
                }
                if (orderBy == FootballPlayerFieldEnum.PlayerName)
                {
                    footballPlayerList = footballPlayerList.OrderBy(tnm => tnm.PlayerName);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        footballPlayerList = footballPlayerList.OrderByDescending(tnm => tnm.PlayerName);
                    }
                }
                if (orderBy == FootballPlayerFieldEnum.Position)
                {
                    footballPlayerList = footballPlayerList.OrderBy(tnm => tnm.Position);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        footballPlayerList = footballPlayerList.OrderByDescending(tnm => tnm.Position);
                    }
                }
                if (orderBy == FootballPlayerFieldEnum.DateOfBirth)
                {
                    footballPlayerList = footballPlayerList.OrderBy(tnm => tnm.IdNavigation.DateOfBirth);
                    if (orderType == SortTypeEnum.DESC)
                    {
                        footballPlayerList = footballPlayerList.OrderByDescending(tnm => tnm.IdNavigation.DateOfBirth);
                    }
                }

                var footballPlayerListPaging = footballPlayerList.Skip((pageIndex - 1) * limit).Take(limit).ToList();
                List<FootballPlayerVM> listVM = _mapper.Map<List<FootballPlayerVM>>(footballPlayerListPaging);
                foreach(var fp in listVM)
                {
                    fp.UserVM = _mapper.Map<UserVM>(await _userService.GetByIdAsync(fp.Id));
                }

                var footballPlayerListResponse = new FootballPlayerListVM
                {
                    FootballPlayers = listVM,
                    CountList = countList,
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
                if(currentFootballPlayer == null)
                {
                    return NotFound("Cầu thủ không tồn tại");
                }
                FootballPlayerVM footballPlayerVM = _mapper.Map<FootballPlayerVM>(currentFootballPlayer);
                footballPlayerVM.UserVM = _mapper.Map<UserVM>(await _userService.GetByIdAsync(id));
                if (currentFootballPlayer != null)
                {
                    return Ok(footballPlayerVM);
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
            try
            {
                FootballPlayer currentFootballPlayer = await _footballPlayerService.GetByIdAsync(model.Id);
                if (currentFootballPlayer != null)
                {
                    return BadRequest("Cầu thủ đã tồn tại");
                }
                FootballPlayer footballPlayer = new FootballPlayer();
                User user = await _userService.GetByIdAsync(model.Id);
                if(user == null)
                {
                    return NotFound("Người dùng không tồn tại");
                }
                footballPlayer.Id = model.Id;
                footballPlayer.PlayerName = model.PlayerName;
                try
                {
                    if (!String.IsNullOrEmpty(model.PlayerAvatar.ToString()))
                    {
                        string fileUrl = await _uploadFileService.UploadFile(model.PlayerAvatar, "images", "image-url");
                        footballPlayer.PlayerAvatar = fileUrl;
                    }
                }
                catch (Exception)
                {
                    footballPlayer.PlayerAvatar = "https://t3.ftcdn.net/jpg/03/46/83/96/360_F_346839683_6nAPzbhpSkIpb8pmAwufkC7c5eD7wYws.jpg";
                }
                footballPlayer.Position = model.Position;
                footballPlayer.Description = model.Description;
                footballPlayer.DateCreate = DateTime.Now;
                footballPlayer.Status = true;

                FootballPlayer footballPlayerCreated = await _footballPlayerService.AddAsync(footballPlayer);
                if (footballPlayerCreated != null)
                {
                    FootballPlayerVM footballPlayerVM = _mapper.Map<FootballPlayerVM>(footballPlayerCreated);
                    footballPlayerVM.UserVM = _mapper.Map<UserVM>(await _userService.GetByIdAsync(model.Id));
                    return Ok(footballPlayerVM);
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
            try
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
                }
                catch (Exception)
                {
                }

                currentFootballPlayer.PlayerName = String.IsNullOrEmpty(model.PlayerName) ? currentFootballPlayer.PlayerName : model.PlayerName.Trim();
                currentFootballPlayer.Position = String.IsNullOrEmpty(model.Position) ? currentFootballPlayer.Position : model.Position.Trim();
                currentFootballPlayer.Description = String.IsNullOrEmpty(model.Description) ? currentFootballPlayer.Description : model.Description.Trim();
                currentFootballPlayer.DateUpdate = DateTime.Now;

                bool isUpdated = await _footballPlayerService.UpdateAsync(currentFootballPlayer);
                if (isUpdated)
                {
                    FootballPlayerVM footballPlayerVM = _mapper.Map<FootballPlayerVM>(currentFootballPlayer);
                    footballPlayerVM.UserVM = _mapper.Map<UserVM>(await _userService.GetByIdAsync(model.Id));
                    return Ok(footballPlayerVM);
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
