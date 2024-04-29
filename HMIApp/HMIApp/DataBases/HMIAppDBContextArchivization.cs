using HMIApp.Components.DataBase;
using Microsoft.EntityFrameworkCore;

namespace HMIApp.Data
{
    public class HMIAppDBContextArchivization : DbContext
    {
        //konstruktor dbcontextu
        public HMIAppDBContextArchivization(DbContextOptions<HMIAppDBContextArchivization> options)
            : base(options)
        {
        }

        public DbSet<ArchivizationModelExtendedDataBase> ArchivizationsForParameters {get; set;}

    }
}
