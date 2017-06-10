using System;
using System.Globalization;
using System.Linq;
using KlotosLib;
using KlotosLib.StringTools;

namespace MyzukaRuGrabberCore.DataModels
{
    /// <summary>
    /// Модель данных одной распарсенной песни. Неизменяемый класс.
    /// </summary>
    [Serializable()]
    public sealed class OneSongHeader : ICommonHeader, IEquatable<OneSongHeader>
    {
        /// <summary>
        /// Конструктор, создающий пустой экземпляр
        /// </summary>
        public OneSongHeader()
        {
            
        }

        /// <summary>
        /// Конструктор, заполняющий экземпляр всеми необходимыми данными
        /// </summary>
        /// <param name="Number"></param>
        /// <param name="Title"></param>
        /// <param name="Name"></param>
        /// <param name="Artist"></param>
        /// <param name="Album"></param>
        /// <param name="Genre"></param>
        /// <param name="Duration"></param>
        /// <param name="Size"></param>
        /// <param name="Bitrate"></param>
        /// <param name="Format"></param>
        /// <param name="Uploader"></param>
        /// <param name="SongImageURI"></param>
        /// <param name="SongPageUri">URI на страницу, представляющую (содержащую) данную песню</param>
        /// <param name="IsAvailableForDownload">Определяет, доступен ли файл песни для скачивания, основываясь на текстовом сообщении на странице</param>
        public OneSongHeader
            (Byte Number, String Title, String Name, String Artist, String Album, String Genre,
            String Duration, String Size, String Bitrate, String Format, String Uploader, Uri SongImageURI, Uri SongPageUri, 
            Boolean IsAvailableForDownload)
        {
            this._number = Number;
            this._title = Title;
            this._name = Name;
            this._artist = Artist;
            this._album = Album;
            this._genre = Genre;

            this._duration = Duration;
            this._size = Size;
            this._bitrate = Bitrate;
            this._format = Format;
            this._uploader = Uploader;
            this._songImageURI = SongImageURI;
            this._songPageURI = SongPageUri;
            this._isAvailableForDownload = IsAvailableForDownload;
        }

        private Byte _number;

        /// <summary>
        /// Номер песни
        /// </summary>
        public Byte Number
        {
            get { return this._number; }
            set { this._number = value; }
        }

        private String _title;

        /// <summary>
        /// Название песни
        /// </summary>
        public String Title
        {
            get { return this._title; }
            set { this._title = value; }
        }

        private String _name;

        /// <summary>
        /// Название песни без исполнителя
        /// </summary>
        public String Name
        {
            get { return this._name; }
            set { this._name = value; }
        }

        private String _artist;

        /// <summary>
        /// Имя исполнителя песни
        /// </summary>
        public String Artist
        {
            get { return this._artist; }
            set { this._artist = value; }
        }

        private String _album;

        /// <summary>
        /// Альбом, к которому относится песня
        /// </summary>
        public String Album
        {
            get { return this._album; }
            set { this._album = value; }
        }

        private String _genre;

        /// <summary>
        /// Название жанра песни
        /// </summary>
        public String Genre
        {
            get { return this._genre; }
            set { this._genre = value; }
        }

        private String _duration;

        /// <summary>
        /// Длительность песни
        /// </summary>
        public String Duration
        {
            get { return this._duration; }
            set { this._duration = value; }
        }

        private String _size;

        /// <summary>
        /// Байтовый размер песни
        /// </summary>
        public String Size
        {
            get { return this._size; }
            set { this._size = value; }
        }

        private String _bitrate;

        /// <summary>
        /// Битрейт песни
        /// </summary>
        public String Bitrate
        {
            get { return this._bitrate; }
            set { this._bitrate = value; }
        }

        private String _format;

        /// <summary>
        /// Формат песни
        /// </summary>
        public String Format
        {
            get { return this._format; }
            set { this._format = value; }
        }

        private String _uploader;

        /// <summary>
        /// Загрузчик песни
        /// </summary>
        public String Uploader
        {
            get { return this._uploader; }
            set { this._uploader = value; }
        }

        private Uri _songImageURI;

