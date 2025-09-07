using AutoMapper;

namespace Cssao.Infrastructure.AutoMapper
{
    /// <summary>
    /// AutoMapper 扩展方法，用于全局忽略指定名称的属性
    /// </summary>
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// 全局忽略指定名称的属性（不参与任何映射）
        /// </summary>
        public static void IgnoreProperties(this Profile profile, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                profile.CreateMap<object, object>().ForMember(propertyName, opt => opt.Ignore());
            }
        }
        /// <summary>
        /// 全局忽略指定的审计字段（适用于 AutoMapper 11.0+）
        /// </summary>
        public static void IgnoreAuditFields(this Profile profile, params string[] propertyNames)
        {
            foreach (var propertyName in propertyNames)
            {
                profile.AddGlobalIgnore(propertyName);
            }
        }
    }
}