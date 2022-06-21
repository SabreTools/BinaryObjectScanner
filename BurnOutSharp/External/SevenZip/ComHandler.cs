// ComHandler.cpp

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace NArchive
{
    enum kProps : byte
    {
        kpidPath,
        kpidSize,
        kpidPackSize,
        kpidCTime,
        kpidMTime
    };

    enum kArcProps : byte
    {
        kpidExtension,
        kpidClusterSize,
        kpidSectorSize
    };
}

//namespace NArchive
//{
//    // #define BitConverter.ToUInt16(p) GetUi16(p)
//    // #define BitConverter.ToUInt32(p) GetUi32(p)

//    class CHandler : IInArchive, IInArchiveGetStream, CMyUnknownImp
//    {
//        private CMyComPtr<IInStream> _stream;
//        NArchive.NCom.CDatabase _db;

//        public MY_UNKNOWN_IMP2(IInArchive, IInArchiveGetStream);
//        public INTERFACE_IInArchive();
//        public STDMETHOD(GetStream)(uint index, ISequentialInStream** stream);
//    }

//    IMP_IInArchive_Props
//    IMP_IInArchive_ArcProps

//    STDMETHODIMP CHandler::GetArchiveProperty(kProps propID, PROPVARIANT* value)
//    {
//        COM_TRY_BEGIN
//        NWindows::NCOM::CPropVariant prop;
//        switch (propID)
//        {
//            case kpidExtension: prop = kExtensions[(ulong)_db.Type]; break;
//            case kpidPhySize: prop = _db.PhySize; break;
//            case kpidClusterSize: prop = (uint)1 << _db.SectorSizeBits; break;
//            case kpidSectorSize: prop = (uint)1 << _db.MiniSectorSizeBits; break;
//            case kpidMainSubfile: if (_db.MainSubfile >= 0) prop = (uint)_db.MainSubfile; break;
//            case kpidIsNotArcType: if (_db.IsNotArcType()) prop = true; break;
//        }
//        prop.Detach(value);
//        return true;
//        COM_TRY_END
//    }

//    STDMETHODIMP CHandler::GetProperty(uint index, PROPID propID, PROPVARIANT* value)
//    {
//        COM_TRY_BEGIN
//        NWindows::NCOM::CPropVariant prop;
//        const CRef &ref = _db.Refs[index];
//        const CItem &item = _db.Items[ref.Did];

//        switch (propID)
//        {
//            case kpidPath: prop = _db.GetItemPath(index); break;
//            case kpidIsDir: prop = item.IsDir(); break;
//            case kpidCTime: prop = item.CTime; break;
//            case kpidMTime: prop = item.MTime; break;
//            case kpidPackSize: if (!item.IsDir()) prop = _db.GetItemPackSize(item.Size); break;
//            case kpidSize: if (!item.IsDir()) prop = item.Size; break;
//        }
//        prop.Detach(value);
//        return true;
//        COM_TRY_END
//    }

//    STDMETHODIMP CHandler::Open(Stream inStream,
//        const ulong* /* maxCheckStartPosition */,
//        IArchiveOpenCallback* /* openArchiveCallback */)
//    {
//        COM_TRY_BEGIN
//      Close();
//        try
//        {
//            if (_db.Open(inStream) != true)
//                return false;
//            _stream = inStream;
//        }
//        catch (...) { return false; }
//        return true;
//        COM_TRY_END
//      }

//        STDMETHODIMP CHandler::Close()
//    {
//            _db.Clear();
//            _stream.Release();
//            return true;
//        }

//        STDMETHODIMP CHandler::Extract(const uint* indices, uint numItems,
//        int testMode, IArchiveExtractCallback*extractCallback)
//{
//            COM_TRY_BEGIN
//  bool allFilesMode = (numItems == (uint)(int)-1);
//            if (allFilesMode)
//                numItems = _db.Refs.Size();
//            if (numItems == 0)
//                return true;
//            uint i;
//            ulong totalSize = 0;
//            for (i = 0; i < numItems; i++)
//            {
//                const CItem &item = _db.Items[_db.Refs[allFilesMode ? i : indices[i]].Did];
//                if (!item.IsDir())
//                    totalSize += item.Size;
//            }
//            RINOK(extractCallback.SetTotal(totalSize));