        /// <summary>
        /// URI на изображение (обложку) текущей песни
        /// </summary>
        public Uri SongImageURI
        {
            get { return this._songImageURI; }
            set { this._songImageURI = value; }
        }
        /// <summary>
        /// URI на изображение (обложку) текущей песни
        /// </summary>
        public Uri CoverImageURI { get { return this._songImageURI; } }

        private Uri _songPageURI;

        /// <summary>
        /// URI на страницу, представляющую (содержащую) данную песню
        /// </summary>
        public Uri SongPageURI
        {
            get { return this._songPageURI; }
            set { this._songPageURI = value; }
        }
        /// <summary>
        /// URI на страницу, представляющую (содержащую) данную песню
        /// </summary>
        public Uri PageURI { get { return this._songPageURI; } }

        private Uri _lyricsPageUri;

        /// <summary>
        /// URI на страницу, представляющую (содержащую) текст данной песни, если он есть, или NULL, если текста песни нет
        /// </summary>
        public Uri LyricsPageUri
        {
            get { return this._lyricsPageUri; }
            set { this._lyricsPageUri = value; }
        }

        private Boolean _isAvailableForDownload;

        /// <summary>
        /// Определяет, доступен ли файл песни для скачивания, основываясь на текстовом сообщении на странице
        /// </summary>
        public Boolean IsAvailableForDownload
        {
            get { return this._isAvailableForDownload; }
            set { this._isAvailableForDownload = value; }
        }

        private UInt32 _rating;

        /// <summary>
        /// Рейтинг песни по версии сайта myzuka.fm
        /// </summary>
        public UInt32 Rating
        {
            get { return this._rating; }
            set { this._rating = value; }
        }

        /// <summary>
        /// Добавляет в экземпляр указанное значение по указаннгому ключу
        /// </summary>
        /// <param name="key"></param>
        /// <param name="value"></param>
        /// <param name="htmlValue"></param>
        public void Accept(String key, String value, String htmlValue)
        {
            if (key.Equals("Жанр:", StringComparison.OrdinalIgnoreCase))
            {
                this._genre = value;
            }
            else if (key.Equals("Исполнитель:", StringComparison.OrdinalIgnoreCase))
            {
                this._artist = value;
            }
            else if (key.Equals("Альбом:", StringComparison.OrdinalIgnoreCase))
            {
                this._album = value;
            }
            else if (key.Equals("Длительность:", StringComparison.OrdinalIgnoreCase))
            {
                this._duration = value;
            }
            else if (key.Equals("Размер:", StringComparison.OrdinalIgnoreCase))
            {
                this._size = value;
            }
            else if (key.Equals("Рейтинг:", StringComparison.OrdinalIgnoreCase))
            {
                this._rating = UInt32.Parse(value, NumberStyles.Integer);
            }
            else if (key.Equals("Загрузил:", StringComparison.OrdinalIgnoreCase))
            {
                this._uploader = value;
            }
            else if (key.Equals("Текст песни:", StringComparison.OrdinalIgnoreCase))
            {
                string relativeUrl = KlotosLib.StringTools.SubstringHelpers.GetInnerStringBetweenTokens(htmlValue, "\"", "\"", 0, 0, false, Direction.FromStartToEnd, StringComparison.Ordinal).Value;
                this._lyricsPageUri = new Uri(relativeUrl, UriKind.Relative);
            }
            else
            {
                throw new NotSupportedException(String.Format("Ключ '{0}' неизвестен, поэтому его значение '{1}' не может быть обработано", key, value));
            }
        }

        /// <summary>
        /// Генерирует имя аудио-файла, содержащего песню, на основании метаданных песни
        /// </summary>
        /// <param name="origFilename">Оригинальное название песни, полученное из скачанного файла; 
        /// из него извлекается расширение имени файла.</param>
        /// <returns></returns>
        public String GenerateSongFilename(String origFilename)
        {
            String output;
            String filename_without_ext = this.Number.ToString(CultureInfo.InvariantCulture) + ". " +
                this.Artist + " - " + this.Name + " [" + this.Album + "]";
            String ext = origFilename.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            if (ext.HasAlphaNumericChars() == false)
            {
                output = filename_without_ext + ".mp3";
            }
            else
            {
                output = filename_without_ext + "." + ext;
            }
            return output;
        }

