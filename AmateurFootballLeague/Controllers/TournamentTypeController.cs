using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/tournament-types")]
    [ApiController]
    //[Authorize(Roles = "ADMIN")]
    public class TournamentTypeController : ControllerBase
    {
        private readonly ITournamentTypeService _tournamentTypeService;
        private readonly IMapper _mapper;

        public TournamentTypeController(ITournamentTypeService tournamentTypeService, IMapper mapper)
        {
            _tournamentTypeService = tournamentTypeService;
            _mapper = mapper;
        }

        /// <summary>Get list tournament types</summary>
        /// <returns>List tournament types</returns>
        /// <response code="200">Returns list tournament types</response>
        /// <response code="404">Not found tournament types</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<TournamentTypeListVM> GetListTournamentType(
            [FromQuery(Name = "tournament-type-name")] string? name,
            [FromQuery(Name = "order-by")] TournamentTypeFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<TournamentType> tournamentTypeList = _tournamentTypeService.GetList();
                if (!String.IsNullOrEmpty(name))
                {
                    tournamentTypeList = tournamentTypeList.Where(s => s.TournamentTypeName!.ToUpper().Contains(name.Trim().ToUpper()));
                }
                var tournamentTypeListPaging = tournamentTypeList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var tournamentTypeListFilter = new List<TournamentType>();
                if (orderBy == TournamentTypeFieldEnum.Id)
                {
                    tournamentTypeListFilter = tournamentTypeListPaging.OrderBy(tnmT => tnmT.Id).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        tournamentTypeListFilter = tournamentTypeListPaging.OrderByDescending(tnmT => tnmT.Id).ToList();
                    }
                }
                if (orderBy == TournamentTypeFieldEnum.TournamentTypeName)
                {
                    tournamentTypeListFilter = tournamentTypeListPaging.OrderBy(tnmT => tnmT.TournamentTypeName).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        tournamentTypeListFilter = tournamentTypeListPaging.OrderByDescending(tnmT => tnmT.TournamentTypeName).ToList();
                    }
                }

                var tournamentTypeListResponse = new TournamentTypeListVM
                {
                    TournamentTypes = _mapper.Map<List<TournamentType>, List<TournamentTypeVM>>(tournamentTypeListFilter),
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(tournamentTypeListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get tournament type by id</summary>
        /// <returns>Return the tournament type with the corresponding id</returns>
        /// <response code="200">Returns the tournament type with the specified id</response>
        /// <response code="404">No tournament type found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentTypeVM>> GetTournamentTypeById(int id)
        {
            try
            {
                TournamentType currentTournamentType = await _tournamentTypeService.GetByIdAsync(id);
                if (currentTournamentType != null)
                {
                    return Ok(_mapper.Map<TournamentTypeVM>(currentTournamentType));
                }
                return NotFound("Không tìm thấy loại giải đấu với id là " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new tournament type</summary>
        /// <response code="201">Created new tournament type successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentTypeVM>> CreateTournamentType([FromBody] TournamentTypeCM model)
        {
            TournamentType tournamentType = _mapper.Map<TournamentType>(model);
            try
            {
                bool isDuplicated = _tournamentTypeService.GetList().Where(s => s.TournamentTypeName!.Trim().ToUpper().Equals(model.TournamentTypeName!.Trim().ToUpper())).FirstOrDefault() != null;
                if (isDuplicated)
                {
                    return BadRequest(new
                    {
                        message = "Tên loại giải đấu đã tồn tại"
                    });
                }

                TournamentType tournamentTypeCreated = await _tournamentTypeService.AddAsync(tournamentType);
                if (tournamentTypeCreated != null)
                {
                    return CreatedAtAction("GetTournamentTypeById", new { id = tournamentTypeCreated.Id }, _mapper.Map<TournamentTypeVM>(tournamentTypeCreated));
                }
                return BadRequest("Tạo loại giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a tournament type</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<TournamentTypeVM>> UpdateTournamentType([FromQuery(Name = "id")] int id, [FromQuery(Name = "tournament-type-name")] string? tournamentTypeName, [FromQuery(Name = "description")] string? description)
        {
            TournamentType currentTournamentType = await _tournamentTypeService.GetByIdAsync(id);
            if (currentTournamentType == null)
            {
                return NotFound("Không tìm thấy loại giải đấu với id là " + id);
            }
            if (!String.IsNullOrEmpty(tournamentTypeName))
            {
                if (!currentTournamentType.TournamentTypeName!.ToUpper().Equals(tournamentTypeName.ToUpper()) && _tournamentTypeService.GetList().Where(s => s.TournamentTypeName!.Trim().ToUpper().Equals(tournamentTypeName.Trim().ToUpper())).FirstOrDefault() != null)
                {
                    return BadRequest(new
                    {
                        message = "Tên loại giải đấu đã tồn tại"
                    });
                }
            }

            try
            {
                currentTournamentType.TournamentTypeName = String.IsNullOrEmpty(tournamentTypeName) ? currentTournamentType.TournamentTypeName : tournamentTypeName.Trim();
                currentTournamentType.Description = String.IsNullOrEmpty(description) ? currentTournamentType.Description : description.Trim();

                bool isUpdated = await _tournamentTypeService.UpdateAsync(currentTournamentType);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<TournamentTypeVM>(currentTournamentType));
                }
                return BadRequest("Cập nhật loại giải đấu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete tournament type By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            TournamentType currentTournamentType = await _tournamentTypeService.GetByIdAsync(id);
            if (currentTournamentType == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy loại giải đấu với id là " + id
                });
            }
            try
            {
                bool isDeleted = await _tournamentTypeService.DeleteAsync(currentTournamentType);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Xóa loại giải đấu thành công"
                    });
                }
                return BadRequest("Xóa loại giải đấu thất bạ");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
