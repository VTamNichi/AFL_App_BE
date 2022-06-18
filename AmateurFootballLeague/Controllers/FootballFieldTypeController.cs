using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/football-field-types")]
    [ApiController]
    //[Authorize(Roles = "ADMIN")]
    public class FootballFieldTypeController : ControllerBase
    {
        private readonly IFootballFieldTypeService _footballFieldTypeService;
        private readonly IMapper _mapper;

        public FootballFieldTypeController(IFootballFieldTypeService footballFieldTypeService, IMapper mapper)
        {
            _footballFieldTypeService = footballFieldTypeService;
            _mapper = mapper;
        }

        /// <summary>Get list football field types</summary>
        /// <returns>List football field types</returns>
        /// <response code="200">Returns list football field types</response>
        /// <response code="404">Not found football field types</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<FootballFieldTypeListVM> GetListTournamentType(
            [FromQuery(Name = "football-field-type-name")] string? name,
            [FromQuery(Name = "order-by")] FootballFieldTypeEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<FootballFieldType> footballFieldTypeList = _footballFieldTypeService.GetList();
                if (!String.IsNullOrEmpty(name))
                {
                    footballFieldTypeList = footballFieldTypeList.Where(s => s.FootballFieldTypeName!.ToUpper().Contains(name.Trim().ToUpper()));
                }
                var footballFieldTypeListPaging = footballFieldTypeList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var footballFieldTypeListFilter = new List<FootballFieldType>();
                if (orderBy == FootballFieldTypeEnum.Id)
                {
                    footballFieldTypeListFilter = footballFieldTypeListPaging.OrderBy(fbfT => fbfT.Id).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        footballFieldTypeListFilter = footballFieldTypeListPaging.OrderByDescending(fbfT => fbfT.Id).ToList();
                    }
                }
                if (orderBy == FootballFieldTypeEnum.FootballFieldTypeName)
                {
                    footballFieldTypeListFilter = footballFieldTypeListPaging.OrderBy(fbfT => fbfT.FootballFieldTypeName).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        footballFieldTypeListFilter = footballFieldTypeListPaging.OrderByDescending(fbfT => fbfT.FootballFieldTypeName).ToList();
                    }
                }

                var footballFieldTypeListResponse = new FootballFieldTypeListVM
                {
                    FootballFieldTypes = _mapper.Map<List<FootballFieldType>, List<FootballFieldTypeVM>>(footballFieldTypeListFilter),
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(footballFieldTypeListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get football field type by id</summary>
        /// <returns>Return the football field type with the corresponding id</returns>
        /// <response code="200">Returns the football field type with the specified id</response>
        /// <response code="404">No football field type found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<FootballFieldTypeVM>> GetFootballFieldTypeById(int id)
        {
            try
            {
                FootballFieldType currentFootballFieldType = await _footballFieldTypeService.GetByIdAsync(id);
                if (currentFootballFieldType != null)
                {
                    return Ok(_mapper.Map<FootballFieldTypeVM>(currentFootballFieldType));
                }
                return NotFound("Không tìm thấy loại sân với id là " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new football field type</summary>
        /// <response code="201">Created new football field type successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<FootballFieldTypeVM>> CreateFootballFieldType([FromBody] FootballFieldTypeCM model)
        {
            FootballFieldType footballFieldType = _mapper.Map<FootballFieldType>(model);
            try
            {
                bool isDuplicated = _footballFieldTypeService.GetList().Where(s => s.FootballFieldTypeName!.Trim().ToUpper().Equals(model.FootballFieldTypeName!.Trim().ToUpper())).FirstOrDefault() != null;
                if (isDuplicated)
                {
                    return BadRequest(new
                    {
                        message = "Tên loại sân đã tồn tại"
                    });
                }

                FootballFieldType footballFieldTypeCreated = await _footballFieldTypeService.AddAsync(footballFieldType);
                if (footballFieldTypeCreated != null)
                {
                    return CreatedAtAction("GetFootballFieldTypeById", new { id = footballFieldTypeCreated.Id }, _mapper.Map<FootballFieldTypeVM>(footballFieldTypeCreated));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a football field type</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<FootballFieldTypeVM>> UpdateFootballFieldType([FromQuery(Name = "id")] int id, [FromQuery(Name = "football-field-type-name")] string? footballFieldTypeName, [FromQuery(Name = "description")] string? description)
        {
            FootballFieldType currentFootballFieldType = await _footballFieldTypeService.GetByIdAsync(id);
            if (currentFootballFieldType == null)
            {
                return NotFound("Không tìm thấy loại sân với id là " + id);
            }
            if (!String.IsNullOrEmpty(footballFieldTypeName))
            {
                if (!currentFootballFieldType.FootballFieldTypeName!.ToUpper().Equals(footballFieldTypeName.ToUpper()) && _footballFieldTypeService.GetList().Where(s => s.FootballFieldTypeName!.Trim().ToUpper().Equals(footballFieldTypeName.Trim().ToUpper())).FirstOrDefault() != null)
                {
                    return BadRequest(new
                    {
                        message = "Loại sân bóng đã tồn tại"
                    });
                }
            }

            try
            {
                currentFootballFieldType.FootballFieldTypeName = String.IsNullOrEmpty(footballFieldTypeName) ? currentFootballFieldType.FootballFieldTypeName : footballFieldTypeName.Trim();
                currentFootballFieldType.Description = String.IsNullOrEmpty(description) ? currentFootballFieldType.Description : description.Trim();

                bool isUpdated = await _footballFieldTypeService.UpdateAsync(currentFootballFieldType);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<FootballFieldTypeVM>(currentFootballFieldType));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete football field type By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            FootballFieldType currentFootballFieldType = await _footballFieldTypeService.GetByIdAsync(id);
            if (currentFootballFieldType == null)
            {
                return NotFound(new
                {
                    message = "Không tìm thấy loại sân với id là " + id
                });
            }
            try
            {
                bool isDeleted = await _footballFieldTypeService.DeleteAsync(currentFootballFieldType);
                if (isDeleted)
                {
                    return Ok(new
                    {
                        message = "Xóa loại sân thành công"
                    });
                }
                return BadRequest("Xóa loại sân thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
