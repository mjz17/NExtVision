using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace StyleBase
{
    public class DataGridHelper
    {
        public static int GetRowIndex(DependencyObject obj)
        {
            return (int)obj.GetValue(RowIndexPropery);
        }

        public static void SetRowIndex(DependencyObject obj, int value)
        {
            obj.SetValue(RowIndexPropery, value);
        }

        public static readonly DependencyProperty RowIndexPropery = DependencyProperty.RegisterAttached("RowIndex", typeof(int), typeof(DataGridHelper));

    }
}
