// This file is used by Code Analysis to maintain 
// CA_GLOBAL_SUPPRESS_MESSAGE macros that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File".
// You do not need to add suppressions to this file manually.

// https://connect.microsoft.com/VisualStudio/feedback/details/559820/getting-ca1806-on-c-cli-code-when-catching-exceptions
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Usage", "CA1806:DoNotIgnoreMethodResults", MessageId="<Module>.__CxxRegisterExceptionObject(System.Void*,System.Void*)", Scope="member", Target="PcapDotNet.Core.BerkeleyPacketFilter.#Initialize(pcap*,System.String,PcapDotNet.Core.IpV4SocketAddress)");

// https://connect.microsoft.com/VisualStudio/feedback/details/551348/brocken-vs-2008-functionality-it-is-impossible-to-build-strong-name-c-cli-assembly
CA_GLOBAL_SUPPRESS_MESSAGE("Microsoft.Design", "CA2210:AssembliesShouldHaveValidStrongNames");
