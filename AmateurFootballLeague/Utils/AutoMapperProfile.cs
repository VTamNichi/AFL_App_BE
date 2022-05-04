using AmateurFootballLeague.Models;
using AutoMapper;
using AmateurFootballLeague.ViewModels.Requests;
using AmateurFootballLeague.ViewModels.Responses;

namespace AmateurFootballLeague.Utils
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            #region AutoMapper RoleViewModel
            CreateMap<Role, RoleVM>();
            CreateMap<RoleCM, Role>();
            #endregion

            #region AutoMapper UserViewModel
            CreateMap<User, UserVM>();
            CreateMap<UserCM, User>();
            #endregion
        }
    }
}