//            ulong totalPackSize;
//            totalSize = totalPackSize = 0;

//            NCompress::CCopyCoder* copyCoderSpec = new NCompress::CCopyCoder();
//            CMyComPtr<ICompressCoder> copyCoder = copyCoderSpec;

//            CLocalProgress* lps = new CLocalProgress;
//            CMyComPtr<ICompressProgressInfo> progress = lps;
//            lps.Init(extractCallback, false);

//            for (i = 0; i < numItems; i++)
//            {
//                lps.InSize = totalPackSize;
//                lps.OutSize = totalSize;
//                RINOK(lps.SetCur());
//                int index = allFilesMode ? i : indices[i];
//                const CItem &item = _db.Items[_db.Refs[index].Did];

//                CMyComPtr<ISequentialOutStream> outStream;
//                int askMode = testMode ?
//                    NExtract::NAskMode::kTest :
//                    NExtract::NAskMode::kExtract;
//                RINOK(extractCallback.GetStream(index, &outStream, askMode));

//                if (item.IsDir())
//                {
//                    RINOK(extractCallback.PrepareOperation(askMode));
//                    RINOK(extractCallback.SetOperationResult(NExtract::NOperationResult::kOK));
//                    continue;
//                }

//                totalPackSize += _db.GetItemPackSize(item.Size);
//                totalSize += item.Size;

//                if (!testMode && !outStream)
//                    continue;
//                RINOK(extractCallback.PrepareOperation(askMode));
//                int res = NExtract::NOperationResult::kDataError;
//                CMyComPtr<ISequentialInStream> inStream;
//                uint hres = GetStream(index, &inStream);
//                if (hres == false)
//                    res = NExtract::NOperationResult::kDataError;
//                else if (hres == E_NOTIMPL)
//                    res = NExtract::NOperationResult::kUnsupportedMethod;
//                else
//                {
//                    RINOK(hres);
//                    if (inStream)
//                    {
//                        RINOK(copyCoder.Code(inStream, outStream, null, null, progress));
//                        if (copyCoderSpec.TotalSize == item.Size)
//                            res = NExtract::NOperationResult::kOK;
//                    }
//                }
//                outStream.Release();
//                RINOK(extractCallback.SetOperationResult(res));
//            }
//            return true;
//            COM_TRY_END
//}

//        STDMETHODIMP CHandler::GetNumberOfItems(uint * numItems)
//    {
//            *numItems = _db.Refs.Size();
//            return true;
//        }

//        STDMETHODIMP CHandler::GetStream(uint index, ISequentialInStream * *stream)
//    {
//            COM_TRY_BEGIN
//            * stream = 0;
//            uint itemIndex = _db.Refs[index].Did;
//            const CItem &item = _db.Items[itemIndex];
//            CClusterInStream* streamSpec = new CClusterInStream;
//            CMyComPtr<ISequentialInStream> streamTemp = streamSpec;
//            streamSpec.Stream = _stream;
//            streamSpec.StartOffset = 0;

//            bool isLargeStream = (itemIndex == 0 || _db.IsLargeStream(item.Size));
//            int bsLog = isLargeStream ? _db.SectorSizeBits : _db.MiniSectorSizeBits;
//            streamSpec.BlockSizeLog = bsLog;
//            streamSpec.Size = item.Size;

//            uint clusterSize = (uint)1 << bsLog;
//            ulong numClusters64 = (item.Size + clusterSize - 1) >> bsLog;
//            if (numClusters64 >= ((uint)1 << 31))
//                return E_NOTIMPL;
//            streamSpec.Vector.ClearAndReserve((ulong)numClusters64);
//            uint sid = item.Sid;
//            ulong size = item.Size;

