using Cssao.Application.DTOs;
using Cssao.Application.Features.News.Commands;
using Cssao.Application.Features.News.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cssao.api.Controllers
{
    [ApiController]  // ✅ 必须添加：启用 API 特性（自动模型验证、FromBody 推断等）
    [Route("api/news")]  // ✅ 必须添加：明确路由前缀
    [ApiExplorerSettings(GroupName = "backend")]
    [Authorize]// 需要token认证才能访问，也可以单独在方法上添加
    public class NewsAdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NewsAdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// 发布新闻
        /// </summary>
        [HttpPost("publish")]  // ✅ 明确子路由
        public async Task<ActionResult<int>> PublishNews([FromBody] CreateNewsCommand command)
        {
            var newsId = await _mediator.Send(command);
            return CreatedAtAction("GetNewsDetail", new { id = newsId }, newsId);
        }

       
    }
}
