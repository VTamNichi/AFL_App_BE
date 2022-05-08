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

            #region AutoMapper TournamentTypeViewModel
            CreateMap<TournamentType, TournamentTypeVM>();
            CreateMap<TournamentTypeCM, TournamentType>();
            #endregion

            #region AutoMapper FootballFieldTypeViewModel
            CreateMap<FootballFieldType, FootballFieldTypeVM>();
            CreateMap<FootballFieldTypeCM, FootballFieldType>();
            #endregion

            #region AutoMapper TournamentViewModel
            CreateMap<Tournament, TournamentVM>();
            CreateMap<TournamentCM, Tournament>();
            #endregion

            #region AutoMapper TeamViewModel
            CreateMap<Team, TeamVM>();
            CreateMap<TeamCM, Team>();
            #endregion

            #region AutoMapper MatchViewModel
            CreateMap<Match, MatchVM>();
            #endregion

            #region AutoMapper NewsViewModel
            CreateMap<Image, ImageVM>();
            CreateMap<ImageCM, Image>();
            #endregion

            #region AutoMapper FootballPlayerViewModel
            CreateMap<FootballPlayer, FootballPlayerVM>();
            CreateMap<FootballPlayerCM, FootballPlayer>();
            #endregion

            #region AutoMapper TeamInTournamentViewModel
            CreateMap<TeamInTournament, TeamInTournamentVM>();
            CreateMap<TeamInTournamentCM, TeamInTournament>();
            #endregion
        }
    }
}
