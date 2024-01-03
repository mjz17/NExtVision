using ClassLibBase;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StyleBase
{
    public class HeaderFrmModel: NoitifyBase
    {
        //表头属性
        private Visibility controlVisibility;
        public Visibility ControlVisibility
        {
            get { return controlVisibility; }
            set { controlVisibility = value; this.DoNitify(); }
        }


        //表头显示名称
        private string name;
        public string HeadName
        {
            get { return name; }
            set { name = value; this.DoNitify(); }
        }

    }
}
