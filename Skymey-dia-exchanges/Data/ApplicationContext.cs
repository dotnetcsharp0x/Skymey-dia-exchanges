using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.EntityFrameworkCore.Extensions;
using System.Collections.Generic;
using System.Reflection.Emit;

namespace Skymey_dia_exchanges.Data
{
    public class ApplicationContext : DbContext
    {
        public DbSet<Skymey_main_lib.Models.Exchanges> Exchanges { get; init; }
        public static ApplicationContext Create(IMongoDatabase database) =>
            new(new DbContextOptionsBuilder<ApplicationContext>()
                .UseMongoDB(database.Client, database.DatabaseNamespace.DatabaseName)
                .Options);
        public ApplicationContext(DbContextOptions options)
            : base(options)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Skymey_main_lib.Models.Exchanges>().ToCollection("exchanges");
        }
    }
}
