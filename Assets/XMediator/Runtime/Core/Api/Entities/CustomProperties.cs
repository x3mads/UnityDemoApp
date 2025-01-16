using System.Collections.Generic;
using JetBrains.Annotations;

namespace XMediator.Api
{
    /// <summary>
    /// A container class for key-value custom properties.
    /// Use <see cref="Builder"/> to create an instance of this class.
    /// </summary>
    public class CustomProperties
    {
        private IDictionary<string, object> Properties;

        private CustomProperties(IDictionary<string, object> properties)
        {
            Properties = properties;
        }

        public int? GetInt(string key)
        {
            return Properties[key] as int?;
        }

        public long? GetLong(string key)
        {
            return Properties[key] as long?;
        }

        public float? GetFloat(string key)
        {
            return Properties[key] as float?;
        }

        public double? GetDouble(string key)
        {
            return Properties[key] as double?;
        }

        [CanBeNull]
        public string GetString(string key)
        {
            return Properties[key] as string;
        }

        [CanBeNull]
        public IEnumerable<string> GetStringSet(string key)
        {
            return Properties[key] as IEnumerable<string>;
        }

        internal IDictionary<string, object> GetAll()
        {
            return Properties;
        }

        /// <summary>
        /// Returns a new <see cref="Builder"/> with all of the key-value pairs from this object.
        /// </summary>
        /// <returns></returns>
        public Builder NewBuilder()
        {
            return new Builder(this);
        }

        /// <summary>
        /// A convenience class for creating CustomProperties.
        /// </summary>
        public class Builder
        {
            private IDictionary<string, object> Properties;

            public Builder()
            {
                Properties = new Dictionary<string, object>();
            }

            public Builder(CustomProperties customProperties)
            {
                Properties = customProperties.Properties;
            }

            public Builder AddBoolean(string key, bool value)
            {
                Properties[key] = value;
                return this;
            }

            public Builder AddInt(string key, int value)
            {
                Properties[key] = value;
                return this;
            }

            // TODO: not public because it is not implemented in iOS
            private Builder AddLong(string key, long value)
            {
                Properties[key] = value;
                return this;
            }

            public Builder AddFloat(string key, float value)
            {
                Properties[key] = value;
                return this;
            }

            public Builder AddDouble(string key, double value)
            {
                Properties[key] = value;
                return this;
            }

            public Builder AddString(string key, string value)
            {
                Properties[key] = value;
                return this;
            }

            public Builder AddStringSet(string key, IEnumerable<string> value)
            {
                Properties[key] = value;
                return this;
            }

            public CustomProperties Build()
            {
                return new CustomProperties(Properties);
            }
        }
    }
}