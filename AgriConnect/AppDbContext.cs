using AgriConnect.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
<<<<<<< agriBlog
    public DbSet<Blog>    Blogs    { get; set; }
    public DbSet<Comment> Comments { get; set; }
=======

    public DbSet<Consultation> Consultations { get; set; }
>>>>>>> dev
}