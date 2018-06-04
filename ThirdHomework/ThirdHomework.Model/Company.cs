using ThirdHomework.MyAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdHomework.Model
{
    /// <summary>
    /// 公司
    /// </summary>
    public class Company : BaseModel
    {
        /// <summary>
        /// 名称
        /// </summary>
        [Remark("公司名称")]
        [Required()]
        public string Name { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [Remark("创建时间")]
        [Required()]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建者
        /// </summary>
        [Remark("创建者")]
        [Required()]
        public int CreatorId { get; set; }

        /// <summary>
        /// 最后修改者Id
        /// </summary>
        [Remark("最后修改者Id")]
        public int LastModifierId { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [Remark("最后修改时间")]
        public DateTime LastModifyTime { get; set; }
    }
}
