// 文件：GetNewsDetailQueryHandler.cs
using MediatR;
using Cssao.Domain.IRepositories;
using Cssao.Application.DTOs;
using AutoMapper;
using Cssao.Shared.Models.Admin;

namespace Cssao.Application.Features.News.Queries
{
    public class GetNewsDetailQueryHandler :
        IRequestHandler<GetNewsDetailQuery, NewsDto?>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;

        public GetNewsDetailQueryHandler(INewsRepository newsRepository, IMapper mapper)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
        }

        public async Task<NewsDto?> Handle(
            GetNewsDetailQuery request,
            CancellationToken cancellationToken)
        {
            if (request.Id <= 0) return null;

            var news = await _newsRepository.GetByIdWithCommentsAsync(request.Id, cancellationToken);
            if (news == null) return null;

            var newsDto = _mapper.Map<Cssao.Domain.Entities.News, NewsDto>(news);

            return newsDto;
        }
    }
}