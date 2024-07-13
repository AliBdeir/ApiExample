var builder = WebApplication.CreateBuilder(args);


string configConnectionString = builder.Configuration.GetConnectionString("AppConfig") ?? throw new Exception("App Configuration Connection String Not Set");
builder.Configuration.AddAzureAppConfiguration(configConnectionString);
Console.WriteLine(builder.Configuration["AppName"]);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
