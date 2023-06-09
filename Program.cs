using AutoMapper;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SixMinApi.Data;
using SixMinApi.Dtos;
using SixMinApi.Models;
using SixMinAPI.Data;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlConnBuilder = new SqlConnectionStringBuilder();

sqlConnBuilder.ConnectionString = 
builder.Configuration.GetConnectionString("SQLDbConnection");
sqlConnBuilder.UserID = builder.Configuration["UserId"];
sqlConnBuilder.Password = builder.Configuration["Password"];

builder.Services.AddDbContext<AppDbContext>(opt => opt.UseSqlServer(sqlConnBuilder.ConnectionString));
builder.Services.AddScoped<ICommandRepo, CommandRepo>();
builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/api/v1/commands", async (ICommandRepo repo, IMapper mapper) => {
    var commands = await repo.GetAllCommands();
    return Results.Ok(mapper.Map<IEnumerable<CommandReadDto>>(commands));
});

app.MapGet("/api/v1/commands/{id}", async (ICommandRepo repo, IMapper mapper, int id) => {
    var command = await repo.GetCommandById(id);
    if(command != null){
        return Results.Ok(mapper.Map<CommandReadDto>(command));
    }
    return Results.NotFound();
});

app.MapPost("/api/v1/commands", async (ICommandRepo repo, IMapper mapper, CommandCreateDto commandCreateDto) => {
    var commandModel = mapper.Map<Command>(commandCreateDto);

    await repo.CreateCommand(commandModel);
    await repo.SaveChanges();

    var cmdReadDto = mapper.Map<CommandReadDto>(commandModel);

    return Results.Created($"/api/v1/commands/{cmdReadDto.Id}", cmdReadDto);
});

app.MapPut("/api/v1/commands/{id}", async (ICommandRepo repo, IMapper mapper, int id, CommandUpdateDto cmdUpdateDto) => {
    var command = await repo.GetCommandById(id);
    
    if(command == null){
        return Results.NotFound();
    }

    mapper.Map(cmdUpdateDto, command);
    await repo.SaveChanges();

    return Results.NoContent();
});

app.MapDelete("/api/v1/commands/{id}", async (ICommandRepo repo, IMapper mapper, int id) => {
    var command = await repo.GetCommandById(id);
    
    if(command == null){
        return Results.NotFound();
    }

    repo.DeleteCommand(command);
    await repo.SaveChanges();
    return Results.NoContent();
});

app.Run();




