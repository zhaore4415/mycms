using Ardalis.GuardClauses;
using AutoMapper;
using Cssao.Application.DTOs;
using Cssao.Domain.Entities;
using Cssao.Domain.IRepositories;
using Cssao.Shared.Models.Admin;
using MediatR;
using X.PagedList;

namespace Cssao.Application.Features.News.Queries
{
    public record GetAllCategoriesQuery : IRequest<List<CategoryDto>>;
    public class GetNewsCategoriesQueryHandler : IRequestHandler<GetAllCategoriesQuery, List<CategoryDto>>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;

        public GetNewsCategoriesQueryHandler(INewsRepository newsRepository, IMapper mapper)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
        }

        public async Task<List<CategoryDto>> Handle(GetAllCategoriesQuery request, CancellationToken ct)
        {
            var categorys = await _newsRepository.GetCategoriesAsync(ct);

            var categoryDtos = _mapper.Map<List<Category>, List<CategoryDto>>(categorys);
            Guard.Against.Null(categoryDtos, nameof(categoryDtos));

            return categoryDtos;
        }
    }
}
