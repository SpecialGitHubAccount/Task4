using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task4.Configuration
{
    class RuleElement : ConfigurationElement
    {
        [ConfigurationProperty("pattern", IsKey = true, IsRequired = true)]
        public string RegexPattern
        {
            get
            {
                return (string)this["pattern"];
            }
            set
            {
                this["pattern"] = value;
            }
        }

        [ConfigurationProperty("destinationFolder", DefaultValue = "Other")]
        public string DestinationFolder
        {
            get
            {
                return (string)this["destinationFolder"];
            }
            set
            {
                this["destinationFolder"] = value;
            }
        }

        [ConfigurationProperty("isAutoIncrementId")]
        public bool IsAutoIncrementId
        {
            get
            {
                return (bool)this["isAutoIncrementId"];
            }
            set
            {
                this["isAutoIncrementId"] = value;
            }
        }

        [ConfigurationProperty("isAppendDate")]
        public bool IsAppendDate
        {
            get
            {
                return (bool)this["isAppendDate"];
            }
            set
            {
                this["isAppendDate"] = value;
            }
        }

        public override bool IsReadOnly()
        {
            return false;
        }
    }
}
