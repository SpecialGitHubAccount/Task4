using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task4.Configuration
{
    public class DirectoryElement : ConfigurationElement
    {
        [ConfigurationProperty("path", IsKey = true, IsRequired = true)]
        public string Path
        {
            set
            {
                this["path"] = value;
            }
            get
            {
                return (string)this["path"];       
            }
        }
        public override bool IsReadOnly()
        {
            return false;
        }
    }

}
