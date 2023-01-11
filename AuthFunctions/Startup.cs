using AuthFunctions.Data.Contexts;
using AuthFunctions.Data.Repositories;
using AuthFunctions.Data.UnitOfWorks;
using AuthFunctions.Domain.Dtos;
using AuthFunctions.Domain.Validators;
using FluentValidation;
using Microsoft.Azure.Functions.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

[assembly: FunctionsStartup(typeof(AuthFunctions.Startup))]

namespace AuthFunctions
{
    public class Startup : FunctionsStartup
    {
        public override void Configure(IFunctionsHostBuilder builder)
        {
            builder.Services.AddDbContext<AuthDbContext>(options =>
                options.UseCosmos(
                    Environment.GetEnvironmentVariable("CosmosAccountEndpoint"),
                    Environment.GetEnvironmentVariable("CosmosAccountKey"),
                    Environment.GetEnvironmentVariable("CosmosDatabaseName")));

            builder.Services.AddScoped<IValidator<RegisterDto>, RegisterDtoValidator>();
            builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();

            builder.Services.AddScoped<IUserRepository, UserRepository>();
        }
    }
}
