using Common;
using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using VisionCore;

namespace Plugin.VBScript
{
    [Category("逻辑工具")]
    [DisplayName("VB脚本")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {
        //标记为不能序列化 
        [NonSerialized]
        public ScriptSupport ScriptSupport;

        public string InCode = "";

        protected override void AfterSetModuleParam()
        {
            //因为是先反射创建后,再赋值ModuleParam,所以在这里才能拿到ProjectID,ModuleName
            Task.Run(() =>
            {
                ScriptSupport.Source = ScriptTemplate.GetScriptCode(ModuleParam.ProjectID, ModuleParam.ModuleName, InCode);
                ScriptSupport.Compile();
            });
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
                if (ScriptSupport != null)
                {
                    ScriptSupport.CodeRun();
                    ModuleParam.BlnSuccessed = true;//运行成功
                }
                else
                {
                    ModuleParam.BlnSuccessed = false;//运行失败
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

        [OnDeserialized()]
        internal void OnDeSeriaLizingMethod(StreamingContext context)
        {
            ScriptSupport = new ScriptSupport();
            AfterSetModuleParam();
        }

    }
}
