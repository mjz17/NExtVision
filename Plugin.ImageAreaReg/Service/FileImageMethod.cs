using HalconDotNet;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.ImageAreaReg
{
    [Serializable]
    public class FileImageMethod
    {
        /// <summary>
        /// 文件夹路径
        /// </summary>
        private string _filePath;
        public string m_filePath
        {
            get { return _filePath; }
            set { _filePath = value; }
        }

        /// <summary>
        /// 循环运行
        /// </summary>
        private bool _cycleRun;
        public bool m_cycleRun
        {
            get { return _cycleRun; }
            set { _cycleRun = value; }
        }

        //索引
        [NonSerialized]
        public int m_Index = 0;

        //文件集合
        [NonSerialized]
        FileInfo[] m_FileList;

        public FileImageMethod()
        {

        }

        /// <summary>
        /// 将图片格式的文件读取出来
        /// </summary>
        /// <returns></returns>
        public List<FileImageModel> ReadFileImageName()
        {
            try
            {
                List<FileImageModel> fileName = new List<FileImageModel>();
                DirectoryInfo di = new DirectoryInfo(m_filePath);
                m_FileList = di.GetFiles();
                for (int i = 0; i < m_FileList.Length; i++)
                {
                    string ext = System.IO.Path.GetExtension(m_FileList[i].Name.ToUpper());
                    if (ext == ".HE" || ext == ".BMP" || ext == ".JPG" || ext == ".PNG" || ext == ".TIFF" || ext == ".TIF" || ext == ".JPEG")
                    {
                        fileName.Add(new FileImageModel() { m_imageName = m_FileList[i].Name, m_imagePath = m_FileList[i].FullName });
                    }
                }
                return fileName;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public HImage GetFileToImage(List<FileImageModel> Info)
        {
            try
            {
                if (m_Index >= Info.Count)
                    m_Index = 0;

                string fullName = Info[m_Index].m_imagePath;

                if (m_cycleRun)
                {
                    m_Index++;
                }

                return new HImage(fullName);
            }
            catch (Exception)
            {
                throw;
            }
        }

    }

    [Serializable]
    public class FileImageModel
    {
        //文件路径
        private string _imagePath;
        public string m_imagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }

        //文件名称
        private string _imageName;
        public string m_imageName
        {
            get { return _imageName; }
            set { _imageName = value; }
        }
    }

}
