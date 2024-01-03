using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VisionCore;
using Common;
using ModuleDataVar;

namespace Plugin.LapseTime
{

    [Category("逻辑工具")]
    [DisplayName("时间")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {

        [NonSerialized]
        public AutoResetEvent eventWait = new AutoResetEvent(false);//采集信号
        [NonSerialized]
        CancellationTokenSource cts;

        /// <summary>
        /// 延时时间
        /// </summary>
        private int delayTime;

        public int DelayTime
        {
            get
            {
                if (delayTime < 1)
                {
                    delayTime = 1;
                }
                return delayTime;
            }

            set
            {
                delayTime = value;
            }
        }

        /// <summary>
        /// 停止当前等待
        /// </summary>
        public override void Stop()
        {
            eventWait.Set();
            if (cts != null)
            {
                cts.Cancel(); //取消任务
            }
        }

        private bool WaitTimeTask(int delayTime, CancellationToken token)
        {
            //延时使用循环延时  否则取消时候回出现滞后响应
            if (delayTime < 1)
            {
                eventWait.Set();
                return true;
            }
            for (int i = 0; i < delayTime; i++)
            {
                Thread.Sleep(1);
                if (token.IsCancellationRequested)
                {
                    eventWait.Set();
                    return false;
                }
            }
            //Thread.Sleep(delayTime);
            if (token.IsCancellationRequested == false)
            {
                eventWait.Set();
                return true;
            }
            return false;
        }

        public override void ExeModule(bool blnByHand = false)
        {
            base.ExeModule(blnByHand);
            sw = new System.Diagnostics.Stopwatch();
            sw.Start();

            try
            {

                cts = new CancellationTokenSource();
                eventWait.Reset();

                Task<bool> longTask = new Task<bool>(() => WaitTimeTask(DelayTime, cts.Token), cts.Token);
                longTask.Start();

                //Task tt = Task.Factory.StartNew(() => WaitTimeTask(DelayTime, cts.Token), cts.Token);
                //Console.WriteLine("取消前，第一个任务的状态：{0}", tt.Status);
                eventWait.WaitOne();

                if (cts != null)
                {
                    cts.Dispose();
                    cts = null;
                }
              
                ModuleParam.BlnSuccessed = longTask.Result;

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

        [OnDeserializing()]
        internal void OnDeSerializingMethod(StreamingContext context)
        {
            eventWait = new AutoResetEvent(false);//采集信号
        }

    }
}
