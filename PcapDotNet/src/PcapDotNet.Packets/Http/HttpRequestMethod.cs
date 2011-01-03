using System;
using System.Collections.Generic;
using System.Text;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// HTTP request method.
    /// Example: GET
    /// </summary>
    public class HttpRequestMethod : IEquatable<HttpRequestMethod>
    {
        /// <summary>
        /// Creates a method from a method string.
        /// </summary>
        public HttpRequestMethod(string method)
        {
            Method = method;
        }
        
        /// <summary>
        /// Creates a method from a known method.
        /// </summary>
        public HttpRequestMethod(HttpRequestKnownMethod method)
        {
            Method = method.ToString().ToUpperInvariant();
            if (!_knownMethods.ContainsKey(Method))
                throw new ArgumentException("Invalid known request method given: " + method, "method");
        }

        /// <summary>
        /// The method string.
        /// </summary>
        public string Method { get; private set; }

        /// <summary>
        /// Returns the known method that matches the method string.
        /// Returns HttpRequestKnownMethod.Unknown if no matching known method could be found.
        /// </summary>
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

        /// <summary>
        /// The number of bytes this method takes.
        /// </summary>
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

        /// <summary>
        /// Two methods are equal iff they have the same method string.
        /// </summary>
        public bool Equals(HttpRequestMethod other)
        {
            return other != null && Method.Equals(other.Method);
        }

        /// <summary>
        /// Two methods are equal iff they have the same method string.
        /// </summary>
        public override bool Equals(object obj)
        {
            return Equals(obj as HttpRequestMethod);
        }

        /// <summary>
        /// The hash code of the method string.
        /// </summary>
        public override int GetHashCode()
        {
            return Method.GetHashCode();
        }

        private static readonly Dictionary<string, HttpRequestKnownMethod> _knownMethods = CreateKnownMethodsTable();
    }
}