using Microsoft.AspNetCore.Builder;
using StudentApi.Hateoas;
using StudentApi.Middleware;
using StudentApi.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var builder = WebApplication.CreateBuilder(args);

// Добавление сервисов в контейнер.
builder.Services.AddControllers()
    .AddXmlSerializerFormatters(); // Добавление поддержки XML

// Настройка DbContext
builder.Services.AddDbContext<StudentContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Регистрация HateoasLinkGenerator как Scoped
builder.Services.AddScoped<ILinkGenerator, HateoasLinkGenerator>();

// Добавление Swagger для документации API
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Применение миграций и создание базы данных при запуске
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<StudentContext>();
    dbContext.Database.Migrate();
}


    app.UseSwagger();
    app.UseSwaggerUI();

// Добавление глобального обработчика ошибок
app.UseMiddleware<ErrorHandlingMiddleware>();

app.UseAuthorization();

app.MapControllers();

app.Run();