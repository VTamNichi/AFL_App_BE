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
            CreateMap<UserGGCM, User>();
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
            CreateMap<Match, MatchFVM>();
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

            #region AutoMapper PlayerInTeamViewModel
            CreateMap<PlayerInTeam, PlayerInTeamVM>();
            CreateMap<PlayerInTeam, PlayerInTeamFVM>();
            CreateMap<PlayerInTeamCM, PlayerInTeam>();
            #endregion
            
            #region AutoMapper PlayerInTournamentViewModel
            CreateMap<PlayerInTournament, PlayerInTournamentVM>();
            CreateMap<PlayerInTournamentCM, PlayerInTournament>();
            #endregion
            
            #region AutoMapper TeamInMatchViewModel
            CreateMap<TeamInMatch, TeamInMatchVM>();
            CreateMap<TeamInMatchCM, TeamInMatch>();
            CreateMap<TeamInMatch, TeamInMatchMT>();
            #endregion

            #region AutoMapper MatchDetail
            CreateMap<MatchDetail, MatchDetailVM>();
            CreateMap<MatchDetailCM, MatchDetail>();
            CreateMap<MatchDetail, MatchDetailFVM>();
            #endregion
            
            #region AutoMapper ScorePrediction
            CreateMap<ScorePrediction, ScorePredictionVM>();
            CreateMap<ScorePredictionCM, ScorePrediction>();
            CreateMap<ScorePrediction, ScorePredictionFVM>();
            #endregion

            #region AutoMapper TournamentResultViewModel
            CreateMap<TournamentResult, TournamentResultVM>();
            CreateMap<TournamentResultCM, TournamentResult>();
            #endregion

            #region AutoMapper PromoteRequestViewModel
            CreateMap<PromoteRequest, PromoteRequestVM>();
            CreateMap<PromoteRequestCM, PromoteRequest>();
            #endregion
        }
    }
}
