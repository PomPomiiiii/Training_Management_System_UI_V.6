using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Scalar.AspNetCore;
using System.Threading.RateLimiting;
using TrainingManagementSystem.Api.Common.Configurations;
using TrainingManagementSystem.Api.Data;
using TrainingManagementSystem.Api.Repositories.AttendeeRepository;
using TrainingManagementSystem.Api.Repositories.MaterialRepository;
using TrainingManagementSystem.Api.Repositories.TrainingRepository;
using TrainingManagementSystem.Api.Repositories.UnitOfWork;
using TrainingManagementSystem.Api.Repositories.UserRepository;
using TrainingManagementSystem.Api.Services.AuthService;
using TrainingManagementSystem.Api.Services.EmailService;
using TrainingManagementSystem.Api.Services.FileStorageService;
using TrainingManagementSystem.Api.Services.MaterialService;
using TrainingManagementSystem.Api.Services.TrainingService;
using TrainingManagementSystem.Api.Services.UserService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<AppDbContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
});

// Change SameSite from Strict to None for cross-origin requests
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.Cookie.HttpOnly = true;
        options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
        options.Cookie.SameSite = SameSiteMode.None;  // changed from Strict to None
        options.ExpireTimeSpan = TimeSpan.FromMinutes(30);
        options.SlidingExpiration = true;

        options.Events.OnRedirectToLogin = context =>
        {
            context.Response.StatusCode = 401;
            return context.Response.WriteAsync("Unauthorized");
        };

        options.Events.OnRedirectToAccessDenied = context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            return context.Response.WriteAsync("Forbidden");
        };
    });

builder.Services.AddRateLimiter(rateLimitterOptions =>
{
    //global 
    //rateLimitterOptions.AddFixedWindowLimiter("fixed", options =>
    //{
    //    options.Window = TimeSpan.FromMinutes(60);
    //    options.QueueLimit = 0;
    //    options.PermitLimit = 5;
    //    options.QueueProcessingOrder = QueueProcessingOrder.OldestFirst;
    //});

    rateLimitterOptions.OnRejected = async (context, token) =>
    {
        context.HttpContext.Response.StatusCode = StatusCodes.Status429TooManyRequests;
        await context.HttpContext.Response.WriteAsync("Too many request");
    };

    rateLimitterOptions.AddPolicy("login-policy", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                QueueLimit = 0,
                Window = TimeSpan.FromMinutes(60)
            }
        )
    );
});

builder.Services.AddControllers();

builder.Services.AddCors(options => 
{
    options.AddPolicy("FrontEndSample", policy =>
    {
        policy.WithOrigins(
            "https://localhost:7003",   // Backend (https)
            "https://localhost:5007",
            "https://localhost:7076",   // My frontend (https)
            "http://localhost:5026"
        );
        policy.AllowAnyMethod();
        policy.AllowAnyHeader();
        policy.AllowCredentials();
    });
}); 

builder.Services.AddAntiforgery(options => 
{
    options.HeaderName = "X-CSRF-TOKEN";
});

builder.Services.AddAuthorization();

//options
builder.Services.AddOptions<EmailSettings>()
    .Bind(builder.Configuration.GetSection("EmailSettings"))
    .ValidateOnStart();


//services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<IEmailService, EmailService>();

builder.Services.AddScoped<IMaterialService, MaterialService>();
builder.Services.AddScoped<IMaterialRepository, MaterialRepository>();

builder.Services.AddScoped<IFileStorageService, FileStorageService>();

builder.Services.AddScoped<ITrainingRepository, TrainingRepository>();
builder.Services.AddScoped<ITrainingService, TrainingService>();

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IAttendeeRepository, AttendeeRepository>();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference();
}


app.UseStaticFiles();

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseCors("FrontEndSample");

app.UseAuthentication();
app.UseAuthorization();

//to valdiate antiforgery in every post,patch,delete, and put method request
//app.Use(async (context, next) =>
//{
//    var path = context.Request.Path.Value?.ToLower();

//    var isAuthEndpoint =
//        path!.Contains("/api/auth/login") ||
//        path.Contains("/api/auth/logout");

//    if (!isAuthEndpoint && 
//        (HttpMethods.IsPut(context.Request.Method) ||
//        HttpMethods.IsPatch(context.Request.Method) ||
//        HttpMethods.IsDelete(context.Request.Method) ||
//        HttpMethods.IsPost(context.Request.Method)))
//    {
//        var antiforgery = context.RequestServices.GetRequiredService<IAntiforgery>();

//        try
//        {
//            await antiforgery.ValidateRequestAsync(context);
//        }
//        catch (AntiforgeryValidationException ex)
//        {
//            Console.WriteLine(ex.Message);
//            context.Response.StatusCode = StatusCodes.Status400BadRequest;
//            await context.Response.WriteAsync("Invalid CSRF token.");
//            return;
//        }
//    }

//    await next();
//});

app.MapControllers();

app.Run();
