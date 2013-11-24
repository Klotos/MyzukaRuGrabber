using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using HtmlAgilityPack;
using MyzukaRuGrabberCore.DataModels;
using KlotosLib;

namespace MyzukaRuGrabberCore
{
    /// <summary>
    /// Инкапсулирует все операции по парсингу страниц сайта myzuka.ru
    /// </summary>
    public static class Core
    {
        /// <summary>
        /// Парсит и преобразовывает указанную строку, представляющую URI, 
        /// и возвращает результат операции через выводной параметр
        /// </summary>
        /// <param name="Input"></param>
        /// <param name="ErrorMessage"></param>
        /// <returns></returns>
        public static Uri TryParseURI(String Input, out String ErrorMessage)
        {
            ErrorMessage = null;
            if (Input.HasAlphaNumericChars() == false)
            {
                ErrorMessage = "Input string has no any alphanumeric character";
                return null;
            }
            Input = Input.CleanString().Trim();
            if (Input.StartsWith("http://", StringComparison.OrdinalIgnoreCase) == false)
            {
                Input = "http://" + Input;
            }
            Uri input_URI;
            Boolean result = Uri.TryCreate(Input, UriKind.Absolute, out input_URI);
            if (result == false)
            {
                ErrorMessage = "Input string is invalid URI";
                return null;
            }
            if (input_URI.Scheme.Equals("http", StringComparison.OrdinalIgnoreCase)==false)
            {
                ErrorMessage = "Only HTTP scheme is valid, but not " + input_URI.Scheme;
                return null;
            }
            if (input_URI.Authority.Equals("myzuka.ru", StringComparison.OrdinalIgnoreCase)==false)
            {
                ErrorMessage = "Only myzuka.ru domain is supported, but not " + input_URI.Authority;
                return null;
            }
            if (input_URI.Segments.Length < 4)
            {
                ErrorMessage = "URI is invalid";
                return null;
            }
            String segment1 = input_URI.Segments[1];
            if (segment1.Trim('/').IsIn(StringComparison.OrdinalIgnoreCase, "album", "song") == false)
            {
                ErrorMessage = "First segment of URI - '"+segment1+"', is invalid";
                return null;
            }
            String segment2 = input_URI.Segments[2];
            if (segment2.Trim('/').TryParseNumber<UInt32>(NumberStyles.None, null)==null)
            {
                ErrorMessage = "Second segment of URI - '" + segment2 + "', is invalid";
                return null;
            }
            return input_URI;
        }

