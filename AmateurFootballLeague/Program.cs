using AmateurFootballLeague.IRepositories;
using AmateurFootballLeague.IServices;
using AmateurFootballLeague.Services;
using AmateurFootballLeague.Utils;
using AmateurFootballLeague.Models;
using AmateurFootballLeague.ExternalService;
using AmateurFootballLeague.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Filters;
using System.Reflection;
using System.Text;
using Newtonsoft.Json;
using System.Text.Json.Serialization;
using Newtonsoft.Json.Converters;
using FirebaseAdmin;
using Google.Apis.Auth.OAuth2;

var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
        policy =>
        {
            policy.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin();
        });
});
// Add services to the container.

builder.Services.AddDbContext<AmateurFootballLeagueContext>(options =>
{
    options.UseSqlServer(builder.Configuration.GetConnectionString("DbContext"));
});

builder.Services.AddAutoMapper(typeof(AutoMapperProfile).Assembly);

var pathToKey = Path.Combine(Directory.GetCurrentDirectory(), "Keys", "firebase_admin_sdk.json");
FirebaseApp.Create(new AppOptions
{
    Credential = GoogleCredential.FromFile(pathToKey)
});
Environment.SetEnvironmentVariable("GOOGLE_APPLICATION_CREDENTIALS", pathToKey);

builder.Services.AddScoped(typeof(IRepository<,>), typeof(Repository<,>));
builder.Services.AddSingleton<IJWTProvider, JWTProvider>();

builder.Services.AddTransient<IUploadFileService, UploadFileService>();
builder.Services.AddTransient<ISendEmailService, SendEmailService>();
builder.Services.AddTransient<IAgoraProvider, AgoraProvider>();

builder.Services.AddTransient<IRoleRepository, RoleRepository>();
builder.Services.AddTransient<IRoleService, RoleService>();

builder.Services.AddTransient<IUserRepository, UserRepository>();
builder.Services.AddTransient<IUserService, UserService>();

builder.Services.AddTransient<ITournamentTypeRepository, TournamentTypeRepository>();
builder.Services.AddTransient<ITournamentTypeService, TournamentTypeService>();

builder.Services.AddTransient<IFootballFieldTypeRepository, FootballFieldTypeRepository>();
builder.Services.AddTransient<IFootballFieldTypeService, FootballFieldTypeService>();

builder.Services.AddTransient<ITournamentRepository, TournamentRepository>();
builder.Services.AddTransient<ITournamentService, TournamentService>();

builder.Services.AddTransient<ITeamRepository, TeamRepository>();
builder.Services.AddTransient<ITeamService, TeamService>();

builder.Services.AddTransient<IMatchRepository, MatchRepository>();
builder.Services.AddTransient<IMatchService, MatchService>();

builder.Services.AddTransient<INewsRepository, NewsRepository>();
builder.Services.AddTransient<INewsService, NewsService>();

builder.Services.AddTransient<IImageRepository, ImageRepository>();
builder.Services.AddTransient<IImageService, ImageService>();

builder.Services.AddTransient<IFootballPlayerRepository, FootballPlayerRepository>();
builder.Services.AddTransient<IFootballPlayerService, FootballPlayerService>();

builder.Services.AddTransient<ITeamInTournamentRepository, TeamInTournamentRepository>();
builder.Services.AddTransient<ITeamInTournamentService, TeamInTournamentService>();

builder.Services.AddTransient<IPlayerInTeamRepository, PlayerInTeamRepository>();
builder.Services.AddTransient<IPlayerInTeamService, PlayerInTeamService>();

builder.Services.AddTransient<IplayerInTournament, PlayerInTournamentRepository>();
builder.Services.AddTransient<IPlayerInTournamentService, PlayerInTournamentService>();

builder.Services.AddTransient<ITeamInMatchRepository, TeamInMatchRepository>();
builder.Services.AddTransient<ITeamInMatchService, TeamInMatchService>();

builder.Services.AddTransient<IMatchDetailRepository, MatchDetailRepository>();
builder.Services.AddTransient<IMatchDetailService, MatchDetailService>();

builder.Services.AddTransient<IScorePredictionRepository, ScorePredictionRepository>();
builder.Services.AddTransient<IScorePredictionService, ScorePredictionService>();

builder.Services.AddTransient<ITournamentResultRepository, TournamentResultRepository>();
builder.Services.AddTransient<ITournamentResultService, TournamentResultService>();

builder.Services.AddTransient<IVerifyCodeRepository, VerifyCodeRepository>();
builder.Services.AddTransient<IVerifyCodeService, VerifyCodeService>();

builder.Services.AddTransient<IPromoteRequestRepository, PromoteRequestRepository>();
builder.Services.AddTransient<IPromoteRequestService, PromoteRequestService>();

builder.Services.AddTransient<ICommentRepository, CommentRepository>();
builder.Services.AddTransient<ICommentService, CommentService>();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.Converters.Add(new StringEnumConverter());
    options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
}).AddJsonOptions(options => {
    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.JsonSerializerOptions.PropertyNamingPolicy = System.Text.Json.JsonNamingPolicy.CamelCase;
});
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSwaggerGen(options => {
    options.AddSecurityDefinition("oauth2", new OpenApiSecurityScheme
    {
        Description = "Standard Authorization header using the Bearer scheme (\"bearer {token}\")",
        In = ParameterLocation.Header,
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey
    });

    options.SwaggerDoc("v1", new OpenApiInfo { Title = "AmateurFootballLeague", Version = "v1" });

    options.OperationFilter<SecurityRequirementsOperationFilter>();

    var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));
});
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8
                .GetBytes(builder.Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));
}

app.UseHttpsRedirection();

app.UseCors(MyAllowSpecificOrigins);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
