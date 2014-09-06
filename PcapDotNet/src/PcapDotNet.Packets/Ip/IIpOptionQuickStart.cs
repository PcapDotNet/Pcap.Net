using PcapDotNet.Packets.IpV4;

namespace PcapDotNet.Packets.Ip
{
    /// <summary>
    /// RFC 4782.
    /// Common interface of a quick start option.
    /// </summary>
    public interface IIpOptionQuickStart
    {
        /// <summary>
        /// The function of this quick start option.
        /// </summary>
        IpV4OptionQuickStartFunction Function { get; }

        /// <summary>
        /// If function is request then this field is the rate request.
        /// If function is report then this field is the rate report.
        /// </summary>
        byte Rate { get; }

        /// <summary>
        /// The rate translated to KBPS.
        /// </summary>
        int RateKbps { get; }

        /// <summary>
        /// The Quick-Start TTL (QS TTL) field.  
        /// The sender MUST set the QS TTL field to a random value.
        /// Routers that approve the Quick-Start Request decrement the QS TTL (mod 256) by the same amount that they decrement the IP TTL.  
        /// The QS TTL is used by the sender to detect if all the routers along the path understood and approved the Quick-Start option.
        /// 
        /// <para>
        ///   For a Rate Request, the transport sender MUST calculate and store the TTL Diff, 
        ///   the difference between the IP TTL value, and the QS TTL value in the Quick-Start Request packet, as follows:
        ///   TTL Diff = ( IP TTL - QS TTL ) mod 256                 
        /// </para>
        /// </summary>
        byte Ttl { get; }

        /// <summary>
        /// The QS Nonce gives the Quick-Start sender some protection against receivers lying about the value of the received Rate Request. 
        /// This is particularly important if the receiver knows the original value of the Rate Request 
        /// (e.g., when the sender always requests the same value, and the receiver has a long history of communication with that sender).  
        /// Without the QS Nonce, there is nothing to prevent the receiver from reporting back to the sender a Rate Request of K, 
        /// when the received Rate Request was, in fact, less than K.
        /// 
        /// <para>
        ///   The format for the 30-bit QS Nonce.
        ///   <list type="table">
        ///     <listheader>
        ///         <term>Bits</term>
        ///         <description>Purpose</description>
        ///     </listheader>
        ///     <item><term>Bits 0-1</term><description>Rate 15 -> Rate 14</description></item>
        ///     <item><term>Bits 2-3</term><description>Rate 14 -> Rate 13</description></item>
        ///     <item><term>Bits 4-5</term><description>Rate 13 -> Rate 12</description></item>
        ///     <item><term>Bits 6-7</term><description>Rate 12 -> Rate 11</description></item>
        ///     <item><term>Bits 8-9</term><description>Rate 11 -> Rate 10</description></item>
        ///     <item><term>Bits 10-11</term><description>Rate 10 -> Rate 9</description></item>
        ///     <item><term>Bits 12-13</term><description>Rate 9 -> Rate 8</description></item>
        ///     <item><term>Bits 14-15</term><description>Rate 8 -> Rate 7</description></item>
        ///     <item><term>Bits 16-17</term><description>Rate 7 -> Rate 6</description></item>
        ///     <item><term>Bits 18-19</term><description>Rate 6 -> Rate 5</description></item>
        ///     <item><term>Bits 20-21</term><description>Rate 5 -> Rate 4</description></item>
        ///     <item><term>Bits 22-23</term><description>Rate 4 -> Rate 3</description></item>
        ///     <item><term>Bits 24-25</term><description>Rate 3 -> Rate 2</description></item>
        ///     <item><term>Bits 26-27</term><description>Rate 2 -> Rate 1</description></item>
        ///     <item><term>Bits 28-29</term><description>Rate 1 -> Rate 0</description></item>
        ///   </list>
        /// </para>
        /// 
        /// <para>
        /// The transport sender MUST initialize the QS Nonce to a random value. 
        /// If the router reduces the Rate Request from rate K to rate K-1, 
        /// then the router MUST set the field in the QS Nonce for "Rate K -> Rate K-1" to a new random value.  
        /// Similarly, if the router reduces the Rate Request by N steps, 
        /// the router MUST set the 2N bits in the relevant fields in the QS Nonce to a new random value.  
        /// The receiver MUST report the QS Nonce back to the sender.
        /// </para>
        /// 
        /// <para>
        /// If the Rate Request was not decremented in the network, then the QS Nonce should have its original value.  
        /// Similarly, if the Rate Request was decremented by N steps in the network, 
        /// and the receiver reports back a Rate Request of K, then the last 2K bits of the QS Nonce should have their original value.
        /// </para>
        /// </summary>
        uint Nonce { get; }
    }
}