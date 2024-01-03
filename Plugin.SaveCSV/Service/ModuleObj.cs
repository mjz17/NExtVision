using Common;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Markup;
using VisionCore;

namespace Plugin.SaveCSV
{
    [Category("通讯测试")]
    [DisplayName("CSV存储")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {

        /// <summary>
        /// 保存地址
        /// </summary>
        private string _SavePath;
        public string m_SavePath
        {
            get { return _SavePath; }
            set { _SavePath = value; }
        }

        /// <summary>
        /// 文件名称
        /// </summary>
        private string _SaveName;
        public string m_SaveName
        {
            get { return _SaveName; }
            set { _SaveName = value; }
        }

        /// <summary>
        /// 是否清理
        /// </summary>
        private bool _IsClearFile;
        public bool m_IsClearFile
        {
            get { return _IsClearFile; }
            set { _IsClearFile = value; }
        }

        /// <summary>
        /// 清除时间
        /// </summary>
        private int _ClearTime;
        public int m_ClearTime
        {
            get { return _ClearTime; }
            set { _ClearTime = value; }
        }

        /// <summary>
        /// 是否定义列头
        /// </summary>
        private bool _IsColumnHead;
        public bool m_IsColumnHead
        {
            get { return _IsColumnHead; }
            set { _IsColumnHead = value; }
        }

        /// <summary>
        /// 列头
        /// </summary>
        private string _ColumnHead;
        public string m_ColumnHead
        {
            get { return _ColumnHead; }
            set { _ColumnHead = value; }
        }

        /// <summary>
        /// 存储的链接地址
        /// </summary>
        public List<DataVar> m_LinkData = new List<DataVar>();

        public override void ExeModule(bool blnByHand = false)
        {
            try
            {
                base.ExeModule(blnByHand);
                sw = new System.Diagnostics.Stopwatch();
                sw.Start();

                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    if (m_LinkData.Count > 0)
                    {

                        string SavePath = m_SavePath + "\\CSV\\";

                        List<string> HeaderName = new List<string>();
                        List<string> Content = new List<string>();

                        //自定义列头
                        if (m_IsColumnHead)
                        {
                            //数据出列模块
                            string[] Info = m_ColumnHead.Split('/');
                            for (int i = 0; i < Info.Length; i++)
                            {
                                HeaderName.Add(Info[i]);
                            }

                            //设置文件列头
                            foreach (DataVar item in m_LinkData)
                            {
                                DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == item.m_DataName &&
                                c.m_DataModuleID == item.m_DataModuleID);
                                Content.Add(data.m_DataValue.ToString());
                            }
                        }
                        else
                        {
                            //设置文件列头
                            foreach (DataVar item in m_LinkData)
                            {
                                DataVar data = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataName == item.m_DataName &&
                                c.m_DataModuleID == item.m_DataModuleID);

                                HeaderName.Add(data.m_DataName);
                                Content.Add(data.m_DataValue.ToString());
                            }
                        }

                        //是否按照时间清除文件
                        if (m_IsClearFile)
                        {
                            if (Directory.Exists(SavePath))
                            {
                                DirectoryInfo file = new DirectoryInfo(SavePath);
                                FileInfo[] fileinfo = file.GetFiles();
                                for (int i = 0; i < fileinfo.Length; i++)
                                {
                                    DateTime time1 = fileinfo[i].CreationTime;
                                    DateTime time2 = DateTime.Now;
                                    TimeSpan d3 = time2.Subtract(time1);
                                    //时间大于设置天数
                                    if (d3.Days > m_ClearTime)
                                    {
                                        fileinfo[i].Delete();
                                    }
                                }
                            }
                        }

                        //操作文件
                        OperationCsv operation = new OperationCsv();
                        operation.WriteCsv(SavePath, m_SaveName, HeaderName, Content);

                    }

                    ModuleParam.BlnSuccessed = true;
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
}
