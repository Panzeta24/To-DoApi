using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Linq.Expressions;
using TodoApi.Context;
using TodoApi.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TodoApi.Controllers
{
    [Route("api/[controller]")] // Define la ruta base del controlador: api/Todo
    [ApiController] // Indica que es una clase controlador de API y no de vistas.
    public class TodoController : ControllerBase
    {
        private readonly TodoDbContext _context;
        // esta variable privada representa el contexto de la base de datos 

        public TodoController(TodoDbContext context)  // Contructor: se inyecta el contexto al crear el controlador
        {
            _context = context;
        }


        // GET: api/<TodoController>
        // GET va a devolver todas las tareas guardadas en la base de datos
        [HttpGet]
        public async Task<ActionResult<IEnumerable<TodoItem>>> GetTask()
        {
            try // MANEJO DE ERRORES
            {
                // Consulta asincrona para obtener todas las tareas
                var tareas = await _context.Tareas.ToListAsync();

                // Si no hay tareas, devuelve error 404
                if (tareas == null || !tareas.Any())
                {
                    return NotFound("No se encontraron tareas.");
                }

                // retorna 200 Ok con la lista de tareas
                return Ok(tareas);
            }
            catch (Exception ex)
            {
                //Log del error (en produccion, usar ULogger)
                Console.WriteLine($"Error al obtener tareas: {ex.Message}");

                // Retorna 500 Internal Server Error con un mensaje generico
                return StatusCode(500, "Ocurrio un error al procesar la solicitud.");
            }
        }


        // GET api/<TodoController>/5
        // OBTENER TAREA POR ID
        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItem>> GetTaskById(int id)
        {
            try
            {
                // Buscar la tarea por id
                var tarea = await _context.Tareas.FindAsync(id);

                // si no existe la tarea con ese id, muestra error 404 Not Found
                if (tarea == null)
                {
                    return NotFound($"No se encontro la tarea con ID {id}.");
                }
                // muestra la tarea por id
                return Ok(tarea);
            }
            catch (Exception ex) 
            {
                Console.WriteLine($"Error al obtener tarea con ID {id}: {ex.Message}");
                return StatusCode(500, "Ocurrio un error al buscar la tarea.");
            }
        }
      


        // POST api/<TodoController>
        // CREAR TAREA CON POST
        [HttpPost]
        public async Task<ActionResult<TodoItem>> CreateTask(TodoItem tarea) 
        {
            try
            {
                // Validacion manual (opcional, probar sino con FluentValidation/DataAnnotations)
                if (string.IsNullOrWhiteSpace(tarea.Title))
                {
                    return BadRequest("El titulo de la tarea es obligatorio.");
                }
                // Asignar la fecha de creacion automaticamente
                tarea.CreatedAt = DateTime.Now;

                // Agrega la tarea al contexto de EF Core
                _context.Tareas.Add(tarea);

                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                // Retorna 201 Created con la URL de la nueva tarea
                return CreatedAtAction(
                    nameof(GetTaskById),
                    new { id = tarea.Id },
                    tarea);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al crear tarea: {ex.Message}");
                return StatusCode(500, "Ocurrió un error al crear la tarea.");
            }
        }


        // PUT api/<TodoController>/5
        // ACTUALIZAR TAREA EXISTENTE POR ID
        [HttpPut("{id}")]
        public async Task<IActionResult> ActualizarTarea(int id, TodoItem tarea)
        {
            try
            {
                // Verifica que el ID de la URL coincida con el de la tarea
                if (id != tarea.Id)
                {
                    return BadRequest("El ID de la tarea no coincide con la URL.");
                }

                // Marca la tarea como modificada para que EF Core la actualice
                _context.Entry(tarea).State = EntityState.Modified;

                // Intenta guardar los cambios
                await _context.SaveChangesAsync();

                // Retorna 204 No Content (actualizacion exitosa sin respuesta)
                return NoContent();
            }
            catch (DbUpdateConcurrencyException)
            {
                // Verifica si la tarea aun existe
                if (!_context.Tareas.Any(e => e.Id == id))
                {
                    return NotFound($"No se encontró la tarea con ID {id}.");
                }
                else
                {
                    // Relanza la excepcion si es otro error de concurrencia
                    throw;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al actualizar tarea con ID {id}: {ex.Message}");
                return StatusCode(500, "No se pudo actualizar la tarea.");
            }
        } 
        
        // DELETE api/<TodoController>/5
        // Elimina una tarea por su ID
        [HttpDelete("{id}")]
        public async Task<IActionResult> EliminarTarea(int id)
        {
            try
            {
                // Busca la tarea por ID
                var tarea = await _context.Tareas.FindAsync(id);

                // Si no existe, retorna 404
                if (tarea == null)
                {
                    return NotFound($"No se encontró la tarea con ID {id}.");
                }

                // Elimina la tarea del contexto
                _context.Tareas.Remove(tarea);

                // Guarda los cambios en la base de datos
                await _context.SaveChangesAsync();

                //Retorna 204 No Content (eliminacion exitosa)
                return NoContent();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al eliminar tarea con ID {id}: {ex.Message}");
                return StatusCode(500, "No se pudo eliminar la tarea.");
            }
        }
    }
}