        /// <summary>
        /// Пытается скачать и распарсить страницу альбома или песни по указаннному URI, после чего возвратить результат
        /// </summary>
        /// <param name="PageURI">URI страницы, по которой должен выполняться запрос</param>
        /// <param name="UserAgent">User-Agent, с которым будет исполнен запрос</param>
        /// <param name="DownloadCover">Определяет, необходимо ли загружать обложку альбома или песни</param>
        /// <param name="InvokeEvents">Определяет, необходимо ли вызывать события в процессе выполнения</param>
        /// <param name="CancToken"></param>
        /// <returns>Модель, соответсующая альбому или песни, или же NULL в случае провала парсинга</returns>
        public static ACommonData TryGrabAndParsePage
            (Uri PageURI, String UserAgent, Boolean DownloadCover, Boolean InvokeEvents, CancellationToken CancToken)
        {
            Task<HtmlAgilityPack.HtmlDocument> temp_task = Task.Run<HtmlAgilityPack.HtmlDocument>
                (() => CoreInternal.TryGrabPageWithCancellation(PageURI, UserAgent, CancToken), CancToken);
            try
            {
                temp_task.Wait(CancToken);
            }
            catch (AggregateException aex)
            {
                if (InvokeEvents == true)
                {
                    Core.OnException.Invoke(aex.InnerException);
                }
                return null;
            }
            catch (OperationCanceledException ocex)
            {
                var temp = ocex.TotalMessage();
                CancToken.ThrowIfCancellationRequested();
            }
            HtmlAgilityPack.HtmlDocument HTML_doc = temp_task.Result;
            if (InvokeEvents == true)
            {
                Core.PageWasDownloaded.Invoke(HTML_doc);
            }

            ParsedItemType album_or_song = CoreInternal.DetectItemType(HTML_doc);
            if (album_or_song == ParsedItemType.Unknown)
            {
                if (InvokeEvents == true)
                {
                    Core.OnException.Invoke(new InvalidOperationException(String.Format(
                        "Страница, полученная по URI '{0}' с кодировкой '{1}', не может быть распознана как страница альбома или песни",
                        PageURI.ToString(), HTML_doc.Encoding.ToString())));
                }
                return null;
            }
            if (InvokeEvents == true)
            {
                Core.ItemWasDetected.Invoke(album_or_song);
            }
            CancToken.ThrowIfCancellationRequested();

            ICommonHeader header;
            if (album_or_song == ParsedItemType.Album)
            {
                try
                {
                    header = CoreInternal.ParseAlbumHeader(HTML_doc);
                }
                catch (Exception ex)
                {
                    if (InvokeEvents == true)
                    {
                        Core.OnException.Invoke(new InvalidOperationException(
                            String.Format("Произошла ошибка при парсинге страницы альбома, полученной по URI '{0}'", PageURI), 
                            ex));
                    }
                    return null;
                }
            }
            else
            {
                try
                {
                    header = CoreInternal.ParseOneSongHeader(HTML_doc);
                }
                catch (Exception ex)
                {
                    if (InvokeEvents == true)
                    {
                        Core.OnException.Invoke(new InvalidOperationException(
                            String.Format("Произошла ошибка при парсинге страницы песни, полученной по URI '{0}'", PageURI),
                            ex));
                    }
                    return null;
                }
            }
            if (InvokeEvents == true)
            {
                Core.HeaderWasParsed.Invoke(header);
            }
            CancToken.ThrowIfCancellationRequested();

            DownloadedFile cover_file = null;
            Bitmap cover_image = null;
            if (DownloadCover == true)
            {
                String err_mess;
                cover_file = CoreInternal.TryDownloadFile(header.CoverImageURI, header.PageURI, UserAgent, out err_mess);
                if (cover_file != null)
                {
                    cover_image = CoreInternal.TryConvertFileToImage(cover_file);
                    if (InvokeEvents == true)
                    {
                        Core.CoverWasAcquired.Invoke(cover_file, cover_image);
                    }
                }
            }
            CancToken.ThrowIfCancellationRequested();

            ACommonData output;
            if (album_or_song == ParsedItemType.Album)
            {
                AlbumHeader ah = (AlbumHeader) header;
                List<OneSongHeader> songs;
                try
                {
                    songs = CoreInternal.ParseAllSongsInAlbum(HTML_doc, ah);
                }
                catch (Exception ex)
                {
                    if (InvokeEvents == true)
                    {
                        Core.OnException.Invoke(new InvalidOperationException(
                            String.Format("Произошла ошибка при парсинге списка песен для альбома, полученного по URI '{0}'", PageURI),
                            ex));
                    }
                    return null;
                }
                output = new ParsedAlbum(ah, songs, cover_file, cover_image);
            }
            else
            {
                OneSongHeader sh = (OneSongHeader) header;
                Uri song_URI;
                try
                {
                    song_URI = CoreInternal.ExtractDownloadSongURI(HTML_doc);
                }
                catch (Exception ex)
                {
                    if (InvokeEvents == true)
                    {
                        Core.OnException.Invoke(new InvalidOperationException(String.Format(
                            "Произошла ошибка при парсинге ссылки на скачку файла песни, страница которой получена по URI '{0}'", 
                            PageURI), ex));
                    }
                    return null;
                }
                Uri album_URI;
                try
                {
                    album_URI = CoreInternal.ExtractFromSongAlbumURI(HTML_doc);
                }
                catch (Exception ex)
                {
                    if (InvokeEvents == true)
                    {
                        Core.OnException.Invoke(new InvalidOperationException(String.Format(
                            "Произошла ошибка при парсинге ссылки на страницу альбома песни, страница которой получена по URI '{0}'",
                            PageURI), ex));
                    }
                    return null;
                }
                output = new ParsedSong(sh, song_URI, album_URI, cover_file, cover_image);
            }
            if (InvokeEvents == true)
            {
                Core.WorkIsDone.Invoke(output);
            }
            return output;
        }

