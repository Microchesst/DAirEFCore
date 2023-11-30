using Microsoft.EntityFrameworkCore;
using DAir.Context; // Replace with your actual DbContext's namespace
using DAir.Models;
using DAir.Services; // Replace with your actual Models' namespace
using Microsoft.OpenApi.Models;
using Serilog;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container

builder.Services.Configure<LogsDatabaseSettings>(
    builder.Configuration.GetSection("LogDatabase"));

builder.Services.AddSingleton<LogService>();

// Add DbContext
builder.Services.AddDbContext<DAirDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Add Identity service 
builder.Services.AddIdentity<ApiUser, IdentityRole>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
})
.AddEntityFrameworkStores<DAirDbContext>();

builder.Services.AddAuthentication(options => {
    options.DefaultAuthenticateScheme =
    options.DefaultChallengeScheme =
    options.DefaultForbidScheme =
    options.DefaultScheme =
    options.DefaultSignInScheme =
    options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options => {
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidIssuer = builder.Configuration["JWT:Issuer"],
        ValidateAudience = false,
        ValidAudience = builder.Configuration["JWT:Audience"],
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(
    System.Text.Encoding.UTF8.GetBytes("MyVeryOwnTestSigningKey123$"))
    };
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("adminPolicy", policyBuilder =>
        policyBuilder.RequireClaim("admin", "true"));

    options.AddPolicy("pilotPolicy", policyBuilder =>
    policyBuilder.RequireClaim("pilot", "true"));

    options.AddPolicy("crewPolicy", policyBuilder =>
    policyBuilder.RequireClaim("crew", "true"));
});

// Add Controllers
builder.Services.AddControllers();

// Add Swagger with JWT
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Please enter token",
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "bearer"
    });
    options.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type=ReferenceType.SecurityScheme,
                    Id="Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// Configure Serilog
builder.Host.UseSerilog((context, config) =>
{
    config.ReadFrom.Configuration(context.Configuration);
});

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API V1"));
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Apply EF Core Migrations and Seed the Database
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;

    var dbContext = services.GetRequiredService<DAirDbContext>();

    // Apply migrations (this will apply any pending migrations)
    dbContext.Database.Migrate();

    // Seed the database
    DbInitializer.Initialize(dbContext); 

    // seed admin user 
    var userManager = services.GetService<UserManager<ApiUser>>();
    if (userManager != null)
        DbInitializer.SeedUsers(userManager);
    else throw new Exception("Unable to get UserManager!");
}

app.Run();
