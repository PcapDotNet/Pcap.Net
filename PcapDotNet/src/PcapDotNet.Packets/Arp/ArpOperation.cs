namespace PcapDotNet.Packets.Arp
{
    /// <summary>
    /// Specifies the operation the ARP sender is performing.
    /// </summary>
    public enum ArpOperation : ushort
    {
        /// <summary>
        /// Invalid operation.
        /// </summary>
        None = 0,

        /// <summary>
        /// [RFC826][RFC5227]
        /// </summary>
        Request = 1,

        /// <summary>
        /// [RFC826][RFC5227]
        /// </summary>
        Reply = 2,

        /// <summary>
        /// [RFC903]
        /// </summary>
        ReverseRequest = 3,

        /// <summary>
        /// [RFC903]
        /// </summary>
        ReverseReply = 4,

        /// <summary>
        /// [RFC1931]
        /// </summary>
        DynamicReverseRequest = 5,

        /// <summary>
        /// [RFC1931]
        /// </summary>
        DynamicReverseReply = 6,

        /// <summary>
        /// [RFC1931]
        /// </summary>
        DynamicReverseError = 7,

        /// <summary>
        /// [RFC1293]
        /// </summary>
        InverseRequest = 8,

        /// <summary>
        /// [RFC1293]
        /// </summary>
        InverseReply = 9,

        /// <summary>
        /// [RFC1577]
        /// </summary>
        NegativeAtmReply = 10,

        //MARS-Request          = 11       
        //MARS-Multi            = 12       
        //MARS-MServ            = 13       
        //MARS-Join             = 14       
        //MARS-Leave            = 15       
        //MARS-NAK              = 16       
        //MARS-Unserv           = 17       
        //MARS-SJoin            = 18       
        //MARS-SLeave           = 19       
        //MARS-Grouplist-Request= 20       
        //MARS-Grouplist-Reply  = 21       
        //MARS-Redirect-Map     = 22       

        /// <summary>
        /// [RFC2176]
        /// </summary>
        MultipleAccessOverSynchronousOpticalNetworkingOrSynchronousDigitalHierarchyUnsolicitedArp  = 23,

        /// <summary>
        /// [RFC5494]
        /// </summary>
        Experimental1 = 24,

        /// <summary>
        /// [RFC5494]
        /// </summary>
        Experimental2 = 25
    }
}