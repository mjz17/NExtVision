using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using HalconDotNet;

namespace Plugin.VectorAngleToRigid
{
    [Category("坐标变换")]
    [DisplayName("仿射变换")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {

        #region 输入坐标FormX

        /// <summary>
        /// 输入坐标FormX
        /// </summary>
        public string m_InputFormX = string.Empty;

        /// <summary>
        /// 输入坐标FormX
        /// </summary>
        public DataVar Link_InputFormX;

        /// <summary>
        /// 输入坐标FormX
        /// </summary>
        [NonSerialized]
        public double InputFormX_Data = 0;

        #endregion
        #region 输入坐标FormY

        /// <summary>
        /// 输入坐标FormY
        /// </summary>
        public string m_InputFormY = string.Empty;

        /// <summary>
        /// 输入坐标FormY
        /// </summary>
        public DataVar Link_InputFormY;

        /// <summary>
        /// 输入坐标FormY
        /// </summary>
        [NonSerialized]
        public double InputFormY_Data = 0;

        #endregion
        #region 输入坐标FormAngle

        /// <summary>
        /// 输入坐标FormAngle
        /// </summary>
        public string m_InputFormAngle = string.Empty;

        /// <summary>
        /// 输入坐标FormAngle
        /// </summary>
        public DataVar Link_InputFormAngle;

        /// <summary>
        /// 输入坐标FormAngle
        /// </summary>
        [NonSerialized]
        public double InputFormAngle_Data = 0;

        #endregion

        #region 输出坐标ToX

        /// <summary>
        /// 输出坐标ToX
        /// </summary>
        public string m_InputToX = string.Empty;

        /// <summary>
        /// 输出坐标ToX
        /// </summary>
        public DataVar Link_InputToX;

        /// <summary>
        /// 输出坐标ToX
        /// </summary>
        [NonSerialized]
        public double InputToX_Data = 0;

        #endregion
        #region 输出坐标ToY

        /// <summary>
        /// 输出坐标ToY
        /// </summary>
        public string m_InputToY = string.Empty;

        /// <summary>
        /// 输出坐标ToY
        /// </summary>
        public DataVar Link_InputToY;

        /// <summary>
        /// 输出坐标ToY
        /// </summary>
        [NonSerialized]
        public double InputToY_Data = 0;

        #endregion
        #region 输入坐标ToAngle

        /// <summary>
        /// 输入坐标ToAngle
        /// </summary>
        public string m_InputToAngle = string.Empty;

        /// <summary>
        /// 输入坐标ToAngle
        /// </summary>
        public DataVar Link_InputToAngle;

        /// <summary>
        /// 输入坐标ToAngle
        /// </summary>
        [NonSerialized]
        public double InputToAngle_Data = 0;

        #endregion

        #region 输出旋转中心坐标X

        /// <summary>
        /// 输出旋转中心坐标X
        /// </summary>
        public string m_InputCenterX = string.Empty;

        /// <summary>
        /// 输出旋转中心坐标X
        /// </summary>
        public DataVar Link_InputCenterX;

        /// <summary>
        /// 输出旋转中心坐标X
        /// </summary>
        [NonSerialized]
        public double InputCenterX_Data = 0;

        #endregion
        #region 输出旋转中心坐标Y

        /// <summary>
        /// 输出旋转中心坐标Y
        /// </summary>
        public string m_InputCenterY = string.Empty;

        /// <summary>
        /// 输出旋转中心坐标Y
        /// </summary>
        public DataVar Link_InputCenterY;

        /// <summary>
        /// 输出旋转中心坐标Y
        /// </summary>
        [NonSerialized]
        public double InputCenterY_Data = 0;

        #endregion
        #region 输入坐标ToAngle

        /// <summary>
        /// 输入坐标ToAngle
        /// </summary>
        public string m_InputCenterAngle = string.Empty;

        /// <summary>
        /// 输入坐标ToAngle
        /// </summary>
        public DataVar Link_CenterAngle;

        /// <summary>
        /// 输入坐标ToAngle
        /// </summary>
        [NonSerialized]
        public double InputCenterAngle_Data = 0;

        #endregion

        /// <summary>
        /// 点映射/点集映射
        /// </summary>
        public bool PointOrList = false;

        #region 输入点X

        /// <summary>
        /// 输入点X
        /// </summary>
        public string m_InputX = string.Empty;

        /// <summary>
        /// 输入点X
        /// </summary>
        public DataVar Link_InputX;

        /// <summary>
        /// 输入点X
        /// </summary>
        [NonSerialized]
        public object InputX_Data = 0;

        #endregion
        #region 输入点Y

        /// <summary>
        /// 输入点Y
        /// </summary>
        public string m_InputY = string.Empty;

        /// <summary>
        /// 输入点Y
        /// </summary>
        public DataVar Link_InputY;

        /// <summary>
        /// 输入点Y
        /// </summary>
        [NonSerialized]
        public object InputY_Data = 0;

        #endregion

        //输出点X
        [NonSerialized]
        public object m_OutX;

        //输出点Y
        [NonSerialized]
        public object m_OutY;

        /// <summary>
        /// 仿射矩阵
        /// </summary>
        public HHomMat2D PointHHomMat2D { get; set; }










    }
}
