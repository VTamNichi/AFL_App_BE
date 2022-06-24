using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace AmateurFootballLeague.Models
{
    public partial class AmateurFootballLeagueContext : DbContext
    {
        public AmateurFootballLeagueContext()
        {
        }

        public AmateurFootballLeagueContext(DbContextOptions<AmateurFootballLeagueContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Comment> Comments { get; set; } = null!;
        public virtual DbSet<FootballFieldType> FootballFieldTypes { get; set; } = null!;
        public virtual DbSet<FootballPlayer> FootballPlayers { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Match> Matchs { get; set; } = null!;
        public virtual DbSet<MatchDetail> MatchDetails { get; set; } = null!;
        public virtual DbSet<News> News { get; set; } = null!;
        public virtual DbSet<Notification> Notifications { get; set; } = null!;
        public virtual DbSet<PlayerInTeam> PlayerInTeams { get; set; } = null!;
        public virtual DbSet<PlayerInTournament> PlayerInTournaments { get; set; } = null!;
        public virtual DbSet<PromoteRequest> PromoteRequests { get; set; } = null!;
        public virtual DbSet<Report> Reports { get; set; } = null!;
        public virtual DbSet<Role> Roles { get; set; } = null!;
        public virtual DbSet<ScorePrediction> ScorePredictions { get; set; } = null!;
        public virtual DbSet<Team> Teams { get; set; } = null!;
        public virtual DbSet<TeamInMatch> TeamInMatchs { get; set; } = null!;
        public virtual DbSet<TeamInTournament> TeamInTournaments { get; set; } = null!;
        public virtual DbSet<Tournament> Tournaments { get; set; } = null!;
        public virtual DbSet<TournamentResult> TournamentResults { get; set; } = null!;
        public virtual DbSet<TournamentType> TournamentTypes { get; set; } = null!;
        public virtual DbSet<User> Users { get; set; } = null!;
        public virtual DbSet<VerifyCode> VerifyCodes { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
                optionsBuilder.UseSqlServer("Data Source=afl.c88bvbswfyiv.ap-southeast-1.rds.amazonaws.com;Initial Catalog=AmateurFootballLeague;Persist Security Info=True;User ID=ktat;Password=KhoaTuAnhTam");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Comment>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Content).HasMaxLength(256);

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.DateDelete).HasColumnType("datetime");

                entity.Property(e => e.DateUpdate).HasColumnType("datetime");

                entity.Property(e => e.MatchId).HasColumnName("MatchID");

                entity.Property(e => e.Status).HasMaxLength(32);

                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Match)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.MatchId)
                    .HasConstraintName("FK__Comments__MatchI__74AE54BC");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__Comments__TeamID__75A278F5");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK__Comments__Tourna__76969D2E");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Comments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Comments__UserID__778AC167");
            });

            modelBuilder.Entity<FootballFieldType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.FootballFieldTypeName).HasMaxLength(128);
            });

            modelBuilder.Entity<FootballPlayer>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.DateDelete).HasColumnType("datetime");

                entity.Property(e => e.DateUpdate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.PlayerAvatar).IsUnicode(false);

                entity.Property(e => e.PlayerName).HasMaxLength(128);

                entity.Property(e => e.Position).HasMaxLength(128);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.FootballPlayer)
                    .HasForeignKey<FootballPlayer>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_FootballPlayers_Users");
            });

            modelBuilder.Entity<Image>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.DateDelete).HasColumnType("datetime");

                entity.Property(e => e.DateUpdate).HasColumnType("datetime");

                entity.Property(e => e.ImageUrl)
                    .IsUnicode(false)
                    .HasColumnName("ImageURL");

                entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK__Images__Tourname__4F7CD00D");
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Fight).HasMaxLength(16);

                entity.Property(e => e.GroupFight).HasMaxLength(16);

                entity.Property(e => e.MatchDate).HasColumnType("datetime");

                entity.Property(e => e.Round).HasMaxLength(16);

                entity.Property(e => e.Status).HasMaxLength(32);

                entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.Matches)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK__Matchs__Tourname__49C3F6B7");
            });

            modelBuilder.Entity<MatchDetail>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.ActionMinute)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.MatchId).HasColumnName("MatchID");

                entity.Property(e => e.PlayerInTournamentId).HasColumnName("PlayerInTournamentID");

                entity.HasOne(d => d.Match)
                    .WithMany(p => p.MatchDetails)
                    .HasForeignKey(d => d.MatchId)
                    .HasConstraintName("FK__MatchDeta__Match__6477ECF3");

                entity.HasOne(d => d.PlayerInTournament)
                    .WithMany(p => p.MatchDetails)
                    .HasForeignKey(d => d.PlayerInTournamentId)
                    .HasConstraintName("FK__MatchDeta__Playe__656C112C");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Content).HasMaxLength(256);

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.DateDelete).HasColumnType("datetime");

                entity.Property(e => e.DateUpdate).HasColumnType("datetime");

                entity.Property(e => e.NewsImage).IsUnicode(false);

                entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK__News__Tournament__4CA06362");
            });

            modelBuilder.Entity<Notification>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Content).HasMaxLength(256);

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__Notificat__TeamI__02084FDA");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK__Notificat__Tourn__01142BA1");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Notifications)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Notificat__UserI__00200768");
            });

            modelBuilder.Entity<PlayerInTeam>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FootballPlayerId).HasColumnName("FootballPlayerID");

                entity.Property(e => e.Status).HasMaxLength(32);

                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.HasOne(d => d.FootballPlayer)
                    .WithMany(p => p.PlayerInTeams)
                    .HasForeignKey(d => d.FootballPlayerId)
                    .HasConstraintName("FK__PlayerInT__Footb__59FA5E80");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.PlayerInTeams)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__PlayerInT__TeamI__59063A47");
            });

            modelBuilder.Entity<PlayerInTournament>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PlayerInTeamId).HasColumnName("PlayerInTeamID");

                entity.Property(e => e.Status).HasMaxLength(32);

                entity.Property(e => e.TeamInTournamentId).HasColumnName("TeamInTournamentID");

                entity.HasOne(d => d.PlayerInTeam)
                    .WithMany(p => p.PlayerInTournaments)
                    .HasForeignKey(d => d.PlayerInTeamId)
                    .HasConstraintName("FK__PlayerInT__Playe__5DCAEF64");

                entity.HasOne(d => d.TeamInTournament)
                    .WithMany(p => p.PlayerInTournaments)
                    .HasForeignKey(d => d.TeamInTournamentId)
                    .HasConstraintName("FK__PlayerInT__TeamI__5CD6CB2B");
            });

            modelBuilder.Entity<PromoteRequest>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.DateIssuance).HasColumnType("datetime");

                entity.Property(e => e.IdentityCard)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.NameBusiness).HasMaxLength(128);

                entity.Property(e => e.PhoneBusiness)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.Reason).HasMaxLength(256);

                entity.Property(e => e.RequestContent).HasMaxLength(256);

                entity.Property(e => e.Status).HasMaxLength(32);

                entity.Property(e => e.Tinbusiness)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("TINBusiness");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.PromoteRequests)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__PromoteRe__UserI__71D1E811");
            });

            modelBuilder.Entity<Report>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DateReport).HasColumnType("datetime");

                entity.Property(e => e.FootballPlayerId).HasColumnName("FootballPlayerID");

                entity.Property(e => e.Reason).HasMaxLength(256);

                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.FootballPlayer)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.FootballPlayerId)
                    .HasConstraintName("FK_Reports_FootballPlayer");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__Reports__TeamID__7C4F7684");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK__Reports__Tournam__7D439ABD");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Reports)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Reports__UserID__7A672E12");
            });

            modelBuilder.Entity<Role>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.RoleName).HasMaxLength(16);
            });

            modelBuilder.Entity<ScorePrediction>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.MatchId).HasColumnName("MatchID");

                entity.Property(e => e.Status).HasMaxLength(32);

                entity.Property(e => e.TeamAscore).HasColumnName("TeamAScore");

                entity.Property(e => e.TeamBscore).HasColumnName("TeamBScore");

                entity.Property(e => e.TeamInMatchAid).HasColumnName("TeamInMatchAID");

                entity.Property(e => e.TeamInMatchBid).HasColumnName("TeamInMatchBID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Match)
                    .WithMany(p => p.ScorePredictions)
                    .HasForeignKey(d => d.MatchId)
                    .HasConstraintName("FK__ScorePred__Match__6B24EA82");

                entity.HasOne(d => d.TeamInMatchA)
                    .WithMany(p => p.ScorePredictionTeamInMatchAs)
                    .HasForeignKey(d => d.TeamInMatchAid)
                    .HasConstraintName("FK__ScorePred__TeamI__68487DD7");

                entity.HasOne(d => d.TeamInMatchB)
                    .WithMany(p => p.ScorePredictionTeamInMatchBs)
                    .HasForeignKey(d => d.TeamInMatchBid)
                    .HasConstraintName("FK__ScorePred__TeamI__693CA210");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ScorePredictions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__ScorePred__UserI__6A30C649");
            });

            modelBuilder.Entity<Team>(entity =>
            {
                entity.Property(e => e.Id)
                    .ValueGeneratedNever()
                    .HasColumnName("ID");

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.DateDelete).HasColumnType("datetime");

                entity.Property(e => e.DateUpdate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.TeamAvatar).IsUnicode(false);

                entity.Property(e => e.TeamGender).HasMaxLength(8);

                entity.Property(e => e.TeamName).HasMaxLength(128);

                entity.Property(e => e.TeamPhone)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.HasOne(d => d.IdNavigation)
                    .WithOne(p => p.Team)
                    .HasForeignKey<Team>(d => d.Id)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Teams_Users");
            });

            modelBuilder.Entity<TeamInMatch>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.MatchId).HasColumnName("MatchID");

                entity.Property(e => e.NextTeam).HasMaxLength(128);

                entity.Property(e => e.Result).HasMaxLength(16);

                entity.Property(e => e.TeamInTournamentId).HasColumnName("TeamInTournamentID");

                entity.Property(e => e.TeamName).HasMaxLength(128);

                entity.HasOne(d => d.Match)
                    .WithMany(p => p.TeamInMatches)
                    .HasForeignKey(d => d.MatchId)
                    .HasConstraintName("FK__TeamInMat__Match__619B8048");

                entity.HasOne(d => d.TeamInTournament)
                    .WithMany(p => p.TeamInMatches)
                    .HasForeignKey(d => d.TeamInTournamentId)
                    .HasConstraintName("FK__TeamInMat__TeamI__60A75C0F");
            });

            modelBuilder.Entity<TeamInTournament>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Status).HasMaxLength(32);

                entity.Property(e => e.StatusInTournament).HasMaxLength(32);

                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TeamInTournaments)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__TeamInTou__TeamI__5629CD9C");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.TeamInTournaments)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK__TeamInTou__Tourn__5535A963");
            });

            modelBuilder.Entity<Tournament>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.DateDelete).HasColumnType("datetime");

                entity.Property(e => e.DateUpdate).HasColumnType("datetime");

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.FootballFieldAddress).HasMaxLength(256);

                entity.Property(e => e.FootballFieldTypeId).HasColumnName("FootballFieldTypeID");

                entity.Property(e => e.Mode).HasMaxLength(32);

                entity.Property(e => e.RegisterEndDate).HasColumnType("datetime");

                entity.Property(e => e.StatusTnm).HasMaxLength(16);

                entity.Property(e => e.TournamentAvatar).IsUnicode(false);

                entity.Property(e => e.TournamentEndDate).HasColumnType("datetime");

                entity.Property(e => e.TournamentGender).HasMaxLength(8);

                entity.Property(e => e.TournamentName).HasMaxLength(128);

                entity.Property(e => e.TournamentPhone)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.TournamentStartDate).HasColumnType("datetime");

                entity.Property(e => e.TournamentTypeId).HasColumnName("TournamentTypeID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.FootballFieldType)
                    .WithMany(p => p.Tournaments)
                    .HasForeignKey(d => d.FootballFieldTypeId)
                    .HasConstraintName("FK__Tournamen__Footb__46E78A0C");

                entity.HasOne(d => d.TournamentType)
                    .WithMany(p => p.Tournaments)
                    .HasForeignKey(d => d.TournamentTypeId)
                    .HasConstraintName("FK__Tournamen__Tourn__45F365D3");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Tournaments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Tournamen__UserI__44FF419A");
            });

            modelBuilder.Entity<TournamentResult>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.Prize).HasMaxLength(256);

                entity.Property(e => e.TeamInTournamentId).HasColumnName("TeamInTournamentID");

                entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

                entity.HasOne(d => d.TeamInTournament)
                    .WithMany(p => p.TournamentResults)
                    .HasForeignKey(d => d.TeamInTournamentId)
                    .HasConstraintName("FK__Tournamen__TeamI__6E01572D");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.TournamentResults)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK__Tournamen__Tourn__6EF57B66");
            });

            modelBuilder.Entity<TournamentType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.TournamentTypeName).HasMaxLength(128);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email, "UQ__Users__A9D10534AB2A4446")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address).HasMaxLength(256);

                entity.Property(e => e.Avatar).IsUnicode(false);

                entity.Property(e => e.Bio).HasMaxLength(256);

                entity.Property(e => e.DateBan).HasColumnType("datetime");

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.DateDelete).HasColumnType("datetime");

                entity.Property(e => e.DateIssuance).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.DateUnban).HasColumnType("datetime");

                entity.Property(e => e.DateUpdate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Gender).HasMaxLength(8);

                entity.Property(e => e.IdentityCard)
                    .HasMaxLength(32)
                    .IsUnicode(false);

                entity.Property(e => e.NameBusiness).HasMaxLength(128);

                entity.Property(e => e.Phone)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.PhoneBusiness)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.RoleId).HasColumnName("RoleID");

                entity.Property(e => e.StatusBan).HasMaxLength(16);

                entity.Property(e => e.Tinbusiness)
                    .HasMaxLength(32)
                    .IsUnicode(false)
                    .HasColumnName("TINBusiness");

                entity.Property(e => e.Username).HasMaxLength(128);

                entity.HasOne(d => d.Role)
                    .WithMany(p => p.Users)
                    .HasForeignKey(d => d.RoleId)
                    .HasConstraintName("FK__Users__RoleID__3B75D760");
            });

            modelBuilder.Entity<VerifyCode>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Code).HasMaxLength(16);

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.DateExpire).HasColumnType("datetime");

                entity.Property(e => e.Email).HasMaxLength(64);
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
