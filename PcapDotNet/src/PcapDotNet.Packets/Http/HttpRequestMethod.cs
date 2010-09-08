using System;
using System.Collections.Generic;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    public class HttpRequestMethod : IEquatable<HttpRequestMethod>
    {
        public HttpRequestMethod(string method)
        {
            Method = method;
        }
        
        public HttpRequestMethod(HttpRequestKnownMethod method)
        {
            Method = method.ToString().ToUpper();
            if (!_knownMethods.ContainsKey(Method))
                throw new ArgumentException("Invalid known request method given: " + method, "method");
        }

        public string Method { get; private set; }

        public HttpRequestKnownMethod KnownMethod
        {
            get
            {
                HttpRequestKnownMethod knownMethod;
                if (_knownMethods.TryGetValue(Method, out knownMethod))
                    return knownMethod;

                return HttpRequestKnownMethod.Unknown;
            }
        }

        public int Length
        {
            get { return Method.Length; }
        }

        internal void Write(byte[] buffer, ref int offset)
        {
            buffer.Write(ref offset, Method, Encoding.ASCII);
        }

        private static Dictionary<string, HttpRequestKnownMethod> CreateKnownMethodsTable()
        {
            Dictionary<string, HttpRequestKnownMethod> result = new Dictionary<string, HttpRequestKnownMethod>();
            foreach (HttpRequestKnownMethod method in typeof(HttpRequestKnownMethod).GetEnumValues<HttpRequestKnownMethod>())
            {
                if (method != HttpRequestKnownMethod.Unknown)
                    result.Add(method.ToString().ToUpperInvariant(), method);
            }
            return result;
        }

        public bool Equals(HttpRequestMethod other)
        {
            return other != null && Method.Equals(other.Method);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HttpRequestMethod);
        }

        private static readonly Dictionary<string, HttpRequestKnownMethod> _knownMethods = CreateKnownMethodsTable();
    }
}