using System;
using System.Globalization;
using System.Runtime.InteropServices;

namespace PcapDotNet.Base
{
    /// <summary>
    /// A 48 bit unsigned integer.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct UInt48
    {
        /// <summary>
        /// The number of bytes this type will take.
        /// </summary>
        public const int SizeOf = 6;

        /// <summary>
        /// The minimum value of this type.
        /// </summary>
        public static readonly UInt48 MinValue = 0;

        /// <summary>
        /// The maximum value of this type.
        /// </summary>
        public static readonly UInt48 MaxValue = (UInt48)0x0000FFFFFFFFFFFF;

        /// <summary>
        /// Converts the string representation of a number to its 48-bit unsigned integer equivalent.
        /// </summary>
        /// <param name="value">A string that represents the number to convert.</param>
        /// <returns>A 48-bit unsigned integer equivalent to the number contained in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> parameter is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">The <paramref name="value"/> parameter is not in the correct format.</exception>
        /// <exception cref="OverflowException">The <paramref name="value"/> parameter represents a number less than <see cref="UInt48.MinValue"/> or greater than <see cref="UInt48.MaxValue"/>.</exception>
        /// <remarks>
        /// The <paramref name="value"/> parameter should be the string representation of a number in the following form.
        /// <para>[ws][sign]digits[ws]</para>
        /// <para> Elements in square brackets ([ and ]) are optional. The following table describes each element.</para>
        ///   <list type="table">
        ///     <listheader>
        ///       <term>Element</term>
        ///       <description>Description</description>
        ///     </listheader>
        ///     <item><term>ws</term><description>Optional white space.</description></item>
        ///     <item>
        ///       <term>sign</term>
        ///       <description>
        ///       An optional sign. 
        ///       Valid sign characters are determined by the <see cref="NumberFormatInfo.NegativeSign"/> and <see cref="NumberFormatInfo.PositiveSign"/> properties of the current culture. 
        ///       However, the negative sign symbol can be used only with zero; otherwise, the method throws an <see cref="OverflowException"/>.
        ///       </description>
        ///     </item>
        ///     <item><term>digits</term><description>A sequence of digits from 0 through 9. Any leading zeros are ignored.</description></item>
        ///   </list>
        ///   <note>
        ///   The <paramref name="value"/> parameter is interpreted using the <see cref="NumberStyles.Integer"/> style. 
        ///   It cannot contain any group separators or decimal separator, and it cannot have a decimal portion.
        ///   </note>
        ///   The <paramref name="value"/> parameter is parsed by using the formatting information in a <see cref="NumberFormatInfo"/> object that is initialized for the current system culture. 
        ///   For more information, see <see cref="NumberFormatInfo.CurrentInfo"/>. 
        ///   To parse a string by using the formatting information of a specific culture, use the <see cref="Parse(String, IFormatProvider)"/> method.
        /// </remarks>
        public static UInt48 Parse(string value)
        {
            return Parse(value, NumberStyles.Integer, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Converts the string representation of a number in a specified culture-specific format to its 48-bit unsigned integer equivalent.
        /// </summary>
        /// <param name="value">A string that represents the number to convert.</param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="value"/>.</param>
        /// <returns>A 48-bit unsigned integer equivalent to the number specified in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> parameter is <see langword="null"/>.</exception>
        /// <exception cref="FormatException">The <paramref name="value"/> parameter is not in the correct style.</exception>
        /// <exception cref="OverflowException">
        ///   The <paramref name="value"/> parameter represents a number less than <see cref="UInt48.MinValue"/> or greater than <see cref="UInt48.MaxValue"/>. 
        /// </exception>
        /// <remarks>
        /// This overload of the Parse(String, IFormatProvider) method is typically used to convert text that can be formatted in a variety of ways to a <see cref="UInt48"/> value. 
        /// For example, it can be used to convert the text entered by a user into an HTML text box to a numeric value.
        /// <para>The <paramref name="value"/> parameter contains a number of the form:</para>
        /// <para>[ws][sign]digits[ws]</para>
        /// <para>
        ///   Elements in square brackets ([ and ]) are optional. 
        ///   The following table describes each element:        
        /// </para>
        ///   <list type="table">
        ///     <listheader>
        ///       <term>Element</term>
        ///       <description>Description</description>
        ///     </listheader>
        ///     <item><term>ws</term><description>Optional white space.</description></item>
        ///     <item>
        ///       <term>sign</term>
        ///       <description>An optional positive sign, or a negative sign if <paramref name="value"/> represents the value zero.</description>
        ///     </item>
        ///     <item><term>digits</term><description>A sequence of digits from 0 through 9.</description></item>
        ///   </list>
        ///   The <paramref name="value"/> parameter is interpreted using the <see cref="NumberStyles.Integer"/> style. 
        ///   In addition to the unsigned integer value's decimal digits, only leading and trailing spaces along with a leading sign is allowed. 
        ///   (If the negative sign is present, <paramref name="value"/> must represent a value of zero, or the method throws an <see cref="OverflowException"/>.) 
        ///   To explicitly define the style elements together with the culture-specific formatting information that can be present in <paramref name="value"/>, use the <see cref="Parse(String, NumberStyles, IFormatProvider)"/> method.
        ///   <para>
        ///   The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation whose <see cref="IFormatProvider.GetFormat"/> method returns a <see cref="NumberFormatInfo"/> object that provides culture-specific information about the format of <paramref name="value"/>. 
        ///   There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting information to the parse operation:
        ///   </para>
        ///   <list type="bullet">
        ///     <item>You can pass the actual <see cref="NumberFormatInfo"/> object that provides formatting information. (Its implementation of <see cref="IFormatProvider.GetFormat"/> simply returns itself.)</item>
        ///     <item>You can pass a <see cref="CultureInfo"/> object that specifies the culture whose formatting is to be used. Its <see cref="CultureInfo.NumberFormat"/> property provides formatting information.</item>
        ///     <item>You can pass a custom <see cref="IFormatProvider"/> implementation. Its <see cref="IFormatProvider.GetFormat"/> method must instantiate and return the <see cref="NumberFormatInfo"/> object that provides formatting information.</item>
        ///   </list>
        ///   If provider is <see langword="null"/>, the <see cref="NumberFormatInfo"/> object for the current culture is used.
        /// </remarks>
        public static UInt48 Parse(string value, IFormatProvider provider)
        {
            return Parse(value, NumberStyles.Integer, provider);
        }

        /// <summary>
        /// Converts the string representation of a number in a specified style to its 48-bit unsigned integer equivalent.
        /// </summary>
        /// <param name="value">
        /// A string that represents the number to convert. 
        /// The string is interpreted by using the style specified by the <paramref name="style"/> parameter.
        /// </param>
        /// <param name="style">
        /// A bitwise combination of the enumeration values that specifies the permitted format of <paramref name="value"/>. 
        /// A typical value to specify is <see cref="NumberStyles.Integer"/>.
        /// </param>
        /// <returns>A 48-bit unsigned integer equivalent to the number specified in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="style"/> is not a <see cref="NumberStyles"/> value.
        ///   <para>-or-</para>
        ///   <para><paramref name="style"/> is not a combination of <see cref="NumberStyles.AllowHexSpecifier"/> and <see cref="NumberStyles.HexNumber"/> values.</para>
        /// </exception>
        /// <exception cref="FormatException">The <paramref name="value"/> parameter is not in a format compliant with <paramref name="style"/>.</exception>
        /// <exception cref="OverflowException">
        ///   The <paramref name="value"/> parameter represents a number less than <see cref="UInt48.MinValue"/> or greater than <see cref="UInt48.MaxValue"/>. 
        ///   <para>-or-</para>
        ///   <para><paramref name="value"/> includes non-zero, fractional digits.</para>
        /// </exception>
        /// <remarks>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space, the positive or negative sign symbol, the group separator symbol, or the decimal point symbol) that are allowed in the s parameter for the parse operation to succeed. 
        /// It must be a combination of bit flags from the <see cref="NumberStyles"/> enumeration.
        /// <para>Depending on the value of style, the <paramref name="value"/> parameter may include the following elements:</para>
        /// <para>[ws][$][sign][digits,]digits[.fractional_digits][E[sign]exponential_digits][ws]</para>
        /// <para>
        ///   Elements in square brackets ([ and ]) are optional. 
        ///   If <paramref name="style"/> includes <see cref="NumberStyles.AllowHexSpecifier"/>, the <paramref name="value"/> parameter may contain the following elements:
        /// </para>
        /// <para>[ws]hexdigits[ws]</para>
        /// <para>The following table describes each element.</para>
        ///   <list type="table">
        ///     <listheader>
        ///       <term>Element</term>
        ///       <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///       <term>ws</term>
        ///       <description>
        ///       Optional white space. 
        ///       White space can appear at the start of <paramref name="value"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowLeadingWhite"/> flag, 
        ///       and it can appear at the end of <paramref name="style"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowTrailingWhite"/> flag.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <term>$</term>
        ///       <description>
        ///       A culture-specific currency symbol. 
        ///       Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/> and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the current culture. 
        ///       The current culture's currency symbol can appear in <paramref name="value"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowCurrencySymbol"/> flag.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <term>sign</term>
        ///       <description>
        ///       An optional sign. 
        ///       The sign can appear at the start of <paramref name="value"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowLeadingSign"/> flag, and it can appear at the end of <paramref name="value"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowTrailingSign"/> flag. 
        ///       Parentheses can be used in <paramref name="value"/> to indicate a negative value if <paramref name="style"/> includes the <see cref="NumberStyles.AllowParentheses"/> flag. 
        ///       However, the negative sign symbol can be used only with zero; otherwise, the method throws an <see cref="OverflowException"/>.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <term>digits</term>
        ///       <description>A sequence of digits from 0 through 9.</description>
        ///     </item>
        ///     <item>
        ///       <term>.</term>
        ///       <description>
        ///       A culture-specific decimal point symbol. 
        ///       The current culture's decimal point symbol can appear in <paramref name="value"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowDecimalPoint"/> flag.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <term>,</term>
        ///       <description>
        ///       A culture-specific group separator symbol. 
        ///       The current culture's group separator can appear in <paramref name="value"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowThousands"/> flag.</description>
        ///     </item>
        ///     <item>
        ///       <term>fractional_digits</term>
        ///       <description>
        ///       One or more occurrences of the digit 0-9 if <paramref name="style"/> includes the <see cref="NumberStyles.AllowExponent"/> flag, 
        ///       or one or more occurrences of the digit 0 if it does not. 
        ///       Fractional digits can appear in <paramref name="value"/> only if <paramref name="style"/> includes the <see cref="NumberStyles.AllowDecimalPoint"/> flag.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <term>E</term>
        ///       <description>
        ///       The "e" or "E" character, which indicates that the value is represented in exponential (scientific) notation. 
        ///       The <paramref name="value"/> parameter can represent a number in exponential notation if <paramref name="style"/> includes the <see cref="NumberStyles.AllowExponent"/> flag.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <term>exponential_digits</term>
        ///       <description>
        ///       A sequence of digits from 0 through 9. 
        ///       The <paramref name="value"/> parameter can represent a number in exponential notation if <paramref name="style"/> includes the <see cref="NumberStyles.AllowExponent"/> flag.
        ///       </description>
        ///     </item>
        ///     <item><term>hexdigits</term><description>A sequence of hexadecimal digits from 0 through f, or 0 through F.</description></item>
        ///   </list>
        ///   A string with decimal digits only (which corresponds to the <see cref="NumberStyles.None"/> style) always parses successfully. 
        ///   Most of the remaining <see cref="NumberStyles"/> members control elements that may be present, but are not required to be present, in this input string. 
        ///   The following table indicates how individual <see cref="NumberStyles"/> members affect the elements that may be present in <paramref name="value"/>.
        ///   <list type="table">
        ///     <listheader>
        ///         <term><see cref="NumberStyles"/> value</term>
        ///         <description>Elements permitted in <paramref name="value"/> in addition to digits</description>
        ///     </listheader>
        ///     <item><term><see cref="NumberStyles.None"/></term><description>The digits element only.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowDecimalPoint"/></term><description>The decimal point (.) and fractional_digits elements. However, if <paramref name="style"/> does not include the <see cref="NumberStyles.AllowExponent"/> flag, fractional_digits must consist of only one or more 0 digits; otherwise, an <see cref="OverflowException"/> is thrown.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowExponent"/></term><description>The "e" or "E" character, which indicates exponential notation, along with exponential_digits.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowLeadingWhite"/></term><description>The ws element at the start of <paramref name="value"/>.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowTrailingWhite"/></term><description>The ws element at the end of <paramref name="value"/>.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowLeadingSign"/></term><description>The sign element at the start of <paramref name="value"/>.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowTrailingSign"/></term><description>The sign element at the end of <paramref name="value"/>.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowParentheses"/></term><description>The sign element in the form of parentheses enclosing the numeric value.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowThousands"/></term><description>The group separator (,) element. </description></item>
        ///     <item><term><see cref="NumberStyles.AllowCurrencySymbol"/></term><description>The currency ($) element.</description></item>
        ///     <item><term><see cref="NumberStyles.Currency"/></term><description>All elements. However, <paramref name="value"/> cannot represent a hexadecimal number or a number in exponential notation.</description></item>
        ///     <item><term><see cref="NumberStyles.Float"/></term><description>The ws element at the start or end of <paramref name="value"/>, sign at the start of <paramref name="value"/>, and the decimal point (.) symbol. The <paramref name="value"/> parameter can also use exponential notation.</description></item>
        ///     <item><term><see cref="NumberStyles.Number"/></term><description>The ws, sign, group separator (,), and decimal point (.) elements.</description></item>
        ///     <item><term><see cref="NumberStyles.Any"/></term><description>All elements. However, <paramref name="value"/> cannot represent a hexadecimal number.</description></item>
        ///   </list>
        ///   Unlike the other <see cref="NumberStyles"/> values, which allow for, but do not require, the presence of particular style elements in <paramref name="value"/>, the <see cref="NumberStyles.AllowHexSpecifier"/> style value means that the individual numeric characters in <paramref name="value"/> are always interpreted as hexadecimal characters. 
        ///   Valid hexadecimal characters are 0-9, A-F, and a-f. 
        ///   The only other flags that can be combined with the <paramref name="style"/> parameter are <see cref="NumberStyles.AllowLeadingWhite"/> and <see cref="NumberStyles.AllowTrailingWhite"/>. 
        ///   (The <see cref="NumberStyles"/> enumeration includes a composite number style, <see cref="NumberStyles.HexNumber"/>, that includes both white-space flags.)
        ///   <note>
        ///   If <paramref name="value"/> is the string representation of a hexadecimal number, it cannot be preceded by any decoration (such as 0x or &amp;h) that differentiates it as a hexadecimal number. 
        ///   This causes the conversion to fail.
        ///   </note>
        ///   The <paramref name="value"/> parameter is parsed by using the formatting information in a <see cref="NumberFormatInfo"/> object that is initialized for the current system culture. 
        ///   To specify the culture whose formatting information is used for the parse operation, call the <see cref="Parse(String, NumberStyles, IFormatProvider)"/> overload.
        /// </remarks>
        public static UInt48 Parse(string value, NumberStyles style)
        {
            return Parse(value, style, CultureInfo.CurrentCulture.NumberFormat);
        }

        /// <summary>
        /// Converts the string representation of a number in a specified style and culture-specific format to its 48-bit unsigned integer equivalent.
        /// </summary>
        /// <param name="value">
        /// A string that represents the number to convert. 
        /// The string is interpreted by using the style specified by the <paramref name="style"/> parameter.
        /// </param>
        /// <param name="style">
        /// A bitwise combination of enumeration values that indicates the style elements that can be present in <paramref name="value"/>. 
        /// A typical value to specify is <see cref="NumberStyles.Integer"/>.
        /// </param>
        /// <param name="provider">An object that supplies culture-specific formatting information about <paramref name="value"/>.</param>
        /// <returns>A 48-bit unsigned integer equivalent to the number specified in <paramref name="value"/>.</returns>
        /// <exception cref="ArgumentNullException">The <paramref name="value"/> parameter is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentException">
        ///   <paramref name="style"/> is not a <see cref="NumberStyles"/> value.
        ///   <para>-or-</para>
        ///   <para><paramref name="style"/> is not a combination of <see cref="NumberStyles.AllowHexSpecifier"/> and <see cref="NumberStyles.HexNumber"/> values.</para>
        /// </exception>
        /// <exception cref="FormatException">The <paramref name="value"/> parameter is not in a format compliant with <paramref name="style"/>.</exception>
        /// <exception cref="OverflowException">
        ///   The <paramref name="value"/> parameter represents a number less than <see cref="UInt48.MinValue"/> or greater than <see cref="UInt48.MaxValue"/>. 
        ///   <para>-or-</para>
        ///   <para><paramref name="value"/> includes non-zero, fractional digits.</para>
        /// </exception>
        /// <remarks>
        /// The <paramref name="style"/> parameter defines the style elements (such as white space, the positive or negative sign symbol, the group separator symbol, or the decimal point symbol) that are allowed in the s parameter for the parse operation to succeed. 
        /// It must be a combination of bit flags from the <see cref="NumberStyles"/> enumeration.
        /// <para>Depending on the value of style, the <paramref name="value"/> parameter may include the following elements:</para>
        /// <para>[ws][$][sign][digits,]digits[.fractional_digits][E[sign]exponential_digits][ws]</para>
        /// <para>
        ///   Elements in square brackets ([ and ]) are optional. 
        ///   If <paramref name="style"/> includes <see cref="NumberStyles.AllowHexSpecifier"/>, the <paramref name="value"/> parameter may contain the following elements:
        /// </para>
        /// <para>[ws]hexdigits[ws]</para>
        /// <para>The following table describes each element.</para>
        ///   <list type="table">
        ///     <listheader>
        ///       <term>Element</term>
        ///       <description>Description</description>
        ///     </listheader>
        ///     <item>
        ///       <term>ws</term>
        ///       <description>
        ///       Optional white space. 
        ///       White space can appear at the start of <paramref name="value"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowLeadingWhite"/> flag, 
        ///       and it can appear at the end of <paramref name="style"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowTrailingWhite"/> flag.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <term>$</term>
        ///       <description>
        ///       A culture-specific currency symbol. 
        ///       Its position in the string is defined by the <see cref="NumberFormatInfo.CurrencyNegativePattern"/> and <see cref="NumberFormatInfo.CurrencyPositivePattern"/> properties of the <see cref="NumberFormatInfo"/> object that is returned by the <see cref="IFormatProvider.GetFormat"/> method of the provider parameter. 
        ///       The currency symbol can appear in <paramref name="value"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowCurrencySymbol"/> flag.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <term>sign</term>
        ///       <description>
        ///       An optional sign. 
        ///       The sign can appear at the start of <paramref name="value"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowLeadingSign"/> flag, and it can appear at the end of <paramref name="value"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowTrailingSign"/> flag. 
        ///       Parentheses can be used in <paramref name="value"/> to indicate a negative value if <paramref name="style"/> includes the <see cref="NumberStyles.AllowParentheses"/> flag.
        ///       However, the negative sign symbol can be used only with zero; otherwise, the method throws an <see cref="OverflowException"/>.
        ///       </description>
        ///     </item>
        ///     <item><term>digits</term><description>A sequence of digits from 0 through 9.</description></item>
        ///     <item>
        ///       <term>.</term>
        ///       <description>
        ///       A culture-specific decimal point symbol. 
        ///       The current culture's decimal point symbol can appear in <paramref name="value"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowDecimalPoint"/> flag.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <term>,</term>
        ///       <description>
        ///       A culture-specific group separator symbol. 
        ///       The current culture's group separator can appear in <paramref name="value"/> if <paramref name="style"/> includes the <see cref="NumberStyles.AllowThousands"/> flag.</description>
        ///     </item>
        ///     <item>
        ///       <term>fractional_digits</term>
        ///       <description>
        ///       One or more occurrences of the digit 0-9 if <paramref name="style"/> includes the <see cref="NumberStyles.AllowExponent"/> flag, 
        ///       or one or more occurrences of the digit 0 if it does not. 
        ///       Fractional digits can appear in <paramref name="value"/> only if <paramref name="style"/> includes the <see cref="NumberStyles.AllowDecimalPoint"/> flag.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <term>E</term>
        ///       <description>
        ///       The "e" or "E" character, which indicates that the value is represented in exponential (scientific) notation. 
        ///       The <paramref name="value"/> parameter can represent a number in exponential notation if <paramref name="style"/> includes the <see cref="NumberStyles.AllowExponent"/> flag.
        ///       </description>
        ///     </item>
        ///     <item>
        ///       <term>exponential_digits</term>
        ///       <description>
        ///       A sequence of digits from 0 through 9. 
        ///       The <paramref name="value"/> parameter can represent a number in exponential notation if <paramref name="style"/> includes the <see cref="NumberStyles.AllowExponent"/> flag.
        ///       </description>
        ///     </item>
        ///     <item><term>hexdigits</term><description>A sequence of hexadecimal digits from 0 through f, or 0 through F.</description></item>
        ///   </list>
        ///   A string with decimal digits only (which corresponds to the <see cref="NumberStyles.None"/> style) always parses successfully. 
        ///   Most of the remaining <see cref="NumberStyles"/> members control elements that may be present, but are not required to be present, in this input string. 
        ///   The following table indicates how individual <see cref="NumberStyles"/> members affect the elements that may be present in <paramref name="value"/>.
        ///   <list type="table">
        ///     <listheader>
        ///         <term><see cref="NumberStyles"/> value</term>
        ///         <description>Elements permitted in <paramref name="value"/> in addition to digits</description>
        ///     </listheader>
        ///     <item><term><see cref="NumberStyles.None"/></term><description>The digits element only.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowDecimalPoint"/></term><description>The decimal point (.) and fractional_digits elements. However, if <paramref name="style"/> does not include the <see cref="NumberStyles.AllowExponent"/> flag, fractional_digits must consist of only one or more 0 digits; otherwise, an <see cref="OverflowException"/> is thrown.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowExponent"/></term><description>The "e" or "E" character, which indicates exponential notation, along with exponential_digits.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowLeadingWhite"/></term><description>The ws element at the start of <paramref name="value"/>.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowTrailingWhite"/></term><description>The ws element at the end of <paramref name="value"/>.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowLeadingSign"/></term><description>The sign element at the start of <paramref name="value"/>.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowTrailingSign"/></term><description>The sign element at the end of <paramref name="value"/>.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowParentheses"/></term><description>The sign element in the form of parentheses enclosing the numeric value.</description></item>
        ///     <item><term><see cref="NumberStyles.AllowThousands"/></term><description>The group separator (,) element. </description></item>
        ///     <item><term><see cref="NumberStyles.AllowCurrencySymbol"/></term><description>The currency ($) element.</description></item>
        ///   <item><term><see cref="NumberStyles.Currency"/></term><description>All elements. However, <paramref name="value"/> cannot represent a hexadecimal number or a number in exponential notation.</description></item>
        ///     <item><term><see cref="NumberStyles.Float"/></term><description>The ws element at the start or end of <paramref name="value"/>, sign at the start of <paramref name="value"/>, and the decimal point (.) symbol. The <paramref name="value"/> parameter can also use exponential notation.</description></item>
        ///     <item><term><see cref="NumberStyles.Number"/></term><description>The ws, sign, group separator (,), and decimal point (.) elements.</description></item>
        ///     <item><term><see cref="NumberStyles.Any"/></term><description>All elements. However, <paramref name="value"/> cannot represent a hexadecimal number.</description></item>
        ///   </list>
        ///   Unlike the other <see cref="NumberStyles"/> values, which allow for, but do not require, the presence of particular style elements in <paramref name="value"/>, the <see cref="NumberStyles.AllowHexSpecifier"/> style value means that the individual numeric characters in <paramref name="value"/> are always interpreted as hexadecimal characters. 
        ///   Valid hexadecimal characters are 0-9, A-F, and a-f. 
        ///   The only other flags that can be combined with the <paramref name="style"/> parameter are <see cref="NumberStyles.AllowLeadingWhite"/> and <see cref="NumberStyles.AllowTrailingWhite"/>. 
        ///   (The <see cref="NumberStyles"/> enumeration includes a composite number style, <see cref="NumberStyles.HexNumber"/>, that includes both white-space flags.) 
        ///   <note>
        ///   If <paramref name="value"/> is the string representation of a hexadecimal number, it cannot be preceded by any decoration (such as 0x or &amp;h) that differentiates it as a hexadecimal number. 
        ///   This causes the conversion to fail.
        ///   </note>
        ///   The <paramref name="provider"/> parameter is an <see cref="IFormatProvider"/> implementation whose <see cref="IFormatProvider.GetFormat"/> method returns a <see cref="NumberFormatInfo"/> object that provides culture-specific information about the format of <paramref name="value"/>. 
        ///   There are three ways to use the <paramref name="provider"/> parameter to supply custom formatting information to the parse operation:
        ///   <list type="bullet">
        ///     <item>You can pass the actual <see cref="NumberFormatInfo"/> object that provides formatting information. (Its implementation of <see cref="IFormatProvider.GetFormat"/> simply returns itself.)</item>
        ///     <item>You can pass a <see cref="CultureInfo"/> object that specifies the culture whose formatting is to be used. Its <see cref="CultureInfo.NumberFormat"/> property provides formatting information.</item>
        ///     <item>You can pass a custom <see cref="IFormatProvider"/> implementation. Its <see cref="IFormatProvider.GetFormat"/> method must instantiate and return the <see cref="NumberFormatInfo"/> object that provides formatting information.</item>
        ///   </list>
        ///   If provider is <see langword="null"/>, the <see cref="NumberFormatInfo"/> object for the current culture is used.
        /// </remarks>
        public static UInt48 Parse(string value, NumberStyles style, IFormatProvider provider)
        {
            ulong parsedValue;
            try
            {
                parsedValue = ulong.Parse(value, style, provider);
            }
            catch (OverflowException)
            {
                throw new OverflowException("Value was either too large or too small for a UInt48");
            }
            if (parsedValue > MaxValue)
                throw new OverflowException("Value was too large for a UInt48");

            return (UInt48)parsedValue;
        }

        /// <summary>
        /// Converts a 32 bit unsigned integer to a 48 bit unsigned integer by taking all the 32 bits.
        /// </summary>
        /// <param name="value">The 32 bit value to convert.</param>
        /// <returns>The 48 bit value created by taking all the 32 bits of the 32bit value.</returns>
        public static implicit operator UInt48(uint value)
        {
            return new UInt48(value);
        }

        /// <summary>
        /// Converts a 64 bit signed integer to a 48 bit unsigned integer by taking the 48 least significant bits.
        /// </summary>
        /// <param name="value">The 64 bit value to convert.</param>
        /// <returns>The 48 bit value created by taking the 48 least significant bits of the 64 bit value.</returns>
        public static explicit operator UInt48(long value)
        {
            return new UInt48(value);
        }

        /// <summary>
        /// Converts a 64 bit unsigned integer to a 48 bit unsigned integer by taking the 48 least significant bits.
        /// </summary>
        /// <param name="value">The 64 bit value to convert.</param>
        /// <returns>The 48 bit value created by taking the 48 least significant bits of the 64 bit value.</returns>
        public static explicit operator UInt48(ulong value)
        {
            return new UInt48((long)value);
        }

        /// <summary>
        /// Converts the 48 bits unsigned integer to a 64 bits signed integer.
        /// </summary>
        /// <param name="value">The 48 bit value to convert.</param>
        /// <returns>The 64 bit value converted from the 48 bit value.</returns>
        public static implicit operator long(UInt48 value)
        {
            return value.ToLong();
        }

        /// <summary>
        /// Converts the 48 bits unsigned integer to a 64 bits unsigned integer.
        /// </summary>
        /// <param name="value">The 48 bit value to convert.</param>
        /// <returns>The 64 bit value converted from the 48 bit value.</returns>
        public static implicit operator ulong(UInt48 value)
        {
            return (ulong)value.ToLong();
        }

        /// <summary>
        /// Converts the 48 bits unsigned integer to an 8 bits unsigned integer.
        /// </summary>
        /// <param name="value">The 48 bit value to convert.</param>
        /// <returns>The 8 bit value converted from the 48 bit value.</returns>
        public static explicit operator byte(UInt48 value)
        {
            return value.ToByte();
        }

        /// <summary>
        /// Returns true iff the two values represent the same value.
        /// </summary>
        /// <param name="other">The value to compare to.</param>
        /// <returns>True iff the two values represent the same value.</returns>
        public bool Equals(UInt48 other)
        {
            return _mostSignificant == other._mostSignificant &&
                   _leastSignificant == other._leastSignificant;
        }

        /// <summary>
        /// Indicates whether this instance and a specified object are equal.
        /// </summary>
        /// <returns>
        /// true if <paramref name="obj"/> and this instance are the same type and represent the same value; otherwise, false.
        /// </returns>
        /// <param name="obj">Another object to compare to. </param><filterpriority>2</filterpriority>
        public override bool Equals(object obj)
        {
            return (obj is UInt48) &&
                   Equals((UInt48)obj);
        }

        /// <summary>
        /// Returns true iff the two values represent the same value.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>True iff the two values represent the same value.</returns>
        public static bool operator ==(UInt48 value1, UInt48 value2)
        {
            return value1.Equals(value2);
        }

        /// <summary>
        /// Returns true iff the two values represent different values.
        /// </summary>
        /// <param name="value1">The first value to compare.</param>
        /// <param name="value2">The second value to compare.</param>
        /// <returns>True iff the two values represent different values.</returns>
        public static bool operator !=(UInt48 value1, UInt48 value2)
        {
            return !(value1 == value2);
        }

        /// <summary>
        /// Returns the hash code for this instance.
        /// </summary>
        /// <returns>
        /// A 32-bit signed integer that is the hash code for this instance.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override int GetHashCode()
        {
            return ((long)this).GetHashCode();
        }

        /// <summary>
        /// Returns the fully qualified type name of this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> containing a fully qualified type name.
        /// </returns>
        /// <filterpriority>2</filterpriority>
        public override string ToString()
        {
            return ((long)this).ToString(CultureInfo.InvariantCulture);
        }

        public string ToString(string format)
        {
            return ((long)this).ToString(format);
        }

        private UInt48(long value)
        {
            _mostSignificant = (ushort)(value >> 32);
            _leastSignificant = (uint)value;
        }

        private long ToLong()
        {
            return (((long)_mostSignificant) << 32) + _leastSignificant;
        }

        private byte ToByte()
        {
            return (byte)_leastSignificant;
        }

        private readonly uint _leastSignificant;
        private readonly ushort _mostSignificant;
    }
}