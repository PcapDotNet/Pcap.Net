#pragma once

namespace PcapDotNet { namespace Core 
{
    public ref class SamplingMethod abstract
    {
    internal:
        virtual property int Method
        {
            int get() = 0;
        }

        virtual property int Value
        {
            int get() = 0;
        }

    protected:
        SamplingMethod(){}
    };
}}