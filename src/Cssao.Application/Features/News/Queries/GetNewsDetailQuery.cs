using Cssao.Application.DTOs;
using Cssao.Shared.Models.Admin;
using MediatR;

namespace Cssao.Application.Features.News.Queries
{
    public record GetNewsDetailQuery : IRequest<NewsDto?>
    {
        public int Id { get; init; }
    }
}
