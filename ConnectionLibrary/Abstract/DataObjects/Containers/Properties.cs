using System;
using System.Collections.Generic;
using ConnectionLibrary.Abstract.DataObjects.DeviceInfo;

namespace ConnectionLibrary.Abstract.DataObjects.Containers
{
    public class Properties : Dictionary<string, PropertyInfo>, ICloneable
    {
        public Properties() : base() {}
        public Properties(int capacity) : base(capacity) {}
        public Properties(IDictionary<string, PropertyInfo> dictionary) : base(dictionary) {}
        public object Clone()
        {
            var properties = new Properties(Count);
            foreach (var keyValuePair in this)
            {
                properties.Add(keyValuePair.Key, keyValuePair.Value);
            }
            return properties;
        }
    }
}