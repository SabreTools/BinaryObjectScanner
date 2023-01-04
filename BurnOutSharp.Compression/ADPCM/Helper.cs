using static BurnOutSharp.Compression.ADPCM.Constants;

namespace BurnOutSharp.Compression.ADPCM
{
    /// <see href="https://github.com/ladislav-zezula/StormLib/blob/master/src/adpcm/adpcm.cpp"/>
    internal static unsafe class Helper
    {
        #region Local functions

        public static short GetNextStepIndex(int StepIndex, uint EncodedSample)
        {
            // Get the next step index
            StepIndex = StepIndex + NextStepTable[EncodedSample & 0x1F];

            // Don't make the step index overflow
            if (StepIndex < 0)
                StepIndex = 0;
            else if (StepIndex > 88)
                StepIndex = 88;

            return (short)StepIndex;
        }

        public static int UpdatePredictedSample(int PredictedSample, int EncodedSample, int Difference, int BitMask = 0x40)
        {
            // Is the sign bit set?
            if ((EncodedSample & BitMask) != 0)
            {
                PredictedSample -= Difference;
                if (PredictedSample <= -32768)
                    PredictedSample = -32768;
            }
            else
            {
                PredictedSample += Difference;
                if (PredictedSample >= 32767)
                    PredictedSample = 32767;
            }

            return PredictedSample;
        }

        public static int DecodeSample(int PredictedSample, int EncodedSample, int StepSize, int Difference)
        {
            if ((EncodedSample & 0x01) != 0)
                Difference += (StepSize >> 0);

            if ((EncodedSample & 0x02) != 0)
                Difference += (StepSize >> 1);

            if ((EncodedSample & 0x04) != 0)
                Difference += (StepSize >> 2);

            if ((EncodedSample & 0x08) != 0)
                Difference += (StepSize >> 3);

            if ((EncodedSample & 0x10) != 0)
                Difference += (StepSize >> 4);

            if ((EncodedSample & 0x20) != 0)
                Difference += (StepSize >> 5);

            return UpdatePredictedSample(PredictedSample, EncodedSample, Difference);
        }

        #endregion

        #region ADPCM decompression present in Starcraft I BETA

        public static uint[] InitAdpcmData(ADPCM_DATA pData, byte BitCount)
        {
            switch (BitCount)
            {
                case 2:
                    pData.pValues = adpcm_values_2;
                    break;

                case 3:
                    pData.pValues = adpcm_values_3;
                    break;

                case 4:
                    pData.pValues = adpcm_values_4;
                    break;

                default:
                    pData.pValues = null;
                    break;

                case 6:
                    pData.pValues = adpcm_values_6;
                    break;
            }

            pData.BitCount = BitCount;
            pData.field_C = 0x20000;
            pData.field_8 = 1 << BitCount;
            pData.field_10 = (1 << BitCount) / 2;
            return pData.pValues;
        }

        #endregion
    }
}