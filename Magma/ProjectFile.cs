using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;

namespace MagmaC3
{
    public class ProjectFile : IDisposable
    {
        public const int kProjectVersion = 24;
        private bool mIsDisposed;
        private IntPtr mData;
        private SaveStatus mSaveStatus;
        private string mFilename;
        private FileStream mFileLock;
        private readonly Dictionary<string, TrackInfo> mTrackCache;
        private MagmaFileLock mProjLock;

        public string ToolBuildVersion
        {
            get
            {
                return DataArrayUtl.FindTagStr(mData, "tool_version");
            }
        }

        public int ProjectFileVersion
        {
            get
            {
                return GetProjectVersion();
            }
        }

        public string SongName
        {
            get
            {
                return GetMetadataString("song_name");
            }
            set
            {
                SetMetadataString("song_name", value);
            }
        }

        public string ArtistName
        {
            get
            {
                return GetMetadataString("artist_name");
            }
            set
            {
                SetMetadataString("artist_name", value);
            }
        }

        public bool HasAlbum
        {
            get
            {
                if (GetMetadataInt("has_album") == 1)
                    return true;
                if (GetMetadataInt("has_album") == 0)
                    return false;
                throw new MagmaException("Invalid project file setting for 'has_album'.");
            }
            set { SetMetadataInt("has_album", value ? 1 : 0); }
        }

        public string AlbumName
        {
            get
            {
                return GetMetadataString("album_name");
            }
            set
            {
                SetMetadataString("album_name", value);
            }
        }

        public int YearReleased
        {
            get
            {
                return GetMetadataInt("year_released");
            }
            set
            {
                SetMetadataInt("year_released", value);
            }
        }

        public int TrackNumber
        {
            get
            {
                return GetMetadataInt("track_number");
            }
            set
            {
                SetMetadataInt("track_number", value);
            }
        }


       public string Genre
        {
            get
            {
                return GetMetadataSym("genre");
            }
            set
            {
                SetMetadataSym("genre", value);
            }
        }

        public string SubGenre
        {
            get
            {
                return GetMetadataSym("sub_genre");
            }
            set
            {
                SetMetadataSym("sub_genre", value);
            }
        }

        public string Country
        {
            get
            {
                return GetMetadataSym("country");
            }
            set
            {
                SetMetadataSym("country", value);
            }
        }

        public string Author
        {
            get
            {
                return GetMetadataString("author");
            }
            set
            {
                SetMetadataString("author", value);
            }
        }

        public string RecordLabel
        {
            get
            {
                return GetMetadataString("release_label");
            }
            set
            {
                SetMetadataString("release_label", value);
            }
        }

        public int Price
        {
            get
            {
                return GetMetadataInt("price");
            }
            set
            {
                SetMetadataInt("price", value);
            }
        }

        public int RankGuitar
        {
            get
            {
                return GetGamedataInt("rank_guitar");
            }
            set
            {
                SetGamedataInt("rank_guitar", value);
            }
        }

        public int RankBass
        {
            get
            {
                return GetGamedataInt("rank_bass");
            }
            set
            {
                SetGamedataInt("rank_bass", value);
            }
        }

        public int RankProKeys
        {
            get
            {
                return GetGamedataInt("rank_pro_keys");
            }
            set
            {
                SetGamedataInt("rank_pro_keys", value);
            }
        }

        public int RankKeys
        {
            get
            {
                return GetGamedataInt("rank_keys");
            }
            set
            {
                SetGamedataInt("rank_keys", value);
            }
        }

        public int RankDrum
        {
            get
            {
                return GetGamedataInt("rank_drum");
            }
            set
            {
                SetGamedataInt("rank_drum", value);
            }
        }

        public int RankVocals
        {
            get
            {
                return GetGamedataInt("rank_vocals");
            }
            set
            {
                SetGamedataInt("rank_vocals", value);
            }
        }

        public int RankBand
        {
            get
            {
                return GetGamedataInt("rank_band");
            }
            set
            {
                SetGamedataInt("rank_band", value);
            }
        }

        public int Tempo
        {
            get
            {
                return GetGamedataInt("anim_tempo");
            }
            set
            {
                SetGamedataInt("anim_tempo", value);
            }
        }

