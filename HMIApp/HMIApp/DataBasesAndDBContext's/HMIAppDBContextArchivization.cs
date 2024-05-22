using HMIApp.Components.DataBase;
using Microsoft.EntityFrameworkCore;

namespace HMIApp.Data
{
    public class HMIAppDBContextArchivization : DbContext
    {
        public DataBaseArchivization _databaseArchive = new DataBaseArchivization();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            _databaseArchive.Run();
            optionsBuilder.UseSqlServer(_databaseArchive.ConnectionString);
        }

        public DbSet<ArchivizationModelExtendedDataBase> ArchivizationsForParameters {get; set;}
    }
}
