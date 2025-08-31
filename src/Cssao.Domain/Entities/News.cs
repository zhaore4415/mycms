using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Domain.Entities
{
    // 文章/产品实体
    public class News
    {
        public int Id { get; set; }

        [Required, MaxLength(200)]
        public string Title { get; set; }

        [MaxLength(500)]
        public string Summary { get; set; }

        [Column(TypeName = "text")]
        public string Content { get; set; }

        [MaxLength(300)]
        public string CoverImage { get; set; }

        public bool IsFeatured { get; set; }
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;

        // 分类关系
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // 标签
        public ICollection<NewsTag> ArticleTags { get; set; } = new List<NewsTag>();
    }
}