        public string Gender
        {
            get
            {
                return GetGamedataSym("vocal_gender");
            }
            set
            {
                SetGamedataSym("vocal_gender", value);
            }
        }

        public int ScrollSpeed
        {
            get
            {
                return GetGamedataInt("vocal_scroll_speed");
            }
            set
            {
                SetGamedataInt("vocal_scroll_speed", value);
            }
        }

        public string Percussion
        {
            get
            {
                return GetGamedataSym("vocal_percussion");
            }
            set
            {
                SetGamedataSym("vocal_percussion", value);
            }
        }

        public bool LangEnglish
        {
            get
            {
                return GetLanguage("english");
            }
            set
            {
                SetLanguage("english", value);
            }
        }

        public bool LangFrench
        {
            get
            {
                return GetLanguage("french");
            }
            set
            {
                SetLanguage("french", value);
            }
        }

        public bool LangItalian
        {
            get
            {
                return GetLanguage("italian");
            }
            set
            {
                SetLanguage("italian", value);
            }
        }

        public bool LangSpanish
        {
            get
            {
                return GetLanguage("spanish");
            }
            set
            {
                SetLanguage("spanish", value);
            }
        }

        public bool LangGerman
        {
            get
            {
                return GetLanguage("german");
            }
            set
            {
                SetLanguage("german", value);
            }
        }

        public bool LangJapanese
        {
            get
            {
                return GetLanguage("japanese");
            }
            set
            {
                SetLanguage("japanese", value);
            }
        }

        public string DrumLayout
        {
            get
            {
                return DataArrayUtl.FindTagSym(DataArrayUtl.FindTagArray(mData, "tracks"), "drum_layout");
            }
            set
            {
                DataArrayUtl.SetTagSym(DataArrayUtl.FindTagArray(mData, "tracks"), "drum_layout", value);
                OnChangeToUnsavedData();
            }
        }

        public string MidiFile
        {
            get
            {
                return DataArrayUtl.FindTagStr(DataArrayUtl.FindTagArray(mData, "midi"), "file");
            }
            set
            {
                var tagArray = DataArrayUtl.FindTagArray(mData, "midi");
                if (DataArrayUtl.FindTagStr(tagArray, "file") == value)
                    return;
                DataArrayUtl.SetTagStr(tagArray, "file", value);
                OnChangeToUnsavedData();
            }
        }

        public string AutogenTheme
        {
            get
            {
                return DataArrayUtl.FindTagStr(DataArrayUtl.FindTagArray(mData, "midi"), "autogen_theme");
            }
            set
            {
                var tagArray = DataArrayUtl.FindTagArray(mData, "midi");
                if (DataArrayUtl.FindTagStr(tagArray, "autogen_theme") == value)
                    return;
                DataArrayUtl.SetTagStr(tagArray, "autogen_theme", value);
                OnChangeToUnsavedData();
            }
        }

        public string AlbumArt
        {
            get
            {
                return DataArrayUtl.FindTagStr(DataArrayUtl.FindTagArray(mData, "album_art"), "file");
            }
            set
            {
                var tagArray = DataArrayUtl.FindTagArray(mData, "album_art");
                if (DataArrayUtl.FindTagStr(tagArray, "file") == value)
                    return;
                DataArrayUtl.SetTagStr(tagArray, "file", value);
                OnChangeToUnsavedData();
            }
        }

        public string DestinationFile
        {
            get
            {
                return DataArrayUtl.FindTagStr(mData, "destination_file");
            }
            set
            {
                if (DataArrayUtl.FindTagStr(mData, "destination_file") == value)
                    return;
                DataArrayUtl.SetTagStr(mData, "destination_file", value);
                OnChangeToUnsavedData();
            }
        }

        public int PreviewStart
        {
            get
            {
                return GetGamedataInt("preview_start_ms");
            }
            set
            {
                SetGamedataInt("preview_start_ms", value);
            }
        }

        public string DryVoxFile
        {
            get
            {
                return DataArrayUtl.FindTagStr(DataArrayUtl.FindTagArray(DataArrayUtl.FindTagArray(mData, "dry_vox"), "part0"), "file");
            }
            set
            {
                var tagArray = DataArrayUtl.FindTagArray(DataArrayUtl.FindTagArray(mData, "dry_vox"), "part0");
                if (DataArrayUtl.FindTagStr(tagArray, "file") == value)
                    return;
                DataArrayUtl.SetTagStr(tagArray, "file", value);
                UpdateVocalPartsCount();
                OnChangeToUnsavedData();
            }
        }