//            if (size != 0)
//            {
//                for (; ; size -= clusterSize)
//                {
//                    if (isLargeStream)
//                    {
//                        if (sid >= _db.FatSize)
//                            return false;
//                        streamSpec.Vector.AddInReserved(sid + 1);
//                        sid = _db.Fat[sid];
//                    }
//                    else
//                    {
//                        ulong val = 0;
//                        if (sid >= _db.MatSize || !_db.GetMiniCluster(sid, val) || val >= (ulong)1 << 32)
//                            return false;
//                        streamSpec.Vector.AddInReserved((uint)val);
//                        sid = _db.Mat[sid];
//                    }
//                    if (size <= clusterSize)
//                        break;
//                }
//            }
//            if (sid != NFatID.kEndOfChain)
//                return false;
//            RINOK(streamSpec.InitAndSeek());
//            *stream = streamTemp.Detach();
//            return true;
//            COM_TRY_END
//    }
//}

namespace NArchive.NCom
{
    public enum EType
    {
        k_Type_Common,
        k_Type_Msi,
        k_Type_Msp,
        k_Type_Doc,
        k_Type_Ppt,
        k_Type_Xls
    };

    public enum NFatID : uint
    {
        kFree = 0xFFFFFFFF,
        kEndOfChain = 0xFFFFFFFE,
        kFatSector = 0xFFFFFFFD,
        kMatSector = 0xFFFFFFFC,
        kMaxValue = 0xFFFFFFFA,
    }

    public enum NItemType : byte
    {
        kEmpty = 0,
        kStorage = 1,
        kStream = 2,
        kLockbytes = 3,
        kProperty = 4,
        kRootStorage = 5,
    }

    public class CItem
    {
        #region Properties

        public byte[] Name { get; set; } = new byte[CDatabase.kNameSizeMax];

        // public ushort NameSize { get; set; }

        // public uint Flags { get; set; }

        public DateTime? CTime { get; set; }

        public DateTime? MTime { get; set; }

        public ulong Size { get; set; }

        public uint LeftDid { get; set; }

        public uint RightDid { get; set; }

        public uint SonDid { get; set; }

        public uint Sid { get; set; }

        public NItemType Type { get; set; }

        #endregion

        #region Functions

        public bool IsEmpty() => Type == NItemType.kEmpty;

        public bool IsDir() => Type == NItemType.kStorage || Type == NItemType.kRootStorage;

        public void Parse(in byte[] p, int offset, bool mode64bit)
        {
            Array.Copy(p, offset, Name, 0, CDatabase.kNameSizeMax);
            // NameSize = BitConverter.ToUInt16(p, offset + 64);
            Type = (NItemType)p[offset + 66];
            LeftDid = BitConverter.ToUInt32(p, offset + 68);
            RightDid = BitConverter.ToUInt32(p, offset + 72);
            SonDid = BitConverter.ToUInt32(p, offset + 76);
            // Flags = BitConverter.ToUInt32(p + offset + 96);
            CDatabase.GetFileTimeFromMem(p, offset + 100, out DateTime? ctime);
            CTime = ctime;
            CDatabase.GetFileTimeFromMem(p, offset + 108, out DateTime? mtime);
            MTime = mtime;
            Sid = BitConverter.ToUInt32(p, offset + 116);
            Size = BitConverter.ToUInt32(p, offset + 120);
            if (mode64bit)
                Size |= ((ulong)BitConverter.ToUInt32(p, offset + 124) << 32);
        }

        #endregion
    }

    public class CRef
    {
        public int Parent { get; set; }

        public uint Did { get; set; }
    }

    public class CDatabase
    {
        #region Constants

        internal static readonly byte[] kSignature = { 0xD0, 0xCF, 0x11, 0xE0, 0xA1, 0xB1, 0x1A, 0xE1 };

        internal static readonly string[] kExtensions =
        {
          "compound",
          "msi",
          "msp",
          "doc",
          "ppt",
          "xls",
        };

        internal const int kNameSizeMax = 64;

        internal const uint kNoDid = 0xFFFFFFFF;

        internal const string k_Msi_Chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz._";

        // static const char * const k_Msi_ID = ""; // "{msi}";
        internal const char k_Msi_SpecChar = '!';

        internal const int k_Msi_NumBits = 6;
        internal const int k_Msi_NumChars = 1 << k_Msi_NumBits;
        internal const int k_Msi_CharMask = k_Msi_NumChars - 1;
        internal const char k_Msi_StartUnicodeChar = (char)0x3800;
        internal const int k_Msi_UnicodeRange = k_Msi_NumChars * (k_Msi_NumChars + 1);

