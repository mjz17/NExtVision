using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    /// <summary>
    /// 日志等级
    /// </summary>
    public enum LogLevel
    {
        Debug = 1,  //调试
        Info,       //信息
        Warn,       //警告
        Error,      //错误
        Fatal       //致命的
    }
}
