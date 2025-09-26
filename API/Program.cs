using Backend.Data;
using Backend.Interfaces;
using Backend.Servicios;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


//Servicios del backend
builder.Services.AddScoped<IConnectionFactory, SqlConnectionFactory>();
builder.Services.AddScoped<IUsuarioRepositorio, UsuarioRepositorio>();
builder.Services.AddScoped<IDonanteRepositorio, DonanteRepositorio > ();
builder.Services.AddScoped<IPacienteRepositorio, PacienteRepositorio>();
builder.Services.AddScoped<ITiposSangreRepositorio, TipoSangreRepositorio>();
builder.Services.AddScoped<IHospitalRepositorio, HospitalRepositorio>();
builder.Services.AddScoped<IResultadosPrueba, ResultadosPruebaRepositorio>();
builder.Services.AddScoped<IPruebasLaboratorio, PruebasLaboratorioRepositorio>();
<<<<<<< Updated upstream
builder.Services.AddScoped<IBancoDeSangreRepositorio, BancoDeSangreRepositorio>();
=======
builder.Services.AddScoped<IEnfermedadRepositorio, EnfermedadRepositorio>();
>>>>>>> Stashed changes

//builder.Services.AddSingleton<IJwtService, Jwt>();

//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
/* .AddJwtBearer(o =>
  {
      o.TokenValidationParameters = new TokenValidationParameters
      {
          ValidateIssuer = true,
          ValidateAudience = true,
          ValidateLifetime = true,
          ValidateIssuerSigningKey = true,
          ValidIssuer = builder.Configuration["Jwt:Issuer"],
          ValidAudience = builder.Configuration["Jwt:Audience"],
          IssuerSigningKey = new SymmetricSecurityKey(
              Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
          ),
          ClockSkew = TimeSpan.Zero
      };
  });
*/



var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseRouting();
app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
