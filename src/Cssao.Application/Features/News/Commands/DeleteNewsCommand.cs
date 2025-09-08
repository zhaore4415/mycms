using Ardalis.GuardClauses;
using Cssao.Domain;
using Cssao.Domain.IRepositories;
using MediatR;

namespace Cssao.Application.Features.News.Commands
{
    // Application/Features/News/Commands/DeleteNewsCommand.cs
    public class DeleteNewsCommand : IRequest<Unit>
    {
        public int Id { get; set; }
    }

    public class DeleteNewsCommandHandler : IRequestHandler<DeleteNewsCommand, Unit>
    {
        private readonly INewsRepository _repository;
        private readonly IUnitOfWork _unitOfWork;

        public DeleteNewsCommandHandler(INewsRepository repository, IUnitOfWork unitOfWork)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
        }

        public async Task<Unit> Handle(DeleteNewsCommand request, CancellationToken ct)
        {
            var news = await _repository.GetByIdAsync(request.Id, ct);
            if (news == null)
                throw new NotFoundException(request.Id.ToString(), $"新闻 ID {request.Id} 不存在");

            await _repository.DeleteAsync(news.Id);
            await _unitOfWork.SaveChangesAsync(ct);

            return Unit.Value;
        }
    }
}
