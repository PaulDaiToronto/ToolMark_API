using Microsoft.EntityFrameworkCore;
using TMWebApi.Models;


namespace TMWebApi.Repository
{
    public class APIDbContext : DbContext
    {
        public APIDbContext(DbContextOptions<APIDbContext> options) : base(options)
        {

        }

        public DbSet<Category> Categorys { get; set; }
        public DbSet<Toolmark> Toolmarks { get; set; }


    }

}
