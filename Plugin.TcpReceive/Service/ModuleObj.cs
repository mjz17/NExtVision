using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using Common;
using CommunaCation;
using ModuleDataVar;
using System.Windows.Forms;

namespace Plugin.TcpReceive
{
    [Category("通讯测试")]
    [DisplayName("文本接收")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {
        /// <summary>
        /// 通讯名称
        /// </summary>
        public string m_ComunCation = string.Empty;

        /// <summary>
        /// 备注
        /// </summary>
        public string m_Remarks=string.Empty;

        public override void Stop()
        {
            EComManageer.StopRecStrSignal(m_ComunCation);
        }

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
                if (m_ComunCation != string.Empty)
                {
                    //会阻塞等待  可以通过调用    EComManageer.StopRecStrSignal(m_CurKey); 停止
                    EComManageer.GetEcomRecStr(m_ComunCation, out string recStr);

                    DataVar Out_str = new DataVar(DataVarType.DataAtrribution.局部变量, ModuleParam.ModuleID, ConstantVar.String.ToString(),
                        DataVarType.DataType.String, DataVarType.DataGroup.单量, 1, "0", ModuleParam.ModuleName, recStr);
                    ModuleProject.UpdateLocalVarValue(Out_str);

                    //运行成功
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
