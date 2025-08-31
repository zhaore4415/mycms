using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Domain.Entities
{
    // 文章标签关联
    public class NewsTag
    {
        [Key]  // 使用数据注解方式
        public int Id { get; set; }
        public int NewsId { get; set; }
        public News News { get; set; }

        public int TagId { get; set; }
        public Tag Tag { get; set; }
    }
}
