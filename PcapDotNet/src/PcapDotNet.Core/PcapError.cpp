#include "PcapError.h"
#include "Pcap.h"

using namespace System;
using namespace System::Text;
using namespace PcapDotNet::Core;

// static 
String^ PcapError::GetErrorMessage(pcap_t* pcapDescriptor)
{
    return gcnew String(pcap_geterr(pcapDescriptor));
}

// static 
InvalidOperationException^ PcapError::BuildInvalidOperation(String^ errorMessage, pcap_t* pcapDescriptor)
{
    StringBuilder^ fullError = gcnew StringBuilder(errorMessage);
    if (pcapDescriptor != NULL)
    {
        String^ pcapError = gcnew String(pcap_geterr(pcapDescriptor));
        if (!String::IsNullOrEmpty(pcapError))
        {
            fullError->Append(". ");         
            fullError->Append(pcapError);
        }
    }
    return gcnew InvalidOperationException(fullError->ToString());
}