#pragma once

#include "SamplingMethod.h"

namespace PcapDotNet { namespace Core 
{
    public ref class SamplingMethodNone : SamplingMethod
    {
    internal:
        virtual property int Method
        {
            int get() override;
        }

        virtual property int Value
        {
            int get() override;
        }
    };
}}