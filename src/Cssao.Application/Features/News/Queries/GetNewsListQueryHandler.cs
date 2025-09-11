using Cssao.Application.DTOs;
using Cssao.Domain.IRepositories;
using Cssao.Shared.Models.Admin;
using MediatR;
using X.PagedList;

namespace Cssao.Application.Features.News.Queries
{
    public class GetNewsListQueryHandler : IRequestHandler<GetNewsListQuery, PagedResultDto<NewsDto>>
    {
        private readonly INewsRepository _newsRepository;

        public GetNewsListQueryHandler(INewsRepository newsRepository)
        {
            _newsRepository = newsRepository;
        }

        public async Task<PagedResultDto<NewsDto>> Handle(GetNewsListQuery request, CancellationToken ct)
        {
            IPagedList<Cssao.Domain.Entities.News> pagedList;
            if (request.CategoryId <= 0)
            {
                pagedList = await _newsRepository.GetAllAsync(request.PageIndex,
                request.PageSize);
                var dtos = pagedList.Select(news => new NewsDto
                {
                    Id = news.Id,
                    Title = news.Title,
                    CoverImage = news.CoverImage,
                    PublishDate = news.PublishDate,
                    UpdatedAt = news.UpdatedAt,
                    Category = new CategoryDto
                    {
                        Id = news.Category.Id,
                        Name = news.Category.Name
                    }

                }).ToList();
                return new PagedResultDto<NewsDto>
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
                var dtos = pagedList.Select(news => new NewsDto
                {
                    Id = news.Id,
                    Title = news.Title,
                    CoverImage = news.CoverImage,
                    PublishDate = news.PublishDate,
                    UpdatedAt = news.UpdatedAt,
                    Category = new CategoryDto
                    {
                        Id = news.Category.Id,
                        Name = news.Category.Name
                    }
                }).ToList();
                return new PagedResultDto<NewsDto>
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
