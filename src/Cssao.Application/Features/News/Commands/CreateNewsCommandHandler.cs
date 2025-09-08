using Cssao.Domain.IRepositories;
using MediatR;
using Cssao.Domain.Entities;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace Cssao.Application.Features.News.Commands
{
    public class CreateNewsCommandHandler : IRequestHandler<CreateNewsCommand, int>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateNewsCommandHandler> _logger; // ← 注入日志

        public CreateNewsCommandHandler(INewsRepository newsRepository, IMapper mapper, ILogger<CreateNewsCommandHandler> logger)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<int> Handle(CreateNewsCommand request, CancellationToken ct)
        {
            try
            {
                var news = _mapper.Map<Cssao.Domain.Entities.News>(request);
                await _newsRepository.AddAsync(news, ct);
                return news.Id;
            }
            catch (Exception ex)
            {
                // 🔥 关键：记录完整错误日志
                _logger.LogError(ex, "创建新闻失败，标题：{Title}", request.Title);
                // ✅ 抛出异常，让上层处理
                throw;
            }
        }
    }
}
