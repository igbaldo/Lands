
using System.Data.Entity;

namespace Lands.Domain
{
    public class DataContext : DbContext
    {
        public DataContext() : base("DefaultConnection")
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<GroupTeam> GroupTeams { get; set; }
    } 
}
