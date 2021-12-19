using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Domain.Common;
using Application.Interfaces;
using Infrastructure.Identity.Models;

namespace Infrastructure.Identity.Contexts
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IDateTimeService _dateTime;
        private readonly ICurrentUser _currentUser;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {

        }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IDateTimeService dateTime, ICurrentUser currentUser) : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            _dateTime = dateTime;
            _currentUser = currentUser;
        }

        #region MODELS
        public DbSet<ModelTenant> Tenants { get; set; }
        public DbSet<ModelUser> Users { get; set; }
        public DbSet<ModelRole> Roles { get; set; }
        public DbSet<ModelService> MicroServices { get; set; }
        public DbSet<ModelPermission> Permissions { get; set; }
        public DbSet<ModelRefreshToken> RefreshTokens { get; set; }
        public DbSet<ModelAccessToken> AccessTokens { get; set; }
        #endregion

        #region LINK-MODELS
        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<UserService> UserMicroServices { get; set; }
        public DbSet<TenantService> TenantMicroServices { get; set; }
        public DbSet<TenantUser> TenantUsers { get; set; }
        public DbSet<PermissionRole> PermissionRoles { get; set; }
        public DbSet<PermissionTenant> PermissionTenants { get; set; }
        #endregion

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            foreach (var entry in ChangeTracker.Entries<IAuditableEntity>())
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        entry.Entity.CreatedOn = _dateTime.NowUtc;
                        entry.Entity.CreatedBy = _currentUser.UserId;
                        break;
                    case EntityState.Modified:
                        entry.Entity.LastModifiedOn = _dateTime.NowUtc;
                        entry.Entity.LastModifiedBy = _currentUser.UserId;
                        break;
                }
            }
            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region MODELS
            builder.Entity<ModelUser>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Получаем только записи текущей организации пользователя
                //entity.HasQueryFilter(b => b.TenantId == _currentUser.TenantId);
            });

            builder.Entity<ModelRole>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Получаем только записи текущей организации пользователя
                entity.HasQueryFilter(b => b.TenantId == _currentUser.TenantId);
            });

            builder.Entity<ModelTenant>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Получаем только записи текущей организации пользователя
                entity.HasQueryFilter(b => b.TenantId == _currentUser.TenantId);
            });

            builder.Entity<ModelAccessToken>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Получаем только записи текущей организации пользователя
                entity.HasQueryFilter(b => b.User.TenantId == _currentUser.TenantId);
            });

            builder.Entity<ModelRefreshToken>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Получаем только записи текущей организации пользователя
                entity.HasQueryFilter(b => b.User.TenantId == _currentUser.TenantId);
            });

            builder.Entity<ModelPermission>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();
            });

            builder.Entity<ModelService>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedOnAdd();

                // Получаем только записи текущей организации пользователя
                entity.HasQueryFilter(b => b.Tenants.Any(a => a.TenantId == _currentUser.TenantId));
            });
            #endregion

            #region LINK-MODELS
            builder.Entity<UserRole>(entity =>
            {
                builder.Entity<UserRole>().HasKey(k => new { k.UserId, k.RoleId });

                // Получаем только записи текущей организации пользователя
                entity.HasQueryFilter(b => b.Role.TenantId == _currentUser.TenantId);
            });


            builder.Entity<UserService>(entity =>
            {
                builder.Entity<UserService>().HasKey(k => new { k.UserId, k.MicroServiceId });
            });

            builder.Entity<TenantService>(entity =>
            {
                builder.Entity<TenantService>().HasKey(k => new { k.TenantId, k.MicroServiceId });
            });

            builder.Entity<TenantUser>(entity =>
            {
                builder.Entity<TenantUser>().HasKey(k => new { k.TenantId, k.UserId });
            });

            builder.Entity<PermissionRole>(entity =>
            {
                builder.Entity<PermissionRole>().HasKey(k => new { k.PermissionId, k.RoleId });
            });

            builder.Entity<PermissionTenant>(entity =>
            {
                builder.Entity<PermissionTenant>().HasKey(k => new { k.PermissionId, k.TenantId });
            });
            #endregion
        }
    }
}
