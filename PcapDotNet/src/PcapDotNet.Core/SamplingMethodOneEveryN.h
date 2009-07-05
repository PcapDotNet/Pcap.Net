#pragma once

#include "SamplingMethod.h"

namespace PcapDotNet { namespace Core 
{
    public ref class SamplingMethodOneEveryN : SamplingMethod
    {
    public:
        SamplingMethodOneEveryN(int n);

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
        int _n;
    };
}}