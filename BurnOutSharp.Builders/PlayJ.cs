using System.IO;
using System.Text;
using BurnOutSharp.Models.PlayJ;
using BurnOutSharp.Utilities;
using static BurnOutSharp.Models.PlayJ.Constants;

namespace BurnOutSharp.Builders
{
    public class PlayJ
    {
        #region Byte Data

        /// <summary>
        /// Parse a byte array into a PlayJ playlist
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled playlist on success, null on error</returns>
        public static Playlist ParsePlaylist(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParsePlaylist(dataStream);
        }

        /// <summary>
        /// Parse a byte array into a PlayJ audio file
        /// </summary>
        /// <param name="data">Byte array to parse</param>
        /// <param name="offset">Offset into the byte array</param>
        /// <returns>Filled audio file on success, null on error</returns>
        public static AudioFile ParseAudioFile(byte[] data, int offset)
        {
            // If the data is invalid
            if (data == null)
                return null;

            // If the offset is out of bounds
            if (offset < 0 || offset >= data.Length)
                return null;

            // Create a memory stream and parse that
            MemoryStream dataStream = new MemoryStream(data, offset, data.Length - offset);
            return ParseAudioFile(dataStream);
        }

        #endregion

        #region Stream Data

        /// <summary>
        /// Parse a Stream into a PlayJ playlist
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled playlist on success, null on error</returns>
        public static Playlist ParsePlaylist(Stream data)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new playlist to fill
            var playlist = new Playlist();

            #region Playlist Header

            // Try to parse the playlist header
            var playlistHeader = ParsePlaylistHeader(data);
            if (playlistHeader == null)
                return null;

            // Set the playlist header
            playlist.Header = playlistHeader;

            #endregion

            #region Audio Files

            // Create the audio files array
            playlist.AudioFiles = new AudioFile[playlistHeader.TrackCount];

            // Try to parse the audio files
            for (int i = 0; i < playlist.AudioFiles.Length; i++)
            {
                long currentOffset = data.Position;
                var entryHeader = ParseAudioFile(data, currentOffset);
                if (entryHeader == null)
                    return null;

                playlist.AudioFiles[i] = entryHeader;
            }

            #endregion

            return playlist;
        }

        /// <summary>
        /// Parse a Stream into a PlayJ audio file
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <param name="adjust">Offset to adjust all seeking by</param>
        /// <returns>Filled audio file on success, null on error</returns>
        public static AudioFile ParseAudioFile(Stream data, long adjust = 0)
        {
            // If the data is invalid
            if (data == null || data.Length == 0 || !data.CanSeek || !data.CanRead)
                return null;

            // If the offset is out of bounds
            if (data.Position < 0 || data.Position >= data.Length)
                return null;

            // Cache the current offset
            int initialOffset = (int)data.Position;

            // Create a new audio file to fill
            var audioFile = new AudioFile();

            #region Entry Header

            // Try to parse the entry header
            var entryHeader = ParseEntryHeader(data);
            if (entryHeader == null)
                return null;

            // Set the entry header
            audioFile.Header = entryHeader;

            #endregion

            #region Unknown Block 1

            // Get the unknown block 1 offset
            long offset = entryHeader.UnknownOffset1 + adjust;
            if (offset < 0 || offset >= data.Length)
                return null;

            // Seek to the unknown block 1
            data.Seek(offset, SeekOrigin.Begin);

            // Try to parse the unknown block 1
            var unknownBlock1 = ParseUnknownBlock1(data);
            if (unknownBlock1 == null)
                return null;

            // Set the unknown block 1
            audioFile.UnknownBlock1 = unknownBlock1;

            #endregion

            #region Unknown Value 2

            // Get the unknown value 2 offset
            offset = entryHeader.UnknownOffset2 + adjust;
            if (offset < 0 || offset >= data.Length)
                return null;

            // Seek to the unknown value 2
            data.Seek(offset, SeekOrigin.Begin);

            // Set the unknown value 2
            audioFile.UnknownValue2 = data.ReadUInt32(); ;

            #endregion

            #region Unknown Block 3

            // Get the unknown block 3 offset
            offset = entryHeader.UnknownOffset3 + adjust;
            if (offset < 0 || offset >= data.Length)
                return null;

            // Seek to the unknown block 3
            data.Seek(offset, SeekOrigin.Begin);

            // Try to parse the unknown block 3
            var unknownBlock3 = ParseUnknownBlock3(data);
            if (unknownBlock3 == null)
                return null;

            // Set the unknown block 3
            audioFile.UnknownBlock3 = unknownBlock3;

            #endregion

            return audioFile;
        }

