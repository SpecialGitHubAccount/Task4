using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Task4.Configuration
{
    public class RuleElementCollection : ConfigurationElementCollection
    {
        protected override ConfigurationElement CreateNewElement()
        {
            return new RuleElement();
        }

        protected override object GetElementKey(ConfigurationElement element)
        {
            return ((RuleElement)element).RegexPattern;
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
