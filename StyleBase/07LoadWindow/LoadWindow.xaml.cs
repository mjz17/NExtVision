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
using System.Windows.Shapes;

namespace StyleBase
{

    public delegate void TaskEndNotify(Object result);

    /// <summary>
    /// LoadWindow.xaml 的交互逻辑
    /// </summary>
    public partial class LoadWindow : Window
    {
        public LoadWindow(LongTimeTask timeTask)
        {
            m_Task = timeTask;
            InitializeComponent();
            this.Loaded += LoadWindow_Loaded;
            this.Closing += LoadWindow_Closing;
        }

        private object m_TasResult;

        public object TasResult
        {
            get { return m_TasResult; }
            set { m_TasResult = value; }
        }

        private readonly LongTimeTask m_Task;

        private bool m_Close;

        private void LoadWindow_Loaded(object sender, RoutedEventArgs e)
        {
            m_Task.Start(this);
        }

        private void LoadWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (!m_Close) { e.Cancel = true; }
        }

        private delegate void CloseMethod();

        public void TaskEnd(object retult)
        {
            TasResult = retult;
            m_Close = true;
            Dispatcher.BeginInvoke(new CloseMethod(Close));
        }

    }
}