        public bool DryVoxEnabled
        {
            get
            {
                return GetTrack("vocals").Enabled;
            }
        }

        public string DryVoxHarmony2File
        {
            get
            {
                return DataArrayUtl.FindTagStr(DataArrayUtl.FindTagArray(DataArrayUtl.FindTagArray(mData, "dry_vox"), "part1"), "file");
            }
            set
            {
                var tagArray = DataArrayUtl.FindTagArray(DataArrayUtl.FindTagArray(mData, "dry_vox"), "part1");
                if (DataArrayUtl.FindTagStr(tagArray, "file") == value)
                    return;
                DataArrayUtl.SetTagStr(tagArray, "file", value);
                UpdateVocalPartsCount();
                OnChangeToUnsavedData();
            }
        }

        public bool DryVoxHarmony2Enabled
        {
            get
            {
                return Convert.ToBoolean(DataArrayUtl.FindTagInt(DataArrayUtl.FindTagArray(DataArrayUtl.FindTagArray(mData, "dry_vox"), "part1"), "enabled"));
            }
            set
            {
                var tagArray = DataArrayUtl.FindTagArray(DataArrayUtl.FindTagArray(mData, "dry_vox"), "part1");
                if (Convert.ToBoolean(DataArrayUtl.FindTagInt(tagArray, "enabled")) == value)
                    return;
                DataArrayUtl.SetTagInt(tagArray, "enabled", Convert.ToInt32(value));
                UpdateVocalPartsCount();
                OnChangeToUnsavedData();
            }
        }

        public string DryVoxHarmony3File
        {
            get
            {
                return DataArrayUtl.FindTagStr(DataArrayUtl.FindTagArray(DataArrayUtl.FindTagArray(mData, "dry_vox"), "part2"), "file");
            }
            set
            {
                var tagArray = DataArrayUtl.FindTagArray(DataArrayUtl.FindTagArray(mData, "dry_vox"), "part2");
                if (DataArrayUtl.FindTagStr(tagArray, "file") == value)
                    return;
                DataArrayUtl.SetTagStr(tagArray, "file", value);
                UpdateVocalPartsCount();
                OnChangeToUnsavedData();
            }
        }

        public bool DryVoxHarmony3Enabled
        {
            get
            {
                return Convert.ToBoolean(DataArrayUtl.FindTagInt(DataArrayUtl.FindTagArray(DataArrayUtl.FindTagArray(mData, "dry_vox"), "part2"), "enabled"));
            }
            set
            {
                var tagArray = DataArrayUtl.FindTagArray(DataArrayUtl.FindTagArray(mData, "dry_vox"), "part2");
                if (Convert.ToBoolean(DataArrayUtl.FindTagInt(tagArray, "enabled")) == value)
                    return;
                DataArrayUtl.SetTagInt(tagArray, "enabled", Convert.ToInt32(value));
                UpdateVocalPartsCount();
                OnChangeToUnsavedData();
            }
        }

        public float TuningOffset
        {
            get
            {
                return DataArrayUtl.FindTagFloat(DataArrayUtl.FindTagArray(mData, "dry_vox"), "tuning_offset_cents");
            }
            set
            {
                var tagArray = DataArrayUtl.FindTagArray(mData, "dry_vox");
                if ((double)DataArrayUtl.FindTagFloat(tagArray, "tuning_offset_cents") == value)
                    return;
                DataArrayUtl.SetTagFloat(tagArray, "tuning_offset_cents", value);
                OnChangeToUnsavedData();
            }
        }

        public float GuidePitchVolume
        {
            get
            {
                return DataArrayUtl.FindTagFloat(DataArrayUtl.FindTagArray(mData, "gamedata"), "guide_pitch_volume");
            }
            set
            {
                var tagArray = DataArrayUtl.FindTagArray(mData, "gamedata");
                if ((double)DataArrayUtl.FindTagFloat(tagArray, "guide_pitch_volume") == value)
                    return;
                DataArrayUtl.SetTagFloat(tagArray, "guide_pitch_volume", value);
                OnChangeToUnsavedData();
            }
        }

