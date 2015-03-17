#include "OfflinePacketCommunicator.h"

#include "MarshalingServices.h"
#include "Pcap.h"

using namespace System;
using namespace System::Globalization;
using namespace PcapDotNet::Base;
using namespace PcapDotNet::Core;

PacketTotalStatistics^ OfflinePacketCommunicator::TotalStatistics::get()
{
    throw gcnew InvalidOperationException("Can't get " + PacketTotalStatistics::typeid->Name + " for offline devices");
}

void OfflinePacketCommunicator::Transmit(PacketSendBuffer^, bool)
{
    throw gcnew InvalidOperationException("Can't transmit queue to an offline device");
}

OfflinePacketCommunicator::OfflinePacketCommunicator(String^ filename)
: PacketCommunicator(OpenFile(filename), nullptr)
{
}

// Private

// static
pcap_t* OfflinePacketCommunicator::OpenFile(String^ fileName)
{
    if (fileName == nullptr)
        throw gcnew ArgumentNullException("fileName");
    char errorBuffer[PCAP_ERRBUF_SIZE];
    FILE* file = NULL;
    pcap_t *pcapDescriptor;
    if (!StringExtensions::AreAllCharactersInRange(fileName, 0, 255)) {
        std::wstring unamangedFilename = MarshalingServices::ManagedToUnmanagedWideString(fileName);
        file = _wfopen(unamangedFilename.c_str(), L"rb");
        if (file == NULL)
            throw gcnew InvalidOperationException(String::Format(CultureInfo::InvariantCulture, "Failed opening file {0}.", fileName));
        pcapDescriptor = pcap_fopen_offline(file, errorBuffer);
    } else {
        std::string unamangedFilename = MarshalingServices::ManagedToUnmanagedString(fileName);
        pcapDescriptor = pcap_open_offline(unamangedFilename.c_str(), errorBuffer);
    }
    if (pcapDescriptor == NULL)
    {
        int fcloseResult = file == NULL ? 0 : fclose(file);
        String^ errorMessage = String::Format(CultureInfo::InvariantCulture, "Failed opening file {0}. Error: {1}.", fileName, gcnew String(errorBuffer));
        if (fcloseResult != 0)
            errorMessage += " Also failed closing the file.";
        throw gcnew InvalidOperationException(errorMessage);
    }

    return pcapDescriptor;
}