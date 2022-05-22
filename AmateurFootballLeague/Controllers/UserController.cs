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
    [Route("api/v1/users")]
    [ApiController]
    //[Authorize(Roles = "ADMIN")]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IRoleService _roleService;
        private readonly IUploadFileService _uploadFileService;
        private readonly IMapper _mapper;
        private readonly IJWTProvider _jwtProvider;
        public UserController(IUserService userService, IRoleService roleService, IUploadFileService uploadFileService, IJWTProvider jwtProvider, IMapper mapper)
        {
            _userService = userService;
            _roleService = roleService;
            _uploadFileService = uploadFileService;
            _mapper = mapper;
            _jwtProvider = jwtProvider;
        }

        /// <summary>Get list users</summary>
        /// <returns>List users</returns>
        /// <response code="200">Returns list roles</response>
        /// <response code="404">Not found users</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Produces("application/json")]
        public ActionResult<UserListVM> GetListUser(
            [FromQuery(Name = "user-name")] string? name,
            [FromQuery(Name = "gender")] UserGenderEnum? gender,
            [FromQuery(Name = "start-dob")] DateTime? startDOB,
            [FromQuery(Name = "end-dob")] DateTime? endDOB,
            [FromQuery(Name = "address")] string? address,
            [FromQuery(Name = "phone")] string? phone,
            [FromQuery(Name = "role-id")] int? roleId,
            [FromQuery(Name = "order-by")] UserFieldEnum orderBy,
            [FromQuery(Name = "order-type")] SortTypeEnum orderType,
            [FromQuery(Name = "page-offset")] int pageIndex = 1,
            int limit = 5
        )
        {
            try
            {
                IQueryable<User> userList = _userService.GetList();
                if (!String.IsNullOrEmpty(name))
                {
                    userList = userList.Where(s => s.Username.ToUpper().Contains(name.Trim().ToUpper()));
                }
                if(gender == UserGenderEnum.Male)
                {
                    userList = userList.Where(s => s.Gender == "Male");
                } else if (gender == UserGenderEnum.Female)
                {
                    userList = userList.Where(s => s.Gender == "Female");
                }
                if(!String.IsNullOrEmpty(startDOB.ToString()))
                {
                    userList = userList.Where(s => s.DateOfBirth.Value.CompareTo(startDOB.Value) >= 0);
                }
                if (!String.IsNullOrEmpty(endDOB.ToString()))
                {
                    userList = userList.Where(s => s.DateOfBirth.Value.CompareTo(endDOB.Value) <= 0);
                }

                if (!String.IsNullOrEmpty(address))
                {
                    userList = userList.Where(s => s.Address.ToUpper().Contains(address.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(phone))
                {
                    userList = userList.Where(s => s.Phone.ToUpper().Contains(phone.Trim().ToUpper()));
                }
                if (!String.IsNullOrEmpty(roleId.ToString()))
                {
                    userList = userList.Where(s => s.RoleId == roleId);
                }

                if (orderType == SortTypeEnum.ASC)
                {
                    if(orderBy == UserFieldEnum.Id)
                    {
                        userList = userList.OrderBy(rl => rl.Id);
                    }
                    else if (orderBy == UserFieldEnum.Email)
                    {
                        userList = userList.OrderBy(rl => rl.Email);
                    }
                    else if (orderBy == UserFieldEnum.Username)
                    {
                        userList = userList.OrderBy(rl => rl.Username);
                    }
                    else if (orderBy == UserFieldEnum.Gender)
                    {
                        userList = userList.OrderBy(rl => rl.Gender);
                    }
                    else if (orderBy == UserFieldEnum.DateOfBirth)
                    {
                        userList = userList.OrderBy(rl => rl.DateOfBirth);
                    }
                    else if (orderBy == UserFieldEnum.DateCreate)
                    {
                        userList = userList.OrderBy(rl => rl.DateCreate);
                    }
                } else if(orderType == SortTypeEnum.DESC)
                {
                    if (orderBy == UserFieldEnum.Id)
                    {
                        userList = userList.OrderByDescending(rl => rl.Id);
                    }
                    else if (orderBy == UserFieldEnum.Email)
                    {
                        userList = userList.OrderByDescending(rl => rl.Email);
                    }
                    else if (orderBy == UserFieldEnum.Username)
                    {
                        userList = userList.OrderByDescending(rl => rl.Username);
                    }
                    else if (orderBy == UserFieldEnum.Gender)
                    {
                        userList = userList.OrderByDescending(rl => rl.Gender);
                    }
                    else if (orderBy == UserFieldEnum.DateOfBirth)
                    {
                        userList = userList.OrderByDescending(rl => rl.DateOfBirth);
                    }
                    else if (orderBy == UserFieldEnum.DateCreate)
                    {
                        userList = userList.OrderByDescending(rl => rl.DateCreate);
                    }
                }

                int countList = userList.Count();

                var userListPaging = userList.Skip((pageIndex - 1) * limit).Take(limit).ToList();

                var userListResponse = new UserListVM
                {
                    Users = _mapper.Map<List<User>, List<UserVM>>(userListPaging),
                    CountList = countList,
                    CurrentPage = pageIndex,
                    Size = limit
                };

                return Ok(userListResponse);
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Get user by id or email</summary>
        /// <returns>Return the user with the corresponding id</returns>
        /// <response code="200">Returns the user type with the specified id</response>
        /// <response code="404">No user found with the specified id</response>
        /// <response code="500">Internal server error</response>
        [HttpGet]
        [Route("{search}")]
        [Produces("application/json")]
        public async Task<ActionResult<UserVM>> GetUserByIdOrEmail([FromRoute] string search, [FromQuery(Name = "search-type")] UserSearchType searchType)
        {
            try
            {
                User user = null;
                if (searchType == UserSearchType.Id)
                {
                    try
                    {
                        int result = Int32.Parse(search);
                        user = await _userService.GetByIdAsync(result);
                    }
                    catch (FormatException)
                    {
                        return BadRequest();
                    }
                }
                else if (searchType == UserSearchType.Email && !string.IsNullOrEmpty(search))
                {
                    user = _userService.GetUserByEmail(search.Trim());
                }
                if (user != null)
                {
                    return Ok(_mapper.Map<UserVM>(user));
                }
                return NotFound("Không tìm thấy người dùng");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Create a new user</summary>
        /// <response code="201">Created new user successfull</response>
        /// <response code="400">Field is not matched or duplicated</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost]
        [Produces("application/json")]
        public async Task<ActionResult<UserVM>> CreateUser([FromForm] UserCM model)
        {
            try
            {
                User currentUser = _userService.GetList().Where(s => s.Email.Trim().ToUpper().Equals(model.Email.Trim().ToUpper())).FirstOrDefault();
                if (currentUser != null)
                {
                    return BadRequest(new
                    {
                        message = "Email này đã tồn tại trong hệ thống"
                    });
                }
                User userCheckPhone = _userService.GetList().Where(s => s.Phone.Trim().ToUpper().Equals(model.Phone.Trim().ToUpper())).FirstOrDefault();
                if (currentUser != null)
                {
                    return BadRequest(new
                    {
                        message = "Số điện thoại này đã tồn tại trong hệ thống"
                    });
                }
                Role currenRole = await _roleService.GetByIdAsync(model.RoleId);
                if (currenRole == null)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy vai trò này"
                    });
                }

                User convertUser = _mapper.Map<User>(model);
                convertUser.Email = model.Email.Trim().ToLower();

                if (!String.IsNullOrEmpty(model.Password))
                {
                    List<byte[]> listPassword = _userService.EncriptPassword(model.Password);
                    convertUser.PasswordHash = listPassword[0];
                    convertUser.PasswordSalt = listPassword[1];
                }
                else 
                {
                    Guid pass = new Guid();
                    List<byte[]> listPassword = _userService.EncriptPassword(pass.ToString());
                    convertUser.PasswordHash = listPassword[0];
                    convertUser.PasswordSalt = listPassword[1];
                }

                try
                {
                    if (!String.IsNullOrEmpty(model.Avatar.ToString()))
                    {
                        string fileUrl = await _uploadFileService.UploadFile(model.Avatar, "images", "image-url");
                        convertUser.Avatar = fileUrl;
                    }
                }
                catch (Exception)
                {
                    convertUser.Avatar = "https://t3.ftcdn.net/jpg/03/46/83/96/360_F_346839683_6nAPzbhpSkIpb8pmAwufkC7c5eD7wYws.jpg";
                }

                convertUser.Username = String.IsNullOrEmpty(model.Username) ? "" : model.Username.Trim();
                convertUser.Gender = model.Gender == UserGenderEnum.Male ? "Male" : model.Gender == UserGenderEnum.Female ? "Female" : "All";
                convertUser.DateOfBirth = String.IsNullOrEmpty(model.DateOfBirth.ToString()) ? DateTime.Now.AddYears(-20) : model.DateOfBirth;
                convertUser.Address = String.IsNullOrEmpty(model.Address) ? "" : model.Address.Trim();
                convertUser.Phone = String.IsNullOrEmpty(model.Phone) ? "" : model.Phone.Trim();
                convertUser.Bio = String.IsNullOrEmpty(model.Bio) ? "" : model.Bio.Trim();
                convertUser.StatusBan = "NO BANNED";
                convertUser.FlagReportComment = 0;
                convertUser.FlagReportTeam = 0;
                convertUser.FlagReportTournament = 0;
                convertUser.IdentityCard = String.IsNullOrEmpty(model.IdentityCard) ? "" : model.IdentityCard.Trim(); ;
                convertUser.PhoneBusiness = String.IsNullOrEmpty(model.PhoneBusiness) ? "" : model.PhoneBusiness.Trim(); ;
                convertUser.NameBusiness = String.IsNullOrEmpty(model.NameBusiness) ? "" : model.NameBusiness.Trim(); ;
                convertUser.Tinbusiness = String.IsNullOrEmpty(model.TINBusiness) ? "" : model.TINBusiness.Trim(); ;
                convertUser.Tinbusiness = String.IsNullOrEmpty(model.TINBusiness) ? "" : model.TINBusiness.Trim(); ;
                convertUser.Status = true;
                convertUser.DateCreate = DateTime.Now;
                convertUser.RoleId = model.RoleId;

                User userCreated = await _userService.AddAsync(convertUser);
                if (userCreated != null)
                {
                    return CreatedAtAction("GetUserByIdOrEmail", new { search = userCreated.Id }, _mapper.Map<UserVM>(userCreated));
                }
                return BadRequest(new
                {
                    message = "Tạo tài khoản thất bại"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        [HttpPost("CreateWithGoogle")]
        [Produces("application/json")]
        public async Task<ActionResult<UserLVM>> CreateUserGG([FromForm] UserGGCM model)
        {
            try
            {
                User currentUser = _userService.GetList().Where(s => s.Email.Trim().ToUpper().Equals(model.Email.Trim().ToUpper())).FirstOrDefault();
                if (currentUser != null)
                {
                    return BadRequest(new
                    {
                        message = "Email này đã tồn tại trong hệ thống"
                    });
                }
                Role currenRole = await _roleService.GetByIdAsync(model.RoleId);
                if (currenRole == null)
                {
                    return NotFound(new
                    {
                        message = "Không tìm thấy vai trò này"
                    });
                }

                User convertUser = _mapper.Map<User>(model);
                convertUser.Email = model.Email.Trim().ToLower();
                Guid pass = new Guid();
                List<byte[]> listPassword = _userService.EncriptPassword(pass.ToString());
                convertUser.PasswordHash = listPassword[0];
                convertUser.PasswordSalt = listPassword[1];

                convertUser.Avatar = String.IsNullOrEmpty(model.Avatar) ? "" : model.Avatar;

                convertUser.Username = model.Username.Trim();
                convertUser.Gender = model.Gender == UserGenderEnum.Male ? "Male" : "Female";
                convertUser.Phone = String.IsNullOrEmpty(model.Phone) ? "" : model.Phone.Trim();
                convertUser.StatusBan = "NO BANNED";
                convertUser.FlagReportComment = 0;
                convertUser.FlagReportTeam = 0;
                convertUser.FlagReportTournament = 0;
                convertUser.Status = true;
                convertUser.DateCreate = DateTime.Now;
                convertUser.RoleId = model.RoleId;

                User userCreated = await _userService.AddAsync(convertUser);
                if (userCreated != null)
                {
       
                    User user = _userService.GetUserByEmail(userCreated.Email);
                    UserVM userVM = _mapper.Map<UserVM>(user);
                    UserLVM userLEPVM = new UserLVM
                    {
                        UserVM = userVM,
                        AccessToken = await _jwtProvider.GenerationToken(user)
                    };
                    return Ok(userLEPVM);
                }
                return BadRequest(new
                {
                    message = "Tạo tài khoản thất bại"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Update a user</summary>
        /// <response code="201">Update user successfull</response>
        /// <response code="404">User is not found</response>
        /// <response code="500">Failed to save request</response>
        [HttpPut]
        [Produces("application/json")]
        public async Task<ActionResult<UserVM>> UpdateUser([FromForm] UserUM model)
        {
            try
            {
                User user = await _userService.GetByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }
                
                if (!String.IsNullOrEmpty(model.RoleId.ToString()))
                {
                    Role currenRole = await _roleService.GetByIdAsync((int)model.RoleId);
                    if (currenRole == null)
                    {
                        return NotFound(new
                        {
                            message = "Không tìm thấy vai trò này"
                        });
                    }
                    user.RoleId = (int)model.RoleId;
                }
                if (!String.IsNullOrEmpty(model.Password))
                {
                    List<byte[]> listPassword = _userService.EncriptPassword(model.Password);
                    user.PasswordHash = listPassword[0];
                    user.PasswordSalt = listPassword[1];
                }
                try
                {
                    if (!String.IsNullOrEmpty(model.Avatar.ToString()))
                    {
                        string fileUrl = await _uploadFileService.UploadFile(model.Avatar, "images", "image-url");
                        user.Avatar = fileUrl;
                    }
                }
                catch (Exception) { }
                

                user.Username = String.IsNullOrEmpty(model.Username) ? user.Username : model.Username;
                user.Gender = model.Gender == UserGenderEnum.Male ? "Male" : model.Gender == UserGenderEnum.Female ? "Female" : user.Gender;
                user.DateOfBirth = String.IsNullOrEmpty(model.DateOfBirth.ToString()) ? user.DateOfBirth : model.DateOfBirth;
                user.Address = String.IsNullOrEmpty(model.Address) ? user.Address : model.Address;
                user.Phone = String.IsNullOrEmpty(model.Phone) ? user.Phone : model.Phone;
                user.Bio = String.IsNullOrEmpty(model.Bio) ? user.Bio : model.Bio;
                user.IdentityCard = String.IsNullOrEmpty(model.IdentityCard) ? user.IdentityCard : model.IdentityCard;
                user.PhoneBusiness = String.IsNullOrEmpty(model.PhoneBusiness) ? user.PhoneBusiness : model.PhoneBusiness;
                user.NameBusiness = String.IsNullOrEmpty(model.NameBusiness) ? user.NameBusiness : model.NameBusiness;
                user.Tinbusiness = String.IsNullOrEmpty(model.Tinbusiness) ? user.Tinbusiness : model.Tinbusiness;
                user.DateUpdate = DateTime.Now;

                bool isUpdate = await _userService.UpdateAsync(user);
                if (isUpdate)
                {
                    return CreatedAtAction("GetUserByIdOrEmail", new { search = model.Id }, _mapper.Map<UserVM>(user));
                }
                return BadRequest(new
                {
                    message = "Cập nhật tài khoản thất bại"
                });
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Change status user</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Failed to save request</response>
        [HttpPatch("{id}")]
        public async Task<ActionResult> ChangeStatus([FromRoute] int id)
        {
            try
            {
                User user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                user.Status = !user.Status;

                bool isUpdated = await _userService.UpdateAsync(user);
                if (isUpdated)
                {
                    return Ok(_mapper.Map<UserVM>(user));
                }
                return BadRequest("Thay đổi trạng thái người dùng thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Delete user by Id</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Bad Request</response>
        /// <response code="500">Failed to save request</response>
        [HttpDelete("{id}")]
        [Produces("application/json")]
        public async Task<ActionResult> DeleteUser([FromRoute] int id)
        {
            try
            {
                User user = await _userService.GetByIdAsync(id);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                user.Status = false;
                user.DateDelete = DateTime.Now;
                bool isUpdated = await _userService.UpdateAsync(user);
                if (isUpdated)
                {
                    return Ok("Xóa tài khoản thành công");
                }
                return BadRequest("Xóa tài khoản thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Change password</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Wrong password</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost("change-password")]
        [Produces("application/json")]
        public async Task<ActionResult> ChangePassword([FromBody] UserCPM model)
        {
            try
            {
                User user = await _userService.GetByIdAsync(model.Id);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }
                if (!_userService.CheckPassword(model.CurrentPassword, user.PasswordHash, user.PasswordSalt))
                {
                    return BadRequest("Sai mật khẩu");
                }
                List<byte[]> listPassword = _userService.EncriptPassword(model.NewPassword);
                user.PasswordHash = listPassword[0];
                user.PasswordSalt = listPassword[1];
                user.DateUpdate = DateTime.Now;
                bool isUpdated = await _userService.UpdateAsync(user);
                if (isUpdated)
                {
                    return Ok("Đổi mật khẩu thành công thành công");
                }
                return BadRequest("Đổi mật khẩu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        /// <summary>Reset password</summary>
        /// <response code="200">Success</response>
        /// <response code="404">Not Found</response>
        /// <response code="400">Wrong password</response>
        /// <response code="500">Failed to save request</response>
        [HttpPost("reset-password")]
        [Produces("application/json")]
        public async Task<ActionResult> ResetPassword([FromBody] UserRPM model)
        {
            try
            {
                User user = _userService.GetUserByEmail(model.Email);
                if (user == null)
                {
                    return NotFound("Không tìm thấy người dùng");
                }

                List<byte[]> listPassword = _userService.EncriptPassword(model.NewPassword);
                user.PasswordHash = listPassword[0];
                user.PasswordSalt = listPassword[1];
                user.DateUpdate = DateTime.Now;
                bool isUpdated = await _userService.UpdateAsync(user);
                if (isUpdated)
                {
                    return Ok("Đổi mật khẩu thành công thành công");
                }
                return BadRequest("Đổi mật khẩu thất bại");
            }
            catch (Exception)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }
    }
}