        /// <summary>
        /// Генерирует имя аудио-файла, содержащего песню, по указанному шаблону на основании метаданных песни
        /// </summary>
        /// <param name="origFilename"></param>
        /// <param name="filenameTemplate"></param>
        /// <returns></returns>
        public String GenerateSongFilename(String origFilename, String filenameTemplate)
        {
            String ext = origFilename.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            String output = filenameTemplate
                .Replace("%number%", this.Number.ToString(CultureInfo.InvariantCulture))
                .Replace("%title%", this.Name)
                .Replace("%artist%", this.Artist)
                .Replace("%album%", this.Album)
                + ((ext.HasAlphaNumericChars() == true) ? "." + ext : ".mp3");
            return output;
        }

        /// <summary>
        /// Генерирует имя аудио-файла, содержащего песню, по указанному шаблону на основании метаданных песни 
        /// и принимая во внимание переопределённые пользователем метаданные
        /// </summary>
        /// <param name="origFilename"></param>
        /// <param name="filenameTemplate"></param>
        /// <param name="overridings"></param>
        /// <returns></returns>
        public String GenerateSongFilename(String origFilename, String filenameTemplate, OverridableSongMetadata overridings)
        {
            String ext = origFilename.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            ext = (ext.HasAlphaNumericChars() == true) ? "." + ext : ".mp3";
            String output;
            if (overridings != null)
            {
                output = filenameTemplate
                .Replace("%number%", this.Number.ToString(CultureInfo.InvariantCulture))
                .Replace("%title%", overridings.SongName)
                .Replace("%artist%", overridings.ArtistName)
                .Replace("%album%", overridings.AlbumName)
                + ext;
            }
            else
            {
                output = filenameTemplate
                .Replace("%number%", this.Number.ToString(CultureInfo.InvariantCulture))
                .Replace("%title%", this.Name)
                .Replace("%artist%", this.Artist)
                .Replace("%album%", this.Album)
                + ext;
            }
            return output;
        }

        #region Equatable and hashcode
        /// <summary>
        /// Определяет равенство текущего экземпляра хидера песни с указанным
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public Boolean Equals(OneSongHeader other)
        {
            if (Object.ReferenceEquals(null, other)) { return false; }
            if (Object.ReferenceEquals(this, other)) { return true; }
            Boolean result = this.Album == other.Album && this.Artist == other.Artist && this.Bitrate == other.Bitrate &&
                             this.Duration == other.Duration &&
                             this.Format == other.Format &&
                             this.Genre == other.Genre && this.Name == other.Name && this.Number == other.Number &&
                             this.Size == other.Size &&
                             this.SongImageURI.Authority.Equals(other.SongImageURI.Authority, StringComparison.OrdinalIgnoreCase) &&
                             this.SongPageURI == other.SongPageURI;
            return result;
        }

        /// <summary>
        /// Определяет равенство текущего экземпляра с указанным неопределённого типа
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public override Boolean Equals(Object Other)
        {
            if(Other.IsNull()==true) { return false; }
            OneSongHeader o = Other as OneSongHeader;
            return this.Equals(o);
        }

        /// <summary>
        /// Возвращает хэш-код для данного экземпляра, сгенерированный на основании всей его полей
        /// </summary>
        /// <returns></returns>
        public override Int32 GetHashCode()
        {
            unchecked
            {
                int hashCode = _number.GetHashCode();
                hashCode = (hashCode * 397) ^ (_title != null ? _title.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_name != null ? _name.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_artist != null ? _artist.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_album != null ? _album.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_genre != null ? _genre.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_duration != null ? _duration.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_size != null ? _size.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_bitrate != null ? _bitrate.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_format != null ? _format.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_uploader != null ? _uploader.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_songImageURI != null ? _songImageURI.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (_songPageURI != null ? _songPageURI.GetHashCode() : 0);
                return hashCode;
            }
        }
        #endregion
    }
}
