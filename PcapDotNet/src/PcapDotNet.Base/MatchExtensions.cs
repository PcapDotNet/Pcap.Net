using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Linq;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension methods for Match class.
    /// </summary>
    public static class MatchExtensions
    {
        /// <summary>
        /// Returns all the values that were captured for a given group name.
        /// </summary>
        /// <param name="match">The match to take the captured values from.</param>
        /// <param name="groupName">The name of the capture group to take the values of.</param>
        /// <returns>All the values that were captured for a given group name.</returns>
        public static IEnumerable<string> GroupCapturesValues(this Match match, string groupName)
        {
            if (match == null)
                throw new ArgumentNullException("match");

            return match.Groups[groupName].Captures.Cast<Capture>().Select(capture => capture.Value);
        }
    }
}