namespace PcapDotNet.Packets.IpV6
{
    /// <summary>
    /// RFC 2711.
    /// </summary>
    public enum IpV6RouterAlertType : ushort
    {
        /// <summary>
        /// RFC 2710.
        /// Datagram contains a Multicast Listener Discovery message.
        /// </summary>
        MulticastListenerDiscovery = 0x0000,

        /// <summary>
        /// RFC 2711.
        /// Datagram contains RSVP message.
        /// </summary>
        Rsvp = 0x0001,

        /// <summary>
        /// RFC 2711.
        /// Datagram contains an Active Networks message.
        /// </summary>
        ActiveNetwork = 0x0002,

        /// <summary>
        /// RFC 3175.
        /// 1 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth1 = 0x0004,

        /// <summary>
        /// RFC 3175.
        /// 2 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth2 = 0x0005,

        /// <summary>
        /// RFC 3175.
        /// 3 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth3 = 0x0006,

        /// <summary>
        /// RFC 3175.
        /// 4 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth4 = 0x0007,

        /// <summary>
        /// RFC 3175.
        /// 5 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth5 = 0x0008,

        /// <summary>
        /// RFC 3175.
        /// 6 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth6 = 0x0009,

        /// <summary>
        /// RFC 3175.
        /// 7 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth7 = 0x000A,

        /// <summary>
        /// RFC 3175.
        /// 8 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth8 = 0x000B,

        /// <summary>
        /// RFC 3175.
        /// 9 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth9 = 0x000C,

        /// <summary>
        /// RFC 3175.
        /// 10 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth10 = 0x000D,

        /// <summary>
        /// RFC 3175.
        /// 11 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth11 = 0x000E,

        /// <summary>
        /// RFC 3175.
        /// 12 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth12 = 0x000F,

        /// <summary>
        /// RFC 3175.
        /// 13 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth13 = 0x0010,


        /// <summary>
        /// RFC 3175.
        /// 14 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth14 = 0x0011,

        /// <summary>
        /// RFC 3175.
        /// 15 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth15 = 0x0012,

        /// <summary>
        /// RFC 3175.
        /// 16 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth16 = 0x0013,

        /// <summary>
        /// RFC 3175.
        /// 17 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth17 = 0x0014,

        /// <summary>
        /// RFC 3175.
        /// 18 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth18 = 0x0015,

        /// <summary>
        /// RFC 3175.
        /// 19 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth19 = 0x0016,

        /// <summary>
        /// RFC 3175.
        /// 20 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth20 = 0x0017,

        /// <summary>
        /// RFC 3175.
        /// 21 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth21 = 0x0018,

        /// <summary>
        /// RFC 3175.
        /// 22 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth22 = 0x0019,

        /// <summary>
        /// RFC 3175.
        /// 23 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth23 = 0x001A,

        /// <summary>
        /// RFC 3175.
        /// 24 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth24 = 0x001B,

        /// <summary>
        /// RFC 3175.
        /// 25 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth25 = 0x001C,

        /// <summary>
        /// RFC 3175.
        /// 26 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth26 = 0x001D,

        /// <summary>
        /// RFC 3175.
        /// 27 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth27 = 0x001E,

        /// <summary>
        /// RFC 3175.
        /// 28 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth28 = 0x001F,

        /// <summary>
        /// RFC 3175.
        /// 29 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth29 = 0x0020,

        /// <summary>
        /// RFC 3175.
        /// 30 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth30 = 0x0021,

        /// <summary>
        /// RFC 3175.
        /// 31 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth31 = 0x0022,

        /// <summary>
        /// RFC 3175.
        /// 32 depth of nesting of aggregation.
        /// </summary>
        AggregationNestingDepth32 = 0x0023,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 0.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel0 = 0x0024,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 1.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel1 = 0x0025,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 2.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel2 = 0x0026,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 3.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel3 = 0x0027,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 4.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel4 = 0x0028,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 5.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel5 = 0x0029,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 6.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel6 = 0x002A,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 7.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel7 = 0x002B,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 8.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel8 = 0x002C,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 9.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel9 = 0x002D,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 10.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel10 = 0x002E,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 11.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel11 = 0x002F,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 12.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel12 = 0x0030,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 13.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel13 = 0x0031,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 14.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel14 = 0x0032,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 15.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel15 = 0x0033,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 16.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel16 = 0x0034,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 17.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel17 = 0x0035,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 18.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel18 = 0x0036,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 19.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel19 = 0x0037,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 20.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel20 = 0x0038,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 21.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel21 = 0x0039,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 22.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel22 = 0x003A,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 23.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel23 = 0x003B,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 24.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel24 = 0x003C,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 25.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel25 = 0x003D,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 26.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel26 = 0x003E,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 27.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel27 = 0x003F,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 28.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel28 = 0x0040,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 29.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel29 = 0x0041,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 30.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel30 = 0x0042,

        /// <summary>
        /// RFC 5974.
        /// QualityOfService NSLP Aggregation Level 31.
        /// </summary>
        QualityOfServiceNextStepsInSignalingLayerProtocolAggregationLevel31 = 0x0043,

        /// <summary>
        /// RFC 5973.
        /// NSIS NATFW NSLP.
        /// </summary>
        NextStepsInSignalingNatFirewallLayerProtocol = 0x0044,
    }
}