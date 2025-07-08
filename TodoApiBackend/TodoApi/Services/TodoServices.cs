using Microsoft.EntityFrameworkCore;
using TodoApi.Context;
using TodoApi.Models;

namespace TodoApi.Services
{
    public class TodoServices
    {
        private readonly TodoDbContext _context; // Inyecto el DbContext SOLO en el servicio

        public TodoServices(TodoDbContext context)
        {
            _context = context;
        }

        //Obtener todas las tareas
        public async Task<List<TodoItem>> ObtenerTareas()
        {
            return await _context.Tareas.ToListAsync();
        }

        // Obtener tareas por ID
        public async Task<TodoItem?> ObtenerTareasPorId(int id)
        {
            return await _context.Tareas.FindAsync(id);
        }

        // Crear una nueva tarea
        public async Task CrearTarea(TodoItem tarea)
        {
            tarea.CreatedAt = DateTime.Now;     // añade fecha de creacion
            _context.Tareas.Add(tarea);
            await _context.SaveChangesAsync();
        }

        // Actualizar Tarea existente
        public async Task<bool> ActualizarTarea(int id, TodoItem tarea)
        {
            if (id != tarea.Id)
            {
                return false;
            }

            var tareaDb = await _context.Tareas.FindAsync(id);

            if(tareaDb == null)
            {
                return false;
            }

            // actualiza las propiedades de la entidad rastreada(tareaDb)
            // con los valores del objeto que vino de la peticion

            tareaDb.Title = tarea.Title;
            tareaDb.Description = tarea.Description;
            tareaDb.IsCompleted = tarea.IsCompleted;

            //  NO se agrega CreatedAt porque eso no se modifica
            try
            {
                await _context.SaveChangesAsync();
                return true;    // exito
            }
            catch (DbUpdateConcurrencyException)
            {
                // este catch sigue sinedo util si dos personas intentan editar el mismo registro al mismo tiempo.
                throw;
            }
            /*_context.Entry(tarea).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
                return true;
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _context.Tareas.AnyAsync(t => t.Id == id))
                    return false;
                throw; // vuelve a mandar la excepcion si hay otro problema
            }*/


        } 

        // Eliminar tarea por ID
        public async Task<bool> EliminarTarea(int id)
        {
            var tarea = await _context.Tareas.FindAsync(id); // Variable "tarea" va a contener la formula de busqueda por id

            if (tarea == null)
                return false;
            _context.Tareas.Remove(tarea);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
