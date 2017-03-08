using System;

namespace MyzukaRuGrabberCore.DataModels
{
    /// <summary>
    /// Представляет те метаданные по одной песне, которые пользователь может переопределить, 
    /// т.е. заменить ими оригинальные данные при помощи, к примеру, графического интерфейса
    /// </summary>
    public sealed class OverridableSongMetadata
    {
        /// <summary>
        /// Название исполнителя
        /// </summary>
        public String ArtistName { get; set; } 

        /// <summary>
        /// Название песни
        /// </summary>
        public String SongName { get; set; }

        /// <summary>
        /// Название альбома
        /// </summary>
        public String AlbumName { get; set; }
    }
}