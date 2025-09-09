using Cssao.Shared.Models.Admin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Application.DTOs
{
    public class NewsBaseDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Summary { get; set; } = string.Empty;
        public string? CoverImage { get; set; }
        public bool IsFeatured { get; set; }
        public DateTime PublishDate { get; set; }
        public CategoryDto Category { get; set; } = null!;
    }

    // 列表用
    public class NewsListDto : NewsBaseDto
    {
        // 可以额外加字段，如评论数、点赞数等
        public int CommentCount { get; set; }
        public int ViewCount { get; set; }
    }

    //// 详情用
    //public class NewsDetailDto : NewsBaseDto
    //{
    //    public string Content { get; set; } = string.Empty; // 仅详情需要
      
    //    public AuthorDto? Author { get; set; }
    //}

}
