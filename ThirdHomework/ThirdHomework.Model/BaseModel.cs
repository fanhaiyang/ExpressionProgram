using ThirdHomework.MyAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ThirdHomework.Model
{
    public class BaseModel
    {
        [Remark("主键ID")]
        public int Id { get; set; }
    }
}
