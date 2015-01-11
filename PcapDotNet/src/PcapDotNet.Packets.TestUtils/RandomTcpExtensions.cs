using System;
using System.Collections.Generic;
using PcapDotNet.Packets.Transport;
using PcapDotNet.TestUtils;

namespace PcapDotNet.Packets.TestUtils
{
    public static class RandomTcpExtensions
    {
        public static TcpLayer NextTcpLayer(this Random random)
        {
            return new TcpLayer
                   {
                       SourcePort = random.NextUShort(),
                       DestinationPort = random.NextUShort(),
                       SequenceNumber = random.NextUInt(),
                       AcknowledgmentNumber = random.NextUInt(),
                       ControlBits = random.NextFlags<TcpControlBits>(),
                       Window = random.NextUShort(),
                       UrgentPointer = random.NextUShort(),
                       Options = random.NextTcpOptions(),
                   };
        }

        public static TcpOptions NextTcpOptions(this Random random)
        {
            int optionsLength = random.Next(TcpOptions.MaximumBytesLength) / 4 * 4;
            List<TcpOption> options = new List<TcpOption>();
            while (optionsLength > 0)
            {
                TcpOption option = random.NextTcpOption(optionsLength);

                if (option.IsAppearsAtMostOnce &&
                    options.FindIndex(option.Equivalent) != -1)
                {
                    continue;
                }

                options.Add(option);
                optionsLength -= option.Length;

                if (option.OptionType == TcpOptionType.EndOfOptionList)
                    break;
            }
            return new TcpOptions(options);
        }

        public static TcpOptionUnknown NextTcpOptionUnknown(this Random random, int maximumOptionLength)
        {
            TcpOptionType unknownOptionType;
            byte unknownOptionTypeValue;
            do
            {
                unknownOptionTypeValue = random.NextByte();
                unknownOptionType = (TcpOptionType)unknownOptionTypeValue;
            } while (unknownOptionType.ToString() != unknownOptionTypeValue.ToString());

            byte[] unknownOptionData = random.NextBytes(maximumOptionLength - TcpOptionUnknown.OptionMinimumLength + 1);

            return new TcpOptionUnknown(unknownOptionType, unknownOptionData);
        }

