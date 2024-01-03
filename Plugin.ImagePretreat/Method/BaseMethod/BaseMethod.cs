using HalconDotNet;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using VisionCore;

namespace Plugin.ImagePretreat
{
    [Serializable]
    public class BaseMethod
    {

        /// <summary>
        /// 是否启用
        /// </summary>
        private bool _EnableOrnot = true;
        public bool m_EnableOrnot
        {
            get { return _EnableOrnot; }
            set { _EnableOrnot = value; }
        }

        /// <summary>
        /// 输入图像
        /// </summary>
        [NonSerialized]
        private HImageExt _ImageExt;
        public HImageExt m_ImageExt
        {
            get { return _ImageExt; }
            set { _ImageExt = value; }
        }

        /// <summary>
        /// 输出图像
        /// </summary>
        [NonSerialized]
        private HObject _OutObj;
        public HObject m_OutObj
        {
            get { return _OutObj; }
            set { _OutObj = value; }
        }

        [NonSerialized]
        public System.Windows.Controls.UserControl m_Control;

        /// <summary>
        /// 图像增强类型
        /// </summary>
        private EnhanType _Enhan;
        public EnhanType m_Enhan
        {
            get { return _Enhan; }
            set { _Enhan = value; }
        }

        /// <summary>
        /// 对应的参数
        /// </summary>
        private string _Param;
        public string m_Param
        {
            get { return _Param; }
            set { _Param = value; }
        }

        [NonSerialized]
        public PluginFrmBase frm_Obj;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="Obj"></param>
        public BaseMethod(PluginFrmBase Obj)
        {
            frm_Obj = Obj;
        }

        /// <summary>
        /// 图像增强
        /// </summary>
        public virtual void Execute() { }

        /// <summary>
        /// 写入参数
        /// </summary>
        public virtual void WritepRram() { }

    }
}
