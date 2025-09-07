using Cssao.Domain.IRepositories;
using MediatR;
using Cssao.Domain.Entities;
using AutoMapper;

namespace Cssao.Application.Features.News.Commands
{
    public class CreateNewsCommandHandler : IRequestHandler<CreateNewsCommand, int>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;

        public CreateNewsCommandHandler(INewsRepository newsRepository, IMapper mapper)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
        }

        public async Task<int> Handle(CreateNewsCommand request, CancellationToken ct)
        {
            
            var news = _mapper.Map<Cssao.Domain.Entities.News>(request);
            await _newsRepository.AddAsync(news, ct);
            return news.Id;
        }
    }
}
