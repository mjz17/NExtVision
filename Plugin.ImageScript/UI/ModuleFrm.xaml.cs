using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionCore;
using ScintillaNET;
using System.Windows.Forms;
using Microsoft.Win32;
using HalconDotNet;
using System.Drawing;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Reflection.Emit;
using Common;
using ModuleDataVar;
using StyleBase;

namespace Plugin.ImageScript
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        private ModuleObj frm_ModuleObj;

        private List<string> m_EProcedureNameList = new List<string>();//记录内部函数的名称,用以染色


        /// <summary>
        /// 可用于运行的函数名称列表
        /// </summary>
        private List<string> RunProcedureNameList
        {
            get
            {
                List<string> nameList = new List<string>();
                if (frm_ModuleObj.m_EProcedureList != null)
                {
                    foreach (EProcedure item in frm_ModuleObj.m_EProcedureList)
                    {
                        //主函数不添加到列表中 
                        if (item.Name != "main")
                        {
                            nameList.Add(item.Name);
                        }
                    }
                }
                return nameList;
            }
        }

        public ModuleFrm()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 16;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            this.DataContext = this;
            this.Loaded += ModuleFrm_Loaded;
        }

        private void ModuleFrm_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //加载参数
                frm_ModuleObj = (ModuleObj)m_ModuleObjBase;
                //窗体名称
                Title.HeadName = frm_ModuleObj.ModuleParam.ModuleName;

                if (m_ModuleObjBase.blnNewModule)
                    theSecondTime();

                m_MyEditer.TextChanged += TextBox1_TextChanged;//文本改变 修改注释颜色                                                                       //初始化控件
                InitScintilla();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        //初次打开
        public override void theFirsttime()
        {
            base.theFirsttime();
        }

        //非初次打开
        public override void theSecondTime()
        {
            base.theSecondTime();

            if (frm_ModuleObj.m_EProcedureList == null)
                return;

            m_EProcedureNameList.Clear();

            List<string> eProcedureNameList = new List<string>();
            //设置名称
            foreach (EProcedure eProcedure in frm_ModuleObj.m_EProcedureList)
            {
                eProcedureNameList.Add(eProcedure.GetProcedureMethod());
                m_EProcedureNameList.Add(eProcedure.Name);
            }
            CmbprocedureMethod.ItemsSource = eProcedureNameList;

            if (eProcedureNameList.Count > 0)
            {
                CmbprocedureMethod.SelectedIndex = 0;
            }

            //根据外部函数名称 染色
            InitSyntaxColoring(m_MyEditer);

            CmbRunProcedureMethod.ItemsSource = null;
            CmbRunProcedureMethod.ItemsSource = RunProcedureNameList;
            if (CmbRunProcedureMethod.Items.Count > 0)
            {
                CmbRunProcedureMethod.SelectedIndex = 0;
                CmbprocedureMethod_DropDownClosed(null, null);//如果之前就在第一个选项 不会触发更新,故手动更新一次 magical 2019-2-18 20:28:52
            }

            dgv_LinkInput.ItemsSource = null;
            dgv_LinkInput.ItemsSource = frm_ModuleObj.m_Inputlink;

            dgv_LinkOutput.ItemsSource = null;
            dgv_LinkOutput.ItemsSource = frm_ModuleObj.m_OutputVar;
        }

        #region sci控件

        /// <summary>
        /// 初始化染色控件
        /// </summary>
        private void InitScintilla()
        {
            // 字体包裹模式
            m_MyEditer.WrapMode = WrapMode.None;
            //自定义关键字代码提示功能
            m_MyEditer.AutoCIgnoreCase = true;//代码提示的时候,不区分大小写
            //染色
            InitSyntaxColoring(m_MyEditer);
            // 这两种操作会导致乱码
            m_MyEditer.ClearCmdKey(Keys.Control | Keys.S);
            m_MyEditer.ClearCmdKey(Keys.Control | Keys.F);
        }

        //设置语法高亮规则
        private void InitSyntaxColoring(Scintilla scintilla)
        {
            // 设置默认格式
            scintilla.StyleResetDefault();
            scintilla.Styles[ScintillaNET.Style.Default].Font = "Consolas";
            scintilla.Styles[ScintillaNET.Style.Default].Size = 10;
            //背景色
            scintilla.Styles[ScintillaNET.Style.Default].ForeColor = System.Drawing.Color.Black;
            scintilla.StyleClearAll();
            scintilla.ScrollWidth = 100;//设置水平滚动条为100 这样水平就不会默认显示滚动条

            //普通代码的颜色
            scintilla.Styles[ScintillaNET.Style.Sql.Default].ForeColor = ColorTranslator.FromHtml("#644614");
            scintilla.Styles[ScintillaNET.Style.Sql.Comment].ForeColor = ColorTranslator.FromHtml("#644614");
            scintilla.Styles[ScintillaNET.Style.Sql.Number].ForeColor = ColorTranslator.FromHtml("#FF6532");
            scintilla.Styles[ScintillaNET.Style.Sql.Character].ForeColor = ColorTranslator.FromHtml("#A31515");
            // scintilla.Styles[ScintillaNET.Style.Sql.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
            //操作符
            scintilla.Styles[ScintillaNET.Style.Sql.Operator].ForeColor = ColorTranslator.FromHtml("#644614");
            scintilla.Styles[ScintillaNET.Style.Sql.User1].ForeColor = ColorTranslator.FromHtml("#0000FF");//关键字
            scintilla.SetKeywords(4, Keyword.s_HalconString.ToLower());
            scintilla.Styles[ScintillaNET.Style.Sql.User2].ForeColor = ColorTranslator.FromHtml("#000096");//halcon算子
            scintilla.SetKeywords(5, Keyword.s_HalconProcedure.ToLower());  //这里的索引需要去查看 ScintillaNET.Style.Sql.Word2 对应的注释
            scintilla.Styles[ScintillaNET.Style.Sql.User3].ForeColor = ColorTranslator.FromHtml("#640096");//本地函数
            scintilla.SetKeywords(6, string.Join(" ", m_EProcedureNameList).ToLower());
            scintilla.Lexer = Lexer.Sql;

            //行号字体颜色
            scintilla.Styles[ScintillaNET.Style.LineNumber].ForeColor = ColorTranslator.FromHtml("#8DA3C1");
            //行号相关设置
            var nums = scintilla.Margins[1];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            //注释
            int NUM = 8; // Indicators 0-7 could be in use by a lexerso we'll use indicator 8 to highlight words.
            m_MyEditer.IndicatorCurrent = NUM;//
            m_MyEditer.Indicators[NUM].Style = IndicatorStyle.TextFore;
            m_MyEditer.Indicators[NUM].ForeColor = ColorTranslator.FromHtml("#008000");
            m_MyEditer.Indicators[NUM].OutlineAlpha = 100;
            m_MyEditer.Indicators[NUM].Alpha = 100;
        }

        protected void TextBox1_TextChanged(object sender, EventArgs e)
        {
            ChangeAnnotationColor();
        }

        //修改注释的文本颜色
        private void ChangeAnnotationColor()
        {
            m_MyEditer.IndicatorClearRange(0, m_MyEditer.TextLength);
            foreach (ScintillaNET.Line line in m_MyEditer.Lines)
            {
                if (line.Text.Trim().StartsWith("*"))//开始的是* 则是注释符号
                {
                    string text = line.Text;
                    // Update indicator appearance
                    // Search the document
                    m_MyEditer.TargetStart = 0;
                    m_MyEditer.TargetEnd = m_MyEditer.TextLength;
                    m_MyEditer.SearchFlags = SearchFlags.None;
                    while (m_MyEditer.SearchInTarget(text) != -1)
                    {
                        // Mark the search results with the current indicator
                        m_MyEditer.IndicatorFillRange(m_MyEditer.TargetStart, m_MyEditer.TargetEnd - m_MyEditer.TargetStart);
                        // Search the remainder of the document
                        m_MyEditer.TargetStart = m_MyEditer.TargetEnd;
                        m_MyEditer.TargetEnd = m_MyEditer.TextLength;
                    }
                }
            }

            if (frm_ModuleObj.m_EProcedureList != null)
            {
                //赋值
                frm_ModuleObj.m_EProcedureList[CmbprocedureMethod.SelectedIndex].Body = m_MyEditer.Text;
            }
            else
            {
                System.Windows.Forms.MessageBox.Show("请先导入正确的图像脚本");
            }
        }

        //转换color
        public static System.Drawing.Color IntToColor(int rgb)
        {
            return System.Drawing.Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        #endregion

        /// <summary>
        /// 导入参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PutIn_Param_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog openFile = new System.Windows.Forms.OpenFileDialog();
            openFile.Title = "选择halcon文件";
            openFile.Filter = "halcon文件|*.hdev";
            openFile.FileName = string.Empty;
            openFile.FilterIndex = 1;
            openFile.Multiselect = false;

            if (openFile.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                frm_ModuleObj.m_EProcedureList = EProcedure.LoadXmlByFile(openFile.FileName);

                ShowProcedure();

                //重新绑定运行方法名称 由于是导入 这里默认选择第一个方法作为运行方法
                CmbRunProcedureMethod.ItemsSource = null;
                CmbRunProcedureMethod.ItemsSource = RunProcedureNameList;
                if (CmbRunProcedureMethod.Items.Count > 0)
                {
                    CmbRunProcedureMethod.SelectedIndex = 0;
                    CmbprocedureMethod_DropDownClosed(null, null);//如果之前就在第一个选项 不会触发更新,故手动更新一次 magical 2019-2-18 20:28:52
                }
            }
        }

        private void CmbprocedureMethod_DropDownClosed(object sender, EventArgs e)
        {
            if (CmbprocedureMethod.SelectedIndex == -1)
            {
                return;
            }
            m_MyEditer.Text = frm_ModuleObj.m_EProcedureList[CmbprocedureMethod.SelectedIndex].Body;
        }

        /// <summary>
        /// 导出参数
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void PutOut_Param_Click(object sender, RoutedEventArgs e)
        {
            if (frm_ModuleObj.m_EProcedureList != null && frm_ModuleObj.m_EProcedureList.Count > 0)
            {
                System.Windows.Forms.SaveFileDialog sfd = new System.Windows.Forms.SaveFileDialog();

                //设置文件类型 
                sfd.Filter = "halcon文件|*.hdev";

                //设置默认文件类型显示顺序 
                sfd.FilterIndex = 1;

                sfd.InitialDirectory = @"C:\Users\Administrator\Desktop\temp";

                //保存对话框是否记忆上次打开的目录 
                sfd.RestoreDirectory = true;

                //点了保存按钮进入 
                if (sfd.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    string localFilePath = sfd.FileName.ToString(); //获得文件路径 
                    EProcedure.SaveToFile(localFilePath, frm_ModuleObj.m_EProcedureList);
                }
            }
        }

        /// <summary>
        /// 展示Halcon的函数
        /// </summary>
        private void ShowProcedure()
        {
            if (frm_ModuleObj.m_EProcedureList == null)
                return;

            m_EProcedureNameList.Clear();

            List<string> eProcedureNameList = new List<string>();
            //设置名称
            foreach (EProcedure eProcedure in frm_ModuleObj.m_EProcedureList)
            {
                eProcedureNameList.Add(eProcedure.GetProcedureMethod());
                m_EProcedureNameList.Add(eProcedure.Name);
            }
            CmbprocedureMethod.ItemsSource = eProcedureNameList;

            if (eProcedureNameList.Count > 0)
            {
                CmbprocedureMethod.SelectedIndex = 0;
            }

            //根据外部函数名称 染色
            InitSyntaxColoring(m_MyEditer);

            Inputlink();
            Outputlink();
        }

        public void Inputlink()
        {
            frm_ModuleObj.m_Inputlink.Clear();

            //输入图像参数
            for (int i = 0; i < frm_ModuleObj.m_EProcedureList[1].IconicInputList.Count; i++)
            {
                frm_ModuleObj.m_Inputlink.Add(new Inputlink
                {
                    m_LinkName = frm_ModuleObj.m_EProcedureList[1].IconicInputList[i],
                    m_ImageScriptType = ImageScriptType.输入图像参数,
                    m_LinkData = "Null",
                });
            }

            //输入变量参数
            for (int i = 0; i < frm_ModuleObj.m_EProcedureList[1].CtrlInputList.Count; i++)
            {
                frm_ModuleObj.m_Inputlink.Add(new Inputlink
                {
                    m_LinkName = frm_ModuleObj.m_EProcedureList[1].CtrlInputList[i],
                    m_ImageScriptType = ImageScriptType.输入变量参数,
                    m_LinkData = "Null",
                });
            }

            dgv_LinkInput.ItemsSource = null;
            dgv_LinkInput.ItemsSource = frm_ModuleObj.m_Inputlink;
        }

        public void Outputlink()
        {
            frm_ModuleObj.m_OutputVar.Clear();
            //输出图像参数
            for (int i = 0; i < frm_ModuleObj.m_EProcedureList[1].IconicOutputList.Count; i++)
            {
                frm_ModuleObj.m_OutputVar.Add(new OutputVar
                {
                    m_LinkName = frm_ModuleObj.m_EProcedureList[1].IconicOutputList[i],
                    m_ImageScriptType = ImageScriptType.输出图像参数,
                });
            }

            //输出变量参数
            for (int i = 0; i < frm_ModuleObj.m_EProcedureList[1].CtrlOutputList.Count; i++)
            {
                frm_ModuleObj.m_OutputVar.Add(new OutputVar
                {
                    m_LinkName = frm_ModuleObj.m_EProcedureList[1].CtrlOutputList[i],
                    m_ImageScriptType = ImageScriptType.输出变量参数,
                });
            }
            dgv_LinkOutput.ItemsSource = null;
            dgv_LinkOutput.ItemsSource = frm_ModuleObj.m_OutputVar;
        }

        private void dgv_LinkInput_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (dgv_LinkInput.SelectedItem != null)
            {
                e.Handled = true;
                //Int32 row = this.dgv_DataVar.Items.IndexOf(this.dgv_DataVar.CurrentItem);
                Int32 Col = this.dgv_LinkInput.Columns.IndexOf(this.dgv_LinkInput.CurrentColumn);//获取列位置
                string name = dgv_LinkInput.Columns[Col].Header.ToString();
                if (name.Contains("变量链接"))
                {
                    ShowLinfkFrm(dgv_LinkInput.SelectedIndex);
                }
            }
        }

        private void ShowLinfkFrm(int index)
        {
            LinkVarFrm linkVar = new LinkVarFrm();

            LinkDataVarViewModel dataviewModel = new LinkDataVarViewModel("全局变量");

            linkVar.DataContext = dataviewModel;

            dataviewModel.sendMessage = Recevie;

            bool? conStatus = linkVar.ShowDialog();

            if (conStatus == true)
            {
                //链接写入

                frm_ModuleObj.m_Inputlink[index].m_Link_Var = m_DataVar;

                frm_ModuleObj.m_Inputlink[index].m_LinkData = m_DataVar.m_DataTip.ToString() + "." + m_DataVar.m_DataName;

                dgv_LinkInput.ItemsSource = null;
                dgv_LinkInput.ItemsSource = frm_ModuleObj.m_Inputlink;
            }

        }

        private DataVar m_DataVar;

        public void Recevie(DataVar value)
        {
            m_DataVar = value;
        }

        private void dgv_LinkOutput_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (dgv_LinkOutput.SelectedIndex > -1)
            {
                InitCmb();
                e.Handled = true;
            }
        }

        private void InitCmb()
        {
            List<string> list = new List<string>();
            foreach (string cameraInfo in Enum.GetNames(typeof(OutDataVarType)))
            {
                list.Add(cameraInfo);
            }
            Cmb_DataType.ItemsSource = list;
            Cmb_DataType.SelectedIndex = 0;
        }

        private void Cmb_DataType_DropDownClosed(object sender, EventArgs e)
        {
            if (frm_ModuleObj.m_OutputVar.Count > 0 && dgv_LinkOutput.SelectedIndex > -1)
            {
                frm_ModuleObj.m_OutputVar[dgv_LinkOutput.SelectedIndex].m_DataType = (OutDataVarType)Cmb_DataType.SelectedIndex;

                dgv_LinkOutput.ItemsSource = null;
                dgv_LinkOutput.ItemsSource = frm_ModuleObj.m_OutputVar;
            }
        }

        /// <summary>
        /// 执行一次模块
        /// </summary>
        public override void ExModule()
        {
            try
            {
                //删除本ID的变量
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == frm_ModuleObj.ModuleParam.ProjectID);
                if (proIndex > -1)
                {
                    SysProcessPro.g_ProjectList[proIndex].m_Var_List.RemoveAll(c => c.m_DataTip.Contains(frm_ModuleObj.ModuleParam.ModuleName));
                }

                frm_ModuleObj.ExeModule();

                dgv_LinkOutput.ItemsSource = null;
                dgv_LinkOutput.ItemsSource = frm_ModuleObj.m_OutputVar;

            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 保存参数
        /// </summary>
        public override void SaveModuleParam()
        {
            try
            {
                //执行算子名称
                frm_ModuleObj.RunMethod = CmbRunProcedureMethod.Text;

                ((ModuleObj)m_ModuleObjBase).blnNewModule = true;
                this.Close();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void SuperButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SysProcessPro.s_HDevEngine.SetEngineAttribute("execute_procedures_jit_compiled", "false");
                // Start debug server
                SysProcessPro.s_HDevEngine.StopDebugServer();
                SysProcessPro.s_HDevEngine.StartDebugServer();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void SuperButton_Click_1(object sender, RoutedEventArgs e)
        {
            try
            {
                SysProcessPro.s_HDevEngine.SetEngineAttribute("execute_procedures_jit_compiled", "true");
                SysProcessPro.s_HDevEngine.StopDebugServer();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void SuperButton_Click_2(object sender, RoutedEventArgs e)
        {
            try
            {
                string tempName = Environment.GetEnvironmentVariable("TEMP") + $"/temp.hdev";

                EProcedure.SaveToFile(tempName, frm_ModuleObj.m_EProcedureList);

                HDevProgram program = new HDevProgram(tempName);

                //加载的方法名称 halcon导出后和文件名称一致 使用哪个算子
                HDevProcedure procedure = new HDevProcedure(program, CmbRunProcedureMethod.Text);

                frm_ModuleObj.m_HDevProcedureCall = new HDevProcedureCall(procedure);

                System.IO.File.Delete(tempName);
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 窗体关闭
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Title_CloseWindow(object sender, RoutedEventArgs e)
        {
            CancelModuleParam();
        }

    }
}
