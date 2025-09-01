using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cssao.Domain.Entities
{
    public class BlacklistedToken
    {
        public int Id { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiredAt { get; set; } // 可自动清理过期 Token
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
