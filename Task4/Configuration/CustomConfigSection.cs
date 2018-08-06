using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;

namespace Task4.Configuration
{
    public class CustomConfigSection : ConfigurationSection, ICloneable
    {
        [ConfigurationProperty("name")]
        public string Name
        {
            get
            {
                return (string)this["name"];
            }
            set
            {
                this["name"] = value;
            }
        }

        [ConfigurationProperty("count")]
        public int Count
        {
            get
            {
                return (int)this["count"];
            }
            set
            {
                this["count"] = value;
            }
        }

        [ConfigurationProperty("culture")]
        public CultureElement Culture
        {
            set
            {
                this["culture"] = value;
            }
            get
            {
                return (CultureElement)this["culture"];
            }
        }

        [ConfigurationCollection(typeof(DirectoryElement),
                                 AddItemName = "directory",
                                 ClearItemsName = "clear",
                                 RemoveItemName = "delete")]
        [ConfigurationProperty("directories")]
        public DirectoryElementCollection Directories
        {
            set { this["directories"] = value; }
            get { return (DirectoryElementCollection)this["directories"]; }
        }

        [ConfigurationCollection(typeof(RuleElement),
                                 AddItemName = "rule",
                                 ClearItemsName = "clear",
                                 RemoveItemName = "delete")]
        [ConfigurationProperty("rules")]
        public RuleElementCollection Rules
        {
            get { return (RuleElementCollection)this["rules"]; }
        }

        public override bool IsReadOnly()
        {
            return false;
        }

        public object Clone()
        {
            CustomConfigSection clone = new CustomConfigSection();

            clone.Count = this.Count;
            clone.Name = this.Name;

            clone.Culture = this.Culture;

            foreach (DirectoryElement item in this.Directories)
            {
                clone.Directories.Add(item);
            }

            foreach (RuleElement item in this.Rules)
            {
                clone.Rules.Add(item);
            }

            return clone;
        }
    }
}
