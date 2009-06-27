#pragma once

namespace PcapDotNet { namespace Core 
{
    public value class PcapDataLink : BPacket::IDataLink
    {
    public:
        PcapDataLink(int value);
        PcapDataLink(System::String^ name);

        virtual property BPacket::DataLinkKind Kind
        {
            BPacket::DataLinkKind get();
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

    private:
        int _value;
    };
}}