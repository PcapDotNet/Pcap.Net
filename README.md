# Pcap.Net
.NET wrapper for WinPcap written in C++/CLI and C#, which features almost all WinPcap features and includes a packet interpretation framework.

## Need help?
* See the [Pcap.Net wiki](https://github.com/PcapDotNet/Pcap.Net/wiki) for guides on using and developing Pcap.Net.
* Use the [Pcap.Net Q&A Group](https://groups.google.com/forum/#!forum/pcapdotnet) to ask questions.

## Features

### .Net wrap for WinPcap

Including:
* Getting the list of Live Devices on the local host.
* Reading packets from Live Devices (Network Devices) and Offline Devices (Files) using the different WinPcap methods.
* Receiving statistics on the entire capture.
* Receiving statistics of packets instead of the full packets.
* Using different sampling methods.
* Applying Berkley Packet Filters.
* Sending packets to Live Devices directly or using WinPcap's send queues.
* Dumping packets to Pcap files.
* Using Enumerables to receive packets (and LINQ).

Not including:
* AirPcap features.
* Remote Pcap features.

### Packet interpretation
* Ethernet + VLAN tagging (802.1Q)
* ARP
* IPv4
* IPv6
* GRE
* ICMP
* IGMP
* UDP
* TCP
* DNS
* HTTP

[![Follow Pcap.Net on Google+](https://ssl.gstatic.com/images/icons/gplus-32.png)](https://plus.google.com/111766834267147414574) [Follow Pcap.Net on Google+](https://plus.google.com/111766834267147414574)

<a href='https://pledgie.com/campaigns/28716'><img alt='Click here to lend your support to: Pcap.Net and make a donation at pledgie.com !' src='https://pledgie.com/campaigns/28716.png?skin_name=chrome' border='0' ></a>

<a href='https://salt.bountysource.com/teams/pcapdotnet'>Support Pcap.Net using Salt</a>
