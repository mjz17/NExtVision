using Common;
using System;
using System.Collections.Generic;
using System.Linq;
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

namespace Plugin.BlobAnalysis
{
    /// <summary>
    /// ImageSelect_shape.xaml 的交互逻辑
    /// </summary>
    public partial class ImageSelect_shape : UserControl
    {
        //对应的后台程序
        public Select_shapeImage m_select_Shape;

        public ImageSelect_shape(BaseMethod method)
        {
            InitializeComponent();
            m_select_Shape = (Select_shapeImage)method;
            this.DataContext = this;
            InitCmbShapeModel();
            Refresh();
            this.cmb_shapeModel.DropDownClosed += Cmb_shapeModel_DropDownClosed;
            this.Cmb_SelectIndex.DropDownClosed += ComboBox_DropDownClosed;
            this.Loaded += ImageSelect_shape_Loaded;
        }

        private void ImageSelect_shape_Loaded(object sender, RoutedEventArgs e)
        {
            //索引
            if (m_select_Shape.m_Index == 0)
            {
                //控件隐藏
                m_Control = Visibility.Hidden;
            }
            else
            {
                //控件隐藏
                m_Control = Visibility.Visible;
                //数据写入
                Cmb_SelectIndex.ItemsSource = m_select_Shape.InitCmb();
                //当前的选择为前面一个
                this.Cmb_SelectIndex.SelectedIndex = m_select_Shape.m_LinkIndex == "上一个区域" ? 0 : m_select_Shape.m_Index - 1;
            }
        }

        #region 索引状态

        public Visibility m_Control
        {
            get { return (Visibility)this.GetValue(m_ControlProperty); }
            set { this.SetValue(m_ControlProperty, value); }
        }

        public static readonly DependencyProperty m_ControlProperty =
            DependencyProperty.Register("m_Control", typeof(Visibility), typeof(ImageSelect_shape), new PropertyMetadata(default(Visibility)));

        #endregion

        public void InitCmbShapeModel()
        {
            cmb_shapeModel.ItemsSource = Enum.GetNames(typeof(shapeModel));
            cmb_shapeModel.SelectedIndex = (int)m_select_Shape.m_shapeModel;
        }

        private void ComboBox_DropDownClosed(object sender, EventArgs e)
        {
            m_select_Shape.m_LinkIndex = this.Cmb_SelectIndex.Text;
        }

        private void Cmb_shapeModel_DropDownClosed(object sender, EventArgs e)
        {
            m_select_Shape.m_shapeModel = (shapeModel)cmb_shapeModel.SelectedIndex;
        }

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                System.Windows.Controls.MenuItem menu = sender as System.Windows.Controls.MenuItem;
                if (menu == null)
                    return;

                m_select_Shape.m_shapes.Add(new shapeInfo
                {
                    m_shapeType = (shapeType)Convert.ToInt32(menu.Tag),
                });

                Refresh();
            }
            catch (Exception ex)
            {
                Log.Error(ex.ToString());
                System.Windows.Forms.MessageBox.Show(ex.ToString());
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

        }

        /// <summary>
        /// 刷新Dgv
        /// </summary>
        private void Refresh()
        {
            this.dgv_Shape.ItemsSource = null;
            this.dgv_Shape.ItemsSource = m_select_Shape.m_shapes;
        }



    }
}
