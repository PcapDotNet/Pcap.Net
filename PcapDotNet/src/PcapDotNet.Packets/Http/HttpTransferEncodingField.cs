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
        public const string NameLower = "transfer-encoding";
        private const string RegexTransferCodingGroupName = "TransferCoding";

        public HttpTransferEncodingField(IList<string> transferCodings)
            :base(Name, transferCodings.SequenceToString(","))
        {
            SetTransferCodings(transferCodings);
        }

        public HttpTransferEncodingField(params string[] transferCodings)
            :this((IList<string>)transferCodings)
        {
        }

        public bool Equals(HttpTransferEncodingField other)
        {
            return other != null &&
                    (ReferenceEquals(TransferCodings, other.TransferCodings) ||
                    TransferCodings != null && other.TransferCodings != null && TransferCodings.SequenceEqual(other.TransferCodings));
        }

        public ReadOnlyCollection<string> TransferCodings{get{return _transferCodings;}}
        
        private void SetTransferCodings(IList<string> transferCodings)
        {
            if (transferCodings.Any(coding => coding.Any(c => c.IsUppercaseAlpha())))
                _transferCodings = transferCodings.Select(coding => coding.ToLowerInvariant()).ToArray().AsReadOnly();
            else
                _transferCodings = transferCodings.AsReadOnly();
        }

        public override bool Equals(HttpField other)
        {
            return Equals(other as HttpTransferEncodingField);
        }

        internal HttpTransferEncodingField(byte[] fieldValue)
            : base(Name, fieldValue)
        {
            string fieldValueString = HttpRegex.GetString(fieldValue);
            Match match = _regex.Match(fieldValueString);
            if (!match.Success)
                return;

            SetTransferCodings(match.GroupCapturesValues(RegexTransferCodingGroupName).ToArray());
        }

        private ReadOnlyCollection<string> _transferCodings;

        private static readonly Regex _transferExtensionRegex = HttpRegex.Concat(HttpRegex.Token, HttpRegex.OptionalParameters);
        private static readonly Regex _transferCodingRegex = HttpRegex.Capture(HttpRegex.Or(HttpRegex.Build("chunked"), _transferExtensionRegex), RegexTransferCodingGroupName);
        private static readonly Regex _regex = HttpRegex.MatchEntire(HttpRegex.CommaSeparatedRegex(_transferCodingRegex, 1));
    }
}