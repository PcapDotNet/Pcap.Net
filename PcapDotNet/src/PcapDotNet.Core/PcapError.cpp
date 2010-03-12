#include "PcapError.h"
#include "Pcap.h"

using namespace System;
using namespace System::Text;
using namespace PcapDotNet::Core;

// static 
String^ PcapError::GetErrorMessage(pcap_t* pcapDescriptor)
{
	char* unmanagedPcapError = pcap_geterr(pcapDescriptor);
	if (unmanagedPcapError == NULL)
		return nullptr;
    return gcnew String(unmanagedPcapError);
}

// static 
InvalidOperationException^ PcapError::BuildInvalidOperation(String^ errorMessage, pcap_t* pcapDescriptor)
{
    StringBuilder^ fullError = gcnew StringBuilder(errorMessage);
    if (pcapDescriptor != NULL)
    {
		String^ pcapError = GetErrorMessage(pcapDescriptor);
		if (!String::IsNullOrEmpty(pcapError))
		{
			fullError->Append(". WinPcap Error: ");         
			fullError->Append(pcapError);
		}
    }
    return gcnew InvalidOperationException(fullError->ToString());
}
