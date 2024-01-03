using Common;
using HalconDotNet;
using ModuleDataVar;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PublicDefine;
using StyleBase;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows.Forms;
using VisionCore;

namespace Plugin.VBScript
{
    /// <summary>
    /// 脚本基类,可以自己定义方法在内部使用
    /// </summary>
    public class ScriptMethods
    {
        public int ProjectID { get; set; } = 0;//脚本所在项目的id /执行run方法的时候 赋值
        public string ModuleName { get; set; } //脚本所在对应的模块名称

        public CommonConfig config = new CommonConfig();

        /// <summary>
        /// 显示一个内容
        /// </summary>
        /// <param name="str"></param>
        public void Show(string str)
        {
            MessageBox.Show(str);
        }

        /// <summary>
        /// 显示一个内容
        /// </summary>
        /// <param name="str"></param>
        public void LogInfo(string str)
        {
            Log.Info(str);
        }

        /// <summary>
        /// 根据格式获取系统时间
        /// </summary>
        /// <param name="format"></param>
        public string GenDateTime(string format)
        {
            return System.DateTime.Now.ToString(format);
        }

        /// <summary>
        /// 图像显示字符
        /// </summary>
        /// <param name="ProjectName">流程名称</param>
        /// <param name="ModuleName">模块名称</param>
        /// <param name="ModuleDescribe">描述文件</param>
        /// <param name="Txt">显示内容</param>
        /// <param name="color">颜色</param>
        public void ShowHwindowText(string ProjectName, string ModuleName, string ModuleDescribe, string Txt, string color)
        {
            //查询流程索引
            int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectName == ProjectName);
            if (proIndex > -1)
            {
                int index = -1;
                index = SysProcessPro.g_ProjectList[proIndex].m_ModuleObjList.FindIndex(c => c.ModuleParam.ModuleName == ModuleName);
                if (index > -1)
                {
                    ModuleParam par = SysProcessPro.g_ProjectList[proIndex].m_ModuleObjList[index].ModuleParam;
                    //ROI搜索范围的轮廓
                    ModuleROIText 结果 = new ModuleROIText(
                    par.ToString(),
                    par.ToString(),
                    ModuleDescribe,
                    ModuleRoiType.文字显示.ToString(),
                    color,
                    Txt,
                    24);

                    if (SysProcessPro.g_ProjectList[proIndex].m_ModuleObjList[index].m_Image != null)
                    {
                        SysProcessPro.g_ProjectList[proIndex].m_ModuleObjList[index].m_Image.UpdateTxtList(结果);
                    }
                    else
                    {
                        Log.Error("对应的Himage为Null！");
                    }
                }
            }
        }

        /// <summary>
        /// 根据名称获取全局变量的数据
        /// </summary>
        /// <param name="ValueName">变量的名称</param>
        /// <returns></returns>
        public object GetGlobalValue(string ValueName)
        {
            return OperDataVar.GetGlobalValue(ValueName);
        }

