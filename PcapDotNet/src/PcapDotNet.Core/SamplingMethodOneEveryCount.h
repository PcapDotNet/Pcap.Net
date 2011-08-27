#pragma once

#include "SamplingMethod.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// Defines that only 1 out of count packets must be returned to the user.
    /// In other words, if the count is set to 10, the first packet is returned to the caller, while the following 9 are discarded.
    /// </summary>
    public ref class SamplingMethodOneEveryCount sealed : SamplingMethod
    {
    public:
        /// <summary>
        /// Constructs by giving a count.
        /// </summary>
        /// <param name="count">1 packet out of count packets will be sampled (for each sampled packet, count-1 will be discarded).</param>
        /// <exception cref="System::ArgumentOutOfRangeException">The given count is non-positive.</exception>
        SamplingMethodOneEveryCount(int count);

    internal:
        virtual property int Method
        {
            int get() override;
        }

        /// <summary>
        /// Indicates the number of packets (minus 1) that must be discarded before one packet got accepted. 
        /// In other words, if 'value = 10', the first packet is returned to the caller, while the following 9 are discarded.
        /// </summary>
        virtual property int Value
        {
            int get() override;
        }

    private:
        int _count;
    };
}}