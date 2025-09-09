using Cssao.Application.DTOs;
using Cssao.Domain.IRepositories;
using Cssao.Shared.Models.Admin;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Application.Features.News.Queries
{
    public record GetNewsListQuery : IRequest<PagedResultDto<NewsDto>>
    {
        public int CategoryId { get; init; }
        public int PageIndex { get; init; } = 1;
        public int PageSize { get; init; } = 10;
    }
   
}
