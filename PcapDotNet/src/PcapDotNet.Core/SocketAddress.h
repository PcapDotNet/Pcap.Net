#pragma once

#include "SocketAddressFamily.h"

namespace PcapDotNet { namespace Core 
{
	/// <summary>
	/// The base of all device addresses.
	/// Contains the family (type) of the address.
	/// </summary>
    public ref class SocketAddress abstract
    {
	public:
		/// <summary>
		/// Family (type) of the socket address.
		/// </summary>
        property SocketAddressFamily^ Family
        {
            SocketAddressFamily^ get();
        }

        virtual System::String^ ToString() override;

    protected:
        SocketAddress(unsigned short family);

	private:
        SocketAddressFamily^ _family;
    };
}}