namespace PcapDotNet.Base
{
    public class Tuple<T1, T2>
    {
        public Tuple(T1 value1, T2 value2)
        {
            _value1 = value1;
            _value2 = value2;
        }

        public T1 Value1
        {
            get { return _value1; }
        }

        public T2 Value2
        {
            get { return _value2; }
        }

        private readonly T1 _value1;
        private readonly T2 _value2;
    }
}