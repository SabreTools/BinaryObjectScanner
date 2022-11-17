using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;

namespace BurnOutSharp.Builder
{
    /// <summary>
    /// ASN.1 type indicators
    /// </summary>
    [Flags]
    public enum ASN1Type : byte
    {
        #region Modifiers

        V_ASN1_UNIVERSAL = 0x00,
        V_ASN1_PRIMITIVE_TAG = 0x1F,
        V_ASN1_CONSTRUCTED = 0x20,
        V_ASN1_APPLICATION = 0x40,
        V_ASN1_CONTEXT_SPECIFIC = 0x80,
        V_ASN1_PRIVATE = 0xC0,

        #endregion

        #region Types

        V_ASN1_EOC = 0x00,
        V_ASN1_BOOLEAN = 0x01,
        V_ASN1_INTEGER = 0x02,
        V_ASN1_BIT_STRING = 0x03,
        V_ASN1_OCTET_STRING = 0x04,
        V_ASN1_NULL = 0x05,
        V_ASN1_OBJECT = 0x06,
        V_ASN1_OBJECT_DESCRIPTOR = 0x07,
        V_ASN1_EXTERNAL = 0x08,
        V_ASN1_REAL = 0x09,
        V_ASN1_ENUMERATED = 0x0A,
        V_ASN1_UTF8STRING = 0x0C,
        V_ASN1_SEQUENCE = 0x10,
        V_ASN1_SET = 0x11,
        V_ASN1_NUMERICSTRING = 0x12,
        V_ASN1_PRINTABLESTRING = 0x13,
        V_ASN1_T61STRING = 0x14,
        V_ASN1_TELETEXSTRING = 0x14,
        V_ASN1_VIDEOTEXSTRING = 0x15,
        V_ASN1_IA5STRING = 0x16,
        V_ASN1_UTCTIME = 0x17,
        V_ASN1_GENERALIZEDTIME = 0x18,
        V_ASN1_GRAPHICSTRING = 0x19,
        V_ASN1_ISO64STRING = 0x1A,
        V_ASN1_VISIBLESTRING = 0x1A,
        V_ASN1_GENERALSTRING = 0x1B,
        V_ASN1_UNIVERSALSTRING = 0x1C,
        V_ASN1_BMPSTRING = 0x1E,

        #endregion
    }

    /// <summary>
    /// ASN.1 Parser
    /// </summary>
    public class AbstractSyntaxNotationOne
    {
        /// <summary>
        /// Parse a byte array into a DER-encoded ASN.1 structure
        /// </summary>
        /// <param name="data">Byte array representing the data</param>
        /// <param name="pointer">Current pointer into the data</param>
        /// <returns></returns>
        public static List<ASN1TypeLengthValue> Parse(byte[] data, int pointer)
        {
            // Create the output list to return
            var topLevelValues = new List<ASN1TypeLengthValue>();

            // Loop through the data and return all top-level values
            while (pointer < data.Length)
            {
                var topLevelValue = new ASN1TypeLengthValue(data, ref pointer);
                topLevelValues.Add(topLevelValue);
            }

            return topLevelValues;
        }
    }

    /// <summary>
    /// ASN.1 type/length/value class that all types are based on
    /// </summary>
    public class ASN1TypeLengthValue
    {
        /// <summary>
        /// The ASN.1 type
        /// </summary>
        public ASN1Type Type { get; private set; }

        /// <summary>
        /// Length of the value
        /// </summary>
        public ulong Length { get; private set; }

        /// <summary>
        /// Generic value associated with <see cref="Type"/>
        /// </summary>
        public object Value { get; private set; }

