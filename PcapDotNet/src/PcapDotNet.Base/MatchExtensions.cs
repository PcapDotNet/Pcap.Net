using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace PcapDotNet.Base
{
    public static class MatchExtensions
    {
        public static IEnumerable<string> GroupCapturesValues(this Match match, string groupName)
        {
            return match.Groups[groupName].Captures.Cast<Capture>().Select(capture => capture.Value);
        }
    }
}