using System;
using System.Collections.Generic;
using System.Drawing;

namespace MyzukaRuGrabberCore.DataModels
{
    /// <summary>
    /// Представляет все данные, полученные из распарсенной страницы альбома. Неизменяемый класс.
    /// </summary>
    [Serializable()]
    public sealed class ParsedAlbum : ACommonData
    {
        /// <summary>
        /// Конструктор, заполняет экземпляр модели распарсенного альбома всеми необходимыми полными данными
        /// </summary>
        /// <param name="Header"></param>
        /// <param name="Songs"></param>
        /// <param name="CoverFile"></param>
        /// <param name="CoverImage"></param>
        public ParsedAlbum(AlbumHeader Header, List<OneSongHeader> Songs, DownloadedFile CoverFile, Bitmap CoverImage)
        {
            this._header = Header;
            this._songs = Songs;
            this._coverFile = CoverFile;
            this._coverImage = CoverImage;
        }

        private readonly AlbumHeader _header;
        /// <summary>
        /// Метаинформация по альбому
        /// </summary>
        public AlbumHeader Header { get { return this._header; } }

        private readonly List<OneSongHeader> _songs;
        /// <summary>
        /// Список всех песен в альбоме
        /// </summary>
        public List<OneSongHeader> Songs { get { return this._songs; } }

        private readonly DownloadedFile _coverFile;
        /// <summary>
        /// Файл обложки альбома или NULL, 
        /// если его не требуется скачать или не удалось скачать
        /// </summary>
        public override DownloadedFile CoverFile { get { return this._coverFile; } }

        private readonly Bitmap _coverImage;
        /// <summary>
        /// Изображение обложки альбома или NULL, 
        /// если его не требуется скачать, не удалось скачать или не удалось преобразовать в изображение
        /// </summary>
        public override Bitmap CoverImage { get { return this._coverImage; } }

        #region Inrterface
        public override String Title { get { return this._header.Title; } }

        public override String Artist { get { return this._header.Artist; } }

        public override String Album { get { return this._header.Title; } }

        public override String Genre { get { return this._header.Genre; } }

        public override String Uploader { get { return this._header.Uploader; } }

        public override Uri ItemLink { get { return this._header.AlbumPageURI; } }

        public override Uri CoverURI { get { return this._header.AlbumImageURI; } }

        #endregion
    }
}
