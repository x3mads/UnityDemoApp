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

        internal CustomProperties(IDictionary<string, object> properties)
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

        /// <summary>
        /// Gets all the custom properties as a dictionary.
        /// </summary>
        /// <returns>
        /// A dictionary containing all the custom properties.
        /// </returns>
        public IDictionary<string, object> GetAll()
        {
            return Properties;
        }
        
        /// <summary>
        /// Creates a new <see cref="Builder"/> with the current key-value pairs from this object.
        /// </summary>
        /// <returns>A new <see cref="Builder"/> with the current properties.</returns>
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

            /// <summary>
            /// Creates a new <see cref="Builder"/> with no properties.
            /// </summary>
            public Builder()
            {
                Properties = new Dictionary<string, object>();
            }

            public Builder(CustomProperties customProperties)
            {
                Properties = customProperties.Properties;
            }
            
            /// <summary>
            /// Removes a property from the custom properties by key.
            /// </summary>
            /// <param name="key">The key of the property to remove.</param>
            /// <returns>The same <see cref="Builder"/> instance.</returns>
            public Builder Remove(string key)
            {
                Properties.Remove(key);
                return this;
            }

            /// <summary>
            /// Adds a boolean value to the custom properties.
            /// </summary>
            /// <param name="key">The key of the property.</param>
            /// <param name="value">The value of the property.</param>
            /// <returns>The same <see cref="Builder"/> instance.</returns>
            public Builder AddBoolean(string key, bool value)
            {
                Properties[key] = value;
                return this;
            }

            /// <summary>
            /// Adds an integer value to the custom properties.
            /// </summary>
            /// <param name="key">The key of the property.</param>
            /// <param name="value">The value of the property.</param>
            /// <returns>The same <see cref="Builder"/> instance.</returns>
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

            /// <summary>
            /// Adds a float value to the custom properties.
            /// </summary>
            /// <param name="key">The key of the property.</param>
            /// <param name="value">The value of the property.</param>
            /// <returns>The same <see cref="Builder"/> instance.</returns>
            public Builder AddFloat(string key, float value)
            {
                Properties[key] = value;
                return this;
            }

            /// <summary>
            /// Adds a double value to the custom properties.
            /// </summary>
            /// <param name="key">The key of the property.</param>
            /// <param name="value">The value of the property.</param>
            /// <returns>The same <see cref="Builder"/> instance.</returns>
            public Builder AddDouble(string key, double value)
            {
                Properties[key] = value;
                return this;
            }

            /// <summary>
            /// Adds a string value to the custom properties.
            /// </summary>
            /// <param name="key">The key of the property.</param>
            /// <param name="value">The value of the property.</param>
            /// <returns>The same <see cref="Builder"/> instance.</returns>
            public Builder AddString(string key, string value)
            {
                Properties[key] = value;
                return this;
            }

            /// <summary>
            /// Adds a string set value to the custom properties.
            /// </summary>
            /// <param name="key">The key of the property.</param>
            /// <param name="value">The value of the property.</param>
            /// <returns>The same <see cref="Builder"/> instance.</returns>
            public Builder AddStringSet(string key, IEnumerable<string> value)
            {
                Properties[key] = value;
                return this;
            }

            /// <summary>
            /// Builds a new <see cref="CustomProperties"/> instance with all
            /// the key-value pairs specified so far.
            /// </summary>
            /// <returns>A new <see cref="CustomProperties"/> instance.</returns>
            public CustomProperties Build()
            {
                return new CustomProperties(Properties);
            }
        }
    }
}