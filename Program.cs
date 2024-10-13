using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.ResponseCompression;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using MurabaDemo;
using MurabaDemo.Database.Log;
using MurabaDemo.Database.Main;
using MurabaDemo.Middlewares;
using MurabaDemo.Models.Configuration;
using Unchase.Swashbuckle.AspNetCore.Extensions.Extensions;

var builder = WebApplication.CreateBuilder(new WebApplicationOptions()
{
    Args = args,
});
var corsSettings = builder.Configuration.GetSection("Cors").Get<CORSConfig>();
var swaggerSettings = builder.Configuration.GetSection("Swagger").Get<SwaggerConfig>();
var jwtSettings = builder.Configuration.GetSection("JWT").Get<JWTConfig>();

builder.Services.AddAutoMapper((IEnumerable<Assembly>)AppDomain.CurrentDomain.GetAssemblies());
builder.Services.AddDbContext<MainDbContext>(db => db.UseNpgsql(builder.Configuration.GetConnectionString("main")));
builder.Services.AddDbContext<LogDbContext>(db => db.UseSqlite(builder.Configuration.GetConnectionString("log")));
builder.Services.Configure<JWTConfig>(builder.Configuration.GetSection("JWT"));
AppContext.SetSwitch(switchName: "Npgsql.EnableLegacyTimestampBehavior", true);



builder.Services.AddControllers()
    .AddJsonOptions(opt =>
    {
        opt.JsonSerializerOptions.ReferenceHandler  = ReferenceHandler.IgnoreCycles;
        opt.JsonSerializerOptions.WriteIndented = true;
    
    });

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
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] { }
        }
    });
    options.AddEnumsWithValuesFixFilters(o =>
    {
        o.ApplySchemaFilter = true;

        o.XEnumNamesAlias = "x-enum-varnames";

        o.XEnumDescriptionsAlias = "x-enum-descriptions";

        o.ApplyParameterFilter = true;

        o.ApplyDocumentFilter = true;

        o.IncludeDescriptions = true;

        o.IncludeXEnumRemarks = true;

        o.DescriptionSource = DescriptionSources.DescriptionAttributesThenXmlComments;


        o.NewLine = "\n";


    });
});

builder.Services.AddAuthentication(t =>
{
    t.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    t.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(
    options =>
    {
        if (jwtSettings == null)
            return;
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey = true,
            ValidateIssuer = true,
            ValidateAudience = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings!.Secret)),
            ValidIssuer = jwtSettings!.Issuer,
            ValidAudience = jwtSettings!.Audience,
        };
    }
);
builder.Services.AddCors(options => options.AddPolicy("local", policy =>
{
    if (corsSettings == null)
        return;

    policy = policy.WithOrigins(corsSettings!.Origins.ToArray());
    if (corsSettings.AllowAnyMethod)
        policy = policy.AllowAnyMethod();

    if (corsSettings.AllowAnyHeader)
        policy = policy.AllowAnyHeader();

    if (corsSettings.AllowCredentials)
        policy = policy.AllowCredentials();
}));
builder.Services.InjectServices();
builder.Services.AddHttpContextAccessor();
builder.Services.AddAntiforgery(opt => { opt.SuppressXFrameOptionsHeader = true; });
builder.Services.AddResponseCompression(opt => { opt.EnableForHttps = true; });
builder.Services.Configure<BrotliCompressionProviderOptions>(opt =>
    opt.Level = System.IO.Compression.CompressionLevel.Fastest);
var app = builder.Build();
app.UseResponseCompression();
if (app.Environment.IsDevelopment() || swaggerSettings!.isEnable)
{
    app.UseWhen(ctx => ctx.Request.Path.StartsWithSegments("/swagger", StringComparison.OrdinalIgnoreCase), swaggerApp =>
    {
        swaggerApp.UseMiddleware<SwaggerAuthMiddleware>();
    });
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint(swaggerSettings!.DocPath, swaggerSettings.Name);
        c.DocumentTitle = swaggerSettings.Name;
        if (swaggerSettings.customeStyle)
        {
            if (swaggerSettings.DarkTheme && !string.IsNullOrWhiteSpace(swaggerSettings.Drak_StylePath))
                c.InjectStylesheet(swaggerSettings.Drak_StylePath);
            if (swaggerSettings.LightTheme && !string.IsNullOrWhiteSpace(swaggerSettings.Light_StylePath))
                c.InjectStylesheet(swaggerSettings.Drak_StylePath);
        }

        if (swaggerSettings.AutoCollapse)
            c.DocExpansion(Swashbuckle.AspNetCore.SwaggerUI.DocExpansion.None);
        if (swaggerSettings.Filter)
            c.EnableFilter();
        if (swaggerSettings.RequestDuration)
            c.DisplayRequestDuration();
    });
}

app.UseRouting();
app.UseCors("local");
app.UseStaticFiles();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();