        public ProjectFile()
        {
            var dataStatus = Wrapper.CloneDataTemplate("rbproj", out mData);
            if (dataStatus != DataStatus.kSuccess)
            {
                Console.Write("CloneDataTemplate returned: " + dataStatus);
                Console.WriteLine(", so errors may happen in the future...");
            }
            UpdateVersions();
            mSaveStatus = SaveStatus.kNoData;
            mFilename = "";
            mTrackCache = new Dictionary<string, TrackInfo>();
            mProjLock = null;
            mIsDisposed = false;
        }

        ~ProjectFile()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool isDisposing)
        {
            if (!mIsDisposed)
            {
                if (isDisposing)
                    UnlockProj();
                Unlock();
                Wrapper.SafeRelease(ref mData);
            }
            mIsDisposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public SaveStatus GetSaveStatus()
        {
            return mSaveStatus;
        }

        public void SetSaveStatus(SaveStatus saveStatus)
        {
            mSaveStatus = saveStatus;
        }

        protected void OnChangeToUnsavedData()
        {
            if (mSaveStatus == SaveStatus.kNoData)
            {
                mSaveStatus = SaveStatus.kUnsavedNoFilename;
            }
            else
            {
                if (mSaveStatus != SaveStatus.kSaved)
                    return;
                mSaveStatus = SaveStatus.kUnsaved;
            }
        }

        public bool HasUnsavedChanges()
        {
            if (mSaveStatus != SaveStatus.kUnsaved)
                return mSaveStatus == SaveStatus.kUnsavedNoFilename;
            return true;
        }

        public bool HasFilename()
        {
            return mFilename != "";
        }

        public string GetFilename()
        {
            return mFilename;
        }

        protected void SetFilename(string strFilename)
        {
            mFilename = strFilename;
        }

        public void ReadFile(string strFilename)
        {
            var flag = !(mProjLock != null && mProjLock.GetName() == MagmaFileLock.GetLockNameFromPath(strFilename));
            var magmaFileLock1 = (MagmaFileLock)null;
            if (flag)
                magmaFileLock1 = LockFile(strFilename);
            IntPtr dataArray;
            var fileStatus = Wrapper.ReadDtaFile(strFilename, out dataArray);
            var message = "Unknown error opening your project file " + strFilename;
            if (fileStatus != FileStatus.kSuccess)
            {
                if (magmaFileLock1 != null)
                {
                    magmaFileLock1.Dispose();
                }
                switch (fileStatus)
                {
                    case FileStatus.kErrorUnknown:
                    case FileStatus.kErrorDuringParse:
                    case FileStatus.kErrorBadOutPtr:
                    case FileStatus.kErrorBadDataArrayPtr:
                        message = "An error occurred loading your project file. Please make sure that the file is a .rbproj and is not corrupt. Attempted file: " + strFilename;
                        break;
                    case FileStatus.kErrorFileNotFound:
                        message = "Project file is in use or not found\n" + strFilename;
                        break;
                }
                throw new MagmaException(message);
            }
            Wrapper.SafeRelease(ref mData);
            Unlock();
            mData = dataArray;
            SetFilename(strFilename);
            if (!PatchPreviousProjectVersions())
            {
                if (magmaFileLock1 != null)
                {
                    magmaFileLock1.Dispose();
                }
                throw new MagmaException("Unsupported rbproj version. This file is version " + Convert.ToString(GetProjectVersion()));
            }
            if (magmaFileLock1 != null)
            {
                UnlockProj();
                mProjLock = magmaFileLock1;
            }
            Lock();
            UpdateVersions();
            SetSaveStatus(SaveStatus.kSaved);
        }

        protected bool PatchPreviousProjectVersions()
        {
            var flag = false;
            if (GetProjectVersion() < 3)
                return false;
            switch (GetProjectVersion())
            {
                case 3:
                    DataArrayUtl.InsertTagStrAtEnd(mData, "destination_file", GetDefaultDestination());
                    flag = true;
                    break;
                case 4:
                    DataArrayUtl.InsertTagIntAtEnd(DataArrayUtl.FindTagArray(mData, "metadata"), "has_album", 1);
                    if (TrackNumber == 0)
                        TrackNumber = 1;
                    flag = true;
                    break;
            }
            if (GetProjectVersion() < 20)
            {
                var tagArray1 = DataArrayUtl.FindTagArray(mData, "gamedata");
                DataArrayUtl.InsertTagIntAtEnd(tagArray1, "rank_keys", 1);
                DataArrayUtl.InsertTagIntAtEnd(tagArray1, "rank_pro_keys", 1);
                var tagArray2 = DataArrayUtl.FindTagArray(mData, "dry_vox");
                var tagStr = DataArrayUtl.FindTagStr(tagArray2, "file");
                DataArrayUtl.InsertTagStrAtEnd(DataArrayUtl.InsertTagArrayAtEnd(tagArray2, "part0"), "file", tagStr);
                DataArrayUtl.InsertTagStrAtEnd(DataArrayUtl.InsertTagArrayAtEnd(tagArray2, "part1"), "file", "");
                DataArrayUtl.InsertTagStrAtEnd(DataArrayUtl.InsertTagArrayAtEnd(tagArray2, "part2"), "file", "");
                DataArrayUtl.RemoveTagArray(tagArray2, "file");
                DataArrayUtl.InsertTagIntAtEnd(tagArray1, "vocal_parts", tagStr.Length > 0 ? 1 : 0);
                var data = DataArrayUtl.InsertTagArrayAtEnd(DataArrayUtl.FindTagArray(mData, "tracks"), "keys");
                DataArrayUtl.InsertTagIntAtEnd(data, "enabled", 0);
                DataArrayUtl.InsertTagIntAtEnd(data, "channels", 0);
                DataArrayUtl.InsertTagArrayAtEnd(data, "pan");
                DataArrayUtl.InsertTagArrayAtEnd(data, "vol");
                DataArrayUtl.InsertTagStrAtEnd(data, "file", "");
                flag = true;
            }
            if (GetProjectVersion() < 21)
            {
                DataArrayUtl.InsertTagStrAtEnd(DataArrayUtl.FindTagArray(mData, "midi"), "autogen_theme", "");
                flag = true;
            }
            if (GetProjectVersion() < 22)
            {
                var tagArray = DataArrayUtl.FindTagArray(mData, "languages");
                DataArrayUtl.InsertTagIntAtEnd(tagArray, "german", 0);
                DataArrayUtl.InsertTagIntAtEnd(tagArray, "japanese", 0);
                flag = true;
            }
            if (GetProjectVersion() < 23)
            {
                DataArrayUtl.AddTagFloatAtEnd(DataArrayUtl.FindTagArray(mData, "gamedata"), "guide_pitch_volume", -3f);
                flag = true;
            }
            if (GetProjectVersion() < 24)
            {
                var tagArray1 = DataArrayUtl.FindTagArray(mData, "dry_vox");
                var tagArray2 = DataArrayUtl.FindTagArray(tagArray1, "part0");
                var tagArray3 = DataArrayUtl.FindTagArray(tagArray1, "part1");
                var tagArray4 = DataArrayUtl.FindTagArray(tagArray1, "part2");
                var tagStr1 = DataArrayUtl.FindTagStr(tagArray2, "file");
                var tagStr2 = DataArrayUtl.FindTagStr(tagArray3, "file");
                var tagStr3 = DataArrayUtl.FindTagStr(tagArray4, "file");
                DataArrayUtl.InsertTagIntAtEnd(tagArray2, "enabled", tagStr1.Length > 0 ? 1 : 0);
                DataArrayUtl.InsertTagIntAtEnd(tagArray3, "enabled", tagStr2.Length > 0 ? 1 : 0);
                DataArrayUtl.InsertTagIntAtEnd(tagArray4, "enabled", tagStr3.Length > 0 ? 1 : 0);
                flag = true;
            }
            return flag || GetProjectVersion() == 24;
        }

        public string GetDefaultDestination()
        {
            return mFilename.Replace(".rbproj", ".rba");
        }

        private static MagmaFileLock LockFile(string strFile)
        {
            var magmaFileLock = new MagmaFileLock(strFile);
            switch (magmaFileLock.GetStatus())
            {
                case MagmaFileLock.FileLockStatus.kMFL_LockAcquired:
                    return magmaFileLock;
                case MagmaFileLock.FileLockStatus.kMFL_LockTimeout:
                    throw new MagmaException("Project file '" + Path.GetFileName(strFile) + "' is already open in another window.");
                default:
                    throw new MagmaException("There was a problem obtaining a lock on project file.");
            }
        }

        public void LockProj()
        {
            UnlockProj();
            mProjLock = LockFile(mFilename);
        }

        public void UnlockProj()
        {
            if (mProjLock == null)
                return;
            mProjLock.Dispose();
            mProjLock = null;
        }

        public void Lock()
        {
            Unlock();
            try
            {
                mFileLock = File.OpenWrite(mFilename);
                mFileLock.Lock(0L, 1L);
            }
            catch
            {
                throw new MagmaException("Project file is in use or not writeable.");
            }
        }

        public void Unlock()
        {
            if (mFileLock == null)
                return;
            if (!mFileLock.CanWrite)
                return;
            try
            {
                mFileLock.Unlock(0L, 1L);
            }
            finally
            {
                mFileLock.Dispose();
            }
        }

        public void WriteFile(string strFilename)
        {
            var flag = !(mProjLock != null && mProjLock.GetName() == MagmaFileLock.GetLockNameFromPath(strFilename));
            var magmaFileLock = (MagmaFileLock)null;
            if (flag)
                magmaFileLock = LockFile(strFilename);
            Unlock();
            var fileStatus = Wrapper.WriteDtaFile(strFilename, mData);
            if (fileStatus != FileStatus.kSuccess)
            {
                if (magmaFileLock != null)
                    magmaFileLock.Dispose();
                throw new MagmaException("WriteDtaFile(" + strFilename + ") returned " + fileStatus + ".");
            }
            SetFilename(strFilename);
            if (magmaFileLock != null)
            {
                UnlockProj();
                mProjLock = magmaFileLock;
            }
            Lock();
            SetSaveStatus(SaveStatus.kSaved);
        }

        public void WriteFile()
        {
            if (mSaveStatus == SaveStatus.kNoData || mSaveStatus == SaveStatus.kUnsavedNoFilename)
                throw new MagmaException("WriteFile() called on unnamed project.");
            WriteFile(GetFilename());
        }

        protected int GetProjectVersion()
        {
            int n;
            var tagInt = Wrapper.FindTagInt(mData, "project_version", out n);
            switch (tagInt)
            {
                case DataStatus.kErrorTagNotFound:
                    n = 0;
                    goto case DataStatus.kSuccess;
                case DataStatus.kSuccess:
                    return n;
                default:
                    throw new MagmaException("FindTagInt(project_version) returned " + tagInt + ".");
            }
        }

        protected void UpdateVersions()
        {
            DataArrayUtl.SetOrInsertTagIntAtStart(mData, "project_version", 24);
            DataArrayUtl.SetOrInsertTagStrAtStart(mData, "tool_version", SystemUtl.GetBuildVersion());
        }

        protected string GetMetadataString(string strTag)
        {
            return DataArrayUtl.FindTagStr(DataArrayUtl.FindTagArray(mData, "metadata"), strTag);
        }

        protected string GetMetadataSym(string strTag)
        {
            return DataArrayUtl.FindTagSym(DataArrayUtl.FindTagArray(mData, "metadata"), strTag);
        }

        protected int GetMetadataInt(string strTag)
        {
            return DataArrayUtl.FindTagInt(DataArrayUtl.FindTagArray(mData, "metadata"), strTag);
        }

        protected void SetMetadataString(string strTag, string strValue)
        {
            var tagArray = DataArrayUtl.FindTagArray(mData, "metadata");
            if (DataArrayUtl.FindTagStr(tagArray, strTag) == strValue)
                return;
            DataArrayUtl.SetTagStr(tagArray, strTag, strValue);
            OnChangeToUnsavedData();
        }

        protected void SetMetadataSym(string strTag, string strValue)
        {
            var tagArray = DataArrayUtl.FindTagArray(mData, "metadata");
            if (DataArrayUtl.FindTagSym(tagArray, strTag).ToString(CultureInfo.InvariantCulture) == strValue)
                return;
            DataArrayUtl.SetTagSym(tagArray, strTag, strValue);
            OnChangeToUnsavedData();
        }

        protected void SetMetadataInt(string strTag, int nValue)
        {
            var tagArray = DataArrayUtl.FindTagArray(mData, "metadata");
            if (DataArrayUtl.FindTagInt(tagArray, strTag) == nValue)
                return;
            DataArrayUtl.SetTagInt(tagArray, strTag, nValue);
            OnChangeToUnsavedData();
        }

        protected int GetGamedataInt(string strTag)
        {
            return DataArrayUtl.FindTagInt(DataArrayUtl.FindTagArray(mData, "gamedata"), strTag);
        }

        protected string GetGamedataSym(string strTag)
        {
            return DataArrayUtl.FindTagSym(DataArrayUtl.FindTagArray(mData, "gamedata"), strTag);
        }

        protected string GetGamedataString(string strTag)
        {
            return DataArrayUtl.FindTagStr(DataArrayUtl.FindTagArray(mData, "gamedata"), strTag);
        }

        protected void SetGamedataInt(string strTag, int nValue)
        {
            var tagArray = DataArrayUtl.FindTagArray(mData, "gamedata");
            if (DataArrayUtl.FindTagInt(tagArray, strTag) == nValue)
                return;
            DataArrayUtl.SetTagInt(tagArray, strTag, nValue);
            OnChangeToUnsavedData();
        }

        protected void SetGamedataSym(string strTag, string strValue)
        {
            var tagArray = DataArrayUtl.FindTagArray(mData, "gamedata");
            if (DataArrayUtl.FindTagSym(tagArray, strTag) == strValue)
                return;
            DataArrayUtl.SetTagSym(tagArray, strTag, strValue);
            OnChangeToUnsavedData();
        }

        protected void SetGamedataString(string strTag, string strValue)
        {
            var tagArray = DataArrayUtl.FindTagArray(mData, "gamedata");
            if (DataArrayUtl.FindTagStr(tagArray, strTag).ToString(CultureInfo.InvariantCulture) == strValue)
                return;
            DataArrayUtl.SetTagStr(tagArray, strTag, strValue);
            OnChangeToUnsavedData();
        }

        protected bool GetLanguage(string strTag)
        {
            return DataArrayUtl.FindTagInt(DataArrayUtl.FindTagArray(mData, "languages"), strTag) != 0;
        }

        protected void SetLanguage(string strTag, bool nValue)
        {
            var tagArray = DataArrayUtl.FindTagArray(mData, "languages");
            if (DataArrayUtl.FindTagInt(tagArray, strTag) != 0 == nValue)
                return;
            DataArrayUtl.SetTagInt(tagArray, strTag, nValue ? 1 : 0);
            OnChangeToUnsavedData();
        }

        public void RemoveAllTracks()
        {
            DataArrayUtl.ReleaseContents(DataArrayUtl.FindTagArray(mData, "tracks"));
            OnChangeToUnsavedData();
        }

        public void AddTrack(TrackInfo track)
        {
            var data = DataArrayUtl.InsertArrayAtEnd(DataArrayUtl.FindTagArray(mData, "tracks"));
            DataArrayUtl.InsertTagIntAtEnd(data, "enabled", track.Enabled ? 1 : 0);
            DataArrayUtl.InsertTagSymAtEnd(data, "type", track.TrackType);
            DataArrayUtl.InsertTagIntAtEnd(data, "channels", track.NumChannels);
            switch (track.NumChannels)
            {
                case 1:
                    DataArrayUtl.AddTagFloatAtEnd(data, "pan", track.PanLeft);
                    DataArrayUtl.AddTagFloatAtEnd(data, "vol", track.VolLeft);
                    break;
                case 2:
                    DataArrayUtl.AddTagFloatAtEnd(data, "pan", track.PanLeft);
                    DataArrayUtl.AddTagFloatAtEnd(data, "pan", track.PanRight);
                    DataArrayUtl.AddTagFloatAtEnd(data, "vol", track.VolLeft);
                    DataArrayUtl.AddTagFloatAtEnd(data, "vol", track.VolRight);
                    break;
                default:
                    throw new MagmaException("Invalid NumChannels: " + track.NumChannels);
            }
            DataArrayUtl.InsertTagStrAtEnd(data, "file", track.Filename);
            OnChangeToUnsavedData();
        }

        public void SetTrack(TrackInfo track)
        {
            var tagArray = DataArrayUtl.FindTagArray(DataArrayUtl.FindTagArray(mData, "tracks"), track.TrackType);
            DataArrayUtl.ReleaseContents(tagArray);
            DataArrayUtl.InsertTagIntAtEnd(tagArray, "enabled", track.Enabled ? 1 : 0);
            DataArrayUtl.InsertTagIntAtEnd(tagArray, "channels", track.NumChannels);
            if (track.TrackType == "vocals")
                DataArrayUtl.SetTagInt(DataArrayUtl.FindTagArray(DataArrayUtl.FindTagArray(mData, "dry_vox"), "part0"), "enabled", track.Enabled ? 1 : 0);
            switch (track.NumChannels)
            {
                case 0:
                    DataArrayUtl.AddTagFloatAtEnd(tagArray, "pan", 0.0f);
                    DataArrayUtl.AddTagFloatAtEnd(tagArray, "vol", 0.0f);
                    break;
                case 1:
                    DataArrayUtl.AddTagFloatAtEnd(tagArray, "pan", track.PanLeft);
                    DataArrayUtl.AddTagFloatAtEnd(tagArray, "vol", track.VolLeft);
                    break;
                case 2:
                    DataArrayUtl.AddTagFloatAtEnd(tagArray, "pan", track.PanLeft);
                    DataArrayUtl.AddTagFloatAtEnd(tagArray, "pan", track.PanRight);
                    DataArrayUtl.AddTagFloatAtEnd(tagArray, "vol", track.VolLeft);
                    DataArrayUtl.AddTagFloatAtEnd(tagArray, "vol", track.VolRight);
                    break;
                default:
                    throw new MagmaException("Invalid NumChannels: " + track.NumChannels);
            }
            DataArrayUtl.InsertTagStrAtEnd(tagArray, "file", track.Filename);
            mTrackCache[track.TrackType] = track;
            OnChangeToUnsavedData();
        }

        public TrackInfo GetTrack(string trackType)
        {
            if (mTrackCache.ContainsKey(trackType))
            {
                return mTrackCache[trackType];
            }
            var tagArray1 = DataArrayUtl.FindTagArray(DataArrayUtl.FindTagArray(mData, "tracks"), trackType);
            var tagStr = DataArrayUtl.FindTagStr(tagArray1, "file");
            var track = new TrackInfo {TrackType = trackType};
            var tagInt = DataArrayUtl.FindTagInt(tagArray1, "enabled");
            switch (tagInt)
            {
                case 0:
                case 1:
                    track.Enabled = tagInt == 1;
                    if (tagStr == "")
                        return track;
                    track.InitFromFile(tagStr, trackType);
                    switch (track.NumChannels > 0 ? track.NumChannels : DataArrayUtl.FindTagInt(tagArray1, "channels"))
                    {
                        case 1:
                            track.PanLeft = DataArrayUtl.FindTagFloat(tagArray1, "pan");
                            track.VolLeft = DataArrayUtl.FindTagFloat(tagArray1, "vol");
                            break;
                        case 2:
                            var tagArray2 = DataArrayUtl.FindTagArray(tagArray1, "pan");
                            track.PanLeft = DataArrayUtl.GetFloat(tagArray2, 1U);
                            track.PanRight = DataArrayUtl.GetFloat(tagArray2, 2U);
                            var tagArray3 = DataArrayUtl.FindTagArray(tagArray1, "vol");
                            track.VolLeft = DataArrayUtl.GetFloat(tagArray3, 1U);
                            track.VolRight = DataArrayUtl.GetFloat(tagArray3, 2U);
                            break;
                    }
                    if (!track.IsValid())
                        return track;
                    if (track.NumChannels != DataArrayUtl.FindTagInt(tagArray1, "channels"))
                    {
                        track.SetDefaultPan();
                        track.SetDefaultVol();
                        SetTrack(track);
                    }
                    mTrackCache[trackType] = track;
                    return track;
                default:
                    throw new MagmaException("Invalid value for 'enabled' on track " + trackType);
            }
        }

        private void UpdateVocalPartsCount()
        {
            var nValue = 0;
            if (DryVoxEnabled)
                ++nValue;
            if (DryVoxHarmony2Enabled)
                ++nValue;
            if (DryVoxHarmony3Enabled)
                ++nValue;
            SetGamedataInt("vocal_parts", nValue);
        }
    }
}
