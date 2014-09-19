using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 5949.
    /// <pre>
    /// +-----+-------------+--------------+
    /// | Bit | 0-7         | 8-15         |
    /// +-----+-------------+--------------+
    /// | 0   | Option Type | Opt Data Len |
    /// +-----+-------------+--------------+
    /// | 16  | Request 1                  |
    /// | ... | Request 2                  |
    /// |     | ...                        |
    /// |     | Request n                  |
    /// +-----+----------------------------+
    /// </pre>
    /// </summary>
    [IpV6MobilityOptionTypeRegistration(IpV6MobilityOptionType.ContextRequest)]
    public sealed class IpV6MobilityOptionContextRequest : IpV6MobilityOptionComplex
    {
        /// <summary>
        /// Creates an instance from an array of requests.
        /// </summary>
        /// <param name="requests">The requests types and options.</param>
        public IpV6MobilityOptionContextRequest(params IpV6MobilityOptionContextRequestEntry[] requests)
            : this(requests.AsReadOnly())
        {
        }

        /// <summary>
        /// Creates an instance from a list of requests.
        /// </summary>
        /// <param name="requests">The requests types and options.</param>
        public IpV6MobilityOptionContextRequest(IList<IpV6MobilityOptionContextRequestEntry> requests)
            : this(requests.AsReadOnly())
        {
        }

        /// <summary>
        /// Creates an instance from a collection of requests.
        /// </summary>
        /// <param name="requests">The requests types and options.</param>
        public IpV6MobilityOptionContextRequest(ReadOnlyCollection<IpV6MobilityOptionContextRequestEntry> requests)
            : base(IpV6MobilityOptionType.ContextRequest)
        {
            Requests = requests;
            _dataLength = Requests.Sum(request => request.Length);
            if (_dataLength > byte.MaxValue)
                throw new ArgumentOutOfRangeException("requests", requests, string.Format("requests length is too large. Takes over {0}>{1} bytes.", _dataLength, byte.MaxValue));
        }

        /// <summary>
        /// The requests types and options.
        /// </summary>
        public ReadOnlyCollection<IpV6MobilityOptionContextRequestEntry> Requests { get; private set; }

        internal override IpV6MobilityOption CreateInstance(DataSegment data)
        {
            List<IpV6MobilityOptionContextRequestEntry> requests = new List<IpV6MobilityOptionContextRequestEntry>();
            int offset = 0;
            while (data.Length > offset)
            {
                byte requestType = data[offset++];

                if (offset >= data.Length)
                    return null;
                byte requestLength = data[offset++];

                if (offset + requestLength > data.Length)
                    return null;

                DataSegment requestOption = data.Subsegment(offset, requestLength);
                offset += requestLength;

                requests.Add(new IpV6MobilityOptionContextRequestEntry(requestType, requestOption));
            }

            return new IpV6MobilityOptionContextRequest(requests);
        }

        internal override int DataLength
        {
            get { return _dataLength; }
        }

        internal override bool EqualsData(IpV6MobilityOption other)
        {
            return EqualsData(other as IpV6MobilityOptionContextRequest);
        }

        internal override int GetDataHashCode()
        {
            return Requests.SequenceGetHashCode();
        }

        internal override void WriteData(byte[] buffer, ref int offset)
        {
            foreach (var request in Requests)
                request.Write(buffer, ref offset);
        }

        private IpV6MobilityOptionContextRequest()
            : this(new IpV6MobilityOptionContextRequestEntry[0])
        {
        }

        private bool EqualsData(IpV6MobilityOptionContextRequest other)
        {
            return other != null &&
                   Requests.SequenceEqual(other.Requests);
        }

        private readonly int _dataLength;
    }
}