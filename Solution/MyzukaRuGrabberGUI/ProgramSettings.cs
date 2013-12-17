using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using KlotosLib;

namespace MyzukaRuGrabberGUI
{
    /// <summary>
    /// Инкапсулирует все настройки программы. Синглтон.
    /// </summary>
    internal class ProgramSettings
    {
        #region Constants
        private const String _DEFAULT_USERAGENT = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:24.0) Gecko/20100101 Firefox/24.0";

        private const String _NUMBER_TOKEN = "%number%";
        private const String _TITLE_TOKEN = "%title%";
        private const String _ARTIST_TOKEN = "%artist%";
        private const String _ALBUM_TOKEN = "%album%";
        #endregion

        #region Constructors
        static ProgramSettings(){}

        /// <summary>
        /// true - default, false - empty
        /// </summary>
        /// <param name="Default"></param>
        private ProgramSettings(Boolean Default)
        {
            if (Default == true)
            {
                this._useDistinctFolder = false;
                this._useServerFilenames = true;
                this._maxDownloadThreads = (Byte)Environment.ProcessorCount;
                this._savedFilesPath = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                this._userAgent = _DEFAULT_USERAGENT;
                this._filenameTemplate = _NUMBER_TOKEN+". "+_ARTIST_TOKEN+" - "+_TITLE_TOKEN + " ["+_ALBUM_TOKEN+"]";
            }
        }

        private ProgramSettings
            (Boolean UseDistinctFolder, Boolean UseServerFilenames, Byte MaxDownloadThreads, String SavedFilesPath, String UserAgent, 
            String FilenameTemplate)
        {
            this._useDistinctFolder = UseDistinctFolder;
            this._useServerFilenames = UseServerFilenames;
            this._maxDownloadThreads = MaxDownloadThreads;
            this._savedFilesPath = SavedFilesPath;
            this._userAgent = UserAgent;
            this._filenameTemplate = FilenameTemplate;
        }
        #endregion

        private static readonly ProgramSettings _default = new ProgramSettings(true);

        private static ProgramSettings _instance = new ProgramSettings(true);

        internal static ProgramSettings Instance { get { return ProgramSettings._instance; }}

        internal static ProgramSettings Default { get {return ProgramSettings._default;}}

        internal static Boolean SetDefault()
        {
            if (ProgramSettings._default.IsDefault == true) { return false; }
            ProgramSettings._instance = ProgramSettings._default;
            return true;
        }
        
        /// <summary>
        /// Подготавливает и возвращает путь для сохранения файлов
        /// </summary>
        /// <param name="AlbumName"></param>
        /// <returns></returns>
        internal static String PrepareSavePath(String AlbumName)
        {
            String save_path;
            if (ProgramSettings.Instance.UseDistinctFolder == true)
            {
                String new_folder;
                Boolean success = FilePathTools.TryCleanFilename(AlbumName, out new_folder);
                if (success == false)
                { throw new ArgumentException("Невозможно исправить указанное название альбома '"+AlbumName+"'", "AlbumName"); }
                save_path = Path.Combine(ProgramSettings.Instance.SavedFilesPath, new_folder);
            }
            else
            {
                save_path = ProgramSettings.Instance.SavedFilesPath;
            }
            if (Directory.Exists(save_path) == false)
            {
                Directory.CreateDirectory(save_path);
            }
            return save_path;
        }

