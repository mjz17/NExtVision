using HalconDotNet;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.RobotCotrol
{
    public class RobotCommon
    {

        /// <summary>
        /// 输入图像坐标，输出机械坐标
        /// </summary>
        /// <param name="m_HomMat2D">矩阵</param>
        /// <param name="InputImageX">输入图像坐标</param>
        /// <param name="InputImageY">输入图像坐标</param>
        /// <param name="OutX">输出机械坐标X</param>
        /// <param name="OutY">输出机械坐标Y</param>
        public void AffineTrans(HHomMat2D m_HomMat2D, double InputImageX, double InputImageY, out double OutX, out double OutY)
        {
            OutX = m_HomMat2D.AffineTransPoint2d(InputImageX, InputImageY, out OutY);//2023.5.29，赵一修改，线线相交，坐标
        }

        /// <summary>
        /// 固定相机,先拍后抓,机械手旋转中心对物料中心
        /// </summary>
        /// <param name="m_HomMat2D">输入矩阵</param>
        /// <param name="InputCircleX">输入旋转中心（图像坐标X）</param>
        /// <param name="InputCircleY">输入旋转中心（图像坐标Y）</param>
        /// <param name="InputCircleX_W">输入旋转中心（机械坐标X）</param>
        /// <param name="InputCircleY_W">输入旋转中心（机械坐标Y）</param>
        /// <param name="InputImageX">输入图像坐标</param>
        /// <param name="InputImageY">输入图像坐标</param>
        /// <param name="OutX">输出机械坐标</param>
        /// <param name="OutY">输出机械坐标</param>
        public void AffineTransRotate(HHomMat2D m_HomMat2D, double InputCircleX, double InputCircleY,
            double InputCircleX_W, double InputCircleY_W,
            double InputImageX, double InputImageY, out double OutX, out double OutY)
        {
            double RigidX, RigidY;
            double MidX, MidY;
            double Mid_OutX, Mid_OutY;

            //旋转中心图像坐标映射为世界坐标（机械坐标）
            RigidX = m_HomMat2D.AffineTransPoint2d(InputCircleY, InputCircleX, out RigidY);
            //旋转中心世界坐标和实际的机械坐标进行差值
            MidX = InputCircleY_W - RigidX;
            MidY = InputCircleX_W - RigidY;

            //输入图像坐标转为机械坐标
            Mid_OutX = m_HomMat2D.AffineTransPoint2d(InputImageX, InputImageY, out Mid_OutY);

            //旋转中心差值+图像坐标的映射=机械手中心抓取坐标
            OutX = MidX + Mid_OutX;
            OutY = MidY + Mid_OutY;
        }

        /// <summary>
        /// 先抓后拍
        /// </summary>
        /// <param name="m_HomMat2D">输入矩阵</param>
        /// <param name="InputCircleX">旋转中心X</param>
        /// <param name="InputCircleY">旋转中心Y</param>
        /// <param name="InputImageX">输入图像坐标X</param>
        /// <param name="InputImageY">输入图像坐标Y</param>
        /// <param name="Phi">角度</param>
        /// <param name="ReferenceX">参考坐标X</param>
        /// <param name="ReferenceY">参考坐标Y</param>
        /// <param name="InputMachX">输入机械坐标X</param>
        /// <param name="InputMachY">输入机械坐标Y</param>
        /// <param name="OutMachX">输出机械坐标X</param>
        /// <param name="OutMachY">输出机械坐标Y</param>
        public void AffineTransRotate(HHomMat2D m_HomMat2D, double InputCircleX, double InputCircleY, double InputImageX, double InputImageY,
            double Phi, double ReferenceX, double ReferenceY, double InputMachX, double InputMachY, out double OutMachX, out double OutMachY)
        {

            double RigidX, RigidY;
            double MidX, MidY;
            double MidX1, MidY1;
            double MidX2, MidY2;

            //角度转弧度
            double radians = (Math.PI / 180) * Phi;
            //先旋转,根据旋转中心，旋转角度，获得新的位置
            HHomMat2D mat2D = new HHomMat2D();
            mat2D.VectorAngleToRigid(InputCircleX, InputCircleY, 0, InputCircleX, InputCircleY, 0);
            RigidX = mat2D.AffineTransPoint2d(InputImageX, InputImageY, out RigidY); 
            //再平移
            //转为机械坐标系
            MidX = m_HomMat2D.AffineTransPoint2d(RigidX, RigidY, out MidY);
            MidX1 = m_HomMat2D.AffineTransPoint2d(ReferenceX, ReferenceY, out MidY1);
            //测出位置差
            mat2D = new HHomMat2D();
            mat2D.VectorAngleToRigid(MidX, MidY, 0, MidX1, MidY1, 0);
            MidX2 = mat2D.AffineTransPoint2d(0, 0, out MidY2);

            //输出机械坐标
            OutMachX = InputMachX + MidX2;
            OutMachY = InputMachY + MidY2;

        }


    }
}
