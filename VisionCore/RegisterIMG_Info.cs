using PublicDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VisionCore
{
    /// <summary>
    /// 注册图像信息
    /// </summary>
    [Serializable]
    public class RegisterIMG_Info
    {

        /// <summary>
        /// 注册图像ID
        /// 流程名称+模块名称
        /// </summary>
        private string _Image_ID;
        public string m_Image_ID
        {
            set { _Image_ID = value; }
            get { return _Image_ID; }
        }

        /// <summary>
        /// 注册图像（模板匹配切割后的）
        /// </summary>
        private HImageExt _Image;
        public HImageExt m_Image
        {
            set { _Image = value; }
            get { return _Image; }
        }

        /// <summary>
        /// 原尺寸地址
        /// </summary>
        private string _Image_Pth;

        public string m_Image_Pth
        {
            get { return _Image_Pth; }
            set { _Image_Pth = value; }
        }

        /// <summary>
        /// 构造函数1
        /// </summary>
        public RegisterIMG_Info()
        {

        }

        /// <summary>
        /// 构造函数2
        /// </summary>
        /// <param name="_ImageID"></param>
        /// <param name="_Himage"></param>
        /// <param name="_ImagePath"></param>
        public RegisterIMG_Info(string _ImageID, HImageExt _Himage, string _ImagePath)
        {
            _Image_ID = _ImageID;
            _Image = _Himage;
            _Image_Pth = _ImagePath;
        }
    }

    /// <summary>
    /// 展示图像分类
    /// </summary>
    public enum ImageCatagory
    {
        当前图像,
        注册图像
    }
}
