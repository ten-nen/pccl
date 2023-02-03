
using Pccl.ProjectTemplate.Api;
using Pccl.Audit;
using Pccl.ProjectTemplate.Infrastructure.Data;
using Pccl.Auth;
using LogDashboard;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;
using Serilog;
using Pccl.ProjectTemplate.Application;
using Pccl.Repository;
using Pccl.BackgroundWorker;
using Pccl.AutoDI;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Pccl.Cache;
using Pccl.EventBus.AspNetCore;

Log.Logger = new LoggerConfiguration()
#if DEBUG
                .WriteTo.Console()
#endif
                .CreateLogger();

try
{
    Log.Information("Starting web host.");

    var builder = WebApplication.CreateBuilder(args);

    builder.Host.UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .WriteTo.File($"{AppContext.BaseDirectory}logs/.log", rollingInterval: RollingInterval.Day, outputTemplate: "{Timestamp:HH:mm} || {Level} || {SourceContext:l} || {Message} || {Exception} ||end {NewLine}")
#if DEBUG
                    .WriteTo.Console()
#endif
                    );

    #region Configure Services

    //≈‰÷√ª∫¥Ê
    builder.Services.AddCache();

    //≈‰÷√…Ûº∆
    builder.Services.AddAudit();

    //≈‰÷√EventBus
    builder.Services.AddEventBus(typeof(MappingProfile).Assembly);

    //≈‰÷√BackgroundWorker
    builder.Services.AddBackgroundWorker(typeof(Program).Assembly);

    //≈‰÷√ ˝æ›ø‚
    builder.Services.AddDbContext<DatabaseContext>((serviceProvider, options) =>
#if (mysql)
                options.UseMySql(builder.Configuration.GetConnectionString("Default"), MySqlServerVersion.LatestSupportedServerVersion)
#elif (sqlite)     
                options.UseSqlite(builder.Configuration.GetConnectionString("Default"))
#else
                options.UseSqlServer(builder.Configuration.GetConnectionString("Default"))
#endif
                                       .AddAuditInterceptors(serviceProvider));

    //≈‰÷√ ˝æ›≤÷¥¢ uow
    builder.Services.AddRepositories(typeof(DatabaseContext).Assembly);

    //≈‰÷√service£¨ π”√◊‘∂Ø◊¢»Î
    builder.Services.AddAutoRegisterServices(typeof(MappingProfile).Assembly);

    //AutoMapper
    builder.Services.AddAutoMapper(typeof(MappingProfile));

    //»’÷æ√Ê∞Â
    builder.Services.AddLogDashboard(options =>
    {
        options.AddAuthorizationFilter(new LogDashboard.Authorization.Filters.LogDashboardRoleAuthorizeFilter(new List<string>() { "admin" }));
    });

    builder.Services.AddHttpContextAccessor();
    builder.Services.AddControllers();

    //≈‰÷√»œ÷§º¯»®
    builder.Services.AddAuth<ApiPermissionChecker>(builder.Configuration);

    // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
    builder.Services.AddEndpointsApiExplorer();
    //≈‰÷√Swagger
    builder.Services.AddSwaggerGen(options =>
    {
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "Pccl.ProjectTemplate API", Version = "v1" });
        options.DocInclusionPredicate((docName, description) => true);
        options.CustomSchemaIds(type => type.FullName);
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Description = @"JWT Authorization header using the Bearer scheme. \r\n\r\n 
                          Enter 'Bearer' [space] and then your token in the text input below.
                          \r\n\r\nExample: 'Bearer 12345abcdef'",
            Name = "Authorization",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.ApiKey,
            Scheme = "Bearer"
        });

        options.AddSecurityRequirement(new OpenApiSecurityRequirement(){
                                                        {
                                                         new OpenApiSecurityScheme{Reference = new OpenApiReference{Type = ReferenceType.SecurityScheme,Id = "Bearer"}},Array.Empty<string>()
                                                        }});
    }
    );

    //≈‰÷√øÁ”Ú
    builder.Services.AddCors(options =>
    {
        options.AddDefaultPolicy(policy =>
        {
            policy
                .WithOrigins(
                    builder.Configuration["App:CorsOrigins"]
                        .Split(",", StringSplitOptions.RemoveEmptyEntries)
                        .ToArray()
                )
                .SetIsOriginAllowedToAllowWildcardSubdomains()
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials();
        });
    });
    #endregion

    var app = builder.Build();

    #region Configure

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            //options.RoutePrefix = "";
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Pccl.ProjectTemplate API");
        });
    }

    //app.UseHttpsRedirection();

    app.UseStaticFiles();

    app.UseBackgroundWorker();

    app.UseAudit();

    app.UseRouting();

    app.UseCors();

    app.UseAuth();

    app.UseLogDashboard();

    app.MapControllers();

    #endregion

    using (var scope = app.Services.CreateScope())
    {
        var scopedProvider = scope.ServiceProvider;
        var actionDescriptorCollectionProvider = scopedProvider.GetRequiredService<IActionDescriptorCollectionProvider>();
        var permissionDefinitionProvider = new RoutePermissionDefinitionProvider(actionDescriptorCollectionProvider, builder.Configuration);
        await DatabaseContextSeed.SeedRoutePermissionsAsync(permissionDefinitionProvider);
    }

    app.Run();
    return 0;
}
catch (Exception ex)
{
    Log.Fatal(ex, "Host terminated unexpectedly!");
    return 1;
}
finally
{
    Log.CloseAndFlush();
}