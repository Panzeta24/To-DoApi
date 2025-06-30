using System.ComponentModel.DataAnnotations;

namespace TodoApi.Models.DTOs.Request
{
    public class CreateTodoItemDto
    {
        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        public string Description { get; set; } = string.Empty;
    }
}
