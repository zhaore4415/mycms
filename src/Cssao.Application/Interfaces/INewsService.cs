using Cssao.Application.DTOs;
using Cssao.Domain.Entities;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using X.PagedList;

namespace Cssao.Application.Interfaces
{
    public interface INewsService
    {
        /// <summary>
        /// 发布新闻（含敏感词校验）
        /// </summary>
        Task PublishNewsAsync(CreateNewsRequestDto  dto);

        /// <summary>
        /// 获取新闻详情（带缓存）
        /// </summary>
        Task<ActionResult<NewsDetailDto>> GetNewsDetailAsync(int id);

        /// <summary>
        /// 分页列表
        /// </summary>
        /// <param name="categoryId"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        Task<ActionResult<IPagedList<NewsListDto>>> GetByCategoryAsync(int categoryId, int pageIndex, int pageSize);
        ///// <summary>
        ///// 更新新闻状态
        ///// </summary>
        //Task UpdateNewsStatusAsync(Guid newsId, NewsStatus status);
    }
}
