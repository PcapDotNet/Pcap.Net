#pragma once

namespace PcapDotNet { namespace Core 
{
    public value class PcapDataLink : Packets::IDataLink, System::IEquatable<PcapDataLink>
    {
    public:
        PcapDataLink(Packets::DataLinkKind kind);
        PcapDataLink(int value);
        PcapDataLink(System::String^ name);

        virtual property Packets::DataLinkKind Kind
        {
            Packets::DataLinkKind get();
        }

        property int Value
        {
            int get();
        }

        property System::String^ Name
        {
            System::String^ get();
        }

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