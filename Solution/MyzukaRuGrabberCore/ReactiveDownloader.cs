using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using KlotosLib;
using KlotosLib.StringTools;
using MyzukaRuGrabberCore.DataModels;

namespace MyzukaRuGrabberCore
{
    /// <summary>
    /// Утилитарный класс, осуществляющий скачивание и сохранение на диск песен и взвращающий результаты реактивным способом
    /// </summary>
    public class ReactiveDownloader
    {
        #region Fields
        private readonly OneSongHeader[] _songs;

        private readonly String _userAgent;

        private readonly String _folderPath;

        private readonly Boolean _generateNewFilenames;

        private readonly String _filenameTemplate;
        #endregion

        private ReactiveDownloader
            (IList<OneSongHeader> Songs, String UserAgent, String FolderPath, Boolean GenerateNewFilenames, String FilenameTemplate)
        {
            this._songs = new OneSongHeader[Songs.Count];
            Songs.CopyTo(this._songs, 0);
            this._userAgent = UserAgent;
            this._folderPath = FolderPath;
            this._generateNewFilenames = GenerateNewFilenames;
            this._filenameTemplate = FilenameTemplate;
        }

        /// <summary>
        /// Создаёт и возвращает новое задание по скачиванию указанных песен и последующему сохранению их на диск по указанному пути
        /// </summary>
        /// <param name="Songs">Список всех хидеров песен, файлы песен для которых следует скачать и вернуть. 
        /// Не может быть NULL или пустым.</param>
        /// <param name="UserAgent">User-Agent, который будет использоваться при выполнении запросов</param>
        /// <param name="FolderPath">Существующий путь на диске, по которому будут сохранены песни. 
        /// Если NULL, некорректный или не существует, будет выброшено исключение.</param>
        /// <param name="GenerateNewFilenames">Определяет, следует ли генерировать новое имя файла на основании тэгов песни (true), 
        /// или же использовать то имя файла, которое "пришло" с сервера (false). Если будет указана генерация нового, однако 
        /// получившееся имя будет некорректным, метод попытается его исправить. 
        /// Если же исправить не получится, будет использовано имя с сервера.</param>
        /// <param name="FilenameTemplate">Шаблон имени файла песни</param>
        /// <returns></returns>
        public static ReactiveDownloader CreateTask
            (IList<OneSongHeader> Songs, String UserAgent, String FolderPath, Boolean GenerateNewFilenames, String FilenameTemplate)
        {
            if (UserAgent.HasAlphaNumericChars() == false) { throw new ArgumentException("User-Agent не может быть пустым", "UserAgent"); }
            if (FilePathTools.IsValidFilePath(FolderPath) == false)
            {
                throw new ArgumentException
                    ("Путь для сохранения файлов песен = '" + FolderPath.ToStringS("NULL") + "' некорректен", "FolderPath");
            }
            if(Songs == null) {throw new ArgumentNullException("Songs");}
            if(Songs.Any()==false) {throw new ArgumentException("Список песен для обработки не может быть пустым", "Songs");}
            if(GenerateNewFilenames == true && FilenameTemplate.HasAlphaNumericChars()==false)
            {throw new ArgumentException("Шаблон имени файла не может быть пуст, если требуется клиентская генерация имён файлов");}
            return new ReactiveDownloader(Songs, UserAgent, FolderPath, GenerateNewFilenames, FilenameTemplate);
        }

        /// <summary>
        /// Запускает на выполнение задачу по скачиванию песен и возвращает результат
        /// </summary>
        /// <param name="CancToken">Токен отмены операции</param>
        /// <param name="MaxDegreeOfParallelism">Максимальное количество потоков, которое будет использоваться для запросов к серверу. 
        /// Если меньше 1, ограничение на количество потоков будет снято.</param>
        /// <returns></returns>
        public IDictionary<OneSongHeader, Exception> Start(CancellationToken CancToken, Int32 MaxDegreeOfParallelism)
        {
            if (MaxDegreeOfParallelism < 1) { MaxDegreeOfParallelism = -1; }

            ConcurrentDictionary<OneSongHeader, Exception> intermediate =
                new ConcurrentDictionary<OneSongHeader, Exception>(MaxDegreeOfParallelism, this._songs.Length);
            ParallelOptions opt = new ParallelOptions() { CancellationToken = CancToken, MaxDegreeOfParallelism = MaxDegreeOfParallelism };
            
            try
            {
                ParallelLoopResult p_res = Parallel.ForEach(this._songs, opt,
                (OneSongHeader song, ParallelLoopState pls, Int64 i) =>
                {
                    if (pls.ShouldExitCurrentIteration)
                    {

                        pls.Stop();
                    }
                    KeyValuePair<OneSongHeader, Exception> res =
                        Core.DownloadAndSaveOneSong
                        (song, this._userAgent, this._generateNewFilenames, this._filenameTemplate, this._folderPath, (Int32)i + 1);
                    this.OnNext.Invoke(res.Key, res.Value);
                    intermediate.TryAdd(res.Key, res.Value);
                }
                );
            }
            catch (OperationCanceledException)
            {
                this.OnCancellation(intermediate.Count, this._songs.Length);
                CancToken.ThrowIfCancellationRequested();
            }
            this.OnComplete.Invoke(intermediate);
            return intermediate;
        }

        /// <summary>
        /// Запускает на выполнение асинхронную задачу по скачиванию песен и возвращает результат в обёртке типа Task
        /// </summary>
        /// <param name="CancToken">Токен отмены операции</param>
        /// <param name="MaxDegreeOfParallelism">Максимальное количество потоков, которое будет использоваться для запросов к серверу. 
        /// Если меньше 1, ограничение на количество потоков будет снято.</param>
        /// <returns></returns>
        public async Task<IDictionary<OneSongHeader, Exception>> StartAsync(CancellationToken CancToken, Int32 MaxDegreeOfParallelism)
        {
            return await Task.Factory.StartNew<IDictionary<OneSongHeader, Exception>>(
                () => this.Start(CancToken, MaxDegreeOfParallelism), CancToken, TaskCreationOptions.LongRunning, TaskScheduler.Default);
        }

        /// <summary>
        /// Возникает при завершении обработки (скачивания и сохранения) одной песни вне зависимости от того, 
        /// успешна ли была обработка или нет. Первый аргумент возвращает метаданные песни, а второй - исключение, 
        /// если при обработке возникла ошибка, или NULL, если всё успешно.
        /// </summary>
        public event Action<OneSongHeader, Exception> OnNext;

        /// <summary>
        /// Возникает при завершении обработки (скачивания и сохранения) всех песен, но не в случае, 
        /// если выполнение было прервано токеном отмены.
        /// </summary>
        public event Action<IDictionary<OneSongHeader, Exception>> OnComplete;

        /// <summary>
        /// Возникает при прерывании обработки при помощи токена отмены. Возвращает в первом параметре количество песен, 
        /// которые уже успели обработаться, а во втором - количество всех запрошенных ко скачиванию песен.
        /// </summary>
        public event Action<Int32, Int32> OnCancellation;
    }
}
