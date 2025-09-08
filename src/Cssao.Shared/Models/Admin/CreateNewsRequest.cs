using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Shared.Models.Admin
{
    public record CreateNewsRequest
    {
        [Required(ErrorMessage = "标题是必填的")]
        [StringLength(200, ErrorMessage = "标题不能超过200个字符")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "内容是必填的")]
        public string Content { get; set; } = string.Empty;

        public string? Category { get; set; } = string.Empty;

        public string? Summary { get; init; }

        public string? CoverImage { get; init; }

        public bool IsFeatured { get; init; } = false;

        public DateTime? PublishDate { get; set; }

        public int CategoryId { get; set; } = 0;
    }
}
