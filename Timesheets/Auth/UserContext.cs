using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timesheets.Auth
{
    public class UserContext : IdentityDbContext<User,Role,int>
    {
        public UserContext(DbContextOptions<UserContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<IdentityUserLogin<int>>(x =>
            {
                x.Property(l => l.LoginProvider).HasMaxLength(128);
                x.Property(l => l.ProviderKey).HasMaxLength(128);
            });
            builder.Entity<IdentityUserToken<int>>(x =>
            {
                x.Property(t => t.LoginProvider).HasMaxLength(128);
                x.Property(t => t.Name).HasMaxLength(128);
            });

            foreach (var entityType in builder.Model.GetEntityTypes())
            {
                foreach (var property in entityType.GetProperties())
                {
                    if(property.ClrType == typeof(bool))
                    {
                        property.SetValueConverter(new BoolToIntConverter());
                    }
                }
            }
        }
    }
}
