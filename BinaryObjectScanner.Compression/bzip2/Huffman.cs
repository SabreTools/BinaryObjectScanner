using static BinaryObjectScanner.Compression.bzip2.Constants;

namespace BinaryObjectScanner.Compression.bzip2
{
    /// <summary>
    /// Huffman coding low-level stuff
    /// </summary>
    /// <see href="https://github.com/ladislav-zezula/StormLib/blob/master/src/bzip2/huffman.c"/>
    internal static unsafe class Huffman
    {
        public static void BZ2_hbMakeCodeLengths(byte* len, int* freq, int alphaSize, int maxLen)
        {
            /*--
                Nodes and heap entries run from 1.  Entry 0
                for both the heap and nodes is a sentinel.
            --*/
            int nNodes, nHeap, n1, n2, i, j, k;
            bool tooLong;

            int[] heap = new int[BZ_MAX_ALPHA_SIZE + 2];
            int[] weight = new int[BZ_MAX_ALPHA_SIZE * 2];
            int[] parent = new int[BZ_MAX_ALPHA_SIZE * 2];

            for (i = 0; i < alphaSize; i++)
            {
                weight[i + 1] = (freq[i] == 0 ? 1 : freq[i]) << 8;
            }

            while (true)
            {

                nNodes = alphaSize;
                nHeap = 0;

                heap[0] = 0;
                weight[0] = 0;
                parent[0] = -2;

                for (i = 1; i <= alphaSize; i++)
                {
                    parent[i] = -1;
                    nHeap++;
                    heap[nHeap] = i;
                    UPHEAP(nHeap, heap, weight);
                }

                //AssertH(nHeap < (BZ_MAX_ALPHA_SIZE + 2), 2001);

                while (nHeap > 1)
                {
                    n1 = heap[1]; heap[1] = heap[nHeap]; nHeap--; DOWNHEAP(1, nHeap, heap, weight);
                    n2 = heap[1]; heap[1] = heap[nHeap]; nHeap--; DOWNHEAP(1, nHeap, heap, weight);
                    nNodes++;
                    parent[n1] = parent[n2] = nNodes;
                    weight[nNodes] = ADDWEIGHTS(weight[n1], weight[n2]);
                    parent[nNodes] = -1;
                    nHeap++;
                    heap[nHeap] = nNodes;
                    UPHEAP(nHeap, heap, weight);
                }

                //AssertH(nNodes < (BZ_MAX_ALPHA_SIZE * 2), 2002);

                tooLong = false;
                for (i = 1; i <= alphaSize; i++)
                {
                    j = 0;
                    k = i;
                    while (parent[k] >= 0) { k = parent[k]; j++; }
                    len[i - 1] = (byte)j;
                    if (j > maxLen) tooLong = true;
                }

                if (!tooLong) break;

                /* 17 Oct 04: keep-going condition for the following loop used
                   to be 'i < alphaSize', which missed the last element,
                   theoretically leading to the possibility of the compressor
                   looping.  However, this count-scaling step is only needed if
                   one of the generated Huffman code words is longer than
                   maxLen, which up to and including version 1.0.2 was 20 bits,
                   which is extremely unlikely.  In version 1.0.3 maxLen was
                   changed to 17 bits, which has minimal effect on compression
                   ratio, but does mean this scaling step is used from time to
                   time, enough to verify that it works.
                   This means that bzip2-1.0.3 and later will only produce
                   Huffman codes with a maximum length of 17 bits.  However, in
                   order to preserve backwards compatibility with bitstreams
                   produced by versions pre-1.0.3, the decompressor must still
                   handle lengths of up to 20. */

                for (i = 1; i <= alphaSize; i++)
                {
                    j = weight[i] >> 8;
                    j = 1 + (j / 2);
                    weight[i] = j << 8;
                }
            }
        }

        public static void BZ2_hbAssignCodes(int* code, byte* length, int minLen, int maxLen, int alphaSize)
        {
            int n, vec, i;

            vec = 0;
            for (n = minLen; n <= maxLen; n++)
            {
                for (i = 0; i < alphaSize; i++)
                {
                    if (length[i] == n)
                    {
                        code[i] = vec;
                        vec++;
                    }
                };

                vec <<= 1;
            }
        }

        public static void BZ2_hbCreateDecodeTables(int* limit, int* @base, int* perm, byte* length, int minLen, int maxLen, int alphaSize)
        {
            int pp, i, j, vec;

            pp = 0;
            for (i = minLen; i <= maxLen; i++)
            {
                for (j = 0; j < alphaSize; j++)
                {
                    if (length[j] == i) { perm[pp] = j; pp++; }
                }
            };

            for (i = 0; i < BZ_MAX_CODE_LEN; i++)
            {
                @base[i] = 0;
            }

            for (i = 0; i < alphaSize; i++)
            {
                @base[length[i] + 1]++;
            }

            for (i = 1; i < BZ_MAX_CODE_LEN; i++)
            {
                @base[i] += @base[i - 1];
            }

            for (i = 0; i < BZ_MAX_CODE_LEN; i++)
            {
                limit[i] = 0;
            }

            vec = 0;

            for (i = minLen; i <= maxLen; i++)
            {
                vec += (@base[i + 1] - @base[i]);
                limit[i] = vec - 1;
                vec <<= 1;
            }
            
            for (i = minLen + 1; i <= maxLen; i++)
            {
                @base[i] = ((limit[i - 1] + 1) << 1) - @base[i];
            }
        }

        #region Macros

        private static int WEIGHTOF(int zz0) => (int)(zz0 & 0xffffff00);

        private static int DEPTHOF(int zz1) => zz1 & 0x000000ff;

        private static int MYMAX(int zz2, int zz3) => zz2 > zz3 ? zz2 : zz3;

        private static int ADDWEIGHTS(int zw1, int zw2) => (WEIGHTOF(zw1) + WEIGHTOF(zw2)) | (1 + MYMAX(DEPTHOF(zw1), DEPTHOF(zw2)));

        private static void UPHEAP(int z, int[] heap, int[] weight)
        {
            int zz, tmp;
            zz = z; tmp = heap[zz];
            while (weight[tmp] < weight[heap[zz >> 1]])
            {
                heap[zz] = heap[zz >> 1];
                zz >>= 1;
            }

            heap[zz] = tmp;
        }

        private static void DOWNHEAP(int z, int nHeap, int[] heap, int[] weight)
        {
            int zz, yy, tmp;
            zz = z; tmp = heap[zz];
            while (true)
            {
                yy = zz << 1;
                if (yy > nHeap)
                    break;

                if (yy < nHeap && weight[heap[yy + 1]] < weight[heap[yy]])
                    yy++;

                if (weight[tmp] < weight[heap[yy]])
                    break;

                heap[zz] = heap[yy];
                zz = yy;
            }

            heap[zz] = tmp;
        }

        #endregion
    }
}