using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// Represents a set of HTTP field parameters.
    /// Used for some of HTTP fields.
    /// All parameters must have different names.
    /// </summary>
    public sealed class HttpFieldParameters : IEnumerable<KeyValuePair<string, string>>, IEquatable<HttpFieldParameters>
    {
        /// <summary>
        /// Creates the parameters from an array of parameters. Keys are the parameters names and values are the parameters values.
        /// </summary>
        public HttpFieldParameters(params KeyValuePair<string, string>[] parameters)
            :this((IEnumerable<KeyValuePair<string, string>>)parameters)
        {
        }


        /// <summary>
        /// Creates the parameters from an enumerable of parameters. Keys are the parameters names and values are the parameters values.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1006:DoNotNestGenericTypesInMemberSignatures")]
        public HttpFieldParameters(IEnumerable<KeyValuePair<string, string>> parameters)
        {
            _parameters = parameters.ToDictionary(pair => pair.Key, pair => pair.Value);
        }

        /// <summary>
        /// Number of parameters.
        /// </summary>
        public int Count { get { return _parameters.Count; } }

        /// <summary>
        /// Returns the value of the given parameter name.
        /// </summary>
        /// <param name="name">The name of the parameter.</param>
        /// <returns>The value of the parameter.</returns>
        public string this[string name]
        {
            get
            {
                string value;
                if (!_parameters.TryGetValue(name, out value))
                    return null;
                return value;
            }
        }

        /// <summary>
        /// Enumerates over the parameters.
        /// </summary>
        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        /// <summary>
        /// Two HTTP field parameters are equal if all of their parameters are equal.
        /// </summary>
        public bool Equals(HttpFieldParameters other)
        {
            return other != null && _parameters.DictionaryEquals(other._parameters);
        }

        /// <summary>
        /// Two HTTP field parameters are equal if all of their parameters are equal.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as HttpFieldParameters);
        }

        /// <summary>
        /// Xor of all of the hash codes of the parameters names and values.
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            return
                _parameters.Select(pair => new KeyValuePair<string, string>(pair.Key.ToUpperInvariant(), pair.Value))
                    .Xor(pair => Sequence.GetHashCode(pair.Key, pair.Value));
        }

        /// <summary>
        /// Returns a string of parameters beginning and separated by semicolon and equal sign between keys and values.
        /// </summary>
        public override string ToString()
        {
            if (!this.Any())
                return string.Empty;

            StringBuilder stringBuilder = new StringBuilder();
            foreach (var parameter in this)
            {
                stringBuilder.Append(";");
                stringBuilder.Append(parameter.Key);
                stringBuilder.Append("=");
                stringBuilder.Append(parameter.Value);
            }

            return stringBuilder.ToString();
        }

        internal HttpFieldParameters(IEnumerable<string> parametersNames, IEnumerable<string> parametersValues)
        {
            var nameEnumerator = parametersNames.GetEnumerator();
            var valueEnumerator = parametersValues.GetEnumerator();
            while (nameEnumerator.MoveNext())
            {
                if (!valueEnumerator.MoveNext())
                    throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "more names ({0}) were given than values ({1})", parametersNames.Count(), parametersValues.Count()), "parametersValues");

                _parameters.Add(nameEnumerator.Current, valueEnumerator.Current);
            }
            if (valueEnumerator.MoveNext())
                throw new ArgumentException(string.Format(CultureInfo.InvariantCulture, "more values ({0}) were given than names ({1})", parametersValues.Count(), parametersNames.Count()), "parametersNames");
        }
        
        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();
    }
}