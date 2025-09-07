using Cssao.Application.DTOs;
using MediatR;

namespace Cssao.Application.Features.News.Queries
{
    public record GetNewsDetailQuery : IRequest<NewsDetailDto?>
    {
        public int Id { get; init; }
    }
}
