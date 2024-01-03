using ModuleDataVar;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using HalconDotNet;
using Common;

namespace Plugin.ImagesUnit
{
    [Category("图像处理")]
    [DisplayName("图像合并")]
    [Serializable]
    public class ModuleObj : ModuleObjBase
    {
        /// <summary>
        /// 链接的数据
        /// </summary>
        public List<ImageUnitPram> ImageUnit = new List<ImageUnitPram>();

        /// <summary>
        /// 合并的图像的宽
        /// </summary>
        public int lastImageWidth = 0;

        /// <summary>
        /// 合并图像的高
        /// </summary>
        public int lastImageHeight = 0;

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
                //查询索引
                int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == ModuleParam.ProjectID);
                //定义数组
                string[] Info;
                //合并图像
                HImage image = new HImage();
                //查询数据
                foreach (ImageUnitPram item in ImageUnit)
                {
                    //分割字符
                    Info = item.m_UnitName.Split('.');
                    //查询模块获得模块图像
                    DataVar dataVar = SysProcessPro.g_ProjectList[proIndex].m_Var_List.Find(c => c.m_DataTip == Info[0] && c.m_DataName == Info[1]);
                    if (dataVar.m_DataValue != null)
                    {
                        item.m_HImage = ((HImage)dataVar.m_DataValue).Clone();//克隆出Image
                    }

                    //查询模块,获得图像数据
                    ModuleObjBase ProcessNum = SysProcessPro.g_ProjectList[proIndex].m_ModuleObjList.Find(c => c.ModuleParam.ModuleName == Info[0]);
                    if (ProcessNum != null)
                    {
                        //获得HHomMat2D,转换图像
                        item.m_HImage = ProcessNum.ModuleImageParam.m_homMat2D.AffineTransImageSize(item.m_HImage, "constant", lastImageWidth, lastImageHeight);
                    }
                    image = image.ConcatObj(item.m_HImage);
                }

                HTuple offsetRow = new HTuple(0, 0);
                HTuple offsetCol = new HTuple(0, 0);
                HTuple row1 = new HTuple(-1, -1);
                HTuple col1 = new HTuple(-1, -1);
                HTuple row2 = new HTuple(-1, -1);
                HTuple col2 = new HTuple(-1, -1);
                int width = 0;
                int height = 0;

                //合并图像
                image.TileImagesOffset(offsetRow, offsetCol, row1, col1, row2, col2, width, height);

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

    [Serializable]
    public class ImageUnitPram
    {
        //索引
        public string m_Index { get; set; }
        //合并图像的名称
        public string m_UnitName { get; set; }
        //参数
        public HHomMat2D m_HHomMat2D { get; set; }
        //转换的图像
        public HImage m_HImage { get; set; }
        public int m_Row1 { get; set; } = 0;
        public int m_Col1 { get; set; } = 0;
        public int m_Row2 { get; set; } = 0;
        public int m_Col2 { get; set; } = 0;
    }

}
