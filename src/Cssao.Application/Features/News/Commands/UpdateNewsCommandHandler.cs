using Ardalis.GuardClauses;
using AutoMapper;
using Cssao.Domain.IRepositories;
using MediatR;

namespace Cssao.Application.Features.News.Commands
{
    public class UpdateNewsCommandHandler : IRequestHandler<UpdateNewsCommand, Unit>
    {
        private readonly INewsRepository _newsRepository;
        private readonly IMapper _mapper;

        public UpdateNewsCommandHandler(
            INewsRepository newsRepository, IMapper mapper)
        {
            _newsRepository = newsRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(UpdateNewsCommand request, CancellationToken ct)
        {
            var news = await _newsRepository.GetByIdAsync(request.Id, ct);
            if (news == null) throw new NotFoundException(nameof(News), request.Id.ToString());

            // 更新主表字段
            _mapper.Map(request, news);

            // 更新分类（假设只改 CategoryId）
            news.CategoryId = request.CategoryId;

            //// 更新标签（示例）先不做
            //var newTags = await _tagRepository.GetTagsByIdsAsync(request.TagIds, ct);
            //news.Tags.Clear();
            //foreach (var tag in newTags)
            //{
            //    news.Tags.Add(tag);
            //}

            await _newsRepository.UpdateAsync(news, ct);
            return Unit.Value; // ✅ 必须返回
            // 5️⃣ 可选：发布领域事件
            // news.AddDomainEvent(new NewsUpdatedEvent(news.Id));
        }

    }
}
