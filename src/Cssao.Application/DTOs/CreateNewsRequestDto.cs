using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Application.DTOs
{
    public class CreateNewsRequestDto
    {
        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Summary { get; set; }

        [Required]
        [DataType(DataType.Text)]
        public string Content { get; set; }

        [MaxLength(300)]
        public string CoverImage { get; set; }

        public bool IsFeatured { get; set; } = false;

        public DateTime? PublishDate { get; set; } // 可选，不传则用默认值

        [Required]
        public int CategoryId { get; set; }

        // 注意：这里不需要 Id，也不需要导航属性（或只保留 ID）
    }
}
