using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClassLibBase;

namespace StyleBase
{
    public class CreatePrjModel: NoitifyBase
    {

        /// <summary>
        /// 工程名称
        /// </summary>
        private string _projectName;

        public string ProjectName
        {
            get { return _projectName; }
            set { _projectName = value; this.DoNitify(); }
        }

        /// <summary>
        /// 工程路径
        /// </summary>
        private string _projectPath;

        public string ProjectPath
        {
            get { return _projectPath; }
            set { _projectPath = value; this.DoNitify(); }
        }

    }
}
