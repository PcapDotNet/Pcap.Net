#include "PcapDeclarations.h"

struct pcap{};
struct pcap_dumper{};

private ref class PcapTypeDefs
{
private:
    CA_SUPPRESS_MESSAGE("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")
    pcap_t *pcap;
    CA_SUPPRESS_MESSAGE("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")
    pcap_dumper_t *pcap_dumper;
};