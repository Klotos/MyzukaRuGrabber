using System;
using System.IO;

namespace MyzukaRuGrabberCore.DataModels
{
    /// <summary>
    /// Представляет один скачанный файл. Неизменяемый класс.
    /// </summary>
    [Serializable()]
    public sealed class DownloadedFile : IDisposable
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
    }
}
