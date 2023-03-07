using static BinaryObjectScanner.Compression.ADPCM.Constants;
using static BinaryObjectScanner.Compression.ADPCM.Helper;

namespace BinaryObjectScanner.Compression.ADPCM
{
    public unsafe class Compressor
    {
        /// <summary>
        /// Compression routine
        /// </summary>
        /// <see href="https://github.com/ladislav-zezula/StormLib/blob/master/src/adpcm/adpcm.cpp"/>
        public int CompressADPCM(void* pvOutBuffer, int cbOutBuffer, void* pvInBuffer, int cbInBuffer, int ChannelCount, int CompressionLevel)
        {
            TADPCMStream os = new TADPCMStream(pvOutBuffer, cbOutBuffer);      // The output stream
            TADPCMStream @is = new TADPCMStream(pvInBuffer, cbInBuffer);        // The input stream
            byte BitShift = (byte)(CompressionLevel - 1);
            short[] PredictedSamples = new short[MAX_ADPCM_CHANNEL_COUNT];// Predicted samples for each channel
            short[] StepIndexes = new short[MAX_ADPCM_CHANNEL_COUNT];     // Step indexes for each channel
            short InputSample = 0;                              // Input sample for the current channel
            int TotalStepSize;
            int ChannelIndex;
            int AbsDifference;
            int Difference;
            int MaxBitMask;
            int StepSize;

            // First byte in the output stream contains zero. The second one contains the compression level
            os.WriteByteSample(0);
            if (!os.WriteByteSample(BitShift))
                return 2;

            // Set the initial step index for each channel
            PredictedSamples[0] = PredictedSamples[1] = 0;
            StepIndexes[0] = StepIndexes[1] = INITIAL_ADPCM_STEP_INDEX;

            // Next, InitialSample value for each channel follows
            for (int i = 0; i < ChannelCount; i++)
            {
                // Get the initial sample from the input stream
                if (!@is.ReadWordSample(ref InputSample))
                    return os.LengthProcessed(pvOutBuffer);

                // Store the initial sample to our sample array
                PredictedSamples[i] = InputSample;

                // Also store the loaded sample to the output stream
                if (!os.WriteWordSample(InputSample))
                    return os.LengthProcessed(pvOutBuffer);
            }

            // Get the initial index
            ChannelIndex = ChannelCount - 1;

            // Now keep reading the input data as long as there is something in the input buffer
            while (@is.ReadWordSample(ref InputSample))
            {
                int EncodedSample = 0;

                // If we have two channels, we need to flip the channel index
                ChannelIndex = (ChannelIndex + 1) % ChannelCount;

                // Get the difference from the previous sample.
                // If the difference is negative, set the sign bit to the encoded sample
                AbsDifference = InputSample - PredictedSamples[ChannelIndex];
                if (AbsDifference < 0)
                {
                    AbsDifference = -AbsDifference;
                    EncodedSample |= 0x40;
                }

                // If the difference is too low (higher that difference treshold),
                // write a step index modifier marker
                StepSize = StepSizeTable[StepIndexes[ChannelIndex]];
                if (AbsDifference < (StepSize >> CompressionLevel))
                {
                    if (StepIndexes[ChannelIndex] != 0)
                        StepIndexes[ChannelIndex]--;

                    os.WriteByteSample(0x80);
                }
                else
                {
                    // If the difference is too high, write marker that
                    // indicates increase in step size
                    while (AbsDifference > (StepSize << 1))
                    {
                        if (StepIndexes[ChannelIndex] >= 0x58)
                            break;

                        // Modify the step index
                        StepIndexes[ChannelIndex] += 8;
                        if (StepIndexes[ChannelIndex] > 0x58)
                            StepIndexes[ChannelIndex] = 0x58;

                        // Write the "modify step index" marker
                        StepSize = StepSizeTable[StepIndexes[ChannelIndex]];
                        os.WriteByteSample(0x81);
                    }

                    // Get the limit bit value
                    MaxBitMask = (1 << (BitShift - 1));
                    MaxBitMask = (MaxBitMask > 0x20) ? 0x20 : MaxBitMask;
                    Difference = StepSize >> BitShift;
                    TotalStepSize = 0;

                    for (int BitVal = 0x01; BitVal <= MaxBitMask; BitVal <<= 1)
                    {
                        if ((TotalStepSize + StepSize) <= AbsDifference)
                        {
                            TotalStepSize += StepSize;
                            EncodedSample |= BitVal;
                        }
                        StepSize >>= 1;
                    }

                    PredictedSamples[ChannelIndex] = (short)UpdatePredictedSample(PredictedSamples[ChannelIndex],
                                                                                  EncodedSample,
                                                                                  Difference + TotalStepSize);
                    // Write the encoded sample to the output stream
                    if (!os.WriteByteSample((byte)EncodedSample))
                        break;

                    // Calculates the step index to use for the next encode
                    StepIndexes[ChannelIndex] = GetNextStepIndex(StepIndexes[ChannelIndex], (uint)EncodedSample);
                }
            }

            return os.LengthProcessed(pvOutBuffer);
        }
    }
}