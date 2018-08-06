using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task4.Configuration
{
    class CustomConfigSectionManager : IDisposable
    {
        public CustomConfigSectionManager()
        {
            exeConfiguration = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
            customConfig = (CustomConfigSection)exeConfiguration.GetSection("CustomConfigSection");
            customConfigSectionCopy = customConfig.Clone() as CustomConfigSection;
            exeConfiguration.Sections.Remove("CustomConfigSection");
        }

        public void AddDirectory(DirectoryElement directory)
        {
            customConfigSectionCopy.Directories.Add(directory);
        }

        public void AddRule(RuleElement rule)
        {
            customConfigSectionCopy.Rules.Add(rule);
        }

        public void UpdateCulture(CultureElement culture)
        {
            customConfigSectionCopy.Culture = culture;
        }

        public void UpdateCounter(int i)
        {
            customConfigSectionCopy.Count = i;
        }

        public CultureElement GetCurrentCulture()
        {
            return customConfigSectionCopy.Culture;
        }

        public int GetCurrentCount()
        {
            return customConfigSectionCopy.Count;
        }

        public RuleElementCollection GetRules()
        {
            return customConfigSectionCopy.Rules;
        }

        public DirectoryElementCollection GetDirectories()
        {
            return customConfigSectionCopy.Directories;
        }

        public void Dispose()
        {
            exeConfiguration.Sections.Add("CustomConfigSection", customConfigSectionCopy);
            exeConfiguration.Save(ConfigurationSaveMode.Modified);
            ConfigurationManager.RefreshSection("CustomConfigSection");
        }

        private System.Configuration.Configuration exeConfiguration = null;
        private CustomConfigSection customConfig = null;
        CustomConfigSection customConfigSectionCopy = null;
    }
}
