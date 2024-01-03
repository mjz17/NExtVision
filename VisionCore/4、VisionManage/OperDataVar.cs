using ModuleDataVar;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common;

namespace VisionCore
{
    public class OperDataVar
    {

        public static object obj = new object();

        /// <summary>
        /// 修改全局变量中的数据
        /// </summary>
        /// <param name="inVariableList"></param>
        /// <param name="data"></param>
        public static void UpdateGlobalValue(ref List<DataVar> inVariableList, DataVar data)
        {
            lock (obj)
            {
                int index = inVariableList.FindIndex(c => c.m_DataName == data.m_DataName);
                if (index > -1)
                {
                    inVariableList[index] = data;
                }
                else
                {
                    inVariableList.Add(data);
                }
            }
        }

        /// <summary>
        /// 根据名称获取全局变量
        /// </summary>
        /// <param name="ValueName">名称</param>
        /// <returns></returns>
        public static object GetGlobalValue(string ValueName)
        {
            lock (obj)
            {
                int index = SysProcessPro.g_VarList.FindIndex(c => c.m_DataName == ValueName);
                if (index > -1)
                {
                    return SysProcessPro.g_VarList[index].m_DataValue;
                }
                Log.Error("未查询到对应的变量名！");
                return null;
            }
        }

        /// <summary>
        /// 根据名称获取全局变量
        /// </summary>
        /// <param name="ValueName">名称</param>
        /// <returns></returns>
        public static DataVar GetGlobalValueBackData(string ValueName)
        {
            lock (obj)
            {
                int index = SysProcessPro.g_VarList.FindIndex(c => c.m_DataName == ValueName);
                if (index > -1)
                {
                    return SysProcessPro.g_VarList[index];
                }
                Log.Error("未查询到对应的变量名！");
                return new DataVar();
            }
        }

    }
}
