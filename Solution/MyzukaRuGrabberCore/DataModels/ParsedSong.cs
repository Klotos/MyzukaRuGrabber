using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using KlotosLib;

namespace MyzukaRuGrabberCore.DataModels
{
    /// <summary>
    /// Представляет все данные, полученные из распарсенной страницы одной песни. Неизменяемый класс.
    /// </summary>
    [Serializable()]
    public sealed class ParsedSong : CommonDataBase, IEquatable<ParsedSong>
    {
        /// <summary>
        /// Заполняет экземпляр всеми необходимыми данными
        /// </summary>
        /// <param name="Header"></param>
        /// <param name="DownloadLink"></param>
        /// <param name="AlbumLink"></param>
        /// <param name="CoverFile"></param>
        /// <param name="CoverImage"></param>
        public ParsedSong(OneSongHeader Header, Uri DownloadLink, Uri AlbumLink, DownloadedFile CoverFile, Bitmap CoverImage)
        {
            this._header = Header;
            this._downloadLink = DownloadLink;
            this._albumLink = AlbumLink;
            this._coverFile = CoverFile;
            this._coverImage = CoverImage;
        }
        
        private readonly OneSongHeader _header;
        /// <summary>
        /// Метаинформация по песне
        /// </summary>
        public OneSongHeader Header { get { return this._header; }}

        private readonly Uri _downloadLink;
        /// <summary>
        /// Ссылка на скачивание песни
        /// </summary>
        public Uri DownloadLink { get { return this._downloadLink; }}

        private readonly Uri _albumLink;
        /// <summary>
        /// Ссылка на альбом, к которому относится данная песня
        /// </summary>
        public Uri AlbumLink { get { return this._albumLink; }}

        private readonly DownloadedFile _coverFile;
        /// <summary>
        /// Файл обложки песни или NULL, 
        /// если его не требуется скачать или не удалось скачать
        /// </summary>
        public override DownloadedFile CoverFile { get { return this._coverFile; } }

        private readonly Bitmap _coverImage;
        /// <summary>
        /// Изображение обложки песни или NULL, 
        /// если его не требуется скачать, не удалось скачать или не удалось преобразовать в изображение
        /// </summary>
        public override Bitmap CoverImage { get { return this._coverImage; } }

        #region Inrterface
        public override String Title { get { return this._header.Title; }}

        public override String Artist { get { return this._header.Artist; } }

        public override String Album { get { return this._header.Album; } }

        public override String Genre { get { return this._header.Genre; } }

        public override String Uploader { get { return this._header.Uploader; } }

        public override Uri ItemLink { get { return this._header.SongPageURI; } }

        /// <summary>
        /// URI на обложку песни, дублирует соответствующее свойство из хидера
        /// </summary>
        public override Uri CoverURI { get { return this._header.SongImageURI; } }
        #endregion
        
        #region Overridings
        /// <summary>
        /// Сравнивает текущий экземпляр с указанным
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(ParsedSong other)
        {
            if (ReferenceEquals(null, other)) {return false;}
            if (ReferenceEquals(this, other)) {return true;}
            Boolean result = 
                this._header.Equals(other._header) && 
                this._albumLink == other._albumLink &&
                this._downloadLink == other._downloadLink && 
                this._coverFile.Equals(other._coverFile) &&
                this._coverImage.Size == other._coverImage.Size;
            return result;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) {return false;}
            if (ReferenceEquals(this, obj)) {return true;}
            return obj is ParsedSong && Equals((ParsedSong) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = (_header != null ? _header.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_albumLink != null ? _albumLink.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_downloadLink != null ? _downloadLink.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_coverFile != null ? _coverFile.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (_coverImage != null ? _coverImage.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion
    }
}
