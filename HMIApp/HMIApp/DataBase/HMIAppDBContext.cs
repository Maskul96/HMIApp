﻿using HMIApp.Components.DataBase;
using Microsoft.EntityFrameworkCore;

namespace HMIApp.Data
{
    public class HMIAppDBContext : DbContext
    {
        //konstruktor dbcontextu
        public HMIAppDBContext(DbContextOptions<HMIAppDBContext> options)
            : base(options)
        {
        }

        public DbSet<Reference> References {get; set;}

    }
}
