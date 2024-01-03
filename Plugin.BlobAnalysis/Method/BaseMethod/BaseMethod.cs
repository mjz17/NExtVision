using HalconDotNet;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.BlobAnalysis
{
    [Serializable]
    public class BaseMethod
    {

        #region 是否启用

        /// <summary>
        /// 是否启用
        /// </summary>
        private bool _EnableOrnot = true;
        public bool m_EnableOrnot
        {
            get { return _EnableOrnot; }
            set { _EnableOrnot = value; }
        }

        #endregion

        #region 输入Region

        /// <summary>
        /// 输入Region
        /// </summary>
        [NonSerialized]
        private HRegion _InHRegion;
        public HRegion m_InHRegion
        {
            get { return _InHRegion; }
            set { _InHRegion = value; }
        }

        #endregion

        #region 输出Region

        /// <summary>
        /// 输出Region
        /// </summary>
        [NonSerialized]
        private HObject _OutObj;
        public HObject m_OutObj
        {
            get { return _OutObj; }
            set { _OutObj = value; }
        }

        #endregion


        [NonSerialized]
        public System.Windows.Controls.UserControl m_Control;

        #region Blobl类型

        /// <summary>
        /// Blobl类型
        /// </summary>
        private BlobMethod _blobMethod;
        public BlobMethod m_blobMethod
        {
            get { return _blobMethod; }
            set { _blobMethod = value; }
        }

        #endregion

        /// <summary>
        /// 当前模块的索引
        /// </summary>
        [NonSerialized]
        public int m_Index = 0;

        /// <summary>
        /// 总长度
        /// </summary>
        [NonSerialized]
        public int m_IndexLength = 0;

        #region 链接的索引

        /// <summary>
        /// 链接的索引
        /// </summary>
        private string _LinkIndex= "上一个区域";
        public string m_LinkIndex
        {
            get { return _LinkIndex; }
            set { _LinkIndex = value; }
        }

        #endregion

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
        public virtual void SetRram() { }

        public List<string> InitCmb()
        {
            List<string> list = new List<string>();
            list.Add("上一个区域");
            if (m_IndexLength > 0)
            {
                for (int i = 0; i < m_IndexLength - 1; i++)
                {
                    list.Add(i.ToString());
                }
            }
            return list;
        }

    }
}
