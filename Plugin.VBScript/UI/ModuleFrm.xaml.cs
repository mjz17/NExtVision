using Common;
using ScintillaNET;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Xml.Linq;
using VisionCore;
using static ScintillaNET.Style;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

namespace Plugin.VBScript
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        private ModuleObj frm_ModuleObj;

        private string m_AutoStr;//自动提示的关键字
        private Dictionary<string, ScriptMethodInfo> methodDic = new Dictionary<string, ScriptMethodInfo>();//脚本方法字典,
        public ScriptSupport m_TempScriptSupport = new ScriptSupport();

        public ModuleFrm()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 16;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            this.Loaded += ModuleFrm_Loaded;
        }

        private void ModuleFrm_Loaded(object sender, RoutedEventArgs e)
        {
            //加载参数
            frm_ModuleObj = (ModuleObj)m_ModuleObjBase;
            //窗体名称
            Title.HeadName = frm_ModuleObj.ModuleParam.ModuleName;
            //模块当前ID
            CurrentModelID = frm_ModuleObj.ModuleParam.ModuleName;

            m_MyEditer.Text = frm_ModuleObj.InCode;

            //初始化控件
            InitScintilla();

            InitTxt();
        }

        private void InitTxt()
        {
            string str = "\"";//单引号
            string str1 = "\"," + str;// ",

            #region VB基本语法

            //数据声明
            RichtxtboxInput(System.Windows.Media.Brushes.Green, "VB基本语法");

            //数据声明
            RichtxtboxInput(System.Windows.Media.Brushes.Red, "1、数据声明");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "Dim int As Integer = 0");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "Dim str As String = \"\"");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "Dim flag As Boolean = False");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "Dim dbl As Double = 0.0");

            //数组声明
            RichtxtboxInput(System.Windows.Media.Brushes.Red, "2、数组声明");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "Dim arrayl(3) As Integer '3是个数");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "Dim array2 As Integer() = {2， 4， 8}");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "Dim list As New List(Of String)");

            //判断
            RichtxtboxInput(System.Windows.Media.Brushes.Red, "3、判断");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "If True Then\r\n'do something\r\nEnd If");

            //循环
            RichtxtboxInput(System.Windows.Media.Brushes.Red, "4、循环");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "For index As Integer = 1 To 10\r\n'do something\r\nNext");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "While True\r\n'do something\r\nEnd While");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "For Each i As Integer In arrayl\r\n\r\nNext");

            //分割字符
            RichtxtboxInput(System.Windows.Media.Brushes.Red, "5、分割字符");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "Dim arr As String() = \"sa;ba;ca\".Split(\";\")");

            #endregion

            #region 其他函数

            //数据声明
            RichtxtboxInput(System.Windows.Media.Brushes.Green, "其他函数");

            //打印日志
            RichtxtboxInput(System.Windows.Media.Brushes.Red, "1、打印日志");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "MessageBox(string)");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "Show(" + str + "string" + str + ")");

            RichtxtboxInput(System.Windows.Media.Brushes.Red, "2、打印Log日志");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "LogInfo(" + str + "打印内容" + str + ")");

            RichtxtboxInput(System.Windows.Media.Brushes.Red, "3、获取系统时间");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "GenDateTime(" + str + "格式" + str + ")");

            RichtxtboxInput(System.Windows.Media.Brushes.Red, "4、图像显示内容");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "ShowHwindowText(" + str + "流程名称" + str1 + "模块名称" + str1 + "描述" + str1 + "显示内容" + str1 + "颜色" + str + ")");

            RichtxtboxInput(System.Windows.Media.Brushes.Red, "5、根据变量名称,获取全局变量的数据");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "GetGlobalValue(" + str + "全局变量名称" + str + ")");

            RichtxtboxInput(System.Windows.Media.Brushes.Red, "6、根据变量名称,设置变量值");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "SetGlobalValue(" + str + "全局变量名称" + str1 + "输入值" + str + ")");

            RichtxtboxInput(System.Windows.Media.Brushes.Red, "7、根据流程名称,获取模块的数据");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "GetModuleValue(" + str + "流程名称" + str1 + "模块名称" + str1 + "变量名称" + str + ")");
            
            RichtxtboxInput(System.Windows.Media.Brushes.Red, "8、根据流程名称,修改模块的数据");
            RichtxtboxInput(System.Windows.Media.Brushes.Blue, "SetModuleValue(" + str + "流程名称" + str1 + "模块名称" + str1 + "变量名称" + str1 + "值" + str + ")");

            #endregion

        }

        FlowDocument Doc = new FlowDocument();

        private void RichtxtboxInput(System.Windows.Media.Brush brush, string txt)
        {
            Run item = new Run(txt);
            Paragraph paragraph = new Paragraph();
            paragraph.Inlines.Add(item);
            paragraph.LineHeight = 1.0;
            paragraph.Foreground = brush;
            this.Txt_Tip.Document.Blocks.Add(paragraph);

        }

        #region sci控件

        /// <summary>
        /// 初始化染色控件
        /// </summary>
        private void InitScintilla()
        {
            // 字体包裹模式
            m_MyEditer.WrapMode = WrapMode.None;

            //高亮显示
            InitSyntaxColoring(m_MyEditer);

            //自定义关键字代码提示功能
            m_MyEditer.AutoCIgnoreCase = true;//代码提示的时候,不区分大小写
            AutoComplete();

            // 这两种操作会导致乱码
            m_MyEditer.ClearCmdKey(Keys.Control | Keys.S);
            m_MyEditer.ClearCmdKey(Keys.Control | Keys.F);

            m_MyEditer.AutoCCompleted += m_MyEditer_AutoCCompleted;
        }

        //自动补全时出现
        private void m_MyEditer_AutoCCompleted(object sender, AutoCSelectionEventArgs e)
        {
            string tip = ShowTipByWord();
            //tip提示
            if (!string.IsNullOrWhiteSpace(tip))
            {
                m_MyEditer.CallTipShow(m_MyEditer.CurrentPosition, tip);
            }
        }

        private void InitTipSyntaxColoring(Scintilla scintilla)
        {
            // 设置默认格式
            scintilla.StyleResetDefault();
            scintilla.Styles[ScintillaNET.Style.Default].Font = "Consolas";
            scintilla.Styles[ScintillaNET.Style.Default].Size = 9;
            //scintilla.Styles[ScintillaNET.Style.Default].BackColor = System.Drawing.SystemColors.Control;
            //背景色
            scintilla.Styles[ScintillaNET.Style.Default].ForeColor = System.Drawing.Color.Black;
            scintilla.Styles[ScintillaNET.Style.Default].BackColor = System.Drawing.Color.Pink;
            scintilla.ScrollWidth = 100;//设置水平滚动条为100 这样水平就不会默认显示滚动条

            //普通代码的颜色
            scintilla.Styles[ScintillaNET.Style.Vb.Comment].ForeColor = ColorTranslator.FromHtml("#008000");
            scintilla.Styles[ScintillaNET.Style.Vb.Number].ForeColor = ColorTranslator.FromHtml("#FF6532");
            scintilla.Styles[ScintillaNET.Style.Vb.String].ForeColor = ColorTranslator.FromHtml("#A31515");
            scintilla.Styles[ScintillaNET.Style.Vb.Preprocessor].ForeColor = IntToColor(0x8AAFEE);
            //操作符
            scintilla.Styles[ScintillaNET.Style.Vb.Operator].ForeColor = ColorTranslator.FromHtml("#A31515");

            scintilla.Styles[ScintillaNET.Style.Vb.Keyword].ForeColor = ColorTranslator.FromHtml("#0000FF");
            scintilla.Styles[ScintillaNET.Style.Vb.Keyword2].ForeColor = ColorTranslator.FromHtml("#5CBAC7");
            scintilla.Styles[ScintillaNET.Style.Vb.Keyword3].ForeColor = System.Drawing.Color.Red;// ColorTranslator.FromHtml("#0000FF");
            scintilla.Styles[ScintillaNET.Style.Vb.Keyword4].ForeColor = ColorTranslator.FromHtml("#0000FF");

            //每个关键字的都有自己单独的背景色 ,没有找到统一设置背景色的方法!!!!故如此使用

            foreach (ScintillaNET.Style item in scintilla.Styles)
            {
                if (item.BackColor == System.Drawing.Color.White)
                {
                    item.BackColor = System.Drawing.SystemColors.Control;
                }
            }

            scintilla.Lexer = Lexer.Vb;

            // 可以设置两种颜色的关键字 输入只支持小写
            //对应ScintillaNET.Style.Vb.Keyword
            scintilla.SetKeywords(0, ScriptTemplate.VBString().ToLower());
            // //对应ScintillaNET.Style.Vb.Keyword2
            string s2 = GetMethodsString();
            scintilla.SetKeywords(1, s2.ToLower());

            //行号字体颜色
            scintilla.Styles[ScintillaNET.Style.LineNumber].ForeColor = ColorTranslator.FromHtml("#8DA3C1");

            //行号相关设置
            var nums = scintilla.Margins[1];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

        }

        //设置语法高亮规则
        private void InitSyntaxColoring(Scintilla scintilla)
        {
            // 设置默认格式
            scintilla.StyleResetDefault();
            scintilla.Styles[ScintillaNET.Style.Default].Font = "Consolas";
            scintilla.Styles[ScintillaNET.Style.Default].Size = 10;

            //前景色
            scintilla.Styles[ScintillaNET.Style.Default].ForeColor = System.Drawing.Color.Black;
            //背景色
            //scintilla.Styles[ScintillaNET.Style.Default].BackColor = System.Drawing.Color.Black;

            scintilla.StyleClearAll();

            scintilla.ScrollWidth = 100;//设置水平滚动条为100 这样水平就不会默认显示滚动条

            //普通代码的颜色
            scintilla.Styles[ScintillaNET.Style.Vb.Comment].ForeColor = ColorTranslator.FromHtml("#008000");
            scintilla.Styles[ScintillaNET.Style.Vb.Number].ForeColor = ColorTranslator.FromHtml("#FF6532");
            scintilla.Styles[ScintillaNET.Style.Vb.String].ForeColor = ColorTranslator.FromHtml("#A31515");
            scintilla.Styles[ScintillaNET.Style.Vb.Preprocessor].ForeColor = IntToColor(0x8AAFEE);

            //操作符
            scintilla.Styles[ScintillaNET.Style.Vb.Operator].ForeColor = ColorTranslator.FromHtml("#A31515");

            scintilla.Styles[ScintillaNET.Style.Vb.Keyword].ForeColor = ColorTranslator.FromHtml("#0000FF");
            scintilla.Styles[ScintillaNET.Style.Vb.Keyword2].ForeColor = ColorTranslator.FromHtml("#5CBAC7");
            scintilla.Styles[ScintillaNET.Style.Vb.Keyword3].ForeColor = System.Drawing.Color.Red;// ColorTranslator.FromHtml("#0000FF");
            scintilla.Styles[ScintillaNET.Style.Vb.Keyword4].ForeColor = ColorTranslator.FromHtml("#0000FF");
            scintilla.Lexer = Lexer.Vb;

            // 可以设置两种颜色的关键字 输入只支持小写
            //对应ScintillaNET.Style.Vb.Keyword
            scintilla.SetKeywords(0, ScriptTemplate.VBString().ToLower());
            // //对应ScintillaNET.Style.Vb.Keyword2
            string s2 = GetMethodsString();
            scintilla.SetKeywords(1, s2.ToLower());

            //行号字体颜色
            scintilla.Styles[ScintillaNET.Style.LineNumber].ForeColor = ColorTranslator.FromHtml("#8DA3C1");

            //行号相关设置
            var nums = scintilla.Margins[1];
            nums.Width = 30;
            nums.Type = MarginType.Number;
            nums.Sensitive = true;
            nums.Mask = 0;

            // TextArea.MarginClick += TextArea_MarginClick;
        }

        //代码提示功能
        public void AutoComplete()
        {
            //绑定输入事件
            m_MyEditer.CharAdded += Scintilla_CharAdded;
            string s = GetMethodsString();
            string str = ScriptTemplate.VBString() + " " + s;
            //获取

            //分割字符串成list
            List<string> autoStrList = str.Split(new char[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries).ToList();

            //对list排序重新转换为string
            m_AutoStr = string.Join(" ", autoStrList.OrderBy(x => x.ToUpper()));
        }

        //输入结束事件
        private void Scintilla_CharAdded(object sender, CharAddedEventArgs e)
        {
            // Find the word start
            var currentPos = m_MyEditer.CurrentPosition;
            var wordStartPos = m_MyEditer.WordStartPosition(currentPos, true);

            // Display the autocompletion list
            var lenEntered = currentPos - wordStartPos;
            if (lenEntered > 0)
            {
                if (!m_MyEditer.AutoCActive)
                {
                    //此处必须是按照字母排序才能显示出来
                    m_MyEditer.AutoCShow(lenEntered, m_AutoStr);
                }
            }

            //  ShowTipByWord();
        }

        //转换color
        public static System.Drawing.Color IntToColor(int rgb)
        {
            return System.Drawing.Color.FromArgb(255, (byte)(rgb >> 16), (byte)(rgb >> 8), (byte)rgb);
        }

        /// <summary>
        /// 根据word 在tip窗口显示提示
        /// </summary>
        private string ShowTipByWord()
        {
            // Find the word start
            var currentPos = m_MyEditer.CurrentPosition;
            var wordStartPos = m_MyEditer.WordStartPosition(currentPos, true);
            var wordEndPos = m_MyEditer.WordEndPosition(currentPos, true);
            string word = m_MyEditer.GetTextRange(wordStartPos, wordEndPos - wordStartPos);
            string tip = "";
            if (!string.IsNullOrWhiteSpace(word))
            {
                if (methodDic.ContainsKey(word))
                {
                    tip = methodDic[word].Description;
                    tb_Compile.Text = tip;
                }
            }

            return tip;//返回提示信息
        }

        //获取当前程序集指定方法
        public string GetMethodsString()
        {
            List<string> strList = new List<string>();

            List<Type> typeList = new List<Type>();
            typeList.Add(typeof(Object));
            typeList.Add(typeof(Math));
            typeList.Add(typeof(string));
            typeList.Add(typeof(System.Collections.Generic.List<double>));
            typeList.Add(typeof(Enumerable));
            typeList.Add(typeof(System.Windows.MessageBox));

            //
            typeList.Add(typeof(ScriptMethods));
            foreach (Type item in typeList)
            {
                if (item.IsEnum == true)
                {
                    string[] rolearry = Enum.GetNames(item);
                    strList.AddRange(rolearry);
                    strList.Add(item.Name);
                }
                else
                {
                    //添加type的方法
                    MethodInfo[] methods = item.GetMethods();
                    foreach (MethodInfo m in methods)
                    {
                        strList.Add(m.ToString().Split(' ')[1].Split('(')[0].Replace("set_", "").Replace("get_", "").Split('[')[0]);
                    }
                    strList.Add(item.Name);
                }
            }
            //自定义提示
            strList.Add("List(OfInteger)");
            strList.Add("List(OfDouble)");
            strList.Add("List(OfBoolean)");
            strList.Add("List(OfString)");

            return string.Join(" ", strList.Distinct().ToList().OrderBy(x => x.ToUpper())); ;
        }

        #endregion

        /// <summary>
        /// 编译
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Compile_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Compile();
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 执行模块
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


                Compile();
                m_TempScriptSupport.CodeRun();

                //frm_ModuleObj.InCode = m_MyEditer.Text;
                //frm_ModuleObj.ScriptSupport = m_TempScriptSupport;
                //frm_ModuleObj.ExeModule();

            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        /// <summary>
        /// 保存参数
        /// </summary>
        public override void SaveModuleParam()
        {
            base.SaveModuleParam();
            frm_ModuleObj.InCode = m_MyEditer.Text;
            frm_ModuleObj.ScriptSupport = m_TempScriptSupport;
            this.Close();
        }

        /// <summary>
        /// 编译
        /// </summary>
        public void Compile()
        {
            m_TempScriptSupport.Source = ScriptTemplate.GetScriptCode(frm_ModuleObj.ModuleParam.ProjectID, frm_ModuleObj.ModuleParam.ModuleName, m_MyEditer.Text);
            if (m_TempScriptSupport.Compile())
            {
                tb_Compile.Text = "编译成功";
            }
            else
            {
                tb_Compile.TextWrapping = TextWrapping.Wrap;
                tb_Compile.Text = m_TempScriptSupport.ErrorText;
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
