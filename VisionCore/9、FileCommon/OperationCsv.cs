using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace VisionCore
{
    /// <summary>
    /// Csv操作类
    /// </summary>
    public class OperationCsv
    {
        static object obj = new object();

        /// <summary>
        /// 写入地址
        /// </summary>
        /// <param name="CsvPath">写入路径</param>
        /// <param name="HeaderName">表头</param>
        /// <param name="Content">内容</param>
        public void WriteCsv(string CsvPath, string CsvName, List<string> HeaderName, List<string> Content)
        {
            lock (obj)
            {
                //保存路径
                //string path = CsvPath + "\\CSV\\";
                //文件名
                string fileName = CsvPath + DateTime.Now.ToString("yyyy-MM-dd-HH") + CsvName + ".csv";
                //年月日小时分钟秒
                string Datedate = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");

                try
                {
                    //判断是否有文件夹
                    if (!Directory.Exists(CsvPath))
                    {
                        Directory.CreateDirectory(CsvPath);
                    }
                    if (!File.Exists(fileName))
                    {
                        StreamWriter sw = new StreamWriter(fileName, true, Encoding.UTF8);
                        string strName = "时间,";
                        //表头
                        for (int i = 0; i < HeaderName.Count; i++)
                        {
                            strName += HeaderName[i] + ",";
                        }
                        strName += "\t\n";
                        sw.Write(strName);
                        sw.Close();
                    }


                    //填入的信息
                    string ContentInfo = string.Empty;
                    for (int i = 0; i < Content.Count; i++)
                    {
                        ContentInfo += Content[i] + ",";
                    }
                    string str = Datedate + "," + ContentInfo + "\t\n";

                    //创建码流类
                    StreamWriter swl = new StreamWriter(fileName, true, Encoding.UTF8);
                    swl.Write(str);
                    swl.Close();

                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


    }

}
