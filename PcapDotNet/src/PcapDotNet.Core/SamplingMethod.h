#pragma once

namespace PcapDotNet { namespace Core 
{
    /// <summary>
    /// This is the base sampling method class.
    /// Every sampling method is defined by a method and an optional value, both for internal usage.
    /// </summary>
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