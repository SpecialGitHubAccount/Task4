using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task4.Configuration
{
    public class CultureElement : ConfigurationElement
    {
        [ConfigurationProperty("name", IsRequired = true)]
        public Cultures Name
        {
            set
            {                
                this["name"] = value;
            }    
            get
            {
                return (Cultures)this["name"];
            }
        }       
    }
}
