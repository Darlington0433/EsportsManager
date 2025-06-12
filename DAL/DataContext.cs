using Microsoft.EntityFrameworkCore;
using EsportManager.BL.Models;

namespace EsportManager.DAL
{
    public class DataContext : DbContext
    {
        public DbSet<User> Users { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }
        public DbSet<MatchResult> MatchResults { get; set; }
        public DbSet<Tournament> Tournaments { get; set; }
        public DbSet<Sponsor> Sponsors { get; set; }
        public DbSet<TeamSponsor> TeamSponsors { get; set; }
        public DbSet<Game> Games { get; set; }
        public DbSet<UserTournament> UserTournaments { get; set; }
        public DbSet<Award> Awards { get; set; }
        public DbSet<News> News { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<Statistic> Statistics { get; set; }
        public DbSet<Schedule> Schedules { get; set; }
        public DbSet<Referee> Referees { get; set; }

        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            // Add Fluent API configurations here if needed
        }
    }
}