using Application.Interfaces.Options;
using Application.Interfaces.Repositories;
using Application.Interfaces.Services;
using Application.MappingProfiles;
using Application.Services;
using Domain.Entities;
using Infrastructure.Dal.EntityFramework;
using Infrastructure.Dal.Repositories;
using Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Quartz;
using System.Text;
using Infrastructure.Jobs;
using Telegram.Bot;

namespace Infrastructure;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Logging.ClearProviders();
        builder.Logging.AddConsole();

        builder.Services.AddCors(policy => policy.AddPolicy("default", opt =>
        {
            opt.AllowAnyHeader();
            opt.AllowCredentials();
            opt.AllowAnyMethod();
            opt.SetIsOriginAllowed(_ => true);
        }));
        builder.Services.AddControllers();
        builder.Services.AddSwaggerGen();
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration
                    .GetSection("Jwt")
                    .GetSection("SecretKey").Value!))
            };
        });
        builder.Services.AddAuthorization(options =>
        {
            options.AddPolicy(nameof(User), policy =>
            {
                policy.RequireClaim(nameof(User.Id));
                policy.RequireClaim(nameof(User.TelegramId));
            });
        });
        builder.Services.AddSwaggerGen(option =>
        {
            option.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme()
            {
                In = ParameterLocation.Header,
                Description = "Введите существующий токен",
                Name = "Authorization",
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                Scheme = "Bearer"
            });
            option.AddSecurityRequirement(new OpenApiSecurityRequirement()
            {
                {
                    new OpenApiSecurityScheme()
                    {
                        Reference = new OpenApiReference()
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    }, []
                }
            });
        });

        builder.Services.AddQuartz(q =>
        {
            var recoveryEnergyJobKey = new JobKey(nameof(RecoveryEnergyJob));

            q.AddJob<RecoveryEnergyJob>(opts => opts.WithIdentity(recoveryEnergyJobKey));
            
            q.AddTrigger(opts => opts
                .ForJob(recoveryEnergyJobKey)
                .WithIdentity(recoveryEnergyJobKey.Name + "-trigger")
                .WithCronSchedule("0/1 * * * * ?"));
            
            var updateEveryDayTasksJobKey = new JobKey(nameof(UpdateEveryDayTasksJob));
            
            q.AddJob<UpdateEveryDayTasksJob>(opts => opts.WithIdentity(updateEveryDayTasksJobKey));
            
            q.AddTrigger(opts => opts
                .ForJob(updateEveryDayTasksJobKey)
                .WithIdentity(updateEveryDayTasksJobKey.Name + "-trigger")
                .WithCronSchedule("0 0 0 * * ?"));
        });
        
        builder.Services.AddQuartzHostedService(q => q.WaitForJobsToComplete = true);
        
        var connectionString = builder.Configuration.GetConnectionString("PostgreSQL");
        builder.Services.AddDbContext<BoomTokenContext>(
            o => o.UseNpgsql(connectionString)
                //.UseLoggerFactory(LoggerFactory.Create(builder => { builder.AddConsole(); }))
                //.EnableSensitiveDataLogging()
            );

        builder.Services.AddSingleton<ITelegramBotClient, TelegramBotClient>(
            sp => new TelegramBotClient(sp.GetRequiredService<IOptions<TelegramOptions>>().Value.Token));
        
        builder.Services.AddAutoMapper(typeof(ImprovementProfile));
        builder.Services.AddAutoMapper(typeof(TaskForRewardProfile));
        builder.Services.AddAutoMapper(
            (provider, expression) =>
            {
                UserProfile.Initialize(provider);
                expression.AddProfile<UserProfile>();
            },
            typeof(UserProfile).Assembly
        );
        
        builder.Services.AddScoped<IImprovementAccessRepository, ImprovementAccessRepository>();
        builder.Services.AddScoped<IImprovementRepository, ImprovementRepository>();
        builder.Services.AddScoped<IReferalUsersRepository, ReferalUsersRepository>();
        builder.Services.AddScoped<ITaskForRewardAccessRepository, TaskForRewardAccessRepository>();
        builder.Services.AddScoped<ITaskForRewardRepository, TaskForRewardRepository>();
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        
        builder.Services.AddScoped<IImprovementService, ImprovementService>();
        builder.Services.AddScoped<ITaskForRewardService, TaskForRewardService>();
        builder.Services.AddScoped<IReferalUsersService, ReferalUsersService>();
        builder.Services.AddScoped<IUserService, UserService>();

        builder.Services.AddHostedService<TelegramBotBackgroundService>();

        builder.Services.Configure<TelegramOptions>(builder.Configuration.GetSection("Telegram"));
        builder.Services.Configure<JwtOption>(builder.Configuration.GetSection("Jwt"));
        builder.Services.AddSingleton<IJwtOption>(sp =>
        {
            var jwtOption = sp.GetRequiredService<IOptions<JwtOption>>().Value;
            return new JwtOption
            {
                Issure = jwtOption.Issure,
                Expires = jwtOption.Expires,
                SecretKey = jwtOption.SecretKey
            };
        });

        var app = builder.Build();

        app.UseSwagger();
        app.UseSwaggerUI();
        app.UseCors("default");

        app.UseHttpsRedirection();

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
/*
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣀⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⡤⠒⠒⠢⣴⣿⣿⣷⠤⠄⠀⠀⠤⠤⠠⢤⣀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢠⠋⢀⠀⠀⢀⡿⠛⠋⠀⠀⢀⠀⠀⠀⠀⠀⠀⠈⠑⠢⡀⢀⣴⣿⣿⡦⠤⢄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣾⣤⡟⣇⠔⠉⠓⠂⣤⣴⢯⡿⠤⣴⣧⣀⣀⠀⠀⠀⡀⣸⣿⣿⠋⠀⠀⠀⠀⢳⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣷⡝⢠⣧⠐⢂⣿⣿⣿⣎⣼⡏⣿⣿⣳⣷⣹⡏⣥⠉⢱⣿⣿⠇⡆⣠⣄⠀⠀⣌⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢿⢁⢿⣟⣆⢼⢸⡿⠁⢴⡏⡅⣿⣏⡮⠘⣸⣿⢼⣿⣶⣿⣿⣦⡷⡟⠙⣶⡇⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠸⢸⣾⢿⣼⡌⣿⡷⠛⢶⡇⠣⡟⢻⡶⠻⠻⣆⣾⡇⣿⣿⣿⣿⡇⠀⠀⠏⢳⣿⣿⠂⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡏⣿⣜⣿⣷⢸⠀⠀⠀⠀⠀⠁⠀⠀⠀⠀⠟⣯⣧⣿⣿⡟⣿⡇⠀⠀⠀⣸⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣷⠘⢧⣻⣿⣿⠀⠀⠀⠠⠄⢄⠠⠀⠀⠀⣼⣿⣿⣿⡿⠃⣿⢹⠀⠀⠀⡟⣿⣿⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠙⡆⠀⣿⣿⣿⣶⣤⣀⠀⠀⠀⠀⢀⣠⣼⣿⣿⣿⣿⠃⠀⣹⠈⠀⠀⢰⢻⣿⣿⡆⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡖⡇⠀⣿⢿⣿⣿⣿⣿⣿⡖⠒⢚⡭⣾⣿⣿⣿⡀⠀⠀⠀⢸⢀⠀⠀⣞⣿⣿⣿⣧⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣷⣷⠀⢻⣿⣿⡿⠋⢙⢿⠳⣀⣠⣴⣿⣿⡟⠚⠓⠢⡀⠀⢸⢹⢀⡀⣿⣿⣿⣿⣾⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣼⣿⣿⢠⡘⣿⣿⠀⠀⢸⢸⢠⠛⢿⣿⢻⣿⠀⠀⠀⠀⢻⡄⢸⣸⢸⠇⣿⣿⣿⣿⣿⣿⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣠⢞⣿⡿⣿⠘⣇⢻⣿⡄⠀⣹⠈⢿⣤⡟⠻⠟⢸⠀⠀⠀⠀⢸⠃⢸⣼⢻⢰⣿⣿⣿⣿⣿⣿⣷⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣠⣴⣿⣷⠿⠋⢡⣿⠀⣿⠈⣿⣇⠀⢸⠀⠀⠛⡇⠀⠀⠸⢦⠀⠸⠶⣿⠀⢸⣼⡇⡿⣾⣿⣿⣿⠈⠻⣿⣿⣄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡠⣾⡿⠛⠉⠁⠀⠀⣼⠋⠀⢣⢧⠈⣿⣠⡇⠀⢘⠀⡗⣮⣽⣇⣿⣤⣤⣤⣿⠀⣿⣿⢸⡇⣿⣿⣿⣿⡆⠀⠙⢿⣿⣦⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣳⡏⠀⠀⠀⠀⠀⢠⣧⡇⠀⠸⣾⡄⢻⣿⣿⣿⣿⡀⣿⣿⣿⣿⣿⣿⣿⣿⡏⠀⣿⢿⣼⣶⣿⣿⣿⣿⣧⠀⠀⠈⢿⣿⣷⡀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣿⣇⠀⠀⠀⠀⢀⠞⣿⠁⠀⣷⣷⣧⠸⣿⣿⣿⣿⣇⠸⣿⣿⣿⣿⣿⣿⣿⣇⠀⡟⡌⠉⠏⣿⣿⣿⣿⣾⢇⠀⠀⠈⣿⣿⣷⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠻⣦⡀⠀⢀⢎⡼⣿⡄⠀⢹⣧⢿⠀⣿⣿⣿⣧⣹⠀⣿⣿⣿⣿⣿⣿⣿⣿⣄⣧⠁⡇⣰⣿⣿⣿⣿⣿⣿⡄⠀⠀⢸⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠉⠺⣦⢞⡾⢁⣿⠁⠀⢸⣻⣟⡇⢹⣿⣿⣿⣿⡆⡏⣿⡌⣿⣿⣿⣿⡇⢻⡼⢰⣷⣟⣿⣿⡟⠻⣿⣿⣿⣄⡀⣸⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⣫⡾⠀⡼⡏⠀⠀⠀⠙⣿⡇⢸⣿⣿⣿⣿⣷⣧⠸⣿⣿⣿⣿⣿⣷⣾⡇⢸⣿⣄⡿⣿⣿⣦⠈⢿⣿⣿⣿⣷⣿⣿⡇⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢀⢮⣿⠃⡰⣻⡇⠀⠀⠀⠀⣿⣿⠘⣿⣿⣿⣿⣿⡿⠃⢻⣿⣿⣿⣿⣿⢙⠁⠈⣿⣿⢹⠹⣿⣿⣷⠛⢿⣿⡿⣾⣿⠟⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⡾⢻⣿⡰⣱⣿⠃⠀⠀⢠⠸⣿⡇⢠⣿⣿⣿⣿⣿⣿⠀⢸⣿⣿⣿⣿⣿⣴⡀⠀⢱⡻⡜⣧⣿⣿⣿⣦⢼⣽⠿⣿⣷⡄⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⢇⡸⠋⢠⣿⡟⠀⣼⠀⡼⡇⣿⡇⣸⣿⣿⣿⣿⣿⣿⡆⢸⣿⣿⣿⣿⣿⣧⡇⠀⠀⠹⣿⣽⣿⣿⣾⣿⣟⡇⠀⢸⣿⣿⣷⣦⣀⠀⠀⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⣀⣤⠴⠋⠀⣰⣿⡿⠁⢼⠍⣼⣻⢱⣿⣷⣿⣿⣿⣿⣿⣿⣿⣿⣸⣿⣿⣿⣿⣿⣿⣷⡀⠀⠀⠙⢿⣿⣿⣿⡇⢀⠇⠀⣿⣿⣿⣿⣿⣿⣷⣄⠀⠀⠀⠀⠀⠀
⠀⠀⠀⠀⠀⡀⠤⣤⣶⡿⢛⣵⡶⢋⣴⣿⠟⣠⡾⢃⣼⠟⣫⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣿⣿⣿⣿⣿⣿⣿⣿⣧⠙⢦⣄⠘⣿⣿⣟⡀⣜⣀⣼⣿⣿⣿⣿⣿⣿⣿⣿⣧⠀⠀⠀⠀⠀
⠀⠀⢀⢔⣭⣶⣿⣿⢥⣾⠟⠋⣰⣿⣿⣿⠞⣫⣴⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣦⠀⠻⣷⣮⣿⣦⡹⠀⣸⣿⣿⣿⣿⣿⣿⣿⣿⣿⡟⠀⠀⠀⠀⠀
⠀⢠⣻⣿⣿⣿⣿⢞⣋⣤⣴⢿⡻⣯⣿⣵⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣷⣄⡀⠙⠻⠿⣿⣶⣿⣿⣿⣿⣿⣿⣿⣿⣿⡏⠀⠀⠀⠀⠀⠀
⠀⠘⣿⢸⣿⣿⣿⣟⠿⣹⣶⣷⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⡿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣶⣤⢄⣀⡈⠉⠀⠂⠀⠉⠉⠛⠛⢿⣤⣤⣤⣄⣀⡀⠀
⠀⠀⠈⠛⢿⣿⣿⣡⣾⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⢿⣾⣎⠻⣿⣿⣿⣿⣿⣿⣿⡿⣿⠿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⣿⠿⣞⣭⠍⠓⠒⣒⣒⣲⣶⣶⣿⠟⠛⠛⣉⣿⣿⠆
⢠⣶⣾⣿⣿⣛⡛⠛⢛⣻⣿⣿⣿⣿⣟⣿⣯⡉⠁⠀⠀⠀⠀⠙⠿⠽⣾⢽⡿⠝⠺⢝⡇⠀⢜⡩⠖⠋⡿⣿⣿⢿⡿⣚⠷⠚⠈⠙⢿⣷⡀⠉⠙⠛⠒⠒⠛⠛⠛⠋⠁⠀⠀⠸⠟⠉⠁⠀
⢀⣙⣛⣛⣯⣿⡿⢶⢽⡟⠛⠛⠉⠉⠉⠙⠉⠀⠀⠀⠀⠀⠀⠀⠀⠀⠈⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠘⠒⠁⠀⠊⠃⠀⠉⠀⢠⣤⡴⠟⠁⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀⠀
*/