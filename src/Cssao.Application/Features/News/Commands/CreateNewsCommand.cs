using MediatR;

namespace Cssao.Application.Features.News.Commands
{
    public record CreateNewsCommand : IRequest<int>
    {
        public string Title { get; init; } = default!;
        public string? Summary { get; init; }
        public string Content { get; init; } = default!;
        public string? CoverImage { get; init; }
        public bool IsFeatured { get; init; }
        public DateTime? PublishDate { get; init; } = DateTime.Now;
        public int CategoryId { get; init; }
    }
    // UpdateNewsCommand.cs
    public record UpdateNewsCommand : IRequest<Unit>
    {
        public int Id { get; init; } // 或 get; set;

        public string Title { get; init; } = default!;
        public string? Summary { get; init; }
        public string Content { get; init; } = default!;
        public string? CoverImage { get; init; }
        public bool IsFeatured { get; init; }
        public DateTime? PublishDate { get; init; }
        public int CategoryId { get; init; }
    }

}
