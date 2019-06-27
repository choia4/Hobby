using Microsoft.EntityFrameworkCore;

namespace Hobby.Models
{
    public class MyContext : DbContext
    {
        // base() calls the parent class' constructor passing the "options" parameter along
        public MyContext(DbContextOptions options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Hobbies> Hobbies {get;set;}
        public DbSet<HIU> Enthusiasts {get;set;}

    }
}