        /// <summary>
        /// Пытается асинхронно, не блокируя основной поток выполнения, скачать и распарсить страницу альбома или песни по указаннному URI, после чего возвратить результат, инкапсулированный в Task. Поддерживает токен отмены.
        /// </summary>
        /// <param name="PageURI">URI страницы, по которой должен выполняться запрос</param>
        /// <param name="UserAgent">User-Agent, с которым будет исполнен запрос</param>
        /// <param name="DownloadCover">Определяет, необходимо ли загружать обложку альбома или песни</param>
        /// <param name="InvokeEvents">Определяет, необходимо ли вызывать события в процессе выполнения</param>
        /// <param name="CancToken">Токен отмены</param>
        /// <returns>Модель, соответсующая альбому или песни, или же NULL в случае провала парсинга</returns>
        public static async Task<ACommonData> TryGrabAndParsePageAsync
            (Uri PageURI, String UserAgent, Boolean DownloadCover, Boolean InvokeEvents, CancellationToken CancToken)
        {
            Task<ACommonData> t = Task.Factory.StartNew<ACommonData>(
                        () => Core.TryGrabAndParsePage(PageURI, UserAgent, DownloadCover, InvokeEvents, CancToken),
                        CancToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
            return await t;
        }

        #region Events
        /// <summary>
        /// Возникает при завершении успешного скачивания страницы и преобразования её в валидный HTML-документ; 
        /// возвращает этот HTML-документ.
        /// </summary>
        public static event Action<HtmlDocument> PageWasDownloaded;

        /// <summary>
        /// Возникает при определении принадлежности страницы; возвращает идентифицированный тип страницы
        /// </summary>
        public static event Action<ParsedItemType> ItemWasDetected;

        /// <summary>
        /// Возникает при успешном парсинге хидера альбома или песни; возвращает этот хидер
        /// </summary>
        public static event Action<ICommonHeader> HeaderWasParsed;

        /// <summary>
        /// Возникает при успешном нахождении, скачивании и преобразовании в битовую карту обложки альбома или песни; 
        /// возвращает обложку в виде скачанного файла и битовой карты
        /// </summary>
        public static event Action<DownloadedFile, Bitmap> CoverWasAcquired;

        /// <summary>
        /// Возникает при успешном завершении всех работ, непосредственно перед возвращением результата из метода. 
        /// В единественном параметре возвращается собственно результат.
        /// </summary>
        public static event Action<ACommonData> WorkIsDone;

        /// <summary>
        /// Возникает при выбрасывании исключения во время работы метода; возвращает появившееся исключение.
        /// </summary>
        public static event Action<Exception> OnException;
        #endregion

        /// <summary>
        /// Скачивает все песни, указанные в наборе, выполняя параллельные запросы к серверу
        /// </summary>
        /// <param name="Songs">Список всех хидеров песен, файлы песен для которых следует скачать и вернуть</param>
        /// <param name="UserAgent">UserAgent, который будет использоваться при выполнении запросов</param>
        /// <param name="FillFailedWithNull">Определяет, необходимо ли при возникновении ошибки скачивания песни 
        /// добавлять в выходной словарь хидер песни с NULL-значением (true), или же не вообще добавлять в словарь (false).</param>
        /// <param name="ErrorsList">Выводной параметр, содержащий список всех ошибок, 
        /// которые возникли в процессе выполнения задачи, или NULL, если не тпроизошло ни одной ошибки.</param>
        /// <returns>Словарь, где ключ - это полученный хидер песни, а значение - успешно скачанный файл этой песни</returns>
        public static ConcurrentDictionary<OneSongHeader, DownloadedFile> DownloadAllSongs
            (IList<OneSongHeader> Songs, String UserAgent, Boolean FillFailedWithNull, out List<String> ErrorsList)
        {
            ErrorsList = new List<string>();
            Songs.ThrowIfNullOrEmpty();
            if (UserAgent == null) { throw new ArgumentNullException("UserAgent"); }

            ConcurrentBag<String> errors = new ConcurrentBag<string>();
            ConcurrentDictionary<OneSongHeader, DownloadedFile> output_files =
                new ConcurrentDictionary<OneSongHeader, DownloadedFile>(Songs.Count, Songs.Count);

            ParallelLoopResult p_res = Parallel.ForEach
            (Songs, new ParallelOptions(){MaxDegreeOfParallelism = 4},
                (OneSongHeader song) =>
                {
                    // 1. запрос по УРЛ и перевести результат в ХТМЛдокумент
                    String err_mess;
                    HtmlAgilityPack.HtmlDocument HTML_doc = 
                        CoreInternal.TryGrabPage(song.SongPageURI, UserAgent, out err_mess);
                    if (HTML_doc == null)
                    {
                        errors.Add(String.Format("Невозможно скачать HTML-документ по ссылке '{0}': {1}",
                            song.SongPageURI.ToString(), err_mess));
                        if (FillFailedWithNull == true)
                        {
                            output_files.TryAdd(song, null);
                        }
                        return;
                    }

                    // 2. извлечь УРЛ на скачивание самой песни
                    Uri download_link = CoreInternal.ExtractDownloadSongURI(HTML_doc);
                    // 3. скачать саму песню и всунуть её в контейнер DownloadedFile
                    DownloadedFile song_file = CoreInternal.TryDownloadFile
                        (download_link, song.SongPageURI, UserAgent, out err_mess);
                    if (song_file == null)
                    {
                        errors.Add(String.Format(
                            "Невозможно скачать файл песни '{0}.{1}' (альбом {2}) по ссылке '{3}': {4}",
                            song.Number, song.Name, song.Album, download_link.ToString(), err_mess));
                        if (FillFailedWithNull == true)
                        {
                            output_files.TryAdd(song, null);
                        }
                        return;
                    }
                    output_files.TryAdd(song, song_file);
                }
            );
            
            // 5. возвратить все контейнеры
            ErrorsList = errors.IsEmpty == true ? null : errors.ToList();
            return output_files;
        }

        /// <summary>
        /// Скачивает и сохраняет на диск по указанному пути все песни, указанные в наборе, 
        /// выполняя запросы к серверу с указанной степенью паралеллизма
        /// </summary>
        /// <param name="Songs">Список всех хидеров песен, файлы песен для которых следует скачать и вернуть</param>
        /// <param name="UserAgent">UserAgent, который будет использоваться при выполнении запросов</param>
        /// <param name="FolderPath">Существующий путь на диске, по которому будут сохранены песни. 
        /// Если NULL, некорректный или не существует, будет выброшено исключение.</param>
        /// <param name="GenerateNewFilenames">Определяет, следует ли генерировать новое имя файла на основании тэгов песни (true), 
        /// или же использовать то имя файла, которое "пришло" с сервера (false). Если будет указана генерация нового, однако 
        /// получившееся имя будет некорректным, метод попытается его исправить. 
        /// Если же исправить не получится, будет использовано имя с сервера.</param>
        /// <param name="MaxDegreeOfParallelism">Максимальное количество потоков, которое будет использоваться для запросов к серверу. 
        /// Если меньше 1, ограничение на количество потоков будет снято.</param>
        /// <returns>Словарь ключей и значений, где ключ - это поданный на вход хидер песни, а значение - возможное исключение, 
        /// которое возникло в процессе скачивания и сохранения песни, или же NULL, если песня была успешно скачана и сохранена.</returns>
        public static IDictionary<OneSongHeader, Exception> TryDownloadAndSaveAllSongs
            (IList<OneSongHeader> Songs, String UserAgent,
                String FolderPath, Boolean GenerateNewFilenames,
                Int32 MaxDegreeOfParallelism)
        {
            return Core.TryDownloadAndSaveAllSongs
                (Songs, UserAgent, FolderPath, GenerateNewFilenames, CancellationToken.None, MaxDegreeOfParallelism);
        }

        /// <summary>
        /// Скачивает и сохраняет на диск по указанному пути все песни, указанные в наборе, 
        /// выполняя запросы к серверу с указанной степенью паралеллизма и с указанным токеном отмены операции
        /// </summary>
        /// <param name="Songs">Список всех хидеров песен, файлы песен для которых следует скачать и вернуть</param>
        /// <param name="UserAgent">UserAgent, который будет использоваться при выполнении запросов</param>
        /// <param name="FolderPath">Существующий путь на диске, по которому будут сохранены песни. 
        /// Если NULL, некорректный или не существует, будет выброшено исключение.</param>
        /// <param name="GenerateNewFilenames">Определяет, следует ли генерировать новое имя файла на основании тэгов песни (true), 
        /// или же использовать то имя файла, которое "пришло" с сервера (false). Если будет указана генерация нового, однако 
        /// получившееся имя будет некорректным, метод попытается его исправить. 
        /// Если же исправить не получится, будет использовано имя с сервера.</param>
        /// <param name="CancToken">Токен отмены операции</param>
        /// <param name="MaxDegreeOfParallelism">Максимальное количество потоков, которое будет использоваться для запросов к серверу. 
        /// Если меньше 1, ограничение на количество потоков будет снято.</param>
        /// <returns>Словарь ключей и значений, где ключ - это поданный на вход хидер песни, а значение - возможное исключение, 
        /// которое возникло в процессе скачивания и сохранения песни, или же NULL, если песня была успешно скачана и сохранена.</returns>
        public static IDictionary<OneSongHeader, Exception> TryDownloadAndSaveAllSongs
            (IList<OneSongHeader> Songs, String UserAgent,
            String FolderPath, Boolean GenerateNewFilenames,
            CancellationToken CancToken, Int32 MaxDegreeOfParallelism)
        {
            Songs.ThrowIfNullOrEmpty();
            if (UserAgent == null) { throw new ArgumentNullException("UserAgent"); }
            if (FilePathTools.IsValidFilePath(FolderPath) == false) 
            {throw new ArgumentException("Путь = '" + FolderPath.ToStringS("NULL") + "' некорректен", "FolderPath");}
            if (MaxDegreeOfParallelism < 1) {MaxDegreeOfParallelism = -1;}


            ConcurrentDictionary<OneSongHeader, Exception> intermediate = 
                new ConcurrentDictionary<OneSongHeader, Exception>(MaxDegreeOfParallelism, Songs.Count);
            ParallelOptions opt = new ParallelOptions() { CancellationToken = CancToken, MaxDegreeOfParallelism = MaxDegreeOfParallelism };
            ParallelLoopResult p_res = Parallel.ForEach(Songs, opt, 
                (OneSongHeader song, ParallelLoopState pls, Int64 i) =>
                {
                    opt.CancellationToken.ThrowIfCancellationRequested();
                    if(pls.ShouldExitCurrentIteration){pls.Break();}
                    KeyValuePair<OneSongHeader, Exception> res = 
                        Core.DownloadAndSaveOneSong(song, UserAgent, GenerateNewFilenames, FolderPath, (Int32)i + 1);
                    intermediate.TryAdd(res.Key, res.Value);
                }
            );
            
            return intermediate;
        }

        internal static KeyValuePair<OneSongHeader, Exception> DownloadAndSaveOneSong
            (OneSongHeader Song, String UserAgent, Boolean GenerateNewFilenames, String FolderPath, Int32 IterationNumber)
        {
            // 1. запрос по УРЛ и перевести результат в ХТМЛдокумент
            String err_mess;
            HtmlAgilityPack.HtmlDocument HTML_doc =
                CoreInternal.TryGrabPage(Song.SongPageURI, UserAgent, out err_mess);
            if (HTML_doc == null)
            {
                return new KeyValuePair<OneSongHeader, Exception>(Song, new InvalidOperationException(
                        String.Format("Невозможно скачать HTML-документ по ссылке '{0}' на итерации {1}: {2}",
                            Song.SongPageURI.ToString(), IterationNumber, err_mess)));
            }
            // 2. извлечь УРЛ на скачивание самой песни
            Uri download_link;
            try
            {
                download_link = CoreInternal.ExtractDownloadSongURI(HTML_doc);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<OneSongHeader, Exception>(Song,  new InvalidOperationException(
                        "Невозможно извлечь ссылку на скачивание песни из HTML-документа на итерации " + IterationNumber, ex));
            }
            // 3. скачать саму песню и всунуть её в контейнер DownloadedFile
            DownloadedFile song_file = CoreInternal.TryDownloadFile
                (download_link, Song.SongPageURI, UserAgent, out err_mess);
            if (song_file == null)
            {
                return new KeyValuePair<OneSongHeader, Exception>(Song, new InvalidOperationException(String.Format(
                    "Невозможно скачать файл песни '{0}.{1}' (альбом {2}) из страницы '{3}' по ссылке '{4}' на итерации {5}: {6}",
                    Song.Number, Song.Name, Song.Album, Song.SongPageURI.ToString(), download_link.ToString(), IterationNumber, err_mess)));
            }

            //4. определиться с названием сохраняемого файла
            String new_filename = null;
            if (GenerateNewFilenames == true)
            {
                new_filename = Song.GenerateSongFilename(song_file.Filename);
                FilePathTools.TryCleanFilename(new_filename, out new_filename);
            }
            if (new_filename == null)
            {
                new_filename = song_file.Filename;
            }
            new_filename = Path.Combine(FolderPath, new_filename);
            //5. начать сохранение
            try
            {
                using (FileStream fs = new FileStream(new_filename, FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    song_file.FileBody.Position = 0;
                    song_file.FileBody.CopyTo(fs, 1024*8);
                }
                return new KeyValuePair<OneSongHeader, Exception>(Song, null);
            }
            catch (Exception ex)
            {
                return new KeyValuePair<OneSongHeader, Exception>(Song, new InvalidOperationException(String.Format(
                    "Невозможно сохранить файл песни '{0}.{1}' (альбом {2}) на диск с полным именем файла '{3}' на итерации {4}",
                    Song.Number, Song.Name, Song.Album, new_filename, IterationNumber), ex));
            }
            finally
            {
                song_file.Dispose();
                song_file = null;
            }
        }

        /// <summary>
        /// Асинхронно, без задержки вызывающего потока, скачивает и сохраняет на диск по указанному пути все песни, указанные в наборе, 
        /// выполняя запросы к серверу с указанной степенью паралеллизма и с указанным токеном отмены операции
        /// </summary>
        /// <param name="Songs">Список всех хидеров песен, файлы песен для которых следует скачать и вернуть</param>
        /// <param name="UserAgent">UserAgent, который будет использоваться при выполнении запросов</param>
        /// <param name="FolderPath">Существующий путь на диске, по которому будут сохранены песни. 
        /// Если NULL, некорректный или не существует, будет выброшено исключение.</param>
        /// <param name="GenerateNewFilenames">Определяет, следует ли генерировать новое имя файла на основании тэгов песни (true), 
        /// или же использовать то имя файла, которое "пришло" с сервера (false). Если будет указана генерация нового, однако 
        /// получившееся имя будет некорректным, метод попытается его исправить. 
        /// Если же исправить не получится, будет использовано имя с сервера.</param>
        /// <param name="CancToken">Токен отмены операции</param>
        /// <param name="MaxDegreeOfParallelism">Максимальное количество потоков, которое будет использоваться для запросов к серверу. 
        /// Если меньше 1, ограничение на количество потоков будет снято.</param>
        /// <returns></returns>
        public static async Task<IDictionary<OneSongHeader, Exception>> TryDownloadAndSaveAllSongsAsync
            (IList<OneSongHeader> Songs, String UserAgent,
                String FolderPath, Boolean GenerateNewFilenames,
                CancellationToken CancToken, Int32 MaxDegreeOfParallelism)
        {
            Songs.ThrowIfNullOrEmpty();
            if (UserAgent == null)
            {
                throw new ArgumentNullException("UserAgent");
            }
            if (FilePathTools.IsValidFilePath(FolderPath) == false)
            {
                throw new ArgumentException("Путь = '" + FolderPath.ToStringS("NULL") + "' некорректен", "FolderPath");
            }
            if (MaxDegreeOfParallelism < 1)
            {
                MaxDegreeOfParallelism = -1;
            }
            return await Task.Factory.StartNew<IDictionary<OneSongHeader, Exception>>(
                () => Core.TryDownloadAndSaveAllSongs(Songs, UserAgent, FolderPath, GenerateNewFilenames, CancToken, MaxDegreeOfParallelism), 
                CancToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Пытается асинхронно, не блокируя основной поток выполнения, скачать все песни, указанные в наборе, 
        /// и возвращает их в виде задачи, инкапсулирующей словарь скачанных файлов
        /// </summary>
        /// <param name="Songs">Список всех хидеров песен, файлы песен для которых следует скачать и вернуть</param>
        /// <param name="UserAgent">UserAgent, который будет использоваться при выполнении запросов</param>
        /// <returns>Задача, инкапсулирующая конкурентный словарь, 
        /// где ключ - это полученный хидер песни, а значение - успешно скачанный файл этой песни. 
        /// Если значение NULL - при попытке скачивания песни произошла ошибка.</returns>
        public static async Task<ConcurrentDictionary<OneSongHeader, DownloadedFile>> DownloadAllSongsAsync
            (IList<OneSongHeader> Songs, String UserAgent)
        {
            Songs.ThrowIfNullOrEmpty();
            if (UserAgent == null) { throw new ArgumentNullException("UserAgent"); }

            return await Task.Run<ConcurrentDictionary<OneSongHeader, DownloadedFile>>(
                () =>
            {
                List<String> ErrorsList;
                return Core.DownloadAllSongs(Songs, UserAgent, true, out ErrorsList);
            });
        }

        /// <summary>
        /// Пытается асинхронно, не блокируя основной поток выполнения, скачать все песни, указанные в наборе, 
        /// и возвращает их в виде задачи, инкапсулирующей словарь скачанных файлов. 
        /// Принимает токен отмены, позволяющий отменить выполнение операции на любом этапе.
        /// </summary>
        /// <param name="Songs">Список всех хидеров песен, файлы песен для которых следует скачать и вернуть</param>
        /// <param name="UserAgent">UserAgent, который будет использоваться при выполнении запросов</param>
        /// <param name="CancToken"></param>
        /// <returns>Задача, инкапсулирующая конкурентный словарь, 
        /// где ключ - это полученный хидер песни, а значение - успешно скачанный файл этой песни. 
        /// Если значение NULL - при попытке скачивания песни произошла ошибка.</returns>
        public static async Task<ConcurrentDictionary<OneSongHeader, DownloadedFile>> DownloadAllSongsAsync
            (IList<OneSongHeader> Songs, String UserAgent, CancellationToken CancToken)
        {
            Songs.ThrowIfNullOrEmpty();
            if (UserAgent == null) { throw new ArgumentNullException("UserAgent"); }
            
            return await Task.Run<ConcurrentDictionary<OneSongHeader, DownloadedFile>>(
                () =>
                {
                    List<String> ErrorsList;
                    return Core.DownloadAllSongs(Songs, UserAgent, true, out ErrorsList);
                }, CancToken);
        }

        /// <summary>
        /// Пытается асинхронно, не блокируя основной поток выполнения, скачать одну указанную песню, 
        /// информация о которой получена в процессе парсинга страницы песни. В случае ошибки возвращает NULL.
        /// </summary>
        /// <param name="Song"></param>
        /// <param name="UserAgent"></param>
        /// <returns></returns>
        public static async Task<DownloadedFile> DownloadOneSongAsync(ParsedSong Song, String UserAgent)
        {
            if (Song == null) { throw new ArgumentNullException("Song"); }
            if (UserAgent == null) { throw new ArgumentNullException("UserAgent"); }
            return await CoreInternal.TryDownloadFileAsync(Song.DownloadLink, Song.Header.SongPageURI, UserAgent);
        }

        /// <summary>
        /// Сохраняет указанный файл в указанную папку с опционально указанным именем
        /// </summary>
        /// <param name="ReceivedFile"></param>
        /// <param name="FolderPath"></param>
        /// <param name="NewFilename">Необязательное новое имя файла. Если не указано или некорректно, будет использовано то имя файла, которое было получено с сервера</param>
        /// <returns>Сообщение об ошибке, если она произошла, или NULL, если всё успешно</returns>
        public static String TrySaveDownloadedFileToDisk
            (DownloadedFile ReceivedFile, String FolderPath, String NewFilename = null)
        {
            if (ReceivedFile == null) { throw new ArgumentNullException("ReceivedFile", "ReceivedFile не может быть NULL"); }
            if (FolderPath == null) { throw new ArgumentNullException("FolderPath", "FolderPath не может быть NULL"); }

            String filename = FilePathTools.IsValidFilename(NewFilename) == true
                ? NewFilename 
                : ReceivedFile.Filename;
            String full_filename = Path.Combine(FolderPath, filename);

            try
            {
                using (FileStream fs = new FileStream(full_filename,
                            FileMode.Create, FileAccess.Write, FileShare.None))
                {
                    StreamTools.CopyStream(ReceivedFile.FileBody, fs, true, false);
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.TotalMessage();
            }
        }

        /// <summary>
        /// Пытается асинхронно, не блокируя основной поток выполнения, 
        /// сохранить указанный файл в указанную папку с опционально указанным именем файла
        /// </summary>
        /// <param name="ReceivedFile"></param>
        /// <param name="FolderPath"></param>
        /// <param name="NewFilename">Необязательное новое имя файла. Если не указано или некорректно, будет использовано то имя файла, которое было получено с сервера</param>
        /// <returns>Сообщение об ошибке, если она произошла, или NULL, если всё успешно</returns>
        public static async Task<String> TrySaveDownloadedFileToDiskAsync
            (DownloadedFile ReceivedFile, String FolderPath, String NewFilename = null)
        {
            const Int32 buffer_size = 1024*16;
            if (ReceivedFile == null) { throw new ArgumentNullException("ReceivedFile", "ReceivedFile не может быть NULL"); }
            if (FolderPath == null) { throw new ArgumentNullException("FolderPath", "FolderPath не может быть NULL"); }

            String filename = FilePathTools.IsValidFilename(NewFilename) == true
                ? NewFilename
                : ReceivedFile.Filename;
            String full_filename = Path.Combine(FolderPath, filename);

            try
            {
                using (FileStream fs = new FileStream(full_filename,
                            FileMode.Create, FileAccess.Write, FileShare.None, buffer_size, FileOptions.Asynchronous))
                {
                    ReceivedFile.FileBody.Position = 0;
                    await ReceivedFile.FileBody.CopyToAsync(fs, buffer_size);
                    return null;
                }
            }
            catch (Exception ex)
            {
                return ex.TotalMessage();
            }
        }

        /// <summary>
        /// Пытается асинхронно, не блокируя основной поток выполнения, сохранить все указанные файлы на диск в указанной папке
        ///  с серверными или самостоятельно сгенерированными именами файлов
        /// </summary>
        /// <param name="Songs"></param>
        /// <param name="FolderPath"></param>
        /// <param name="GenerateNewFilenames">Определяет, следует ли генерировать новое имя файла на основании тэгов песни (true), 
        /// или же использовать то имя файла, которое "пришло" с сервера (false). Если будет указана генерация нового, однако 
        /// получившееся имя будет некорректным, метод попытается его исправить. 
        /// Если же исправить не получится, будет использовано имя с сервера.</param>
        /// <returns></returns>
        public static async Task<List<String>> TrySaveSongsToDiskAsync
            (IDictionary<OneSongHeader, DownloadedFile> Songs, String FolderPath, Boolean GenerateNewFilenames)
        {
            if(Songs == null) {throw new ArgumentNullException("Songs"); }
            if (FolderPath == null) { throw new ArgumentNullException("FolderPath"); }
            List<String> output = new List<string>(Songs.Count);

            IEnumerable<KeyValuePair<OneSongHeader, DownloadedFile>> part = Songs.Where(kvp => kvp.Value != null);
            foreach (KeyValuePair<OneSongHeader, DownloadedFile> song in part)
            {
                String filename = null;
                if (GenerateNewFilenames == true)
                {
                    filename = song.Key.GenerateSongFilename(song.Value.Filename);
                    FilePathTools.TryCleanFilename(filename, out filename);
                }
                
                String message = await Core.TrySaveDownloadedFileToDiskAsync(song.Value, FolderPath, filename);
                output.Add(message);
            }
            return output.Any()==true ? null : output;
        }
    }
}
