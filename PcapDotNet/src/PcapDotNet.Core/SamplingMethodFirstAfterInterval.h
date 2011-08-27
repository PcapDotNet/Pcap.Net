#pragma once

#include "SamplingMethod.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// This sampling method defines that we have to return 1 packet every given time-interval.
    /// In other words, if the interval is set to 10 milliseconds, the first packet is returned to the caller; the next returned one will be the first packet that arrives when 10ms have elapsed.
    /// </summary>
    public ref class SamplingMethodFirstAfterInterval sealed : SamplingMethod
    {
    public:
        /// <summary>
        /// Constructs by giving an interval in milliseconds.
        /// </summary>
        /// <param name="intervalInMs">The number of milliseconds to wait between packets sampled.</param>
        /// <exception cref="System::ArgumentOutOfRangeException">The given number of milliseconds is negative.</exception>
        SamplingMethodFirstAfterInterval(int intervalInMs);

        /// <summary>
        /// Constructs by giving an interval as TimeSpan.
        /// </summary>
        /// <param name="interval">The time to wait between packets sampled.</param>
        /// <exception cref="System::ArgumentOutOfRangeException">The interval is negative or larger than 2^31 milliseconds.</exception>
        SamplingMethodFirstAfterInterval(System::TimeSpan interval);

    internal:
        virtual property int Method
        {
            int get() override;
        }

        /// <summary>
        /// Indicates the 'waiting time' in milliseconds before one packet got accepted. 
        /// In other words, if 'value = 10', the first packet is returned to the caller; the next returned one will be the first packet that arrives when 10ms have elapsed.
        /// </summary>
        virtual property int Value
        {
            int get() override;
        }

    private:
        int _intervalInMs;
    };
}}