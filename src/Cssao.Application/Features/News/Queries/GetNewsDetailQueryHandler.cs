// 文件：GetNewsDetailQueryHandler.cs
using MediatR;
using Cssao.Domain.IRepositories;
using Cssao.Application.DTOs;
using AutoMapper;

namespace Cssao.Application.Features.News.Queries
{
    public class GetNewsDetailQueryHandler :
        IRequestHandler<GetNewsDetailQuery, NewsDetailDto?>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;

        public GetNewsDetailQueryHandler(INewsRepository newsRepository, IMapper mapper)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
        }

        public async Task<NewsDetailDto?> Handle(
            GetNewsDetailQuery request,
            CancellationToken cancellationToken)
        {
            if (request.Id <= 0) return null;

            var news = await _newsRepository.GetByIdWithCommentsAsync(request.Id, cancellationToken);
            if (news == null) return null;

            var newsDto = _mapper.Map<Cssao.Domain.Entities.News, NewsDetailDto>(news);

            return newsDto;
        }
    }
}