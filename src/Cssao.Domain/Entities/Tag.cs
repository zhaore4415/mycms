using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Domain.Entities
{
    // 标签
    public class Tag
    {
        public int Id { get; set; }

        [Required, MaxLength(50)]
        public string Name { get; set; }

        public ICollection<NewsTag> ArticleTags { get; set; } = new List<NewsTag>();
    }
}
