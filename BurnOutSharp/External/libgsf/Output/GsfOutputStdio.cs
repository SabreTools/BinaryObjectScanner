/* vim: set sw=8: -*- Mode: C; tab-width: 8; indent-tabs-mode: t; c-basic-offset: 8 -*- */
/*
 * gsf-output-stdio.c: stdio based output
 *
 * Copyright (C) 2002-2006 Jody Goldberg (jody@gnome.org)
 *
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of version 2.1 of the GNU Lesser General Public
 * License as published by the Free Software Foundation.
 *
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 *
 * You should have received a copy of the GNU Lesser General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301
 * USA
 */

using System;
using System.IO;
using System.Text;

namespace LibGSF.Output
{
    public class GsfOutputStdio : GsfOutput
    {
        #region Constants

        private static int W_OK = 2;

        private static int GSF_MAX_LINK_LEVEL = 256;

        #endregion

        #region Properties

        public FileStream FileStream { get; set; } = null;

        public string RealFilename { get; set; }

        public string TempFilename { get; set; }

        public bool CreateBackupCopy { get; set; } = false;

        public bool KeepOpen { get; set; } = false;

        public FileInfo Stat { get; set; }

        #endregion

        #region Constructor and Destructor

        /// <summary>
        /// Private constructor
        /// </summary>
        private GsfOutputStdio() { }

        public static GsfOutputStdio Create(string filename, ref Exception err)
        {
            try
            {
                FileStream fs = File.OpenWrite(filename);
                return new GsfOutputStdio
                {
                    FileStream = fs,
                    RealFilename = filename,
                };
            }
            catch (Exception ex)
            {
                err = ex;
                return null;
            }
        }

        #endregion

        #region Functions

        /// <inheritdoc/>
        protected override bool CloseImpl()
        {
            bool res;
            string backup_filename = null;

            if (FileStream == null)
                return false;

            if (Error != null)
            {
                res = true;
                if (!KeepOpen && !CloseFileHelper(false))
                    res = false;

                if (!UnlinkFileHelper())
                    res = false;

                return res;
            }

            if (KeepOpen)
            {
                FileStream.Flush();
                FileStream = null;
                return true;
            }

            res = CloseFileHelper(true);

            // short circuit our when dealing with raw FILE
            if (RealFilename == null)
                return res;

            if (!res)
            {
                UnlinkFileHelper();
                return false;
            }

            // Move the original file to a backup
            if (CreateBackupCopy)
            {
                backup_filename = $"{RealFilename}.bak";
                int result = RenameWrapper(RealFilename, backup_filename);
                if (result != 0)
                {
                    Error = new Exception($"Could not backup the original as {backup_filename}.");
                    return false;
                }
            }

            // Move the temp file to the original file
            if (RenameWrapper(TempFilename, RealFilename) != 0)
            {
                Error = new Exception();
                return false;
            }

            DateTime? modtime = ModTime;
            if (modtime != null)
                new FileInfo(RealFilename).LastWriteTime = modtime.Value;

            // Restore permissions.  There is not much error checking we
            // can do here, I'm afraid.  The final data is saved anyways.
            // Note the order: mode, uid+gid, gid, uid, mode.
            new FileInfo(RealFilename).Attributes = Stat.Attributes;
            return res;
        }

        /// <inheritdoc/>
        protected override bool SeekImpl(long offset, SeekOrigin whence)
        {
            if (FileStream == null)
                return SetError(0, "Missing file");

            if (!FileStream.CanSeek)
                return SetError(0, "Stream can't seek");

            try
            {
                FileStream.Seek(offset, whence);
                return true;
            }
            catch (Exception ex)
            {
                return SetError(0, $"Stream can't seek to {offset} from {whence}: {ex.Message}");
            }
        }

        /// <inheritdoc/>
        protected override bool WriteImpl(int num_bytes, byte[] data)
        {
            if (FileStream == null)
                return false;

            try
            {
                FileStream.Write(data, 0, num_bytes);
                return true;
            }
            catch (Exception ex)
            {
                return SetError(0, $"Stream can't write {num_bytes}: {ex.Message}");
            }
        }

        protected override long VPrintFImpl(string format, params string[] args)
        {
            if (FileStream == null)
                return -1;

            string temp = string.Format(format, args);
            byte[] tempBytes = Encoding.UTF8.GetBytes(temp);
            FileStream.Write(tempBytes, 0, tempBytes.Length);
            return temp.Length;
        }

        #endregion

        #region Utilities

        private static int RenameWrapper(string oldfilename, string newfilename)
        {
            try
            {
                System.IO.File.Move(oldfilename, newfilename);
                return 0;
            }
            catch
            {
                return 1;
            }
        }

        private static string FollowSymlinks(string filename, ref Exception error)
        {
            FileAttributes fa = System.IO.File.GetAttributes(filename);
            while (fa.HasFlag(FileAttributes.ReparsePoint))
            {
                // TODO: This should actually try to follow links
                break;
            }

            return filename;
        }

        private bool CloseFileHelper(bool seterr)
        {
            try
            {
                FileStream.Close();
                return true;
            }
            catch (Exception ex)
            {

                Error = new Exception($"Failed to close file: {ex.Message}");
                return false;
            }
            finally
            {
                FileStream = null;
            }
        }

        private bool UnlinkFileHelper()
        {
            if (TempFilename == null)
                return true;

            // TODO: This should actually try to unlink
            return true;
        }

        #endregion
    }
}
