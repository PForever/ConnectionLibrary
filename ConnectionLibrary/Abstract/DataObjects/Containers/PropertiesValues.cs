using System.Collections.Generic;

namespace ConnectionLibrary.Abstract.DataObjects.Containers
{
    public class PropertiesValues : Dictionary<string, string>
    {
        public PropertiesValues() : base()
        {
        }

        public PropertiesValues(int count) : base(count)
        {
        }
        public PropertiesValues(IDictionary<string, string> dictionary) : base(dictionary) { }
    }
}