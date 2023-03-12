using BookHouse.Clients.Magfa.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddMagfaClient(opt =>
{
    opt.SenderNumber = new[] { builder.Configuration["MagfaSMS:SenderNumber"] };
    opt.Domain = builder.Configuration["MagfaSMS:Domain"];
    opt.Username = builder.Configuration["MagfaSMS:Username"];
    opt.Password = builder.Configuration["MagfaSMS:Password"];
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
