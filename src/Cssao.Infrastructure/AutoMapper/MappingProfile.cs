using AutoMapper;
using Cssao.Application.DTOs;
using Cssao.Application.Features.News.Commands;
using Cssao.Domain.Entities;
using Cssao.Infrastructure.AutoMapper;
using Cssao.Shared.Models.Admin; // 🔹 显式引入扩展方法命名空间！

namespace Cssao.Infrastructure.AutoMapper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // ✅ 现在一定能识别！
            //IgnoreAuditProperties();
            this.IgnoreAuditFields(
               "CreatedAt", "UpdatedAt",
               "IsDeleted", "DeletedAt",
               "CreatedBy", "UpdatedBy"
           );
            // 🔹 命令 → 实体
            CreateMap<CreateNewsCommand, News>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()); // 如果需要，单独忽略 Id

            CreateMap<Category, CategoryDto>(); // 如果需要，单独忽略 Id
            CreateMap<News, NewsDto>(); // 如果需要，单独忽略 Id

            CreateMap<UpdateNewsCommand, News>()
                .ForMember(dest => dest.Id, opt => opt.Ignore()) // Id 通常也不应由前端修改
                .ForMember(dest => dest.CreatedAt, opt => opt.Ignore()) // 防止篡改
                .ForMember(dest => dest.PublishDate, opt => opt.Ignore()) // 由系统自动设置
                .ForMember(dest => dest.CreatedBy, opt => opt.Ignore());
        }
    }
}