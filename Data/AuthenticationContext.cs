using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using demo_test_api.Models;
using System.Reflection.Metadata;

namespace demo_test_api.Data
{
    public class AuthenticationContext : DbContext
    {
        public AuthenticationContext (DbContextOptions<AuthenticationContext> options)
            : base(options)
        {
        }

        public DbSet<Authentication> Authentications { get; set; } = default!;
    }
}
