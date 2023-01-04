using static BurnOutSharp.Compression.ADPCM.Constants;
using static BurnOutSharp.Compression.ADPCM.Helper;

namespace BurnOutSharp.Compression.ADPCM
{
    public unsafe class Decompressor
    {
        /// <summary>
        /// Decompression routine
        /// </summary>
        /// <see href="https://github.com/ladislav-zezula/StormLib/blob/master/src/adpcm/adpcm.cpp"/>
        public int DecompressADPCM(void* pvOutBuffer, int cbOutBuffer, void* pvInBuffer, int cbInBuffer, int ChannelCount)
        {
            TADPCMStream os = new TADPCMStream(pvOutBuffer, cbOutBuffer);          // Output stream
            TADPCMStream @is = new TADPCMStream(pvInBuffer, cbInBuffer);            // Input stream
            byte EncodedSample = 0;
            byte BitShift = 0;
            short[] PredictedSamples = new short[MAX_ADPCM_CHANNEL_COUNT];    // Predicted sample for each channel
            short[] StepIndexes = new short[MAX_ADPCM_CHANNEL_COUNT];         // Predicted step index for each channel
            int ChannelIndex;                                   // Current channel index

            // Initialize the StepIndex for each channel
            PredictedSamples[0] = PredictedSamples[1] = 0;
            StepIndexes[0] = StepIndexes[1] = INITIAL_ADPCM_STEP_INDEX;

            // The first byte is always zero, the second one contains bit shift (compression level - 1)
            @is.ReadByteSample(ref BitShift);
            @is.ReadByteSample(ref BitShift);

            // Next, InitialSample value for each channel follows
            for (int i = 0; i < ChannelCount; i++)
            {
                // Get the initial sample from the input stream
                short InitialSample = 0;

                // Attempt to read the initial sample
                if (!@is.ReadWordSample(ref InitialSample))
                    return os.LengthProcessed(pvOutBuffer);

                // Store the initial sample to our sample array
                PredictedSamples[i] = InitialSample;

                // Also store the loaded sample to the output stream
                if (!os.WriteWordSample(InitialSample))
                    return os.LengthProcessed(pvOutBuffer);
            }

            // Get the initial index
            ChannelIndex = ChannelCount - 1;

            // Keep reading as long as there is something in the input buffer
            while (@is.ReadByteSample(ref EncodedSample))
            {
                // If we have two channels, we need to flip the channel index
                ChannelIndex = (ChannelIndex + 1) % ChannelCount;

                if (EncodedSample == 0x80)
                {
                    if (StepIndexes[ChannelIndex] != 0)
                        StepIndexes[ChannelIndex]--;

                    if (!os.WriteWordSample(PredictedSamples[ChannelIndex]))
                        return os.LengthProcessed(pvOutBuffer);
                }
                else if (EncodedSample == 0x81)
                {
                    // Modify the step index
                    StepIndexes[ChannelIndex] += 8;
                    if (StepIndexes[ChannelIndex] > 0x58)
                        StepIndexes[ChannelIndex] = 0x58;

                    // Next pass, keep going on the same channel
                    ChannelIndex = (ChannelIndex + 1) % ChannelCount;
                }
                else
                {
                    int StepIndex = StepIndexes[ChannelIndex];
                    int StepSize = StepSizeTable[StepIndex];

                    // Encode one sample
                    PredictedSamples[ChannelIndex] = (short)DecodeSample(PredictedSamples[ChannelIndex],
                                                                         EncodedSample,
                                                                         StepSize,
                                                                         StepSize >> BitShift);

                    // Write the decoded sample to the output stream
                    if (!os.WriteWordSample(PredictedSamples[ChannelIndex]))
                        break;

                    // Calculates the step index to use for the next encode
                    StepIndexes[ChannelIndex] = GetNextStepIndex(StepIndex, EncodedSample);
                }
            }

            // Return total bytes written since beginning of the output buffer
            return os.LengthProcessed(pvOutBuffer);
        }

