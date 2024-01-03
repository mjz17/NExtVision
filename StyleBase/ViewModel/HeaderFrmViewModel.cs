using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ClassLibBase;

namespace StyleBase
{
    /// <summary>
    /// 表头
    /// </summary>
    public class HeaderFrmViewModel : NoitifyBase
    {

        public CommandBase CloseWindow { get; set; }
        public CommandBase MaxWindow { get; set; }
        public CommandBase MinWindow { get; set; }
        public CommandBase MouseMoveWindow { get; set; }
        public HeaderFrmModel HeaderFrm { get; set; } = new HeaderFrmModel();

        /// <summary>
        /// 构造函数
        /// </summary>
        public HeaderFrmViewModel()
        {
            this.CloseWindow = new CommandBase();
            this.CloseWindow.DoExecute = new Action<object>((o) =>
            {
                (o as Window).Close();

            });
            this.CloseWindow.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.MaxWindow = new CommandBase();
            this.MaxWindow.DoExecute = new Action<object>((o) =>
            {
                (o as Window).WindowState = (o as Window).WindowState == WindowState.Maximized ? WindowState.Normal : WindowState.Maximized;

            });
            this.MaxWindow.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.MinWindow = new CommandBase();
            this.MinWindow.DoExecute = new Action<object>((o) =>
            {
                (o as Window).WindowState = WindowState.Minimized;

            });
            this.MinWindow.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

            this.MouseMoveWindow = new CommandBase();
            this.MouseMoveWindow.DoExecute = new Action<object>((o) =>
            {
                (o as Window).DragMove();

            });
            this.MouseMoveWindow.DoCanExecute = new Func<object, bool>((o) =>
            {
                return true;
            });

        }

    }
}
