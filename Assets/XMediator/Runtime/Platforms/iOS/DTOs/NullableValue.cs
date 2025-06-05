using System;
using JetBrains.Annotations;
using UnityEngine;

namespace XMediator.iOS
{
    [Serializable]
    internal class NullableValue<T>
    {
        [SerializeField] internal bool hasValue;
        [SerializeField] internal T value;

        internal NullableValue(bool hasValue, T value)
        {
            this.hasValue = hasValue;
            this.value = value;
        }
    }
    
    [Serializable]
    internal class NullableLongValue: NullableValue<long>
    {
        internal NullableLongValue(long? value) : base(value != null, value ?? default) {}
        
        internal long? GetValue() => hasValue ? value : null;
    }
    
    [Serializable]
    internal class NullableObject<T>: NullableValue<T> where T : class
    {
        internal NullableObject(T value) : base(value != null, value) {}
        
        [CanBeNull] internal T GetValue() => hasValue ? value : null;
    }
    
    [Serializable]
    internal class NullableIntValue: NullableValue<int>
    {
        internal NullableIntValue(int? value) : base(value != null, value ?? default) {}
        
        internal int? GetValue() => hasValue ? value : null;
    }
    
    [Serializable]
    internal class NullableDoubleValue: NullableValue<double>
    {
        internal NullableDoubleValue(double? value) : base(value != null, value ?? default) {}
        
        internal double? GetValue() => hasValue ? value : null;
    }
}