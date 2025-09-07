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
    public class News: AuditableEntity
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Required, MaxLength(200)]
        public string Title { get; set; }

        /// <summary>
        /// 总结
        /// </summary>
        [MaxLength(500)]
        public string? Summary { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [Column(TypeName = "text")]
        public string Content { get; set; }

        [MaxLength(300)]
        public string? CoverImage { get; set; }

        /// <summary>
        /// 特色
        /// </summary>
        public bool IsFeatured { get; set; }
        /// <summary>
        /// 发布时间
        /// </summary>
        public DateTime PublishDate { get; set; } = DateTime.UtcNow;

        // 分类关系
        public int CategoryId { get; set; }
        public Category Category { get; set; }

        // 标签
        public ICollection<NewsTag> ArticleTags { get; set; } = new List<NewsTag>();
    }
}
