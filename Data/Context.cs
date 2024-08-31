using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Entities.Entities;



namespace Data
{
    public class Context : DbContext
    {
        public Context(DbContextOptions<Context> options) : base(options){}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("dbo");
            new PersonsMap().Configure(modelBuilder.Entity<Persons>());
            new RelationshipsMap().Configure(modelBuilder.Entity<Relationships>());

        }
        public virtual DbSet<Persons> Persons { get; set; }
        public virtual DbSet<Relationships> Relationships { get; set; }


    }
}
