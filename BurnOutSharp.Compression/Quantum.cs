using System.Linq;
using BurnOutSharp.Models.Compression.Quantum;
using static BurnOutSharp.Models.Compression.Quantum.Constants;

namespace BurnOutSharp.Compression
{
    public class Quantum
    {
        // TODO: Implement Quantum decompression

        /// <summary>
        /// Initialize a Quantum model that decodes symbols from s to (s + n - 1)
        /// </summary>
        /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/fdi.c"/>
        public static void InitModel(Model model, ModelSymbol[] symbols, int entryCount, int initialSymbol)
        {
            // Set the basic values
            model.ShiftsLeft = 4;
            model.Entries = entryCount;
            model.Symbols = symbols;

            // Clear out the look-up table
            model.LookupTable = Enumerable.Repeat<ushort>(0xFF, model.LookupTable.Length).ToArray();
        
            // Loop through and build the look-up table
            for (ushort i = 0; i < entryCount; i++)
            {
                // Set up a look-up entry for symbol
                model.LookupTable[i + initialSymbol] = i;

                // Actual symbol
                model.Symbols[i].Symbol = (ushort)(i + initialSymbol);

                // Current frequency of that symbol
                model.Symbols[i].CumulativeFrequency = (ushort)(entryCount - i);
            }

            // Set the last symbol frequency to 0
            model.Symbols[entryCount].CumulativeFrequency = 0;
        }

        /// <summary>
        /// Update the quantum model for a particular symbol
        /// </summary>
        /// <see href="https://github.com/wine-mirror/wine/blob/master/dlls/cabinet/fdi.c"/>
        public static void UpdateModel(Model model, int symbol)
        {
            // Update the cumulative frequency for all symbols less than the provided
            for (int i = 0; i < symbol; i++)
            {
                model.Symbols[i].CumulativeFrequency += 8;
            }

            // If the first symbol still has a cumulative frequency under 3800
            if (model.Symbols[0].CumulativeFrequency <= 3800)
                return;

            // If we have more than 1 shift left in the model
            if (--model.ShiftsLeft != 0)
            {
                // Loop through the entries from highest to lowest,
                // performing the shift on the cumulative frequencies
                for (int i = model.Entries - 1; i >= 0; i--)
                {
                    // -1, not -2; the 0 entry saves this
                    model.Symbols[i].CumulativeFrequency >>= 1;
                    if (model.Symbols[i].CumulativeFrequency <= model.Symbols[i + 1].CumulativeFrequency)
                        model.Symbols[i].CumulativeFrequency = (ushort)(model.Symbols[i + 1].CumulativeFrequency + 1);
                }
            }

            // If we have no shifts left in the model
            else
            {
                // Reset the shifts left value to 50
                model.ShiftsLeft = 50;

                // Loop through the entries setting the cumulative frequencies
                for (int i = 0; i < model.Entries; i++)
                {
                    // No -1, want to include the 0 entry
                    // This converts cumfreqs into frequencies, then shifts right
                    model.Symbols[i].CumulativeFrequency -= model.Symbols[i + 1].CumulativeFrequency;
                    model.Symbols[i].CumulativeFrequency++; // Avoid losing things entirely
                    model.Symbols[i].CumulativeFrequency >>= 1;
                }

                // Now sort by frequencies, decreasing order -- this must be an
                // inplace selection sort, or a sort with the same (in)stability
                // characteristics
                for (int i = 0; i < model.Entries - 1; i++)
                {
                    for (int j = i + 1; j < model.Entries; j++)
                    {
                        if (model.Symbols[i].CumulativeFrequency < model.Symbols[j].CumulativeFrequency)
                        {
                            var temp = model.Symbols[i];
                            model.Symbols[i] = model.Symbols[j];
                            model.Symbols[j] = temp;
                        }
                    }
                }

                // Then convert frequencies back to cumfreq
                for (int i = model.Entries - 1; i >= 0; i--)
                {
                    model.Symbols[i].CumulativeFrequency += model.Symbols[i + 1].CumulativeFrequency;
                }

                // Then update the other part of the table
                for (int i = 0; i < model.Entries; i++)
                {
                    model.LookupTable[model.Symbols[i].Symbol] = (ushort)i;
                }
            }
        }
    }
}