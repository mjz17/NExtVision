using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace StyleBase
{
    public class ElementTool
    {

        /// <summary>
        /// 查询依赖对象的属性
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static T FindVisualParent<T>(DependencyObject obj) where T : class
        {
            while (obj != null)
            {
                if (obj is T)
                {
                    return obj as T;
                }
                obj = VisualTreeHelper.GetParent(obj);
            }
            return null;
        }

    }
}