        /// <summary>
        /// 根据变量名称设置变量值
        /// </summary>
        /// <param name="ValueName"></param>
        /// <param name="obj"></param>
        public void SetGlobalValue(string ValueName, object obj)
        {
            bool Conver = false;
            int index = SysProcessPro.g_VarList.FindIndex(c => c.m_DataName == ValueName);
            if (index > -1)
            {
                //判断是否是该类型的变量
                switch (SysProcessPro.g_VarList[index].m_DataType)
                {
                    case DataVarType.DataType.Int:
                        Conver = obj is int;
                        break;
                    case DataVarType.DataType.Double:
                        if (double.TryParse(obj.ToString(), out double result))
                        {
                            //输入的是Double类型的
                            Conver = true;
                        }
                        else
                        {
                            //输入的不可以表示成Double类型
                            Conver = false;
                        }
                        //Conver = obj is double;
                        break;
                    case DataVarType.DataType.Float:
                        Conver = obj is float;
                        break;
                    case DataVarType.DataType.Bool:
                        Conver = obj is bool;
                        break;
                    case DataVarType.DataType.String:
                        Conver = obj is String;
                        break;
                    default:
                        break;
                }

                DataVar objStatus;
                if (SysProcessPro.SaveUpValue)
                {
                    //写入初始值
                    objStatus = new DataVar(SysProcessPro.g_VarList[index].m_DataAtrr, SysProcessPro.g_VarList[index].m_DataModuleID,
                      SysProcessPro.g_VarList[index].m_DataName, SysProcessPro.g_VarList[index].m_DataType, SysProcessPro.g_VarList[index].m_DataGroup,
                      SysProcessPro.g_VarList[index].m_Data_Num, obj.ToString(), SysProcessPro.g_VarList[index].m_DataTip, obj);
                }
                else
                {
                    //不写入初始值
                    objStatus = new DataVar(SysProcessPro.g_VarList[index].m_DataAtrr, SysProcessPro.g_VarList[index].m_DataModuleID,
                       SysProcessPro.g_VarList[index].m_DataName, SysProcessPro.g_VarList[index].m_DataType, SysProcessPro.g_VarList[index].m_DataGroup,
                       SysProcessPro.g_VarList[index].m_Data_Num, SysProcessPro.g_VarList[index].m_Data_InitValue, SysProcessPro.g_VarList[index].m_DataTip, obj);
                }

                if (Conver)
                {
                    VisionCore.OperDataVar.UpdateGlobalValue(ref SysProcessPro.g_VarList, objStatus);
                }
                else
                {
                    Log.Error("VB脚本模块，根据变量名称设置变量值,转换格式错误,请查询接收变量类型是否与输入变量类型一致！");
                }
            }
            else
            {
                Log.Error("未查询到对应的变量名！");
            }
        }

        /// <summary>
        /// 获取流程中，模块的值
        /// </summary>
        /// <param name="ProjectName">流程名称</param>
        /// <param name="ModuleName">模块名称</param>
        /// <param name="ModuleVar">值</param>
        public object GetModuleValue(string ProjectName, string ModuleName, string ModuleVar)
        {
            //查询索引
            int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectName == ProjectName);
            if (proIndex > -1)
            {
                int index = -1;

                index = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataTip == ModuleName && c.m_DataName == ModuleVar);

