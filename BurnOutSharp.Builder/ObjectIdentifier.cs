using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BurnOutSharp.Builder
{
    /// <summary>
    /// Methods related to Object Identifiers (OID)
    /// </summary>
    public static class ObjectIdentifier
    {
        // TODO: ulong[] isn't going to work. If we can use .NET 7, we can use UInt128
        // We might want to look into storing all values as GUID? I don't remember if
        // you can do value comparisions between an integral value and a GUID, though.

        #region Byte Array to Separated Value

        /// <summary>
        /// Parse an OID in DER-encoded byte notation into a list of values
        /// </summary>
        /// <param name="data">Byte array representing the data to read</param>
        /// <param name="length">Total length of the data according to the DER TLV</param>
        /// <returns>Array of values representing the OID</returns>
        public static ulong[] ParseDERIntoArray(byte[] data, ulong length)
        {
            // The first byte contains nodes 1 and 2
            int firstNode = Math.DivRem(data[0], 40, out int secondNode);

            // Create a list for all nodes
            List<ulong> nodes = new List<ulong> { (ulong)firstNode, (ulong)secondNode };

            // All other nodes are encoded uniquely
            int offset = 1;
            while (offset < (long)length)
            {
                // If bit 7 is not set
                if ((data[offset] & 0x80) == 0)
                {
                    nodes.Add(data[offset]);
                    offset++;
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
                    if ((data[offset] & 0x80) == 0)
                        doneProcessing = true;

                    // Clear the top byte
                    unchecked { data[offset] &= (byte)~0x80; }

                    // Add the new value to the result
                    dotValue |= data[offset];

                    // Increment the offset
                    offset++;
                } while (offset < data.Length && !doneProcessing);

                // Add the parsed value to the output
                nodes.Add(dotValue);
            }

            return nodes.ToArray();
        }

        #endregion

        #region Separated Value to ASN.1

        /// <summary>
        /// Parse an OID in separated-value notation into ASN.1 notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <param name="index">Current index into the list</param>
        /// <returns>ASN.1 formatted string, if possible</returns>
        /// <remarks>
        public static string ParseOIDToASN1Notation(ulong[] values, ref int index)
        {
            // TODO: Once the modified OID-IRI formatting is done, make an ASN.1 notation version
            return null;
        }

        #endregion

        #region Separated Value to Dot

        /// <summary>
        /// Parse an OID in separated-value notation into dot notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <returns>List of values representing the dot notation</returns>
        public static string ParseOIDToDotNotation(ulong[] values)
        {
            return string.Join(".", values);
        }

        #endregion

        #region Separated Value to OID-IRI

        /// <summary>
        /// Parse an OID in separated-value notation into OID-IRI notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <param name="index">Current index into the list</param>
        /// <returns>OID-IRI formatted string, if possible</returns>
        /// <see href="http://www.oid-info.com/index.htm"/>
        public static string ParseOIDToOIDIRINotation(ulong[] values)
        {
            // If we have an invalid set of values, we can't do anything
            if (values == null || values.Length == 0)
                return null;

            // Set the initial index
            int index = 0;

            // Get a string builder for the path
            var nameBuilder = new StringBuilder();

            // Try to parse the standard value
            string standard = ParseOIDToOIDIRINotation(values, ref index);
            if (standard == null)
                return null;

            // Add the standard value to the output
            nameBuilder.Append(standard);

            // If we have no more items
            if (index == values.Length)
                return nameBuilder.ToString();

            // Add trailing items as just values
            nameBuilder.Append("/");
            nameBuilder.Append(string.Join("/", values.Skip(index)));

            // Create and return the string
            return nameBuilder.ToString();
        }

        /// <summary>
        /// Parse an OID in separated-value notation into OID-IRI notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <param name="index">Current index into the list</param>
        /// <returns>OID-IRI formatted string, if possible</returns>
        /// <see href="http://www.oid-info.com/index.htm"/>
        private static string ParseOIDToOIDIRINotation(ulong[] values, ref int index)
        {
            // TODO: Once the modified OID-IRI formatting is done, make a compliant version
            return null;
        }

        #endregion

        #region Separated Value to Modified OID-IRI

        /// <summary>
        /// Parse an OID in dot notation into modified OID-IRI notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <returns>OID-IRI formatted string, if possible</returns>
        /// <remarks>
        /// If a value does not have a fully-descriptive name, it may be replaced by
        /// a string from the official description. As such, the output of this is
        /// not considered to be fully OID-IRI compliant.
        /// </remarks>
        /// <see href="http://www.oid-info.com/index.htm"/>
        public static string ParseOIDToModifiedOIDIRI(ulong[] values)
        {
            // If we have an invalid set of values, we can't do anything
            if (values == null || values.Length == 0)
                return null;

            // Set the initial index
            int index = 0;

            // Get a string builder for the path
            var nameBuilder = new StringBuilder();

            // Try to parse the standard value
            string standard = ParseOIDToModifiedOIDIRI(values, ref index);
            if (standard == null)
                return null;

            // Add the standard value to the output
            nameBuilder.Append(standard);

            // If we have no more items
            if (index == values.Length)
                return nameBuilder.ToString();

            // Add trailing items as just values
            nameBuilder.Append("/");
            nameBuilder.Append(string.Join("/", values.Skip(index)));

            // Create and return the string
            return nameBuilder.ToString();
        }

        /// <summary>
        /// Parse an OID in separated-value notation into modified OID-IRI notation
        /// </summary>
        /// <param name="values">List of values to check against</param>
        /// <param name="index">Current index into the list</param>
        /// <returns>OID-IRI formatted string, if possible</returns>
        /// <remarks>
        /// If a value does not have a fully-descriptive name, it may be replaced by
        /// a string from the official description. As such, the output of this is
        /// not considered to be fully OID-IRI compliant.
        /// </remarks>
        private static string ParseOIDToModifiedOIDIRI(ulong[] values, ref int index)
        {
            // If we have an invalid set of values, we can't do anything
            if (values == null || values.Length == 0)
                return null;

            // If we have an invalid index, we can't do anything
            if (index < 0 || index >= values.Length)
                return null;

            #region Start

            oid_start:

            switch (values[index++])
            {
                case 0: goto oid_0;
                case 1: goto oid_1;
                case 2: goto oid_2;
                default: return $"/{values[index - 1]}";
            }

        #endregion

        // itu-t, ccitt, itu-r
        #region 0.*

        oid_0:

            if (index == values.Length) return "/ITU-T";
            switch (values[index++])
            {
                case 0: goto oid_0_0;
                case 1: return "/ITU-T/1/[question]";
                case 2: goto oid_0_2;
                case 3: goto oid_0_3;
                case 4: goto oid_0_4;
                case 5: return "/ITU-R/R-Recommendation";
                case 9: goto oid_0_9;
                default: return $"/ITU-T/{values[index - 1]}";
            };

        // recommendation
        #region 0.0.*

        oid_0_0:

            if (index == values.Length) return "/ITU-T/Recommendation";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/A";
                case 2: return "/ITU-T/Recommendation/B";
                case 3: return "/ITU-T/Recommendation/C";
                case 4: return "/ITU-T/Recommendation/D";
                case 5: goto oid_0_0_5;
                case 6: return "/ITU-T/Recommendation/F";
                case 7: goto oid_0_0_7;
                case 8: goto oid_0_0_8;
                case 9: goto oid_0_0_9;
                case 10: return "/ITU-T/Recommendation/J";
                case 11: return "/ITU-T/Recommendation/K";
                case 12: return "/ITU-T/Recommendation/L";
                case 13: goto oid_0_0_13;
                case 14: return "/ITU-T/Recommendation/N";
                case 15: return "/ITU-T/Recommendation/O";
                case 16: return "/ITU-T/Recommendation/P";
                case 17: goto oid_0_0_17;
                case 18: return "/ITU-T/Recommendation/R";
                case 19: return "/ITU-T/Recommendation/S";
                case 20: goto oid_0_0_20;
                case 21: return "/ITU-T/Recommendation/U";
                case 22: goto oid_0_0_22;
                case 24: goto oid_0_0_24;
                case 25: return "/ITU-T/Recommendation/Y";
                case 26: return "/ITU-T/Recommendation/Z";
                case 59: return "/ITU-T/Recommendation/[xcmJobZeroDummy]";
                case 74: return "/ITU-T/Recommendation/[xcmSvcMonZeroDummy]";
                default: return $"/ITU-T/Recommendation/{values[index - 1]}";
            }

        // e
        #region 0.0.5.*

        oid_0_0_5:

            if (index == values.Length) return "/ITU-T/Recommendation/E";
            switch (values[index++])
            {
                case 115: goto oid_0_0_5_115;
                default: return $"/ITU-T/Recommendation/E/{values[index - 1]}";
            }

        #region 0.0.5.115.*

        oid_0_0_5_115:

            if (index == values.Length) return "/ITU-T/Recommendation/E/[Computerized directory assistance]";
            switch (values[index++])
            {
                case 1: goto oid_0_0_5_115_1;
                case 2: goto oid_0_0_5_115_2;
                default: return $"/ITU-T/Recommendation/E/[Computerized directory assistance]/{values[index - 1]}";
            }

        #region 0.0.5.115.1.*

        oid_0_0_5_115_1:

            if (index == values.Length) return "/ITU-T/Recommendation/E/[Computerized directory assistance]/[E115v1]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/E/[Computerized directory assistance]/[E115v1]/[Version 1.00]";
                default: return $"/ITU-T/Recommendation/E/[Computerized directory assistance]/[E115v1]/{values[index - 1]}";
            }

        #endregion

        #region 0.0.5.115.2.*

        oid_0_0_5_115_2:

            if (index == values.Length) return "/ITU-T/Recommendation/E/[Computerized directory assistance]/[E115v2]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/E/[Computerized directory assistance]/[E115v2]/[Version 2.00]";
                case 1: return "/ITU-T/Recommendation/E/[Computerized directory assistance]/[E115v2]/[Version 2.01]";
                case 10: return "/ITU-T/Recommendation/E/[Computerized directory assistance]/[E115v2]/[Version 2.10]";
                default: return $"/ITU-T/Recommendation/E/[Computerized directory assistance]/[E115v2]/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        // g
        #region 0.0.7.*

        oid_0_0_7:

            if (index == values.Length) return "/ITU-T/Recommendation/G";
            switch (values[index++])
            {
                case 711: goto oid_0_0_7_711;
                case 719: goto oid_0_0_7_719;
                case 726: goto oid_0_0_7_726;
                case 774: goto oid_0_0_7_774;
                case 7221: goto oid_0_0_7_7221;
                case 7222: goto oid_0_0_7_7222;
                case 7761: goto oid_0_0_7_7761;
                case 85501: goto oid_0_0_7_85501;
                default: return $"/ITU-T/Recommendation/G/{values[index - 1]}";
            }

        // g711
        #region 0.0.7.711.*

        oid_0_0_7_711:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[G.711 series]";
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_711_1;
                default: return $"/ITU-T/Recommendation/G/[G.711 series]/{values[index - 1]}";
            }

        // dot
        #region 0.0.7.711.1.*

        oid_0_0_7_711_1:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[G.711 series]/[G.711.x series of Recommendations]";
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_711_1_1;
                default: return $"/ITU-T/Recommendation/G/[G.711 series]/[G.711.x series of Recommendations]/{values[index - 1]}";
            }

        // part1
        #region 0.0.7.711.1.1.*

        oid_0_0_7_711_1_1:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[G.711 series]/[G.711.x series of Recommendations]/[Wideband embedded extension for G.711 pulse code modulation]";
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_711_1_1_1;
                default: return $"/ITU-T/Recommendation/G/[G.711 series]/[G.711.x series of Recommendations]/[Wideband embedded extension for G.711 pulse code modulation]/{values[index - 1]}";
            }

        // generic-capabilities
        #region 0.0.7.711.1.1.1.*

        oid_0_0_7_711_1_1_1:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[G.711 series]/[G.711.x series of Recommendations]/[Wideband embedded extension for G.711 pulse code modulation]/[Generic capabilities]";
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_711_1_1_1_0;
                case 1: goto oid_0_0_7_711_1_1_1_1;
                default: return $"/ITU-T/Recommendation/G/[G.711 series]/[G.711.x series of Recommendations]/[Wideband embedded extension for G.711 pulse code modulation]/[Generic capabilities]/{values[index - 1]}";
            }

        // u-law
        #region 0.0.7.711.1.1.1.0.*

        oid_0_0_7_711_1_1_1_0:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[G.711 series]/[G.711.x series of Recommendations]/[Wideband embedded extension for G.711 pulse code modulation]/[Generic capabilities]/[μ-law capability identifier]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[G.711 series]/[G.711.x series of Recommendations]/[Wideband embedded extension for G.711 pulse code modulation]/[Generic capabilities]/[μ-law capability identifier]/[μ-law core capability identifier]";
                default: return $"/ITU-T/Recommendation/G/[G.711 series]/[G.711.x series of Recommendations]/[Wideband embedded extension for G.711 pulse code modulation]/[Generic capabilities]/[μ-law capability identifier]/{values[index - 1]}";
            }

        #endregion

        // a-law
        #region 0.0.7.711.1.1.1.1.*

        oid_0_0_7_711_1_1_1_1:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[G.711 series]/[G.711.x series of Recommendations]/[Wideband embedded extension for G.711 pulse code modulation]/[Generic capabilities]/[a-law]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[G.711 series]/[G.711.x series of Recommendations]/[Wideband embedded extension for G.711 pulse code modulation]/[Generic capabilities]/[a-law]/[a-law core capability identifier]";
                default: return $"/ITU-T/Recommendation/G/[G.711 series]/[G.711.x series of Recommendations]/[Wideband embedded extension for G.711 pulse code modulation]/[Generic capabilities]/[a-law]/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        #endregion

        #endregion

        // 719
        #region 0.0.7.719.*

        oid_0_0_7_719:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Low-complexity, full-band audio coding for high-quality, conversational applications]";
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_719_1;
                default: return $"/ITU-T/Recommendation/G/[Low-complexity, full-band audio coding for high-quality, conversational applications]/{values[index - 1]}";
            }

        // generic-capabilities
        #region 0.0.7.719.1.*

        oid_0_0_7_719_1:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Low-complexity, full-band audio coding for high-quality, conversational applications]/[Generic capabilities]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Low-complexity, full-band audio coding for high-quality, conversational applications]/[Generic capabilities]/[capability]";
                default: return $"/ITU-T/Recommendation/G/[Low-complexity, full-band audio coding for high-quality, conversational applications]/[Generic capabilities]/{values[index - 1]}";
            }

        #endregion

        #endregion

        // 726
        #region 0.0.7.726.*

        oid_0_0_7_726:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[40, 32, 24, 16 kbit/s Adaptive Differential Pulse Code Modulation (ADPCM)]";
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_726_1;
                default: return $"/ITU-T/Recommendation/G/[40, 32, 24, 16 kbit/s Adaptive Differential Pulse Code Modulation (ADPCM)]/{values[index - 1]}";
            }

        // generic-capabilities
        #region 0.0.7.726.*

        oid_0_0_7_726_1:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[40, 32, 24, 16 kbit/s Adaptive Differential Pulse Code Modulation (ADPCM)]/[generic capabilities]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[40, 32, 24, 16 kbit/s Adaptive Differential Pulse Code Modulation (ADPCM)]/[generic capabilities]/[Version 2003]";
                default: return $"/ITU-T/Recommendation/G/[40, 32, 24, 16 kbit/s Adaptive Differential Pulse Code Modulation (ADPCM)]/[generic capabilities]/{values[index - 1]}";
            }

        #endregion

        #endregion

        // sdhm, g774
        #region 0.0.7.774.*

        oid_0_0_7_774:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]";
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_0;
                case 1: goto oid_0_0_7_774_1;
                case 2: goto oid_0_0_7_774_2;
                case 127: goto oid_0_0_7_774_127;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.0.*

        oid_0_0_7_774_0:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Standard-specific extensions to the allocation scheme]";
                case 2: goto oid_0_0_7_774_0_2;
                case 3: goto oid_0_0_7_774_0_3;
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[GDMO packages]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Guidelines for the Definition of Managed Objects (GDMO) parameters]";
                case 6: goto oid_0_0_7_774_0_6;
                case 7: goto oid_0_0_7_774_0_7;
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Guidelines for the Definition of Managed Objects (GDMO) attribute groups]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Actions]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Guidelines for the Definition of Managed Objects (GDMO) notifications]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.0.2.*

        oid_0_0_7_774_0_2:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[ASN.1 modules]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[ASN.1 modules]/[SDH]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[ASN.1 modules]/{values[index - 1]}";
            }

        #endregion

        // managedObjectClass
        #region 0.0.7.774.0.3.*

        oid_0_0_7_774_0_3:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]";
            switch (values[index++])
            {
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[au3CTPSource]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[au4CTPSource]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[augBidirectional]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[augSink]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[augSource]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[electricalSPITTPBidirectional]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[electricalSPITTPSink]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[electricalSPITTPSource]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[indirectAdaptorBidirectional]";
                case 14: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[indirectAdaptorSink]";
                case 15: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[indirectAdaptorSource]";
                case 16: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[msCTPBidirectional]";
                case 17: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[msCTPSink]";
                case 18: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[msCTPSource]";
                case 19: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[msDatacomCTPBidirectional]";
                case 20: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[msDatacomCTPSink]";
                case 21: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[msDatacomCTPSource]";
                case 22: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[msOrderwireCTPBidirectional]";
                case 23: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[msOrderwireCTPSink]";
                case 24: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[msOrderwireCTPSource]";
                case 25: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[msTTPBidirectional]";
                case 26: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[msTTPSink]";
                case 27: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[msTTPSource]";
                case 28: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[opticalSPITTPBidirectional]";
                case 29: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[opticalSPITTPSink]";
                case 30: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[opticalSPITTPSource]";
                case 31: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsCTPBidirectional]";
                case 32: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsCTPSink]";
                case 33: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsCTPSource]";
                case 34: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsDatacomCTPBidirectional]";
                case 35: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsDatacomCTPSink]";
                case 36: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsDatacomCTPSource]";
                case 37: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsOrderwireCTPBidirectional]";
                case 38: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsOrderwireCTPSink]";
                case 39: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsOrderwireCTPSource]";
                case 40: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsTTPBidirectional]";
                case 41: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsTTPSink]";
                case 42: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsTTPSource]";
                case 43: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsUserChannelCTPBidirectional]";
                case 44: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsUserChannelCTPSink]";
                case 45: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsUserChannelCTPSource]";
                case 46: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[sdhNE]";
                case 49: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tu11CTPSource]";
                case 52: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tu12CTPSource]";
                case 55: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tu2CTPSource]";
                case 58: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tu3CTPSource]";
                case 59: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tug2Bidirectional]";
                case 60: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tug2Sink]";
                case 61: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tug2Source]";
                case 62: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tug3Bidirectional]";
                case 63: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tug3Sink]";
                case 64: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tug3Source]";
                case 67: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc11TTPSource]";
                case 70: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc12TTPSource]";
                case 73: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc2TTPSource]";
                case 80: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vcnUserChannelCTPBidirectional]";
                case 81: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vcnUserChannelCTPSink]";
                case 82: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vcnUserChannelCTPSource]";
                case 83: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[au3CTPBidirectionalR1]";
                case 84: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[au3CTPSinkR1]";
                case 85: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[au4CTPBidirectionalR1]";
                case 86: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[au4CTPSinkR1]";
                case 87: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tu11CTPBidirectionalR1]";
                case 88: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tu11CTPSinkR1]";
                case 89: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tu12CTPBidirectionalR1]";
                case 90: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tu12CTPSinkR1]";
                case 91: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tu2CTPBidirectionalR1]";
                case 92: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tu2CTPSinkR1]";
                case 93: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tu3CTPBidirectionalR1]";
                case 94: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[tu3CTPBidirectionalR1]";
                case 95: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc11TTPBidirectionalR1]";
                case 96: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc11TTPSinkR1]";
                case 97: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc12TTPBidirectionalR1]";
                case 98: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc12TTPSinkR1]";
                case 99: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc2TTPBidirectionalR1]";
                case 100: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc2TTPSinkR1]";
                case 101: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc3TTPBidirectionalR1]";
                case 102: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc3TTPSinkR1]";
                case 103: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc3TTPSourceR1]";
                case 104: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc4TTPBidirectionalR1]";
                case 105: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc4TTPSinkR1]";
                case 106: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[vc4TTPSourceR1]";
                case 107: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsTTPTrailTraceBidirectional]";
                case 108: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsTTPTrailTraceSink]";
                case 109: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/[rsTTPTrailTraceSource]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Managed object classes]/{values[index - 1]}";
            }

        #endregion

        // nameBinding
        #region 0.0.7.774.0.6.*

        oid_0_0_7_774_0_6:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]";
            switch (values[index++])
            {
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[au3CTPSource-augBidirectional]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[au3CTPSource-augSource]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[au4CTPSource-augBidirectional]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[au4CTPSource-augSource]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[augBidirectional-msTTPBidirectional]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[augSink-msTTPSink]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[augSource-msTTPSource]";
                case 14: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[electricalSPITTPBidirectional-sdhNE]";
                case 15: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[electricalSPITTPSink-sdhNE]";
                case 16: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[electricalSPITTPSource-sdhNE]";
                case 17: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msCTPBidirectional-rsTTPBidirectional]";
                case 18: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msCTPSink-rsTTPBidirectional]";
                case 19: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msCTPSink-rsTTPSink]";
                case 20: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msCTPSource-rsTTPBidirectional]";
                case 21: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msCTPSource-rsTTPSource]";
                case 22: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msDatacomCTPBidirectional-msTTPBidirectional]";
                case 23: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msDatacomCTPSink-msTTPBidirectional]";
                case 24: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msDatacomCTPSink-msTTPSink]";
                case 25: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msDatacomCTPSource-msTTPBidirectional]";
                case 26: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msDatacomCTPSource-msTTPSource]";
                case 27: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msOrderwireCTPBidirectional-msTTPBidirectional]";
                case 28: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msOrderwireCTPSink-msTTPBidirectional]";
                case 29: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msOrderwireCTPSink-msTTPSink]";
                case 30: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msOrderwireCTPSource-msTTPBidirectional]";
                case 31: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msOrderwireCTPSource-msTTPSource]";
                case 32: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msTTPBidirectional-sdhNE]";
                case 33: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msTTPSink-sdhNE]";
                case 34: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[msTTPSource-sdhNE]";
                case 35: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[opticalSPITTPBidirectional-sdhNE]";
                case 36: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[opticalSPITTPSink-sdhNE]";
                case 37: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[opticalSPITTPSource-sdhNE]";
                case 38: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsCTPBidirectional-electricalSPITTPBidirectional]";
                case 39: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsCTPSink-electricalSPITTPBidirectional]";
                case 40: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsCTPSink-electricalSPITTPSink]";
                case 41: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsCTPSource-electricalSPITTPBidirectional]";
                case 42: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsCTPSource-electricalSPITTPSource]";
                case 43: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsCTPBidirectional-opticalSPITTPBidirectional]";
                case 44: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsCTPSink-opticalSPITTPBidirectional]";
                case 45: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsCTPSink-opticalSPITTPSink]";
                case 46: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsCTPSource-opticalSPITTPBidirectional]";
                case 47: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsCTPSource-opticalSPITTPSource]";
                case 48: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsDatacomCTPBidirectional-rsTTPBidirectional]";
                case 49: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsDatacomCTPSink-rsTTPBidirectional]";
                case 50: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsDatacomCTPSink-rsTTPSink]";
                case 51: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsDatacomCTPSource-rsTTPBidirectional]";
                case 52: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsDatacomCTPSource-rsTTPSource]";
                case 53: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsOrderwireCTPBidirectional-rsTTPBidirectional]";
                case 54: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsOrderwireCTPSink-rsTTPBidirectional]";
                case 55: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsOrderwireCTPSink-rsTTPSink]";
                case 56: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsOrderwireCTPSource-rsTTPBidirectional]";
                case 57: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsOrderwireCTPSource-rsTTPSource]";
                case 58: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsTTPBidirectional-sdhNE]";
                case 59: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsTTPSink-sdhNE]";
                case 60: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsTTPSource-sdhNE]";
                case 61: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsUserChannelCTPBidirectional-rsTTPBidirectional]";
                case 62: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsUserChannelCTPSink-rsTTPBidirectional]";
                case 63: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsUserChannelCTPSink-rsTTPSink]";
                case 64: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsUserChannelCTPSource-rsTTPBidirectional]";
                case 65: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[rsUserChannelCTPSource-rsTTPSource]";
                case 69: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu11CTPSource-tug2Bidirectional]";
                case 70: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu11CTPSource-tug2Source]";
                case 74: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu12CTPSource-tug2Bidirectional]";
                case 75: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu12CTPSource-tug2Source]";
                case 79: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu2CTPSource-tug2Bidirectional]";
                case 80: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu2CTPSource-tug2Source]";
                case 84: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu3CTPSource-tug3Bidirectional]";
                case 85: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu3CTPSource-tug3Source]";
                case 86: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tug2Bidirectional-tug3Bidirectional]";
                case 87: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tug2Sink-tug3Sink]";
                case 88: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tug2Source-tug3Source]";
                case 97: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc11TTPSource-sdhNE]";
                case 100: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc12TTPSource-sdhNE]";
                case 103: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc2TTPSource-sdhNE]";
                case 121: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[au3CTPBidirectionalR1-augBidirectional]";
                case 122: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[au3CTPSinkR1-augBidirectional]";
                case 123: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[au3CTPSinkR1-augSink]";
                case 124: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[au4CTPBidirectionalR1-augBidirectional]";
                case 125: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[au4CTPSinkR1-augBidirectional]";
                case 126: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[au4CTPSinkR1-augSink]";
                case 127: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu11CTPBidirectionalR1-tug2Bidirectional]";
                case 128: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu11CTPSinkR1-tug2Bidirectional]";
                case 129: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu11CTPSinkR1-tug2Sink]";
                case 130: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu12CTPBidirectionalR1-tug2Bidirectional]";
                case 131: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu12CTPSinkR1-tug2Bidirectional]";
                case 132: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu12CTPSinkR1-tug2Sink]";
                case 133: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu2CTPBidirectionalR1-tug2Bidirectional]";
                case 134: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu2CTPSinkR1-tug2Bidirectional]";
                case 135: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu2CTPSinkR1-tug2Sink]";
                case 136: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu3CTPBidirectionalR1-tug3Bidirectional]";
                case 137: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu3CTPSinkR1-tug3Bidirectional]";
                case 138: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tu3CTPSinkR1-tug3Sink]";
                case 139: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tug2Bidirectional-vc3TTPBidirectionalR1]";
                case 140: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tug2Sink-vc3TTPSinkR1]";
                case 141: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tug2Source-vc3TTPSourceR1]";
                case 142: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tug3Bidirectional-vc4TTPBidirectionalR1]";
                case 143: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tug3Sink-vc4TTPSinkR1]";
                case 144: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[tug3Source-vc4TTPSourceR1]";
                case 145: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc11TTPBidirectionalR1-sdhNE]";
                case 146: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc11TTPSinkR1-sdhNE]";
                case 147: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc12TTPBidirectionalR1-sdhNE]";
                case 148: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc12TTPSinkR1-sdhNE]";
                case 149: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc2TTPBidirectionalR1-sdhNE]";
                case 150: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc2TTPSinkR1-sdhNE]";
                case 151: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc3TTPBidirectionalR1-sdhNE]";
                case 152: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc3TTPSinkR1-sdhNE]";
                case 153: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc3TTPSourceR1-sdhNE]";
                case 154: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc4TTPBidirectionalR1-sdhNE]";
                case 155: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc4TTPSinkR1-sdhNE]";
                case 156: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vc4TTPSourceR1-sdhNE]";
                case 157: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vcnUserChannelCTPBidirectional-vc3TTPBidirectionalR1]";
                case 158: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vcnUserChannelCTPSink-vc3TTPBidirectionalR1]";
                case 159: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vcnUserChannelCTPSink-vc3TTPSinkR1]";
                case 160: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vcnUserChannelCTPSource-vc3TTPBidirectionalR1]";
                case 161: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vcnUserChannelCTPSource-vc3TTPSourceR1]";
                case 162: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vcnUserChannelCTPBidirectional-vc4TTPBidirectionalR1]";
                case 163: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vcnUserChannelCTPSink-vc4TTPBidirectionalR1]";
                case 164: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vcnUserChannelCTPSink-vc4TTPSinkR1]";
                case 165: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vcnUserChannelCTPSource-vc4TTPBidirectionalR1]";
                case 166: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/[vcnUserChannelCTPSource-vc4TTPSourceR1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Name bindings]/{values[index - 1]}";
            }

        #endregion

        // attribute
        #region 0.0.7.774.0.7.*

        oid_0_0_7_774_0_7:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[electricalSPIPackage]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[au4CTPId]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[augId]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[c2SignalLabelExpected]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[c2SignalLabelReceive]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[c2SignalLabelSend]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[electricalSPITTPId]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[excessiveBERMtceInhibit]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[j1PathTraceExpected]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[j1PathTraceReceive]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[j1PathTraceSend]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[msCTPId]";
                case 14: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[msDatacomCTPId]";
                case 15: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[msOrderwireCTPId]";
                case 16: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[msTTPId]";
                case 17: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[opticalReach]";
                case 18: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[opticalSPITTPId]";
                case 19: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[opticalWavelength]";
                case 20: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[pointerSinkType]";
                case 21: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[pointerSourceType]";
                case 22: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[rsCTPId]";
                case 23: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[rsDatacomCTPId]";
                case 24: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[rsOrderwireCTPId]";
                case 25: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[rsTTPId]";
                case 26: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[rsUserChannelCTPId]";
                case 27: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[signalDegradeThreshold]";
                case 28: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[stmLevel]";
                case 29: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[tu11CTPId]";
                case 30: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[tu12CTPId]";
                case 31: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[tu2CTPId]";
                case 32: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[tu3CTPId]";
                case 33: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[tug2Id]";
                case 34: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[tug3Id]";
                case 35: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[v5SignalLabelExpected]";
                case 36: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[v5SignalLabelReceive]";
                case 37: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[v5SignalLabelSend]";
                case 38: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[vc11TTPId]";
                case 39: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[vc12TTPId]";
                case 40: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[vc2TTPId]";
                case 41: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[vc3TTPId]";
                case 42: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[vc4TTPId]";
                case 43: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[vcnUserChannelCTPId]";
                case 44: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[trailTraceExpected]";
                case 45: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[trailTraceReceive]";
                case 46: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/[trailTraceSend]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Information model]/[Attributes]/{values[index - 1]}";
            }

        #endregion

        #endregion

        // protocolSupport
        #region 0.0.7.774.1.*

        oid_0_0_7_774_1:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Protocol support]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Protocol support]/[Application contexts]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Protocol support]/{values[index - 1]}";
            }

        #endregion

        // managementApplicationSupport
        #region 0.0.7.774.2.*

        oid_0_0_7_774_2:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Management application support]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Management application support]/[Standard-specific extensions to the allocation scheme]";
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Management application support]/[Functional unit packages]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Management application support]/[ASN.1 modules]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Management application support]/{values[index - 1]}";
            }

        #endregion

        // dot, hyphen
        #region 0.0.7.774.127.*

        oid_0_0_7_774_127:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]";
            switch (values[index++])
            {
                case 1: goto oid_0_0_7_774_127_1;
                case 2: goto oid_0_0_7_774_127_2;
                case 3: goto oid_0_0_7_774_127_3;
                case 4: goto oid_0_0_7_774_127_4;
                case 5: goto oid_0_0_7_774_127_5;
                case 6: goto oid_0_0_7_774_127_6;
                case 7: goto oid_0_0_7_774_127_7;
                case 8: goto oid_0_0_7_774_127_8;
                case 9: goto oid_0_0_7_774_127_9;
                case 10: goto oid_0_0_7_774_127_10;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/{values[index - 1]}";
            }

        // part1, pm
        #region 0.0.7.774.127.1.*

        oid_0_0_7_774_127_1:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]";
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_1_0;
                case 1: goto oid_0_0_7_774_127_1_1;
                case 2: goto oid_0_0_7_774_127_1_2;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.1.0.*

        oid_0_0_7_774_127_1_0:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Standard specific extension]";
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Functional unit packages]";
                case 2: goto oid_0_0_7_774_127_1_0_2;
                case 3: goto oid_0_0_7_774_127_1_0_3;
                case 4: goto oid_0_0_7_774_127_1_0_4;
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Parameters]";
                case 6: goto oid_0_0_7_774_127_1_0_6;
                case 7: goto oid_0_0_7_774_127_1_0_7;
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attribute groups]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Actions]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Notifications]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.1.0.2.*

        oid_0_0_7_774_127_1_0_2:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[ASN.1 modules]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[ASN.1 modules]/[SDHPMASN1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[ASN.1 modules]/{values[index - 1]}";
            }

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.1.0.3.*

        oid_0_0_7_774_127_1_0_3:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[sdhCurrentData]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[rsCurrentData]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[rsCurrentDataTR]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[electricalSourceSPICurrentData]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[opticalSourceSPICurrentData]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[msCurrentData]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[msCurrentDataTR]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[protectionCurrentData]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[pathTerminationCurrentData]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[pathTerminationCurrentDataTR]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[msAdaptationCurrentData]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[rsHistoryData]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[electricalSPIHistoryData]";
                case 14: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[opticalSPIHistoryData]";
                case 15: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[msHistoryData]";
                case 16: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[protectionHistoryData]";
                case 17: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[pathTerminationHistoryData]";
                case 18: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/[msAdaptationHistoryData]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Managed object classes]/{values[index - 1]}";
            }

        #endregion

        // package
        #region 0.0.7.774.127.1.0.4.*

        oid_0_0_7_774_127_1_0_4:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[cSESCurrentDataPackage]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[farEndCSESCurrentDataPackage]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[farEndCurrentDataPackage]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[farEndHistoryDataPackage]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[historyPackage]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[laserBiasCurrentDataPackage]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[laserBiasTideMarkPackage]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[laserTemperatureCurrentDataPackage]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[laserTemperatureTideMarkPackage]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[oFSCurrentDataPackage]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[oFSHistoryDataPackage]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[transmitPowerLevelCurrentDataPackage]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[transmitPowerLevelTideMarkPackage]";
                case 14: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[thresholdResetPackage]";
                case 15: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[uASCurrentDataPackage]";
                case 16: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[uASHistoryDataPackage]";
                case 17: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/[unavailableTimeAlarmPackage]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Packages]/{values[index - 1]}";
            }

        #endregion

        // nameBinding
        #region 0.0.7.774.127.1.0.6.*

        oid_0_0_7_774_127_1_0_6:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[historyData-sdhCurrentData]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[msCurrentData-msTTPSink]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[msCurrentDataTR-msTTPSink]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[msCurrentData-protectedTTPSink]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[msCurrentDataTR-protectedTTPSink]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[protectionCurrentData-protectionUnit]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[rsCurrentData-rsTTPSink]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[rsCurrentDataTR-rsTTPSink]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentData-vc4TTPSink]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentData-vc3TTPSink]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentData-vc2TTPSink]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentData-vc12TTPSink]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentData-vc11TTPSink]";
                case 14: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentDataTR-vc4TTPSink]";
                case 15: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentDataTR-vc3TTPSink]";
                case 16: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentDataTR-vc2TTPSink]";
                case 17: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentDataTR-vc12TTPSink]";
                case 18: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentDataTR-vc11TTPSink]";
                case 19: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[electricalSourceSPICurrentData-electricalSPITTPSource]";
                case 20: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[opticalSourceSPICurrentData-opticalSPITTPSource-electricalSPITTPSource]";
                case 21: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[msAdaptationCurrentData-au4CTPSource]";
                case 22: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[msAdaptationCurrentData-au3CTPSource]";
                case 23: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentData-vc4TTPSinkR1]";
                case 24: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentData-vc3TTPSinkR1]";
                case 25: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentData-vc2TTPSinkR1]";
                case 26: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentData-vc12TTPSinkR1]";
                case 27: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentData-vc11TTPSinkR1]";
                case 28: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentDataTR-vc4TTPSinkR1]";
                case 29: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentDataTR-vc3TTPSinkR1]";
                case 30: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentDataTR-vc2TTPSinkR1]";
                case 31: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentDataTR-vc12TTPSinkR1]";
                case 32: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/[pathTerminationCurrentDataTR-vc11TTPSinkR1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Name bindings]/{values[index - 1]}";
            }

        #endregion

        // attribute
        #region 0.0.7.774.127.1.0.7.*

        oid_0_0_7_774_127_1_0_7:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[cSESEvent]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[eS]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[fEES]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[fEBBE]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[fECSESEvent]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[laserBias]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[laserBiasTideMarkMax]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[laserBiasTideMarkMin]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[laserTemperature]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[laserTemperatureTideMarkMax]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[laserTemperatureTideMarkMin]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[nCSES]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[bBE]";
                case 14: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[oFS]";
                case 15: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[pSC]";
                case 16: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[pSD]";
                case 17: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[sES]";
                case 18: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[fESES]";
                case 19: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[transmitPowerLevel]";
                case 20: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[transmitPowerLevelTideMarkMax]";
                case 21: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[transmitPowerLevelTideMarkMin]";
                case 22: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[uAS]";
                case 23: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[pJCHigh]";
                case 24: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/[pJCLow]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Information model]/[Attributes]/{values[index - 1]}";
            }

        #endregion

        #endregion

        // protocolSupport
        #region 0.0.7.774.127.1.1.*

        oid_0_0_7_774_127_1_1:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Protocol support]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Protocol support]/[Application contexts]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Protocol support]/{values[index - 1]}";
            }

        #endregion

        // protocolSupport
        #region 0.0.7.774.127.1.2.*

        oid_0_0_7_774_127_1_2:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Management applications support]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Management applications support]/[Standard specific extension]";
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Management applications support]/[Functional unit packages]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Management applications support]/[ASN.1 modules]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Recommendation ITU-T G.774.1]/[Management applications support]/{values[index - 1]}";
            }

        #endregion

        #endregion

        // conf
        #region 0.0.7.774.127.2.*

        oid_0_0_7_774_127_2:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]";
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_2_0;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.2.0.*

        oid_0_0_7_774_127_2_0:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]";
            switch (values[index++])
            {
                case 2: goto oid_0_0_7_774_127_2_0_2;
                case 3: goto oid_0_0_7_774_127_2_0_3;
                case 5: goto oid_0_0_7_774_127_2_0_5;
                case 6: goto oid_0_0_7_774_127_2_0_6;
                case 9: goto oid_0_0_7_774_127_2_0_9;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.2.0.2.*

        oid_0_0_7_774_127_2_0_2:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[ASN.1 modules]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[ASN.1 modules]/[SDHConfASN1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[ASN.1 modules]/{values[index - 1]}";
            }

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.2.0.3.*

        oid_0_0_7_774_127_2_0_3:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableAugBidirectional]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableAugSink]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableAugSource]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableTug3Bidirectional]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableTug3Sink]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableTug3Source]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableTug2Bidirectional]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableTug2Sink]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableTug2Source]";
                case 18: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC2TTPSource]";
                case 21: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC12TTPSource]";
                case 24: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC11TTPSource]";
                case 25: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC4TTPBidirectionalR1]";
                case 26: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC4TTPSinkR1]";
                case 27: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC4TTPSourceR1]";
                case 28: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC3TTPBidirectionalR1]";
                case 29: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC3TTPSinkR1]";
                case 30: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC3TTPSourceR1]";
                case 31: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC2TTPBidirectionalR1]";
                case 32: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC2TTPSinkR1]";
                case 33: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC12TTPBidirectionalR1]";
                case 34: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC12TTPSinkR1]";
                case 35: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC11TTPBidirectionalR1]";
                case 36: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/[modifiableVC11TTPSinkR1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Managed object classes]/{values[index - 1]}";
            }

        #endregion

        // parameter
        #region 0.0.7.774.127.2.0.5.*

        oid_0_0_7_774_127_2_0_5:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Parameters]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Parameters]/[defineSDHStructureError]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Parameters]/{values[index - 1]}";
            }

        #endregion

        // nameBinding
        #region 0.0.7.774.127.2.0.6.*

        oid_0_0_7_774_127_2_0_6:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]";
            switch (values[index++])
            {
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[au3CTPSource-augSource]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[au4CTPSource-augSource]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[augSink-msTTPSink]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[augSource-msTTPSource]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[electricalSPITTPSink-sdhNE]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[electricalSPITTPSource-sdhNE]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[msCTPSink-rsTTPSink]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[msCTPSource-rsTTPSource]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[msDatacomCTPSink-msTTPSink]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[msDatacomCTPSource-msTTPSource]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[msOrderwireCTPSink-msTTPSink]";
                case 14: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[msOrderwireCTPSource-msTTPSource]";
                case 15: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[msTTPSink-sdhNE]";
                case 16: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[msTTPSource-sdhNE]";
                case 17: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[opticalSPITTPSink-sdhNE]";
                case 18: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[opticalSPITTPSource-sdhNE]";
                case 19: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[rsCTPSink-electricalSPITTPSink]";
                case 20: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[rsCTPSource-electricalSPITTPSource]";
                case 21: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[rsCTPSink-opticalSPITTPSink]";
                case 22: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[rsCTPSource-opticalSPITTPSource]";
                case 23: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[rsDatacomCTPSink-rsTTPSink]";
                case 24: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[rsDatacomCTPSource-rsTTPSource]";
                case 25: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[rsOrderwireCTPSink-rsTTPSink]";
                case 26: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[rsOrderwireCTPSource-rsTTPSource]";
                case 27: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[rsTTPSink-sdhNE]";
                case 28: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[rsTTPSource-sdhNE]";
                case 29: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[rsUserChannelCTPSink-rsTTPSink]";
                case 30: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[rsUserChannelCTPSource-rsTTPSource]";
                case 32: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tu11CTPSource-tug2Source]";
                case 34: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tu12CTPSource-tug2Source]";
                case 36: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tu2CTPSource-tug2Source]";
                case 38: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tu3CTPSource-tug3Source]";
                case 39: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tug2Sink-tug3Sink]";
                case 40: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tug2Source-tug3Source]";
                case 46: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vc11TTPSource-sdhNE]";
                case 48: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vc12TTPSource-sdhNE]";
                case 50: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vc2TTPSource-sdhNE]";
                case 59: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[au3CTPSinkR1-augSink]";
                case 60: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[au4CTPSinkR1-augSink]";
                case 61: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tu11CTPSinkR1-tug2Sink]";
                case 62: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tu12CTPSinkR1-tug2Sink]";
                case 63: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tu2CTPSinkR1-tug2Sink]";
                case 64: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tu3CTPSinkR1-tug3Sink]";
                case 65: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tug2Sink-vc3TTPSinkR1]";
                case 66: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tug2Source-vc3TTPSourceR1]";
                case 67: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tug3Sink-vc4TTPSinkR1]";
                case 68: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[tug3Source-vc4TTPSourceR1]";
                case 69: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vc11TTPSinkR1-sdhNE]";
                case 70: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vc12TTPSinkR1-sdhNE]";
                case 71: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vc2TTPSinkR1-sdhNE]";
                case 72: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vc3TTPSinkR1-sdhNE]";
                case 73: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vc3TTPSourceR1-sdhNE]";
                case 74: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vc4TTPSinkR1-sdhNE]";
                case 75: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vc4TTPSourceR1-sdhNE]";
                case 76: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vcnUserChannelCTPSink-vc3TTPSinkR1]";
                case 77: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vcnUserChannelCTPSource-vc3TTPSourceR1]";
                case 78: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vcnUserChannelCTPSink-vc4TTPSinkR1]";
                case 79: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/[vcnUserChannelCTPSource-vc4TTPSourceR1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Name bindings]/{values[index - 1]}";
            }

        #endregion

        // action
        #region 0.0.7.774.127.2.0.9.*

        oid_0_0_7_774_127_2_0_9:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Action types]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Action types]/[defineAUGStructure]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Action types]/[defineVC4Structure]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Action types]/[defineVC3Structure]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Action types]/[defineTug3Structure]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Action types]/[defineTug2Structure]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Action types]/[defineClientType]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) configuration of the payload structure for the network element view]/[Information model]/[Action types]/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        // prot
        #region 0.0.7.774.127.3.*

        oid_0_0_7_774_127_3:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]";
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_3_0;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.3.0.*

        oid_0_0_7_774_127_3_0:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]";
            switch (values[index++])
            {
                case 2: goto oid_0_0_7_774_127_3_0_2;
                case 3: goto oid_0_0_7_774_127_3_0_3;
                case 4: goto oid_0_0_7_774_127_3_0_4;
                case 5: goto oid_0_0_7_774_127_3_0_5;
                case 6: goto oid_0_0_7_774_127_3_0_6;
                case 7: goto oid_0_0_7_774_127_3_0_7;
                case 9: goto oid_0_0_7_774_127_3_0_9;
                case 10: goto oid_0_0_7_774_127_3_0_10;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.3.0.2.*

        oid_0_0_7_774_127_3_0_2:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[ASN.1 modules]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[ASN.1 modules]/[SDHProtASN1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[ASN.1 modules]/{values[index - 1]}";
            }

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.3.0.3.*

        oid_0_0_7_774_127_3_0_3:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/[apsReportRecord]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/[protectedTTPBidirectional]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/[protectedTTPSink]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/[protectedTTPSource]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/[protectionGroup]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/[protectionUnit]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/[sdhMSProtectionGroup]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/[sdhMSProtectionUnit]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/[unprotectedCTPBidirectional]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/[unprotectedCTPSink]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/[unprotectedCTPSource]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/[protectionGroupR1]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/[sdhMSProtectionGroupR1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Managed object classes]/{values[index - 1]}";
            }

        #endregion

        // package
        #region 0.0.7.774.127.3.0.4.*

        oid_0_0_7_774_127_3_0_4:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Packages]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Packages]/[extraTrafficControlPkg]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Packages]/[lastAttemptResultPkg]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Packages]/[protectionSwitchExercisePkg]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Packages]/[protectionMismatchStatusPkg]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Packages]/[priorityPkg]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Packages]/[sdhPriorityPkg]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Packages]/{values[index - 1]}";
            }

        #endregion

        // parameter
        #region 0.0.7.774.127.3.0.5.*

        oid_0_0_7_774_127_3_0_5:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Parameters]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Parameters]/[invokeProtectionError]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Parameters]/[releaseProtectionError]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Parameters]/[protectionStatusParameter]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Parameters]/{values[index - 1]}";
            }

        #endregion

        // nameBinding
        #region 0.0.7.774.127.3.0.6.*

        oid_0_0_7_774_127_3_0_6:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/[protectedTTPBidirectional-sdhNE]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/[protectedTTPSink-sdhNE]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/[protectedTTPSource-sdhNE]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/[protectionGroup-managedElement]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/[augBidirectional-protectedTTPBidirectional]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/[augSink-protectedTTPSink]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/[augSource-protectedTTPSource]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/[protectionUnit-protectionGroup]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/[unprotectedCTPBidirectional-msTTPBidirectional]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/[unprotectedCTPSink-msTTPSink]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/[unprotectedCTPSource-msTTPSource]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/[protectionGroupR1-managedElement]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/[protectionUnit-protectionGroupR1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Name bindings]/{values[index - 1]}";
            }

        #endregion

        // attribute
        #region 0.0.7.774.127.3.0.7.*

        oid_0_0_7_774_127_3_0_7:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[channelNumber]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[lastAttemptResult]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[priority]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[protectedTTPId]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[reportedProtectionUnit]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[protectionGroupId]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[protectionGroupType]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[protectionMismatchStatus]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[protectionStatus]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[protectionSwitchMode]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[protectionUnitId]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[protecting]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[reliableResourcePointer]";
                case 14: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[revertive]";
                case 15: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[sdhPriority]";
                case 16: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[unprotectedCTPId]";
                case 17: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[unreliableResourcePointer]";
                case 18: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[waitToRestoreTime]";
                case 19: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/[notifiedProtectionUnit]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Attribute types]/{values[index - 1]}";
            }

        #endregion

        // action
        #region 0.0.7.774.127.3.0.9.*

        oid_0_0_7_774_127_3_0_9:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Action types]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Action types]/[invokeExercise]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Action types]/[invokeProtection]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Action types]/[releaseProtection]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Action types]/{values[index - 1]}";
            }

        #endregion

        // notification
        #region 0.0.7.774.127.3.0.10.*

        oid_0_0_7_774_127_3_0_10:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Notifications]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Notifications]/[protectionSwitchReporting]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Notifications]/[protectionSwitchReportingR1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of multiplex-section protection for the network element view]/[Information model]/[Notifications]/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        // sncp
        #region 0.0.7.774.127.4.*

        oid_0_0_7_774_127_4:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]";
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_4_0;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.4.0.*

        oid_0_0_7_774_127_4_0:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]";
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_4_0_0;
                case 2: goto oid_0_0_7_774_127_4_0_2;
                case 3: goto oid_0_0_7_774_127_4_0_3;
                case 4: goto oid_0_0_7_774_127_4_0_4;
                case 5: goto oid_0_0_7_774_127_4_0_5;
                case 6: goto oid_0_0_7_774_127_4_0_6;
                case 7: goto oid_0_0_7_774_127_4_0_7;
                case 9: goto oid_0_0_7_774_127_4_0_9;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/{values[index - 1]}";
            }

        // standardSpecificExtension
        #region 0.0.7.774.127.4.0.0.*

        oid_0_0_7_774_127_4_0_0:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Standard specific extensions]";
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_4_0_0_0;
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Standard specific extensions]/[SubNetwork Connection Protection (SNCP) path trace mismatch criteria]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Standard specific extensions]/[SubNetwork Connection Protection (SNCP) excessive error criteria]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Standard specific extensions]/{values[index - 1]}";
            }

        // sncpProtectionCriteria
        #region 0.0.7.774.127.4.0.0.0.*

        oid_0_0_7_774_127_4_0_0_0:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Standard specific extensions]/[sncpProtectionCriteria]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Standard specific extensions]/[sncpProtectionCriteria]/[sncpPathTraceMismatchCriteria]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Standard specific extensions]/[sncpProtectionCriteria]/[sncpExcessiveErrorCriteria]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Standard specific extensions]/[sncpProtectionCriteria]/{values[index - 1]}";
            }

        #endregion

        #endregion

        // informationModel
        #region 0.0.7.774.127.4.0.2.*

        oid_0_0_7_774_127_4_0_2:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[ASN.1 modules]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[ASN.1 modules]/[SDHSNCPASN1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[ASN.1 modules]/{values[index - 1]}";
            }

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.4.0.3.*

        oid_0_0_7_774_127_4_0_3:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Managed object classes]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Managed object classes]/[connectionProtectionGroup]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Managed object classes]/[connectionProtection]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Managed object classes]/[mpConnectionProtection]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Managed object classes]/[sncpFabric]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Managed object classes]/[connectionProtectionGroupR1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Managed object classes]/{values[index - 1]}";
            }

        #endregion

        // package
        #region 0.0.7.774.127.4.0.4.*

        oid_0_0_7_774_127_4_0_4:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Packages]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Packages]/[holdOffTimePackage]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Packages]/{values[index - 1]}";
            }

        #endregion

        // parameter
        #region 0.0.7.774.127.4.0.5.*

        oid_0_0_7_774_127_4_0_5:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Parameters]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Parameters]/[switchStatusParameter]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Parameters]/{values[index - 1]}";
            }

        #endregion

        // nameBinding
        #region 0.0.7.774.127.4.0.6.*

        oid_0_0_7_774_127_4_0_6:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Name bindings]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Name bindings]/[connectionProtection-connectionProtectionGroup]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Name bindings]/[connectionProtectionGroup-sncpFabric]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Name bindings]/[crossConnection-mpConnectionProtection]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Name bindings]/[mpConnectionProtection-connectionProtectionGroup]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Name bindings]/[crossConnection-sncpFabric]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Name bindings]/[mpCrossConnection-sncpFabric]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Name bindings]/[connectionProtection-connectionProtectionGroupR1]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Name bindings]/[connectionProtectionGroupR1-sncpFabric]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Name bindings]/[mpConnectionProtection-connectionProtectionGroupR1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Name bindings]/{values[index - 1]}";
            }

        #endregion

        // attribute
        #region 0.0.7.774.127.4.0.7.*

        oid_0_0_7_774_127_4_0_7:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Attribute types]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Attribute types]/[holdOffTime]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Attribute types]/[protectionCriteria]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Attribute types]/[switchStatus]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Attribute types]/{values[index - 1]}";
            }

        #endregion

        // attribute
        #region 0.0.7.774.127.4.0.9.*

        oid_0_0_7_774_127_4_0_9:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Action types]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Action types]/[protectedConnect]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Action types]/[protectUnprotect]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of the subnetwork connection protection for the network element view]/[Information model]/[Action types]/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        // cs
        #region 0.0.7.774.127.5.*

        oid_0_0_7_774_127_5:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]";
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_5_0;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.5.0.*

        oid_0_0_7_774_127_5_0:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]";
            switch (values[index++])
            {
                case 2: goto oid_0_0_7_774_127_5_0_2;
                case 3: goto oid_0_0_7_774_127_5_0_3;
                case 4: goto oid_0_0_7_774_127_5_0_4;
                case 6: goto oid_0_0_7_774_127_5_0_6;
                case 7: goto oid_0_0_7_774_127_5_0_7;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.5.0.2.*

        oid_0_0_7_774_127_5_0_2:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[ASN.1 modules]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[ASN.1 modules]/[SDHCSASN1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[ASN.1 modules]/{values[index - 1]}";
            }

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.5.0.3.*

        oid_0_0_7_774_127_5_0_3:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]";
            switch (values[index++])
            {
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[au4SupervisedCTPSource]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[au3SupervisedCTPSource]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[tu3SupervisedCTPSource]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[tu2SupervisedCTPSource]";
                case 15: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[tu12SupervisedCTPSource]";
                case 18: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[tu11SupervisedCTPSource]";
                case 19: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[au4SupervisedCTPBidirectionalR1]";
                case 20: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[au4SupervisedCTPSinkR1]";
                case 21: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[au3SupervisedCTPBidirectionalR1]";
                case 22: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[au3SupervisedCTPSinkR1]";
                case 23: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[tu3SupervisedCTPBidirectionalR1]";
                case 24: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[tu3SupervisedCTPSinkR1]";
                case 25: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[tu2SupervisedCTPBidirectionalR1]";
                case 26: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[tu2SupervisedCTPSinkR1]";
                case 27: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[tu12SupervisedCTPBidirectionalR1]";
                case 28: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[tu12SupervisedCTPSinkR1]";
                case 29: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[tu11SupervisedCTPBidirectionalR1]";
                case 30: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/[tu11SupervisedCTPSinkR1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Managed object classes]/{values[index - 1]}";
            }

        #endregion

        // package
        #region 0.0.7.774.127.5.0.4.*

        oid_0_0_7_774_127_5_0_4:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Packages]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Packages]/[vc11-2SupervisionBidirectionalPackage]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Packages]/[vc11-2SupervisionSourcePackage]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Packages]/[vc3-4SupervisionBidirectionalPackage]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Packages]/[vc3-4SupervisionSourcePackage]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Packages]/[vc11-2SupervisionSinkPackageR1]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Packages]/[vc3-4SupervisionSinkPackageR1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Packages]/{values[index - 1]}";
            }

        #endregion

        // nameBinding
        #region 0.0.7.774.127.5.0.6.*

        oid_0_0_7_774_127_5_0_6:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Name bindings]";
            switch (values[index++])
            {
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentData-au4SupervisedCTPSinkR1]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentData-au3SupervisedCTPSinkR1]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentData-tu3SupervisedCTPSinkR1]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentData-tu2SupervisedCTPSinkR1]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentData-tu12SupervisedCTPSinkR1]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentData-tu11SupervisedCTPSinkR1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Name bindings]/{values[index - 1]}";
            }

        #endregion

        // attribute
        #region 0.0.7.774.127.5.0.7.*

        oid_0_0_7_774_127_5_0_7:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Attribute types]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Attribute types]/[generatorEnabled]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Attribute types]/[monitorActive]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Attribute types]/[j1PathTraceReceive]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Attribute types]/[j1PathTraceSend]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Attribute types]/[j2PathTraceExpected]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Attribute types]/[j2PathTraceReceive]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Attribute types]/[j2PathTraceSend]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) management of connection supervision functionality (HCS/LCS) for the network element view]/[Information model]/[Attribute types]/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        // pmUni
        #region 0.0.7.774.127.6.*

        oid_0_0_7_774_127_6:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]";
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_6_0;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.6.0.*

        oid_0_0_7_774_127_6_0:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]";
            switch (values[index++])
            {
                case 2: goto oid_0_0_7_774_127_6_0_2;
                case 3: goto oid_0_0_7_774_127_6_0_3;
                case 4: goto oid_0_0_7_774_127_6_0_4;
                case 6: goto oid_0_0_7_774_127_6_0_6;
                case 7: goto oid_0_0_7_774_127_6_0_7;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.6.0.2.*

        oid_0_0_7_774_127_6_0_2:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[ASN.1 modules]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[ASN.1 modules]/[SDHPMUNIASN1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[ASN.1 modules]/{values[index - 1]}";
            }

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.6.0.3.*

        oid_0_0_7_774_127_6_0_3:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/[sdhCurrentDataUnidirectional]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/[msCurrentDataNearEnd]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/[msCurrentDataNearEndTR]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/[pathTerminationCurrentDataNearEnd]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/[pathTerminationCurrentDataNearEndTR]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/[msCurrentDataFarEnd]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/[msCurrentDataFarEndTR]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/[pathTerminationCurrentDataFarEnd]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/[pathTerminationCurrentDataFarEndTR]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/[msHistoryDataNearEnd]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/[pathTerminationHistoryDataNearEnd]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/[msHistoryDataFarEnd]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/[pathTerminationHistoryDataFarEnd]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Managed object classes]/{values[index - 1]}";
            }

        #endregion

        // package
        #region 0.0.7.774.127.6.0.4.*

        oid_0_0_7_774_127_6_0_4:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[nearEndUASCurrentDataPackage]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[farEndUASCurrentDataPackage]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[nearEndUASHistoryDataPackage]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[farEndUASHistoryDataPackage]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[failureCountsNearEndPackage]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[eSANearEndPackage]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[eSBNearEndPackage]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[failureCountsFarEndPackage]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[eSAFarEndPackage]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[eSBFarEndPackage]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[failureCountsNearEndHistoryDataPackage]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[eSANearEndHistoryDataPackage]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[eSBNearEndHistoryDataPackage]";
                case 14: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[failureCountsFarEndHistoryDataPackage]";
                case 15: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[eSAFarEndHistoryDataPackage]";
                case 16: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/[eSBFarEndHistoryDataPackage]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Packages]/{values[index - 1]}";
            }

        #endregion

        // nameBinding
        #region 0.0.7.774.127.6.0.6.*

        oid_0_0_7_774_127_6_0_6:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[msCurrentDataNearEnd-msTTPSink]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[msCurrentDataNearEndTR-msTTPSink]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[msCurrentDataFarEnd-msTTPSink]";
                case 14: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[msCurrentDataFarEndTR-msTTPSink]";
                case 49: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEnd-vc4TTPSinkR1]";
                case 50: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEnd-vc3TTPSinkR1]";
                case 51: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEnd-vc2TTPSinkR1]";
                case 52: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEnd-vc12TTPSinkR1]";
                case 53: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEnd-vc11TTPSinkR1]";
                case 54: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEndTR-vc4TTPSinkR1]";
                case 55: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEndTR-vc3TTPSinkR1]";
                case 56: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEndTR-vc2TTPSinkR1]";
                case 57: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEndTR-vc12TTPSinkR1]";
                case 58: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEndTR-vc11TTPSinkR1]";
                case 59: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEnd-vc4TTPSinkR1]";
                case 60: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEnd-vc3TTPSinkR1]";
                case 61: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEnd-vc2TTPSinkR1]";
                case 62: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEnd-vc12TTPSinkR1]";
                case 63: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEnd-vc11TTPSinkR1]";
                case 64: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEndTR-vc4TTPSinkR1]";
                case 65: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEndTR-vc3TTPSinkR1]";
                case 66: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEndTR-vc2TTPSinkR1]";
                case 67: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEndTR-vc12TTPSinkR1]";
                case 68: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEndTR-vc11TTPSinkR1]";
                case 69: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEnd-au4SupervisedCTPSinkR1]";
                case 70: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEnd-au3SupervisedCTPSinkR1]";
                case 71: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEnd-tu3SupervisedCTPSinkR1]";
                case 72: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEnd-tu2SupervisedCTPSinkR1]";
                case 73: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEnd-tu12SupervisedCTPSinkR1]";
                case 74: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEnd-tu11SupervisedCTPSinkR1]";
                case 75: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEnd-au4SupervisedCTPSinkR1]";
                case 76: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEnd-au3SupervisedCTPSinkR1]";
                case 77: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEnd-tu3SupervisedCTPSinkR1]";
                case 78: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEnd-tu2SupervisedCTPSinkR1]";
                case 79: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEnd-tu12SupervisedCTPSinkR1]";
                case 80: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEnd-tu11SupervisedCTPSinkR1]";
                case 81: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEndTR-au4SupervisedCTPSinkR1]";
                case 82: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEndTR-au3SupervisedCTPSinkR1]";
                case 83: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEndTR-tu3SupervisedCTPSinkR1]";
                case 84: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEndTR-tu2SupervisedCTPSinkR1]";
                case 85: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEndTR-tu12SupervisedCTPSinkR1]";
                case 86: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataNearEndTR-tu11SupervisedCTPSinkR1]";
                case 87: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEndTR-au4SupervisedCTPSinkR1]";
                case 88: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEndTR-au3SupervisedCTPSinkR1]";
                case 89: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEndTR-tu3SupervisedCTPSinkR1]";
                case 90: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEndTR-tu2SupervisedCTPSinkR1]";
                case 91: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEndTR-tu12SupervisedCTPSinkR1]";
                case 92: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[pathTerminationCurrentDataFarEndTR-tu11SupervisedCTPSinkR1]";
                case 93: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[msAdaptationCurrentData-au4CTPSource]";
                case 94: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/[msAdaptationCurrentData-au3CTPSource]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Name bindings]/{values[index - 1]}";
            }

        #endregion

        // attribute
        #region 0.0.7.774.127.6.0.7.*

        oid_0_0_7_774_127_6_0_7:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Attribute types]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Attribute types]/[nEUAS]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Attribute types]/[fEUAS]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Attribute types]/[fCNearEnd]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Attribute types]/[eSANearEnd]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Attribute types]/[eSBNearEnd]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Attribute types]/[fCFarEnd]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Attribute types]/[eSAFarEnd]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Attribute types]/[eSBFarEnd]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Unidirectional performance monitoring for the network element view]/[Information model]/[Attribute types]/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        // ptl
        #region 0.0.7.774.127.7.*

        oid_0_0_7_774_127_7:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]";
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_7_0;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.7.0.*

        oid_0_0_7_774_127_7_0:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]";
            switch (values[index++])
            {
                case 2: goto oid_0_0_7_774_127_7_0_2;
                case 3: goto oid_0_0_7_774_127_7_0_3;
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Packages]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Parameters]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Name bindings]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Attribute types]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Action types]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[ANotifications]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/{values[index - 1]}";
            }

        // asn1Module
        #region 0.0.7.774.127.7.0.2.*

        oid_0_0_7_774_127_7_0_2:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[ASN.1 modules]";
            switch (values[index++])
            {
                case 0: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[ASN.1 modules]/[SDHPTLASN1]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[ASN.1 modules]/{values[index - 1]}";
            }

        #endregion

        // managedObjectClass
        #region 0.0.7.774.127.7.0.3.*

        oid_0_0_7_774_127_7_0_3:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]";
            switch (values[index++])
            {
                case 1: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[labelledElectricalSPITTPBidirectional]";
                case 2: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[labelledElectricalSPITTPSink]";
                case 3: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[labelledElectricalSPITTPSource]";
                case 4: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[labelledOpticalSPITTPBidirectional]";
                case 5: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[labelledOpticalSPITTPSink]";
                case 6: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[labelledOpticalSPITTPSource]";
                case 7: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[vc11PathTraceTTPBidirectional]";
                case 8: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[vc11PathTraceTTPSink]";
                case 9: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[vc11PathTraceTTPSource]";
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[vc12PathTraceTTPBidirectional]";
                case 11: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[vc12PathTraceTTPSink]";
                case 12: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[vc12PathTraceTTPSource]";
                case 13: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[vc2PathTraceTTPBidirectional]";
                case 14: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[vc2PathTraceTTPSink]";
                case 15: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[vc2PathTraceTTPSource]";
                case 16: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[modifiableVC2PathTraceTTPBidirectional]";
                case 17: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[modifiableVC2PathTraceTTPSink]";
                case 18: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[modifiableVC2PathTraceTTPSource]";
                case 19: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[modifiableVC12PathTraceTTPBidirectional]";
                case 20: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[modifiableVC12PathTraceTTPSink]";
                case 21: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[modifiableVC12PathTraceTTPSource]";
                case 22: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[modifiableVC11PathTraceTTPBidirectional]";
                case 23: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[modifiableVC11PathTraceTTPSink]";
                case 24: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/[modifiableVC11PathTraceTTPSource]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of lower order path trace and interface labelling for the network element view]/[Information model]/[Managed object classes]/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        // sdhRadioIM
        #region 0.0.7.774.127.8.*

        oid_0_0_7_774_127_8:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of radio-relay systems for the network element view]";
            switch (values[index++])
            {
                case 0: goto oid_0_0_7_774_127_8_0;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of radio-relay systems for the network element view]/{values[index - 1]}";
            }

        // informationModel
        #region 0.0.7.774.127.8.0.*

        oid_0_0_7_774_127_8_0:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of radio-relay systems for the network element view]/[Information model]";
            switch (values[index++])
            {
                //TODO: case 2: goto oid_0_0_7_774_127_8_0_2;
                //TODO: case 3: goto oid_0_0_7_774_127_8_0_3;
                //TODO: case 4: goto oid_0_0_7_774_127_8_0_4;
                //TODO: case 5: goto oid_0_0_7_774_127_8_0_5;
                //TODO: case 6: goto oid_0_0_7_774_127_8_0_6;
                //TODO: case 7: goto oid_0_0_7_774_127_8_0_7;
                //TODO: case 9: goto oid_0_0_7_774_127_8_0_9;
                case 10: return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of radio-relay systems for the network element view]/[Information model]/[Notifications]";
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Management of radio-relay systems for the network element view]/[Information model]/{values[index - 1]}";
            }

        #endregion

        #endregion

        // protCoord
        #region 0.0.7.774.127.9.*

        oid_0_0_7_774_127_9:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Configuration of linear multiplex-section protection for the network element view]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_0_0_7_774_127_9_0;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) - Configuration of linear multiplex-section protection for the network element view]/{values[index - 1]}";
            }

        #endregion

        // msspr
        #region 0.0.7.774.127.10.*

        oid_0_0_7_774_127_10:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) Multiplex Section (MS) shared protection ring management for the network element view]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_0_0_7_774_127_10_0;
                default: return $"/ITU-T/Recommendation/G/[Synchronous Digital Hierarchy (SDH)]/[Parts of Recommendation ITU-T G.774]/[Synchronous Digital Hierarchy (SDH) Multiplex Section (MS) shared protection ring management for the network element view]/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        // g722-1, g7221
        #region 0.0.7.7221.*

        oid_0_0_7_7221:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Coding at 24 and 32 kbit/s for hands-free operation in systems with low frame loss]";
            switch (values[index++])
            {
                //TODO: case 1: goto oid_0_0_7_7221_1;
                default: return $"/ITU-T/Recommendation/G/[Coding at 24 and 32 kbit/s for hands-free operation in systems with low frame loss]/{values[index - 1]}";
            }

        #endregion

        // 7222
        #region 0.0.7.7222.*

        oid_0_0_7_7222:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Wideband coding of speech at around 16 kbit/s using Adaptive Multi-Rate Wideband (AMR-WB)]";
            switch (values[index++])
            {
                //TODO: case 1: goto oid_0_0_7_7222_1;
                default: return $"/ITU-T/Recommendation/G/[Wideband coding of speech at around 16 kbit/s using Adaptive Multi-Rate Wideband (AMR-WB)]/{values[index - 1]}";
            }

        #endregion

        // g776-1, g7761
        #region 0.0.7.7761.*

        oid_0_0_7_7761:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[Managed objects for signal processing network elements]";
            switch (values[index++])
            {
                //TODO: case 7: goto oid_0_0_7_7761_7;
                default: return $"/ITU-T/Recommendation/G/[Managed objects for signal processing network elements]/{values[index - 1]}";
            }

        #endregion

        // gntm
        #region 0.0.7.85501.*

        oid_0_0_7_85501:

            if (index == values.Length) return "/ITU-T/Recommendation/G/[GDMO engineering viewpoint for the generic network level model]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_0_0_7_85501_0;
                default: return $"/ITU-T/Recommendation/G/[GDMO engineering viewpoint for the generic network level model]/{values[index - 1]}";
            }

        #endregion

        #endregion

        // h
        #region 0.0.8.*

        oid_0_0_8:

            if (index == values.Length) return "/ITU-T/Recommendation/H";
            switch (values[index++])
            {
                //TODO: case 224: goto oid_0_0_8_224;
                //TODO: case 230: goto oid_0_0_8_230;
                //TODO: case 235: goto oid_0_0_8_235;
                //TODO: case 239: goto oid_0_0_8_239;
                //TODO: case 241: goto oid_0_0_8_241;
                //TODO: case 245: goto oid_0_0_8_245;
                //TODO: case 248: goto oid_0_0_8_248;
                //TODO: case 249: goto oid_0_0_8_249;
                //TODO: case 263: goto oid_0_0_8_263;
                //TODO: case 282: goto oid_0_0_8_282;
                //TODO: case 283: goto oid_0_0_8_283;
                //TODO: case 323: goto oid_0_0_8_323;
                //TODO: case 324: goto oid_0_0_8_324;
                //TODO: case 341: goto oid_0_0_8_341;
                //TODO: case 350: goto oid_0_0_8_350;
                //TODO: case 450: goto oid_0_0_8_450;
                //TODO: case 460: goto oid_0_0_8_460;
                //TODO: case 641: goto oid_0_0_8_641;
                //TODO: case 2250: goto oid_0_0_8_2250;
                default: return $"/ITU-T/Recommendation/H/{values[index - 1]}";
            }

        #endregion

        // i
        #region 0.0.9.*

        oid_0_0_9:

            if (index == values.Length) return "/ITU-T/Recommendation/I";
            switch (values[index++])
            {
                //TODO: case 751: goto oid_0_0_9_751;
                default: return $"/ITU-T/Recommendation/I/{values[index - 1]}";
            }

        #endregion

        // m
        #region 0.0.13.*

        oid_0_0_13:

            if (index == values.Length) return "/ITU-T/Recommendation/M";
            switch (values[index++])
            {
                //TODO: case 3100: goto oid_0_0_13_3100;
                //TODO: case 3108: goto oid_0_0_13_3108;
                //TODO: case 3611: goto oid_0_0_13_3611;
                //TODO: case 3640: goto oid_0_0_13_3640;
                //TODO: case 3641: goto oid_0_0_13_3641;
                //TODO: case 3650: goto oid_0_0_13_3650;
                default: return $"/ITU-T/Recommendation/M/{values[index - 1]}";
            }

        #endregion

        // q
        #region 0.0.17.*

        oid_0_0_17:

            if (index == values.Length) return "/ITU-T/Recommendation/Q";
            switch (values[index++])
            {
                //TODO: case 733: goto oid_0_0_17_733;
                //TODO: case 736: goto oid_0_0_17_736;
                //TODO: case 751: goto oid_0_0_17_751;
                //TODO: case 753: goto oid_0_0_17_753;
                //TODO: case 754: goto oid_0_0_17_754;
                //TODO: case 755: goto oid_0_0_17_755;
                //TODO: case 763: goto oid_0_0_17_763;
                //TODO: case 765: goto oid_0_0_17_765;
                //TODO: case 773: goto oid_0_0_17_773;
                //TODO: case 775: goto oid_0_0_17_775;
                //TODO: case 813: goto oid_0_0_17_813;
                //TODO: case 814: goto oid_0_0_17_814;
                //TODO: case 815: goto oid_0_0_17_815;
                //TODO: case 821: goto oid_0_0_17_821;
                //TODO: case 822: goto oid_0_0_17_822;
                //TODO: case 823: goto oid_0_0_17_823;
                //TODO: case 824: goto oid_0_0_17_824;
                //TODO: case 825: goto oid_0_0_17_825;
                //TODO: case 826: goto oid_0_0_17_826;
                //TODO: case 831: goto oid_0_0_17_831;
                //TODO: case 832: goto oid_0_0_17_832;
                //TODO: case 835: goto oid_0_0_17_835;
                //TODO: case 836: goto oid_0_0_17_836;
                //TODO: case 860: goto oid_0_0_17_860;
                //TODO: case 932: goto oid_0_0_17_932;
                //TODO: case 941: goto oid_0_0_17_941;
                //TODO: case 950: goto oid_0_0_17_950;
                //TODO: case 951: goto oid_0_0_17_951;
                //TODO: case 952: goto oid_0_0_17_952;
                //TODO: case 953: goto oid_0_0_17_953;
                //TODO: case 954: goto oid_0_0_17_954;
                //TODO: case 955: goto oid_0_0_17_955;
                //TODO: case 956: goto oid_0_0_17_956;
                //TODO: case 957: goto oid_0_0_17_957;
                //TODO: case 1218: goto oid_0_0_17_1218;
                //TODO: case 1228: goto oid_0_0_17_1228;
                //TODO: case 1238: goto oid_0_0_17_1238;
                //TODO: case 1248: goto oid_0_0_17_1248;
                //TODO: case 1400: goto oid_0_0_17_1400;
                //TODO: case 1551: goto oid_0_0_17_1551;
                //TODO: case 1831: goto oid_0_0_17_1831;
                //TODO: case 2724: goto oid_0_0_17_2724;
                //TODO: case 2751: goto oid_0_0_17_2751;
                //TODO: case 2932: goto oid_0_0_17_2932;
                //TODO: case 2964: goto oid_0_0_17_2964;
                //TODO: case 2981: goto oid_0_0_17_2981;
                //TODO: case 2984: goto oid_0_0_17_2984;
                //TODO: case 3303: goto oid_0_0_17_3303;
                //TODO: case 3304: goto oid_0_0_17_3304;
                //TODO: case 3308: goto oid_0_0_17_3308;
                case 8361: return "/ITU-T/Recommendation/Q/[Specifications of Signalling System No. 7 -- Q3 interface]";
                default: return $"/ITU-T/Recommendation/Q/{values[index - 1]}";
            }

        #endregion

        // t
        #region 0.0.20.*

        oid_0_0_20:

            if (index == values.Length) return "/ITU-T/Recommendation/T";
            switch (values[index++])
            {
                //TODO: case 43: goto oid_0_0_20_43;
                //TODO: case 123: goto oid_0_0_20_123;
                //TODO: case 124: goto oid_0_0_20_124;
                //TODO: case 126: goto oid_0_0_20_126;
                //TODO: case 127: goto oid_0_0_20_127;
                //TODO: case 128: goto oid_0_0_20_128;
                //TODO: case 134: goto oid_0_0_20_134;
                //TODO: case 135: goto oid_0_0_20_135;
                //TODO: case 136: goto oid_0_0_20_136;
                //TODO: case 137: goto oid_0_0_20_137;
                case 330: return "/ITU-T/Recommendation/T/[TLMAAbsService]";
                //TODO: case 433: goto oid_0_0_20_433;
                //TODO: case 434: goto oid_0_0_20_434;
                //TODO: case 435: goto oid_0_0_20_435;
                //TODO: case 436: goto oid_0_0_20_436;
                default: return $"/ITU-T/Recommendation/T/{values[index - 1]}";
            }

        #endregion

        // v
        #region 0.0.22.*

        oid_0_0_22:

            if (index == values.Length) return "/ITU-T/Recommendation/V";
            switch (values[index++])
            {
                //TODO: case 43: goto oid_0_0_22_150;
                default: return $"/ITU-T/Recommendation/V/{values[index - 1]}";
            }

        #endregion

        // x
        #region 0.0.24.*

        oid_0_0_24:

            if (index == values.Length) return "/ITU-T/Recommendation/X";
            switch (values[index++])
            {
                //TODO: case 162: goto oid_0_0_24_162;
                //TODO: case 754: goto oid_0_0_24_754;
                //TODO: case 790: goto oid_0_0_24_790;
                //TODO: case 792: goto oid_0_0_24_792;
                //TODO: case 894: goto oid_0_0_24_894;
                //TODO: case 1084: goto oid_0_0_24_1084;
                //TODO: case 1089: goto oid_0_0_24_1089;
                //TODO: case 1125: goto oid_0_0_24_1125;
                //TODO: case 1243: goto oid_0_0_24_1243;
                //TODO: case 1303: goto oid_0_0_24_1303;
                default: return $"/ITU-T/Recommendation/X/{values[index - 1]}";
            }

        #endregion

        #endregion

        // administration
        #region 0.2.*

        oid_0_2:

            if (index == values.Length) return "/ITU-T/Administration";
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
                //TODO: case 228: goto oid_0_2_228;
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
                //TODO: case 242: goto oid_0_2_242;
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
                //TODO: case 262: goto oid_0_2_262;
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
                //TODO: case 440: goto oid_0_2_440;
                case 441:
                case 442:
                case 443: return "/ITU-T/Administration/[Japan]";
                //TODO: case 450: goto oid_0_2_450;
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
                default: return $"/ITU-T/Administration/{values[index - 1]}";
            }

        #endregion

        // network-operator
        #region 0.3.*

        oid_0_3:

            if (index == values.Length) return "/ITU-T/Network-Operator";
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
                case 4001: return "/ITU-T/Network-Operator/[Azerbaijan: AZPAK (AZerbaijan public PAcKet switched data network)]";
                case 4002: return "/ITU-T/Network-Operator/[Azerbaijan: \"AzEuroTel\" Joint Venture]";
                case 4010: return "/ITU-T/Network-Operator/[Kazakhstan: KazNet X.25]";
                case 4011: return "/ITU-T/Network-Operator/[Kazakhstan: BankNet X.25]";
                case 4041: return "/ITU-T/Network-Operator/[India: \"RABMN\"]";
                case 4042: return "/ITU-T/Network-Operator/[India: International Gateway Packet Switching System (GPSS)]";
                case 4043: return "/ITU-T/Network-Operator/[India: \"INET\" (Packet Switched Public Data Network)]";
                case 4045: return "/ITU-T/Network-Operator/[India: HVnet]";
                case 4046: return "/ITU-T/Network-Operator/[India: Shared Data Network Identification Code (DNIC) for \"VSAT\" based private data networks]";
                case 4101: return "/ITU-T/Network-Operator/[Pakistan: TRANSLINK]";
                case 4132: return "/ITU-T/Network-Operator/[Sri Lanka: Lanka Communication Services (Pvt) Limited]";
                case 4133: return "/ITU-T/Network-Operator/[Sri Lanka: Electroteks (Pvt) Limited]";
                case 4141: return "/ITU-T/Network-Operator/[Myanmar: MYANMARP]";
                case 4155: return "/ITU-T/Network-Operator/[Lebanon: Reseau public de transmission de donnees par paquets]";
                case 4195: return "/ITU-T/Network-Operator/[Kuwait: Qualitynet]";
                case 4201: return "/ITU-T/Network-Operator/[Saudi Arabia: ALWASEET - Public Packet Switched Data Network]";
                case 4241: return "/ITU-T/Network-Operator/[United Arab Emirates: \"EMDAN\" Teletex Network]";
                case 4243: return "/ITU-T/Network-Operator/[United Arab Emirates: \"EMDAN\" X.25 and X.28 Terminals]";
                case 4251: return "/ITU-T/Network-Operator/[Israel: ISRANET]";
                case 4260: return "/ITU-T/Network-Operator/[Bahrain: Batelco Global System for Mobile communications (GSM) Service]";
                case 4262: return "/ITU-T/Network-Operator/[Bahrain: Bahrain MAnaged DAta Network (MADAN)]";
                case 4263: return "/ITU-T/Network-Operator/[Bahrain: Batelco Packet Switched Node]";
                case 4271: return "/ITU-T/Network-Operator/[Qatar: \"DOHPAK\"]";
                case 4290: return "/ITU-T/Network-Operator/[Nepal: NEPal PAcKet switched public data network (NEPPAK)]";
                case 4321: return "/ITU-T/Network-Operator/[Islamic Republic of Iran: IranPac]";
                case 4341: return "/ITU-T/Network-Operator/[Uzbekistan: UzPAK]";
                case 4400: return "/ITU-T/Network-Operator/[Japan: GLOBALNET (Network of the Global \"VAN\" Japan Incorporation)]";
                //TODO: case 4401: goto oid_0_3_4401;
                case 4402: return "/ITU-T/Network-Operator/[Japan: NEC-NET (NEC Corporation)]";
                case 4403: return "/ITU-T/Network-Operator/[Japan: \"JENSNET\" (\"JENS Corporation\")]";
                case 4404: return "/ITU-T/Network-Operator/[Japan: JAIS-NET (Japan Research Institute Ltd.)]";
                case 4405: return "/ITU-T/Network-Operator/[Japan: NCC-VAN (NRI Co., Ltd.)]";
                case 4406: return "/ITU-T/Network-Operator/[Japan: TYMNET-Japan (Japan TELECOM COMMUNICATIONS SERVICES CO., LTD.)]";
                case 4407: return "/ITU-T/Network-Operator/[Japan: International High Speed Switched Data Transmission Network (\"KDDI\") (code canceled)]";
                case 4408: return "/ITU-T/Network-Operator/[Japan: International Packet Switched Data Transmission Network (\"KDDI\") (code canceled)]";
                case 4412: return "/ITU-T/Network-Operator/[Japan: Sprintnet (Global One Communications, INC.)]";
                case 4413: return "/ITU-T/Network-Operator/[Japan: \"KYODO NET\" (\"UNITED NET\" Corp)]";
                case 4415: return "/ITU-T/Network-Operator/[Japan: \"FENICS\" (Fujitsu Limited)]";
                case 4416: return "/ITU-T/Network-Operator/[Japan: \"HINET\" (Hitachi Information Network, Ltd.)]";
                case 4417: return "/ITU-T/Network-Operator/[Japan: TIS-Net (TOYO Information Systems Co., Ltd.)]";
                case 4418: return "/ITU-T/Network-Operator/[Japan: TG-VAN (TOSHIBA Corporation)]";
                case 4420: return "/ITU-T/Network-Operator/[Japan: Pana-Net (Matsushita Electric Industrial Co. Ltd.)]";
                case 4421: return "/ITU-T/Network-Operator/[Japan: \"DDX-P\" (Nippon Telegraph and Telephone (NTT) Communications Corporation) (code canceled)]";
                case 4422: return "/ITU-T/Network-Operator/[Japan: CTC-P (CHUBU TELECOMMUNICATIONS CO., INC.)]";
                case 4423: return "/ITU-T/Network-Operator/[Japan: \"JENSNET\" (\"JENS Corporation\")]";
                case 4424: return "/ITU-T/Network-Operator/[Japan: \"SITA\" Network]";
                case 4425: return "/ITU-T/Network-Operator/[Japan: Global Managed Data Service (Cable & Wireless IDC-Si)]";
                case 4426: return "/ITU-T/Network-Operator/[Japan: \"ECHO-NET\" (Hitachi Information Systems Ltd.)]";
                case 4427: return "/ITU-T/Network-Operator/[Japan: U-net (Nihon Unysys Information Systems Ltd.)]";
                case 4500: return "/ITU-T/Network-Operator/[Republic of Korea: HiNET-P (Korea Telecom)]";
                case 4501: return "/ITU-T/Network-Operator/[Republic of Korea: DACOM-NET]";
                case 4502: return "/ITU-T/Network-Operator/[Republic of Korea: \"CSDN\" (only assigned to Teletex)]";
                case 4538: return "/ITU-T/Network-Operator/[Hong Kong, China: Cable & Wireless Regional Businesses (Hong Kong) Limited]";
                case 4540: return "/ITU-T/Network-Operator/[Hong Kong, China: Public Switched Document Transfer Service]";
                case 4541: return "/ITU-T/Network-Operator/[Hong Kong, China: Hutchison Global Crossing Limited]";
                case 4542: return "/ITU-T/Network-Operator/[Hong Kong, China: INTELPAK (code canceled)]";
                case 4543: return "/ITU-T/Network-Operator/[Hong Kong, China: New T&T]";
                case 4545: return "/ITU-T/Network-Operator/[Hong Kong, China: Datapak]";
                case 4546: return "/ITU-T/Network-Operator/[Hong Kong, China: iAsiaWorks (Hong Kong) Service]";
                case 4547: return "/ITU-T/Network-Operator/[Hong Kong, China: New World Telephone Limited]";
                case 4548: return "/ITU-T/Network-Operator/[Hong Kong, China: \"KDD\" Telecomet Hong Kong Ltd.]";
                case 4550: return "/ITU-T/Network-Operator/[Macau: \"MACAUPAC\"]";
                case 4601: return "/ITU-T/Network-Operator/[China: Teletex and low speed data network]";
                case 4603: return "/ITU-T/Network-Operator/[China: \"CHINAPAC\"]";
                case 4604: return "/ITU-T/Network-Operator/[China: Reserved for public mobile data service]";
                case 4605: return "/ITU-T/Network-Operator/[China: Public data network]";
                case 4606:
                case 4607:
                case 4608: return "/ITU-T/Network-Operator/[China: Dedicated network]";
                case 4609: return "/ITU-T/Network-Operator/[China: China Railcom \"PAC\"]";
                case 4720: return "/ITU-T/Network-Operator/[Maldives: DATANET (Maldives Packet Switching Service)]";
                case 5020: return "/ITU-T/Network-Operator/[Malaysia: \"COINS\" Global Frame Relay]";
                case 5021: return "/ITU-T/Network-Operator/[Malaysia: Malaysian Public Packet Switched Public Data Network (\"MAYPAC\")]";
                case 5023: return "/ITU-T/Network-Operator/[Malaysia: Corporate Information Networks]";
                case 5024: return "/ITU-T/Network-Operator/[Malaysia: ACASIA-ASEAN Managed Overlay Network]";
                case 5026: return "/ITU-T/Network-Operator/[Malaysia: Mutiara Frame Relay Network]";
                case 5027: return "/ITU-T/Network-Operator/[Malaysia: Mobile Public Data Network (WAVENET)]";
                case 5028: return "/ITU-T/Network-Operator/[Malaysia: Global Management Data Services (GMDS)]";
                case 5052: return "/ITU-T/Network-Operator/[Australia: Telstra Corporation Ltd. - AUSTPAC packet switching network]";
                case 5053: return "/ITU-T/Network-Operator/[Australia: Telstra Corporation Ltd. - AUSTPAC International]";
                case 5057: return "/ITU-T/Network-Operator/[Australia: Australian Private Networks]";
                case 5101: return "/ITU-T/Network-Operator/[Indonesia: Sambungan Komunikasi Data Paket (SKDP) Packet Switched Service]";
                case 5151: return "/ITU-T/Network-Operator/[Philippines: \"CWI DATANET\" - Capitol Wireless, Inc. (CAPWIRE)]";
                case 5152: return "/ITU-T/Network-Operator/[Philippines: Philippine Global Communications, Inc. (PHILCOM)]";
                case 5154: return "/ITU-T/Network-Operator/[Philippines: Globe-Mackay Cable and Radio corp. (GMCR)]";
                case 5156: return "/ITU-T/Network-Operator/[Philippines: Eastern Telecommunications Philippines, Inc. (ETPI)]";
                case 5157: return "/ITU-T/Network-Operator/[Philippines: DATAPAC]";
                case 5202: return "/ITU-T/Network-Operator/[Thailand: THAIPAK 2 - Value Added Public Packet Switched Data Network]";
                case 5203: return "/ITU-T/Network-Operator/[Thailand: \"CAT\" Store and Forward Fax Network]";
                case 5209: return "/ITU-T/Network-Operator/[Thailand: \"TOT\" Integrated Services Digital Network (ISDN)]";
                case 5250: return "/ITU-T/Network-Operator/[Singapore: International telephone prefix]";
                case 5251: return "/ITU-T/Network-Operator/[Singapore: Inmarsat service]";
                case 5252: return "/ITU-T/Network-Operator/[Singapore: TELEPAC (Public Packet Switching Data Network)]";
                case 5253: return "/ITU-T/Network-Operator/[Singapore: High speed data/long packet service]";
                case 5254:
                case 5255: return "/ITU-T/Network-Operator/[Singapore: Public Data Network]";
                case 5257: return "/ITU-T/Network-Operator/[Singapore: Integrated Services Digital Network (ISDN) packet switching service]";
                case 5258: return "/ITU-T/Network-Operator/[Singapore: Telex]";
                case 5259: return "/ITU-T/Network-Operator/[Singapore: Public Switched Telephone Network (PSTN) access (dial-in/out)]";
                case 5301: return "/ITU-T/Network-Operator/[New Zealand: \"PACNET\" Packet Switching Network]";
                case 5351: return "/ITU-T/Network-Operator/[Guam: The Pacific Connection, Inc. - Pacnet Public Packet Switching Service]";
                case 5390: return "/ITU-T/Network-Operator/[Tonga: TONGAPAK]";
                case 5400: return "/ITU-T/Network-Operator/[Solomon Islands: Datanet]";
                case 5410: return "/ITU-T/Network-Operator/[Vanuatu: Vanuatu International Access for PACkets (VIAPAC)]";
                case 5420: return "/ITU-T/Network-Operator/[Fiji: \"FIJPAK\"]";
                case 5421: return "/ITU-T/Network-Operator/[Fiji: FIJINET]";
                case 5460: return "/ITU-T/Network-Operator/[New Caledonia: Transpac - Nouvelle Calédonie et opérateur public local]";
                case 5470: return "/ITU-T/Network-Operator/[French Polynesia: Transpac - Polynésie et opérateur public local]";
                case 5501: return "/ITU-T/Network-Operator/[Micronesia: \"FSMTC\" Packet Switched Network]";
                case 6026: return "/ITU-T/Network-Operator/[Egypt: \"EGYPTNET\"]";
                case 6030: return "/ITU-T/Network-Operator/[Algeria: \"DZ PAC\" (Réseau public de données à commutation par paquets)]";
                case 6041: return "/ITU-T/Network-Operator/[Morocco: MAGHRIPAC]";
                case 6042: return "/ITU-T/Network-Operator/[Morocco: MAGHRIPAC X.32]";
                case 6049: return "/ITU-T/Network-Operator/[Morocco: MAGHRIPAC \"RTC PAD\"]";
                case 6070: return "/ITU-T/Network-Operator/[Gambia: \"GAMNET\"]";
                case 6081: return "/ITU-T/Network-Operator/[Senegal: \"SENPAC\"]";
                case 6122: return "/ITU-T/Network-Operator/[Côte d'Ivoire: SYTRANPAC]";
                case 6132: return "/ITU-T/Network-Operator/[Burkina Faso: FASOPAC]";
                case 6202: return "/ITU-T/Network-Operator/[Ghana: DATATEL]";
                case 6222: return "/ITU-T/Network-Operator/[Chad: TCHADPAC]";
                case 6242: return "/ITU-T/Network-Operator/[Cameroon: \"CAMPAC\"]";
                case 6255: return "/ITU-T/Network-Operator/[Cape Verde: \"CVDATA\"]";
                case 6280: return "/ITU-T/Network-Operator/[Gabon: GABONPAC (Réseau de transmission de données à commutation par paquets)]";
                case 6282: return "/ITU-T/Network-Operator/[Gabon: GABONPAC2]";
                case 6315: return "/ITU-T/Network-Operator/[Angola: ANGOPAC]";
                case 6331: return "/ITU-T/Network-Operator/[Seychelles: Infolink]";
                case 6390: return "/ITU-T/Network-Operator/[Kenya: \"KENPAC\" - Telkom Kenya Ltd.]";
                case 6435: return "/ITU-T/Network-Operator/[Mozambique: \"COMPAC\" (Packet Switching Public Data Network)]";
                case 6451: return "/ITU-T/Network-Operator/[Zambia: \"ZAMPAK\"]";
                case 6460: return "/ITU-T/Network-Operator/[Madagascar: INFOPAC]";
                case 6484: return "/ITU-T/Network-Operator/[Zimbabwe: \"ZIMNET\"]";
                case 6490: return "/ITU-T/Network-Operator/[Namibia: \"SWANET\" (Public Packet Switched Network)]";
                case 6550: return "/ITU-T/Network-Operator/[South Africa: Saponet - P]";
                case 7080: return "/ITU-T/Network-Operator/[Honduras: HONDUPAQ]";
                case 7100: return "/ITU-T/Network-Operator/[Nicaragua: NicaPac]";
                case 7120: return "/ITU-T/Network-Operator/[Costa Rica: RACSADATOS]";
                case 7141: return "/ITU-T/Network-Operator/[Panama: Red de transmision de datos con conmutacion de paquetes (INTELPAQ)]";
                case 7144: return "/ITU-T/Network-Operator/[Panama: \"CWP\" data network]";
                case 7160: return "/ITU-T/Network-Operator/[Peru: \"MEGANET\" (\"PERUNET\")]";
                case 7161: return "/ITU-T/Network-Operator/[Peru: \"MEGANET\"]";
                case 7221: return "/ITU-T/Network-Operator/[Argentina: Nodo Internacional de Datos - TELINTAR]";
                case 7222: return "/ITU-T/Network-Operator/[Argentina: \"ARPAC\" (\"ENTEL\")]";
                case 7223: return "/ITU-T/Network-Operator/[Argentina: EASYGATE (\"ATT\")]";
                case 7240: return "/ITU-T/Network-Operator/[Brazil: International Packet Switching Data Communication Service (INTERDATA)]";
                case 7241: return "/ITU-T/Network-Operator/[Brazil: National Packet Switching Data Communication Service (\"RENPAC\")]";
                case 7242: return "/ITU-T/Network-Operator/[Brazil: \"RIOPAC\"]";
                case 7243: return "/ITU-T/Network-Operator/[Brazil: MINASPAC]";
                case 7244: return "/ITU-T/Network-Operator/[Brazil: TRANSPAC]";
                case 7245: return "/ITU-T/Network-Operator/[Brazil: Fac Simile Service (DATA FAX)]";
                case 7246: return "/ITU-T/Network-Operator/[Brazil: Brazilian private networks]";
                case 7247: return "/ITU-T/Network-Operator/[Brazil: \"DATASAT BI\"]";
                case 7251: return "/ITU-T/Network-Operator/[Brazil: S.PPAC]";
                case 7252: return "/ITU-T/Network-Operator/[Brazil: \"TELEST\" Public packet data network]";
                case 7253: return "/ITU-T/Network-Operator/[Brazil: TELEMIG Public Switched Packet Data Network]";
                case 7254: return "/ITU-T/Network-Operator/[Brazil: \"PACPAR\"]";
                case 7255: return "/ITU-T/Network-Operator/[Brazil: CRT/CTMR]";
                case 7256: return "/ITU-T/Network-Operator/[Brazil: Western and Midwestern Public Switched Packet Data Network]";
                case 7257: return "/ITU-T/Network-Operator/[Brazil: TELEBAHIA and TELERGIPE Public Switched Packet Data Network]";
                case 7258: return "/ITU-T/Network-Operator/[Brazil: Northeastern Public Switched Packet Data Network]";
                case 7259: return "/ITU-T/Network-Operator/[Brazil: Northern Public Switched Packet Data Network]";
                case 7302: return "/ITU-T/Network-Operator/[Chile: Red nacional de transmision de datos]";
                case 7321: return "/ITU-T/Network-Operator/[Colombia: Red de Alta Velocidad]";
                case 7380: return "/ITU-T/Network-Operator/[Guyana: \"GT&T PAC\"]";
                case 7440: return "/ITU-T/Network-Operator/[Paraguay: PARABAN]";
                case 7447: return "/ITU-T/Network-Operator/[Paraguay: ANTELPAC]";
                case 7448: return "/ITU-T/Network-Operator/[Paraguay: PARAPAQ]";
                case 7482: return "/ITU-T/Network-Operator/[Uruguay: \"URUPAC\" - Servicio publico de transmision de datos con conmutacion de paquetes]";
                case 7488: return "/ITU-T/Network-Operator/[Uruguay: URUPAC - Interfuncionamiento con la red telex]";
                case 7489: return "/ITU-T/Network-Operator/[Uruguay: URUPAC - Interfuncionamiento con la red telefonica]";
                case 23030: return "/ITU-T/Network-Operator/[Czech Republic: \"G-NET\" (code canceled)]";
                case 23040:
                case 23041:
                case 23042:
                case 23043:
                case 23044: return "/ITU-T/Network-Operator/[Czech Republic: RadioNET]";
                case 41362: return "/ITU-T/Network-Operator/[Sri Lanka: \"MTT\" Network (Pvt) Limited]";
                case 41363: return "/ITU-T/Network-Operator/[Sri Lanka: \"DPMC\" Electronics (Pvt) Limited]";
                case 260621: return "/ITU-T/Network-Operator/[Poland: DATACOM]";
                case 260622: return "/ITU-T/Network-Operator/[Poland: \"MNI\"]";
                case 260641: return "/ITU-T/Network-Operator/[Poland: \"PAGI\"]";
                case 260642: return "/ITU-T/Network-Operator/[Poland: Crowley Data Poland]";
                case 260651: return "/ITU-T/Network-Operator/[Poland: MEDIATEL]";
                case 260661: return "/ITU-T/Network-Operator/[Poland: \"KOLPAK\"]";
                case 260662: return "/ITU-T/Network-Operator/[Poland: Energis Polska]";
                case 260672: return "/ITU-T/Network-Operator/[Poland: Virtual Private Network (VPN) Service]";
                case 260681: return "/ITU-T/Network-Operator/[Poland: Exatel]";
                case 260691: return "/ITU-T/Network-Operator/[Poland: \"NETIA\"]";
                case 460200:
                case 460201:
                case 460202:
                case 460203:
                case 460204:
                case 460205:
                case 460206:
                case 460207: return "/ITU-T/Network-Operator/[China: \"CAAC\" privileged data network]";
                default: return $"/ITU-T/Network-Operator/{values[index - 1]}";
            }

        #endregion

        // identified-organization
        #region 0.4.*

        oid_0_4:

            if (index == values.Length) return "/ITU-T/Identified-Organization";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_0_4_0;
                default: return $"/ITU-T/Identified-Organization/{values[index - 1]}";
            }

        #endregion

        // data
        #region 0.9.*

        oid_0_9:

            if (index == values.Length) return "/ITU-T/Data";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_0_9_0;
                default: return $"/ITU-T/Data/{values[index - 1]}";
            }

        #endregion

        #endregion

        // iso
        #region 1.*

        oid_1:

            if (index == values.Length) return "/ISO";
            switch (values[index++])
            {
                case 0: goto oid_1_0;
                case 1: goto oid_1_1;
                case 2: goto oid_1_2;
                case 3: goto oid_1_3;
                default: return $"/ISO/{values[index - 1]}";
            }

        // standard
        #region 1.0.*

        oid_1_0:

            if (index == values.Length) return "/ISO/Standard";
            switch (values[index++])
            {
                //TODO: case 639: goto oid_1_0_639;
                //TODO: case 1087: goto oid_1_0_1087;
                //TODO: case 2022: goto oid_1_0_2022;
                //TODO: case 2382: goto oid_1_0_2382;
                //TODO: case 3166: goto oid_1_0_3166;
                case 4217: return "/ISO/Standard/[Currency Codes]";
                //TODO: case 4426: goto oid_1_0_4426;
                //TODO: case 4922: goto oid_1_0_4922;
                case 5218: return "/ISO/Standard/[Information technology -- Codes for the representation of human sexes]";
                case 6523: return "/ISO/Standard/[Information technology -- Structure for the identification of organizations and organization parts]";
                //TODO: case 7498: goto oid_1_0_7498;
                //TODO: case 7816: goto oid_1_0_7816;
                //TODO: case 8571: goto oid_1_0_8571;
                case 8601: return "/ISO/Standard/[Data elements and interchange formats -- Information interchange -- Representation of dates and times]";
                //TODO: case 8802: goto oid_1_0_8802;
                //TODO: case 9040: goto oid_1_0_9040;
                //TODO: case 9041: goto oid_1_0_9041;
                //TODO: case 9069: goto oid_1_0_9069;
                case 9362: return "/ISO/Standard/[Banking -- Banking telecommunication messages -- Business Identifier Code (BIC)]";
                //TODO: case 9506: goto oid_1_0_9506;
                //TODO: case 9596: goto oid_1_0_9596;
                //TODO: case 9796: goto oid_1_0_9796;
                //TODO: case 9797: goto oid_1_0_9797;
                //TODO: case 9798: goto oid_1_0_9798;
                //TODO: case 9834: goto oid_1_0_9834;
                //TODO: case 9979: goto oid_1_0_9979;
                //TODO: case 9992: goto oid_1_0_9992;
                //TODO: case 10021: goto oid_1_0_10021;
                //TODO: case 10116: goto oid_1_0_10116;
                //TODO: case 10118: goto oid_1_0_10118;
                //TODO: case 10161: goto oid_1_0_10161;
                //TODO: case 10166: goto oid_1_0_10166;
                case 10374: return "/ISO/Standard/[Freight containers -- Automatic identification]";
                //TODO: case 10646: goto oid_1_0_10646;
                //TODO: case 10746: goto oid_1_0_10746;
                case 10891: return "/ISO/Standard/[Freight containers -- Radio frequency identification (RFID) -- Licence plate tag]";
                //TODO: case 11188: goto oid_1_0_11188;
                case 11404: return "/ISO/Standard/[Information technology -- Programming languages, their environments and system software interfaces -- Language-independent datatypes]";
                //TODO: case 11578: goto oid_1_0_11578;
                //TODO: case 11582: goto oid_1_0_11582;
                //TODO: case 11770: goto oid_1_0_11770;
                //TODO: case 12813: goto oid_1_0_12813;
                //TODO: case 12855: goto oid_1_0_12855;
                //TODO: case 13141: goto oid_1_0_13141;
                case 13616: return "/ISO/Standard/[Financial services -- International Bank Account Number (IBAN)]";
                //TODO: case 13868: goto oid_1_0_13868;
                //TODO: case 13869: goto oid_1_0_13869;
                //TODO: case 13870: goto oid_1_0_13870;
                //TODO: case 13873: goto oid_1_0_13873;
                //TODO: case 13874: goto oid_1_0_13874;
                //TODO: case 13888: goto oid_1_0_13888;
                //TODO: case 14813: goto oid_1_0_14813;
                //TODO: case 14816: goto oid_1_0_14816;
                //TODO: case 14823: goto oid_1_0_14823;
                //TODO: case 14843: goto oid_1_0_14843;
                //TODO: case 14844: goto oid_1_0_14844;
                //TODO: case 14846: goto oid_1_0_14846;
                //TODO: case 14888: goto oid_1_0_14888;
                //TODO: case 14906: goto oid_1_0_14906;
                //TODO: case 15050: goto oid_1_0_15050;
                //TODO: case 15052: goto oid_1_0_15052;
                //TODO: case 15054: goto oid_1_0_15054;
                //TODO: case 15118: goto oid_1_0_15118;
                //TODO: case 15418: goto oid_1_0_15418;
                //TODO: case 15429: goto oid_1_0_15429;
                //TODO: case 15431: goto oid_1_0_15431;
                //TODO: case 15433: goto oid_1_0_15433;
                case 15434: return "/ISO/Standard/[Transfer Syntax for High Capacity data carrier]";
                //TODO: case 15459: goto oid_1_0_15459;
                //TODO: case 15506: goto oid_1_0_15506;
                //TODO: case 15507: goto oid_1_0_15507;
                //TODO: case 15628: goto oid_1_0_15628;
                //TODO: case 15772: goto oid_1_0_15772;
                //TODO: case 15946: goto oid_1_0_15946;
                //TODO: case 15961: goto oid_1_0_15961;
                //TODO: case 15992: goto oid_1_0_15992;
                //TODO: case 16460: goto oid_1_0_16460;
                //TODO: case 16785: goto oid_1_0_16785;
                //TODO: case 17090: goto oid_1_0_17090;
                //TODO: case 17262: goto oid_1_0_17262;
                //TODO: case 17264: goto oid_1_0_17264;
                //TODO: case 17419: goto oid_1_0_17419;
                //TODO: case 17423: goto oid_1_0_17423;
                //TODO: case 17429: goto oid_1_0_17429;
                //TODO: case 17515: goto oid_1_0_17515;
                //TODO: case 17573: goto oid_1_0_17573;
                //TODO: case 17575: goto oid_1_0_17575;
                //TODO: case 17876: goto oid_1_0_17876;
                //TODO: case 17878: goto oid_1_0_17878;
                //TODO: case 17922: goto oid_1_0_17922;
                //TODO: case 18013: goto oid_1_0_18013;
                //TODO: case 18014: goto oid_1_0_18014;
                case 18031: return "/ISO/Standard/[Information technology -- Security techniques -- Random bit generation]";
                case 18032: return "/ISO/Standard/[Information technology -- Security techniques -- Prime number generation]";
                //TODO: case 18033: goto oid_1_0_18033;
                //TODO: case 18370: goto oid_1_0_18370;
                //TODO: case 18750: goto oid_1_0_18750;
                //TODO: case 19079: goto oid_1_0_19079;
                //TODO: case 19091: goto oid_1_0_19091;
                //TODO: case 19321: goto oid_1_0_19321;
                //TODO: case 19460: goto oid_1_0_19460;
                //TODO: case 19592: goto oid_1_0_19592;
                //TODO: case 19772: goto oid_1_0_19772;
                //TODO: case 19785: goto oid_1_0_19785;
                //TODO: case 19794: goto oid_1_0_19794;
                //TODO: case 20008: goto oid_1_0_20008;
                //TODO: case 20009: goto oid_1_0_20009;
                case 20022: return "/ISO/Standard/[Universal Financial Industry message scheme]";
                //TODO: case 20248: goto oid_1_0_20248;
                //TODO: case 20684: goto oid_1_0_20684;
                //TODO: case 20828: goto oid_1_0_20828;
                //TODO: case 21000: goto oid_1_0_21000;
                //TODO: case 21091: goto oid_1_0_21091;
                //TODO: case 21177: goto oid_1_0_21177;
                //TODO: case 21184: goto oid_1_0_21184;
                //TODO: case 21185: goto oid_1_0_21185;
                //TODO: case 21192: goto oid_1_0_21192;
                //TODO: case 21193: goto oid_1_0_21193;
                //TODO: case 21210: goto oid_1_0_21210;
                //TODO: case 21215: goto oid_1_0_21215;
                //TODO: case 21218: goto oid_1_0_21218;
                //TODO: case 21407: goto oid_1_0_21407;
                //TODO: case 21889: goto oid_1_0_21889;
                //TODO: case 22418: goto oid_1_0_22418;
                //TODO: case 22895: goto oid_1_0_22895;
                //TODO: case 23264: goto oid_1_0_23264;
                //TODO: case 24102: goto oid_1_0_24102;
                case 24531: return "/ISO/Standard/[Intelligent Transport Systems (ITS) -- System architecture, taxonomy and terminology -- Using eXtensible Markup Language (XML) in ITS standards, data registries and data dictionaries]";
                //TODO: case 24534: goto oid_1_0_24534;
                //TODO: case 24727: goto oid_1_0_24727;
                //TODO: case 24753: goto oid_1_0_24753;
                //TODO: case 24761: goto oid_1_0_24761;
                case 24787: return "/ISO/Standard/[Information technology -- Identification cards -- On-card biometric comparison]";
                //TODO: case 29150: goto oid_1_0_29150;
                //TODO: case 29192: goto oid_1_0_29192;
                //TODO: case 29281: goto oid_1_0_29281;
                //TODO: case 30107: goto oid_1_0_30107;
                //TODO: case 39794: goto oid_1_0_39794;
                //TODO: case 62351: goto oid_1_0_62351;
                //TODO: case 62379: goto oid_1_0_62379;
                //TODO: case 62439: goto oid_1_0_62439;
                //TODO: case 63047: goto oid_1_0_63047;
                default: return $"/ISO/Standard/{values[index - 1]}";
            }

        #endregion

        // registration-authority
        #region 1.1.*

        oid_1_1:

            if (index == values.Length) return "/ISO/Registration-Authority";
            switch (values[index++])
            {
                case 1: return "/ISO/Registration-Authority/[reserved]";
                case 2: return "/ISO/Registration-Authority/[document-type]";
                case 3:
                case 4:
                case 5:
                case 6:
                case 7:
                case 8:
                case 9:
                case 10: return "/ISO/Registration-Authority/[reserved]";
                case 2108: return "/ISO/Registration-Authority/[Information and documentation -- International Standard Book Numbering (ISBN)]";
                //TODO: case 2375: goto oid_1_1_2375;
                //TODO: case 10036: goto oid_1_1_10036;
                //TODO: case 19785: goto oid_1_1_19785;
                //TODO: case 24727: goto oid_1_1_24727;
                default: return $"/ISO/Registration-Authority/{values[index - 1]}";
            }

        #endregion

        // member-body
        #region 1.2.*

        oid_1_2:

            if (index == values.Length) return "/ISO/Member-Body";
            switch (values[index++])
            {
                //TODO: case 36: goto oid_1_2_36;
                //TODO: case 40: goto oid_1_2_40;
                //TODO: case 56: goto oid_1_2_56;
                case 124: return "/ISO/Member-Body/CA";
                //TODO: case 156: goto oid_1_2_156;
                //TODO: case 203: goto oid_1_2_203;
                //TODO: case 208: goto oid_1_2_208;
                //TODO: case 246: goto oid_1_2_246;
                //TODO: case 250: goto oid_1_2_250;
                //TODO: case 276: goto oid_1_2_276;
                case 280: return "/ISO/Member-Body/[Germany: Bundesrepublik Deutschland]";
                //TODO: case 300: goto oid_1_2_300;
                case 344: return "/ISO/Member-Body/HK";
                //TODO: case 372: goto oid_1_2_372;
                //TODO: case 392: goto oid_1_2_392;
                case 398: return "/ISO/Member-Body/KZ";
                //TODO: case 410: goto oid_1_2_410;
                //TODO: case 498: goto oid_1_2_498;
                //TODO: case 528: goto oid_1_2_528;
                case 566: return "/ISO/Member-Body/NG";
                //TODO: case 578: goto oid_1_2_578;
                //TODO: case 616: goto oid_1_2_616;
                //TODO: case 643: goto oid_1_2_643;
                //TODO: case 702: goto oid_1_2_702;
                //TODO: case 752: goto oid_1_2_752;
                //TODO: case 804: goto oid_1_2_804;
                //TODO: case 826: goto oid_1_2_826;
                //TODO: case 840: goto oid_1_2_840;
                default: return $"/ISO/Member-Body/{values[index - 1]}";
            }

        #endregion

        // identified-organization
        #region 1.3.*

        oid_1_3:

            if (index == values.Length) return "/ISO/Identified-Organization";
            switch (values[index++])
            {
                case 1: return "/ISO/Identified-Organization/[Not assigned]";
                case 2: return "/ISO/Identified-Organization/[Système d'Information et Répertoire des ENtreprises et des Etablissements (SIRENE)]";
                case 3: return "/ISO/Identified-Organization/[Codification numérique des établissements financiers en Belgique]";
                case 4: return "/ISO/Identified-Organization/[National Bureau of Standards (NBS) Open System Interconnection NETwork (OSINET)]";
                case 5: return "/ISO/Identified-Organization/[United States Federal Government Open System interconnection NETwork (GOSNET)]";
                case 6: return "/ISO/Identified-Organization/[\"DODNET\": Open System Interconnection (OSI) network for the Department of Defense (DoD)]";
                case 7: return "/ISO/Identified-Organization/[Organisationsnummer]";
                case 8: return "/ISO/Identified-Organization/[Le Numéro national]";
                case 9: return "/ISO/Identified-Organization/[Système d'Identification du Répertoire des ETablissements (SIRET) codes]";
                case 10: return "/ISO/Identified-Organization/[Organizational identifiers for structured names under ISO 9541-2]";
                case 11: return "/ISO/Identified-Organization/[OSI-based amateur radio organizations, network objects and application services]";
                //TODO: case 12: goto oid_1_3_12;
                case 13: return "/ISO/Identified-Organization/[Code assigned by the German Automotive Association to companies operating file transfer stations using Odette File Transfer Protocol (OFTP) (formerly, \"VSA\" File Transfer Protocol (FTP) code)]";
                //TODO: case 14: goto oid_1_3_14;
                //TODO: case 15: goto oid_1_3_15;
                //TODO: case 16: goto oid_1_3_16;
                case 17: return "/ISO/Identified-Organization/[COMMON LANGUAGE]";
                //TODO: case 18: goto oid_1_3_18;
                case 19: return "/ISO/Identified-Organization/[Air Transport Industry Services Communications Network]";
                case 20: return "/ISO/Identified-Organization/[European Laboratory for Particle Physics \"CERN\"]";
                case 21: return "/ISO/Identified-Organization/[Society for Worldwide Interbank Financial Telecommunication (SWIFT)]";
                //TODO: case 22: goto oid_1_3_22;
                case 23: return "/ISO/Identified-Organization/[Nordic University and Research Network: NORDUnet]";
                case 24: return "/ISO/Identified-Organization/[Digital Equipment Corporation (DEC)]";
                case 25: return "/ISO/Identified-Organization/[OSI Asia-Oceania Workshop (AOW)]";
                //TODO: case 26: goto oid_1_3_26;
                //TODO: case 27: goto oid_1_3_27;
                case 28: return "/ISO/Identified-Organization/[Organisation for Data Exchange through TeleTransmission in Europe (ODETTE)]";
                case 29: return "/ISO/Identified-Organization/[The all-union classifier of enterprises and organizations]";
                case 30: return "/ISO/Identified-Organization/[AT&T/OSI network]";
                case 31: return "/ISO/Identified-Organization/[AT&T/Electronic Data Interchange (EDI) partner identification code]";
                case 32: return "/ISO/Identified-Organization/[Telecom Australia]";
                case 33: return "/ISO/Identified-Organization/[S G Warburg Group Management Ltd OSI Internetwork]";
                case 34: return "/ISO/Identified-Organization/[Reuter open address standard]";
                case 35: return "/ISO/Identified-Organization/[British Petroleum Ltd]";
                //TODO: case 36: goto oid_1_3_36;
                case 37: return "/ISO/Identified-Organization/[LY-tunnus]";
                case 38: return "/ISO/Identified-Organization/[The Australian Government Open Systems Interconnection Profile (GOSIP) network]";
                case 39: return "/ISO/Identified-Organization/[\"OZDOD DEFNET\": Australian Department Of Defence (DOD) OSI network]";
                case 40: return "/ISO/Identified-Organization/[Unilever Group Companies]";
                case 41: return "/ISO/Identified-Organization/[Citicorp Global Information Network (CGIN)]";
                case 42: return "/ISO/Identified-Organization/[Deutsche BundesPost (DBP) Telekom]";
                case 43: return "/ISO/Identified-Organization/[HydroNETT]";
                case 44: return "/ISO/Identified-Organization/[Thai Industrial Standards Institute (TISI)]";
                case 45: return "/ISO/Identified-Organization/[\"ICI\" Company]";
                case 46: return "/ISO/Identified-Organization/[Philips FUNction LOCations (FUNLOC)]";
                case 47: return "/ISO/Identified-Organization/[Bull \"ODI\"/Distributed System Architecture (DSA)/Unix network]";
                case 48: return "/ISO/Identified-Organization/[\"OSINZ\"]";
                case 49: return "/ISO/Identified-Organization/[Auckland Area Health]";
                case 50: return "/ISO/Identified-Organization/[Firmenich]";
                case 51: return "/ISO/Identified-Organization/[\"AGFA-DIS\"]";
                case 52: return "/ISO/Identified-Organization/[Society of Motion Picture and Television Engineers (SMPTE)]";
                case 53: return "/ISO/Identified-Organization/[Migros_Network M_NETOPZ]";
                case 54: return "/ISO/Identified-Organization/[Pfizer]";
                case 55: return "/ISO/Identified-Organization/[Energy Net]";
                case 56: return "/ISO/Identified-Organization/[Nokia]";
                case 57: return "/ISO/Identified-Organization/[Saint Gobain]";
                case 58: return "/ISO/Identified-Organization/[Siemens Corporate Network (SCN)]";
                case 59: return "/ISO/Identified-Organization/[\"DANZNET\"]";
                case 60: return "/ISO/Identified-Organization/[Dun & Bradstreet Data Universal Numbering System (D-U-N-S)]";
                case 61: return "/ISO/Identified-Organization/[\"SOFFEX\" OSI]";
                case 62: return "/ISO/Identified-Organization/[Koninklijke \"PTT\" Nederland (KPN) \"OVN\" (operator fixed networks)]";
                case 63: return "/ISO/Identified-Organization/[AscomOSINet]";
                case 64: return "/ISO/Identified-Organization/[Uniform Transport Code (UTC)]";
                case 65: return "/ISO/Identified-Organization/[Solvay Group]";
                case 66: return "/ISO/Identified-Organization/[Roche Corporate Network]";
                case 67: return "/ISO/Identified-Organization/[Zellweger]";
                case 68: return "/ISO/Identified-Organization/[Intel Corporation]";
                case 69: return "/ISO/Identified-Organization/[SITA (Société Internationale de Télécommunications Aéronautiques)]";
                case 70: return "/ISO/Identified-Organization/[DaimlerChrysler Corporate Network (DCCN)]";
                case 71: return "/ISO/Identified-Organization/[LEGOnet]";
                case 72: return "/ISO/Identified-Organization/[Navistar]";
                case 73: return "/ISO/Identified-Organization/[Formatted Asynchronous Transfer Mode (ATM) address]";
                case 74: return "/ISO/Identified-Organization/[\"ARINC\"]";
                case 75: return "/ISO/Identified-Organization/[Alcanet (Alcatel-Alsthom vorporate network)]";
                //TODO: case 76: goto oid_1_3_76;
                case 77: return "/ISO/Identified-Organization/[Sistema Italiano di Indirizzamento di Reti OSI Gestito da \"UNINFO\"]";
                case 78: return "/ISO/Identified-Organization/[Mitel terminal or switching equipment]";
                case 79: return "/ISO/Identified-Organization/[Asynchronous Transfer Mode (ATM) Forum]";
                case 80: return "/ISO/Identified-Organization/[UK national health service scheme (Electronic Data Interchange Registration Authorities (EDIRA) compliant)]";
                case 81: return "/ISO/Identified-Organization/[International Network Service Access Point (NSAP)]";
                case 82: return "/ISO/Identified-Organization/[Norwegian Telecommunications Authority (NTA)]";
                case 83: return "/ISO/Identified-Organization/[Advanced Telecommunications Modules Limited Corporate Network]";
                case 84: return "/ISO/Identified-Organization/[Athens Chamber of Commerce & Industry Scheme]";
                case 85: return "/ISO/Identified-Organization/[Swisskey certificate authority coding system]";
                case 86: return "/ISO/Identified-Organization/[United States Council for International Business (USCIB)]";
                case 87: return "/ISO/Identified-Organization/[National Federation of Chambers of Commerce & Industry of Belgium Scheme]";
                case 88: return "/ISO/Identified-Organization/[Global Location Number (GLN) (previously, European Article Number (EAN) location code)]";
                case 89: return "/ISO/Identified-Organization/[Association of British Chambers of Commerce Ltd. scheme]";
                case 90: return "/ISO/Identified-Organization/[Internet Protocol (IP) addressing]";
                case 91: return "/ISO/Identified-Organization/[Cisco Systems / Open Systems Interconnection (OSI) network]";
                case 92: return "/ISO/Identified-Organization/[Not to be assigned]";
                case 93: return "/ISO/Identified-Organization/[Revenue Canada Business Number (BN) registration]";
                case 94: return "/ISO/Identified-Organization/[Deutscher Industrie- und HandelsTag (DIHT) scheme]";
                case 95: return "/ISO/Identified-Organization/[Hewlett-Packard (HP) Company internal Asynchronous Transfer Mode (ATM) network]";
                case 96: return "/ISO/Identified-Organization/[Danish Chamber of Commerce & Industry]";
                case 97: return "/ISO/Identified-Organization/[\"FTI\" - Ediforum Italia]";
                case 98: return "/ISO/Identified-Organization/[Chamber of Commerce Tel Aviv-Jaffa]";
                case 99: return "/ISO/Identified-Organization/[Siemens Supervisory Systems Network]";
                case 100: return "/ISO/Identified-Organization/[PNG_ICD scheme]";
                //TODO: case 101: goto oid_1_3_101;
                case 102: return "/ISO/Identified-Organization/[\"HEAG\" holding group]";
                case 103: return "/ISO/Identified-Organization/[Reserved for later allocation]";
                case 104: return "/ISO/Identified-Organization/[British Telecommunications plc (BT)]";
                case 105: return "/ISO/Identified-Organization/[Portuguese Chamber of Commerce and Industry]";
                case 106: return "/ISO/Identified-Organization/[Vereniging van Kamers van Koophandel en Fabrieken in Nederland]";
                case 107: return "/ISO/Identified-Organization/[Association of Swedish Chambers of Commerce and Industry]";
                case 108: return "/ISO/Identified-Organization/[Australian Chambers of Commerce and Industry]";
                case 109: return "/ISO/Identified-Organization/[BellSouth Asynchronous Transfer Mode (ATM) End System Address (AESA)]";
                case 110: return "/ISO/Identified-Organization/[Bell Atlantic]";
                //TODO: case 111: goto oid_1_3_111;
                //TODO: case 112: goto oid_1_3_112;
                case 113: return "/ISO/Identified-Organization/[OriginNet]";
                //TODO: case 114: goto oid_1_3_114;
                case 115: return "/ISO/Identified-Organization/[Pacific Bell data communications network]";
                case 116: return "/ISO/Identified-Organization/[Postal Security Services (PSS)]";
                case 117: return "/ISO/Identified-Organization/[Stentor]";
                case 118: return "/ISO/Identified-Organization/[ATM-Network ZN\"96]";
                case 119: return "/ISO/Identified-Organization/[\"MCI\" OSI network]";
                case 120: return "/ISO/Identified-Organization/[Advantis]";
                case 121: return "/ISO/Identified-Organization/[Affable Software data interchange codes]";
                case 122: return "/ISO/Identified-Organization/[BB-DATA GmbH]";
                case 123: return "/ISO/Identified-Organization/[Badische Anilin und Soda Fabrik (BASF) company Asynchronous Transfer Mode (ATM) network]";
                //TODO: case 124: goto oid_1_3_124;
                case 125: return "/ISO/Identified-Organization/[Henkel Corporate Network (H-Net)]";
                case 126: return "/ISO/Identified-Organization/[\"GTE\" OSI network]";
                case 127: return "/ISO/Identified-Organization/[Allianz Technology]";
                case 128: return "/ISO/Identified-Organization/[\"BCNR\" (Swiss clearing bank number)]";
                case 129: return "/ISO/Identified-Organization/[Telekurs Business Partner Identification (BPI)]";
                //TODO: case 130: goto oid_1_3_130;
                case 131: return "/ISO/Identified-Organization/[Code for the Identification of National Organizations]";
                //TODO: case 132: goto oid_1_3_132;
                //TODO: case 133: goto oid_1_3_133;
                case 134: return "/ISO/Identified-Organization/[Infonet Services Corporation]";
                case 135: return "/ISO/Identified-Organization/[Societa Interbancaria per l'Automazione (SIA) S.p.A.]";
                case 136: return "/ISO/Identified-Organization/[Cable & Wireless global Asynchronous Transfer Mode (ATM) end-system address plan]";
                case 137: return "/ISO/Identified-Organization/[Global One Asynchronous Transfer Mode (ATM) End System Address (AESA) scheme]";
                case 138: return "/ISO/Identified-Organization/[France Telecom Asynchronous Transfer Mode (ATM) End System Address (AESA) plan]";
                case 139: return "/ISO/Identified-Organization/[Savvis Communications Asynchronous Transfer Mode (ATM) End System Address (AESA)]";
                case 140: return "/ISO/Identified-Organization/[Toshiba Organizations, Partners, And Suppliers (TOPAS) code]";
                case 141: return "/ISO/Identified-Organization/[North Atlantic Treaty Organization (NATO) Commercial And Government Entity (NCAGE) system, a.k.a. NCAGE NATO Code of manufacturers]";
                case 142: return "/ISO/Identified-Organization/[\"SECETI S.p.A.\"]";
                case 143: return "/ISO/Identified-Organization/[EINESTEINet AG]";
                case 144: return "/ISO/Identified-Organization/[Department of Defense Activity Address Code (DoDAAC)]";
                case 145: return "/ISO/Identified-Organization/[Direction Générale de la Comptabilité Publique (DGCP) administrative accounting identification scheme]";
                case 146: return "/ISO/Identified-Organization/[Direction Générale des Impots (DGI)]";
                case 147: return "/ISO/Identified-Organization/[Standard company code (partner identification code) registered with \"JIPDEC\"]";
                case 148: return "/ISO/Identified-Organization/[International Telecommunication Union (ITU) Data Network Identification Codes (DNIC)]";
                case 149: return "/ISO/Identified-Organization/[Global Business Identifier (GBI)]";
                case 150: return "/ISO/Identified-Organization/[Madge Networks Ltd Asynchronous Transfer Mode (ATM) addressing scheme]";
                case 151: return "/ISO/Identified-Organization/[Australian Business Number (ABN) scheme]";
                case 152: return "/ISO/Identified-Organization/[Electronic Data Interchange Registration Authorities (EDIRA) scheme identifier code]";
                case 153: return "/ISO/Identified-Organization/[Concert Global network services Asynchronous Transfer Mode (ATM) End System Address (AESA)]";
                //TODO: case 154: goto oid_1_3_154;
                case 155: return "/ISO/Identified-Organization/[Global Crossing Asynchronous Transfer Mode (ATM) End System Address (AESA)]";
                case 156: return "/ISO/Identified-Organization/[\"AUNA\"]";
                case 157: return "/ISO/Identified-Organization/[Informatie en communicatie Technologie Organisatie (ITO) Drager Net]";
                //TODO: case 158: goto oid_1_3_158;
                //TODO: case 159: goto oid_1_3_159;
                case 160: return "/ISO/Identified-Organization/[GS1 Global Trade Item Number (GTIN)]";
                case 161: return "/ISO/Identified-Organization/[Electronic Commerce Code Management Association (ECCMA) open technical dictionary]";
                //TODO: case 162: goto oid_1_3_162;
                case 163: return "/ISO/Identified-Organization/[United States Environmental Protection Agency (US-EPA) facilities]";
                case 164: return "/ISO/Identified-Organization/[\"TELUS\" Corporation Asynchronous Transfer Mode (ATM) End System Address (AESA) addressing scheme for ATM Private Network-to-Network Interface (PNNI) implementation]";
                case 165: return "/ISO/Identified-Organization/[\"FIEIE\"]";
                case 166: return "/ISO/Identified-Organization/[Swissguide]";
                case 167: return "/ISO/Identified-Organization/[Priority Telecom Asynchronous Transfer Mode (ATM) End System Address (AESA) plan]";
                case 168: return "/ISO/Identified-Organization/[Vodafone Ireland]";
                case 169: return "/ISO/Identified-Organization/[Swiss Federal Business Identification Number]";
                case 170: return "/ISO/Identified-Organization/[Teikoku Company Code]";
                //TODO: case 171: goto oid_1_3_171;
                case 172: return "/ISO/Identified-Organization/[Project Group \"Lists of properties\" (PROLIST®)]";
                case 173: return "/ISO/Identified-Organization/[eCl@ss]";
                case 174: return "/ISO/Identified-Organization/[StepNexus]";
                case 175: return "/ISO/Identified-Organization/[Siemens AG]";
                case 176: return "/ISO/Identified-Organization/[Paradine GmbH]";
                //TODO: case 177: goto oid_1_3_177;
                case 178: return "/ISO/Identified-Organization/[Route1's MobiNET]";
                //TODO: case 179: goto oid_1_3_179;
                case 180: return "/ISO/Identified-Organization/[Lithuanian military Public Key Infrastructure (PKI)]";
                case 183: return "/ISO/Identified-Organization/[Unique IDentification Business (UIDB) number]";
                case 184: return "/ISO/Identified-Organization/[\"DIGSTORG\"]";
                case 185: return "/ISO/Identified-Organization/[Perceval Object Code (POC)]";
                case 186: return "/ISO/Identified-Organization/[TrustPoint]";
                case 187: return "/ISO/Identified-Organization/[Amazon Unique Identification Scheme (AUIS)]";
                case 188: return "/ISO/Identified-Organization/[Corporate number of the social security and tax number system of Japan]";
                case 189: return "/ISO/Identified-Organization/[European Business IDentifier (EBID)]";
                case 190: return "/ISO/Identified-Organization/[Organisatie Identificatie Nummer (OIN)]";
                case 191: return "/ISO/Identified-Organization/[Company code (Estonia)]";
                case 192: return "/ISO/Identified-Organization/[Organisasjonsnummer, Norway]";
                case 193: return "/ISO/Identified-Organization/[UBL.BE party identifier]";
                case 194: return "/ISO/Identified-Organization/[\"KOIOS\" Open Technical Dictionary (OTD)]";
                case 195: return "/ISO/Identified-Organization/[Singapore nationwide e-invoice framework]";
                case 196: return "/ISO/Identified-Organization/[Íslensk kennitala]";
                case 197: return "/ISO/Identified-Organization/[Reserved]";
                case 198: return "/ISO/Identified-Organization/[ERSTORG]";
                case 199: return "/ISO/Identified-Organization/[Legal Entity Identifier (LEI)]";
                case 200: return "/ISO/Identified-Organization/[Legal entity code (Lithuania)]";
                case 201: return "/ISO/Identified-Organization/[Codice Univoco Unità Organizzativa iPA]";
                case 202: return "/ISO/Identified-Organization/[Indirizzo di Posta Elettronica Certificata]";
                case 203: return "/ISO/Identified-Organization/[eDelivery network participant identifier]";
                case 204: return "/ISO/Identified-Organization/[Leitweg-ID]";
                case 205: return "/ISO/Identified-Organization/[CODDEST]";
                case 206: return "/ISO/Identified-Organization/[Registre du Commerce et de l'Industrie (RCI), Monaco]";
                case 207: return "/ISO/Identified-Organization/[Pilog Ontology Codification Identifier (POCI)]";
                case 208: return "/ISO/Identified-Organization/[Numéro d'entreprise / Ondernemingsnummer / Unternehmensnummer, Belgium]";
                case 209: return "/ISO/Identified-Organization/[GS1 identification keys]";
                case 210: return "/ISO/Identified-Organization/[Codice fiscale]";
                case 211: return "/ISO/Identified-Organization/[Partita iva]";
                case 212: return "/ISO/Identified-Organization/[Finnish Organization Identifier]";
                case 213: return "/ISO/Identified-Organization/[Finnish organization value add tax identifier]";
                case 214: return "/ISO/Identified-Organization/[Tradeplace TradePI (Product Information) standard]";
                case 215: return "/ISO/Identified-Organization/[Net service identifier]";
                case 216: return "/ISO/Identified-Organization/[OVTcode]";
                case 9900:
                case 9991:
                case 9992:
                case 9993:
                case 9994:
                case 9995:
                case 9996:
                case 9997:
                case 9998:
                case 9999: return "/ISO/Identified-Organization/[Reserved]";
                default: return $"/ISO/Identified-Organization/{values[index - 1]}";
            }

        #endregion

        #endregion

        // joint-iso-itu-t, joint-iso-ccitt
        #region 2.*

        oid_2:

            if (index == values.Length) return "/Joint-ISO-ITU-T";
            switch (values[index++])
            {
                case 0: return "/Joint-ISO-ITU-T/[Presentation layer service and protocol]";
                case 1: goto oid_2_1;
                case 2: goto oid_2_2;
                case 3: goto oid_2_3;
                case 4: goto oid_2_4;
                case 5: goto oid_2_5;
                case 6: goto oid_2_6;
                case 7: goto oid_2_7;
                case 8: goto oid_2_8;
                case 9: goto oid_2_9;
                case 10: goto oid_2_10;
                case 11: goto oid_2_11;
                case 12: goto oid_2_12;
                case 13: goto oid_2_13;
                case 14: goto oid_2_14;
                case 15: goto oid_2_15;
                case 16: goto oid_2_16;
                case 17: goto oid_2_17;
                case 18: goto oid_2_18;
                case 19: goto oid_2_19;
                case 20: goto oid_2_20;
                case 21: goto oid_2_21;
                case 22: goto oid_2_22;
                case 23: goto oid_2_23;
                case 24: goto oid_2_24;
                case 25: goto oid_2_25;
                case 26: goto oid_2_26;
                case 27: goto oid_2_27;
                case 28: goto oid_2_28;
                case 40: goto oid_2_40;
                case 41: goto oid_2_41;
                case 42: goto oid_2_42;
                case 48: goto oid_2_48;
                case 49: goto oid_2_49;
                case 50: goto oid_2_50;
                case 51: goto oid_2_51;
                case 52: goto oid_2_52;
                case 999: return "/Joint-ISO-ITU-T/Example";
                default: return $"/Joint-ISO-ITU-T/{values[index - 1]}";
            }

        // asn1
        #region 2.1.*

        oid_2_1:

            if (index == values.Length) return "/ASN.1";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_1_0;
                case 1: return "/ASN.1/[Basic Encoding Rules (BER)]";
                //TODO: case 2: goto oid_2_1_2;
                //TODO: case 3: goto oid_2_1_3;
                //TODO: case 4: goto oid_2_1_4;
                //TODO: case 5: goto oid_2_1_5;
                //TODO: case 6: goto oid_2_1_6;
                case 7: return "/ASN.1/[Javascript object notation Encoding Rules (JER)]";
                //TODO: case 8: goto oid_2_1_8;
                case 123: return "/ASN.1/[Examples]";
                default: return $"/ASN.1/{values[index - 1]}";
            }

        #endregion

        // association-control
        #region 2.2.*

        oid_2_2:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Association Control Service Element (ACSE)]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_2_0;
                //TODO: case 1: goto oid_2_2_1;
                //TODO: case 2: goto oid_2_2_2;
                //TODO: case 3: goto oid_2_2_3;
                default: return $"/Joint-ISO-ITU-T/[Association Control Service Element (ACSE)]/{values[index - 1]}";
            }

        #endregion

        // reliable-transfer
        #region 2.3.*

        oid_2_3:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Reliable transfer service element]";
            switch (values[index++])
            {
                case 0: return "/Joint-ISO-ITU-T/[Reliable transfer service element]/[Reliable-Transfer-APDU]";
                case 1: return "/Joint-ISO-ITU-T/[Reliable transfer service element]/[Reliable Transfer Service Element (RTSE) identifier]";
                case 2: return "/Joint-ISO-ITU-T/[Reliable transfer service element]/[Abstract syntaxes]";
                default: return $"/Joint-ISO-ITU-T/[Reliable transfer service element]/{values[index - 1]}";
            }

        #endregion

        // remote-operations
        #region 2.4.*

        oid_2_4:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Remote Operations Service Element (ROSE)]";
            switch (values[index++])
            {
                case 0: return "/Joint-ISO-ITU-T/[Remote Operations Service Element (ROSE)]/[Remote-Operation-Notation]";
                case 1: return "/Joint-ISO-ITU-T/[Remote Operations Service Element (ROSE)]/[Remote-Operations-APDUs]";
                case 2: return "/Joint-ISO-ITU-T/[Remote Operations Service Element (ROSE)]/[Remote-Operations-Notation-extension]";
                case 3: return "/Joint-ISO-ITU-T/[Remote Operations Service Element (ROSE)]/[Application Service Element (ASE) identifier]";
                case 4: return "/Joint-ISO-ITU-T/[Remote Operations Service Element (ROSE)]/[Association Control Service Element (ACSE)]";
                //TODO: case 5: goto oid_2_4_5;
                //TODO: case 6: goto oid_2_4_6;
                //TODO: case 7: goto oid_2_4_7;
                //TODO: case 8: goto oid_2_4_8;
                //TODO: case 9: goto oid_2_4_9;
                //TODO: case 10: goto oid_2_4_10;
                //TODO: case 11: goto oid_2_4_11;
                //TODO: case 12: goto oid_2_4_12;
                default: return $"/Joint-ISO-ITU-T/[Remote Operations Service Element (ROSE)]/{values[index - 1]}";
            }

        #endregion

        // ds, directory
        #region 2.5.*

        oid_2_5:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Directory services]";
            switch (values[index++])
            {
                //TODO: case 1: goto oid_2_5_1;
                case 2: return "/Joint-ISO-ITU-T/[Directory services]/[Directory service elements]";
                //TODO: case 3: goto oid_2_5_3;
                //TODO: case 4: goto oid_2_5_4;
                //TODO: case 5: goto oid_2_5_5;
                //TODO: case 6: goto oid_2_5_6;
                case 7: return "/Joint-ISO-ITU-T/[Directory services]/[X.500 attribute sets]";
                //TODO: case 8: goto oid_2_5_8;
                //TODO: case 9: goto oid_2_5_9;
                //TODO: case 12: goto oid_2_5_12;
                //TODO: case 13: goto oid_2_5_13;
                //TODO: case 14: goto oid_2_5_14;
                //TODO: case 15: goto oid_2_5_15;
                case 16: return "/Joint-ISO-ITU-T/[Directory services]/[Groups]";
                //TODO: case 17: goto oid_2_5_17;
                //TODO: case 18: goto oid_2_5_18;
                //TODO: case 19: goto oid_2_5_19;
                //TODO: case 20: goto oid_2_5_20;
                //TODO: case 21: goto oid_2_5_21;
                //TODO: case 23: goto oid_2_5_23;
                //TODO: case 24: goto oid_2_5_24;
                //TODO: case 25: goto oid_2_5_25;
                //TODO: case 26: goto oid_2_5_26;
                //TODO: case 27: goto oid_2_5_27;
                //TODO: case 28: goto oid_2_5_28;
                //TODO: case 29: goto oid_2_5_29;
                //TODO: case 30: goto oid_2_5_30;
                //TODO: case 31: goto oid_2_5_31;
                //TODO: case 32: goto oid_2_5_32;
                //TODO: case 33: goto oid_2_5_33;
                //TODO: case 34: goto oid_2_5_34;
                //TODO: case 35: goto oid_2_5_35;
                case 36: return "/Joint-ISO-ITU-T/[Directory services]/[Matching restrictions]";
                //TODO: case 37: goto oid_2_5_37;
                case 38: return "/Joint-ISO-ITU-T/[Directory services]/[Key purposes]";
                //TODO: case 39: goto oid_2_5_39;
                //TODO: case 40: goto oid_2_5_40;
                //TODO: case 41: goto oid_2_5_41;
                //TODO: case 42: goto oid_2_5_42;
                //TODO: case 43: goto oid_2_5_43;
                //TODO: case 44: goto oid_2_5_44;
                default: return $"/Joint-ISO-ITU-T/[Directory services]/{values[index - 1]}";
            }

        #endregion

        // mhs, mhs-motis
        #region 2.6.*

        oid_2_6:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Message Handling System (MHS)]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_6_0;
                //TODO: case 1: goto oid_2_6_1;
                //TODO: case 2: goto oid_2_6_2;
                //TODO: case 3: goto oid_2_6_3;
                //TODO: case 4: goto oid_2_6_4;
                //TODO: case 5: goto oid_2_6_5;
                //TODO: case 6: goto oid_2_6_6;
                //TODO: case 7: goto oid_2_6_7;
                //TODO: case 8: goto oid_2_6_8;
                //TODO: case 9: goto oid_2_6_9;
                //TODO: case 10: goto oid_2_6_10;
                default: return $"/Joint-ISO-ITU-T/[Message Handling System (MHS)]/{values[index - 1]}";
            }

        #endregion

        // ccr
        #region 2.7.*

        oid_2_7:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Commitment, Concurrency and Recovery (CCR) service and protocol]";
            switch (values[index++])
            {
                //TODO: case 1: goto oid_2_7_1;
                //TODO: case 2: goto oid_2_7_2;
                default: return $"/Joint-ISO-ITU-T/[Commitment, Concurrency and Recovery (CCR) service and protocol]/{values[index - 1]}";
            }

        #endregion

        // oda
        #region 2.8.*

        oid_2_8:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Open Document Architecture (ODA)]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_8_0;
                //TODO: case 1: goto oid_2_8_1;
                //TODO: case 2: goto oid_2_8_2;
                //TODO: case 3: goto oid_2_8_3;
                case 4: return "/Joint-ISO-ITU-T/[Open Document Architecture (ODA)]/[Identification of a document application profile]";
                default: return $"/Joint-ISO-ITU-T/[Open Document Architecture (ODA)]/{values[index - 1]}";
            }

        #endregion

        // ms, osi-management
        #region 2.9.*

        oid_2_9:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[OSI network management]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_9_0;
                //TODO: case 1: goto oid_2_9_1;
                //TODO: case 2: goto oid_2_9_2;
                //TODO: case 3: goto oid_2_9_3;
                //TODO: case 4: goto oid_2_9_4;
                default: return $"/Joint-ISO-ITU-T/[OSI network management]/{values[index - 1]}";
            }

        #endregion

        // transaction-processing
        #region 2.10.*

        oid_2_10:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Transaction processing]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_10_0;
                //TODO: case 1: goto oid_2_10_1;
                //TODO: case 2: goto oid_2_10_2;
                default: return $"/Joint-ISO-ITU-T/[Transaction processing]/{values[index - 1]}";
            }

        #endregion

        // dor, distinguished-object-reference
        #region 2.11.*

        oid_2_11:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Information technology -- Text and office systems -- Distributed-office-applications model -- Part 2: Distinguished-object-reference and associated procedures]";
            switch (values[index++])
            {
                case 0: return "/Joint-ISO-ITU-T/[Information technology -- Text and office systems -- Distributed-office-applications model -- Part 2: Distinguished-object-reference and associated procedures]/[DOR-definition]";
                case 1: return "/Joint-ISO-ITU-T/[Information technology -- Text and office systems -- Distributed-office-applications model -- Part 2: Distinguished-object-reference and associated procedures]/[Abstract syntax of \"distinguished-object-reference\"]";
                //TODO: case 2: goto oid_2_11_2;
                default: return $"/Joint-ISO-ITU-T/[Information technology -- Text and office systems -- Distributed-office-applications model -- Part 2: Distinguished-object-reference and associated procedures]/{values[index - 1]}";
            }

        #endregion

        // reference-data-transfer, rdt
        #region 2.12.*

        oid_2_12:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Referenced Data Transfer (RDT)]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_12_0;
                //TODO: case 1: goto oid_2_12_1;
                //TODO: case 2: goto oid_2_12_2;
                //TODO: case 3: goto oid_2_12_3;
                //TODO: case 4: goto oid_2_12_4;
                default: return $"/Joint-ISO-ITU-T/[Referenced Data Transfer (RDT)]/{values[index - 1]}";
            }

        #endregion

        // network-layer, network-layer-management
        #region 2.13.*

        oid_2_13:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Network layer management]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_13_0;
                default: return $"/Joint-ISO-ITU-T/[Network layer management]/{values[index - 1]}";
            }

        #endregion

        // transport-layer, transport-layer-management
        #region 2.14.*

        oid_2_14:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Transport layer management]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_14_0;
                default: return $"/Joint-ISO-ITU-T/[Transport layer management]/{values[index - 1]}";
            }

        #endregion

        // datalink-layer, datalink-layer-management, datalink-layer-management-information
        #region 2.15.*

        oid_2_15:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[OSI data link layer management]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_15_0;
                default: return $"/Joint-ISO-ITU-T/[OSI data link layer management]/{values[index - 1]}";
            }

        #endregion

        // country
        #region 2.16.*

        oid_2_16:

            if (index == values.Length) return "/Country";
            switch (values[index++])
            {
                case 4: return "/Country/AF";
                case 8: return "/Country/AL";
                //case 12: goto oid_2_16_12;
                //case 20: goto oid_2_16_20;
                case 24: return "/Country/AO";
                case 28: return "/Country/AG";
                case 31: return "/Country/AZ";
                //case 32: goto oid_2_16_32;
                case 36: return "/Country/AU";
                case 40: return "/Country/AT";
                case 44: return "/Country/BS";
                case 48: return "/Country/BH";
                //case 50: goto oid_2_16_50;
                case 51: return "/Country/AM";
                case 52: return "/Country/BB";
                case 56: return "/Country/BE";
                //case 60: goto oid_2_16_60;
                case 64: return "/Country/BT";
                case 68: return "/Country/BO";
                case 70: return "/Country/BA";
                case 72: return "/Country/BW";
                //case 76: goto oid_2_16_76;
                case 84: return "/Country/BZ";
                case 90: return "/Country/SB";
                case 96: return "/Country/BN";
                case 100: return "/Country/BG";
                case 104: return "/Country/MM";
                case 108: return "/Country/BI";
                case 112: return "/Country/BY";
                case 116: return "/Country/KH";
                case 120: return "/Country/CM";
                //case 124: goto oid_2_16_124;
                case 132: return "/Country/CV";
                case 140: return "/Country/CF";
                //case 144: goto oid_2_16_144;
                case 148: return "/Country/TD";
                case 152: return "/Country/CL";
                //case 156: goto oid_2_16_156;
                //case 158: goto oid_2_16_158;
                case 170: return "/Country/CO";
                case 174: return "/Country/KM";
                case 178: return "/Country/CG";
                case 180: return "/Country/CD";
                case 188: return "/Country/CR";
                //case 191: goto oid_2_16_191;
                case 192: return "/Country/CU";
                case 196: return "/Country/CY";
                case 203: return "/Country/CZ";
                case 204: return "/Country/BJ";
                case 208: return "/Country/DK";
                case 212: return "/Country/DM";
                case 214: return "/Country/DO";
                //case 218: goto oid_2_16_218;
                case 222: return "/Country/SV";
                case 226: return "/Country/GQ";
                case 231: return "/Country/ET";
                case 232: return "/Country/ER";
                case 233: return "/Country/EE";
                case 242: return "/Country/FJ";
                case 246: return "/Country/FI";
                case 250: return "/Country/FR";
                case 262: return "/Country/DJ";
                case 266: return "/Country/GA";
                case 268: return "/Country/GE";
                case 270: return "/Country/GM";
                case 275: return "/Country/PS";
                //case 276: goto oid_2_16_276;
                case 288: return "/Country/GH";
                case 296: return "/Country/KI";
                case 300: return "/Country/GR";
                case 308: return "/Country/GD";
                case 320: return "/Country/GT";
                case 324: return "/Country/GN";
                case 328: return "/Country/GY";
                case 332: return "/Country/HT";
                case 336: return "/Country/VA";
                //case 340: goto oid_2_16_340;
                //case 344: goto oid_2_16_344;
                case 348: return "/Country/HU";
                //case 352: goto oid_2_16_352;
                //case 356: goto oid_2_16_356;
                case 360: return "/Country/ID";
                //case 364: goto oid_2_16_364;
                case 368: return "/Country/IQ";
                case 372: return "/Country/IE";
                case 376: return "/Country/IL";
                //case 380: goto oid_2_16_380;
                case 384: return "/Country/CI";
                //case 388: goto oid_2_16_388;
                case 392: return "/Country/JP";
                case 398: return "/Country/KZ";
                case 400: return "/Country/JO";
                case 404: return "/Country/KE";
                case 408: return "/Country/KP";
                case 410: return "/Country/KR";
                case 414: return "/Country/KW";
                case 417: return "/Country/KG";
                case 418: return "/Country/LA";
                case 422: return "/Country/LB";
                case 426: return "/Country/LS";
                case 428: return "/Country/LV";
                case 430: return "/Country/LR";
                case 434: return "/Country/LY";
                case 438: return "/Country/LI";
                case 440: return "/Country/LT";
                case 442: return "/Country/LU";
                case 450: return "/Country/MG";
                case 454: return "/Country/MW";
                case 458: return "/Country/MY";
                case 462: return "/Country/MV";
                case 466: return "/Country/ML";
                //case 470: goto oid_2_16_470;
                case 478: return "/Country/MR";
                case 480: return "/Country/MU";
                case 484: return "/Country/MX";
                case 492: return "/Country/MC";
                case 496: return "/Country/MN";
                case 498: return "/Country/MD";
                case 499: return "/Country/ME";
                case 504: return "/Country/MA";
                case 508: return "/Country/MZ";
                case 512: return "/Country/OM";
                case 516: return "/Country/NA";
                case 520: return "/Country/NR";
                case 524: return "/Country/NP";
                //case 528: goto oid_2_16_528;
                case 530: return "/Country/AN";
                case 548: return "/Country/VU";
                //case 554: goto oid_2_16_554;
                case 558: return "/Country/NI";
                case 562: return "/Country/NE";
                case 566: return "/Country/NG";
                //case 578: goto oid_2_16_578;
                case 583: return "/Country/FM";
                case 584: return "/Country/MH";
                case 585: return "/Country/PW";
                case 586: return "/Country/PK";
                //case 591: goto oid_2_16_591;
                case 598: return "/Country/PG";
                case 600: return "/Country/PY";
                case 604: return "/Country/PE";
                case 608: return "/Country/PH";
                case 616: return "/Country/PL";
                //case 620: goto oid_2_16_620;
                case 624: return "/Country/GW";
                case 626: return "/Country/TL";
                case 634: return "/Country/QA";
                case 642: return "/Country/RO";
                case 643: return "/Country/RU";
                case 646: return "/Country/RW";
                case 659: return "/Country/KN";
                case 662: return "/Country/LC";
                case 670: return "/Country/VC";
                case 674: return "/Country/SM";
                case 678: return "/Country/ST";
                //case 682: goto oid_2_16_682;
                case 686: return "/Country/SN";
                case 688: return "/Country/RS";
                case 690: return "/Country/SC";
                case 694: return "/Country/SL";
                case 702: return "/Country/SG";
                case 703: return "/Country/SK";
                case 704: return "/Country/VN";
                case 705: return "/Country/SI";
                case 706: return "/Country/SO";
                case 710: return "/Country/ZA";
                case 716: return "/Country/ZW";
                //case 724: goto oid_2_16_724;
                case 728: return "/Country/SS";
                case 729: return "/Country/SD";
                case 736: return "/Country/[Sudan (old code \"retired\")";
                case 740: return "/Country/SR";
                case 748: return "/Country/SZ";
                case 752: return "/Country/SE";
                //case 756: goto oid_2_16_756;
                case 760: return "/Country/SY";
                case 762: return "/Country/TJ";
                //case 764: goto oid_2_16_764;
                case 768: return "/Country/TG";
                case 776: return "/Country/TO";
                case 780: return "/Country/TT";
                //case 784: goto oid_2_16_784;
                //case 788: goto oid_2_16_788;
                //case 792: goto oid_2_16_792;
                case 795: return "/Country/TM";
                case 798: return "/Country/TV";
                case 800: return "/Country/UG";
                case 804: return "/Country/UA";
                case 807: return "/Country/MK";
                case 818: return "/Country/EG";
                case 826: return "/Country/GB";
                case 834: return "/Country/TZ";
                //case 840: goto oid_2_16_840;
                case 854: return "/Country/BF";
                //case 858: goto oid_2_16_858;
                case 860: return "/Country/UZ";
                case 862: return "/Country/VE";
                case 882: return "/Country/WS";
                //case 886: goto oid_2_16_886;
                case 887: return "/Country/YE";
                case 891: return "/Country/[Serbia and Montenegro (code not in current use)]";
                case 894: return "/Country/ZM";
                default: return $"/Country/{values[index - 1]}";
            }

        #endregion

        // registration-procedures
        #region 2.17.*

        oid_2_17:

            if (index == values.Length) return "/Joint-ISO-ITU-T/Registration-Procedures";
            switch (values[index++])
            {
                //TODO: case 1: goto oid_2_17_1;
                //TODO: case 2: goto oid_2_17_2;
                case 3: return "/Joint-ISO-ITU-T/Registration-Procedures/[Registration procedures for the registration authority of international ASN.1 names]";
                case 5: return "/Joint-ISO-ITU-T/Registration-Procedures/[Registration procedures for the registration authority of international ADministration Management Domain (ADMD) alphanumeric names and international PRivate Management Domain (PRMD) alphanumeric names for Originator/Recipient (O/R) Rec. ITU-T X.400 addresses]";
                case 6: return "/Joint-ISO-ITU-T/Registration-Procedures/[Registration procedures for the registration authority of international organization alphanumeric names for use in Rec. ITU-T X.500 directory distinguished names]";
                default: return $"/Joint-ISO-ITU-T/Registration-Procedures/{values[index - 1]}";
            }

        #endregion

        // physical-layer, physical-layer-management
        #region 2.18.*

        oid_2_18:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Physical layer management]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_18_0;
                default: return $"/Joint-ISO-ITU-T/[Physical layer management]/{values[index - 1]}";
            }

        #endregion

        // mheg
        #region 2.19.*

        oid_2_19:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Multimedia and Hypermedia information coding Expert Group (MHEG)]";
            switch (values[index++])
            {
                //TODO: case 1: goto oid_2_19_1;
                default: return $"/Joint-ISO-ITU-T/[Multimedia and Hypermedia information coding Expert Group (MHEG)]/{values[index - 1]}";
            }

        #endregion

        // genericULS, generic-upper-layers-security, guls
        #region 2.20.*

        oid_2_20:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Generic Upper Layers Security (GULS)]";
            switch (values[index++])
            {
                //TODO: case 1: goto oid_2_20_1;
                case 2: return "/Joint-ISO-ITU-T/[Generic Upper Layers Security (GULS)]/[General transfer syntax]";
                //TODO: case 3: goto oid_2_20_3;
                //TODO: case 4: goto oid_2_20_4;
                //TODO: case 5: goto oid_2_20_5;
                //TODO: case 7: goto oid_2_20_7;
                default: return $"/Joint-ISO-ITU-T/[Generic Upper Layers Security (GULS)]/{values[index - 1]}";
            }

        #endregion

        // transport-layer-security-protocol
        #region 2.21.*

        oid_2_21:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Transport layer security protocol]";
            switch (values[index++])
            {
                //TODO: case 1: goto oid_2_21_1;
                default: return $"/Joint-ISO-ITU-T/[Transport layer security protocol]/{values[index - 1]}";
            }

        #endregion

        // network-layer-security-protocol
        #region 2.22.*

        oid_2_22:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Network layer security protocol]";
            switch (values[index++])
            {
                //TODO: case 1: goto oid_2_22_1;
                default: return $"/Joint-ISO-ITU-T/[Network layer security protocol]/{values[index - 1]}";
            }

        #endregion

        // international-organizations
        #region 2.23.*

        oid_2_23:

            if (index == values.Length) return "/Joint-ISO-ITU-T/International-Organizations";
            switch (values[index++])
            {
                //TODO: case 42: goto oid_2_23_42;
                //TODO: case 43: goto oid_2_23_43;
                case 128: return "/Joint-ISO-ITU-T/International-Organizations/[Teleglobe, Inc.]";
                case 129: return "/Joint-ISO-ITU-T/International-Organizations/[Key Recovery Alliance]";
                case 130: return "/Joint-ISO-ITU-T/International-Organizations/[Object Management Group]";
                case 131: return "/Joint-ISO-ITU-T/International-Organizations/[Visa International]";
                case 132: return "/Joint-ISO-ITU-T/International-Organizations/[Comprehensive nuclear-Test-Ban Treaty Organization (CTBTO) Public-Key Infrastructure (PKI)]";
                //TODO: case 133: goto oid_2_23_133;
                case 134: return "/Joint-ISO-ITU-T/International-Organizations/[Ceska Posta s.p.]";
                case 135: return "/Joint-ISO-ITU-T/International-Organizations/[\"HBOS Plc\"]";
                //TODO: case 136: goto oid_2_23_136;
                case 137: return "/Joint-ISO-ITU-T/International-Organizations/[Comrad Medical Systems]";
                case 138: return "/Joint-ISO-ITU-T/International-Organizations/[International Atomic Energy Agency (IAEA)]";
                case 139: return "/Joint-ISO-ITU-T/International-Organizations/[British Sky Broadcasting Group]";
                //TODO: case 140: goto oid_2_23_140;
                case 141: return "/Joint-ISO-ITU-T/International-Organizations/[\"WAC\" Application Services Ltd.]";
                //TODO: case 143: goto oid_2_23_143;
                case 144: return "/Joint-ISO-ITU-T/International-Organizations/[Directorate General of The General Security of Lebanon]";
                //TODO: case 146: goto oid_2_23_146;
                case 147: return "/Joint-ISO-ITU-T/International-Organizations/[Peripheral Component Interconnect Special Interest Group (PCI-SIG) component measurement and authorization]";
                default: return $"/Joint-ISO-ITU-T/International-Organizations/{values[index - 1]}";
            }

        #endregion

        // sios
        #region 2.24.*

        oid_2_24:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Security Information Objects (SIOs) for access control]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_24_0;
                default: return $"/Joint-ISO-ITU-T/[Security Information Objects (SIOs) for access control]/{values[index - 1]}";
            }

        #endregion

        // uuid [TODO: Requires 128-bit values]
        #region 2.25.*

        oid_2_25:

            if (index == values.Length) return "/Joint-ISO-ITU-T/UUID";
            switch (values[index++])
            {
                case 0: return "/Joint-ISO-ITU-T/UUID/00000000-0000-0000-0000-000000000000";
                //case 288786655511405443130567505384701230: return "/Joint-ISO-ITU-T/UUID/00379e48-0a2b-1085-b288-0002a5d5fd2e";
                //case 987895962269883002155146617097157934: return "/Joint-ISO-ITU-T/UUID/00be4308-0c89-1085-8ea0-0002a5d5fd2e";
                //case 1858228783942312576083372383319475483: return "/Joint-ISO-ITU-T/UUID/0165e1c0-a655-11e0-95b8-0002a5d5c51b";
                //case 2474299330026746002885628159579243803: return "/Joint-ISO-ITU-T/UUID/01dc8860-25fb-11da-82b2-0002a5d5c51b";
                //case 3263645701162998421821186056373271854: return "/Joint-ISO-ITU-T/UUID/02748e28-08c4-1085-b21d-0002a5d5fd2e";
                //case 3325839809379844461264382260940242222: return "/Joint-ISO-ITU-T/UUID/02808890-0ad8-1085-9bdf-0002a5d5fd2e";
                // TODO: Left off at http://www.oid-info.com/get/2.25.3664154270495270126161055518190585115
                default: return $"/Joint-ISO-ITU-T/UUID/{values[index - 1]}";
            }

        #endregion

        // odp
        #region 2.26.*

        oid_2_26:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Information technology -- Open Distributed Processing (ODP)]";
            switch (values[index++])
            {
                case 0: goto oid_2_26_0;
                default: return $"/Joint-ISO-ITU-T/[Information technology -- Open Distributed Processing (ODP)]/{values[index - 1]}";
            }

        // trader
        #region 2.26.0.*

        oid_2_26_0:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Information technology -- Open Distributed Processing (ODP)]/[Open Distributed Processing -- Trading Function: Provision of trading function using OSI Directory service]";
            switch (values[index++])
            {
                //TODO: case 2: goto oid_2_26_0_2;
                //TODO: case 4: goto oid_2_26_0_4;
                //TODO: case 6: goto oid_2_26_0_6;
                //TODO: case 13: goto oid_2_26_0_13;
                //TODO: case 15: goto oid_2_26_0_15;
                default: return $"/Joint-ISO-ITU-T/[Information technology -- Open Distributed Processing (ODP)]/[Open Distributed Processing -- Trading Function: Provision of trading function using OSI Directory service]/{values[index - 1]}";
            }

        #endregion

        #endregion

        // tag-based, nid
        #region 2.27.*

        oid_2_27:

            if (index == values.Length) return "/Tag-Based";
            switch (values[index++])
            {
                case 1: return "/Tag-Based/[mCode, micro-mCode and mini-mCode for mobile RFID services]";
                case 2: return "/Tag-Based/[\"ucode\" identification scheme]";
                default: return $"/Tag-Based/{values[index - 1]}";
            }

        #endregion

        // its
        #region 2.28.*

        oid_2_28:

            if (index == values.Length) return "/Joint-ISO-ITU-T/ITS";
            switch (values[index++])
            {
                case 0: goto oid_2_28_0;
                case 3: return "/Joint-ISO-ITU-T/ITS/[fieldDevice]";
                case 4: return "/Joint-ISO-ITU-T/ITS/[fdVms]";
                case 5: return "/Joint-ISO-ITU-T/ITS/[Graphic Data Dictionary (GDD) codes]";
                default: return $"/Joint-ISO-ITU-T/ITS/{values[index - 1]}";
            }

        // its-misc
        #region 2.28.0.*

        oid_2_28_0:

            if (index == values.Length) return "/Joint-ISO-ITU-T/ITS/[Intelligent Transport Systems (ITS) data concepts that do not relate to internationally standardized object classes]";
            switch (values[index++])
            {
                case 1: return "/Joint-ISO-ITU-T/ITS/[Intelligent Transport Systems (ITS) data concepts that do not relate to internationally standardized object classes]/[ISO member bodies]";
                case 2: return "/Joint-ISO-ITU-T/ITS/[Intelligent Transport Systems (ITS) data concepts that do not relate to internationally standardized object classes]/[Standard development organizations]";
                //TODO: case 3: goto oid_2_28_0_3;
                //TODO: case 4: goto oid_2_28_0_4;
                case 50: return "/Joint-ISO-ITU-T/ITS/[Intelligent Transport Systems (ITS) data concepts that do not relate to internationally standardized object classes]/[Private entities, such as corporations]";
                default: return $"/Joint-ISO-ITU-T/ITS/[Intelligent Transport Systems (ITS) data concepts that do not relate to internationally standardized object classes]/{values[index - 1]}";
            }

        #endregion

        #endregion

        // upu
        #region 2.40.*

        oid_2_40:

            if (index == values.Length) return "/Joint-ISO-ITU-T/[Universal Postal Union (UPU)]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_40_0;
                case 2: return "/Joint-ISO-ITU-T/[Universal Postal Union (UPU)]/[Member bodies (postal administrations)]";
                //TODO: case 3: goto oid_2_40_3;
                default: return $"/Joint-ISO-ITU-T/[Universal Postal Union (UPU)]/{values[index - 1]}";
            }

        #endregion

        // bip
        #region 2.41.*

        oid_2_41:

            if (index == values.Length) return "/BIP";
            switch (values[index++])
            {
                case 0: goto oid_2_41_0;
                default: return $"/BIP/{values[index - 1]}";
            }

        // bip
        #region 2.41.0.*

        oid_2_41_0:

            if (index == values.Length) return "/BIP/[ASN.1 modules]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_41_0_0;
                //TODO: case 1: goto oid_2_41_0_1;
                //TODO: case 2: goto oid_2_41_0_2;
                default: return $"/BIP/[ASN.1 modules]/{values[index - 1]}";
            }

        #endregion

        #endregion

        // telebiometrics
        #region 2.42.*

        oid_2_42:

            if (index == values.Length) return "/Telebiometrics";
            switch (values[index++])
            {
                case 0: goto oid_2_42_0;
                case 1: goto oid_2_42_1;
                case 2: goto oid_2_42_2;
                case 3: goto oid_2_42_3;
                case 10: goto oid_2_42_10;
                default: return $"/Telebiometrics/{values[index - 1]}";
            }

        // modules
        #region 2.42.0.*

        oid_2_42_0:

            if (index == values.Length) return "/Telebiometrics/Modules";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_42_0_0;
                default: return $"/Telebiometrics/Modules/{values[index - 1]}";
            }

        #endregion

        // tmm
        #region 2.42.1.*

        oid_2_42_1:

            if (index == values.Length) return "/Telebiometrics/TMM";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_42_1_0;
                //TODO: case 1: goto oid_2_42_1_1;
                //TODO: case 2: goto oid_2_42_1_2;
                //TODO: case 3: goto oid_2_42_1_3;
                //TODO: case 4: goto oid_2_42_1_4;
                default: return $"/Telebiometrics/TMM/{values[index - 1]}";
            }

        #endregion

        // human-physiology
        #region 2.42.2.*

        oid_2_42_2:

            if (index == values.Length) return "/Telebiometrics/Human_Physiology";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_42_2_0;
                //TODO: case 1: goto oid_2_42_2_1;
                //TODO: case 2: goto oid_2_42_2_2;
                default: return $"/Telebiometrics/Human_Physiology/{values[index - 1]}";
            }

        #endregion

        // obj-cat, telehealth, e-health-protocol, th
        #region 2.42.3.*

        oid_2_42_3:

            if (index == values.Length) return "/Telebiometrics/E_Health_Protocol";
            switch (values[index++])
            {
                case 0: goto oid_2_42_3_0;
                case 1: return "/Telebiometrics/E_Health_Protocol/[Patient schemes]";
                case 2: return "/Telebiometrics/E_Health_Protocol/[Medical staff schemes]";
                case 3: return "/Telebiometrics/E_Health_Protocol/[Observer schemes]";
                case 4: return "/Telebiometrics/E_Health_Protocol/[Pharmaceutical schemes]";
                case 5: return "/Telebiometrics/E_Health_Protocol/[Laboratory schemes]";
                case 6: return "/Telebiometrics/E_Health_Protocol/[Drug manufacturer schemes]";
                case 7: return "/Telebiometrics/E_Health_Protocol/[Medical device schemes]";
                case 8: return "/Telebiometrics/E_Health_Protocol/[Medical software schemes]";
                case 9: return "/Telebiometrics/E_Health_Protocol/[Medical insurance schemes]";
                case 10: return "/Telebiometrics/E_Health_Protocol/[Medical record schemes]";
                default: return $"/Telebiometrics/E_Health_Protocol/{values[index - 1]}";
            }

        // obj-cat, telehealth, e-health-protocol, th
        #region 2.42.3.0.*

        oid_2_42_3_0:

            if (index == values.Length) return "/Telebiometrics/E_Health_Protocol/Modules";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_42_3_0_0;
                //TODO: case 1: goto oid_2_42_3_0_1;
                //TODO: case 2: goto oid_2_42_3_0_2;
                //TODO: case 3: goto oid_2_42_3_0_3;
                //TODO: case 4: goto oid_2_42_3_0_4;
                //TODO: case 5: goto oid_2_42_3_0_5;
                //TODO: case 6: goto oid_2_42_3_0_6;
                //TODO: case 10: goto oid_2_42_3_0_10;
                default: return $"/Telebiometrics/E_Health_Protocol/Modules/{values[index - 1]}";
            }

        #endregion

        #endregion

        // thprot
        #region 2.42.10.*

        oid_2_42_10:

            if (index == values.Length) return "/Telebiometrics/[e-Health and world-wide telemedicines]";
            switch (values[index++])
            {
                //TODO: case 0: goto oid_2_42_10_0;
                //TODO: case 1: goto oid_2_42_10_1;
                //TODO: case 2: goto oid_2_42_10_2;
                default: return $"/Telebiometrics/[e-Health and world-wide telemedicines]/{values[index - 1]}";
            }

        #endregion

        #endregion

        // cybersecurity
        #region 2.48.*

        oid_2_48:

            if (index == values.Length) return "/Cybersecurity";
            switch (values[index++])
            {
                case 1: return "/Cybersecurity/Country";
                case 2: return "/Cybersecurity/International-Org";
                default: return $"/Cybersecurity/{values[index - 1]}";
            }

        #endregion

        // alerting
        #region 2.49.*

        oid_2_49:

            if (index == values.Length) return "/Alerting";
            switch (values[index++])
            {
                case 0: goto oid_2_49_0;
                default: return $"/Alerting/{values[index - 1]}";
            }

        // wmo
        #region 2.49.0.*

        oid_2_49_0:

            if (index == values.Length) return "/Alerting/WMO";
            switch (values[index++])
            {
                case 0: goto oid_2_49_0_0;
                case 1: return "/Alerting/WMO/[Alerting messages of countries]";
                //TODO: case 2: goto oid_2_49_0_2;
                //TODO: case 3: goto oid_2_49_0_3;
                default: return $"/Alerting/WMO/{values[index - 1]}";
            }

        // authority
        #region 2.49.0.0.*

        oid_2_49_0_0:

            if (index == values.Length) return "/Alerting/WMO/[Alerting authorities of countries]";
            switch (values[index++])
            {
                //TODO: case 4: goto oid_2_49_0_0_4;
                //TODO: case 8: goto oid_2_49_0_0_8;
                //TODO: case 12: goto oid_2_49_0_0_12;
                //TODO: case 20: goto oid_2_49_0_0_20;
                //TODO: case 24: goto oid_2_49_0_0_24;
                //TODO: case 28: goto oid_2_49_0_0_28;
                //TODO: case 31: goto oid_2_49_0_0_31;
                //TODO: case 32: goto oid_2_49_0_0_32;
                //TODO: case 36: goto oid_2_49_0_0_36;
                //TODO: case 40: goto oid_2_49_0_0_40;
                //TODO: case 44: goto oid_2_49_0_0_44;
                //TODO: case 48: goto oid_2_49_0_0_48;
                //TODO: case 50: goto oid_2_49_0_0_50;
                //TODO: case 51: goto oid_2_49_0_0_51;
                //TODO: case 52: goto oid_2_49_0_0_52;
                //TODO: case 56: goto oid_2_49_0_0_56;
                case 60: return "/Alerting/WMO/[Alerting authorities of countries]/[Alerting authorities of Bermuda]";
                //TODO: case 64: goto oid_2_49_0_0_64;
                //TODO: case 68: goto oid_2_49_0_0_68;
                //TODO: case 70: goto oid_2_49_0_0_70;
                //TODO: case 72: goto oid_2_49_0_0_72;
                //TODO: case 76: goto oid_2_49_0_0_76;
                //TODO: case 84: goto oid_2_49_0_0_84;
                //TODO: case 90: goto oid_2_49_0_0_90;
                //TODO: case 92: goto oid_2_49_0_0_92;
                //TODO: case 96: goto oid_2_49_0_0_96;
                //TODO: case 100: goto oid_2_49_0_0_100;
                //TODO: case 104: goto oid_2_49_0_0_104;
                //TODO: case 108: goto oid_2_49_0_0_108;
                //TODO: case 112: goto oid_2_49_0_0_112;
                //TODO: case 116: goto oid_2_49_0_0_116;
                //TODO: case 120: goto oid_2_49_0_0_120;
                //TODO: case 124: goto oid_2_49_0_0_124;
                //TODO: case 132: goto oid_2_49_0_0_132;
                //TODO: case 136: goto oid_2_49_0_0_136;
                //TODO: case 140: goto oid_2_49_0_0_140;
                //TODO: case 144: goto oid_2_49_0_0_144;
                //TODO: case 148: goto oid_2_49_0_0_148;
                //TODO: case 152: goto oid_2_49_0_0_152;
                //TODO: case 156: goto oid_2_49_0_0_156;
                //TODO: case 170: goto oid_2_49_0_0_170;
                //TODO: case 174: goto oid_2_49_0_0_174;
                //TODO: case 178: goto oid_2_49_0_0_178;
                //TODO: case 180: goto oid_2_49_0_0_180;
                //TODO: case 184: goto oid_2_49_0_0_184;
                //TODO: case 188: goto oid_2_49_0_0_188;
                //TODO: case 191: goto oid_2_49_0_0_191;
                //TODO: case 192: goto oid_2_49_0_0_192;
                //TODO: case 196: goto oid_2_49_0_0_196;
                //TODO: case 203: goto oid_2_49_0_0_203;
                //TODO: case 204: goto oid_2_49_0_0_204;
                //TODO: case 208: goto oid_2_49_0_0_208;
                //TODO: case 212: goto oid_2_49_0_0_212;
                //TODO: case 214: goto oid_2_49_0_0_214;
                //TODO: case 218: goto oid_2_49_0_0_218;
                //TODO: case 222: goto oid_2_49_0_0_222;
                //TODO: case 231: goto oid_2_49_0_0_231;
                //TODO: case 232: goto oid_2_49_0_0_232;
                //TODO: case 233: goto oid_2_49_0_0_233;
                //TODO: case 242: goto oid_2_49_0_0_242;
                //TODO: case 246: goto oid_2_49_0_0_246;
                //TODO: case 250: goto oid_2_49_0_0_250;
                //TODO: case 258: goto oid_2_49_0_0_258;
                //TODO: case 262: goto oid_2_49_0_0_262;
                //TODO: case 266: goto oid_2_49_0_0_266;
                //TODO: case 268: goto oid_2_49_0_0_268;
                //TODO: case 270: goto oid_2_49_0_0_270;
                //TODO: case 276: goto oid_2_49_0_0_276;
                //TODO: case 288: goto oid_2_49_0_0_288;
                //TODO: case 296: goto oid_2_49_0_0_296;
                //TODO: case 300: goto oid_2_49_0_0_300;
                //TODO: case 308: goto oid_2_49_0_0_308;
                //TODO: case 320: goto oid_2_49_0_0_320;
                //TODO: case 324: goto oid_2_49_0_0_324;
                //TODO: case 328: goto oid_2_49_0_0_328;
                //TODO: case 332: goto oid_2_49_0_0_332;
                //TODO: case 340: goto oid_2_49_0_0_340;
                //TODO: case 344: goto oid_2_49_0_0_344;
                //TODO: case 348: goto oid_2_49_0_0_348;
                //TODO: case 352: goto oid_2_49_0_0_352;
                //TODO: case 356: goto oid_2_49_0_0_356;
                //TODO: case 360: goto oid_2_49_0_0_360;
                //TODO: case 364: goto oid_2_49_0_0_364;
                //TODO: case 368: goto oid_2_49_0_0_368;
                //TODO: case 372: goto oid_2_49_0_0_372;
                //TODO: case 376: goto oid_2_49_0_0_376;
                //TODO: case 380: goto oid_2_49_0_0_380;
                //TODO: case 384: goto oid_2_49_0_0_384;
                //TODO: case 388: goto oid_2_49_0_0_388;
                //TODO: case 392: goto oid_2_49_0_0_392;
                //TODO: case 398: goto oid_2_49_0_0_398;
                // TODO: Left off at http://www.oid-info.com/get/2.49.0.0.400
                default: return $"/Alerting/WMO/[Alerting authorities of countries]/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        // ors
        #region 2.50.*

        oid_2_50:

            if (index == values.Length) return "/OIDResolutionSystem";
            switch (values[index++])
            {
                case 0: goto oid_2_50_0;
                default: return $"/OIDResolutionSystem/{values[index - 1]}";
            }

        // modules
        #region 2.50.0.*

        oid_2_50_0:

            if (index == values.Length) return "/OIDResolutionSystem/[ASN.1 modules]";
            switch (values[index++])
            {
                case 0: goto oid_2_50_0_0;
                case 1: goto oid_2_50_0_1;
                default: return $"/OIDResolutionSystem/[ASN.1 modules]/{values[index - 1]}";
            }

        // cinf
        #region 2.50.0.0.*

        oid_2_50_0_0:

            if (index == values.Length) return "/OIDResolutionSystem/[ASN.1 modules]/[CINF-module]";
            switch (values[index++])
            {
                case 1: return "/OIDResolutionSystem/[ASN.1 modules]/[CINF-module]/[Version 1]";
                default: return $"/OIDResolutionSystem/[ASN.1 modules]/[CINF-module]/{values[index - 1]}";
            }

        #endregion

        // rinf
        #region 2.50.0.1.*

        oid_2_50_0_1:

            if (index == values.Length) return "/OIDResolutionSystem/[ASN.1 modules]/[RINF-module]";
            switch (values[index++])
            {
                case 1: return "/OIDResolutionSystem/[ASN.1 modules]/[RINF-module]/[Version 1]";
                default: return $"/OIDResolutionSystem/[ASN.1 modules]/[RINF-module]/{values[index - 1]}";
            }

        #endregion

        #endregion

        #endregion

        // gs1
        #region 2.51.*

        oid_2_51:

            if (index == values.Length) return "/GS1";
            switch (values[index++])
            {
                case 1: goto oid_2_51_1;
                case 2: goto oid_2_51_2;
                case 3: return "/GS1/[GS1 business data]";
                case 4: return "/GS1/[GS1 technical data]";
                default: return $"/GS1/{values[index - 1]}";
            }

        // 1
        #region 2.51.1.*

        oid_2_51_1:

            if (index == values.Length) return "/GS1/[GS1 identification keys]";
            switch (values[index++])
            {
                case 1: return "/GS1/[GS1 identification keys]/[Global Trade Item Number (the GS1 Identification Key used to identify trade items)]";
                case 2: return "/GS1/[GS1 identification keys]/[Serial Shipping Container Code (GS1 Identification Key used to identify logistics units)]";
                case 3: return "/GS1/[GS1 identification keys]/[Global Location Number (GS1 Identification Key used to identify physical locations or parties)]";
                case 4: return "/GS1/[GS1 identification keys]/[Global Returnable Asset Identifier (GS1 Identification Key used to identify Returnable Assets)]";
                case 5: return "/GS1/[GS1 identification keys]/[Global Individual Asset Identifier (GS1 Identification Key used to identify an Individual Asset)]";
                case 6: return "/GS1/[GS1 identification keys]/[Global Document Type Identifier (GS1 Identification Key used to identify a document type)]";
                case 7: return "/GS1/[GS1 identification keys]/[Global Service Relation Number (a non-significant number used to identify the relationship between an organization offering services and the individual entities providing or benefiting from the services)]";
                case 8: return "/GS1/[GS1 identification keys]/[Global Shipment Identification Number (GSIN)]";
                case 9: return "/GS1/[GS1 identification keys]/[Global Identification Number for Consignment (GS1 Identification Key used to identify a logical grouping of logistic or transport units that are assembled to be transported under one transport document, e.g., HWB)]";
                case 10: return "/GS1/[GS1 identification keys]/[Global Coupon Number (GS1 Identification Key that provides a globally unique identification for a coupon, with an optional serial number)]";
                case 11: return "/GS1/[GS1 identification keys]/[Component or Part IDentifier (CPID)]";
                case 12: return "/GS1/[GS1 identification keys]/[Global Model Number (GMN) used to identify a product model]";
                default: return $"/GS1/[GS1 identification keys]/{values[index - 1]}";
            }

        #endregion

        // 2
        #region 2.51.2.*

        oid_2_51_2:

            if (index == values.Length) return "/GS1/[GS1 supplementary data]";
            switch (values[index++])
            {
                case 10: return "/GS1/[GS1 supplementary data]/[Number that associates an item with information the manufacturer considers relevant for traceability purposes]";
                case 21: return "/GS1/[GS1 supplementary data]/[Numeric or alphanumeric code assigned to an individual instance of an entity for its lifetime]";
                case 254: return "/GS1/[GS1 supplementary data]/[Alphanumeric extension component used to identify internal physical locations within a location]";
                case 8011: return "/GS1/[GS1 supplementary data]/[Numeric identifier assigned to an individual instance of a component/part]";
                case 8019: return "/GS1/[GS1 supplementary data]/[Numeric identifier allowing to distinguish different encounters during a service relationship]";
                default: return $"/GS1/[GS1 supplementary data]/{values[index - 1]}";
            }

        #endregion

        #endregion

        // uav
        #region 2.52.*

        oid_2_52:

            if (index == values.Length) return "/Joint-ISO-ITU-T/UAV";
            switch (values[index++])
            {
                case 1: return "/Joint-ISO-ITU-T/UAV/[Unmanned Aerial Vehicle (UAV) devices]";
                case 2: return "/Joint-ISO-ITU-T/UAV/[Ground control stations]";
                case 3: return "/Joint-ISO-ITU-T/UAV/[Monitoring devices]";
                case 4: goto oid_2_52_4;
                case 5: return "/Joint-ISO-ITU-T/UAV/[Security modules]";
                case 6: goto oid_2_52_6;
                default: return $"/Joint-ISO-ITU-T/UAV/{values[index - 1]}";
            }

        // 4
        #region 2.52.4*

        oid_2_52_4:

            if (index == values.Length) return "/Joint-ISO-ITU-T/UAV/[Data modules for the full life-cycle management of Unmanned Aerial Vehicles (UAVs)]";
            switch (values[index++])
            {
                case 1: return "/Joint-ISO-ITU-T/UAV/[Data modules for the full life-cycle management of Unmanned Aerial Vehicles (UAVs)]/[Data modules of a manufacturing system]";
                case 2: return "/Joint-ISO-ITU-T/UAV/[Data modules for the full life-cycle management of Unmanned Aerial Vehicles (UAVs)]/[Data modules of a sales and logistical system]";
                case 3: return "/Joint-ISO-ITU-T/UAV/[Data modules for the full life-cycle management of Unmanned Aerial Vehicles (UAVs)]/[Data modules of a repairing system]";
                case 4: return "/Joint-ISO-ITU-T/UAV/[Data modules for the full life-cycle management of Unmanned Aerial Vehicles (UAVs)]/[Data modules of a scrapping system]";
                default: return $"/Joint-ISO-ITU-T/UAV/[Data modules for the full life-cycle management of Unmanned Aerial Vehicles (UAVs)]/{values[index - 1]}";
            }

        #endregion

        // 6
        #region 2.52.6*

        oid_2_52_6:

            if (index == values.Length) return "/Joint-ISO-ITU-T/UAV/[Data modules for identity recognition of Unmanned Aerial Vehicles (UAVs)]";
            switch (values[index++])
            {
                case 1: return "/Joint-ISO-ITU-T/UAV/[Data modules for identity recognition of Unmanned Aerial Vehicles (UAVs)]/[Data modules of Unmanned Aerial Vehicle (UAV) systems]";
                case 2: return "/Joint-ISO-ITU-T/UAV/[Data modules for identity recognition of Unmanned Aerial Vehicles (UAVs)]/[Data modules of Unmanned Aerial Vehicle (UAV) Ground Control Station (GCS) systems]";
                case 3: return "/Joint-ISO-ITU-T/UAV/[Data modules for identity recognition of Unmanned Aerial Vehicles (UAVs)]/[Data modules of Unmanned Aerial Vehicle (UAV) Monitoring and Control Station/server (MCS) systems]";
                case 4: return "/Joint-ISO-ITU-T/UAV/[Data modules for identity recognition of Unmanned Aerial Vehicles (UAVs)]/[Data modules of Unmanned Aerial Vehicle (UAV) monitoring cloud systems]";
                default: return $"/Joint-ISO-ITU-T/UAV/[Data modules for identity recognition of Unmanned Aerial Vehicles (UAVs)]/{values[index - 1]}";
            }

            #endregion

            #endregion

            #endregion
        }

        #endregion
    }
}
