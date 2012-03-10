namespace PcapDotNet.Packets.Dns
{
    /// <summary>
    /// http://files.dns-sd.org/draft-sekar-dns-llq.txt.
    /// </summary>
    public enum DnsLongLivedQueryOpCode : ushort
    {
        /// <summary>
        /// Undefined value.
        /// </summary>
        None = 0,

        /// <summary>
        /// An LLQ is initiated by a client, and is completed via a four-way handshake. 
        /// This handshake provides resilience to packet loss, demonstrates client reachability, and reduces denial of service attack opportunities.
        /// </summary>
        Setup = 1,

        /// <summary>
        /// If the client desires to maintain the LLQ beyond the duration specified in the LEASE-LIFE field of the Request Acknowledgment,
        /// the client must send a Refresh Request.
        /// A Refresh Request is identical to an LLQ Challenge Response, but with the LLQ-OPCODE set to LLQ-REFRESH.
        /// Unlike a Challenge Response, a Refresh Request returns no answers.
        /// </summary>
        Refresh = 2,

        /// <summary>
        /// When a change ("event") occurs to a name server's zone, the server must check if the new or deleted resource records answer any LLQs.
        /// If so, the resource records must be sent to the LLQ requesters in the form of a gratuitous DNS response sent to the client,
        /// with the question(s) being answered in the Question section, and answers to these questions in the Answer section.
        /// The response also includes an OPT RR as the last record in the Additional section.
        /// This OPT RR contains, in its RDATA, an entry for each LLQ being answered in the message.
        /// Entries must include the LLQ-ID. This reduces the potential for spoof events being sent to a client.
        /// </summary>
        Event = 3,
    }
}