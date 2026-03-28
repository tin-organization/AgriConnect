using AgriConnect.Models;
using Microsoft.EntityFrameworkCore;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Blog>    Blogs    { get; set; }
    public DbSet<Comment> Comments { get; set; }


    public DbSet<Consultation> Consultations { get; set; }

    public DbSet<Produce> Produces { get; set; }

    public DbSet<Equipment> Equipments { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<Income> Incomes { get; set; }
}