        internal static Dictionary<String, String> TransactionalApply(Boolean UseDistinctFolder, Boolean UseServerFilenames,
            String MaxDownloadThreads, String SavedFilesPath, String UserAgent, String FilenameTemplate)
        {
            Dictionary<String, String> output = new Dictionary<string, string>();
            if (UserAgent.HasAlphaNumericChars() == false)
            {
                output.Add(UserAgent.MemberName(_ => UserAgent), "User-Agent can not be empty");
            }
            if (SavedFilesPath.HasAlphaNumericChars() == false)
            {
                output.Add(SavedFilesPath.MemberName(_ => SavedFilesPath), "File path for saving files can not be empty");
            }
            else if (FilePathTools.IsValidFilePath(SavedFilesPath) == false)
            {
                output.Add(SavedFilesPath.MemberName(_ => SavedFilesPath), "File path is invalid");
            }
            Nullable<Byte> mdt = MaxDownloadThreads.TryParseNumber<Byte>(NumberStyles.None, CultureInfo.InvariantCulture);
            if (mdt == null)
            {
                output.Add(MaxDownloadThreads.MemberName(_ => MaxDownloadThreads), "Value '" + MaxDownloadThreads+"' is invalid");
            }
            Char[] intersects = FilenameTemplate.ToCharArray().Intersect(Path.GetInvalidFileNameChars()).ToArray();
            if (intersects.IsNullOrEmpty()==false)
            {
                output.Add(FilenameTemplate.MemberName(_ => FilenameTemplate)+"1", 
                    "Filename template contains next invalid characters: "+intersects.ConcatToString(", ")+".");
            }
            if (FilenameTemplate.Contains(_TITLE_TOKEN, StringComparison.Ordinal) == false &&
                FilenameTemplate.Contains(_NUMBER_TOKEN, StringComparison.Ordinal) == false)
            {
                output.Add(FilenameTemplate.MemberName(_ => FilenameTemplate)+"2", 
                    "Filename template must contain "+_TITLE_TOKEN+" or "+_NUMBER_TOKEN+" token");
            }

            if (output.Any() == true)
            {
                return output;
            }
            Byte value = mdt.Value > (Byte)99 ? (Byte)99 : mdt.Value;
            ProgramSettings._instance = 
                new ProgramSettings(UseDistinctFolder, UseServerFilenames, value, SavedFilesPath.Trim(), UserAgent.Trim(), FilenameTemplate.Trim());
            return null;
        }

        #region Instance fields
        private readonly Boolean _useDistinctFolder;
        /// <summary>
        /// Определяет, необходимо ли при сохранении песен альбома помещать их в отдельную папку (true) 
        /// или же помещать в общую папку (false)
        /// </summary>
        internal Boolean UseDistinctFolder { get { return this._useDistinctFolder; } }

        private readonly Boolean _useServerFilenames;
        /// <summary>
        /// Определяет, использовать ли поставляемые сервером (true) или собственно сгенерированные (false) имена 
        /// файлов песен и обложек при их сохранении
        /// </summary>
        internal Boolean UseServerFilenames { get { return this._useServerFilenames; } }

        private readonly Byte _maxDownloadThreads;
        /// <summary>
        /// Определяет количество потоков, в которых одновременно будут выполняться запросы на скачивание песен с сервера. 0 - неограничено.
        /// </summary>
        internal Byte MaxDownloadThreads { get { return this._maxDownloadThreads; } }

        private readonly String _savedFilesPath;
        /// <summary>
        /// Путь, по которому будут сохраняться файлы
        /// </summary>
        internal String SavedFilesPath { get { return this._savedFilesPath; } }

        private readonly String _userAgent;
        /// <summary>
        /// User-Agent, который будет использоваться для запросов к сайту
        /// </summary>
        internal String UserAgent { get { return this._userAgent; } }

        private readonly String _filenameTemplate;
        /// <summary>
        /// Шаблон имени сохраняемого файла песни
        /// </summary>
        public String FilenameTemplate {get { return this._filenameTemplate; }}
        #endregion

        internal Boolean IsDefault
        {
            get
            {
                Boolean result =
                    _instance.UseDistinctFolder == _default.UseDistinctFolder &&
                    _instance.UseServerFilenames == _default.UseServerFilenames &&
                    _instance.MaxDownloadThreads == _default.MaxDownloadThreads &&
                    _instance.UserAgent.Equals(_default.UserAgent, StringComparison.Ordinal) &&
                    _instance.SavedFilesPath.Equals(_default.SavedFilesPath, StringComparison.Ordinal) &&
                    _instance.FilenameTemplate.Equals(_default.FilenameTemplate, StringComparison.Ordinal);
                return result;
            }
        }
    }
}