        // There is name "[!]MsiPatchSequence" in msp files
        internal const int kMspSequence_Size = 18;
        internal static readonly byte[] kMspSequence = new byte[kMspSequence_Size]
        {
            0x40, 0x48, 0x96, 0x45, 0x6C, 0x3E, 0xE4, 0x45,
            0xE6, 0x42, 0x16, 0x42, 0x37, 0x41, 0x27, 0x41,
            0x37, 0x41
        };

        #endregion

        #region Properties

        internal uint NumSectorsInMiniStream { get; set; }

        internal uint[] MiniSids { get; set; }

        public uint[] Fat { get; set; }

        public uint FatSize { get; set; }

        public uint[] Mat { get; set; }

        public uint MatSize { get; set; }

        public List<CItem> Items { get; set; }

        public List<CRef> Refs { get; set; }

        public uint LongStreamMinSize { get; set; }

        public int SectorSizeBits { get; set; }

        public int MiniSectorSizeBits { get; set; }

        public int MainSubfile { get; set; }

        public long PhySize { get; set; }

        public EType Type { get; set; }

        #endregion

        #region Functions

        public bool IsNotArcType() => Type != EType.k_Type_Msi && Type != EType.k_Type_Msp;

        public void UpdatePhySize(long val)
        {
            if (PhySize < val)
                PhySize = val;
        }

        public bool ReadSector(Stream inStream, byte[] buf, int sectorSizeBits, uint sid)
        {
            UpdatePhySize(((long)sid + 2) << sectorSizeBits);

            long offset = ((long)sid + 1) << sectorSizeBits;
            if (inStream.Seek(offset, SeekOrigin.Begin) != offset)
                return false;

            return inStream.Read(buf, 0, 1 << sectorSizeBits) == 1 << sectorSizeBits;
        }

        public bool ReadIDs(Stream inStream, byte[] buf, int sectorSizeBits, uint sid, uint[] dest, int destPtr)
        {
            ReadSector(inStream, buf, sectorSizeBits, sid);
            int sectorSize = 1 << sectorSizeBits;
            for (int i = destPtr, t = 0; t < sectorSize; i++, t += 4)
            {
                dest[i] = BitConverter.ToUInt32(buf, t);
            }

            return true;
        }

        public bool Update_PhySize_WithItem(int index)
        {
            CItem item = Items[index];
            bool isLargeStream = (index == 0 || IsLargeStream(item.Size));
            if (!isLargeStream)
                return true;

            int bsLog = isLargeStream ? SectorSizeBits : MiniSectorSizeBits;
            // streamSpec.Size = item.Size;

            uint clusterSize = (uint)1 << bsLog;
            ulong numClusters64 = (item.Size + clusterSize - 1) >> bsLog;
            if (numClusters64 >= ((uint)1 << 31))
                return false;

            uint sid = item.Sid;
            ulong size = item.Size;

            if (size != 0)
            {
                for (; ; size -= clusterSize)
                {
                    // if (isLargeStream)
                    {
                        if (sid >= FatSize)
                            return false;

                        UpdatePhySize((sid + 2) << bsLog);
                        sid = Fat[(int)sid];
                    }

                    if (size <= clusterSize)
                        break;
                }
            }
            if (sid != (uint)NFatID.kEndOfChain)
                return false;

            return true;
        }

        public void Clear()
        {
            PhySize = 0;

            Array.Clear(Fat, 0, Fat.Length);
            Array.Clear(MiniSids, 0, MiniSids.Length);
            Array.Clear(Mat, 0, Mat.Length);
            Items.Clear();
            Refs.Clear();
        }

        public bool IsLargeStream(ulong size) => size >= LongStreamMinSize;

        public string GetItemPath(uint index)
        {
            string s = string.Empty;
            while (index != kNoDid)
            {
                CRef cref = Refs[(int)index];
                CItem item = Items[(int)cref.Did];
                if (!string.IsNullOrEmpty(s))
                    s = Path.DirectorySeparatorChar + s;
                s.Insert(0, ConvertName(item.Name));
                index = (uint)cref.Parent;
            }
            return s;
        }

