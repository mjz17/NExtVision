using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Reflection;
using System.ComponentModel;

namespace VisionCore
{
    public class PluginService
    {

        /// <summary>
        /// 插件/字典
        /// </summary>
        public static Dictionary<string, ModuleInfo> s_PluginDic = new Dictionary<string, ModuleInfo>();

        /// <summary>
        /// 初始化插件
        /// </summary>
        public static void InitPlugin()
        {
            string PlugInsDir = Path.Combine(System.Environment.CurrentDirectory, "Plugins\\");

            if (Directory.Exists(PlugInsDir) == false)//判断是否存在
                return;

            foreach (var dllFile in Directory.GetFiles(PlugInsDir))
            {
                try
                {

                    FileInfo fi = new FileInfo(dllFile);

                    if (!fi.Name.StartsWith("Plugin.") || !fi.Name.EndsWith(".dll"))
                        continue;

                    Assembly assemPlugIn = AppDomain.CurrentDomain.Load(Assembly.LoadFile(fi.FullName).GetName());

                    //判断是否包含ModuleObjBase
                    foreach (Type type in assemPlugIn.GetTypes())
                    {
                        if (typeof(ModuleObjBase).IsAssignableFrom(type))//是ModuleBase的子类
                        {
                            ModuleInfo info = new ModuleInfo();
                            if (GetPluginInfo(assemPlugIn, type, ref info))
                            {

                                s_PluginDic[info.PluginName] = info;
                            }
                            break;
                        }
                    }

                }
                catch (Exception ex)
                {
                    System.Windows.Forms.MessageBox.Show(ex.ToString());
                }
            }
        }

        /// <summary>
        /// 获取插件类别
        /// </summary>
        /// <param name="assemPlugIn"></param>
        /// <param name="type"></param>
        /// <param name="info"></param>
        /// <returns></returns>
        public static bool GetPluginInfo(Assembly assemPlugIn, Type type, ref ModuleInfo info)
        {
            try
            {
                object[] categoryObjs = type.GetCustomAttributes(typeof(CategoryAttribute), true);
                object[] dispNameObjs = type.GetCustomAttributes(typeof(DisplayNameAttribute), true);

                info.PluginCategory = (((CategoryAttribute)categoryObjs[0]).Category);
                info.PluginName = (((DisplayNameAttribute)dispNameObjs[0]).DisplayName);
                info.ModuleObjType = type;

                foreach (Type tempType in assemPlugIn.GetTypes())
                {
                    if (typeof(PluginFrmBase).IsAssignableFrom(tempType))
                    {
                        info.ModuleFormType = tempType;
                        return true;
                    }
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }
            return false;
        }

    }
}
