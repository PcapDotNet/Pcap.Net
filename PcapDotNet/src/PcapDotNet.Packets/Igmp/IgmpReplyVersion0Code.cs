namespace PcapDotNet.Packets.Igmp
{
    /// <summary>
    /// The code of the IGMP message for Create Group Request (RFC988).
    /// </summary>
    public enum IgmpReplyVersion0Code : byte
    {
        /// <summary>
        /// Request Granted.
        /// </summary>
        RequestGranted = 0,

        /// <summary>
        /// Request Denied - No Resources.
        /// </summary>
        RequestDeniedNoResources = 1,

        /// <summary>
        /// Request Denied - Invalid Code.
        /// </summary>
        RequestDeniedInvalidCode = 2,

        /// <summary>
        /// Request Denied - Invalid Group Address.
        /// </summary>
        RequestDeniedInvalidGroupAddress = 3,

        /// <summary>
        /// Request Denied - Invalid Access Key.
        /// </summary>
        RequestDeniedInvalidAccessKey = 4,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn5Seconds = 5,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn6Seconds = 6,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn7Seconds = 7,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn8Seconds = 8,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn9Seconds = 9,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn10Seconds = 10,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn11Seconds = 11,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn12Seconds = 12,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn13Seconds = 13,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn14Seconds = 14,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn15Seconds = 15,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn16Seconds = 16,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn17Seconds = 17,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn18Seconds = 18,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn19Seconds = 19,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn20Seconds = 20,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn21Seconds = 21,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn22Seconds = 22,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn23Seconds = 23,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn24Seconds = 24,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn25Seconds = 25,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn26Seconds = 26,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn27Seconds = 27,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn28Seconds = 28,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn29Seconds = 29,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn30Seconds = 30,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn31Seconds = 31,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn32Seconds = 32,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn33Seconds = 33,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn34Seconds = 34,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn35Seconds = 35,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn36Seconds = 36,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn37Seconds = 37,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn38Seconds = 38,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn39Seconds = 39,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn40Seconds = 40,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn41Seconds = 41,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn42Seconds = 42,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn43Seconds = 43,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn44Seconds = 44,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn45Seconds = 45,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn46Seconds = 46,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn47Seconds = 47,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn48Seconds = 48,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn49Seconds = 49,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn50Seconds = 50,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn51Seconds = 51,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn52Seconds = 52,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn53Seconds = 53,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn54Seconds = 54,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn55Seconds = 55,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn56Seconds = 56,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn57Seconds = 57,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn58Seconds = 58,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn59Seconds = 59,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn60Seconds = 60,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn61Seconds = 61,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn62Seconds = 62,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn63Seconds = 63,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn64Seconds = 64,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn65Seconds = 65,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn66Seconds = 66,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn67Seconds = 67,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn68Seconds = 68,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn69Seconds = 69,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn70Seconds = 70,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn71Seconds = 71,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn72Seconds = 72,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn73Seconds = 73,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn74Seconds = 74,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn75Seconds = 75,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn76Seconds = 76,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn77Seconds = 77,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn78Seconds = 78,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn79Seconds = 79,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn80Seconds = 80,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn81Seconds = 81,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn82Seconds = 82,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn83Seconds = 83,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn84Seconds = 84,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn85Seconds = 85,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn86Seconds = 86,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn87Seconds = 87,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn88Seconds = 88,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn89Seconds = 89,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn90Seconds = 90,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn91Seconds = 91,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn92Seconds = 92,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn93Seconds = 93,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn94Seconds = 94,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn95Seconds = 95,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn96Seconds = 96,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn97Seconds = 97,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn98Seconds = 98,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn99Seconds = 99,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn100Seconds = 100,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn101Seconds = 101,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn102Seconds = 102,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn103Seconds = 103,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn104Seconds = 104,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn105Seconds = 105,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn106Seconds = 106,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn107Seconds = 107,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn108Seconds = 108,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn109Seconds = 109,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn110Seconds = 110,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn111Seconds = 111,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn112Seconds = 112,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn113Seconds = 113,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn114Seconds = 114,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn115Seconds = 115,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn116Seconds = 116,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn117Seconds = 117,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn118Seconds = 118,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn119Seconds = 119,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn120Seconds = 120,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn121Seconds = 121,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn122Seconds = 122,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn123Seconds = 123,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn124Seconds = 124,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn125Seconds = 125,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn126Seconds = 126,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn127Seconds = 127,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn128Seconds = 128,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn129Seconds = 129,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn130Seconds = 130,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn131Seconds = 131,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn132Seconds = 132,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn133Seconds = 133,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn134Seconds = 134,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn135Seconds = 135,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn136Seconds = 136,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn137Seconds = 137,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn138Seconds = 138,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn139Seconds = 139,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn140Seconds = 140,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn141Seconds = 141,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn142Seconds = 142,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn143Seconds = 143,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn144Seconds = 144,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn145Seconds = 145,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn146Seconds = 146,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn147Seconds = 147,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn148Seconds = 148,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn149Seconds = 149,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn150Seconds = 150,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn151Seconds = 151,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn152Seconds = 152,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn153Seconds = 153,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn154Seconds = 154,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn155Seconds = 155,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn156Seconds = 156,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn157Seconds = 157,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn158Seconds = 158,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn159Seconds = 159,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn160Seconds = 160,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn161Seconds = 161,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn162Seconds = 162,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn163Seconds = 163,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn164Seconds = 164,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn165Seconds = 165,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn166Seconds = 166,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn167Seconds = 167,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn168Seconds = 168,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn169Seconds = 169,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn170Seconds = 170,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn171Seconds = 171,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn172Seconds = 172,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn173Seconds = 173,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn174Seconds = 174,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn175Seconds = 175,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn176Seconds = 176,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn177Seconds = 177,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn178Seconds = 178,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn179Seconds = 179,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn180Seconds = 180,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn181Seconds = 181,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn182Seconds = 182,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn183Seconds = 183,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn184Seconds = 184,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn185Seconds = 185,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn186Seconds = 186,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn187Seconds = 187,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn188Seconds = 188,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn189Seconds = 189,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn190Seconds = 190,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn191Seconds = 191,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn192Seconds = 192,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn193Seconds = 193,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn194Seconds = 194,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn195Seconds = 195,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn196Seconds = 196,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn197Seconds = 197,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn198Seconds = 198,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn199Seconds = 199,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn200Seconds = 200,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn201Seconds = 201,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn202Seconds = 202,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn203Seconds = 203,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn204Seconds = 204,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn205Seconds = 205,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn206Seconds = 206,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn207Seconds = 207,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn208Seconds = 208,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn209Seconds = 209,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn210Seconds = 210,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn211Seconds = 211,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn212Seconds = 212,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn213Seconds = 213,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn214Seconds = 214,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn215Seconds = 215,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn216Seconds = 216,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn217Seconds = 217,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn218Seconds = 218,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn219Seconds = 219,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn220Seconds = 220,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn221Seconds = 221,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn222Seconds = 222,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn223Seconds = 223,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn224Seconds = 224,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn225Seconds = 225,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn226Seconds = 226,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn227Seconds = 227,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn228Seconds = 228,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn229Seconds = 229,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn230Seconds = 230,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn231Seconds = 231,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn232Seconds = 232,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn233Seconds = 233,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn234Seconds = 234,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn235Seconds = 235,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn236Seconds = 236,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn237Seconds = 237,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn238Seconds = 238,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn239Seconds = 239,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn240Seconds = 240,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn241Seconds = 241,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn242Seconds = 242,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn243Seconds = 243,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn244Seconds = 244,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn245Seconds = 245,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn246Seconds = 246,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn247Seconds = 247,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn248Seconds = 248,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn249Seconds = 249,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn250Seconds = 250,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn251Seconds = 251,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn252Seconds = 252,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn253Seconds = 253,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn254Seconds = 254,

        /// <summary>
        /// Request Pending - Retry in this value many seconds.
        /// </summary>
        RequestPendingRetryIn255Seconds = 255,
    }
}