using Microsoft.EntityFrameworkCore;

namespace CRUD_Operations_Project.Models.Services
{
    public class ApplicationDbContext:DbContext
    {
        public ApplicationDbContext(DbContextOptions options) : base(options)   
        {
            
        }
        public DbSet<Product> Products { get; set; }
    }
}
