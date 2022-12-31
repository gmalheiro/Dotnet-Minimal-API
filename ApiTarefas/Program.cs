using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args); // [5] builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseInMemoryDatabase("TarefasDB"));

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.MapGet("/HelloWorld", () => "Hello,World CSharp  asp .Net core web minimal API");

app.MapGet("/Frases", async () =>
    await new HttpClient().GetStringAsync("https://ron-swanson-quotes.herokuapp.com/v2/quotes")
    );

app.MapGet("/Tarefas", async (AppDbContext db) => await db.Tarefas.ToListAsync());

app.MapPost("/Tarefas", async (AppDbContext db, Tarefa tarefa) =>
{
    db.Tarefas.Add(tarefa);
    await db.SaveChangesAsync();
    return Results.Created($"/Tarefas/{tarefa.Id}", tarefa);
});

//app.UseAuthorization();

app.Run();

class Tarefa
{
    public int Id { get; set; }
    public string? Nome { get; set; }
    public bool IsConcluida { get; set; }

}

class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Tarefa> Tarefas => Set<Tarefa>();
}