        /// <summary>
        /// ADPCM decompression present in Starcraft I BETA
        /// </summary>
        /// <see href="https://github.com/ladislav-zezula/StormLib/blob/master/src/adpcm/adpcm.cpp"/>
        public int DecompressADPCM_SC1B(void* pvOutBuffer, int cbOutBuffer, void* pvInBuffer, int cbInBuffer, int ChannelCount)
        {
            TADPCMStream os = new TADPCMStream(pvOutBuffer, cbOutBuffer);          // Output stream
            TADPCMStream @is = new TADPCMStream(pvInBuffer, cbInBuffer);            // Input stream
            ADPCM_DATA AdpcmData = new ADPCM_DATA();
            int[] LowBitValues = new int[MAX_ADPCM_CHANNEL_COUNT];
            int[] UpperBits = new int[MAX_ADPCM_CHANNEL_COUNT];
            int[] BitMasks = new int[MAX_ADPCM_CHANNEL_COUNT];
            int[] PredictedSamples = new int[MAX_ADPCM_CHANNEL_COUNT];
            int ChannelIndex;
            int ChannelIndexMax;
            int OutputSample;
            byte BitCount = 0;
            byte EncodedSample = 0;
            short InputValue16 = 0;
            int reg_eax;
            int Difference;

            // The first byte contains number of bits
            if (!@is.ReadByteSample(ref BitCount))
                return os.LengthProcessed(pvOutBuffer);
            if (InitAdpcmData(AdpcmData, BitCount) == null)
                return os.LengthProcessed(pvOutBuffer);
            
            //assert(AdpcmData.pValues != NULL);

            // Init bit values
            for (int i = 0; i < ChannelCount; i++)
            {
                byte OneByte = 0;

                if (!@is.ReadByteSample(ref OneByte))
                    return os.LengthProcessed(pvOutBuffer);
                LowBitValues[i] = OneByte & 0x01;
                UpperBits[i] = OneByte >> 1;
            }

            //
            for (int i = 0; i < ChannelCount; i++)
            {
                if (!@is.ReadWordSample(ref InputValue16))
                    return os.LengthProcessed(pvOutBuffer);
                BitMasks[i] = InputValue16 << AdpcmData.BitCount;
            }

            // Next, InitialSample value for each channel follows
            for (int i = 0; i < ChannelCount; i++)
            {
                if (!@is.ReadWordSample(ref InputValue16))
                    return os.LengthProcessed(pvOutBuffer);

                PredictedSamples[i] = InputValue16;
                os.WriteWordSample(InputValue16);
            }

            // Get the initial index
            ChannelIndexMax = ChannelCount - 1;
            ChannelIndex = 0;

            // Keep reading as long as there is something in the input buffer
            while (@is.ReadByteSample(ref EncodedSample))
            {
                reg_eax = ((PredictedSamples[ChannelIndex] * 3) << 3) - PredictedSamples[ChannelIndex];
                PredictedSamples[ChannelIndex] = ((reg_eax * 10) + 0x80) >> 8;

                Difference = (((EncodedSample >> 1) + 1) * BitMasks[ChannelIndex] + AdpcmData.field_10) >> AdpcmData.BitCount;

                PredictedSamples[ChannelIndex] = UpdatePredictedSample(PredictedSamples[ChannelIndex], EncodedSample, Difference, 0x01);

                BitMasks[ChannelIndex] = (int)((AdpcmData.pValues[EncodedSample >> 1] * BitMasks[ChannelIndex] + 0x80) >> 6);
                if (BitMasks[ChannelIndex] < AdpcmData.field_8)
                    BitMasks[ChannelIndex] = AdpcmData.field_8;

                if (BitMasks[ChannelIndex] > AdpcmData.field_C)
                    BitMasks[ChannelIndex] = AdpcmData.field_C;

                reg_eax = (cbInBuffer - @is.LengthProcessed(pvInBuffer)) >> ChannelIndexMax;
                OutputSample = PredictedSamples[ChannelIndex];
                if (reg_eax < UpperBits[ChannelIndex])
                {
                    if (LowBitValues[ChannelIndex] != 0)
                    {
                        OutputSample += (UpperBits[ChannelIndex] - reg_eax);
                        if (OutputSample > 32767)
                            OutputSample = 32767;
                    }
                    else
                    {
                        OutputSample += (reg_eax - UpperBits[ChannelIndex]);
                        if (OutputSample < -32768)
                            OutputSample = -32768;
                    }
                }

                // Write the word sample and swap channel
                os.WriteWordSample((short)(OutputSample));
                ChannelIndex = (ChannelIndex + 1) % ChannelCount;
            }

            return os.LengthProcessed(pvOutBuffer);
        }
    }
}