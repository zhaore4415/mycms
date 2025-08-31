using Cssao.Application.DTOs;
using Cssao.Application.Interfaces;
using Cssao.Domain.Entities;
using Cssao.Domain.IRepositories;
using Microsoft.AspNetCore.Mvc;
using System.Net.WebSockets;
using System.Runtime.InteropServices;
using X.PagedList;

namespace Cssao.Application.Services
{
    // 新闻服务
    [ApiController]
    [Route("[controller]")]
    public class NewsService : ControllerBase, INewsService
    {
        private readonly INewsRepository _newsRepository;
        //private readonly ISensitiveWordFilter _wordFilter;
        //private readonly ICacheService _cacheService;

        public NewsService(INewsRepository newsRepository
            //ISensitiveWordFilter wordFilter,
            //ICacheService cacheService
            )
        {
            _newsRepository = newsRepository;

        }

        /// <summary>
        /// 发布文章
        /// </summary>
        /// <param name="news"></param>
        /// <returns></returns>
        [HttpPost(Name = "PublishNews")]
        public async Task PublishNewsAsync(CreateNewsRequestDto dto)
        {
            //后面可引入校验插件
            //if (_wordFilter.ContainsSensitiveWords(news.Content))
            //    throw new Exception("内容包含敏感词");

            //后面可引入automap
            var news = new News
            {
                Title = dto.Title,
                Summary = dto.Summary,
                Content = dto.Content,
                CoverImage = dto.CoverImage,
                IsFeatured = dto.IsFeatured,
                PublishDate = dto.PublishDate ?? DateTime.UtcNow,
                CategoryId = dto.CategoryId
            };

            await _newsRepository.AddAsync(news);

            //// 发布领域事件
            //DomainEvents.Raise(new NewsPublishedEvent(news.Id));
        }
        /// <summary>
        /// 获取新闻详情
        /// </summary>
        [HttpGet("{id}", Name = "GetNewsDetail")]
        public async Task<ActionResult<NewsDetailDto>> GetNewsDetailAsync(int id)
        {
            if (id <= 0)
                return BadRequest("无效的新闻ID");

            var news = await _newsRepository.GetByIdWithCommentsAsync(id);
            if (news == null)
                return NotFound($"未找到ID为 {id} 的新闻");
            var dto = new NewsDetailDto
            {
                Id = news.Id,
                Title = news.Title,
                Summary = news.Summary,
                Content = news.Content,
                CoverImage = news.CoverImage,
                IsFeatured = news.IsFeatured,
                PublishDate = news.PublishDate,
                Category = new CategoryDto
                {
                    Id = news.Category.Id,
                    Name = news.Category.Name
                    // ❌ 不包含 news.Category.News
                }
            };

            return Ok(dto);
        }

        // <summary>
        /// 根据分类获取新闻分页列表
        /// </summary>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IPagedList<NewsListDto>>> GetByCategoryAsync(
            int categoryId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            if (categoryId <= 0)
                return BadRequest("无效的分类ID");

            // 调用仓库层的分页查询
            var pagedList = await _newsRepository.GetByCategoryAsync(categoryId, pageIndex, pageSize);

            if (!pagedList.Any())
                return Ok(new PagedResultDto<NewsListDto>()); // 返回空分页，而不是 404

            // ✅ 转换为 DTO 列表
            var dtos = pagedList.Select(news => new NewsListDto
            {
                Id = news.Id,
                Title = news.Title,
                Summary = news.Summary,
                CoverImage = news.CoverImage,
                IsFeatured = news.IsFeatured,
                PublishDate = news.PublishDate,
                Category = new CategoryDto
                {
                    Id = news.Category.Id,
                    Name = news.Category.Name
                    // ❌ 不包含 news.Category.News（打破循环）
                }
            }).ToList();

            // 构造分页结果
            var result = new PagedResultDto<NewsListDto>
            {
                PageIndex = pageIndex,
                PageSize = pageSize,
                TotalCount = pagedList.TotalItemCount,
                Items = dtos
            };

            return Ok(result);
        }
    }


    //// 评论审核服务
    //public class CommentAuditService : ICommentAuditService
    //{
    //    private readonly ICommentRepository _commentRepository;
    //    private readonly IAuditLogService _auditLogService;

    //    public async Task BatchAuditCommentsAsync(IEnumerable<Guid> commentIds, AuditStatus status)
    //    {
    //        var comments = await _commentRepository.GetPendingByIdsAsync(commentIds);
    //        foreach (var comment in comments)
    //        {
    //            comment.AuditStatus = status;
    //            _auditLogService.LogAudit(comment.Id, status);
    //        }
    //        await _commentRepository.UpdateRangeAsync(comments);
    //    }
    //}

}