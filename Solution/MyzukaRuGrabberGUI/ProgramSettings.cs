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
                this._userAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64; rv:24.0) Gecko/20100101 Firefox/24.0";
            }
        }

        private ProgramSettings
            (Boolean UseDistinctFolder, Boolean UseServerFilenames, Byte MaxDownloadThreads, String SavedFilesPath, String UserAgent)
        {
            this._useDistinctFolder = UseDistinctFolder;
            this._useServerFilenames = UseServerFilenames;
            this._maxDownloadThreads = MaxDownloadThreads;
            this._savedFilesPath = SavedFilesPath;
            this._userAgent = UserAgent;
        }

        private static ProgramSettings _instance = new ProgramSettings(true);

        internal static ProgramSettings Instance { get { return ProgramSettings._instance; }}

        internal static ProgramSettings Default { get {return new ProgramSettings(true);}}

        internal static void SetDefault()
        {
            ProgramSettings._instance = new ProgramSettings(true);
        }

        internal static Dictionary<String, String> TransactionalApply(Boolean UseDistinctFolder, Boolean UseServerFilenames,
            String MaxDownloadThreads, String SavedFilesPath, String UserAgent)
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
            if (output.Any() == true)
            {
                return output;
            }
            Byte value = mdt.Value > (Byte)99 ? (Byte)99 : mdt.Value;
            ProgramSettings._instance = 
                new ProgramSettings(UseDistinctFolder, UseServerFilenames, value, SavedFilesPath.Trim(), UserAgent.Trim());
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

        

        

        
        #endregion
    }
}
