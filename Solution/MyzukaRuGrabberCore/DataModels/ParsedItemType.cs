using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyzukaRuGrabberCore.DataModels
{
    /// <summary>
    /// Представляет результат идентификации типа HTML-страницы
    /// </summary>
    [Serializable()]
    public enum ParsedItemType : byte
    {
        /// <summary>
        /// Тип страницы неизвестен, продолжение парсинга невозможно = 0
        /// </summary>
        Unknown = 0,

        /// <summary>
        /// Страница альбома = 1
        /// </summary>
        Album = 1,

        /// <summary>
        /// Страница отдельной песни = 2
        /// </summary>
        Song = 2
    }
}