        /// <summary>
        /// Parse a Stream into a playlist header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled playlist header on success, null on error</returns>
        private static PlaylistHeader ParsePlaylistHeader(Stream data)
        {
            // TODO: Use marshalling here instead of building
            PlaylistHeader playlistHeader = new PlaylistHeader();

            playlistHeader.TrackCount = data.ReadUInt32();
            playlistHeader.Data = data.ReadBytes(52);

            return playlistHeader;
        }

        /// <summary>
        /// Parse a Stream into an entry header
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled entry header on success, null on error</returns>
        private static EntryHeader ParseEntryHeader(Stream data)
        {
            // Cache the current offset
            long initialOffset = data.Position;

            // TODO: Use marshalling here instead of building
            EntryHeader entryHeader = new EntryHeader();

            entryHeader.Signature = data.ReadUInt32();
            if (entryHeader.Signature != SignatureUInt32)
                return null;

            // Only V1 is fully supported
            entryHeader.Version = data.ReadUInt32();
            if (entryHeader.Version == 0x00000000)
            {
                entryHeader.TrackID = data.ReadUInt32();
                entryHeader.UnknownOffset1 = data.ReadUInt32();
                entryHeader.UnknownOffset2 = data.ReadUInt32();
                entryHeader.UnknownOffset3 = data.ReadUInt32();
                entryHeader.Unknown1 = data.ReadUInt32();
                entryHeader.Unknown2 = data.ReadUInt32();
                entryHeader.Year = data.ReadUInt32();
                entryHeader.TrackNumber = data.ReadByteValue();
                entryHeader.Subgenre = (Subgenre)data.ReadByteValue();
                entryHeader.Duration = data.ReadUInt32();
            }
            else
            {
                // Discard the following pieces until we can figure out what they are
                _ = data.ReadBytes(0x5C);
            }

            entryHeader.TrackLength = data.ReadUInt16();
            byte[] track = data.ReadBytes(entryHeader.TrackLength);
            if (track != null)
                entryHeader.Track = Encoding.ASCII.GetString(track);

            entryHeader.ArtistLength = data.ReadUInt16();
            byte[] artist = data.ReadBytes(entryHeader.ArtistLength);
            if (artist != null)
                entryHeader.Artist = Encoding.ASCII.GetString(artist);

            entryHeader.AlbumLength = data.ReadUInt16();
            byte[] album = data.ReadBytes(entryHeader.AlbumLength);
            if (album != null)
                entryHeader.Album = Encoding.ASCII.GetString(album);

            entryHeader.WriterLength = data.ReadUInt16();
            byte[] writer = data.ReadBytes(entryHeader.WriterLength);
            if (writer != null)
                entryHeader.Writer = Encoding.ASCII.GetString(writer);

            entryHeader.PublisherLength = data.ReadUInt16();
            byte[] publisher = data.ReadBytes(entryHeader.PublisherLength);
            if (publisher != null)
                entryHeader.Publisher = Encoding.ASCII.GetString(publisher);

            entryHeader.LabelLength = data.ReadUInt16();
            byte[] label = data.ReadBytes(entryHeader.LabelLength);
            if (label != null)
                entryHeader.Label = Encoding.ASCII.GetString(label);

            if (data.Position - initialOffset < entryHeader.UnknownOffset1)
            {
                entryHeader.CommentsLength = data.ReadUInt16();
                byte[] comments = data.ReadBytes(entryHeader.CommentsLength);
                if (comments != null)
                    entryHeader.Comments = Encoding.ASCII.GetString(comments);
            }

            return entryHeader;
        }

        /// <summary>
        /// Parse a Stream into an unknown block 1
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled unknown block 1 on success, null on error</returns>
        private static UnknownBlock1 ParseUnknownBlock1(Stream data)
        {
            // TODO: Use marshalling here instead of building
            UnknownBlock1 unknownBlock1 = new UnknownBlock1();

            unknownBlock1.Length = data.ReadUInt16();
            unknownBlock1.Data = data.ReadBytes(unknownBlock1.Length);

            return unknownBlock1;
        }

        /// <summary>
        /// Parse a Stream into an unknown block 3
        /// </summary>
        /// <param name="data">Stream to parse</param>
        /// <returns>Filled unknown block 3 on success, null on error</returns>
        private static UnknownBlock3 ParseUnknownBlock3(Stream data)
        {
            // TODO: Use marshalling here instead of building
            UnknownBlock3 unknownBlock3 = new UnknownBlock3();

            // No-op because we don't even know the length

            return unknownBlock3;
        }

        #endregion
    }
}
