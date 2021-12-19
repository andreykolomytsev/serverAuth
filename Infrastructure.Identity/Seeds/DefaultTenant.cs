using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Infrastructure.Identity.Contexts;
using Infrastructure.Identity.Models;

namespace Infrastructure.Identity.Seeds
{
    public static class DefaultTenant
    {
        public static async Task SeedAsync(ApplicationDbContext dbContext)
        {
            // Создаем организацию по умолчанию
            var defaultTenant = new ModelTenant
            {
                Id = "780c0562-3e70-40ad-ae3b-3a1e444bc562",
                FullName = "Администраторы",
                Email = "test@admin.com",
                Phone = "",
                Address = "Россия, Санкт-Петербург",
                INN = "123455678",
                KPP = "87654321",
                OGRN = "111111222233333",
                OKPO = "123336667711",
                TenantId = "780c0562-3e70-40ad-ae3b-3a1e444bc562"
            };

            if (await dbContext.Tenants.IgnoreQueryFilters().FirstOrDefaultAsync(x => x.Id == "780c0562-3e70-40ad-ae3b-3a1e444bc562") == null)
            {
                await dbContext.Tenants.AddAsync(defaultTenant);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
