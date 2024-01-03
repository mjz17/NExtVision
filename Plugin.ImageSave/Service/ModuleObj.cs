using Common;
using HalconControl;
using HalconDotNet;
using ModuleDataVar;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;
using VisionCore;

namespace Plugin.ImageSave
{
    [Category("图像处理")]
    [DisplayName("存储图像")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {

        /// <summary>
        /// 保存方式
        /// </summary>
        private SaveInfo _SaveInfo;

        public SaveInfo m_SaveInfo
        {
            get { return _SaveInfo; }
            set { _SaveInfo = value; }
        }

        #region 链接图像

        /// <summary>
        /// 当前图像名称
        /// </summary>
        public string m_CurentImgName = string.Empty;

        /// <summary>
        /// 链接数据信息
        /// </summary>
        public DataVar Link_Image_Data;

        #endregion

        //保存路径
        public string SavePath;

        /// <summary>
        /// 保存图像格式
        /// </summary>
        private Imageformat _Imageformat;
        public Imageformat m_Imageformat
        {
            get { return _Imageformat; }
            set { _Imageformat = value; }
        }

        /// <summary>
        /// 图像根目录
        /// </summary>
        private string _SaveName;
        public string m_SaveName
        {
            get { return _SaveName; }
            set { _SaveName = value; }
        }

        /// <summary>
        /// 等待图片保存完成
        /// </summary>
        private bool _WaitImageSave;

        public bool m_WaitImageSave
        {
            get { return _WaitImageSave; }
            set { _WaitImageSave = value; }
        }

        /// <summary>
        /// 是否清理文件夹
        /// </summary>
        private bool _IsClearFile;

        public bool m_IsClearFile
        {
            get { return _IsClearFile; }
            set { _IsClearFile = value; }
        }

        /// <summary>
        /// 清理时间
        /// </summary>
        private int _ClearTime;

        public int m_ClearTime
        {
            get { return _ClearTime; }
            set { _ClearTime = value; }
        }


        /// <summary>
        /// 执行模块
        /// </summary>
        /// <param name="blnByHand"></param>
        public override void ExeModule(bool blnByHand = false)
        {
            try
            {
                base.ExeModule(blnByHand);
                sw = new System.Diagnostics.Stopwatch();//模块运行时间
                sw.Start();

                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    //判断保存方式
                    if (m_SaveInfo == SaveInfo.保存图片)
                    {
                        //加载链接图像
                        DataVar data = ModuleProject.GetLocalVarValue(Link_Image_Data);

                        if (data.m_DataValue != null && data.m_DataValue is List<HImageExt>)
                        {
                            m_Image = ((List<HImageExt>)(data).m_DataValue)[0];
                            if (m_Image != null || m_Image.IsInitialized())
                            {
                                //是否按照时间清空数据
                                if (m_IsClearFile)
                                {
                                    if (Directory.Exists(SavePath + "\\" + m_SaveName))
                                    {
                                        DirectoryInfo file = new DirectoryInfo(SavePath + "\\" + m_SaveName);
                                        DirectoryInfo[] fileinfo = file.GetDirectories();

                                        for (int i = 0; i < fileinfo.Length; i++)
                                        {
                                            DateTime time1 = fileinfo[i].CreationTime;
                                            DateTime time2 = DateTime.Now;
                                            TimeSpan d3 = time2.Subtract(time1);
                                            //时间大于设置天数
                                            if (d3.Days > m_ClearTime)
                                            {
                                                fileinfo[i].Delete(true);
                                            }
                                        }
                                    }
                                }

                                //等待保存完成
                                if (m_WaitImageSave)
                                {
                                    m_Image.WriteImageExt(SavePath, m_SaveName, m_Imageformat.ToString());
                                }
                                else
                                {
                                    Task.Run(new Action(() =>
                                    {
                                        ModuleParam.BlnSuccessed = true;
                                        m_Image.WriteImageExt(SavePath, m_SaveName, m_Imageformat.ToString());
                                    }));
                                }
                                ModuleParam.BlnSuccessed = true;
                            }
                            else
                            {
                                ModuleParam.BlnSuccessed = false;
                            }
                        }
                        else
                        {
                            ModuleParam.BlnSuccessed = false;
                        }
                    }
                    else
                    {
                        //加载链接图像
                        DataVar data = ModuleProject.GetLocalVarValue(Link_Image_Data);

                        if (data.m_DataValue != null && data.m_DataValue is List<HImageExt>)
                        {
                            //更新窗体的显示
                            if (SysProcessPro.g_ProjectList[proIndex].m_Hwindow != null)
                            {
                                if (SysProcessPro.g_ProjectList[proIndex].m_Hwindow.hWindow == null)
                                {
                                    SysProcessPro.g_ProjectList[proIndex].m_Hwindow.hWindow = new HWindow_Final();
                                }

                                SysProcessPro.g_ProjectList[proIndex].m_Hwindow.m_HImageExt = ((List<HImageExt>)(data).m_DataValue)[0].Clone();
                                SysProcessPro.g_ProjectList[proIndex].m_Hwindow.m_RepaintHwindow();
                            }

                            if (m_IsClearFile)
                            {
                                if (Directory.Exists(SavePath + "\\" + m_SaveName))
                                {
                                    DirectoryInfo file = new DirectoryInfo(SavePath + "\\" + m_SaveName);
                                    DirectoryInfo[] fileinfo = file.GetDirectories();

                                    for (int i = 0; i < fileinfo.Length; i++)
                                    {
                                        DateTime time1 = fileinfo[i].CreationTime;
                                        DateTime time2 = DateTime.Now;
                                        TimeSpan d3 = time2.Subtract(time1);
                                        //时间大于设置天数
                                        if (d3.Days > m_ClearTime)
                                        {
                                            fileinfo[i].Delete(true);
                                        }
                                    }
                                }
                            }

                            HWindow hWindow = null;

                            hWindow = SysProcessPro.g_ProjectList[proIndex].m_Hwindow.hWindow.getHWindowControl().HalconWindow;

                            if (m_Image == null)//如果对象为空
                            {
                                m_Image = new HImageExt();
                            }

                            if (hWindow != null)
                            {
                                if (m_WaitImageSave)
                                {
                                    m_Image.WriteImageExt(SavePath, m_SaveName, m_Imageformat.ToString(), hWindow);
                                }
                                else
                                {
                                    Task.Run(new Action(() =>
                                    {
                                        ModuleParam.BlnSuccessed = true;
                                        m_Image.WriteImageExt(SavePath, m_SaveName, m_Imageformat.ToString(), hWindow);
                                    }));
                                }
                                ModuleParam.BlnSuccessed = true;
                            }
                            else
                            {
                                ModuleParam.BlnSuccessed = false;
                            }
                        }
                        else
                        {
                            ModuleParam.BlnSuccessed = false;
                        }
                    }
                }
                else
                {
                    ModuleParam.BlnSuccessed = false;
                }
            }
            catch (Exception ex)
            {
                //运行失败
                ModuleParam.BlnSuccessed = false;
                Log.Error(string.Format($"{ModuleProject.ProjectInfo.m_ProjectName}{","}{ModuleParam.ModuleName}{",执行失败,"}{ex.ToString()}"));
            }
            finally
            {
                sw.Stop();
                ModuleParam.ModuleCostTime = Convert.ToInt32(sw.ElapsedMilliseconds);
                //模块运行状态
                {
                    DataVar objStatus = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.状态.ToString(),
                       DataVarType.DataType.Bool, DataVarType.DataGroup.单量, 1, "false", ModuleParam.ModuleName, ModuleParam.BlnSuccessed);
                    ModuleProject.UpdateLocalVarValue(objStatus);
                }
                //模块运行状态
                {
                    DataVar objTime = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.时间.ToString(),
                       DataVarType.DataType.Int, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, ModuleParam.ModuleCostTime);
                    ModuleProject.UpdateLocalVarValue(objTime);
                }
                sw.Reset();
            }
        }


    }

    [Serializable]
    public enum Imageformat
    {
        tiff,
        jpg,
        png,
        bmp,
    }

    [Serializable]
    public enum SaveInfo
    {
        保存图片,
        截取窗体,
    }

}
