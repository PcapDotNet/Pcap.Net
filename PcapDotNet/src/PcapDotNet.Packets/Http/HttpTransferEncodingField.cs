using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    public class HttpTransferEncodingField : HttpField, IEquatable<HttpTransferEncodingField>
    {
        public const string Name = "Transfer-Encoding";
        private const string RegexTransferCodingGroupName = "TransferCoding";

        public HttpTransferEncodingField(IList<string> transferCodings)
            :base(Name, transferCodings.SequenceToString(","))
        {
            TransferCodings = transferCodings.AsReadOnly();
        }

        public HttpTransferEncodingField(params string[] transferCodings)
            :this((IList<string>)transferCodings)
        {
        }

            internal HttpTransferEncodingField(byte[] fieldValue)
            : base(Name, fieldValue)
        {
            string fieldValueString = HttpRegex.GetString(fieldValue);
            Match match = _regex.Match(fieldValueString);
            if (!match.Success)
                return;

            TransferCodings = match.Groups[RegexTransferCodingGroupName].Captures.Cast<Capture>().Select(capture => capture.Value).ToArray().AsReadOnly();
        }

            public bool Equals(HttpTransferEncodingField other)
            {
                return other != null &&
                       (ReferenceEquals(TransferCodings, other.TransferCodings) ||
                        TransferCodings != null && other.TransferCodings != null && TransferCodings.SequenceEqual(other.TransferCodings));
            }

        public ReadOnlyCollection<string> TransferCodings { get; private set; }

        public override bool Equals(HttpField other)
        {
            return Equals(other as HttpTransferEncodingField);
        }

        protected override string ValueToString()
        {
            return TransferCodings == null ? string.Empty : TransferCodings.SequenceToString(",");
        }

        private static readonly Regex _valueRegex = HttpRegex.Or(HttpRegex.Token, HttpRegex.QuotedString);
        private static readonly Regex _attributeRegex = HttpRegex.Token;
        private static readonly Regex _parameterRegex = HttpRegex.Concat(_attributeRegex, HttpRegex.Build("="), _valueRegex);
        private static readonly Regex _transferExtensionRegex = HttpRegex.Concat(HttpRegex.Token, HttpRegex.Any(HttpRegex.Concat(HttpRegex.Build(";"), _parameterRegex)));
        private static readonly Regex _transferCodingRegex = HttpRegex.Capture(HttpRegex.Or(HttpRegex.Build("chunked"), _transferExtensionRegex), RegexTransferCodingGroupName);
        private static readonly Regex _regex = HttpRegex.MatchEntire(HttpRegex.CommaSeparatedRegex(_transferCodingRegex, 1));
    }
}