using Cssao.Application.DTOs;
using Cssao.Application.Features.News.Commands;
using Cssao.Application.Features.News.Queries;
using Cssao.Shared.Models.Admin;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Cssao.api.Controllers
{
    [ApiController]  // ✅ 必须添加：启用 API 特性（自动模型验证、FromBody 推断等）
    [Route("api/news")]  // ✅ 必须添加：明确路由前缀
    [ApiExplorerSettings(GroupName = "frontend")] // 👈 前端分组
    public class NewsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NewsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        ///// <summary>
        ///// 获取新闻详情
        ///// </summary>
        //[HttpGet("{id}")]
        //public async Task<ActionResult<NewsDto>> GetNewsDetail(int id)
        //{
        //    if (id <= 0)
        //        return BadRequest("无效的新闻ID");

        //    var query = new GetNewsDetailQuery { Id = id };
        //    var result = await _mediator.Send(query);

        //    if (result == null)
        //        return NotFound($"未找到ID为 {id} 的新闻");

        //    return Ok(result);
        //}

        /// <summary>
        /// 按分类获取新闻分页列表
        /// </summary>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<PagedResultDto<NewsListDto>>> GetByCategory(
            int categoryId,
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            if (categoryId <= 0)
                return BadRequest("无效的分类ID");

            var query = new GetNewsListQuery
            {
                CategoryId = categoryId,
                PageIndex = pageIndex,
                PageSize = pageSize
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }

        /// <summary>
        /// 获取所有新闻分页列表（不分分类）
        /// </summary>
        [HttpGet] // 匹配 GET /api/admin/news
        [ProducesResponseType(typeof(PagedResultDto<NewsListDto>), 200)]
        public async Task<ActionResult<PagedResultDto<NewsListDto>>> GetAllNews(
            [FromQuery] int pageIndex = 1,
            [FromQuery] int pageSize = 10)
        {
            var query = new GetNewsListQuery
            {
                PageIndex = pageIndex,
                PageSize = pageSize
                // CategoryId = null，表示不限分类
            };

            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
