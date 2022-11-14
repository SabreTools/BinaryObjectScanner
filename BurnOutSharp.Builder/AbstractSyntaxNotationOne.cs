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

        #region Start

        oid_start:

            switch (values[index++])
            {
                case 0: goto oid_0;
                case 1: goto oid_1;
                case 2: goto oid_2;
                default: return null;
            }

        #endregion

        // itu-t, ccitt, itu-r
        #region 0.*

        oid_0:

            switch (values[index++])
            {
                case 0: goto oid_0_0;
                case 1:
                    nameBuilder.Append("/ITU-T/1/[question]");
                    goto oid_end;
                case 2: goto oid_0_2;
                case 3: goto oid_0_3;
                // TODO: case 4: goto oid_0_4;
                case 5:
                    nameBuilder.Append("/ITU-R/R-Recommendation");
                    goto oid_end;
                // TODO: case 9: goto oid_0_9;
                default: return null;
            };

        // recommendation
        #region 0.0.*

        oid_0_0:

            switch (values[index++])
            {
                case 1:
                    nameBuilder.Append("/ITU-T/Recommendation/A");
                    goto oid_end;
                case 2:
                    nameBuilder.Append("/ITU-T/Recommendation/B");
                    goto oid_end;
                case 3:
                    nameBuilder.Append("/ITU-T/Recommendation/C");
                    goto oid_end;
                case 4:
                    nameBuilder.Append("/ITU-T/Recommendation/D");
                    goto oid_end;
                // TODO: case 5: goto oid_0_0_5;
                case 6:
                    nameBuilder.Append("/ITU-T/Recommendation/F");
                    goto oid_end;
                // TODO: case 7: goto oid_0_0_7;
                // TODO: case 8: goto oid_0_0_8;
                // TODO: case 9: goto oid_0_0_9;
                case 10:
                    nameBuilder.Append("/ITU-T/Recommendation/J");
                    goto oid_end;
                case 11:
                    nameBuilder.Append("/ITU-T/Recommendation/K");
                    goto oid_end;
                case 12:
                    nameBuilder.Append("/ITU-T/Recommendation/L");
                    goto oid_end;
                // TODO: case 13: goto oid_0_0_13;
                case 14:
                    nameBuilder.Append("/ITU-T/Recommendation/N");
                    goto oid_end;
                case 15:
                    nameBuilder.Append("/ITU-T/Recommendation/O");
                    goto oid_end;
                case 16:
                    nameBuilder.Append("/ITU-T/Recommendation/P");
                    goto oid_end;
                // TODO: case 17: goto oid_0_0_17;
                case 18:
                    nameBuilder.Append("/ITU-T/Recommendation/R");
                    goto oid_end;
                case 19:
                    nameBuilder.Append("/ITU-T/Recommendation/S");
                    goto oid_end;
                // TODO: case 20: goto oid_0_0_20;
                case 21:
                    nameBuilder.Append("/ITU-T/Recommendation/U");
                    goto oid_end;
                // TODO: case 22: goto oid_0_0_22;
                // TODO: case 24: goto oid_0_0_24;
                case 25:
                    nameBuilder.Append("/ITU-T/Recommendation/Y");
                    goto oid_end;
                case 26:
                    nameBuilder.Append("/ITU-T/Recommendation/Z");
                    goto oid_end;
                case 59:
                    nameBuilder.Append("/ITU-T/Recommendation/[xcmJobZeroDummy]");
                    goto oid_end;
                case 74:
                    nameBuilder.Append("/ITU-T/Recommendation/[xcmSvcMonZeroDummy]");
                    goto oid_end;
                default: return null;
            }

        #endregion

        // administration
        #region 0.2.*

        oid_0_2:

            switch (values[index++])
            {
                case 202:
                    nameBuilder.Append("/ITU-T/Administration/[Greece]");
                    goto oid_end;
                case 204:
                case 205:
                    nameBuilder.Append("/ITU-T/Administration/[Kingdom of the Netherlands]");
                    break;
                case 206:
                    nameBuilder.Append("/ITU-T/Administration/[Belgium]");
                    break;
                case 208:
                case 209:
                case 210:
                case 211:
                    nameBuilder.Append("/ITU-T/Administration/[France]");
                    break;
                case 212:
                    nameBuilder.Append("/ITU-T/Administration/[Principality of Monaco]");
                    break;
                case 213:
                    nameBuilder.Append("/ITU-T/Administration/[Principality of ANDORRA]");
                    break;
                case 214:
                case 215:
                    nameBuilder.Append("/ITU-T/Administration/[Spain]");
                    break;
                case 216:
                    nameBuilder.Append("/ITU-T/Administration/[Hungary]");
                    break;
                case 218:
                    nameBuilder.Append("/ITU-T/Administration/[Bosnia and Herzegovina]");
                    break;
                case 219:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of CROATIA]");
                    break;
                case 220:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Serbia]");
                    break;
                case 222:
                case 223:
                case 224:
                    nameBuilder.Append("/ITU-T/Administration/[Italy]");
                    break;
                case 225:
                    nameBuilder.Append("/ITU-T/Administration/[Vatican City State]");
                    break;
                case 226:
                    nameBuilder.Append("/ITU-T/Administration/[Romania]");
                    break;
                // TODO: case 228: goto oid_0_2_228;
                case 229:
                    nameBuilder.Append("/ITU-T/Administration/[Confederation of Switzerland]");
                    break;
                case 230:
                    nameBuilder.Append("/ITU-T/Administration/[Czech Republic]");
                    break;
                case 231:
                    nameBuilder.Append("/ITU-T/Administration/[Slovakia]");
                    break;
                case 232:
                case 233:
                    nameBuilder.Append("/ITU-T/Administration/[Austria]");
                    break;
                case 234:
                case 235:
                case 236:
                case 237:
                    nameBuilder.Append("/ITU-T/Administration/[United Kingdom of Great Britain and Northern Ireland]");
                    break;
                case 238:
                case 239:
                    nameBuilder.Append("/ITU-T/Administration/[Denmark]");
                    break;
                case 240:
                    nameBuilder.Append("/ITU-T/Administration/[Sweden]");
                    break;
                // TODO: case 242: goto oid_0_2_242;
                case 243:
                    nameBuilder.Append("/ITU-T/Administration/[Norway]");
                    break;
                case 244:
                    nameBuilder.Append("/ITU-T/Administration/[Finland]");
                    break;
                case 246:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of LITHUANIA]");
                    break;
                case 247:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Latvia]");
                    break;
                case 248:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of ESTONIA]");
                    break;
                case 250:
                case 251:
                    nameBuilder.Append("/ITU-T/Administration/[Russian Federation]");
                    break;
                case 255:
                    nameBuilder.Append("/ITU-T/Administration/[Ukraine]");
                    break;
                case 257:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Belarus]");
                    break;
                case 259:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Moldova]");
                    break;
                case 260:
                case 261:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Poland]");
                    break;
                // TODO: case 262: goto oid_0_2_262;
                case 263:
                case 264:
                case 265:
                    nameBuilder.Append("/ITU-T/Administration/[Federal Republic of Germany]");
                    break;
                case 266:
                    nameBuilder.Append("/ITU-T/Administration/[Gibraltar]");
                    break;
                case 268:
                case 269:
                    nameBuilder.Append("/ITU-T/Administration/[Portugal]");
                    break;
                case 270:
                    nameBuilder.Append("/ITU-T/Administration/[Luxembourg]");
                    break;
                case 272:
                    nameBuilder.Append("/ITU-T/Administration/[Ireland]");
                    break;
                case 274:
                    nameBuilder.Append("/ITU-T/Administration/[Iceland]");
                    break;
                case 276:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Albania]");
                    break;
                case 278:
                    nameBuilder.Append("/ITU-T/Administration/[Malta]");
                    break;
                case 280:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Cyprus]");
                    break;
                case 282:
                    nameBuilder.Append("/ITU-T/Administration/[Georgia]");
                    break;
                case 283:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of ARMENIA]");
                    break;
                case 284:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Bulgaria]");
                    break;
                case 286:
                    nameBuilder.Append("/ITU-T/Administration/[Turkey]");
                    break;
                case 288:
                    nameBuilder.Append("/ITU-T/Administration/[Faroe Islands]");
                    break;
                case 290:
                    nameBuilder.Append("/ITU-T/Administration/[Greenland]");
                    break;
                case 292:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of San Marino]");
                    break;
                case 293:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of SLOVENIA]");
                    break;
                case 294:
                    nameBuilder.Append("/ITU-T/Administration/[The Former Yugoslav Republic of Macedonia]");
                    break;
                case 295:
                    nameBuilder.Append("/ITU-T/Administration/[Principality of Liechtenstein]");
                    break;
                case 297:
                    nameBuilder.Append("/ITU-T/Administration/[Montenegro]");
                    break;
                case 302:
                case 303:
                    nameBuilder.Append("/ITU-T/Administration/[Canada]");
                    break;
                case 308:
                    nameBuilder.Append("/ITU-T/Administration/[Saint Pierre and Miquelon (Collectivité territoriale de la République française)]");
                    break;
                case 310:
                case 311:
                case 312:
                case 313:
                case 314:
                case 315:
                case 316:
                    nameBuilder.Append("/ITU-T/Administration/[United States of America]");
                    break;
                case 330:
                    nameBuilder.Append("/ITU-T/Administration/[Puerto Rico]");
                    break;
                case 332:
                    nameBuilder.Append("/ITU-T/Administration/[United States Virgin Islands]");
                    break;
                case 334:
                case 335:
                    nameBuilder.Append("/ITU-T/Administration/[Mexico]");
                    break;
                case 338:
                    nameBuilder.Append("/ITU-T/Administration/[Jamaica]");
                    break;
                case 340:
                    nameBuilder.Append("/ITU-T/Administration/[French Department of Guadeloupe and French Department of Martinique]");
                    break;
                case 342:
                    nameBuilder.Append("/ITU-T/Administration/[Barbados]");
                    break;
                case 344:
                    nameBuilder.Append("/ITU-T/Administration/[Antigua and Barbuda]");
                    break;
                case 346:
                    nameBuilder.Append("/ITU-T/Administration/[Cayman Islands]");
                    break;
                case 348:
                    nameBuilder.Append("/ITU-T/Administration/[British Virgin Islands]");
                    break;
                case 350:
                    nameBuilder.Append("/ITU-T/Administration/[Bermuda]");
                    break;
                case 352:
                    nameBuilder.Append("/ITU-T/Administration/[Grenada]");
                    break;
                case 354:
                    nameBuilder.Append("/ITU-T/Administration/[Montserrat]");
                    break;
                case 356:
                    nameBuilder.Append("/ITU-T/Administration/[Saint Kitts and Nevis]");
                    break;
                case 358:
                    nameBuilder.Append("/ITU-T/Administration/[Saint Lucia]");
                    break;
                case 360:
                    nameBuilder.Append("/ITU-T/Administration/[Saint Vincent and the Grenadines]");
                    break;
                case 362:
                    nameBuilder.Append("/ITU-T/Administration/[Netherlands Antilles]");
                    break;
                case 363:
                    nameBuilder.Append("/ITU-T/Administration/[Aruba]");
                    break;
                case 364:
                    nameBuilder.Append("/ITU-T/Administration/[Commonwealth of the Bahamas]");
                    break;
                case 365:
                    nameBuilder.Append("/ITU-T/Administration/[Anguilla]");
                    break;
                case 366:
                    nameBuilder.Append("/ITU-T/Administration/[Commonwealth of Dominica]");
                    break;
                case 368:
                    nameBuilder.Append("/ITU-T/Administration/[Cuba]");
                    break;
                case 370:
                    nameBuilder.Append("/ITU-T/Administration/[Dominican Republic]");
                    break;
                case 372:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Haiti]");
                    break;
                case 374:
                    nameBuilder.Append("/ITU-T/Administration/[Trinidad and Tobago]");
                    break;
                case 376:
                    nameBuilder.Append("/ITU-T/Administration/[Turks and Caicos Islands]");
                    break;
                case 400:
                    nameBuilder.Append("/ITU-T/Administration/[Azerbaijani Republic]");
                    break;
                case 401:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of KAZAKHSTAN]");
                    break;
                case 404:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of India]");
                    break;
                case 410:
                case 411:
                    nameBuilder.Append("/ITU-T/Administration/[Islamic Republic of Pakistan]");
                    break;
                case 412:
                    nameBuilder.Append("/ITU-T/Administration/[Afghanistan]");
                    break;
                case 413:
                    nameBuilder.Append("/ITU-T/Administration/[Democratic Scialist Republic of Sri Lanka]");
                    break;
                case 414:
                    nameBuilder.Append("/ITU-T/Administration/[Union of MYANMAR]");
                    break;
                case 415:
                    nameBuilder.Append("/ITU-T/Administration/[Lebanon]");
                    break;
                case 416:
                    nameBuilder.Append("/ITU-T/Administration/[Hashemite Kingdom of Jordan]");
                    break;
                case 417:
                    nameBuilder.Append("/ITU-T/Administration/[Syrian Arab Republic]");
                    break;
                case 418:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Iraq]");
                    break;
                case 419:
                    nameBuilder.Append("/ITU-T/Administration/[State of Kuwait]");
                    break;
                case 420:
                    nameBuilder.Append("/ITU-T/Administration/[Kingdom of Saudi Arabia]");
                    break;
                case 421:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Yemen]");
                    break;
                case 422:
                    nameBuilder.Append("/ITU-T/Administration/[Sultanate of Oman]");
                    break;
                case 423:
                    nameBuilder.Append("/ITU-T/Administration/[Reserved]");
                    break;
                case 424:
                    nameBuilder.Append("/ITU-T/Administration/[United Arab Emirates]");
                    break;
                case 425:
                    nameBuilder.Append("/ITU-T/Administration/[State of Israel]");
                    break;
                case 426:
                    nameBuilder.Append("/ITU-T/Administration/[Kingdom of Bahrain]");
                    break;
                case 427:
                    nameBuilder.Append("/ITU-T/Administration/[State of Qatar]");
                    break;
                case 428:
                    nameBuilder.Append("/ITU-T/Administration/[Mongolia]");
                    break;
                case 429:
                    nameBuilder.Append("/ITU-T/Administration/[Nepal]");
                    break;
                case 430:
                    nameBuilder.Append("/ITU-T/Administration/[United Arab Emirates (Abu Dhabi)]");
                    break;
                case 431:
                    nameBuilder.Append("/ITU-T/Administration/[United Arab Emirates (Dubai)]");
                    break;
                case 432:
                    nameBuilder.Append("/ITU-T/Administration/[Islamic Republic of Iran]");
                    break;
                case 434:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of UZBEKISTAN]");
                    break;
                case 436:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Tajikistan]");
                    break;
                case 437:
                    nameBuilder.Append("/ITU-T/Administration/[Kyrgyz Republic]");
                    break;
                case 438:
                    nameBuilder.Append("/ITU-T/Administration/[Turkmenistan]");
                    break;
                // TODO: case 440: goto oid_0_2_440;
                case 441:
                case 442:
                case 443:
                    nameBuilder.Append("/ITU-T/Administration/[Japan]");
                    break;
                // TODO: case 450: goto oid_0_2_450;
                case 452:
                    nameBuilder.Append("/ITU-T/Administration/[Viet Nam]");
                    break;
                case 453:
                case 454:
                    nameBuilder.Append("/ITU-T/Administration/[Hong Kong, China]");
                    break;
                case 455:
                    nameBuilder.Append("/ITU-T/Administration/[Macau, China]");
                    break;
                case 456:
                    nameBuilder.Append("/ITU-T/Administration/[Kingdom of Cambodia]");
                    break;
                case 457:
                    nameBuilder.Append("/ITU-T/Administration/[Lao People's Democratic Republic]");
                    break;
                case 460:
                    nameBuilder.Append("/ITU-T/Administration/[People's Republic of China]");
                    break;
                case 466:
                    nameBuilder.Append("/ITU-T/Administration/[Taiwan, Province of China]");
                    break;
                case 467:
                    nameBuilder.Append("/ITU-T/Administration/[Democratic People's Republic of Korea]");
                    break;
                case 470:
                    nameBuilder.Append("/ITU-T/Administration/[The People's Republic of Bangladesh]");
                    break;
                case 472:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of MALDIVES]");
                    break;
                case 480:
                case 481:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Korea]");
                    break;
                case 502:
                    nameBuilder.Append("/ITU-T/Administration/[Malaysia]");
                    break;
                case 505:
                    nameBuilder.Append("/ITU-T/Administration/[AUSTRALIA]");
                    break;
                case 510:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of INDONESIA]");
                    break;
                case 515:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of the Philippines]");
                    break;
                case 520:
                    nameBuilder.Append("/ITU-T/Administration/[Thailand]");
                    break;
                case 525:
                case 526:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Singapore]");
                    break;
                case 528:
                    nameBuilder.Append("/ITU-T/Administration/[Brunei Darussalam]");
                    break;
                case 530:
                    nameBuilder.Append("/ITU-T/Administration/[New Zealand]");
                    break;
                case 534:
                    nameBuilder.Append("/ITU-T/Administration/[Commonwealth of the Northern Mariana Islands]");
                    break;
                case 535:
                    nameBuilder.Append("/ITU-T/Administration/[Guam]");
                    break;
                case 536:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Nauru]");
                    break;
                case 537:
                    nameBuilder.Append("/ITU-T/Administration/[Papua New Guinea]");
                    break;
                case 539:
                    nameBuilder.Append("/ITU-T/Administration/[Kingdom of Tonga]");
                    break;
                case 540:
                    nameBuilder.Append("/ITU-T/Administration/[Solomon Islands]");
                    break;
                case 541:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Vanuatu]");
                    break;
                case 542:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Fiji]");
                    break;
                case 543:
                    nameBuilder.Append("/ITU-T/Administration/[Wallis and Futuna (French Overseas Territory)]");
                    break;
                case 544:
                    nameBuilder.Append("/ITU-T/Administration/[American Samoa]");
                    break;
                case 545:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Kiribati]");
                    break;
                case 546:
                    nameBuilder.Append("/ITU-T/Administration/[New Caledonia (French Overseas Territory)]");
                    break;
                case 547:
                    nameBuilder.Append("/ITU-T/Administration/[French Polynesia (French Overseas Territory)]");
                    break;
                case 548:
                    nameBuilder.Append("/ITU-T/Administration/[Cook Islands]");
                    break;
                case 549:
                    nameBuilder.Append("/ITU-T/Administration/[Independent State of Samoa]");
                    break;
                case 550:
                    nameBuilder.Append("/ITU-T/Administration/[Federated States of Micronesia]");
                    break;
                case 602:
                    nameBuilder.Append("/ITU-T/Administration/[Arab Republic of Egypt]");
                    break;
                case 603:
                    nameBuilder.Append("/ITU-T/Administration/[People's Democratic Republic of Algeria]");
                    break;
                case 604:
                    nameBuilder.Append("/ITU-T/Administration/[Kingdom of Morocco]");
                    break;
                case 605:
                    nameBuilder.Append("/ITU-T/Administration/[Tunisia]");
                    break;
                case 606:
                    nameBuilder.Append("/ITU-T/Administration/[Socialist People's Libyan Arab Jamahiriya]");
                    break;
                case 607:
                    nameBuilder.Append("/ITU-T/Administration/[The Republic of the Gambia]");
                    break;
                case 608:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Senegal]");
                    break;
                case 609:
                    nameBuilder.Append("/ITU-T/Administration/[Islamic Republic of Mauritania]");
                    break;
                case 610:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Mali]");
                    break;
                case 611:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Guinea]");
                    break;
                case 612:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Côte d'Ivoire]");
                    break;
                case 613:
                    nameBuilder.Append("/ITU-T/Administration/[Burkina Faso]");
                    break;
                case 614:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of the Niger]");
                    break;
                case 615:
                    nameBuilder.Append("/ITU-T/Administration/[Togolese Republic]");
                    break;
                case 616:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Benin]");
                    break;
                case 617:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Mauritius]");
                    break;
                case 618:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Liberia]");
                    break;
                case 619:
                    nameBuilder.Append("/ITU-T/Administration/[Sierra Leone]");
                    break;
                case 620:
                    nameBuilder.Append("/ITU-T/Administration/[Ghana]");
                    break;
                case 621:
                    nameBuilder.Append("/ITU-T/Administration/[Federal Republic of Nigeria]");
                    break;
                case 622:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Chad]");
                    break;
                case 623:
                    nameBuilder.Append("/ITU-T/Administration/[Central African Republic]");
                    break;
                case 624:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Cameroon]");
                    break;
                case 625:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Cape Verde]");
                    break;
                case 626:
                    nameBuilder.Append("/ITU-T/Administration/[Democratic Republic of Sao Tome and Principe]");
                    break;
                case 627:
                    nameBuilder.Append("/ITU-T/Administration/[Equatorial Guinea]");
                    break;
                case 628:
                    nameBuilder.Append("/ITU-T/Administration/[Gabon]");
                    break;
                case 629:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of the Congo]");
                    break;
                case 630:
                    nameBuilder.Append("/ITU-T/Administration/[Democratic Republic of the Congo]");
                    break;
                case 631:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Angola]");
                    break;
                case 632:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Guinea-Bissau]");
                    break;
                case 633:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Seychelles]");
                    break;
                case 634:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of the Sudan]");
                    break;
                case 635:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Rwanda]");
                    break;
                case 636:
                    nameBuilder.Append("/ITU-T/Administration/[Federal Democratic Republic of Ethiopia]");
                    break;
                case 637:
                    nameBuilder.Append("/ITU-T/Administration/[Somali Democratic Republic]");
                    break;
                case 638:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Djibouti]");
                    break;
                case 639:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Kenya]");
                    break;
                case 640:
                    nameBuilder.Append("/ITU-T/Administration/[United Republic of Tanzania]");
                    break;
                case 641:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Uganda]");
                    break;
                case 642:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Burundi]");
                    break;
                case 643:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Mozambique]");
                    break;
                case 645:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Zambia]");
                    break;
                case 646:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Madagascar]");
                    break;
                case 647:
                    nameBuilder.Append("/ITU-T/Administration/[French Departments and Territories in the Indian Ocean]");
                    break;
                case 648:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Zimbabwe]");
                    break;
                case 649:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Namibia]");
                    break;
                case 650:
                    nameBuilder.Append("/ITU-T/Administration/[Malawi]");
                    break;
                case 651:
                    nameBuilder.Append("/ITU-T/Administration/[Kingdom of Lesotho]");
                    break;
                case 652:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Botswana]");
                    break;
                case 653:
                    nameBuilder.Append("/ITU-T/Administration/[Eswatini (formerly, Kingdom of Swaziland)]");
                    break;
                case 654:
                    nameBuilder.Append("/ITU-T/Administration/[Union of the Comoros]");
                    break;
                case 655:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of South Africa]");
                    break;
                case 658:
                    nameBuilder.Append("/ITU-T/Administration/[Eritrea]");
                    break;
                case 702:
                    nameBuilder.Append("/ITU-T/Administration/[Belize]");
                    break;
                case 704:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Guatemala]");
                    break;
                case 706:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of El Salvador]");
                    break;
                case 708:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Honduras]");
                    break;
                case 710:
                    nameBuilder.Append("/ITU-T/Administration/[Nicaragua]");
                    break;
                case 712:
                    nameBuilder.Append("/ITU-T/Administration/[Costa Rica]");
                    break;
                case 714:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Panama]");
                    break;
                case 716:
                    nameBuilder.Append("/ITU-T/Administration/[Peru]");
                    break;
                case 722:
                    nameBuilder.Append("/ITU-T/Administration/[ARGENTINE Republic]");
                    break;
                case 724:
                case 725:
                    nameBuilder.Append("/ITU-T/Administration/[Federative Republic of Brazil]");
                    break;
                case 730:
                    nameBuilder.Append("/ITU-T/Administration/[Chile]");
                    break;
                case 732:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Colombia]");
                    break;
                case 734:
                    nameBuilder.Append("/ITU-T/Administration/[Bolivarian Republic of Venezuela]");
                    break;
                case 736:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Bolivia]");
                    break;
                case 738:
                    nameBuilder.Append("/ITU-T/Administration/[Guyana]");
                    break;
                case 740:
                    nameBuilder.Append("/ITU-T/Administration/[Ecuador]");
                    break;
                case 742:
                    nameBuilder.Append("/ITU-T/Administration/[French Department of Guiana]");
                    break;
                case 744:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of PARAGUAY]");
                    break;
                case 746:
                    nameBuilder.Append("/ITU-T/Administration/[Republic of Suriname]");
                    break;
                case 748:
                    nameBuilder.Append("/ITU-T/Administration/[Eastern Republic of Uruguay]");
                    break;

                default: return null;
            }

        #endregion

        // network-operator
        #region 0.3.*

        oid_0_3:

            switch (values[index++])
            {
                case 1111:
                    nameBuilder.Append("/ITU-T/Network-Operator/[INMARSAT, Atlantic Ocean-East]");
                    goto oid_end;
                case 1112:
                    nameBuilder.Append("/ITU-T/Network-Operator/[INMARSAT, Pacific Ocean]");
                    goto oid_end;
                case 1113:
                    nameBuilder.Append("/ITU-T/Network-Operator/[INMARSAT, Indian Ocean]");
                    goto oid_end;
                case 1114:
                    nameBuilder.Append("/ITU-T/Network-Operator/[INMARSAT, Atlantic Ocean-West]");
                    goto oid_end;
                case 2023:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Greece, Packet Switched Public Data Network (HELLASPAC)]");
                    goto oid_end;
                case 2027:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Greece, LAN-NET]");
                    goto oid_end;
                case 2041:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Netherlands, Datanet 1 X.25 access]");
                    goto oid_end;
                case 2044:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Netherlands, Unisource / Unidata]");
                    goto oid_end;
                case 2046:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Netherlands, Unisource / \"VPNS\"]");
                    goto oid_end;
                case 2052:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Netherlands, Naamloze Vennootschap (NV) CasTel]");
                    goto oid_end;
                case 2053:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Netherlands, Global One Communications BV]");
                    goto oid_end;
                case 2055:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Netherlands, Rabofacet BV]");
                    goto oid_end;
                case 2057:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Netherlands, Trionet v.o.f.]");
                    goto oid_end;
                case 2062:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Belgium, Réseau de transmission de données à commutation par paquets, Data Communication Service (DCS)]");
                    goto oid_end;
                case 2064:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Belgium, CODENET]");
                    goto oid_end;
                case 2065:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Belgium, Code utilisé au niveau national pour le réseau Data Communication Service (DCS)]");
                    goto oid_end;
                case 2066:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Belgium, Unisource Belgium X.25 Service (code canceled)]");
                    goto oid_end;
                case 2067:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Belgium, MOBISTAR]");
                    goto oid_end;
                case 2068:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Belgium, Accès au réseau Data Communication Service (DCS) via le réseau telex commuté national]");
                    goto oid_end;
                case 2069:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Belgium, Acces au reseau DCS via le reseau telephonique commute national]");
                    goto oid_end;
                case 2080:
                    nameBuilder.Append("/ITU-T/Network-Operator/[France, Réseau de transmission de données à commutation par paquets \"TRANSPAC\"]");
                    goto oid_end;
                case 2081:
                    nameBuilder.Append("/ITU-T/Network-Operator/[France, Noeud de transit international]");
                    goto oid_end;
                case 2082:
                    nameBuilder.Append("/ITU-T/Network-Operator/[France, Grands services publics]");
                    goto oid_end;
                case 2083:
                    nameBuilder.Append("/ITU-T/Network-Operator/[France, Administrations]");
                    goto oid_end;
                case 2084:
                    nameBuilder.Append("/ITU-T/Network-Operator/[France, Air France]");
                    goto oid_end;
                case 2085:
                    nameBuilder.Append("/ITU-T/Network-Operator/[France, \"SIRIS\"]");
                    goto oid_end;
                case 2086:
                    nameBuilder.Append("/ITU-T/Network-Operator/[France, BT France]");
                    goto oid_end;
                case 2089:
                    nameBuilder.Append("/ITU-T/Network-Operator/[France, Interconnexion entre le réseau public de transmission de données Transpac et d'autres réseaux publics français, pour des services offerts en mode synchrone]");
                    goto oid_end;
                case 2135:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Andorra, ANDORPAC]");
                    goto oid_end;
                case 2140:
                    nameBuilder.Append("/ITU-T/Network-Operator/[Spain, Administracion Publica]");
                    goto oid_end;

                // TODO: Left off at http://www.oid-info.com/get/0.3.2141
                default: return null;
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
                default:
                    return null;
            }


        #endregion

        // joint-iso-itu-t, joint-iso-ccitt
        #region 2.*

        oid_2:

            switch (values[index++])
            {
                case 0:
                    nameBuilder.Append("/Joint-ISO-ITU-T/[presentation]");
                    goto oid_end;
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
                case 999:
                    nameBuilder.Append("/Joint-ISO-ITU-T/Example");
                    goto oid_end;
                default:
                    return null;
            }


        #endregion

        oid_end:

            // TODO: Handle trailing values (like hashes)

            // Create and return the string
            return nameBuilder.ToString();
        }

        #endregion
    }
}
