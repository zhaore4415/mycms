// 文件：GetNewsDetailQueryHandler.cs
using MediatR;
using Cssao.Domain.IRepositories;
using Cssao.Application.DTOs;

namespace Cssao.Application.Features.News.Queries
{
    public class GetNewsDetailQueryHandler :
        IRequestHandler<GetNewsDetailQuery, NewsDetailDto?>
    {
        private readonly INewsRepository _newsRepository;

        public GetNewsDetailQueryHandler(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<NewsDetailDto?> Handle(
            GetNewsDetailQuery request,
            CancellationToken cancellationToken)
        {
            if (request.Id <= 0) return null;

            var news = await _newsRepository.GetByIdWithCommentsAsync(request.Id, cancellationToken);
            if (news == null) return null;

            return new NewsDetailDto
            {
                Id = news.Id,
                Title = news.Title,
                Summary = news.Summary,
                Content = news.Content,
                CoverImage = news.CoverImage,
                IsFeatured = news.IsFeatured,
                PublishDate = news.PublishDate,
                Category = new CategoryDto
                {
                    Id = news.Category.Id,
                    Name = news.Category.Name
                }
            };
        }
    }
}