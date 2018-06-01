using RMFirstHomework.MyAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RMFirstHomework.Model
{
    public class User : BaseModel
    {
        [Remark("姓名")]
        public string Name { get; set; }

        [Remark("角色")]
        [Required()]
        public string Account { get; set; }

        [Remark("密码")]
        [Required()]
        [Length(6,16)]
        public string Password { get; set; }

        [Remark("邮箱")]
        [Email("^[a-zA-Z0-9_-]+@[a-zA-Z0-9_-]+(\\.[a-zA-Z0-9_-]+)+$")]
        public string Email { get; set; }

        [Remark("电话")]
        [Mobile("^1[34578]\\d{9}$")]
        public string Mobile { get; set; }

        [Remark("公司ID")]
        public int CompanyId { get; set; }

        [Remark("公司名称")]
        public string CompanyName { get; set; }

        [Remark("用户状态")]
        [SetName("State")]
        [Required()]
        public int Status { get; set; } //Status

        [Remark("用户类型")]
        [Required()]
        public int UserType { get; set; }

        [Remark("最后一次登录时间")]
        public DateTime? LastLoginTime { get; set; }

        [Remark("创建时间")]
        [Required()]
        public DateTime CreateTime { get; set; }

        [Remark("创建者ID")]
        [Required()]
        public int CreatorId { get; set; }

        [Remark("最后修改者ID")]
        public int LastModifierId { get; set; }

        [Remark("最后修改时间")]
        public DateTime? LastModifyTime { get; set; }

    }
}
