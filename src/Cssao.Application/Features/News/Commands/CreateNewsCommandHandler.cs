using Cssao.Domain.IRepositories;
using MediatR;
using Cssao.Domain.Entities;

namespace Cssao.Application.Features.News.Commands
{
    public class CreateNewsCommandHandler : IRequestHandler<CreateNewsCommand, int>
    {
        private readonly INewsRepository _newsRepository;

        public CreateNewsCommandHandler(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<int> Handle(CreateNewsCommand request, CancellationToken ct)
        {
            var news = new Cssao.Domain.Entities.News
            {
                Title = request.Title,
                Summary = request.Summary,
                Content = request.Content,
                CoverImage = request.CoverImage,
                IsFeatured = request.IsFeatured,
                PublishDate = request.PublishDate ?? DateTime.UtcNow,
                CategoryId = request.CategoryId
            };

            await _newsRepository.AddAsync(news, ct);
            return news.Id;
        }
    }
}
