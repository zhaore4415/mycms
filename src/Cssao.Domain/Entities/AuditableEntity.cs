using Cssao.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Domain.Entities
{
    public abstract class AuditableEntity :
    IEntity,
    IAuditable,
    ISoftDelete,
    ICreatedBy,
    IUpdatedBy
    {
        public int Id { get; set; }

        // 时间戳
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        // 软删除
        public bool IsDeleted { get; set; }
        public DateTime? DeletedAt { get; set; }

        // 操作人
        public string CreatedBy { get; set; } = default!;
        public string? UpdatedBy { get; set; }
    }
}
