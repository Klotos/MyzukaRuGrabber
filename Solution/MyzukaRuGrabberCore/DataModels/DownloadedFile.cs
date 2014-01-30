using System;
using System.IO;

namespace MyzukaRuGrabberCore.DataModels
{
    /// <summary>
    /// Представляет один скачанный файл. Неизменяемый класс.
    /// </summary>
    [Serializable()]
    public sealed class DownloadedFile : IDisposable, IEquatable<DownloadedFile>
    {
        /// <summary>
        /// Конструктор, заполняет экземпляр скачанного файла всеми необходимыми полными данными
        /// </summary>
        /// <param name="Filename"></param>
        /// <param name="Contentlength"></param>
        /// <param name="FileBody"></param>
        public DownloadedFile(String Filename, Int32 Contentlength, MemoryStream FileBody)
        {
            this._filename = Filename;
            this._contentLength = Contentlength;
            this._fileBody = FileBody;
        }
        
        private readonly String _filename;
        /// <summary>
        /// Название файла, полученное с сервера
        /// </summary>
        public String Filename { get { return this._filename; } }

        private readonly Int32 _contentLength;
        /// <summary>
        /// Байтовая длина файла, полученная с сервера
        /// </summary>
        public Int32 Contentlength { get { return this._contentLength; } }

        private readonly MemoryStream _fileBody;
        /// <summary>
        /// Тело файла
        /// </summary>
        public MemoryStream FileBody { get { return this._fileBody; } }

        /// <summary>
        /// Закрывает байтовый поток в памяти, представляющий данный файл
        /// </summary>
        public void Dispose()
        {
            this._fileBody.Dispose();
        }

        #region Overridings
        /// <summary>
        /// Сравнивает текущий экземпляр с указанным
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(DownloadedFile other)
        {
            if (ReferenceEquals(null, other)) {return false;}
            if (ReferenceEquals(this, other)) {return true;}
            Boolean result = 
                string.Equals(this._filename, other._filename, StringComparison.OrdinalIgnoreCase) && 
                this._contentLength == other._contentLength && 
                this._fileBody.Length == other._fileBody.Length;
            return result;
        }

        public override Boolean Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {return false;}
            if (ReferenceEquals(this, obj)) {return true;}
            return obj is DownloadedFile && Equals((DownloadedFile)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (_filename != null ? _filename.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ _contentLength;
                hashCode = (hashCode * 397) ^ (_fileBody != null ? _fileBody.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion
    }
}
