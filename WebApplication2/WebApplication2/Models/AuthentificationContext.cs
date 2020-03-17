using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication2.Models
{
    public class AuthentificationContext: IdentityDbContext
    {
        public AuthentificationContext(DbContextOptions<AuthentificationContext> options):base(options)
        {
            
        }

        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<Subscription> subscriptions { get; set; }
        public DbSet<QuestionAndAnswer> questionAndAnswers { get; set; }
    }
}
