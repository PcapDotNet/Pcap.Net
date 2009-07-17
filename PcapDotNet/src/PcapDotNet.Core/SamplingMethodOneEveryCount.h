#pragma once

#include "SamplingMethod.h"

namespace PcapDotNet { namespace Core 
{
    public ref class SamplingMethodOneEveryCount : SamplingMethod
    {
    public:
        SamplingMethodOneEveryCount(int count);

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
        int _count;
    };
}}