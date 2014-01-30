using System;
using System.Globalization;
using System.Linq;
using KlotosLib;

namespace MyzukaRuGrabberCore.DataModels
{
    /// <summary>
    /// Модель данных одной распарсенной песни. Неизменяемый класс.
    /// </summary>
    [Serializable()]
    public sealed class OneSongHeader : ICommonHeader, IEquatable<OneSongHeader>
    {
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

        private readonly Byte _number;
        /// <summary>
        /// Номер песни
        /// </summary>
        public Byte Number { get { return this._number; } }

        private readonly String _title;
        /// <summary>
        /// Название песни
        /// </summary>
        public String Title { get { return this._title; } }

        private readonly String _name;
        /// <summary>
        /// Название песни без исполнителя
        /// </summary>
        public String Name { get { return this._name; } }

        private readonly String _artist;
        /// <summary>
        /// Имя исполнителя песни
        /// </summary>
        public String Artist { get { return this._artist; } }

        private readonly String _album;
        /// <summary>
        /// Альбом, к которому относится песня
        /// </summary>
        public String Album { get { return this._album; } }

        private readonly String _genre;
        /// <summary>
        /// Название жанра песни
        /// </summary>
        public String Genre { get { return this._genre; } }

        private readonly String _duration;
        /// <summary>
        /// Длительность песни
        /// </summary>
        public String Duration { get { return this._duration; } }

        private readonly String _size;
        /// <summary>
        /// Байтовый размер песни
        /// </summary>
        public String Size { get { return this._size; } }

        private readonly String _bitrate;
        /// <summary>
        /// Битрейт песни
        /// </summary>
        public String Bitrate { get { return this._bitrate; } }

        private readonly String _format;
        /// <summary>
        /// Формат песни
        /// </summary>
        public String Format { get { return this._format; } }

        private readonly String _uploader;
        /// <summary>
        /// Загрузчик песни
        /// </summary>
        public String Uploader { get { return this._uploader; } }

        private readonly Uri _songImageURI;
        /// <summary>
        /// URI на изображение (обложку) текущей песни
        /// </summary>
        public Uri SongImageURI { get { return this._songImageURI; } }
        /// <summary>
        /// URI на изображение (обложку) текущей песни
        /// </summary>
        public Uri CoverImageURI { get { return this._songImageURI; } }

        private readonly Uri _songPageURI;
        /// <summary>
        /// URI на страницу, представляющую (содержащую) данную песню
        /// </summary>
        public Uri SongPageURI { get { return this._songPageURI; } }
        /// <summary>
        /// URI на страницу, представляющую (содержащую) данную песню
        /// </summary>
        public Uri PageURI { get { return this._songPageURI; } }

        private readonly Boolean _isAvailableForDownload;
        /// <summary>
        /// Определяет, доступен ли файл песни для скачивания, основываясь на текстовом сообщении на странице
        /// </summary>
        public Boolean IsAvailableForDownload {get { return this._isAvailableForDownload; }}

        /// <summary>
        /// Генерирует имя аудио-файла, содержащего песню, на основании метаданных песни
        /// </summary>
        /// <param name="OrigFilename">Оригинальное название песни, полученное из скачанного файла; 
        /// из него извлекается расширение имени файла.</param>
        /// <returns></returns>
        public String GenerateSongFilename(String OrigFilename)
        {
            String output;
            String filename_without_ext = this.Number.ToString(CultureInfo.InvariantCulture) + ". " +
                this.Artist + " - " + this.Name + " [" + this.Album + "]";
            String ext = OrigFilename.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
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
        /// Генерирует имя аудио-файла, содержащего песню, на основании метаданных песни и по указанному шаблону
        /// </summary>
        /// <param name="OrigFilename"></param>
        /// <param name="FilenameTemplate"></param>
        /// <returns></returns>
        public String GenerateSongFilename(String OrigFilename, String FilenameTemplate)
        {
            String ext = OrigFilename.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries).LastOrDefault();
            String output = FilenameTemplate
                .Replace("%number%", this.Number.ToString(CultureInfo.InvariantCulture))
                .Replace("%title%", this.Name)
                .Replace("%artist%", this.Artist)
                .Replace("%album%", this.Album)
                + ((ext.HasAlphaNumericChars() == true) ? "." + ext : ".mp3");
            return output;
        }

        #region Equatable and hashcode
        /// <summary>
        /// Определяет равенство текущего экземпляра хидера песни с указанным
        /// </summary>
        /// <param name="Other"></param>
        /// <returns></returns>
        public Boolean Equals(OneSongHeader Other)
        {
            if(Other.IsNull()==true) { return false; }
            if(Object.ReferenceEquals(this, Other)==true) {return true; }
            Boolean result = this.Album == Other.Album && this.Artist == Other.Artist && this.Bitrate == Other.Bitrate &&
                             this.Duration == Other.Duration &&
                             this.Format == Other.Format &&
                             this.Genre == Other.Genre && this.Name == Other.Name && this.Number == Other.Number &&
                             this.Size == Other.Size &&
                             this.SongImageURI.Authority.Equals(Other.SongImageURI.Authority, StringComparison.OrdinalIgnoreCase) &&
                             this.SongPageURI == Other.SongPageURI;
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
