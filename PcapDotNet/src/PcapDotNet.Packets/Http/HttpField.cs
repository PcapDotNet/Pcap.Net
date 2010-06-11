using System;
using System.Collections.Generic;

namespace PcapDotNet.Packets.Http
{
    public class HttpField : IEquatable<HttpField>
    {
        public static HttpField Create(string name, IEnumerable<byte> value)
        {
            switch (name)
            {
                // general-header
                case "Cache-Control":
                    return new HttpCommaSeparatedField(name, value);
                case "Connection":
                case "Date":
                case "Pragma":
                case "Trailer":
                case "Transfer-Encoding":
                case "Upgrade":
                case "Via":
                case "Warning":
                    break;
            }

            return new HttpField(name);
        }

        public string Name { get; private set; }

        public bool Equals(HttpField other)
        {
            return other != null && Name.Equals(other.Name);
        }

        public override bool Equals(object obj)
        {
            return Equals(obj as HttpField);
        }

        public override string ToString()
        {
            return string.Format("{0}: ", Name);
        }

        internal HttpField(string name)
        {
            Name = name;
        }
    }
}