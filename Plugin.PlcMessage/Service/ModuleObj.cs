using Common;
using CommunaCation;
using CommunaCationPLC;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.PlcMessage
{
    [Category("通讯测试")]
    [DisplayName("PLC通讯")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {
        ////通讯类型
        //public CommunaBase m_Communa;

        //通讯类型
        public CommunaCationPLC.CommunaType m_CommunaType { get; set; }

        //解码格式
        public CommunaCationPLC.DataFormat m_Format { get; set; }

        public string Communa_Port { get; set; }

        public int SlaveAddress { get; set; } = 0;

        //Int
        public CommunaCationPLC.VarType m_IntType { get; set; }

        //Float
        public CommunaCationPLC.VarType m_FloatType { get; set; }

        /// <summary>
        /// 执行模块
        /// </summary>
        /// <param name="blnByHand"></param>
        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            try
            {

                m_Communa.Connect();

                m_Communa.format = m_Format;//解码格式

                m_Communa.SlaveAddress = Convert.ToInt32(SlaveAddress);//从站地址

                m_Communa.Int_Type = m_IntType;//Int变量类型

                m_Communa.Float_Type = m_FloatType;//Int变量类型

                ModuleParam.BlnSuccessed = m_Communa.IsConnected;//通讯是否连接成功

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