                if (index > -1)
                {
                    return SysProcessPro.g_ProjectList[proIndex].m_Var_List[index].m_DataValue;
                }
                else
                {
                    Log.Error("未查询到对应的模块！");
                    return 0;
                }
            }
            else
            {
                Log.Error("未查询到对应的流程！");
                return 0;
            }

        }

        /// <summary>
        /// 设置模块的值
        /// </summary>
        /// <param name="ProjectName"></param>
        /// <param name="ModuleName"></param>
        /// <param name="ModuleVar"></param>
        /// <param name="obj"></param>
        /// <returns></returns>
        public void SetModuleValue(string ProjectName, string ModuleName, string ModuleVar, object obj)
        {
            bool Conver = false;

            //查询索引
            int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectName == ProjectName);
            if (proIndex > -1)
            {
                int index = -1;

                index = SysProcessPro.g_ProjectList[proIndex].m_Var_List.FindIndex(c => c.m_DataTip == ModuleName && c.m_DataName == ModuleVar);

                if (index > -1)
                {
                    //判断是否是该类型的变量
                    switch (SysProcessPro.g_ProjectList[proIndex].m_Var_List[index].m_DataType)
                    {
                        case DataVarType.DataType.Int:
                            Conver = obj is int;
                            break;
                        case DataVarType.DataType.Double:
                            if (double.TryParse(obj.ToString(), out double result))
                            {
                                //输入的是Double类型的
                                Conver = true;
                            }
                            else
                            {
                                //输入的不可以表示成Double类型
                                Conver = false;
                            }
                            //Conver = obj is double;
                            break;
                        case DataVarType.DataType.Float:
                            Conver = obj is float;
                            break;
                        case DataVarType.DataType.Bool:
                            Conver = obj is bool;
                            break;
                        case DataVarType.DataType.String:
                            Conver = obj is String;
                            break;
                        default:
                            break;
                    }

                    DataVar objStatus = new DataVar(
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List[index].m_DataAtrr,
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List[index].m_DataModuleID,
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List[index].m_DataName,
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List[index].m_DataType,
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List[index].m_DataGroup,
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List[index].m_Data_Num,
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List[index].m_Data_InitValue,
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List[index].m_DataTip,
                        obj);

                    if (Conver)
                    {
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List[index] = objStatus;
                    }
                    else
                    {
                        Log.Error("VB脚本模块，设置模块的值,转换格式错误,请查询接收变量类型是否与输入变量类型一致！");
                    }
                }
                else
                {
                    Log.Error("未查询到对应的模块！");
                }
            }
            else
            {
                Log.Error("未查询到对应的流程！");
            }
        }

        /// <summary>
        /// 查询Json数据
        /// </summary>
        /// <param name="nam"></param>
        /// <param name="selectVale"></param>
        /// <returns></returns>
        //public string jsonread(string nam, string selectVale)
        //{

        //    string JsonData = "{\r\n\"command\":\"T1_1\",\r\n\"row_count\":2,\r\n\"col_count\":3,\r\n\"battery_color\":0,\r\n\"battery_shape\":0,\r\n\"battery_len1\":110.0,\r\n\"battery_len2\":0.0,\r\n\"battery_wid1\":80.0,\r\n\"battery_wid2\":0.0,\r\n\"battery_body_len\":0.0\r\n}";
        //    JavaScriptSerializer serializer = new JavaScriptSerializer();
        //    Dictionary<string, object> json = (Dictionary<string, object>)serializer.DeserializeObject(JsonData);
        //    string firstKey = json.ElementAt(0).Key;
        //    string secondKey = json.ElementAt(1).Key;

        //    return json[selectVale].ToString();

        //}

        /// <summary>
        /// 查询Json数据
        /// </summary>
        /// <param name="InputStr">输入字符</param>
        /// <param name="selectVale">键值</param>
        /// <returns></returns>
        public string JsonRead(string InputStr, string selectVale)
        {
            //将json转换为JObject
            JObject jObj = JObject.Parse(InputStr);
            JToken ageToken = jObj[selectVale];

            string str = ageToken.ToString();

            return str;
        }

        /// <summary>
        /// 查询Json数据
        /// </summary>
        /// <param name="InputStr">输入字符</param>
        /// <param name="selectVale">键值</param>
        /// <returns></returns>
        public string JsonRead(string InputStr, string selectVale, string subValue)
        {
            //将json转换为JObject
            JObject jObj = JObject.Parse(InputStr);
            JToken ageToken = jObj[selectVale];
            string str = ageToken[subValue].ToString();
            return str;
        }

        public string JsonWriteArray(Object Input)
        {
            // string[] obj = (string[])Input;

            List<string> obj1 = (List<string>)Input;

            JObject staff = new JObject()
            {
              new JProperty("command", "T1_1"),
              new JProperty("run_result", 1),
            };

            List<JObject> InoutPbj = new List<JObject>();

            for (int i = 0; i < obj1.Count; i++)
            {
                InoutPbj.Add(new JObject
                {
                    new JProperty("battery_index", i),
                    new JProperty("result",Convert.ToInt32( obj1[i]))
                    //new JProperty("result",Convert.ToInt32( obj1[obj1.Count-i-1]))
                });
            }

            staff.Add(new JProperty("results", InoutPbj.ToArray()));

            string json = JsonConvert.SerializeObject(staff);
            Console.WriteLine(json.ToString());
            return json;
        }

        public bool Intersection(string ProjectName1,string ModelName1, string ProjectName2, string ModelName2, int Index)
        {
            try
            {
                HRegion hRegion = (HRegion)GetModuleValue(ProjectName1, ModelName1, "区域");
                HRegion hRegion1 = (HRegion)GetModuleValue(ProjectName2, ModelName2, "区域");

//                int Index = (int)GetModuleValue("测试", "循环开始0", "Int");

                hRegion = hRegion.SelectObj(Index + 2);
                //hRegion1 = hRegion1.SelectObj(Index+2);

                HRegion outRegion = hRegion.Intersection(hRegion1);

                int AreaNum = outRegion.AreaCenter(out double row, out double col);

                return AreaNum > 0;
            }
            catch (Exception)
            {
                return false;
            }

        }

    }
}
