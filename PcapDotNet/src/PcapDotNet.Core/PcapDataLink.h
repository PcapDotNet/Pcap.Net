#pragma once

namespace PcapDotNet { namespace Core 
{
    public value class PcapDataLink : Packets::IDataLink
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

    private:
        static int KindToValue(Packets::DataLinkKind kind);

    private:
        int _value;
    };
}}