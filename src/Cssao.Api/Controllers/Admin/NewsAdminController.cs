using Ardalis.GuardClauses;
using Cssao.Application.DTOs;
using Cssao.Application.Features.News.Commands;
using Cssao.Application.Features.News.Queries;
using Cssao.Shared.Models.Admin;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Cssao.api.Controllers
{
    [ApiController]  // ✅ 必须添加：启用 API 特性（自动模型验证、FromBody 推断等）
    [Route("api/admin/news")]  // ✅ 必须添加：明确路由前缀
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

        /// <summary>
        /// 获取新闻详情
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<NewsDto>> GetNewsDetail(int id)
        {
            if (id <= 0)
                return BadRequest("无效的新闻ID");

            var query = new GetNewsDetailQuery { Id = id };
            var result = await _mediator.Send(query);

            if (result == null)
                return NotFound($"未找到ID为 {id} 的新闻");

            return Ok(result);
        }

        /// <summary>
        /// 更新新闻（全量更新）
        /// </summary>
        [HttpPut("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        public async Task<IActionResult> UpdateNews(int id, [FromBody] UpdateNewsCommand command)
        {
            // 验证 ID 一致性
            if (id <= 0 || command.Id != id)
            {
                return BadRequest("ID 路径参数与请求体不匹配");
            }

            // 验证模型
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                await _mediator.Send(command);
                return NoContent(); // 204：更新成功，无返回内容
            }
            catch (NotFoundException) // 假设你有一个业务异常
            {
                return NotFound($"未找到ID为 {id} 的新闻，无法更新");
            }
            catch (Exception ex)
            {
                // 可记录日志
                return StatusCode(500, $"更新新闻时发生内部错误,{ex}");
            }
        }

        /// <summary>
        /// 删除新闻
        /// </summary>
        [HttpDelete("{id}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteNews(int id)
        {
            if (id <= 0)
            {
                return BadRequest("无效的新闻ID");
            }

            var command = new DeleteNewsCommand { Id = id };

            try
            {
                await _mediator.Send(command);
                return NoContent(); // 204：删除成功
            }
            catch (NotFoundException)
            {
                return NotFound($"未找到ID为 {id} 的新闻，无法删除");
            }
            catch (Exception ex)
            {
                // 可记录日志
                return StatusCode(500, $"删除新闻时发生内部错误,{ex}");
            }
        }

        [HttpGet("Categories")]
        public async Task<ActionResult<List<CategoryDto>>> GetCategories(CancellationToken ct)
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery(), ct);
            return Ok(categories);
        }


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
        [ProducesResponseType(typeof(PagedResultDto<NewsDto>), 200)]
        public async Task<ActionResult<PagedResultDto<NewsDto>>> GetAllNews(
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
