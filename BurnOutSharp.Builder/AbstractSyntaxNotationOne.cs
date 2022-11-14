using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
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
        /// <param name="paddingLevel">[UNUSED] Padding level of the item when formatting</param>
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
                    // The first byte contains nodes 1 and 2
                    int firstNode = Math.DivRem(valueAsByteArray[0], 40, out int secondNode);

                    // Create a list for all nodes
                    List<ulong> objectNodes = new List<ulong> { (ulong)firstNode, (ulong)secondNode };

                    // All other nodes are encoded uniquely
                    int objectValueOffset = 1;
                    while (objectValueOffset < (long)this.Length)
                    {
                        // If bit 7 is not set
                        if ((valueAsByteArray[objectValueOffset] & 0x80) == 0)
                        {
                            objectNodes.Add(valueAsByteArray[objectValueOffset]);
                            objectValueOffset++;
                            continue;
                        }

                        // Otherwise, read the encoded value in a loop
                        ulong dotValue = 0;
                        bool doneProcessing = false;

                        do
                        {
                            // Shift the current encoded value
                            dotValue <<= 7;

                            // If we have a leading zero byte, we're at the end
                            if ((valueAsByteArray[objectValueOffset] & 0x80) == 0)
                                doneProcessing = true;

                            // Clear the top byte
                            unchecked { valueAsByteArray[objectValueOffset] &= (byte)~0x80; }

                            // Add the new value to the result
                            dotValue |= valueAsByteArray[objectValueOffset];

                            // Increment the offset
                            objectValueOffset++;
                        } while (objectValueOffset < valueAsByteArray.Length && !doneProcessing);

                        // Add the parsed value to the output
                        objectNodes.Add(dotValue);
                    }

                    // Append the dot notation and human-readable values
                    string objectNodesReadable = ParseObjectValues(objectNodes);
                    formatBuilder.Append($", Value: {string.Join(".", objectNodes)} ({objectNodesReadable})");
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

        #region Object Identifier Parsing

        /// <summary>
        /// Create a human-readable version of a list of OID values
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <returns>Human-readable string on success, null on error</returns>
        /// <see href="http://www.oid-info.com/index.htm"/>
        private static string ParseObjectValues(List<ulong> values)
        {
            // If we have an invalid set of values, we can't do anything
            if (values == null || values.Count == 0)
                return null;

            // Set the initial index
            int index = 0;

            // Get a string builder for the path
            var nameBuilder = new StringBuilder();

            // Try to parse the standard value
            string standard = ParseOID(values, ref index);
            if (standard == null)
                return null;

            // Add the standard value to the output
            nameBuilder.Append(standard);

            // If we have no more items or we ended on an INVALID
            if (index == values.Count || standard.Contains("INVALID"))
                return nameBuilder.ToString();

            // TODO: Handle trailing values (like hashes)

            // Create and return the string
            return nameBuilder.ToString();
        }

        /// <summary>
        /// Parse an OID in dot notation into OID-IRI notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <param name="index">Current index into the list</param>
        /// <returns>OID-IRI formatted string, id possible</returns>
        /// <remarks>
        /// If a value does not have a fully-descriptive value, some parts may be replaced
        /// with the set of ASN.1 values encclosed in square brwckets.
        /// </remarks>
        /// <see href="http://<www.oid-info.com/index.htm"/>
        private static string ParseOID(List<ulong> values, ref int index)
        {
            // If we have an invalid set of values, we can't do anything
            if (values == null || values.Count == 0)
                return null;

            // If we have an invalid index, we can't do anything
            if (index < 0 || index >= values.Count)
                return null;

            #region Start

            oid_start:

            switch (values[index++])
            {
                case 0: goto oid_0;
                case 1: goto oid_1;
                case 2: goto oid_2;
                default: return $"/INVALID_{index}";
            }

        #endregion

        // itu-t, ccitt, itu-r
        #region 0.*

        oid_0:

            switch (values[index++])
            {
                case 0: goto oid_0_0;
                case 1: return "/ITU-T/1/[question]";
                case 2: goto oid_0_2;
                case 3: goto oid_0_3;
                // TODO: case 4: goto oid_0_4;
                case 5: return "/ITU-R/R-Recommendation";
                // TODO: case 9: goto oid_0_9;
                default: return $"/ITU-T/INVALID_{index}";
            };

        // recommendation
        #region 0.0.*

        oid_0_0:

            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/A";
                case 2: return "/ITU-T/Recommendation/B";
                case 3: return "/ITU-T/Recommendation/C";
                case 4: return "/ITU-T/Recommendation/D";
                // TODO: case 5: goto oid_0_0_5;
                case 6: return "/ITU-T/Recommendation/F";
                // TODO: case 7: goto oid_0_0_7;
                // TODO: case 8: goto oid_0_0_8;
                // TODO: case 9: goto oid_0_0_9;
                case 10: return "/ITU-T/Recommendation/J";
                case 11: return "/ITU-T/Recommendation/K";
                case 12: return "/ITU-T/Recommendation/L";
                // TODO: case 13: goto oid_0_0_13;
                case 14: return "/ITU-T/Recommendation/N";
                case 15: return "/ITU-T/Recommendation/O";
                case 16: return "/ITU-T/Recommendation/P";
                // TODO: case 17: goto oid_0_0_17;
                case 18: return "/ITU-T/Recommendation/R";
                case 19: return "/ITU-T/Recommendation/S";
                // TODO: case 20: goto oid_0_0_20;
                case 21: return "/ITU-T/Recommendation/U";
                // TODO: case 22: goto oid_0_0_22;
                // TODO: case 24: goto oid_0_0_24;
                case 25: return "/ITU-T/Recommendation/Y";
                case 26: return "/ITU-T/Recommendation/Z";
                case 59: return "/ITU-T/Recommendation/[xcmJobZeroDummy]";
                case 74: return "/ITU-T/Recommendation/[xcmSvcMonZeroDummy]";
                default: return $"/ITU-T/Recommendation/INVALID_{index}";
            }

        #endregion

        // administration
        #region 0.2.*

        oid_0_2:

            switch (values[index++])
            {
                case 202: return "/ITU-T/Administration/[Greece]";
                case 204:
                case 205: return "/ITU-T/Administration/[Kingdom of the Netherlands]";
                case 206: return "/ITU-T/Administration/[Belgium]";
                case 208:
                case 209:
                case 210:
                case 211: return "/ITU-T/Administration/[France]";
                case 212: return "/ITU-T/Administration/[Principality of Monaco]";
                case 213: return "/ITU-T/Administration/[Principality of ANDORRA]";
                case 214:
                case 215: return "/ITU-T/Administration/[Spain]";
                case 216: return "/ITU-T/Administration/[Hungary]";
                case 218: return "/ITU-T/Administration/[Bosnia and Herzegovina]";
                case 219: return "/ITU-T/Administration/[Republic of CROATIA]";
                case 220: return "/ITU-T/Administration/[Republic of Serbia]";
                case 222:
                case 223:
                case 224: return "/ITU-T/Administration/[Italy]";
                case 225: return "/ITU-T/Administration/[Vatican City State]";
                case 226: return "/ITU-T/Administration/[Romania]";
                // TODO: case 228: goto oid_0_2_228;
                case 229: return "/ITU-T/Administration/[Confederation of Switzerland]";
                case 230: return "/ITU-T/Administration/[Czech Republic]";
                case 231: return "/ITU-T/Administration/[Slovakia]";
                case 232:
                case 233: return "/ITU-T/Administration/[Austria]";
                case 234:
                case 235:
                case 236:
                case 237: return "/ITU-T/Administration/[United Kingdom of Great Britain and Northern Ireland]";
                case 238:
                case 239: return "/ITU-T/Administration/[Denmark]";
                case 240: return "/ITU-T/Administration/[Sweden]";
                // TODO: case 242: goto oid_0_2_242;
                case 243: return "/ITU-T/Administration/[Norway]";
                case 244: return "/ITU-T/Administration/[Finland]";
                case 246: return "/ITU-T/Administration/[Republic of LITHUANIA]";
                case 247: return "/ITU-T/Administration/[Republic of Latvia]";
                case 248: return "/ITU-T/Administration/[Republic of ESTONIA]";
                case 250:
                case 251: return "/ITU-T/Administration/[Russian Federation]";
                case 255: return "/ITU-T/Administration/[Ukraine]";
                case 257: return "/ITU-T/Administration/[Republic of Belarus]";
                case 259: return "/ITU-T/Administration/[Republic of Moldova]";
                case 260:
                case 261: return "/ITU-T/Administration/[Republic of Poland]";
                // TODO: case 262: goto oid_0_2_262;
                case 263:
                case 264:
                case 265: return "/ITU-T/Administration/[Federal Republic of Germany]";
                case 266: return "/ITU-T/Administration/[Gibraltar]";
                case 268:
                case 269: return "/ITU-T/Administration/[Portugal]";
                case 270: return "/ITU-T/Administration/[Luxembourg]";
                case 272: return "/ITU-T/Administration/[Ireland]";
                case 274: return "/ITU-T/Administration/[Iceland]";
                case 276: return "/ITU-T/Administration/[Republic of Albania]";
                case 278: return "/ITU-T/Administration/[Malta]";
                case 280: return "/ITU-T/Administration/[Republic of Cyprus]";
                case 282: return "/ITU-T/Administration/[Georgia]";
                case 283: return "/ITU-T/Administration/[Republic of ARMENIA]";
                case 284: return "/ITU-T/Administration/[Republic of Bulgaria]";
                case 286: return "/ITU-T/Administration/[Turkey]";
                case 288: return "/ITU-T/Administration/[Faroe Islands]";
                case 290: return "/ITU-T/Administration/[Greenland]";
                case 292: return "/ITU-T/Administration/[Republic of San Marino]";
                case 293: return "/ITU-T/Administration/[Republic of SLOVENIA]";
                case 294: return "/ITU-T/Administration/[The Former Yugoslav Republic of Macedonia]";
                case 295: return "/ITU-T/Administration/[Principality of Liechtenstein]";
                case 297: return "/ITU-T/Administration/[Montenegro]";
                case 302:
                case 303: return "/ITU-T/Administration/[Canada]";
                case 308: return "/ITU-T/Administration/[Saint Pierre and Miquelon (Collectivité territoriale de la République française)]";
                case 310:
                case 311:
                case 312:
                case 313:
                case 314:
                case 315:
                case 316: return "/ITU-T/Administration/[United States of America]";
                case 330: return "/ITU-T/Administration/[Puerto Rico]";
                case 332: return "/ITU-T/Administration/[United States Virgin Islands]";
                case 334:
                case 335: return "/ITU-T/Administration/[Mexico]";
                case 338: return "/ITU-T/Administration/[Jamaica]";
                case 340: return "/ITU-T/Administration/[French Department of Guadeloupe and French Department of Martinique]";
                case 342: return "/ITU-T/Administration/[Barbados]";
                case 344: return "/ITU-T/Administration/[Antigua and Barbuda]";
                case 346: return "/ITU-T/Administration/[Cayman Islands]";
                case 348: return "/ITU-T/Administration/[British Virgin Islands]";
                case 350: return "/ITU-T/Administration/[Bermuda]";
                case 352: return "/ITU-T/Administration/[Grenada]";
                case 354: return "/ITU-T/Administration/[Montserrat]";
                case 356: return "/ITU-T/Administration/[Saint Kitts and Nevis]";
                case 358: return "/ITU-T/Administration/[Saint Lucia]";
                case 360: return "/ITU-T/Administration/[Saint Vincent and the Grenadines]";
                case 362: return "/ITU-T/Administration/[Netherlands Antilles]";
                case 363: return "/ITU-T/Administration/[Aruba]";
                case 364: return "/ITU-T/Administration/[Commonwealth of the Bahamas]";
                case 365: return "/ITU-T/Administration/[Anguilla]";
                case 366: return "/ITU-T/Administration/[Commonwealth of Dominica]";
                case 368: return "/ITU-T/Administration/[Cuba]";
                case 370: return "/ITU-T/Administration/[Dominican Republic]";
                case 372: return "/ITU-T/Administration/[Republic of Haiti]";
                case 374: return "/ITU-T/Administration/[Trinidad and Tobago]";
                case 376: return "/ITU-T/Administration/[Turks and Caicos Islands]";
                case 400: return "/ITU-T/Administration/[Azerbaijani Republic]";
                case 401: return "/ITU-T/Administration/[Republic of KAZAKHSTAN]";
                case 404: return "/ITU-T/Administration/[Republic of India]";
                case 410:
                case 411: return "/ITU-T/Administration/[Islamic Republic of Pakistan]";
                case 412: return "/ITU-T/Administration/[Afghanistan]";
                case 413: return "/ITU-T/Administration/[Democratic Scialist Republic of Sri Lanka]";
                case 414: return "/ITU-T/Administration/[Union of MYANMAR]";
                case 415: return "/ITU-T/Administration/[Lebanon]";
                case 416: return "/ITU-T/Administration/[Hashemite Kingdom of Jordan]";
                case 417: return "/ITU-T/Administration/[Syrian Arab Republic]";
                case 418: return "/ITU-T/Administration/[Republic of Iraq]";
                case 419: return "/ITU-T/Administration/[State of Kuwait]";
                case 420: return "/ITU-T/Administration/[Kingdom of Saudi Arabia]";
                case 421: return "/ITU-T/Administration/[Republic of Yemen]";
                case 422: return "/ITU-T/Administration/[Sultanate of Oman]";
                case 423: return "/ITU-T/Administration/[Reserved]";
                case 424: return "/ITU-T/Administration/[United Arab Emirates]";
                case 425: return "/ITU-T/Administration/[State of Israel]";
                case 426: return "/ITU-T/Administration/[Kingdom of Bahrain]";
                case 427: return "/ITU-T/Administration/[State of Qatar]";
                case 428: return "/ITU-T/Administration/[Mongolia]";
                case 429: return "/ITU-T/Administration/[Nepal]";
                case 430: return "/ITU-T/Administration/[United Arab Emirates (Abu Dhabi)]";
                case 431: return "/ITU-T/Administration/[United Arab Emirates (Dubai)]";
                case 432: return "/ITU-T/Administration/[Islamic Republic of Iran]";
                case 434: return "/ITU-T/Administration/[Republic of UZBEKISTAN]";
                case 436: return "/ITU-T/Administration/[Republic of Tajikistan]";
                case 437: return "/ITU-T/Administration/[Kyrgyz Republic]";
                case 438: return "/ITU-T/Administration/[Turkmenistan]";
                // TODO: case 440: goto oid_0_2_440;
                case 441:
                case 442:
                case 443: return "/ITU-T/Administration/[Japan]";
                // TODO: case 450: goto oid_0_2_450;
                case 452: return "/ITU-T/Administration/[Viet Nam]";
                case 453:
                case 454: return "/ITU-T/Administration/[Hong Kong, China]";
                case 455: return "/ITU-T/Administration/[Macau, China]";
                case 456: return "/ITU-T/Administration/[Kingdom of Cambodia]";
                case 457: return "/ITU-T/Administration/[Lao People's Democratic Republic]";
                case 460: return "/ITU-T/Administration/[People's Republic of China]";
                case 466: return "/ITU-T/Administration/[Taiwan, Province of China]";
                case 467: return "/ITU-T/Administration/[Democratic People's Republic of Korea]";
                case 470: return "/ITU-T/Administration/[The People's Republic of Bangladesh]";
                case 472: return "/ITU-T/Administration/[Republic of MALDIVES]";
                case 480:
                case 481: return "/ITU-T/Administration/[Republic of Korea]";
                case 502: return "/ITU-T/Administration/[Malaysia]";
                case 505: return "/ITU-T/Administration/[AUSTRALIA]";
                case 510: return "/ITU-T/Administration/[Republic of INDONESIA]";
                case 515: return "/ITU-T/Administration/[Republic of the Philippines]";
                case 520: return "/ITU-T/Administration/[Thailand]";
                case 525:
                case 526: return "/ITU-T/Administration/[Republic of Singapore]";
                case 528: return "/ITU-T/Administration/[Brunei Darussalam]";
                case 530: return "/ITU-T/Administration/[New Zealand]";
                case 534: return "/ITU-T/Administration/[Commonwealth of the Northern Mariana Islands]";
                case 535: return "/ITU-T/Administration/[Guam]";
                case 536: return "/ITU-T/Administration/[Republic of Nauru]";
                case 537: return "/ITU-T/Administration/[Papua New Guinea]";
                case 539: return "/ITU-T/Administration/[Kingdom of Tonga]";
                case 540: return "/ITU-T/Administration/[Solomon Islands]";
                case 541: return "/ITU-T/Administration/[Republic of Vanuatu]";
                case 542: return "/ITU-T/Administration/[Republic of Fiji]";
                case 543: return "/ITU-T/Administration/[Wallis and Futuna (French Overseas Territory)]";
                case 544: return "/ITU-T/Administration/[American Samoa]";
                case 545: return "/ITU-T/Administration/[Republic of Kiribati]";
                case 546: return "/ITU-T/Administration/[New Caledonia (French Overseas Territory)]";
                case 547: return "/ITU-T/Administration/[French Polynesia (French Overseas Territory)]";
                case 548: return "/ITU-T/Administration/[Cook Islands]";
                case 549: return "/ITU-T/Administration/[Independent State of Samoa]";
                case 550: return "/ITU-T/Administration/[Federated States of Micronesia]";
                case 602: return "/ITU-T/Administration/[Arab Republic of Egypt]";
                case 603: return "/ITU-T/Administration/[People's Democratic Republic of Algeria]";
                case 604: return "/ITU-T/Administration/[Kingdom of Morocco]";
                case 605: return "/ITU-T/Administration/[Tunisia]";
                case 606: return "/ITU-T/Administration/[Socialist People's Libyan Arab Jamahiriya]";
                case 607: return "/ITU-T/Administration/[The Republic of the Gambia]";
                case 608: return "/ITU-T/Administration/[Republic of Senegal]";
                case 609: return "/ITU-T/Administration/[Islamic Republic of Mauritania]";
                case 610: return "/ITU-T/Administration/[Republic of Mali]";
                case 611: return "/ITU-T/Administration/[Republic of Guinea]";
                case 612: return "/ITU-T/Administration/[Republic of Côte d'Ivoire]";
                case 613: return "/ITU-T/Administration/[Burkina Faso]";
                case 614: return "/ITU-T/Administration/[Republic of the Niger]";
                case 615: return "/ITU-T/Administration/[Togolese Republic]";
                case 616: return "/ITU-T/Administration/[Republic of Benin]";
                case 617: return "/ITU-T/Administration/[Republic of Mauritius]";
                case 618: return "/ITU-T/Administration/[Republic of Liberia]";
                case 619: return "/ITU-T/Administration/[Sierra Leone]";
                case 620: return "/ITU-T/Administration/[Ghana]";
                case 621: return "/ITU-T/Administration/[Federal Republic of Nigeria]";
                case 622: return "/ITU-T/Administration/[Republic of Chad]";
                case 623: return "/ITU-T/Administration/[Central African Republic]";
                case 624: return "/ITU-T/Administration/[Republic of Cameroon]";
                case 625: return "/ITU-T/Administration/[Republic of Cape Verde]";
                case 626: return "/ITU-T/Administration/[Democratic Republic of Sao Tome and Principe]";
                case 627: return "/ITU-T/Administration/[Equatorial Guinea]";
                case 628: return "/ITU-T/Administration/[Gabon]";
                case 629: return "/ITU-T/Administration/[Republic of the Congo]";
                case 630: return "/ITU-T/Administration/[Democratic Republic of the Congo]";
                case 631: return "/ITU-T/Administration/[Republic of Angola]";
                case 632: return "/ITU-T/Administration/[Republic of Guinea-Bissau]";
                case 633: return "/ITU-T/Administration/[Republic of Seychelles]";
                case 634: return "/ITU-T/Administration/[Republic of the Sudan]";
                case 635: return "/ITU-T/Administration/[Republic of Rwanda]";
                case 636: return "/ITU-T/Administration/[Federal Democratic Republic of Ethiopia]";
                case 637: return "/ITU-T/Administration/[Somali Democratic Republic]";
                case 638: return "/ITU-T/Administration/[Republic of Djibouti]";
                case 639: return "/ITU-T/Administration/[Republic of Kenya]";
                case 640: return "/ITU-T/Administration/[United Republic of Tanzania]";
                case 641: return "/ITU-T/Administration/[Republic of Uganda]";
                case 642: return "/ITU-T/Administration/[Republic of Burundi]";
                case 643: return "/ITU-T/Administration/[Republic of Mozambique]";
                case 645: return "/ITU-T/Administration/[Republic of Zambia]";
                case 646: return "/ITU-T/Administration/[Republic of Madagascar]";
                case 647: return "/ITU-T/Administration/[French Departments and Territories in the Indian Ocean]";
                case 648: return "/ITU-T/Administration/[Republic of Zimbabwe]";
                case 649: return "/ITU-T/Administration/[Republic of Namibia]";
                case 650: return "/ITU-T/Administration/[Malawi]";
                case 651: return "/ITU-T/Administration/[Kingdom of Lesotho]";
                case 652: return "/ITU-T/Administration/[Republic of Botswana]";
                case 653: return "/ITU-T/Administration/[Eswatini (formerly, Kingdom of Swaziland)]";
                case 654: return "/ITU-T/Administration/[Union of the Comoros]";
                case 655: return "/ITU-T/Administration/[Republic of South Africa]";
                case 658: return "/ITU-T/Administration/[Eritrea]";
                case 702: return "/ITU-T/Administration/[Belize]";
                case 704: return "/ITU-T/Administration/[Republic of Guatemala]";
                case 706: return "/ITU-T/Administration/[Republic of El Salvador]";
                case 708: return "/ITU-T/Administration/[Republic of Honduras]";
                case 710: return "/ITU-T/Administration/[Nicaragua]";
                case 712: return "/ITU-T/Administration/[Costa Rica]";
                case 714: return "/ITU-T/Administration/[Republic of Panama]";
                case 716: return "/ITU-T/Administration/[Peru]";
                case 722: return "/ITU-T/Administration/[ARGENTINE Republic]";
                case 724:
                case 725: return "/ITU-T/Administration/[Federative Republic of Brazil]";
                case 730: return "/ITU-T/Administration/[Chile]";
                case 732: return "/ITU-T/Administration/[Republic of Colombia]";
                case 734: return "/ITU-T/Administration/[Bolivarian Republic of Venezuela]";
                case 736: return "/ITU-T/Administration/[Republic of Bolivia]";
                case 738: return "/ITU-T/Administration/[Guyana]";
                case 740: return "/ITU-T/Administration/[Ecuador]";
                case 742: return "/ITU-T/Administration/[French Department of Guiana]";
                case 744: return "/ITU-T/Administration/[Republic of PARAGUAY]";
                case 746: return "/ITU-T/Administration/[Republic of Suriname]";
                case 748: return "/ITU-T/Administration/[Eastern Republic of Uruguay]";
                default: return $"/ITU-T/Administration/INVALID_{index}";
            }

        #endregion

        // network-operator
        #region 0.3.*

        oid_0_3:

            switch (values[index++])
            {
                case 1111: return "/ITU-T/Network-Operator/[INMARSAT: Atlantic Ocean-East]";
                case 1112: return "/ITU-T/Network-Operator/[INMARSAT: Pacific Ocean]";
                case 1113: return "/ITU-T/Network-Operator/[INMARSAT: Indian Ocean]";
                case 1114: return "/ITU-T/Network-Operator/[INMARSAT: Atlantic Ocean-West]";
                case 2023: return "/ITU-T/Network-Operator/[Greece: Packet Switched Public Data Network (HELLASPAC)]";
                case 2027: return "/ITU-T/Network-Operator/[Greece: LAN-NET]";
                case 2041: return "/ITU-T/Network-Operator/[Netherlands: Datanet 1 X.25 access]";
                case 2044: return "/ITU-T/Network-Operator/[Netherlands: Unisource / Unidata]";
                case 2046: return "/ITU-T/Network-Operator/[Netherlands: Unisource / \"VPNS\"]";
                case 2052: return "/ITU-T/Network-Operator/[Netherlands: Naamloze Vennootschap (NV) CasTel]";
                case 2053: return "/ITU-T/Network-Operator/[Netherlands: Global One Communications BV]";
                case 2055: return "/ITU-T/Network-Operator/[Netherlands: Rabofacet BV]";
                case 2057: return "/ITU-T/Network-Operator/[Netherlands: Trionet v.o.f.]";
                case 2062: return "/ITU-T/Network-Operator/[Belgium: Réseau de transmission de données à commutation par paquets, Data Communication Service (DCS)]";
                case 2064: return "/ITU-T/Network-Operator/[Belgium: CODENET]";
                case 2065: return "/ITU-T/Network-Operator/[Belgium: Code utilisé au niveau national pour le réseau Data Communication Service (DCS)]";
                case 2066: return "/ITU-T/Network-Operator/[Belgium: Unisource Belgium X.25 Service (code canceled)]";
                case 2067: return "/ITU-T/Network-Operator/[Belgium: MOBISTAR]";
                case 2068: return "/ITU-T/Network-Operator/[Belgium: Accès au réseau Data Communication Service (DCS) via le réseau telex commuté national]";
                case 2069: return "/ITU-T/Network-Operator/[Belgium: Acces au reseau DCS via le reseau telephonique commute national]";
                case 2080: return "/ITU-T/Network-Operator/[France: Réseau de transmission de données à commutation par paquets \"TRANSPAC\"]";
                case 2081: return "/ITU-T/Network-Operator/[France: Noeud de transit international]";
                case 2082: return "/ITU-T/Network-Operator/[France: Grands services publics]";
                case 2083: return "/ITU-T/Network-Operator/[France: Administrations]";
                case 2084: return "/ITU-T/Network-Operator/[France: Air France]";
                case 2085: return "/ITU-T/Network-Operator/[France: \"SIRIS\"]";
                case 2086: return "/ITU-T/Network-Operator/[France: BT France]";
                case 2089: return "/ITU-T/Network-Operator/[France: Interconnexion entre le réseau public de transmission de données Transpac et d'autres réseaux publics français, pour des services offerts en mode synchrone]";
                case 2135: return "/ITU-T/Network-Operator/[Andorra: ANDORPAC]";
                case 2140: return "/ITU-T/Network-Operator/[Spain: Administracion Publica]";
                case 2141: return "/ITU-T/Network-Operator/[Spain: Nodo internacional de datos]";
                case 2142: return "/ITU-T/Network-Operator/[Spain: RETEVISION]";
                case 2145: return "/ITU-T/Network-Operator/[Spain: Red IBERPAC]";
                case 2147: return "/ITU-T/Network-Operator/[Spain: France Telecom Redes y Servicios]";
                case 2149: return "/ITU-T/Network-Operator/[Spain: MegaRed]";
                case 2161: return "/ITU-T/Network-Operator/[Hungary: Packet Switched Data Service]";
                case 2180: return "/ITU-T/Network-Operator/[Bosnia and Herzegovina: \"BIHPAK\"]";
                case 2191: return "/ITU-T/Network-Operator/[Croatia: CROAPAK (Croatian Packet Switching Data Network)]";
                case 2201: return "/ITU-T/Network-Operator/[Serbia and Montenegro: YUgoslav PACket (YUPAC) switched public data network]";
                case 2221: return "/ITU-T/Network-Operator/[Italy: Rete Telex-Dati (Amministrazione P.T. / national)]";
                case 2222: return "/ITU-T/Network-Operator/[Italy: \"ITAPAC\" X.25]";
                case 2223: return "/ITU-T/Network-Operator/[Italy: Packet Network (PAN)]";
                case 2226: return "/ITU-T/Network-Operator/[Italy: \"ITAPAC\" - X.32 Public Switched Telephone Network (Public Switched Telephone Network (PSTN)), X.28, D channel]";
                case 2227: return "/ITU-T/Network-Operator/[Italy: \"ITAPAC International\"]";
                case 2233: return "/ITU-T/Network-Operator/[Italy: \"ALBADATA X.25\"]";
                case 2234: return "/ITU-T/Network-Operator/[Italy: Trasmissione dati a commutazione di pacchetto X.25 (UNISOURCE Italia S.p.A.)]";
                case 2235: return "/ITU-T/Network-Operator/[Italy: Trasmissione dati a commutazione di pacchetto X.25 (INFOSTRADA S.p.A.)]";
                case 2236: return "/ITU-T/Network-Operator/[Italy: Trasmissione dati a commutazione di pacchetto X.25 (WIND Telecomunicazioni S.p.A.)]";
                case 2237: return "/ITU-T/Network-Operator/[Italy: Trasmissione dati a commutazione di pacchetto X.25 (Atlanet S.p.A.)]";
                case 2250: return "/ITU-T/Network-Operator/[Vatican: Packet Switching Data Network (PSDN) of Vatican City State]";
                case 2260: return "/ITU-T/Network-Operator/[Romania: \"ROMPAC\"]";
                case 2280: return "/ITU-T/Network-Operator/[Switzerland: ISDNPac]";
                case 2282: return "/ITU-T/Network-Operator/[Switzerland: Transpac-CH]";
                case 2283: return "/ITU-T/Network-Operator/[Switzerland: Bebbicel]";
                case 2284: return "/ITU-T/Network-Operator/[Switzerland: Telepac]";
                case 2285: return "/ITU-T/Network-Operator/[Switzerland: Telepac (acces de reseaux prives)]";
                case 2286: return "/ITU-T/Network-Operator/[Switzerland: DataRail]";
                case 2287: return "/ITU-T/Network-Operator/[Switzerland: Spack]";
                case 2301: return "/ITU-T/Network-Operator/[Czech Republic: \"NEXTEL\"]";
                case 2302: return "/ITU-T/Network-Operator/[Czech Republic: Aliatel (code canceled)]";
                case 2311: return "/ITU-T/Network-Operator/[Slovakia: EuroTel]";
                case 2322: return "/ITU-T/Network-Operator/[Austria: Dataswitch (DATAKOM)]";
                case 2329: return "/ITU-T/Network-Operator/[Austria: Radausdata (DATAKOM)]";
                case 2340: return "/ITU-T/Network-Operator/[United Kingdom: British Telecommunications plc (BT)]";
                case 2341: return "/ITU-T/Network-Operator/[United Kingdom: International Packet Switching Service (IPSS)]";
                case 2342: return "/ITU-T/Network-Operator/[United Kingdom: Packet Switched Service (PSS)]";
                case 2343:
                case 2344: return "/ITU-T/Network-Operator/[United Kingdom: British Telecommunications plc (BT) Concert Packet Network]";
                case 2347:
                case 2348: return "/ITU-T/Network-Operator/[United Kingdom: British Telecommunications plc (BT)]";
                case 2349: return "/ITU-T/Network-Operator/[United Kingdom: Barclays Technology Services]";
                case 2350:
                case 2351: return "/ITU-T/Network-Operator/[United Kingdom: C&W X.25 Service, international packet gateway]";
                case 2352: return "/ITU-T/Network-Operator/[United Kingdom: Kingston Communications (Hull) Plc.]";
                case 2353: return "/ITU-T/Network-Operator/[United Kingdom: Vodaphone, Packet Network Service]";
                case 2354: return "/ITU-T/Network-Operator/[United Kingdom: Nomura Computer Systems Europe Ltd. (NCC-E)]";
                case 2355: return "/ITU-T/Network-Operator/[United Kingdom: \"JAIS Europe Ltd.\"]";
                case 2357: return "/ITU-T/Network-Operator/[United Kingdom: \"FEDEX UK\"]";
                case 2358: return "/ITU-T/Network-Operator/[United Kingdom: Reuters]";
                case 2359: return "/ITU-T/Network-Operator/[United Kingdom: British Telecommunications plc (BT)]";
                case 2360: return "/ITU-T/Network-Operator/[United Kingdom: AT&T \"ISTEL\"]";
                case 2370: return "/ITU-T/Network-Operator/[United Kingdom: GlobalOne (France Telecom)]";
                case 2378: return "/ITU-T/Network-Operator/[United Kingdom: Racal Telecom]";
                case 2380: return "/ITU-T/Network-Operator/[Denmark: Tele Danmark A/S]";
                case 2381: return "/ITU-T/Network-Operator/[Denmark: \"DATEX\" (Circuit switched network)]";
                case 2382:
                case 2383: return "/ITU-T/Network-Operator/[Denmark: \"DATAPAK\", packet switched network]";
                case 2384: return "/ITU-T/Network-Operator/[Denmark: Transpac]";
                case 2385: return "/ITU-T/Network-Operator/[Denmark: SONOFON Global System for Mobile communications (GSM)]";
                case 2401: return "/ITU-T/Network-Operator/[Sweden: Datex (Circuit Switched Public Data Network) - TeliaSonera AB (code canceled)]";
                case 2402: return "/ITU-T/Network-Operator/[Sweden: WM-data Infrastructur (code canceled)]";
                case 2403: return "/ITU-T/Network-Operator/[Sweden: Datapak (Packet Switched Public Data Network) - TeliaSonera AB]";
                case 2406: return "/ITU-T/Network-Operator/[Sweden: Flex25 (Public Packet Switched Data Network)]";
                case 2407: return "/ITU-T/Network-Operator/[Sweden: Private X.25 Networks (DNIC allocated for a group of private networks) - TeliaSonera AB]";
                case 2408: return "/ITU-T/Network-Operator/[Sweden: TRANSPAC Scandinavia AB (code canceled)]";
                case 2421: return "/ITU-T/Network-Operator/[Norway: \"DATEX\" Circuit Switched Data Network (CSDN)]";
                case 2422: return "/ITU-T/Network-Operator/[Norway: DATAPAK (Packet Switched Network, PSDN)]";
                case 2429: return "/ITU-T/Network-Operator/[Norway: Shared by private data networks for Private Network Identification Code (PNIC) allocation]";
                case 2442: return "/ITU-T/Network-Operator/[Finland: Datapak]";
                case 2443: return "/ITU-T/Network-Operator/[Finland: Finpak (Packet Switched Data Network, PSDN) of Helsinki Telephone Company Ltd.]";
                case 2444: return "/ITU-T/Network-Operator/[Finland: Telia Finland Ltd.]";
                case 2462: return "/ITU-T/Network-Operator/[Lithuania: Vilnius DATAPAK]";
                case 2463: return "/ITU-T/Network-Operator/[Lithuania: Omnitel]";
                case 2471: return "/ITU-T/Network-Operator/[Latvia: Latvia Public Packed Switched Data Network]";
                case 2472: return "/ITU-T/Network-Operator/[Latvia: Tele2]";
                case 2473: return "/ITU-T/Network-Operator/[Latvia: Telekom Baltija]";
                case 2474: return "/ITU-T/Network-Operator/[Latvia: \"MDBA\"]";
                case 2475: return "/ITU-T/Network-Operator/[Latvia: Rigatta]";
                case 2476: return "/ITU-T/Network-Operator/[Latvia: Rixtel]";
                case 2477: return "/ITU-T/Network-Operator/[Latvia: Advem]";
                case 2478: return "/ITU-T/Network-Operator/[Latvia: \"AWA\" Baltic]";
                case 2480: return "/ITU-T/Network-Operator/[Estonia: \"ESTPAK\"]";
                case 2500: return "/ITU-T/Network-Operator/[Russian Federation: Rospack-RT]";
                case 2501: return "/ITU-T/Network-Operator/[Russian Federation: \"SPRINT\" Networks]";
                case 2502: return "/ITU-T/Network-Operator/[Russian Federation: \"IASNET\"]";
                case 2503: return "/ITU-T/Network-Operator/[Russian Federation: \"MMTEL\"]";
                case 2504: return "/ITU-T/Network-Operator/[Russian Federation: INFOTEL]";
                case 2506: return "/ITU-T/Network-Operator/[Russian Federation: \"ROSNET\"]";
                case 2507: return "/ITU-T/Network-Operator/[Russian Federation: ISTOK-K]";
                case 2508: return "/ITU-T/Network-Operator/[Russian Federation: TRANSINFORM]";
                case 2509: return "/ITU-T/Network-Operator/[Russian Federation: LENFINCOM]";
                case 2510: return "/ITU-T/Network-Operator/[Russian Federation: SOVAMNET]";
                case 2511: return "/ITU-T/Network-Operator/[Russian Federation: EDITRANS]";
                case 2512: return "/ITU-T/Network-Operator/[Russian Federation: \"TECOS\"]";
                case 2513: return "/ITU-T/Network-Operator/[Russian Federation: \"PTTNET\"]";
                case 2514: return "/ITU-T/Network-Operator/[Russian Federation: \"BCLNET\"]";
                case 2515: return "/ITU-T/Network-Operator/[Russian Federation: \"SPTNET\"]";
                case 2516: return "/ITU-T/Network-Operator/[Russian Federation: \"AS\" Sirena-3 data communication system]";
                case 2517: return "/ITU-T/Network-Operator/[Russian Federation: TELSYCOM]";
                case 2550: return "/ITU-T/Network-Operator/[Ukraine: UkrPack]";
                case 2551: return "/ITU-T/Network-Operator/[Ukraine: bkcNET]";
                case 2555: return "/ITU-T/Network-Operator/[Ukraine: \"GTNET\"]";
                case 2556: return "/ITU-T/Network-Operator/[Ukraine: UkrPack]";
                case 2570: return "/ITU-T/Network-Operator/[Belarus: \"BELPAK\"]";
                case 2601: return "/ITU-T/Network-Operator/[Poland: \"POLPAK\"]";
                case 2602: return "/ITU-T/Network-Operator/[Poland: \"NASK\" (code canceled)]";
                case 2603: return "/ITU-T/Network-Operator/[Poland: TELBANK]";
                case 2604: return "/ITU-T/Network-Operator/[Poland: \"POLPAK -T\"]";
                case 2605: return "/ITU-T/Network-Operator/[Poland: \"PKONET\" (code canceled)]";
                case 2606: return "/ITU-T/Network-Operator/[Poland: Shared by a number of data networks (code canceled)]";
                case 2607: return "/ITU-T/Network-Operator/[Poland: \"CUPAK\"]";
                case 2621: return "/ITU-T/Network-Operator/[Germany: ISDN/X.25]";
                case 2622: return "/ITU-T/Network-Operator/[Germany: Circuit Switched Data Service (DATEX-L)]";
                case 2624: return "/ITU-T/Network-Operator/[Germany: Packet Switched Data Service (DATEX-P)]";
                case 2625: return "/ITU-T/Network-Operator/[Germany: Satellite Services]";
                case 2627: return "/ITU-T/Network-Operator/[Germany: Teletex]";
                case 2629: return "/ITU-T/Network-Operator/[Germany: D2-Mannesmann]";
                case 2631: return "/ITU-T/Network-Operator/[Germany: CoNetP]";
                case 2632: return "/ITU-T/Network-Operator/[Germany: \"RAPNET\"]";
                case 2633: return "/ITU-T/Network-Operator/[Germany: \"DPS\"]";
                case 2634: return "/ITU-T/Network-Operator/[Germany: EkoNet]";
                case 2636: return "/ITU-T/Network-Operator/[Germany: ARCOR/PSN-1]";
                case 2640: return "/ITU-T/Network-Operator/[Germany: DETECON]";
                case 2641: return "/ITU-T/Network-Operator/[Germany: \"SCN\"]";
                case 2642: return "/ITU-T/Network-Operator/[Germany: \"INFO AG NWS\"]";
                case 2644: return "/ITU-T/Network-Operator/[Germany: \"IDNS\"]";
                case 2645: return "/ITU-T/Network-Operator/[Germany: ARCOR/otelo-net1]";
                case 2646: return "/ITU-T/Network-Operator/[Germany: EuroDATA]";
                case 2647: return "/ITU-T/Network-Operator/[Germany: ARCOR/otelo-net2]";
                case 2648: return "/ITU-T/Network-Operator/[Germany: SNSPac]";
                case 2649: return "/ITU-T/Network-Operator/[Germany: \"MMONET\"]";
                case 2651: return "/ITU-T/Network-Operator/[Germany: WestLB X.25 Net]";
                case 2652: return "/ITU-T/Network-Operator/[Germany: PSN/FSINFOSYSBW]";
                case 2653: return "/ITU-T/Network-Operator/[Germany: ARCOR/PSN-2]";
                case 2654: return "/ITU-T/Network-Operator/[Germany: \"TNET\"]";
                case 2655: return "/ITU-T/Network-Operator/[Germany: ISIS_DUS]";
                case 2656: return "/ITU-T/Network-Operator/[Germany: \"RWE TELPAC\"]";
                case 2657: return "/ITU-T/Network-Operator/[Germany: DTN/AutoF FmNLw]";
                case 2658: return "/ITU-T/Network-Operator/[Germany: \"DRENET\"]";
                case 2659: return "/ITU-T/Network-Operator/[Germany: GCN (Geno Communication Network)]";
                case 2680: return "/ITU-T/Network-Operator/[Portugal: PrimeNet]";
                case 2681: return "/ITU-T/Network-Operator/[Portugal: OniSolutions -Infocomunicacies, S.A.]";
                case 2682: return "/ITU-T/Network-Operator/[Portugal: CPRM-Marconi]";
                case 2683: return "/ITU-T/Network-Operator/[Portugal: Eastecnica, Electronica e Tecnica, S.A.]";
                case 2684: return "/ITU-T/Network-Operator/[Portugal: PrimeNet]";
                case 2685: return "/ITU-T/Network-Operator/[Portugal: Global One - Comunicacies, S.A.]";
                case 2686: return "/ITU-T/Network-Operator/[Portugal: \"HLC\", Telecomunicacies & Multimedia, S.A.]";
                case 2687: return "/ITU-T/Network-Operator/[Portugal: Jazztel Portugal - Servicos de Telecomunicacies, S.A.]";
                case 2702: return "/ITU-T/Network-Operator/[Luxembourg: CODENET]";
                case 2703: return "/ITU-T/Network-Operator/[Luxembourg: Regional \"ATS\" Packet switched NETwork (RAPNET)]";
                case 2704: return "/ITU-T/Network-Operator/[Luxembourg: \"LUXPAC\" (réseau de transmission de données à commutation par paquets)]";
                case 2705: return "/ITU-T/Network-Operator/[Luxembourg: \"LUXNET\" (interconnection avec le réseau public de transmission de données)]";
                case 2709: return "/ITU-T/Network-Operator/[Luxembourg: \"LUXPAC\" (accès X.28 et X.32 au réseau téléphonique commuté)]";
                case 2721: return "/ITU-T/Network-Operator/[Ireland: International Packet Switched Service]";
                case 2723: return "/ITU-T/Network-Operator/[Ireland: EURONET]";
                case 2724: return "/ITU-T/Network-Operator/[Ireland: \"EIRPAC\" (Packet Switched Data Networks)]";
                case 2728: return "/ITU-T/Network-Operator/[Ireland: PostNET (PostGEM Packet Switched Data Network)]";
                case 2740: return "/ITU-T/Network-Operator/[Iceland: ISPAK/ICEPAC]";
                case 2782: return "/ITU-T/Network-Operator/[Malta: MALTAPAC (Packet Switching Service)]";
                case 2802: return "/ITU-T/Network-Operator/[Cyprus: CYTAPAC - Public Switched Data Network (PSDN), subscribers with direct access]";
                case 2808: return "/ITU-T/Network-Operator/[Cyprus: CYTAPAC - Public Switched Data Network (PSDN), subscribers with access via telex]";
                case 2809: return "/ITU-T/Network-Operator/[Cyprus: CYTAPAC - Public Switched Data Network (PSDN), subscribers with access via Public Switched Telephone Network (Public Switched Telephone Network (PSTN)) - X.28, X.32]";
                case 2821: return "/ITU-T/Network-Operator/[Georgia: IBERIAPAC]";
                case 2830: return "/ITU-T/Network-Operator/[Armenia: ArmPac]";
                case 2860: return "/ITU-T/Network-Operator/[Turkey: TELETEX]";
                case 2861: return "/ITU-T/Network-Operator/[Turkey: DATEX-L]";
                case 2863: return "/ITU-T/Network-Operator/[Turkey: TURkish PAcKet switched data network (TURPAK)]";
                case 2864: return "/ITU-T/Network-Operator/[Turkey: \"TURPAK\"]";
                case 2881: return "/ITU-T/Network-Operator/[Faroe Islands: FAROEPAC]";
                case 2901: return "/ITU-T/Network-Operator/[Greenland: DATAPAK (Packet Switched Network)]";
                case 2922: return "/ITU-T/Network-Operator/[San Marino: X-Net \"SMR\"]";
                case 2931: return "/ITU-T/Network-Operator/[Slovenia: SIPAX.25]";
                case 2932: return "/ITU-T/Network-Operator/[Slovenia: SIPAX.25 access through Integrated Services Digital Network (ISDN) (code canceled)]";
                case 2940: return "/ITU-T/Network-Operator/[The Former Yugoslav Republic of Macedonia: \"MAKPAK\"]";
                case 3020: return "/ITU-T/Network-Operator/[Canada: Telecom Canada Datapak Network]";
                case 3021: return "/ITU-T/Network-Operator/[Canada: Telecom Canada Public Switched Telephone Network (Public Switched Telephone Network (PSTN)) Access]";
                case 3022: return "/ITU-T/Network-Operator/[Canada: Stentor Private Packet Switched Data Network Gateway]";
                case 3023: return "/ITU-T/Network-Operator/[Canada: Stentor Integrated Services Digital Network (ISDN) identification]";
                case 3024: return "/ITU-T/Network-Operator/[Canada: Teleglobe Canada - Globedat-C Circuit Switched Data Transmission]";
                case 3025: return "/ITU-T/Network-Operator/[Canada: Teleglobe Canada - Globedat-P Packed Switched Data Transmission]";
                case 3026: return "/ITU-T/Network-Operator/[Canada: AT&T Canada Long Distance Services - FasPac]";
                case 3028: return "/ITU-T/Network-Operator/[Canada: AT&T Canada Long Distance Services - Packet Switched Public Data Network (PSPDN)]";
                case 3036: return "/ITU-T/Network-Operator/[Canada: Sprint Canada Frame Relay Service - Packet-Switched Network]";
                case 3037: return "/ITU-T/Network-Operator/[Canada: \"TMI Communications\", Limited Partnership - Mobile Data Service (MDS)]";
                case 3038: return "/ITU-T/Network-Operator/[Canada: Canada Post - POSTpac - X.25 Packet Switched Data Network]";
                case 3039: return "/ITU-T/Network-Operator/[Canada: Telesat Canada - Anikom 200]";
                case 3101: return "/ITU-T/Network-Operator/[United States: PTN-1 Western Union Packet Switching Network]";
                case 3102: return "/ITU-T/Network-Operator/[United States: \"MCI\" Public Data Network (ResponseNet)]";
                case 3103: return "/ITU-T/Network-Operator/[United States: \"ITT UDTS\" Network]";
                case 3104: return "/ITU-T/Network-Operator/[United States: MCI Public Data Network (International Gateway)]";
                case 3105: return "/ITU-T/Network-Operator/[United States: \"WUI\" Leased Channel Network]";
                case 3106: return "/ITU-T/Network-Operator/[United States: Tymnet Network]";
                case 3107: return "/ITU-T/Network-Operator/[United States: \"ITT\" Datel Network]";
                case 3108: return "/ITU-T/Network-Operator/[United States: ITT Short Term Voice/Data Transmission Network]";
                case 3109: return "/ITU-T/Network-Operator/[United States: \"RCAG DATEL II\"]";
                case 3110: return "/ITU-T/Network-Operator/[United States: Telenet Communications Corporation]";
                case 3111: return "/ITU-T/Network-Operator/[United States: \"RCAG DATEL I\" (Switched Alternate Voice-Data Service)]";
                case 3112: return "/ITU-T/Network-Operator/[United States: Western Union Teletex Service]";
                case 3113: return "/ITU-T/Network-Operator/[United States: \"RCAG\" Remote Global Computer Access Service (Low Speed)]";
                case 3114: return "/ITU-T/Network-Operator/[United States: Western Union Infomaster]";
                case 3115: return "/ITU-T/Network-Operator/[United States: Graphnet Interactive Network]";
                case 3116: return "/ITU-T/Network-Operator/[United States: Graphnet Store and Forward Network]";
                case 3117: return "/ITU-T/Network-Operator/[United States: \"WUI\" Telex Network]";
                case 3118: return "/ITU-T/Network-Operator/[United States: Graphnet Data Network]";
                case 3119: return "/ITU-T/Network-Operator/[United States: \"TRT\" International Packet Switched Service (IPSS)]";
                case 3120: return "/ITU-T/Network-Operator/[United States: \"ITT\" Low Speed Network]";
                case 3121: return "/ITU-T/Network-Operator/[United States: \"FTCC\" Circuit Switched Network]";
                case 3122: return "/ITU-T/Network-Operator/[United States: FTCC Telex]";
                case 3123: return "/ITU-T/Network-Operator/[United States: FTCC Domestic Packet Switched Transmission (PST) Service]";
                case 3124: return "/ITU-T/Network-Operator/[United States: FTCC International PST Service]";
                case 3125: return "/ITU-T/Network-Operator/[United States: \"UNINET\"]";
                case 3126: return "/ITU-T/Network-Operator/[United States: \"ADP\" Autonet]";
                case 3127: return "/ITU-T/Network-Operator/[United States: \"GTE\" Telenet Communications Corporation]";
                case 3128: return "/ITU-T/Network-Operator/[United States: \"TRT\" Mail/Telex Network]";
                case 3129: return "/ITU-T/Network-Operator/[United States: \"TRT\" Circuit Switch Data (\"ICSS\")]";
                case 3130: return "/ITU-T/Network-Operator/[United States: TRT Digital Data Network]";
                case 3131: return "/ITU-T/Network-Operator/[United States: \"RCAG\" Telex Network]";
                case 3132: return "/ITU-T/Network-Operator/[United States: Compuserve Network Services]";
                case 3133: return "/ITU-T/Network-Operator/[United States: \"RCAG XNET\" Service]";
                case 3134: return "/ITU-T/Network-Operator/[United States: AT&T/ACCUNET Packet Switched Capability]";
                case 3135: return "/ITU-T/Network-Operator/[United States: ALASCOM/ALASKANET Service]";
                case 3136: return "/ITU-T/Network-Operator/[United States: Geisco Data Network]";
                case 3137: return "/ITU-T/Network-Operator/[United States: International Information Network Services - INFONET Service]";
                case 3138: return "/ITU-T/Network-Operator/[United States: Fedex International Transmission Corporation - International Document Transmission Service]";
                case 3139: return "/ITU-T/Network-Operator/[United States: \"KDD America\", Inc. - Public Data Network]";
                case 3140: return "/ITU-T/Network-Operator/[United States: Southern New England Telephone Company - Public Packet Network]";
                case 3141: return "/ITU-T/Network-Operator/[United States: Bell Atlantic Telephone Companies - Advance Service]";
                case 3142: return "/ITU-T/Network-Operator/[United States: Bellsouth Corporation - Pulselink Service]";
                case 3143: return "/ITU-T/Network-Operator/[United States: Ameritech Operating Companies - Public Packet Data Networks]";
                case 3144: return "/ITU-T/Network-Operator/[United States: Nynex Telephone Companies - Nynex Infopath Service]";
                case 3145: return "/ITU-T/Network-Operator/[United States: Pacific Telesis Public Packet Switching Service]";
                case 3146: return "/ITU-T/Network-Operator/[United States: Southwestern Bell Telephone Co. - Microlink \"II\" Public Packet Switching Service]";
                case 3147: return "/ITU-T/Network-Operator/[United States: U.S. West, Inc. - Public Packet Switching Service]";
                case 3148: return "/ITU-T/Network-Operator/[United States: United States Telephone Association - to be shared by local exchange telephone companies]";
                case 3149: return "/ITU-T/Network-Operator/[United States: Cable & Wireless Communications, Inc. - Public Data Network]";
                case 3150: return "/ITU-T/Network-Operator/[United States: Globenet, Inc. - Globenet Network Packet Switching Service]";
                case 3151: return "/ITU-T/Network-Operator/[United States: Data America Corporation - Data America Network]";
                case 3152: return "/ITU-T/Network-Operator/[United States: \"GTE\" Hawaiian Telephone Company, Inc. - Public Data Network]";
                case 3153: return "/ITU-T/Network-Operator/[United States: \"JAIS USA-NET\" Public Packet Switching Service]";
                case 3154: return "/ITU-T/Network-Operator/[United States: Nomura Computer Systems America, Inc. - \"NCC-A VAN\" public packet switching service]";
                case 3155: return "/ITU-T/Network-Operator/[United States: Aeronautical Radio, Inc. - GLOBALINK]";
                case 3156: return "/ITU-T/Network-Operator/[United States: American Airlines, Inc. - \"AANET\"]";
                case 3157: return "/ITU-T/Network-Operator/[United States: \"COMSAT\" Mobile Communications - \"C-LINK\"]";
                case 3158: return "/ITU-T/Network-Operator/[United States: Schlumberger Information NETwork (SINET)]";
                case 3159: return "/ITU-T/Network-Operator/[United States: Westinghouse Communications - Westinghouse Packet Network]";
                case 3160: return "/ITU-T/Network-Operator/[United States: Network Users Group, Ltd. - \"WDI NET\" packet]";
                case 3161: return "/ITU-T/Network-Operator/[United States: United States Department of State, Diplomatic Telecommunications Service]";
                case 3162: return "/ITU-T/Network-Operator/[United States: Transaction Network Services (TNS), Inc. -- Public packet-switched network]";
                case 3166: return "/ITU-T/Network-Operator/[United States: U.S. Department of Treasury Wide Area Data Network]";
                case 3168: return "/ITU-T/Network-Operator/[United States: \"BT\" North America packet-switched data network]";
                case 3169: return "/ITU-T/Network-Operator/[United States: Tenzing Communications Inc. - Inflight Network]";
                case 3302: return "/ITU-T/Network-Operator/[Puerto Rico: Asynchronous Transfer Mode (ATM) Broadband Network]";
                case 3303: return "/ITU-T/Network-Operator/[Puerto Rico: TDNet Puerto Rico]";
                case 3340: return "/ITU-T/Network-Operator/[Mexico: \"TELEPAC\"]";
                case 3341: return "/ITU-T/Network-Operator/[Mexico: \"UNITET\"]";
                case 3342: return "/ITU-T/Network-Operator/[Mexico: IUSANET]";
                case 3343: return "/ITU-T/Network-Operator/[Mexico: \"TEI\"]";
                case 3344: return "/ITU-T/Network-Operator/[Mexico: \"OPTEL\"]";
                case 3345: return "/ITU-T/Network-Operator/[Mexico: TELNORPAC]";
                case 3346: return "/ITU-T/Network-Operator/[Mexico: \"TYMPAQ\"]";
                case 3347: return "/ITU-T/Network-Operator/[Mexico: SINFRARED]";
                case 3348: return "/ITU-T/Network-Operator/[Mexico: INTERVAN]";
                case 3349: return "/ITU-T/Network-Operator/[Mexico: INTELCOMNET]";
                case 3350: return "/ITU-T/Network-Operator/[Mexico: AVANTEL, S.A.]";
                case 3351: return "/ITU-T/Network-Operator/[Mexico: ALESTRA, S. de R.L. de C.V.]";
                case 3422: return "/ITU-T/Network-Operator/[Barbados: CARIBNET]";
                case 3423: return "/ITU-T/Network-Operator/[Barbados: International Database Access Service (IDAS)]";
                case 3443: return "/ITU-T/Network-Operator/[Antigua and Barbuda: Antigua Packet Switched Service]";
                case 3463: return "/ITU-T/Network-Operator/[Cayman Islands: Cable and Wireless Packet Switching Node]";
                case 3502: return "/ITU-T/Network-Operator/[Bermuda: Cable and Wireless Data Communications Node]";
                case 3503: return "/ITU-T/Network-Operator/[Bermuda: Cable and Wireless Packet Switched Node]";
                case 3522: return "/ITU-T/Network-Operator/[Grenada: CARIBNET]";
                case 3620: return "/ITU-T/Network-Operator/[Netherlands Antilles: Telematic Network]";
                case 3621: return "/ITU-T/Network-Operator/[Netherlands Antilles: Datanet Curacao]";
                case 3680: return "/ITU-T/Network-Operator/[Cuba: Servicios de informacion por conmutacion de paquetes del \"IDICT\"]";
                case 3706: return "/ITU-T/Network-Operator/[Dominican Republic: All America Cables and Radio Inc.]";
                case 3740: return "/ITU-T/Network-Operator/[Trinidad and Tobago: \"TEXDAT\"]";
                case 3745: return "/ITU-T/Network-Operator/[Trinidad and Tobago: DATANETT]";
                case 3763:
                case 3764: return "/ITU-T/Network-Operator/[Turks and Caicos Islands: Cable and wireless packet switched node]";

                // TODO: Left off at http://www.oid-info.com/get/0.3.4001
                default: return $"/ITU-T/Network-Operator/INVALID_{index}";
            }

        #endregion

        #endregion

        // iso
        #region 1.*

        oid_1:

            switch (values[index++])
            {
                // TODO: case 0: goto oid_1_0;
                // TODO: case 1: goto oid_1_1;
                // TODO: case 2: goto oid_1_2;
                // TODO: case 3: goto oid_1_3;
                default: return $"/ISO/INVALID_{index}";
            }


        #endregion

        // joint-iso-itu-t, joint-iso-ccitt
        #region 2.*

        oid_2:

            switch (values[index++])
            {
                case 0: return "/Joint-ISO-ITU-T/[presentation]";
                // TODO: case 1: goto oid_2_1;
                // TODO: case 2: goto oid_2_2;
                // TODO: case 3: goto oid_2_3;
                // TODO: case 4: goto oid_2_4;
                // TODO: case 5: goto oid_2_5;
                // TODO: case 6: goto oid_2_6;
                // TODO: case 7: goto oid_2_7;
                // TODO: case 8: goto oid_2_8;
                // TODO: case 9: goto oid_2_9;
                // TODO: case 10: goto oid_2_10;
                // TODO: case 11: goto oid_2_11;
                // TODO: case 12: goto oid_2_12;
                // TODO: case 13: goto oid_2_13;
                // TODO: case 14: goto oid_2_14;
                // TODO: case 15: goto oid_2_15;
                // TODO: case 16: goto oid_2_16;
                // TODO: case 17: goto oid_2_17;
                // TODO: case 18: goto oid_2_18;
                // TODO: case 19: goto oid_2_19;
                // TODO: case 20: goto oid_2_20;
                // TODO: case 21: goto oid_2_21;
                // TODO: case 22: goto oid_2_22;
                // TODO: case 23: goto oid_2_23;
                // TODO: case 24: goto oid_2_24;
                // TODO: case 25: goto oid_2_25;
                // TODO: case 26: goto oid_2_26;
                // TODO: case 27: goto oid_2_27;
                // TODO: case 28: goto oid_2_28;
                // TODO: case 40: goto oid_2_40;
                // TODO: case 41: goto oid_2_41;
                // TODO: case 42: goto oid_2_42;
                // TODO: case 48: goto oid_2_48;
                // TODO: case 49: goto oid_2_49;
                // TODO: case 50: goto oid_2_50;
                // TODO: case 51: goto oid_2_51;
                // TODO: case 52: goto oid_2_52;
                case 999: return "/Joint-ISO-ITU-T/Example";
                default: return $"/Joint-ISO-ITU-T/INVALID_{index}";
            }

            #endregion
        }

        #endregion
    }
}
