#include "PcapDeclarations.h"

struct pcap{};
struct pcap_dumper{};

private ref class PcapTypeDefs
{
private:
    pcap_t *pcap;
    pcap_dumper_t *pcap_dumper;
};