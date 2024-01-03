using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    public enum ModelType
    {
        None,
        形状模板,
        灰度模板,
    }

    public enum ConstantVar
    {
        //模块基础
        状态,
        时间,
        //采图模块
        图像,
        图像效正,
        图像预处理,
        阈值区域,
        区域,
        轮廓,
        //仿射变换
        Coord轮廓,
        Coord图像,
        Coord世界,
        //二维码识别
        QrCord,
        //模板匹配
        坐标X图像,
        坐标Y图像,
        坐标Deg图像,

        坐标X图像数组,
        坐标Y图像数组,
        坐标Deg图像数组,

        //直线测量
        Line图像,
        Line_StartX图像,
        Line_StartY图像,
        Line_EndX图像,
        Line_EndY图像,
        直线中心X图像,
        直线中心Y图像,
        直线角度Rad,
        直线角度Phi,
        Line世界,

        //直线测量
        坐标X世界,
        坐标Y世界,
        坐标Deg世界,
        //圆测量
        CircleX图像,
        CircleY图像,
        圆半径Radius,
        Circle图像,
        Circle世界,
        //N点标定
        次数,
        HomMat2D,
        使用旋转中心,
        Rotation_I_X,     //旋转中心X图像坐标
        Rotation_I_Y,     //旋转中心Y图像坐标
        Rotation_M_X,   //旋转中心X实际机械坐标
        Rotation_M_Y,   //旋转中心Y实际机械坐标
        Criterion_Phi,  //基准角度

        //Blob
        面积,
        Area_Row,
        Area_Col,
        筛选结果,
        //循环
        索引,

        //机械手控制
        输出坐标X,
        输出坐标Y,
        输出Deg,

        //数据
        Bool,
        Int,
        Double,
        Double数组,
        Float,
        String,

        //通讯状态
        通讯Connect状态,

        距离图像,
        距离世界,

    }
}
