using Azureblob2.Controllers;
using Azureblob2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using System.Reflection;
using Microsoft.Extensions.Azure;


var builder = WebApplication.CreateBuilder(args);


builder.Services.AddDbContext<DBContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        sqlOptions => sqlOptions.EnableRetryOnFailure()
    ));

// Add services to the container.
// Add services to the container.
builder.Services.Configure<BlobController.BlobStorageSettings>(
    builder.Configuration.GetSection("AzureBlobStorage"));
builder.Services.AddScoped<JwtTokenService>();

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = "Bearer";
    options.DefaultChallengeScheme = "Bearer";
})
.AddJwtBearer("Bearer", options =>
{
    options.RequireHttpsMetadata = false;
    options.SaveToken = true;

    options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
    {
        ValidateIssuer = false,
        ValidateAudience = false,
        ValidateLifetime = false,
        ValidateIssuerSigningKey = false
    };
});

builder.Services.AddAuthorization();

// Register service
builder.Services.AddScoped<IBlobStorageService, BlobStorageService>();
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSwaggerGen(options =>
{
    // Define JWT Bearer Authentication
    options.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\nExample: Bearer 12345abcdef"
    });

    // Add global security requirement
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});
builder.Services.AddAzureClients(clientBuilder =>
{
    clientBuilder.AddBlobServiceClient(builder.Configuration["AzureBlobStorage:ConnectionString1:blobServiceUri"]!).WithName("AzureBlobStorage:ConnectionString1");
    clientBuilder.AddQueueServiceClient(builder.Configuration["AzureBlobStorage:ConnectionString1:queueServiceUri"]!).WithName("AzureBlobStorage:ConnectionString1");
    clientBuilder.AddTableServiceClient(builder.Configuration["AzureBlobStorage:ConnectionString1:tableServiceUri"]!).WithName("AzureBlobStorage:ConnectionString1");
});
var app = builder.Build();




// Configure the HTTP request pipeline.
//if (app.Environment.IsDevelopment())
//{
//    app.UseSwagger();
//    app.UseSwaggerUI();
//}
app.UseSwagger();
app.UseSwaggerUI();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
