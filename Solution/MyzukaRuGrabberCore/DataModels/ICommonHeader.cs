using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyzukaRuGrabberCore.DataModels
{
    /// <summary>
    /// Интерфейс, описывающий некоторые общие члены для хидеров альбома и песни
    /// </summary>
    public interface ICommonHeader
    {
        /// <summary>
        /// Название альбома или песни
        /// </summary>
        String Title { get; }

        /// <summary>
        /// Имя исполнителя альбома или песни
        /// </summary>
        String Artist { get; }

        /// <summary>
        /// Название жанра альбома или песни
        /// </summary>
        String Genre { get; }

        /// <summary>
        /// Имя пользователя, загрузившего на сайт данный альбом или песню
        /// </summary>
        String Uploader { get; }

        /// <summary>
        /// URI страницы, на которой расположен данный альбом или песня
        /// </summary>
        Uri PageURI { get; }

        /// <summary>
        /// URI изображения, представляющего обложку данного альбома или песни
        /// </summary>
        Uri CoverImageURI { get; }
    }
}
