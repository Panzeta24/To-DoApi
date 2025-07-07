using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models
{
    public class TodoItem
    {

        [Key] public int Id { get; set; }

        public required string Title { get; set; } //= string.Empty;

        public required string Description { get; set; } //= string.Empty;
        
        public bool IsCompleted { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // TODO ESTO REPRESENTA LO QUE SE VA A GUARDAR EN LA BASE DE DATOS: ID, TITULO, DESCRIPCION, SI ESTA COMPLETA Y FECHA DE CREACION.
    }
}