        /// <summary>
        /// Read from the source data array at an index
        /// </summary>
        /// <param name="data">Byte array representing data to read</param>
        /// <param name="index">Index within the array to read at</param>
        public ASN1TypeLengthValue(byte[] data, ref int index)
        {
            // Get the type and modifiers
            this.Type = (ASN1Type)data[index++];

            // If we have an end indicator, we just return
            if (this.Type == ASN1Type.V_ASN1_EOC)
                return;

            // Get the length of the value
            this.Length = ReadLength(data, ref index);

            // Read the value
            if (this.Type.HasFlag(ASN1Type.V_ASN1_CONSTRUCTED))
            {
                var valueList = new List<ASN1TypeLengthValue>();

                int currentIndex = index;
                while (index < currentIndex + (int)this.Length)
                {
                    valueList.Add(new ASN1TypeLengthValue(data, ref index));
                }

                this.Value = valueList.ToArray();
            }
            else
            {
                // TODO: Get more granular based on type
                this.Value = data.ReadBytes(ref index, (int)this.Length);
            }
        }

        /// <summary>
        /// Format the TLV as a string
        /// </summary>
        /// <param name="paddingLevel">Padding level of the item when formatting</param>
        /// <returns>String representing the TLV, if possible</returns>
        public string Format(int paddingLevel = 0)
        {
            // Create the left-padding string
            string padding = new string(' ', paddingLevel);

            // If we have an invalid item
            if (this.Type == 0)
                return $"{padding}UNKNOWN TYPE";

            // Create the string builder
            StringBuilder formatBuilder = new StringBuilder();

            // Append the type
            formatBuilder.Append($"{padding}Type: {this.Type}");
            if (this.Type == ASN1Type.V_ASN1_EOC)
                return formatBuilder.ToString();

            // Append the length
            formatBuilder.Append($", Length: {this.Length}");
            if (this.Length == 0)
                return formatBuilder.ToString();

            // If we have a constructed type
            if (this.Type.HasFlag(ASN1Type.V_ASN1_CONSTRUCTED))
            {
                var valueAsObjectArray = this.Value as ASN1TypeLengthValue[];
                if (valueAsObjectArray == null)
                {
                    formatBuilder.Append(", Value: [INVALID DATA TYPE]");
                    return formatBuilder.ToString();
                }

                formatBuilder.Append(", Value:\n");
                for (int i = 0; i < valueAsObjectArray.Length; i++)
                {
                    var child = valueAsObjectArray[i];
                    string childString = child.Format(paddingLevel + 1);
                    formatBuilder.Append($"{childString}\n");
                }

                return formatBuilder.ToString().TrimEnd('\n');
            }

            // Get the value as a byte array
            byte[] valueAsByteArray = this.Value as byte[];
            if (valueAsByteArray == null)
            {
                formatBuilder.Append(", Value: [INVALID DATA TYPE]");
                return formatBuilder.ToString();
            }

            // If we have a primitive type
            switch (this.Type)
            {
                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-boolean"/>
                case ASN1Type.V_ASN1_BOOLEAN:
                    if (this.Length > 1 || valueAsByteArray.Length > 1)
                        formatBuilder.Append($" [Expected length of 1]");

                    bool booleanValue = valueAsByteArray[0] == 0x00 ? false : true;
                    formatBuilder.Append($", Value: {booleanValue}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-integer"/>
                case ASN1Type.V_ASN1_INTEGER:
                    Array.Reverse(valueAsByteArray);
                    BigInteger integerValue = new BigInteger(valueAsByteArray);
                    formatBuilder.Append($", Value: {integerValue}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-bit-string"/>
                case ASN1Type.V_ASN1_BIT_STRING:
                    // TODO: Read into a BitArray and print that out instead?
                    int unusedBits = valueAsByteArray[0];
                    formatBuilder.Append($", Value with {unusedBits} unused bits: {BitConverter.ToString(valueAsByteArray.Skip(1).ToArray()).Replace('-', ' ')}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-octet-string"/>
                case ASN1Type.V_ASN1_OCTET_STRING:
                    formatBuilder.Append($", Value: {BitConverter.ToString(valueAsByteArray).Replace('-', ' ')}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-object-identifier"/>
                /// <see cref="http://snmpsharpnet.com/index.php/2009/03/02/ber-encoding-and-decoding-oid-values/"/>
                case ASN1Type.V_ASN1_OBJECT:
                    // Derive array of values
                    ulong[] objectNodes = ObjectIdentifier.ParseDERIntoArray(valueAsByteArray, this.Length);

                    // Append the dot and modified OID-IRI notations
                    string dotNotationString = ObjectIdentifier.ParseOIDToDotNotation(objectNodes);
                    string oidIriString = ObjectIdentifier.ParseOIDToModifiedOIDIRI(objectNodes);
                    formatBuilder.Append($", Value: {dotNotationString} ({oidIriString})");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-utf8string"/>
                case ASN1Type.V_ASN1_UTF8STRING:
                    formatBuilder.Append($", Value: {Encoding.UTF8.GetString(valueAsByteArray)}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-printablestring"/>
                case ASN1Type.V_ASN1_PRINTABLESTRING:
                    formatBuilder.Append($", Value: {Encoding.ASCII.GetString(valueAsByteArray)}");
                    break;

                //case ASN1Type.V_ASN1_T61STRING:
                case ASN1Type.V_ASN1_TELETEXSTRING:
                    formatBuilder.Append($", Value: {Encoding.ASCII.GetString(valueAsByteArray)}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-ia5string"/>
                case ASN1Type.V_ASN1_IA5STRING:
                    formatBuilder.Append($", Value: {Encoding.ASCII.GetString(valueAsByteArray)}");
                    break;

                case ASN1Type.V_ASN1_UTCTIME:
                    string utctimeString = Encoding.ASCII.GetString(valueAsByteArray);
                    if (DateTime.TryParse(utctimeString, out DateTime utctimeDateTime))
                        formatBuilder.Append($", Value: {utctimeDateTime}");
                    else
                        formatBuilder.Append($", Value: {utctimeString}");
                    break;

                /// <see href="https://learn.microsoft.com/en-us/windows/win32/seccertenroll/about-bmpstring"/>
                case ASN1Type.V_ASN1_BMPSTRING:
                    formatBuilder.Append($", Value: {Encoding.Unicode.GetString(valueAsByteArray)}");
                    break;

                default:
                    formatBuilder.Append($", Value (Unknown Format): {BitConverter.ToString(this.Value as byte[]).Replace('-', ' ')}");
                    break;
            }

            // Return the formatted string
            return formatBuilder.ToString();
        }

        /// <summary>
        /// Reads the length field for a type
        /// </summary>
        /// <param name="data">Byte array representing data to read</param>
        /// <param name="index">Index within the array to read at</param>
        /// <returns>The length value read from the array</returns>
        private static ulong ReadLength(byte[] data, ref int index)
        {
            // If we have invalid data, throw an exception
            if (data == null || index < 0 && index >= data.Length)
                throw new ArgumentException();

            // Read the first byte, assuming it's the length
            byte length = data[index++];

            // If the bit 7 is not set, then use the value as it is
            if ((length & 0x80) == 0)
                return length;

            // Otherwise, use the value as the number of remaining bytes to read
            int bytesToRead = length & ~0x80;
            byte[] bytesRead = data.ReadBytes(ref index, bytesToRead);

            // TODO: Write extensions to read big-endian

            // Reverse the bytes to be in big-endian order
            Array.Reverse(bytesRead);

            switch (bytesRead.Length)
            {
                case 1:
                    return bytesRead[0];
                case 2:
                    return BitConverter.ToUInt16(bytesRead, 0);
                case 3:
                    Array.Resize(ref bytesRead, 4);
                    goto case 4;
                case 4:
                    return BitConverter.ToUInt32(bytesRead, 0);
                case 5:
                case 6:
                case 7:
                    Array.Resize(ref bytesRead, 8);
                    goto case 8;
                case 8:
                    return BitConverter.ToUInt64(bytesRead, 0);
                default:
                    throw new InvalidOperationException();
            }
        }
    }
}
