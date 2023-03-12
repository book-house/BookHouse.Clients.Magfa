using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace BookHouse.Clients.Magfa.AspNetCore
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddMagfaClient(this IServiceCollection services, Action<MagfaClientOptions> options)
        {
            if (options == null)
                throw new ArgumentNullException(nameof(options));


            var option = new MagfaClientOptions();
            options?.Invoke(option);

            services.AddSingleton(option);
            services.AddSingleton<IMagfaV2Client, MagfaV2Client>();

            return services;
        }
    }
}