        public ulong GetItemPackSize(ulong size)
        {
            ulong mask = (ulong)(1 << (IsLargeStream(size) ? SectorSizeBits : MiniSectorSizeBits)) - 1;
            return (size + mask) & ~mask;
        }

        public bool GetMiniCluster(uint sid, ref ulong res)
        {
            int subBits = SectorSizeBits - MiniSectorSizeBits;
            uint fid = sid >> (int)subBits;
            if (fid >= NumSectorsInMiniStream)
                return false;
            res = (ulong)(((MiniSids[(int)fid] + 1) << subBits) + (sid & ((1 << subBits) - 1)));
            return true;
        }

        public bool Open(Stream inStream)
        {
            MainSubfile = -1;
            Type = EType.k_Type_Common;
            int kHeaderSize = 512;
            byte[] p = new byte[kHeaderSize];
            PhySize = kHeaderSize;
            if (inStream.Read(p, 0, kHeaderSize) != kHeaderSize)
                return false;

            if (!p.Take(kHeaderSize).SequenceEqual(kSignature))
                return false;
            if (BitConverter.ToUInt16(p, 0x1A) > 4) // majorVer
                return false;
            if (BitConverter.ToUInt16(p, 0x1C) != 0xFFFE) // Little-endian
                return false;
            int sectorSizeBits = BitConverter.ToUInt16(p, 0x1E);
            bool mode64bit = (sectorSizeBits >= 12);
            int miniSectorSizeBits = BitConverter.ToUInt16(p, 0x20);
            SectorSizeBits = sectorSizeBits;
            MiniSectorSizeBits = miniSectorSizeBits;

            if (sectorSizeBits > 24 ||
                sectorSizeBits < 7 ||
                miniSectorSizeBits > 24 ||
                miniSectorSizeBits < 2 ||
                miniSectorSizeBits > sectorSizeBits)
                return false;
            uint numSectorsForFAT = BitConverter.ToUInt32(p, 0x2C); // SAT
            LongStreamMinSize = BitConverter.ToUInt32(p, 0x38);

            uint sectSize = (uint)1 << sectorSizeBits;

            byte[] sect = new byte[sectSize];

            int ssb2 = sectorSizeBits - 2;
            int numSidsInSec = 1 << ssb2;
            int numFatItems = (int)(numSectorsForFAT << ssb2);
            if ((numFatItems >> ssb2) != numSectorsForFAT)
                return false;
            FatSize = (uint)numFatItems;

            {
                uint numSectorsForBat = BitConverter.ToUInt32(p, 0x48); // master sector allocation table
                const uint kNumHeaderBatItems = 109;
                uint numBatItems = kNumHeaderBatItems + (numSectorsForBat << ssb2);
                if (numBatItems < kNumHeaderBatItems || ((numBatItems - kNumHeaderBatItems) >> ssb2) != numSectorsForBat)
                    return false;
                uint[] bat = new uint[numBatItems];
                uint i;
                for (i = 0; i < kNumHeaderBatItems; i++)
                    bat[i] = BitConverter.ToUInt32(p, (int)(0x4c + i * 4));
                uint sid = BitConverter.ToUInt32(p, 0x44);
                for (uint s = 0; s < numSectorsForBat; s++)
                {
                    if (!ReadIDs(inStream, sect, sectorSizeBits, sid, bat, (int)i))
                        return false;
                    i += (uint)numSidsInSec - 1;
                    sid = bat[i];
                }
                numBatItems = i;

                Fat = new uint[numFatItems];
                uint j = 0;

                for (i = 0; i < numFatItems; j++, i += (uint)numSidsInSec)
                {
                    if (j >= numBatItems)
                        return false;
                    if (!ReadIDs(inStream, sect, sectorSizeBits, bat[j], Fat, (int)i))
                        return false;
                }
                numFatItems = (int)i;
                FatSize = i;
            }

            uint numMatItems;
            {
                uint numSectorsForMat = BitConverter.ToUInt32(p, 0x40);
                numMatItems = numSectorsForMat << ssb2;
                if ((numMatItems >> ssb2) != numSectorsForMat)
                    return false;
                Mat = new uint[numMatItems];
                uint i;
                uint sid = BitConverter.ToUInt32(p, 0x3C); // short-sector table SID
                for (i = 0; i < numMatItems; i += (uint)numSidsInSec)
                {
                    if (!ReadIDs(inStream, sect, sectorSizeBits, sid, Mat, (int)i))
                        return false;
                    if (sid >= numFatItems)
                        return false;
                    sid = Fat[sid];
                }

                if (sid != (uint)NFatID.kEndOfChain)
                    return false;
            }

            {
                byte[] used = new byte[numFatItems];
                for (uint i = 0; i < numFatItems; i++)
                    used[i] = 0;
                uint sid = BitConverter.ToUInt32(p, 0x30); // directory stream SID
                for (; ; )
                {
                    if (sid >= numFatItems)
                        return false;
                    if (used[sid] != 0)
                        return false;
                    used[sid] = 1;
                    if (!ReadSector(inStream, sect, sectorSizeBits, sid))
                        return false;
                    for (uint i = 0; i < sectSize; i += 128)
                    {
                        CItem item = new CItem();
                        item.Parse(sect, (int)i, mode64bit);
                        Items.Add(item);
                    }
                    sid = Fat[sid];
                    if (sid == (uint)NFatID.kEndOfChain)
                        break;
                }
            }

            CItem root = Items[0];

            {
                uint numSectorsInMiniStream;
                {
                    ulong numSatSects64 = (root.Size + sectSize - 1) >> sectorSizeBits;
                    if (numSatSects64 > (uint)NFatID.kMaxValue)
                        return false;
                    numSectorsInMiniStream = (uint)numSatSects64;
                }
                NumSectorsInMiniStream = numSectorsInMiniStream;
                MiniSids = new uint[numSectorsInMiniStream];
                {
                    ulong matSize64 = (root.Size + ((ulong)1 << miniSectorSizeBits) - 1) >> miniSectorSizeBits;
                    if (matSize64 > (uint)NFatID.kMaxValue)
                        return false;
                    MatSize = (uint)matSize64;
                    if (numMatItems < MatSize)
                        return false;
                }

                uint sid = root.Sid;
                for (uint i = 0; ; i++)
                {
                    if (sid == (uint)NFatID.kEndOfChain)
                    {
                        if (i != numSectorsInMiniStream)
                            return false;
                        break;
                    }
                    if (i >= numSectorsInMiniStream)
                        return false;
                    MiniSids[i] = sid;
                    if (sid >= numFatItems)
                        return false;
                    sid = Fat[sid];
                }
            }

            if (!AddNode(-1, root.SonDid))
                return false;

            ulong numCabs = 0;

            for (int i = 0; i < Refs.Count; i++)
            {
                CItem item = Items[(int)Refs[i].Did];
                if (item.IsDir() || numCabs > 1)
                    continue;
                bool isMsiName = false;
                string msiName = ConvertName(item.Name, ref isMsiName);
                if (isMsiName && !string.IsNullOrEmpty(msiName))
                {
                    // bool isThereExt = (msiName.Find(L'.') >= 0);
                    bool isMsiSpec = (msiName[0] == k_Msi_SpecChar);
                    string extension = Path.GetExtension(msiName).Trim('.');

                    if ((msiName.Length >= 4 && extension.Equals("cab", StringComparison.OrdinalIgnoreCase)
                        || (!isMsiSpec && msiName.Length >= 3 && extension.Equals("exe", StringComparison.OrdinalIgnoreCase)))
                        // || (!isMsiSpec && !isThereExt)
                        )
                    {
                        numCabs++;
                        MainSubfile = i;
                    }
                }
            }

            if (numCabs > 1)
                MainSubfile = -1;

            {
                for (int t = 0; t < Items.Count; t++)
                {
                    Update_PhySize_WithItem(t);
                }
            }
            {
                for (int t = 0; t < Items.Count; t++)
                {
                    CItem item = Items[t];

                    if (IsMsiName(item.Name))
                    {
                        Type = EType.k_Type_Msi;

                        if (item.Name.Take(kMspSequence_Size).SequenceEqual(kMspSequence))
                        {
                            Type = EType.k_Type_Msp;
                            break;
                        }
                        continue;
                    }
                    if (AreEqualNames(item.Name, "WordDocument"))
                    {
                        Type = EType.k_Type_Doc;
                        break;
                    }
                    if (AreEqualNames(item.Name, "PowerPoint Document"))
                    {
                        Type = EType.k_Type_Ppt;
                        break;
                    }
                    if (AreEqualNames(item.Name, "Workbook"))
                    {
                        Type = EType.k_Type_Xls;
                        break;
                    }
                }
            }

            return true;
        }

