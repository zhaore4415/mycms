using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Domain.Interfaces
{
    public interface ISoftDelete : IUpdatedBy
    {
        bool IsDeleted { get; set; }
        DateTime? DeletedAt { get; set; }
    }
}
