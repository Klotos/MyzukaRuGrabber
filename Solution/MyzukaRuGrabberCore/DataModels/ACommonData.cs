using System;
using System.Drawing;
using System.Linq;
using KlotosLib;

namespace MyzukaRuGrabberCore.DataModels
{
    /// <summary>
    /// Инкапсулирует общие данные для распарсенных альбома и песни
    /// </summary>
    [Serializable()]
    public abstract class ACommonData : IDisposable
    {
        /// <summary>
        /// Общее, составное название альбома либо песни
        /// </summary>
        abstract public String Title { get; }

        /// <summary>
        /// Исполнитель альбома или песни
        /// </summary>
        abstract public String Artist { get; }

        /// <summary>
        /// Название альбома
        /// </summary>
        abstract public String Album { get; }

        /// <summary>
        /// Жанр альбома или песни
        /// </summary>
        abstract public String Genre { get; }

        /// <summary>
        /// Ссылка на страницу данного альбома или песни
        /// </summary>
        abstract public Uri ItemLink { get; }

        /// <summary>
        /// Пользователь, загрузивший на сайт данный альбом или песню
        /// </summary>
        abstract public String Uploader { get; }

        /// <summary>
        /// Ссылка на изображение обложки альбома или песни
        /// </summary>
        abstract public Uri CoverURI { get; }

        /// <summary>
        /// Скачанное изображение обложки альбома или песни, или же NULL, если скачивание не требуется или не удалось
        /// </summary>
        abstract public DownloadedFile CoverFile { get; }

        /// <summary>
        /// Преобразованное в точечный рисунок изображение обложки альбома или песни, 
        /// или же NULL, если скачивание не требуется или не удалось, или же не удалось преобразовать
        /// </summary>
        abstract public Bitmap CoverImage { get; }

        /// <summary>
        /// Генерирует имя файла, содержащего обложку изображения, на основании метаданных
        /// </summary>
        /// <returns></returns>
        public String GenerateExternalCoverFilename()
        {
            String filename_without_ext = this.Title + " cover";
            String ext = this.CoverFile.Filename.Split(new char[1] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();
            String output_filename;
            if (ext.HasAlphaNumericChars() == false)
            {
                output_filename = filename_without_ext + ".jpg";
            }
            else
            {
                output_filename = filename_without_ext + "." + ext;
            }
            return output_filename;
        }

        /// <summary>
        /// Освобождает ресурсы, занятые обложкой
        /// </summary>
        public void Dispose()
        {
            if (this.CoverImage != null)
            {
                this.CoverImage.Dispose();
            }
            if (this.CoverFile != null)
            {
                this.CoverFile.Dispose();
            }
        }
    }
}
