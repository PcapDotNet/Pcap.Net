using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PcapDotNet.Packets.Transport
{
    /// <summary>
    /// TCP Mood Option:
    /// <pre>
    /// +---------+--------+------------+
    /// | Kind=25 | Length | ASCII Mood |
    /// +---------+--------+------------+
    /// </pre>
    /// 
    /// <para>
    ///  It is proposed that option 25 (released 2000-12-18) be used to define packet mood.
    /// This option would have a length value of 4 or 5 bytes.
    /// All the simple emotions described as expressible via this mechanism can be displayed with two or three 7-bit, ASCII-encoded characters.
    /// Multiple mood options may appear in a TCP header, so as to express more complex moods than those defined here (for instance if a packet were happy and surprised).
    /// </para>
    /// 
    /// <para>
    /// It is proposed that common emoticons be used to denote packet mood.
    /// Packets do not "feel" per se.  The emotions they could be tagged with are a reflection of the user mood expressed through packets.
    /// So the humanity expressed in a packet would be entirely sourced from humans.
    ///  To this end, it is proposed that simple emotions be used convey mood as follows.
    /// 
    /// <pre>
    /// ASCII                Mood
    /// =====                ====
    /// :)                   Happy
    /// :(                   Sad
    /// :D                   Amused
    /// %(                   Confused
    /// :o                   Bored
    /// :O                   Surprised
    /// :P                   Silly
    /// :@                   Frustrated
    /// >:@                  Angry
    /// :|                   Apathetic
    /// ;)                   Sneaky
    /// >:)                  Evil
    /// </pre>
    /// 
    /// Proposed ASCII character encoding
    /// <pre>
    /// Binary          Dec  Hex     Character
    /// ========        ===  ===     =========
    /// 010 0101        37   25      %
    /// 010 1000        40   28      (
    /// 010 1001        41   29      )
    /// 011 1010        58   3A      :
    /// 011 1011        59   3B      ;
    /// 011 1110        62   3E      >
    /// 100 0000        64   40      @
    /// 100 0100        68   44      D
    /// 100 1111        79   4F      O
    /// 101 0000        80   50      P
    /// 110 1111        111  6F      o
    /// 111 1100        124  7C      |
    /// </pre>
    /// </para>
    /// </summary>
    [OptionTypeRegistration(typeof(TcpOptionType), TcpOptionType.Mood)]
    public class TcpOptionMood : TcpOptionComplex, IOptionComplexFactory, IEquatable<TcpOptionMood>
    {
        /// <summary>
        /// The minimum number of bytes this option take.
        /// </summary>
        public const int OptionMinimumLength = OptionHeaderLength + OptionValueMinimumLength;
        public const int OptionMaximumLength = OptionHeaderLength + OptionValueMaximumLength;

        /// <summary>
        /// The minimum number of bytes this option value take.
        /// </summary>
        public const int OptionValueMinimumLength = 2;

        /// <summary>
        /// The maximum number of bytes this option value take.
        /// </summary>
        public const int OptionValueMaximumLength = 3;

        /// <summary>
        /// Creates the option using the given emotion.
        /// </summary>
        public TcpOptionMood(TcpOptionMoodEmotion emotion)
            : base(TcpOptionType.Mood)
        {
            Emotion = emotion;
        }

        /// <summary>
        /// The default emotion is confused.
        /// </summary>
        public TcpOptionMood()
            : this(TcpOptionMoodEmotion.Confused)
        {
        }

        /// <summary>
        /// The emotion of the option.
        /// </summary>
        public TcpOptionMoodEmotion Emotion { get; private set; }

        public string EmotionString
        {
            get
            {
                int emotionValue = (int)Emotion;
                if (emotionValue >= _emotionToString.Length)
                    throw new InvalidOperationException("No string value for emotion " + Emotion);

                return _emotionToString[emotionValue];
            }
        }

        /// <summary>
        /// The number of bytes this option will take.
        /// </summary>
        public override int Length
        {
            get { return OptionHeaderLength + ValueLength; }
        }

        public int ValueLength
        {
            get { return EmotionString.Length; }
        }

        /// <summary>
        /// True iff this option may appear at most once in a datagram.
        /// </summary>
        public override bool IsAppearsAtMostOnce
        {
            get { return false; }
        }

        /// <summary>
        /// Two mood options are equal if they have the same emotion.
        /// </summary>
        public bool Equals(TcpOptionMood other)
        {
            if (other == null)
                return false;

            return Emotion == other.Emotion;
        }

        /// <summary>
        /// Two mood options are equal if they have the same emotion.
        /// </summary>
        public override bool Equals(TcpOption other)
        {
            return Equals(other as TcpOptionMood);
        }

        /// <summary>
        /// The hash code of the echo option is the hash code of the option type xored with the hash code info.
        /// </summary>
        public override int GetHashCode()
        {
            return base.GetHashCode() ^ Emotion.GetHashCode();
        }

        /// <summary>
        /// Tries to read the option from a buffer starting from the option value (after the type and length).
        /// </summary>
        /// <param name="buffer">The buffer to read the option from.</param>
        /// <param name="offset">The offset to the first byte to read the buffer. Will be incremented by the number of bytes read.</param>
        /// <param name="valueLength">The number of bytes the option value should take according to the length field that was already read.</param>
        /// <returns>On success - the complex option read. On failure - null.</returns>
        Option IOptionComplexFactory.CreateInstance(byte[] buffer, ref int offset, byte valueLength)
        {
            if (valueLength < OptionValueMinimumLength || valueLength > OptionValueMaximumLength)
                return null;

            byte[] emotionBuffer = buffer.ReadBytes(ref offset, valueLength);
            TcpOptionMoodEmotion emotion = StringToEmotion(Encoding.ASCII.GetString(emotionBuffer));
            if (emotion == TcpOptionMoodEmotion.None)
                return null;

            return new TcpOptionMood(emotion);
        }

        internal override void Write(byte[] buffer, ref int offset)
        {
            base.Write(buffer, ref offset);
            buffer.Write(ref offset, Encoding.ASCII.GetBytes(EmotionString));
        }

        private static TcpOptionMoodEmotion StringToEmotion(string emotionString)
        {
            TcpOptionMoodEmotion emotion;
            if (_stringToEmotion.TryGetValue(emotionString, out emotion))
                return emotion;

            return TcpOptionMoodEmotion.None;
        }

        private static readonly Dictionary<string, TcpOptionMoodEmotion> _stringToEmotion = new Dictionary<string, TcpOptionMoodEmotion>
                                                                                            {
                                                                                                {":)", TcpOptionMoodEmotion.Happy},
                                                                                                {":(", TcpOptionMoodEmotion.Sad},
                                                                                                {":D", TcpOptionMoodEmotion.Amused},
                                                                                                {"%(", TcpOptionMoodEmotion.Confused},
                                                                                                {":o", TcpOptionMoodEmotion.Bored},
                                                                                                {":O", TcpOptionMoodEmotion.Surprised},
                                                                                                {":P", TcpOptionMoodEmotion.Silly},
                                                                                                {":@", TcpOptionMoodEmotion.Frustrated},
                                                                                                {">:@", TcpOptionMoodEmotion.Angry},
                                                                                                {":|", TcpOptionMoodEmotion.Apathetic},
                                                                                                {";)", TcpOptionMoodEmotion.Sneaky},
                                                                                                {">:)", TcpOptionMoodEmotion.Evil}
                                                                                            };

        private static readonly string[] _emotionToString = _stringToEmotion.OrderBy(pair => pair.Value).Select(pair => pair.Key).ToArray();
    }
}