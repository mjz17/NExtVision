using Common;
using ModuleDataVar;
using PublicDefine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using VisionCore;

namespace Plugin.ImagePretreat
{
    /// <summary>
    /// ModuleFrm.xaml 的交互逻辑
    /// </summary>
    public partial class ModuleFrm : PluginFrmBase
    {

        private ModuleObj frm_ModuleObj;

        public ModuleFrm()
        {
            InitializeComponent();
            this.MaxHeight = SystemParameters.MaximizedPrimaryScreenHeight - 16;
            this.MaxWidth = SystemParameters.MaximizedPrimaryScreenWidth;
            this.DataContext = this;
            this.Loaded += ModuleFrm_Loaded;
        }

        private void ModuleFrm_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                //加载参数
                frm_ModuleObj = (ModuleObj)m_ModuleObjBase;
                //窗体名称
                Title.HeadName = frm_ModuleObj.ModuleParam.ModuleName;
                //模块当前ID
                CurrentModelID = frm_ModuleObj.ModuleParam.ModuleName;

                if (!frm_ModuleObj.blnNewModule)
                {
                    theFirsttime();
                }
                else
                {
                    theSecondTime();
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(ex.ToString());
            }
        }

        public override void theFirsttime()
        {
            base.theFirsttime();
            #region 查询该流程中为图像的模块.

            if (frm_ModuleObj.GenModuleIndex(out string str, out frm_ModuleObj.m_Image, out frm_ModuleObj.Link_Image_Data))
            {
                CurrentImage = str;
                DispHwImg.UpdateWindow(frm_ModuleObj, this.Main_HalconView);
            }

            #endregion
        }

        //第二次打开
        public override void theSecondTime()
        {
            base.theSecondTime();
            #region 读取链接图像并显示

            CurrentImage = frm_ModuleObj.m_CurentImgName;
            if (CurrentImage != null && CurrentImage.Length > 0)
            {
                DataVar var = frm_ModuleObj.m_ModuleProject.GetCurLocalVarValue(frm_ModuleObj.Link_Image_Data);
                if (var.m_DataValue is List<HImageExt>)
                {
                    frm_ModuleObj.m_Image = ((List<HImageExt>)var.m_DataValue)[0];
                    DispHwImg.UpdateWindow(frm_ModuleObj, this.Main_HalconView);
                }
            }

            #endregion
            Refresh();
            InitBaseMethod();
        }

        #region 显示图像

        /// <summary>
        /// 获取变量，传入图像显示窗体
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Gen_ImgPath_EValueAlarm(object sender, RoutedEventArgs e)
        {
            ModuleDataVar.DataVar data = (ModuleDataVar.DataVar)e.OriginalSource;
            //数据不为空，且是图像类型
            if (data.m_DataValue != null && data.m_DataType == ModuleDataVar.DataVarType.DataType.Image)
            {
                try
                {
                    if (data.m_DataValue is List<HImageExt>)
                    {
                        frm_ModuleObj.m_Image = ((List<HImageExt>)(data).m_DataValue)[0];
                        frm_ModuleObj.Link_Image_Data = data;
                        DispHwImg.UpdateWindow(frm_ModuleObj, this.Main_HalconView);
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(ex.ToString());
                }
            }
        }

        #endregion

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                MenuItem menu = sender as MenuItem;
                if (menu == null)
                    return;

                string name = menu.Header.ToString();

                switch (name)
                {
                    case "二值化":
                        //frm_ModuleObj.m_Emhan.Add(new ThresholdImage());
                        break;
                    case "均值滤波":
                        frm_ModuleObj.m_Emhan.Add(new MeanImage(this));
                        break;
                    case "中值滤波":
                        //frm_ModuleObj.m_Emhan.Add(new EmphaImage(this));
                        break;
                    case "高斯滤波":
                        frm_ModuleObj.m_Emhan.Add(new GaussImage(this));
                        break;
                    case "对比度":
                        frm_ModuleObj.m_Emhan.Add(new EmphaImage(this));
                        break;
                    case "反色":
                        frm_ModuleObj.m_Emhan.Add(new InvertImage(this));
                        break;
                    case "灰度开运算":
                        frm_ModuleObj.m_Emhan.Add(new GrayOpening(this));
                        break;
                    case "灰度闭运算":
                        frm_ModuleObj.m_Emhan.Add(new GrayClosed(this));
                        break;
                    case "灰度腐蚀":
                        frm_ModuleObj.m_Emhan.Add(new GrayErosionr(this));
                        break;
                    case "灰度膨胀":
                        frm_ModuleObj.m_Emhan.Add(new GrayDilation(this));
                        break;
                    default:
                        break;
                }

                int index = dgv_Empha.SelectedIndex;
                Refresh();
                if (index < 0)
                {
                    if (frm_ModuleObj.m_Emhan.Count > 0)
                        index = dgv_Empha.SelectedIndex = 0;
                }
                else
                {
                    if (frm_ModuleObj.m_Emhan.Count > 0)
                        dgv_Empha.SelectedIndex = index + 1;
                }

                if (index > -1)
                {
                    //显示控件窗体
                    Page_Change.Content = new Frame()
                    {
                        Content = frm_ModuleObj.m_Emhan[index].m_Control
                    };
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(frm_ModuleObj.ModuleParam.ModuleName + "：" + ex.ToString());
            }
        }

