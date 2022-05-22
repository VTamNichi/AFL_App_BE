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

        public virtual DbSet<FootballFieldType> FootballFieldTypes { get; set; } = null!;
        public virtual DbSet<FootballPlayer> FootballPlayers { get; set; } = null!;
        public virtual DbSet<Image> Images { get; set; } = null!;
        public virtual DbSet<Match> Matchs { get; set; } = null!;
        public virtual DbSet<MatchDetail> MatchDetails { get; set; } = null!;
        public virtual DbSet<News> News { get; set; } = null!;
        public virtual DbSet<PlayerInTeam> PlayerInTeams { get; set; } = null!;
        public virtual DbSet<PlayerInTournament> PlayerInTournaments { get; set; } = null!;
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
                optionsBuilder.UseSqlServer("Data Source=asl.crurou7dzdf4.ap-southeast-1.rds.amazonaws.com;Initial Catalog=AmateurFootballLeague;Persist Security Info=True;User ID=ktat;Password=KhoaTuAnhTam");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<FootballFieldType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.FootballFieldTypeName).HasMaxLength(128);
            });

            modelBuilder.Entity<FootballPlayer>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.DateDelete).HasColumnType("datetime");

                entity.Property(e => e.DateOfBirth).HasColumnType("datetime");

                entity.Property(e => e.DateUpdate).HasColumnType("datetime");

                entity.Property(e => e.Email)
                    .HasMaxLength(64)
                    .IsUnicode(false);

                entity.Property(e => e.Gender).HasMaxLength(8);

                entity.Property(e => e.Phone)
                    .HasMaxLength(16)
                    .IsUnicode(false);

                entity.Property(e => e.PlayerAvatar).IsUnicode(false);

                entity.Property(e => e.PlayerName).HasMaxLength(128);
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

                entity.Property(e => e.NewsId).HasColumnName("NewsID");

                entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

                entity.HasOne(d => d.News)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.NewsId)
                    .HasConstraintName("FK__Images__NewsID__4D94879B");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.Images)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK__Images__Tourname__4E88ABD4");
            });

            modelBuilder.Entity<Match>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Fight).HasMaxLength(16);

                entity.Property(e => e.GroupFight).HasMaxLength(16);

                entity.Property(e => e.MatchDate).HasColumnType("datetime");

                entity.Property(e => e.Round).HasMaxLength(16);

                entity.Property(e => e.Status).HasMaxLength(16);

                entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.Matches)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK__Matchs__Tourname__47DBAE45");
            });

            modelBuilder.Entity<MatchDetail>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.MatchId).HasColumnName("MatchID");

                entity.Property(e => e.PlayerInTeamId).HasColumnName("PlayerInTeamID");

                entity.HasOne(d => d.Match)
                    .WithMany(p => p.MatchDetails)
                    .HasForeignKey(d => d.MatchId)
                    .HasConstraintName("FK__MatchDeta__Match__628FA481");

                entity.HasOne(d => d.PlayerInTeam)
                    .WithMany(p => p.MatchDetails)
                    .HasForeignKey(d => d.PlayerInTeamId)
                    .HasConstraintName("FK__MatchDeta__Playe__6383C8BA");
            });

            modelBuilder.Entity<News>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Content).HasMaxLength(256);

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.DateDelete).HasColumnType("datetime");

                entity.Property(e => e.DateUpdate).HasColumnType("datetime");

                entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.News)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK__News__Tournament__4AB81AF0");
            });

            modelBuilder.Entity<PlayerInTeam>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.FootballPlayerId).HasColumnName("FootballPlayerID");

                entity.Property(e => e.Status).HasMaxLength(16);

                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.HasOne(d => d.FootballPlayer)
                    .WithMany(p => p.PlayerInTeams)
                    .HasForeignKey(d => d.FootballPlayerId)
                    .HasConstraintName("FK__PlayerInT__Footb__5812160E");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.PlayerInTeams)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__PlayerInT__TeamI__571DF1D5");
            });

            modelBuilder.Entity<PlayerInTournament>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.PlayerInTeamId).HasColumnName("PlayerInTeamID");

                entity.Property(e => e.TeamInTournamentId).HasColumnName("TeamInTournamentID");

                entity.HasOne(d => d.PlayerInTeam)
                    .WithMany(p => p.PlayerInTournaments)
                    .HasForeignKey(d => d.PlayerInTeamId)
                    .HasConstraintName("FK__PlayerInT__Playe__5BE2A6F2");

                entity.HasOne(d => d.TeamInTournament)
                    .WithMany(p => p.PlayerInTournaments)
                    .HasForeignKey(d => d.TeamInTournamentId)
                    .HasConstraintName("FK__PlayerInT__TeamI__5AEE82B9");
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

                entity.Property(e => e.Status).HasMaxLength(16);

                entity.Property(e => e.TeamAscore).HasColumnName("TeamAScore");

                entity.Property(e => e.TeamBscore).HasColumnName("TeamBScore");

                entity.Property(e => e.TeamInMatchAid).HasColumnName("TeamInMatchAID");

                entity.Property(e => e.TeamInMatchBid).HasColumnName("TeamInMatchBID");

                entity.Property(e => e.UserId).HasColumnName("UserID");

                entity.HasOne(d => d.Match)
                    .WithMany(p => p.ScorePredictions)
                    .HasForeignKey(d => d.MatchId)
                    .HasConstraintName("FK__ScorePred__Match__693CA210");

                entity.HasOne(d => d.TeamInMatchA)
                    .WithMany(p => p.ScorePredictionTeamInMatchAs)
                    .HasForeignKey(d => d.TeamInMatchAid)
                    .HasConstraintName("FK__ScorePred__TeamI__66603565");

                entity.HasOne(d => d.TeamInMatchB)
                    .WithMany(p => p.ScorePredictionTeamInMatchBs)
                    .HasForeignKey(d => d.TeamInMatchBid)
                    .HasConstraintName("FK__ScorePred__TeamI__6754599E");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.ScorePredictions)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__ScorePred__UserI__68487DD7");
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

                entity.Property(e => e.NextTeam).HasMaxLength(32);

                entity.Property(e => e.Result).HasMaxLength(16);

                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.Property(e => e.TeamName).HasMaxLength(128);

                entity.HasOne(d => d.Match)
                    .WithMany(p => p.TeamInMatches)
                    .HasForeignKey(d => d.MatchId)
                    .HasConstraintName("FK__TeamInMat__Match__5FB337D6");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TeamInMatches)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__TeamInMat__TeamI__5EBF139D");
            });

            modelBuilder.Entity<TeamInTournament>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Status).HasMaxLength(16);

                entity.Property(e => e.TeamId).HasColumnName("TeamID");

                entity.Property(e => e.TournamentId).HasColumnName("TournamentID");

                entity.HasOne(d => d.Team)
                    .WithMany(p => p.TeamInTournaments)
                    .HasForeignKey(d => d.TeamId)
                    .HasConstraintName("FK__TeamInTou__TeamI__5441852A");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.TeamInTournaments)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK__TeamInTou__Tourn__534D60F1");
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
                    .HasConstraintName("FK__Tournamen__Footb__44FF419A");

                entity.HasOne(d => d.TournamentType)
                    .WithMany(p => p.Tournaments)
                    .HasForeignKey(d => d.TournamentTypeId)
                    .HasConstraintName("FK__Tournamen__Tourn__440B1D61");

                entity.HasOne(d => d.User)
                    .WithMany(p => p.Tournaments)
                    .HasForeignKey(d => d.UserId)
                    .HasConstraintName("FK__Tournamen__UserI__4316F928");
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
                    .HasConstraintName("FK__Tournamen__TeamI__6C190EBB");

                entity.HasOne(d => d.Tournament)
                    .WithMany(p => p.TournamentResults)
                    .HasForeignKey(d => d.TournamentId)
                    .HasConstraintName("FK__Tournamen__Tourn__6D0D32F4");
            });

            modelBuilder.Entity<TournamentType>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Description).HasMaxLength(256);

                entity.Property(e => e.TournamentTypeName).HasMaxLength(128);
            });

            modelBuilder.Entity<User>(entity =>
            {
                entity.HasIndex(e => e.Email, "UQ__Users__A9D105343C66ED2E")
                    .IsUnique();

                entity.Property(e => e.Id).HasColumnName("ID");

                entity.Property(e => e.Address).HasMaxLength(256);

                entity.Property(e => e.Avatar).IsUnicode(false);

                entity.Property(e => e.Bio).HasMaxLength(256);

                entity.Property(e => e.DateBan).HasColumnType("datetime");

                entity.Property(e => e.DateCreate).HasColumnType("datetime");

                entity.Property(e => e.DateDelete).HasColumnType("datetime");

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
                    .HasConstraintName("FK__Users__RoleID__398D8EEE");
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
