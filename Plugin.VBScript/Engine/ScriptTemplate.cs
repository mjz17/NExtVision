using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Plugin.VBScript
{

    //脚本内的方法信息
    class ScriptMethodInfo
    {
        public string Name;
        public string Description;
        public string Category;
        public string DisplyName;// 格式"string key, int value, bool addFlag = false"
    }
    public class ScriptTemplate
    {
        //原始代码 不要做任何修改,包括一个空格和空白行!!!! magical  增加一行 要去ScriptSupport.DIFNUM 要加1
        public static  string s_RawScript = @"

            '上面必须空一行给命名空间预留
            Public Class MyScript
                Inherits ScriptMethods'继承
                Public Function Process() As Boolean
                Try
                    Dim resultFag As Boolean = True
                  
            $

                '代码自动插入这里

            $

                    Return resultFag
                Catch ex As Exception
                   ' LogError(ex.ToString()) 大家自己可以打印日志
                    Return false
                End Try
                End Function

            End Class";


        /// <summary>
        /// 拼装完整的脚本
        /// </summary>
        /// <param name="projectID"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        public static string GetScriptCode(int projectID,string moduleName,string code)
        {
            String[] codeArr = s_RawScript.Split('$');
            StringBuilder str = new StringBuilder();
            str.Append(codeArr[0]);
            str.Append($"ProjectID = {projectID}\r\n");
            str.Append($"ModuleName = \"{moduleName}\"\r\n");
            str.Append(code);
            str.Append(codeArr[2]);

            return str.ToString();
        }

        /// <summary>
        /// vb系统关键字
        /// </summary>
        /// 
        public static string VBString()
        {
            string str1 = " DataSelect.All DataSelect.SkipMax_m_Take_n DataSelect.SkipMin_m_Take_n DataSelect.SkipMin_m_SkipMax_n DataModel.Max DataModel.Avg DataModel.Min   ";
            string str2 = " RemoveHandler AddHandler ByRef CByte Class CSng Decimal Double Erase Friend  Handles Interface Loop MyClass Not Inheritable Optional Partial ReDim  Set Stop To Widening AddressOf Byte CChar CLng CStr Declare Each Error Function  If Is Me Namespace Not Overridable Or Private REM  Shadows String TRUE With Alias ByVal CDate CObj CType Default Else Event Get  Implements IsNot Mod Narrowing Object  OrElse Property Remove Handler Shared Structure Try WithEvents And Call CDec Const CUInt Delegate ElseIf Exit GetType  Imports Let Module New Of  Overloads Protected Resume  Short Sub TryCast WriteOnly AndAlso Case CDbl Continue CULng Dim End FALSE GetXML Namespace In Lib MustInherit Next On  Overridable Public Return  Single SyncLock TypeOf Xor As Catch Char CSByte CUShort DirectCast End If Finally Global  Inherits Like MustOverride Not Operator  Overrides RaiseEvent SByte  Static Then UInteger  Boolean CBool CInt CShort Date Do Enum For GoTo  Integer Long MyBase Nothing Option  ParamArray ReadOnly Select  Step Throw While ";

            return str1 + str2;
        }


    }
}
