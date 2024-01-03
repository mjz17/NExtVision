using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace StyleBase
{
    public class LayDatagrid : DataGrid
    {
        /// <summary>
        /// 是否正在刷新
        /// </summary>
        bool flag = false;

        public static int Row = 0;

        protected override void OnLoadingRow(DataGridRowEventArgs e)
        {
            base.OnLoadingRow(e);
            DataGridHelper.SetRowIndex(e.Row, e.Row.GetIndex() + 1);
            Row = e.Row.GetIndex();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (!flag)
            {
                flag = true;
                Items.Refresh();
                flag = false;
            }
        }
    }
}
