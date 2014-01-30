using System;

namespace MyzukaRuGrabberCore.DataModels
{
    /// <summary>
    /// Модель данных распарсенного хидера альбома, без песен. Неизменяемый класс.
    /// </summary>
    [Serializable()]
    public sealed class AlbumHeader : ICommonHeader, IEquatable<AlbumHeader>
    {
        /// <summary>
        /// Конструктор, заполняет экземпляр хидера альбома всеми необходимыми полными данными
        /// </summary>
        /// <param name="Title"></param>
        /// <param name="Genre"></param>
        /// <param name="Artist"></param>
        /// <param name="ReleaseDate"></param>
        /// <param name="Type"></param>
        /// <param name="SongsCount"></param>
        /// <param name="Format"></param>
        /// <param name="Uploader"></param>
        /// <param name="Updater"></param>
        /// <param name="Description"></param>
        /// <param name="AlbumImageURI"></param>
        /// <param name="AlbumPageURI"></param>
        public AlbumHeader
            (String Title, String Genre, String Artist, String ReleaseDate, String Type, Nullable<Byte> SongsCount,
            String Format, String Uploader, String Updater, String Description, Uri AlbumImageURI, Uri AlbumPageURI)
        {
            this._title = Title;
            this._genre = Genre;
            this._artist = Artist;
            this._releaseDate = ReleaseDate;
            this._type = Type;
            this._songsCount = SongsCount;
            this._format = Format;
            this._uploader = Uploader;
            this._updater = Updater;
            this._description = Description;
            this._albumImageURI = AlbumImageURI;
            this._albumPageURI = AlbumPageURI;
        }

        #region Fields and getters
        private readonly String _title;
        /// <summary>
        /// Название альбома
        /// </summary>
        public String Title { get { return this._title; } }

        private readonly String _genre;
        /// <summary>
        /// Название жанра альбома
        /// </summary>
        public String Genre { get { return this._genre; } }

        private readonly String _artist;
        /// <summary>
        /// Название исполнителя альбома
        /// </summary>
        public String Artist { get { return this._artist; } }

        private readonly String _releaseDate;
        /// <summary>
        /// Дата выпуска
        /// </summary>
        public String ReleaseDate { get { return this._releaseDate; } }

        private readonly String _type;
        /// <summary>
        /// Тип альбома
        /// </summary>
        public String Type { get { return this._type; } }

        private readonly Nullable<Byte> _songsCount;
        /// <summary>
        /// Количество песен
        /// </summary>
        public Nullable<Byte> SongsCount { get { return this._songsCount; } }

        private readonly String _format;
        /// <summary>
        /// Формат альбома
        /// </summary>
        public String Format { get { return this._format; } }

        private readonly String _uploader;
        /// <summary>
        /// Изначальный загрузчик альбома
        /// </summary>
        public String Uploader { get { return this._uploader; } }

        private readonly String _updater;
        /// <summary>
        /// Последний, кто обновлял альбом
        /// </summary>
        public String Updater { get { return this._updater; } }

        private readonly String _description;
        /// <summary>
        /// Пользовательское описание
        /// </summary>
        public String Description { get { return this._description; } }

        private readonly Uri _albumImageURI;
        /// <summary>
        /// URI на изображение (обложку) текущего альбома
        /// </summary>
        public Uri AlbumImageURI { get { return this._albumImageURI; } }
        /// <summary>
        /// URI на изображение (обложку) текущего альбома
        /// </summary>
        public Uri CoverImageURI { get { return this._albumImageURI; } }

        private readonly Uri _albumPageURI;
        /// <summary>
        /// URI на HTML-страницу текущего альбома
        /// </summary>
        public Uri AlbumPageURI { get { return this._albumPageURI; } }
        /// <summary>
        /// URI на HTML-страницу текущего альбома
        /// </summary>
        public Uri PageURI { get { return this._albumPageURI; } }
        #endregion;

        #region Overridings
        /// <summary>
        /// Определяет равенство текущего экземпляра с указанным
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(AlbumHeader other)
        {
            if (ReferenceEquals(null, other)) {return false;}
            if (ReferenceEquals(this, other)) {return true;}
            Boolean result = 
                this._albumPageURI == other._albumPageURI &&
                this._albumImageURI.Authority.Equals(other._albumImageURI.Authority, StringComparison.OrdinalIgnoreCase) &&
                this._songsCount == other._songsCount &&
                string.Equals(this._description, other._description, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(this._updater, other._updater, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(this._uploader, other._uploader, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(this._format, other._format, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(this._type, other._type, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(this._releaseDate, other._releaseDate, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(this._artist, other._artist, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(this._genre, other._genre, StringComparison.OrdinalIgnoreCase) &&
                string.Equals(this._title, other._title, StringComparison.OrdinalIgnoreCase);
            return result;
        }

        public override Boolean Equals(Object obj)
        {
            if (ReferenceEquals(null, obj)) {return false;}
            if (ReferenceEquals(this, obj)) {return true;}
            return obj is AlbumHeader && this.Equals((AlbumHeader)obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hashCode = _albumPageURI.GetHashCode();
                hashCode = (hashCode * 397) ^ _albumImageURI.GetHashCode();
                hashCode = (hashCode * 397) ^ _description.GetHashCode();
                hashCode = (hashCode * 397) ^ _updater.GetHashCode();
                hashCode = (hashCode * 397) ^ _uploader.GetHashCode();
                hashCode = (hashCode * 397) ^ _format.GetHashCode();
                hashCode = (hashCode * 397) ^ _songsCount.GetHashCode();
                hashCode = (hashCode * 397) ^ _type.GetHashCode();
                hashCode = (hashCode * 397) ^ _releaseDate.GetHashCode();
                hashCode = (hashCode * 397) ^ _artist.GetHashCode();
                hashCode = (hashCode * 397) ^ _genre.GetHashCode();
                hashCode = (hashCode * 397) ^ _title.GetHashCode();
                return hashCode;
            }
        }
        #endregion
    }
}
