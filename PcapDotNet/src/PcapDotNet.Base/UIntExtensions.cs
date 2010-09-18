using System;

namespace PcapDotNet.Base
{
    /// <summary>
    /// Extension method for UInt structure.
    /// </summary>
    public static class UIntExtensions
    {
        /// <summary>
        /// Returns the number of digits the number will be represented by according to a specific base.
        /// </summary>
        /// <param name="value">The number to check for number of digits.</param>
        /// <param name="digitsBase">The base of the digits.</param>
        /// <returns>The number of digits the number will be represented by according to a specific base.</returns>
        public static int DigitsCount(this uint value, double digitsBase)
        {
            return (int)(Math.Floor(Math.Log(value, digitsBase)) + 1);
        }
    }
}