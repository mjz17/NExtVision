using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VisionCore;
using System.Windows.Controls;
using System.ComponentModel;
using System.Windows;
using System.Configuration;
using System.Collections;

namespace StyleBase
{
    /// <summary>
    /// 公共方法
    /// </summary>
    public class CommonBase
    {
        /// <summary>
        /// 点击流程栏，改变模块栏名称
        /// </summary>
        /// <param name="SelectProName"></param>
        public void RefreshToolList(string SelectProName)
        {
            Project prj = SysProcessPro.g_ProjectList.Find(c => c.ProjectInfo.m_ProjectName == SelectProName);
            if (prj != null)
            {
                SysProcessPro.Cur_Project = prj;
                SysProcessPro.Cur_Project.CurModuleID = prj.ProjectInfo.m_ProjectID;//2023.1.28对应流程修改后，点击流程UI不切换的Bug
                List<ModuleInfo> Info = new List<ModuleInfo>();

                if (prj != null && prj.m_ModuleObjList != null)
                {
                    foreach (ModuleObjBase item in prj.m_ModuleObjList)
                    {
                        Info.Add(new ModuleInfo()
                        {
                            PluginName = item.ModuleParam.PluginName,
                            ModuleNO = item.ModuleParam.ModuleID,
                            ModuleName = item.ModuleParam.ModuleName,
                            IsUse = item.ModuleParam.IsUse,
                            IsSuccess = item.ModuleParam.BlnSuccessed,
                            CostTime = item.ModuleParam.ModuleCostTime,
                        });
                    }
                }
                DataChange.m_Infos = Info;
            }
        }
    }

    /// <summary>
    /// 操作配置文件
    /// </summary>
    public class CommonConfig
    {

        /// <summary>
        /// 输入Key的值，返回配置的值
        /// </summary>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public string ReadConfig(string KeyName)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            return cfa.AppSettings.Settings[KeyName].Value;
        }

        /// <summary>
        /// 根据配置的名称，查询独立的数据
        /// </summary>
        /// <param name="ProName"></param>
        /// <param name="KeyName"></param>
        /// <returns></returns>
        public string ReadConfig(string ProName, string KeyName)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var mySection = config.GetSection(ProName) as CommonSection;
            foreach (CommonSection.CommonKeyValueSetting add in mySection.KeyValues)
            {
                if (add.Key == KeyName)
                {
                    return add.Value;
                }
            }
            return null;
        }

        /// <summary>
        /// 增加配置文件
        /// </summary>
        /// <param name="KeyName"></param>
        /// <param name="Value"></param>
        public void AddConfig(string KeyName, string Value)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings.Add(KeyName, Value);
            cfa.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// 根据配置的名称，查询独立的数据,并添加独立的内容
        /// </summary>
        /// <param name="ProName"></param>
        /// <param name="KeyName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public void AddConfig(string ProName, string KeyName, string Value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var mySection = config.GetSection(ProName) as CommonSection;
            mySection.KeyValues.Add(new CommonSection.CommonKeyValueSetting() { Key = KeyName, Value = Value });
            config.Save();
            ConfigurationManager.RefreshSection(ProName);  //刷新
        }

        /// <summary>
        /// 删除配置文件
        /// </summary>
        /// <param name="KeyName"></param>
        public void DeleteConfig(string KeyName)
        {
            Configuration cfa = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            cfa.AppSettings.Settings.Remove(KeyName);
            cfa.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// 修改配置文件数据
        /// </summary>
        /// <param name="KeyName"></param>
        /// <param name="Value"></param>
        public void WriteConfig(string KeyName, string Value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            config.AppSettings.Settings[KeyName].Value = Value;
            config.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("appSettings");
        }

        /// <summary>
        /// 根据配置的名称，查询独立的数据,并修改内容
        /// </summary>
        /// <param name="ProName"></param>
        /// <param name="KeyName"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public void WriteConfig(string ProName, string KeyName, string Value)
        {
            Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            var mySection = config.GetSection(ProName) as CommonSection;
            mySection.KeyValues.Remove(KeyName);
            mySection.KeyValues.Add(new CommonSection.CommonKeyValueSetting() { Key = KeyName, Value = Value });
            config.Save();
            ConfigurationManager.RefreshSection(ProName);  //刷新
        }

    }

    /// <summary>
    /// 配置节点基类
    /// </summary>
    public class CommonSection : ConfigurationSection
    {

        private static ConfigurationProperty s_property =
            new ConfigurationProperty(string.Empty, typeof(CommonKeyValueCollection), null, ConfigurationPropertyOptions.IsDefaultCollection);

        [ConfigurationProperty("", Options = ConfigurationPropertyOptions.IsDefaultCollection)]
        public CommonKeyValueCollection KeyValues
        {
            get
            {
                return (CommonKeyValueCollection)base[s_property];
            }
        }

        /// <summary>
        /// 自定义一个集合
        /// </summary>
        [ConfigurationCollection(typeof(CommonKeyValueSetting))]
        public class CommonKeyValueCollection : ConfigurationElementCollection
        {
            // 基本上，所有的方法都只要简单地调用基类的实现就可以了
            // 忽略大小写
            public CommonKeyValueCollection() : base(StringComparer.OrdinalIgnoreCase)
            {

            }

            // 其实关键就是这个索引器。但它也是调用基类的实现，只是做下类型转就行了。
            new public CommonKeyValueSetting this[string name]
            {
                get { return (CommonKeyValueSetting)base.BaseGet(name); }
                set { base[name] = value; }
            }

            //下面二个方法中抽象类中必须要实现的
            protected override ConfigurationElement CreateNewElement()
            {
                return new CommonKeyValueSetting();
            }

            //
            protected override object GetElementKey(ConfigurationElement element)
            {
                return ((CommonKeyValueSetting)element).Key;
            }

            public void Add(CommonKeyValueSetting setting)
            {
                this.BaseAdd(setting);
            }

            public void Clear()
            {
                base.BaseClear();
            }

            public void Remove(string name)
            {
                base.BaseRemove(name);
            }

        }

        /// <summary>
        /// 集合中的每个元素
        /// </summary>
        public class CommonKeyValueSetting : ConfigurationElement
        {

            /// <summary>
            /// 键
            /// </summary>
            [ConfigurationProperty("key", IsRequired = true)]
            public string Key
            {
                get { return this["key"].ToString(); }
                set { this["key"] = value; }
            }

            /// <summary>
            /// 值
            /// </summary>
            [ConfigurationProperty("value", IsRequired = true)]
            public string Value
            {
                get { return this["value"].ToString(); }
                set { this["value"] = value; }
            }

        }

    }

}
