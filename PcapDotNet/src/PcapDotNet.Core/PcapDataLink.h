#pragma once

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// A packet communicator datalink.
    /// </summary>
    public value class PcapDataLink sealed : Packets::IDataLink, System::IEquatable<PcapDataLink>
    {
    public:
        /// <summary>
        /// Create the datalink from one of the well defined datalink kinds.
        /// </summary>
        /// <param name="kind">The kind of datalink to create.</param>
        PcapDataLink(Packets::DataLinkKind kind);

        /// <summary>
        /// Create the datalink from an int value (pcap value).
        /// </summary>
        /// <param name="value">The pcap value of the datalink.</param>
        PcapDataLink(int value);

        /// <summary>
        /// Create the datalink its name.
        /// </summary>
        /// <param name="name">The name of the pcap datalink.</param>
        PcapDataLink(System::String^ name);

        /// <summary>
        /// The kind of the datalink.
        /// </summary>
        virtual property Packets::DataLinkKind Kind
        {
            Packets::DataLinkKind get();
        }

        /// <summary>
        /// The pcap value of the datalink.
        /// </summary>
        property int Value
        {
            int get();
        }

        /// <summary>
        /// The name of the datalink.
        /// </summary>
        property System::String^ Name
        {
            System::String^ get();
        }

        /// <summary>
        /// The description of the datalink.
        /// </summary>
        property System::String^ Description
        {
            System::String^ get();
        }

        virtual System::String^ ToString() override;

        virtual bool Equals(PcapDataLink other);
        virtual bool Equals(System::Object^ obj) override;

        virtual int GetHashCode() override;

        static bool operator ==(PcapDataLink dataLink1, PcapDataLink dataLink2);

        static bool operator !=(PcapDataLink dataLink1, PcapDataLink dataLink2);

    private:
        static int KindToValue(Packets::DataLinkKind kind);

    private:
        int _value;
    };
}}