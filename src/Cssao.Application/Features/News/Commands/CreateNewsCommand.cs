using MediatR;

namespace Cssao.Application.Features.News.Commands
{
    public record CreateNewsCommand : IRequest<int>
    {
        public string Title { get; init; } = default!;
        public string Summary { get; init; } = default!;
        public string Content { get; init; } = default!;
        public string? CoverImage { get; init; }
        public bool IsFeatured { get; init; }
        public DateTime? PublishDate { get; init; }
        public int CategoryId { get; init; }
    }
}
