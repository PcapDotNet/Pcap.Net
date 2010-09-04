using System;

namespace PcapDotNet.Base
{
    public static class UIntExtensions
    {
        public static int NumDigits(this uint value, double digitsBase)
        {
            return (int)(Math.Floor(Math.Log(value, digitsBase)) + 1);
        }
    }
}