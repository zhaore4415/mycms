using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Shared.Models.Admin
{
    public record UpdateNewsRequest
    {
        public int Id { get; init; }
        public string Title { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public string? Category { get; set; } = string.Empty;
        public string? Summary { get; init; }
        public string? CoverImage { get; init; }
        public bool IsFeatured { get; init; }
        public DateTime? PublishDate { get; init; }
        public int CategoryId { get; init; }
    }
}
