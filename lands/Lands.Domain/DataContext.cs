
using System.Data.Entity;

namespace Lands.Domain
{
    public class DataContext : DbContext
    {
        public DataContext() : base("DefaultConnection")
        {
            
        }
    }
}
