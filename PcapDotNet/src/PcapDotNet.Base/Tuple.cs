namespace PcapDotNet.Base
{
    public class Tuple<TValue1, TValue2>
    {
        public Tuple(TValue1 value1, TValue2 value2)
        {
            _value1 = value1;
            _value2 = value2;
        }

        public TValue1 Value1
        {
            get { return _value1; }
        }

        public TValue2 Value2
        {
            get { return _value2; }
        }

        private readonly TValue1 _value1;
        private readonly TValue2 _value2;
    }
}