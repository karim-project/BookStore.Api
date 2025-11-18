using BookStore.Api.Models;
using Mapster;

namespace BookStore.Api.Cnfigration
{
    public static class MapesterConfg
    {
        public static void RegisterMapesterConfg(this IServiceCollection services)
        {

            //TypeAdapterConfig<ApplicationUser, ApplicationUserResponse>
            //            .NewConfig()
            //            .Map(d => d.FullName, s => $"{s.FirstName} {s.LastName}")
            //            .TwoWays();

            //TypeAdapterConfig<ApplicationUser, UsersResponse>
            //            .NewConfig()
            //            .Map(d => d.FullName, s => $"{s.FirstName} {s.LastName}")
            //            .TwoWays();
        }



    }
}

