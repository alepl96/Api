using Microsoft.EntityFrameworkCore;
using ApiTest;
var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<lc_db_persona>(opt => opt.UseInMemoryDatabase("lista_personas"));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/personas", async (lc_db_persona db) =>
    await db.personas.ToListAsync());

app.MapGet("/personas/{id}", async (int id, lc_db_persona db) =>
    await db.personas.FindAsync(id)
        is lc_persona ls_persona
            ? Results.Ok(ls_persona)
            : Results.NotFound());

app.MapPost("/personas", async (lc_persona arg_persona, lc_db_persona db) =>
{

    string ls_fecha_null = "Jan 1, 1900";
    DateTime ldt_fecha_null = DateTime.Parse(ls_fecha_null);
    if (arg_persona.nombre_apellido == null || arg_persona.nro_documento == null || arg_persona.correo == null || arg_persona.telefono == null || arg_persona.fecha_nacimiento < ldt_fecha_null)
    {
        return Results.NotFound("Faltan Argumentos");
    }
    else
    {
        if (funciones.f_validar_correo(arg_persona.correo) == false)
        {
            return Results.NotFound("correo invalido");
        };

        int li_cant = 0;
        if (li_cant > 0)
        {
            return Results.NotFound("Nro de Documento ya existe");
        }

        db.personas.Add(arg_persona);
        await db.SaveChangesAsync();
        return Results.Created($"/personas/{arg_persona.id}", arg_persona);
    }

});

app.MapPut("/personas/{id}", async (int id, lc_persona arg_persona, lc_db_persona db) =>
{
    var lv_persona = await db.personas.FindAsync(id);

    if (lv_persona is null) return Results.NotFound();
    string ls_fecha_null = "Jan 1, 1900";
    DateTime ldt_fecha_null = DateTime.Parse(ls_fecha_null);
    if (arg_persona.nombre_apellido == null || arg_persona.nro_documento == null || arg_persona.correo == null || arg_persona.telefono == null || arg_persona.fecha_nacimiento < ldt_fecha_null)
    {
        return Results.NotFound("Faltan Argumentos");
    }
    else
    {
        if (funciones.f_validar_correo(arg_persona.correo) == false)
        {
            return Results.NotFound("Correo Invalido");
        };



        lv_persona.nombre_apellido = arg_persona.nombre_apellido;
        lv_persona.nro_documento = arg_persona.nro_documento;
        lv_persona.correo = arg_persona.correo;
        lv_persona.telefono = arg_persona.telefono;
        lv_persona.fecha_nacimiento = arg_persona.fecha_nacimiento;

        await db.SaveChangesAsync();
        return Results.NoContent();
    }
});

app.MapDelete("/personas/{id}", async (int id, lc_db_persona db) =>
{
    if (await db.personas.FindAsync(id) is lc_persona arg_persona)
    {
        db.personas.Remove(arg_persona);
        await db.SaveChangesAsync();
        return Results.Ok(arg_persona);
    }

    return Results.NotFound();
});

app.Run();

class lc_persona
{
    public int id { get; set; }
    public string? nombre_apellido { get; set; }
    public string? nro_documento { get; set; }
    public string? correo { get; set; }
    public string? telefono { get; set; }
    public DateTime fecha_nacimiento { get; set; }

}

class lc_db_persona : DbContext
{
    public lc_db_persona(DbContextOptions<lc_db_persona> options)
        : base(options) { }

    public DbSet<lc_persona> personas => Set<lc_persona>();
}

