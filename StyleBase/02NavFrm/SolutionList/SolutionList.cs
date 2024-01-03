using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StyleBase
{
    public class SolutionList
    {

		/// <summary>
		/// 解决方案名称
		/// </summary>
		private string _SolutionName;

		public string m_SolutionName
        {
			get { return _SolutionName; }
			set { _SolutionName = value; }
		}

        /// <summary>
        /// 解决方案注释
        /// </summary>
        private string _SolutionTip;

        public string m_SolutionTip
        {
            get { return _SolutionTip; }
            set { _SolutionTip = value; }
        }

        /// <summary>
        /// 解决方案路径
        /// </summary>
        private string _SolutionPath;

        public string m_SolutionPath
        {
            get { return _SolutionPath; }
            set { _SolutionPath = value; }
        }



    }
}