        #endregion

        #region Utilities

        internal bool AddNode(int parent, uint did)
        {
            if (did == kNoDid)
                return true;
            if (did >= (uint)Items.Count)
                return false;
            CItem item = Items[(int)did];
            if (item.IsEmpty())
                return false;

            CRef cref = new CRef();
            cref.Parent = parent;
            cref.Did = did;
            Refs.Add(cref);
            int index = Refs.Count - 1;
            if (Refs.Count > Items.Count)
                return false;

            if (!AddNode(parent, item.LeftDid))
                return false;
            if (!AddNode(parent, item.RightDid))
                return false;
            if (item.IsDir() && !AddNode(index, item.SonDid))
                return false;

            return true;
        }

        internal static void GetFileTimeFromMem(in byte[] p, int offset, out DateTime? ft)
        {
            long time = BitConverter.ToInt64(p, offset);
            ft = DateTime.FromFileTime(time);
        }

        internal static string CompoundNameToFileName(in string s)
        {
            string res = string.Empty;
            for (int i = 0; i < s.Length; i++)
            {
                char c = s[i];
                if (c < 0x20)
                    res += $"[{c}]";
                else
                    res += c;
            }

            return res;
        }

        internal static bool IsMsiName(in byte[] p)
        {
            uint c = BitConverter.ToUInt16(p, 0);
            return c >= k_Msi_StartUnicodeChar && c <= k_Msi_StartUnicodeChar + k_Msi_UnicodeRange;
        }

