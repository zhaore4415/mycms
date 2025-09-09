using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Shared.Models.Admin
{
    // Models/NewsDto.cs
    public class NewsDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;

        public string? CoverImage { get; set; }

        public string Content { get; set; } = string.Empty;
        public CategoryDto? Category { get; set; } = null!;

        public DateTime PublishDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }
   
    
}
