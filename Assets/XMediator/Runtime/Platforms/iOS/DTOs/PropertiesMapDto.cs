using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using UnityEngine;
using XMediator.Api;

namespace XMediator.iOS
{
    [Serializable]
    internal class PropertiesMapDto
    {
        [SerializeField] internal List<BoolPropertyDto> bools;
        [SerializeField] internal List<IntPropertyDto> ints;
        [SerializeField] internal List<DoublePropertyDto> doubles;
        [SerializeField] internal List<StringPropertyDto> strings;
        [SerializeField] internal List<StringListPropertyDto> stringLists;

        public PropertiesMapDto(IDictionary<string, object> properties)
        {
            bools = new List<BoolPropertyDto>();
            ints = new List<IntPropertyDto>();
            doubles = new List<DoublePropertyDto>();
            strings = new List<StringPropertyDto>();
            stringLists = new List<StringListPropertyDto>();
            
            foreach (var keyValue in properties)
            {
                switch (keyValue.Value)
                {
                    case int intValue:
                        ints.Add(new IntPropertyDto(keyValue.Key, intValue));
                        break;
                    case double doubleValue:
                        doubles.Add(new DoublePropertyDto(keyValue.Key, doubleValue));
                        break;
                    case string stringValue:
                        strings.Add(new StringPropertyDto(keyValue.Key, stringValue));
                        break;
                    case bool boolValue:
                        bools.Add(new BoolPropertyDto(keyValue.Key, boolValue));
                        break;
                    case IEnumerable<string> stringEnumerableValue:
                        stringLists.Add(new StringListPropertyDto(keyValue.Key, stringEnumerableValue.ToList()));
                        break;
                }
            }
        }

        public Dictionary<string, object> ToDictionary()
        {
            var result = new Dictionary<string, object>();
            bools?.ForEach(p => result[p.k] = p.v);
            ints?.ForEach(p => result[p.k] = p.v);
            doubles?.ForEach(p => result[p.k] = p.v);
            strings?.ForEach(p => result[p.k] = p.v);
            stringLists?.ForEach(p => result[p.k] = p.v);
            return result;
        }
        
    }

    [Serializable]
    internal class PropertyDto<T>
    {
        [SerializeField] internal string k;
        [SerializeField] internal T v;

        public PropertyDto(string key, T value)
        {
            this.k = key;
            this.v = value;
        }
    }
    
    [Serializable]
    internal class IntPropertyDto: PropertyDto<int>
    {
        public IntPropertyDto(string key, int value) : base(key, value) { }
    }
    
    [Serializable]
    internal class BoolPropertyDto: PropertyDto<bool>
    {
        public BoolPropertyDto(string key, bool value) : base(key, value) { }
    }
    
    [Serializable]
    internal class DoublePropertyDto: PropertyDto<double>
    {
        public DoublePropertyDto(string key, double value) : base(key, value) { }
    }
    
    [Serializable]
    internal class StringPropertyDto: PropertyDto<string>
    {
        public StringPropertyDto(string key, string value) : base(key, value) { }
    }
    
    [Serializable]
    internal class StringListPropertyDto: PropertyDto<List<string>>
    {
        public StringListPropertyDto(string key, List<string> value) : base(key, value) { }
    }
}