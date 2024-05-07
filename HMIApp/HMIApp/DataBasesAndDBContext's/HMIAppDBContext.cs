using HMIApp.Components.DataBase;
using Microsoft.EntityFrameworkCore;

namespace HMIApp.Data
{
    public class HMIAppDBContext : DbContext
    {
        //konstruktor dbcontextu
        //public HMIAppDBContext(DbContextOptions<HMIAppDBContext> options)
        //    : base(options)
        //{
        //}
        DataBase DataBase = new DataBase();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            DataBase.Run();
            optionsBuilder.UseSqlServer(DataBase.ConnectionString);
        }

        public DbSet<Reference> References {get; set;}

    }
}
