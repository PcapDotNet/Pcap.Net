namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// Group Record Type.
    /// </summary>
    public enum IgmpRecordType : byte
    {
        /// <summary>
        /// Illegal record type.
        /// </summary>
        None = 0,

        /// <summary>
        /// A "Current-State Record" is sent by a system in response to a Query received on an interface. 
        /// It reports the current reception state of that interface, with respect to a single multicast address.
        /// <para>
        /// MODE_IS_INCLUDE - indicates that the interface has a filter mode of INCLUDE for the specified multicast address.  
        /// The Source Address [i] fields in this Group Record contain the interface's source list for the specified multicast address, if it is non-empty.
        /// </para>
        /// </summary>
        CurrentStateRecordModeIsInclude = 1,

        /// <summary>
        /// A "Current-State Record" is sent by a system in response to a Query received on an interface. 
        /// It reports the current reception state of that interface, with respect to a single multicast address.
        /// <para>
        /// MODE_IS_EXCLUDE - indicates that the interface has a filter mode of EXCLUDE for the specified multicast address.  
        /// The Source Address [i] fields in this Group Record contain the interface's source list for the specified multicast address, if it is non-empty.
        /// </para>
        /// </summary>
        CurrentStateRecordModeIsExclude = 2,

        /// <summary>
        /// A "Filter-Mode-Change Record" is sent by a system whenever a local invocation of IPMulticastListen causes a change of the filter mode
        /// (i.e., a change from INCLUDE to EXCLUDE, or from EXCLUDE to INCLUDE), 
        /// of the interface-level state entry for a particular multicast address.  
        /// The Record is included in a Report sent from the interface on which the change occurred.
        /// <para>
        /// CHANGE_TO_INCLUDE_MODE - indicates that the interface has changed to INCLUDE filter mode for the specified multicast address.  
        /// The Source Address [i] fields in this Group Record contain the interface's new source list for the specified multicast address, if it is non-empty.
        /// </para>
        /// </summary>
        FilterModeChangeToInclude = 3,

        /// <summary>
        /// A "Filter-Mode-Change Record" is sent by a system whenever a local invocation of IPMulticastListen causes a change of the filter mode
        /// (i.e., a change from INCLUDE to EXCLUDE, or from EXCLUDE to INCLUDE), 
        /// of the interface-level state entry for a particular multicast address.  
        /// The Record is included in a Report sent from the interface on which the change occurred.
        /// <para>
        /// CHANGE_TO_EXCLUDE_MODE - indicates that the interface has changed to EXCLUDE filter mode for the specified multicast address.  
        /// The Source Address [i] fields in this Group Record contain the interface's new source list for the specified multicast address, if it is non-empty.
        /// </para>
        /// </summary>
        FilterModeChangeToExclude= 4,

        /// <summary>
        /// A "Source-List-Change Record" is sent by a system whenever a local invocation of IPMulticastListen causes a change of source list 
        /// that is *not* coincident with a change of filter mode, of the interface-level state entry for a particular multicast address.
        /// The Record is included in a Report sent from the interface on which the change occurred.
        /// <para>
        /// ALLOW_NEW_SOURCES - indicates that the Source Address [i] fields in this Group Record contain a list of the additional sources 
        /// that the system wishes to hear from, for packets sent to the specified multicast address.  
        /// If the change was to an INCLUDE source list, these are the addresses that were added to the list; if the change was to an EXCLUDE source list, 
        /// these are the addresses that were deleted from the list.
        /// </para>
        /// <para>
        /// If a change of source list results in both allowing new sources and blocking old sources, 
        /// then two Group Records are sent for the same multicast address, one of type ALLOW_NEW_SOURCES and one of type BLOCK_OLD_SOURCES.
        /// </para>
        /// </summary>
        SourceListChangeAllowNewSources = 5,

        /// <summary>
        /// A "Source-List-Change Record" is sent by a system whenever a local invocation of IPMulticastListen causes a change of source list 
        /// that is *not* coincident with a change of filter mode, of the interface-level state entry for a particular multicast address.
        /// The Record is included in a Report sent from the interface on which the change occurred.
        /// <para>
        /// BLOCK_OLD_SOURCES - indicates that the Source Address [i] fields in this Group Record contain a list of the sources 
        /// that the system no longer wishes to hear from, for packets sent to the specified multicast address.  
        /// If the change was to an INCLUDE source list, these are the addresses that were deleted from  the list; if the change was to an EXCLUDE source list, 
        /// these are the addresses that were added to the list.
        /// </para>
        /// <para>
        /// If a change of source list results in both allowing new sources and blocking old sources, 
        /// then two Group Records are sent for the same multicast address, one of type ALLOW_NEW_SOURCES and one of type BLOCK_OLD_SOURCES.
        /// </para>
        /// </summary>
        SourceListChangeBlockOldSources = 6,
    }
}