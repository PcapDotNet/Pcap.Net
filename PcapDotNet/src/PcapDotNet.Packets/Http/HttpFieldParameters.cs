using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace PcapDotNet.Packets.Http
{
    public class HttpFieldParameters : IEnumerable<KeyValuePair<string, string>>
    {
        internal HttpFieldParameters(IEnumerable<string> parametersNames, IEnumerable<string> parametersValues)
        {
            var nameEnumerator = parametersNames.GetEnumerator();
            var valueEnumerator = parametersValues.GetEnumerator();
            while (nameEnumerator.MoveNext())
            {
                if (!valueEnumerator.MoveNext())
                    throw new ArgumentException(string.Format("more names ({0}) were given than values ({1})", parametersNames.Count(), parametersValues.Count()), "parametersValues");

                _parameters.Add(nameEnumerator.Current, valueEnumerator.Current);
            }
            if (valueEnumerator.MoveNext())
                throw new ArgumentException(string.Format("more values ({0}) were given than names ({1})", parametersValues.Count(), parametersNames.Count()), "parametersNames");
        }

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

        public IEnumerator<KeyValuePair<string, string>> GetEnumerator()
        {
            return _parameters.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly Dictionary<string, string> _parameters = new Dictionary<string, string>();
    }
}