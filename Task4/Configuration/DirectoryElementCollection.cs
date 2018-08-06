using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Task4.Configuration
{
    public class DirectoryElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new DirectoryElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((DirectoryElement)element).Path;
        }
        public override bool IsReadOnly()
        {
            return false;
        }
        public void Add(ConfigurationElement element)
        {            
            BaseAdd(element);
        }
    }

}