        internal static bool AreEqualNames(in byte[] rawName, in string asciiName)
        {
            for (int i = 0; i < kNameSizeMax / 2; i++)
            {
                char c = (char)BitConverter.ToUInt16(rawName, i * 2);
                char c2 = asciiName[i];
                if (c != c2)
                    return false;
                if (c == 0)
                    return true;
            }
            return false;
        }

        internal static bool CompoundMsiNameToFileName(in string name, ref string res)
        {
            res = string.Empty;
            for (int i = 0; i < name.Length; i++)
            {
                char c = name[i];
                if (c < (char)k_Msi_StartUnicodeChar || c > (char)(k_Msi_StartUnicodeChar + k_Msi_UnicodeRange))
                    return false;
                /*
                if (i == 0)
                  res += k_Msi_ID;
                */
                c -= k_Msi_StartUnicodeChar;

                int c0 = c & k_Msi_CharMask;
                int c1 = c >> k_Msi_NumBits;

                if (c1 <= k_Msi_NumChars)
                {
                    res += k_Msi_Chars[c0];
                    if (c1 == k_Msi_NumChars)
                        break;
                    res += k_Msi_Chars[c1];
                }
                else
                    res += k_Msi_SpecChar;
            }
            return true;
        }

        internal static string ConvertName(in byte[] p, ref bool isMsi)
        {
            isMsi = false;
            string s = string.Empty;

            for (int i = 0; i < kNameSizeMax; i += 2)
            {
                char c = (char)BitConverter.ToUInt16(p, i);
                if (c == 0)
                    break;
                s += c;
            }

            string msiName = string.Empty;
            if (CompoundMsiNameToFileName(s, ref msiName))
            {
                isMsi = true;
                return msiName;
            }
            return CompoundNameToFileName(s);
        }

        internal static string ConvertName(in byte[] p)
        {
            bool isMsi = false;
            return ConvertName(p, ref isMsi);
        }

        #endregion
    }
}