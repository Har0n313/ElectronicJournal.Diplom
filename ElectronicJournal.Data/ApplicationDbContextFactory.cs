using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicJournal.Domain
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            // Укажите вашу строку подключения ниже!
            optionsBuilder.UseNpgsql("Host=localhost;Port=5432;Database=ElectronicJournalsV1;Username=postgres;Password=mysecretpassword");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
