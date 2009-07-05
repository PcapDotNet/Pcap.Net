#pragma once

#include "SamplingMethod.h"

namespace PcapDotNet { namespace Core 
{
    public ref class SamplingMethodFirstAfterInterval : SamplingMethod
    {
    public:
        SamplingMethodFirstAfterInterval(int intervalInMs);
        SamplingMethodFirstAfterInterval(System::TimeSpan interval);

    internal:
        virtual property int Method
        {
            int get() override;
        }

        virtual property int Value
        {
            int get() override;
        }

    private:
        int _intervalInMs;
    };
}}