using Microsoft.VisualBasic;
using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Windows;
using System.Windows.Forms;

namespace Plugin.VBScript
{
    public class ScriptSupport
    {
        const int DIFNUM = 11;//脚本前隐藏了多少行代码,用于提示出错行
        public int ProjectID = 0;//脚本所在项目的id
        private Assembly _objAssembly = null;
        private object _objectClass = null;
        private VBCodeProvider objCSharpCodePrivoder = new VBCodeProvider();
        private string m_Source = "";//编译源代码
        private string m_ErrorText = "";//错误信息
        private bool m_Compiled = false;//是否已经编译正确

        public ScriptSupport(string _soutce)
        {
            
            m_Source = _soutce;

            if (!String.IsNullOrEmpty(m_Source))
            {
                Compile();
            }
        }

        public string ErrorText
        {
            get { return m_ErrorText; }
            set { m_ErrorText = value; }
        }
        public string Source
        {
            get { return m_Source; }
            set { m_Source = value; }
        }
        //引用的dll
        private ArrayList Refrences =
                new ArrayList {
                            "System.dll",
                            "System.Core.dll",
                            "System.Windows.Forms.dll",
                            "mscorlib.dll",
                            "System.Data.dll",
                            "System.Drawing.dll",
                            "Microsoft.VisualBasic.dll"
                };

        //脚本的引用
        private ArrayList Imports=
                        new ArrayList {
                        "System.Windows.Forms"
                        };

        //返回引入的个数,方便统计错误代码出现在哪一行
        public int GetImportsCount()
        {
            return Imports.Count;
        }

        //脚本里需要用到那个类 就在这里注册 则该类同一个命名空间下的所有类都能使用 
        public ScriptSupport()
        {
            AddReferenceAssemblyByType(this.GetType());
            AddReferenceAssemblyByType(typeof(List<Object>));
            AddReferenceAssemblyByType(typeof(Math));
            AddReferenceAssemblyByType(typeof(Thread));
            AddReferenceAssemblyByType(typeof(DateAndTime));//主要是为了 加入命名空间Microsoft.VisualBasic
            AddReferenceAssemblyByType(typeof(AutoResetEvent));
            AddReferenceAssemblyByType(typeof(ScriptMethods));//脚本是继承ScriptMethods

        }

        /// <summary>
        /// 运行脚本
        /// </summary>
        /// <returns></returns>
        public bool CodeRun()
        {
            try
            {
                if (m_Compiled == false) Compile();

                if (m_Compiled)
                {
                    bool flag = (bool)this._objectClass.GetType().InvokeMember("Process", BindingFlags.InvokeMethod, null, this._objectClass, null);
                    return flag;
                }
            }
            catch (Exception /*e*/)
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// 编译代码
        /// </summary>
        /// <returns></returns>
        public bool Compile()
        {

            if (String.IsNullOrEmpty(Source))
            {
                return false;
            } 
         
            Exception exception;
            this._objAssembly = null;
            this._objectClass = null;
            CompilerParameters options = new CompilerParameters
            {
                GenerateExecutable = false,
                GenerateInMemory = true,
                IncludeDebugInformation = false,
                WarningLevel = 4
            };

            foreach (String str in Refrences)
            {
                options.ReferencedAssemblies.Add(str);
            }


            CompilerResults results = null;
            StringBuilder sb = new StringBuilder();
            try
            {

                foreach (string import in this.Imports)
                {
                    sb.Append("\r\nImports " + import);
                }
                sb.Append(Source);
                results = this.objCSharpCodePrivoder.CompileAssemblyFromSource(options, new string[] { sb.ToString() });
             
            }
            catch (Exception exception1)
            {
                exception = exception1;
                m_ErrorText = "编译错误：可能未正确添加引用集";
                return false;
            }
            m_Compiled = true;
            int errorNum = 0;
            StringBuilder sb1 = new StringBuilder();
            for (int i = 0; i < results.Errors.Count; i++)
            {
                if (!results.Errors[i].IsWarning)
                {
                    m_Compiled = false;

                    errorNum++;
                    sb1.Append($"第{results.Errors[i].Line - GetImportsCount()- DIFNUM }行:  {results.Errors[i].ErrorText}");
                    sb1.AppendLine();
                }
            }
            if (errorNum>0)
            {
                m_ErrorText = $"检测出 {errorNum} 个错误:\r\n" + sb1.ToString();
                //Log.Warn(m_ErrorText);
             //   MessageBox.Show(m_ErrorText);
            }
            else
            {
                m_ErrorText = "";
            }
       
            if (m_Compiled)
            {
                this._objAssembly = results.CompiledAssembly;
                this._objectClass = this._objAssembly.CreateInstance("MyScript");
                if (this._objectClass == null)
                {
                    //Log.Warn("创建脚本失败");
                   // MessageBox.Show("创建脚本失败");
                    return false;
                }
                try
                {
                    //在编译的时候执行一个初始方法,暂未使用
                    // this._objectClass.GetType().InvokeMember("Init", BindingFlags.InvokeMethod, null, this._objectClass, null);
                }
                catch (Exception /*e*/)
                {
                    return false;
                }
            }
             
            return m_Compiled;
        }


        /// <summary>
        /// 添加指定的类型所在的引用
        /// </summary>
        /// <param name="SourceType">对象类型</param>
        private void AddReferenceAssemblyByType(Type SourceType)
        {
        
            if (SourceType == null)
            {
                throw new ArgumentNullException("SourceType为空");
            }
            System.Uri uri = new Uri(SourceType.Assembly.CodeBase);
            string path = null;
            if (uri.Scheme == System.Uri.UriSchemeFile)
            {
                path = uri.LocalPath;//C:\Users\hasee\Desktop\ev\EasyVision3\000_exe\Debug\EasyVision.Common.DLL
            }
            else
            {
                path = uri.AbsoluteUri;
            }

            if (this.Refrences.Contains(path) == false)
            {
                this.Refrences.Add(path);
            }
            string str = SourceType.Namespace;//命名空间
            if (this.Imports.Contains(str) == false)
            {
                this.Imports.Add(str);
            }
  
        }
    }
}

