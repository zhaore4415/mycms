using Cssao.Application.DTOs;
using Cssao.Domain.IRepositories;
using MediatR;
using X.PagedList;

namespace Cssao.Application.Features.News.Queries
{
    public class GetNewsListQueryHandler : IRequestHandler<GetNewsListQuery, PagedResultDto<NewsListDto>>
    {
        private readonly INewsRepository _newsRepository;

        public GetNewsListQueryHandler(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<PagedResultDto<NewsListDto>> Handle(GetNewsListQuery request, CancellationToken ct)
        {
            IPagedList<Cssao.Domain.Entities.News> pagedList;
            if (request.CategoryId <= 0)
            {
                pagedList = await _newsRepository.GetAllAsync(request.PageIndex,
                request.PageSize);
              var  dtos = pagedList.Select(news => new NewsListDto
                {
                    Id = news.Id,
                    Title = news.Title,
                    Summary = news.Summary,
                    CoverImage = news.CoverImage,
                    IsFeatured = news.IsFeatured,
                    PublishDate = news.PublishDate

                }).ToList();
                return new PagedResultDto<NewsListDto>
                {
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    TotalCount = pagedList.TotalItemCount,
                    Items = dtos
                };
            }
            else
            {
                pagedList = await _newsRepository.GetByCategoryAsync(
                   request.CategoryId,
                   request.PageIndex,
                   request.PageSize,
                   ct);
                var dtos = pagedList.Select(news => new NewsListDto
                {
                    Id = news.Id,
                    Title = news.Title,
                    Summary = news.Summary,
                    CoverImage = news.CoverImage,
                    IsFeatured = news.IsFeatured,
                    PublishDate = news.PublishDate,
                    Category = new CategoryDto
                    {
                        Id = news.Category.Id,
                        Name = news.Category.Name
                    }
                }).ToList();
                return new PagedResultDto<NewsListDto>
                {
                    PageIndex = request.PageIndex,
                    PageSize = request.PageSize,
                    TotalCount = pagedList.TotalItemCount,
                    Items = dtos
                };
            }
        }
    }
}
