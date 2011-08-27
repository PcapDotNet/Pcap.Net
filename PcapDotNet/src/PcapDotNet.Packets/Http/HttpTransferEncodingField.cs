using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using PcapDotNet.Base;

namespace PcapDotNet.Packets.Http
{
    /// <summary>
    /// RFC 2616.
    /// The Transfer-Encoding general-header field indicates what (if any) type of transformation has been applied to the message body 
    /// in order to safely transfer it between the sender and the recipient. 
    /// This differs from the content-coding in that the transfer-coding is a property of the message, not of the entity.
    /// 
    /// <pre>
    /// Transfer-Encoding       = "Transfer-Encoding" ":" 1#transfer-coding
    /// </pre>
    /// 
    /// Example:
    /// 
    /// <pre>
    /// Transfer-Encoding: chunked
    /// </pre>
    /// 
    /// If multiple encodings have been applied to an entity, the transfer-codings MUST be listed in the order in which they were applied.
    /// Additional information about the encoding parameters MAY be provided by other entity-header fields not defined by this specification.
    /// </summary>
    public sealed class HttpTransferEncodingField : HttpField, IEquatable<HttpTransferEncodingField>
    {
        /// <summary>
        /// The field name.
        /// </summary>
        public const string FieldName = "Transfer-Encoding";

        /// <summary>
        /// The field name in uppercase.
        /// </summary>
        public const string FieldNameUpper = "TRANSFER-ENCODING";

        private const string RegexTransferCodingGroupName = "TransferCoding";

        /// <summary>
        /// Creates an HTTP transfer encoding field from a set of transfer codings.
        /// </summary>
        public HttpTransferEncodingField(IList<string> transferCodings)
            :base(FieldName, transferCodings.SequenceToString(","))
        {
            SetTransferCodings(transferCodings);
        }

        /// <summary>
        /// Creates an HTTP transfer encoding field from a set of transfer codings.
        /// </summary>
        public HttpTransferEncodingField(params string[] transferCodings)
            :this((IList<string>)transferCodings)
        {
        }

        /// <summary>
        /// Transfer-coding values are used to indicate an encoding transformation that has been, can be, 
        /// or may need to be applied to an entity-body in order to ensure "safe transport" through the network.
        /// This differs from a content coding in that the transfer-coding is a property of the message, not of the original entity.
        /// 
        /// <pre>
        /// transfer-coding         = "chunked" | transfer-extension
        /// transfer-extension      = token *( ";" parameter )
        /// </pre>
        /// 
        /// Parameters are in the form of attribute/value pairs.
        /// 
        /// <pre>
        /// parameter               = attribute "=" value
        /// attribute               = token
        /// value                   = token | quoted-string
        /// </pre>
        /// 
        /// All transfer-coding values are case-insensitive. 
        /// Whenever a transfer-coding is applied to a message-body, the set of transfer-codings MUST include "chunked", 
        /// unless the message is terminated by closing the connection. 
        /// When the "chunked" transfer-coding is used, it MUST be the last transfer-coding applied to the message-body. 
        /// The "chunked" transfer-coding MUST NOT be applied more than once to a message-body. 
        /// These rules allow the recipient to determine the transfer-length of the message.
        /// </summary>
        public ReadOnlyCollection<string> TransferCodings { get { return _transferCodings; } }

        /// <summary>
        /// True iff the two HTTP transfer encoding fields are of equal value.
        /// Two HTTP transfer encoding fields are equal iff they have the same transfer codings.
        /// </summary>
        public bool Equals(HttpTransferEncodingField other)
        {
            return other != null &&
                    (ReferenceEquals(TransferCodings, other.TransferCodings) ||
                    TransferCodings != null && other.TransferCodings != null && TransferCodings.SequenceEqual(other.TransferCodings));
        }

        /// <summary>
        /// True iff the two HTTP transfer encoding fields are of equal value.
        /// Two HTTP transfer encoding fields are equal iff they have the same transfer codings.
        /// </summary>
        public override bool Equals(HttpField other)
        {
            return Equals(other as HttpTransferEncodingField);
        }

        internal HttpTransferEncodingField(byte[] fieldValue)
            : base(FieldName, fieldValue)
        {
            string fieldValueString = HttpRegex.GetString(fieldValue);
            Match match = _regex.Match(fieldValueString);
            if (!match.Success)
                return;

            SetTransferCodings(match.GroupCapturesValues(RegexTransferCodingGroupName).ToArray());
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase")]
        private void SetTransferCodings(IList<string> transferCodings)
        {
            if (transferCodings.Any(coding => coding.Any(c => c.IsUppercaseAlpha())))
                _transferCodings = transferCodings.Select(coding => coding.ToLowerInvariant()).ToArray().AsReadOnly();
            else
                _transferCodings = transferCodings.AsReadOnly();
        }

        private ReadOnlyCollection<string> _transferCodings;

        private static readonly Regex _transferExtensionRegex = HttpRegex.Concat(HttpRegex.Token, HttpRegex.OptionalParameters);
        private static readonly Regex _transferCodingRegex = HttpRegex.Capture(HttpRegex.Or(HttpRegex.Build("chunked"), _transferExtensionRegex), RegexTransferCodingGroupName);
        private static readonly Regex _regex = HttpRegex.MatchEntire(HttpRegex.CommaSeparatedRegex(_transferCodingRegex, 1));
    }
}