        public static TcpOption NextTcpOption(this Random random, int maximumOptionLength)
        {
            if (maximumOptionLength == 0)
                throw new ArgumentOutOfRangeException("maximumOptionLength", maximumOptionLength, "option length must be positive");

            if (maximumOptionLength >= TcpOptionUnknown.OptionMinimumLength && random.Next(100) > 90)
                return random.NextTcpOptionUnknown(maximumOptionLength);

            List<TcpOptionType> impossibleOptionTypes = new List<TcpOptionType>();
            if (maximumOptionLength < TcpOptionMaximumSegmentSize.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.MaximumSegmentSize);
            if (maximumOptionLength < TcpOptionWindowScale.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.WindowScale);
            if (maximumOptionLength < TcpOptionSelectiveAcknowledgment.OptionMinimumLength)
                impossibleOptionTypes.Add(TcpOptionType.SelectiveAcknowledgment);
            if (maximumOptionLength < TcpOptionSelectiveAcknowledgmentPermitted.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.SelectiveAcknowledgmentPermitted);
            if (maximumOptionLength < TcpOptionEcho.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.Echo);
            if (maximumOptionLength < TcpOptionEchoReply.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.EchoReply);
            if (maximumOptionLength < TcpOptionTimestamp.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.Timestamp);
            if (maximumOptionLength < TcpOptionPartialOrderServiceProfile.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.PartialOrderServiceProfile);
            if (maximumOptionLength < TcpOptionPartialOrderConnectionPermitted.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.PartialOrderConnectionPermitted);
            if (maximumOptionLength < TcpOptionConnectionCountBase.OptionLength)
            {
                impossibleOptionTypes.Add(TcpOptionType.ConnectionCount);
                impossibleOptionTypes.Add(TcpOptionType.ConnectionCountNew);
                impossibleOptionTypes.Add(TcpOptionType.ConnectionCountEcho);
            }
            if (maximumOptionLength < TcpOptionAlternateChecksumRequest.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.AlternateChecksumRequest);
            if (maximumOptionLength < TcpOptionAlternateChecksumData.OptionMinimumLength)
                impossibleOptionTypes.Add(TcpOptionType.AlternateChecksumData);
            if (maximumOptionLength < TcpOptionMd5Signature.OptionLength)
                impossibleOptionTypes.Add(TcpOptionType.Md5Signature);
            if (maximumOptionLength < TcpOptionMood.OptionMaximumLength)
                impossibleOptionTypes.Add(TcpOptionType.Mood);
            if (maximumOptionLength < TcpOptionAuthentication.OptionMinimumLength)
                impossibleOptionTypes.Add(TcpOptionType.TcpAuthentication);

            impossibleOptionTypes.Add(TcpOptionType.QuickStartResponse);
            impossibleOptionTypes.Add(TcpOptionType.UserTimeout);
            impossibleOptionTypes.Add(TcpOptionType.SelectiveNegativeAcknowledgements); // TODO: Support Selective Negative Acknowledgements.

            TcpOptionType optionType = random.NextEnum<TcpOptionType>(impossibleOptionTypes);
            switch (optionType)
            {
                case TcpOptionType.EndOfOptionList:
                    return TcpOption.End;

                case TcpOptionType.NoOperation:
                    return TcpOption.Nop;

                case TcpOptionType.MaximumSegmentSize:
                    return new TcpOptionMaximumSegmentSize(random.NextUShort());

                case TcpOptionType.WindowScale:
                    return new TcpOptionWindowScale(random.NextByte());

                case TcpOptionType.SelectiveAcknowledgment:
                    int numBlocks = random.Next((maximumOptionLength - TcpOptionSelectiveAcknowledgment.OptionMinimumLength) / 8 + 1);
                    TcpOptionSelectiveAcknowledgmentBlock[] blocks = new TcpOptionSelectiveAcknowledgmentBlock[numBlocks];
                    for (int i = 0; i != numBlocks; ++i)
                        blocks[i] = new TcpOptionSelectiveAcknowledgmentBlock(random.NextUInt(), random.NextUInt());
                    return new TcpOptionSelectiveAcknowledgment(blocks);

                case TcpOptionType.SelectiveAcknowledgmentPermitted:
                    return new TcpOptionSelectiveAcknowledgmentPermitted();

                case TcpOptionType.Echo:
                    return new TcpOptionEcho(random.NextUInt());

                case TcpOptionType.EchoReply:
                    return new TcpOptionEchoReply(random.NextUInt());

                case TcpOptionType.Timestamp:
                    return new TcpOptionTimestamp(random.NextUInt(), random.NextUInt());

                case TcpOptionType.PartialOrderServiceProfile:
                    return new TcpOptionPartialOrderServiceProfile(random.NextBool(), random.NextBool());

                case TcpOptionType.PartialOrderConnectionPermitted:
                    return new TcpOptionPartialOrderConnectionPermitted();

                case TcpOptionType.ConnectionCount:
                    return new TcpOptionConnectionCount(random.NextUInt());

                case TcpOptionType.ConnectionCountEcho:
                    return new TcpOptionConnectionCountEcho(random.NextUInt());

                case TcpOptionType.ConnectionCountNew:
                    return new TcpOptionConnectionCountNew(random.NextUInt());

                case TcpOptionType.AlternateChecksumRequest:
                    return new TcpOptionAlternateChecksumRequest(random.NextEnum<TcpOptionAlternateChecksumType>());

                case TcpOptionType.AlternateChecksumData:
                    return new TcpOptionAlternateChecksumData(random.NextBytes(random.Next(maximumOptionLength - TcpOptionAlternateChecksumData.OptionMinimumLength + 1)));

                case TcpOptionType.Md5Signature:
                    return new TcpOptionMd5Signature(random.NextBytes(TcpOptionMd5Signature.OptionValueLength));

                case TcpOptionType.Mood:
                    return new TcpOptionMood(random.NextEnum(TcpOptionMoodEmotion.None));

                case TcpOptionType.TcpAuthentication:
                    return new TcpOptionAuthentication(random.NextByte(), random.NextByte(), random.NextBytes(random.Next(maximumOptionLength - TcpOptionAuthentication.OptionMinimumLength + 1)));

                default:
                    throw new InvalidOperationException("optionType = " + optionType);
            }
        }
    }
}