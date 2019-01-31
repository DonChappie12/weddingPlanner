using System;
using Microsoft.EntityFrameworkCore;

namespace wedding.Models
{
    public class UserContext : DbContext
    {
        public DbSet<User> user { get; set; }
        public DbSet<Wedding> wedding { get; set; }
        public DbSet<Attendees> attendees { get; set; }
        public UserContext(DbContextOptions<UserContext> options) : base(options) { }
    }
}