        private void btn_Delet_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (dgv_Empha.SelectedIndex > -1)
                {
                    int index = dgv_Empha.SelectedIndex;
                    frm_ModuleObj.m_Emhan.RemoveAt(dgv_Empha.SelectedIndex);
                    Refresh();
                    if (index < 0)
                    {
                        if (frm_ModuleObj.m_Emhan.Count > 0)
                            dgv_Empha.SelectedIndex = 0;
                    }
                    else
                    {
                        if (frm_ModuleObj.m_Emhan.Count > 0)
                            dgv_Empha.SelectedIndex = index - 1;
                    }

                    if (dgv_Empha.SelectedIndex > -1)
                    {
                        //显示控件窗体
                        Page_Change.Content = new Frame()
                        {
                            Content = frm_ModuleObj.m_Emhan[dgv_Empha.SelectedIndex].m_Control
                        };
                    }
                }
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.ToString());
                Log.Error(frm_ModuleObj.ModuleParam.ModuleName + "：" + ex.ToString());
            }
        }

        private void btn_Up_Click(object sender, RoutedEventArgs e)
        {

        }

        private void btn_Down_Click(object sender, RoutedEventArgs e)
        {

        }

        private void dgv_Empha_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            e.Handled = true;
            if (dgv_Empha.SelectedIndex > -1)
            {
                int index = dgv_Empha.SelectedIndex;
                Page_Change.Content = new Frame()
                {
                    Content = frm_ModuleObj.m_Emhan[index].m_Control
                };

                Int32 Col = this.dgv_Empha.Columns.IndexOf(this.dgv_Empha.CurrentColumn);//获取列位置

                if (Col > -1)
                {
                    string name = dgv_Empha.Columns[Col].Header.ToString();
                    if (name.Contains("启用"))
                    {
                        frm_ModuleObj.m_Emhan[index].m_EnableOrnot = frm_ModuleObj.m_Emhan[index].m_EnableOrnot ? false : true;
                    }
                }

                Refresh();
                dgv_Empha.SelectedIndex = index;
            }
        }

        /// <summary>
        /// 对比原图（显示原图）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Contrast_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (frm_ModuleObj.m_Image != null || frm_ModuleObj.m_Image.IsInitialized())
            {
                Main_HalconView.HobjectToHimage(frm_ModuleObj.m_Image);
            }
        }

        /// <summary>
        /// 对比原图（显示处理过的）
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btn_Contrast_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (frm_ModuleObj.m_OutImage != null || frm_ModuleObj.m_OutImage.IsInitialized())
            {
                Main_HalconView.HobjectToHimage(frm_ModuleObj.m_OutImage);
            }
        }

        /// <summary>
        /// 初始化列表
        /// </summary>
        private void InitBaseMethod()
        {
            foreach (BaseMethod item in frm_ModuleObj.m_Emhan)
            {
                switch (item.m_Enhan)
                {
                    case EnhanType.二值化:
                        break;
                    case EnhanType.锐化:
                        break;
                    case EnhanType.均值滤波:
                        item.m_Control = new ImageMean(item);
                        item.frm_Obj = this;
                        break;
                    case EnhanType.中值滤波:

                        break;
                    case EnhanType.高斯滤波:
                        item.m_Control = new ImageGauss(item);
                        item.frm_Obj = this;
                        break;
                    case EnhanType.对比度:
                        item.m_Control = new ImageEmpha(item);
                        item.frm_Obj = this;
                        break;
                    case EnhanType.反色:
                        item.frm_Obj = this;
                        break;
                    case EnhanType.灰度开运算:
                        item.m_Control = new ImageGrayOpening(item);
                        item.frm_Obj = this;
                        break;
                    case EnhanType.灰度闭运算:
                        item.m_Control = new ImageGrayClosed(item);
                        item.frm_Obj = this;
                        break;
                    case EnhanType.灰度腐蚀:
                        item.m_Control = new ImageGrayErosionr(item);
                        item.frm_Obj = this;
                        break;
                    case EnhanType.灰度膨胀:
                        item.m_Control = new ImageGrayDilation(item);
                        item.frm_Obj = this;
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// 刷新Dgv
        /// </summary>
        private void Refresh()
        {
            this.dgv_Empha.ItemsSource = null;
            this.dgv_Empha.ItemsSource = frm_ModuleObj.m_Emhan;
        }

        /// <summary>
        /// 执行
        /// </summary>
        public override void ExModule()
        {
            try
            {
                if (ProtectModuel())
                {
                    //删除本ID的变量
                    int proIndex = SysProcessPro.g_ProjectList.FindIndex(c => c.ProjectInfo.m_ProjectID == frm_ModuleObj.ModuleParam.ProjectID);
                    if (proIndex > -1)
                    {
                        SysProcessPro.g_ProjectList[proIndex].m_Var_List.RemoveAll(c => c.m_DataTip.Contains(frm_ModuleObj.ModuleParam.ModuleName));
                    }

                    frm_ModuleObj.ExeModule();
                    if (frm_ModuleObj.m_OutImage != null && frm_ModuleObj.m_OutImage.IsInitialized())
                    {
                        Main_HalconView.HobjectToHimage(frm_ModuleObj.m_OutImage);
                    }

                }
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
            try
            {
                if (ProtectModuel())
                {
                    //保存参数
                    ((ModuleObj)m_ModuleObjBase).blnNewModule = true;
                    this.Close();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        /// <summary>
        /// 数据保护
        /// </summary>
        /// <returns></returns>
        public override bool ProtectModuel()
        {
            //图像未设置
            if (frm_ModuleObj.Link_Image_Data.m_DataValue == null)
            {
                System.Windows.Forms.MessageBox.Show("图像未设置！");
                return false;
            }

            //判断图像类型
            if (!(frm_ModuleObj.Link_Image_Data.m_DataValue is List<HImageExt>))
            {
                System.Windows.Forms.MessageBox.Show("图像类型错误！");
                return false;
            }

            frm_ModuleObj.m_CurentImgName = CurrentImage;//图像名称
            return true;
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
