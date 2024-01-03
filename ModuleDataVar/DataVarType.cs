using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ModuleDataVar
{
    public class DataVarType
    {
        /// <summary>
        /// 数据类型
        /// </summary>
        public enum DataType
        {
            Int,            //Int类型
            Double,         //Double类型
            Float,          //浮点类型
            Bool,           //布尔型类型
            String,         //字符类型

            Int_Array,      //整数数组
            Double_Array,   //Double数组
            Bool_Array,     //布尔型数组
            String_Array,   //字符数组

            Image,          //图像
            Line,           //直线类型
            Circle,         //圆类型
            Rectangle1,     //矩形1类型
            Rectangle2,     //矩形2类型

            坐标系,         //点坐标
            位置转换2D,     //HHomMat2D   
            平面信息,       
            区域,
            轮廓,
        }

        /// <summary>
        /// 所属类型
        /// </summary>
        public enum DataGroup
        {
            单量,
            数组,
        }

        /// <summary>
        /// 变量属性
        /// </summary>
        public enum DataAtrribution
        {
            全局变量,
            局部变量,
        }

    }
}
