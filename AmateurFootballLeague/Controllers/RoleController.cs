using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;

namespace AmateurFootballLeague.Controllers
{
    [Route("api/v1/roles")]
    [ApiController]
    //[Authorize(Roles = "ADMIN")]
    public class RoleController : ControllerBase
    {
        private readonly IRoleService _roleService;
        private readonly IMapper _mapper;

        public RoleController(IRoleService roleService, IMapper mapper)
        {
            _roleService = roleService;
            _mapper = mapper;
        }

        /// <summary>Get list roles</summary>
        /// <returns>List roles</returns>
        /// <response code="200">Returns list roles</response>
        /// <response code="404">Not found roles</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<RoleListVM> GetListRole(
            [FromQuery(Name = "role-name")] string? name,
            [FromQuery(Name = "order-by")] RoleFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<Role> roleList = _roleService.GetList();
                if (!String.IsNullOrEmpty(name))
                {
                    roleList = roleList.Where(s => s.RoleName.ToUpper().Contains(name.Trim().ToUpper()));
                }
                var roleListPaging = roleList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var roleListFilter = new List<Role>();
                if (orderBy == RoleFieldEnum.Id)
                {
                    roleListFilter = roleListPaging.OrderBy(rl => rl.Id).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        roleListFilter = roleListPaging.OrderByDescending(rl => rl.Id).ToList();
                    }
                }
                if (orderBy == RoleFieldEnum.RoleName)
                {
                    roleListFilter = roleListPaging.OrderBy(rl => rl.RoleName).ToList();
                    if (orderType == SortTypeEnum.DESC)
                    {
                        roleListFilter = roleListPaging.OrderByDescending(rl => rl.RoleName).ToList();
                    }
                }

                var roleListResponse = new RoleListVM
                {
                    Roles = _mapper.Map<List<Role>, List<RoleVM>>(roleListFilter),
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(roleListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get role by id</summary>
        /// <returns>Return the role with the corresponding id</returns>
        /// <response code="200">Returns the role type with the specified id</response>
        /// <response code="404">No role found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult<RoleVM>> GetRoleById(int id)
        {
            try
            {
                Role currentRole = await _roleService.GetByIdAsync(id);
                if (currentRole != null)
                {
                    return Ok(_mapper.Map<RoleVM>(currentRole));
                }
                return NotFound("Can not found role by id: " + id);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new role</summary>
        /// <response code="201">Created new role successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<RoleVM>> CreateRole([FromBody] RoleCM model)
        {
            Role role = _mapper.Map<Role>(model);
            try
            {
                bool isDuplicated = _roleService.GetList().Where(s => s.RoleName.Trim().ToUpper().Equals(model.RoleName.Trim().ToUpper())).FirstOrDefault() != null;
                if (isDuplicated)
                {
                    return BadRequest(new
                    {
                        message = "Role Name is duplicated"
                    });
                }
                role.RoleName = role.RoleName.ToUpper();
                Role roleCreated = await _roleService.AddAsync(role);
                if (roleCreated != null)
                {
                    return CreatedAtAction("GetRoleById", new { id = roleCreated.Id }, _mapper.Map<RoleVM>(roleCreated));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a role</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Field is not matched</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<RoleVM>> UpdateRole([FromQuery(Name = "id")] int id, [FromQuery(Name = "role-name")] string? roleName)
        {
            Role currentRole = await _roleService.GetByIdAsync(id);
            if (currentRole == null)
            {
                return NotFound();
            }
            if (!String.IsNullOrEmpty(roleName))
            {
                if (!currentRole.RoleName.ToUpper().Equals(roleName.ToUpper()) && _roleService.GetList().Where(s => s.RoleName.Trim().ToUpper().Equals(roleName.Trim().ToUpper())).FirstOrDefault() != null)
                {
                    return BadRequest(new
                    {
                        message = "Role Name is duplicated!"
                    });
                }
            }
            try
            {
                currentRole.RoleName = String.IsNullOrEmpty(roleName) ? currentRole.RoleName : roleName.ToUpper();
                bool isUpdated = await _roleService.UpdateAsync(currentRole);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<RoleVM>(currentRole));
                }
                return BadRequest();
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete role By Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Internal server error</response>
        [HttpDelete]
        [Route("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteById(int id)
        {
            Role currentRole = await _roleService.GetByIdAsync(id);
            if (currentRole == null)
            {
                return NotFound(new
                {
                    message = "Can not found role by id: " + id
                });
            }
            try
            {
                bool isDeleted = await _roleService.DeleteAsync(currentRole);
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
