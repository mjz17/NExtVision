using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    /// <summary>
    /// 流程信息
    /// </summary>
    [Serializable]
    public class ProjectInfo
    {
        public int m_ProjectID { get; set; }                                //项目唯一ID

        public string m_ProjectName { get; set; }                           //项目名称，不能重复

        public string m_DispHwindowName { get; set; } = "Main_Hwindow0";    //显示窗体名称

        public Execution m_Execution { get; set; } = Execution.主动执行;    //执行方式

    }

    /// <summary>
    /// 执行方式
    /// </summary>
    [Serializable]
    public enum Execution
    {
        主动执行,
        调用时执行,
        停止时执行,
    }

}
