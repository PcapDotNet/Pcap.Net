#pragma once

#include "SamplingMethod.h"

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// No sampling has to be done on the current capture.
    /// In this case, no sampling algorithms are applied to the current capture. 
    /// </summary>
    public ref class SamplingMethodNone sealed : SamplingMethod
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