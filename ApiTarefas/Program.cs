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

app.MapGet("/ListarTarefas", async (AppDbContext db) => await db.Tarefas.ToListAsync());

app.MapGet("/ListarTarefaPorId", async (int id, AppDbContext db) => 
                                 await db.Tarefas.FindAsync(id) is Tarefa tarefa ? Results.Ok(tarefa) : Results.NotFound("A tarefa desejada não foi encontrada..."));

app.MapGet("/ListarTarefas/Concluida", async(AppDbContext db) => 
                                       await db.Tarefas.Where (t => t.IsConcluida).ToListAsync());

app.MapPost("/CriarTarefa", async (AppDbContext db, Tarefa tarefa) =>
{
    db.Tarefas.Add(tarefa);
    await db.SaveChangesAsync();
    return Results.Created($"/Tarefas/{tarefa.Id}", tarefa);
});

app.MapPut("AlterarTarefa/{id}", async (int id, Tarefa inputTarefa, AppDbContext db) =>
{
    var tarefa = await db.Tarefas.FindAsync(id);

    if (tarefa is null) return Results.NotFound("A tarefa desejada não foi encontrada");
    
    tarefa.Nome = inputTarefa.Nome;
    tarefa.IsConcluida = inputTarefa.IsConcluida;

    await db.SaveChangesAsync();
    return Results.NoContent();
});

app.MapDelete("ExcluirTarefa/{id}", async (int id, AppDbContext db) =>
{
    if (await db.Tarefas.FindAsync(id) is Tarefa tarefa)
    {
        db.Tarefas.Remove(tarefa);
        await db.SaveChangesAsync();
        return Results.Ok(tarefa);
    }
    return Results.NotFound("A tarefa desejada não foi encontrada...");
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