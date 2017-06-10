using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MyzukaRuGrabberCore;
using MyzukaRuGrabberCore.DataModels;
using KlotosLib;
using KlotosLib.StringTools;

namespace MyzukaRuGrabberGUI
{
    /// <summary>
    /// Основная форма программы
    /// </summary>
    public partial class frm_Main : Form
    {
        /// <summary>
        /// Конструктор основной формы без параметров, со стандартным телом
        /// </summary>
        public frm_Main()
        {
            InitializeComponent();
        }

        #region Fields
        private CommonDataBase _parsedItem;

        private System.IO.DirectoryInfo _lastSavePath = null;

        private CancellationTokenSource _cancelGrabbingPage;

        private CancellationTokenSource _cancelSongsDownload;

        private const String empty = "";

        private readonly Stack<Uri> _URI_history = new Stack<Uri>();
        #endregion

        private void frm_Main_Load(object sender, EventArgs e)
        {
            this.FormClosed += new FormClosedEventHandler(OnFormClosed);
            this.ActiveControl = tb_InputURI;
            this.AcceptButton = btn_Grab;
            this.UpdateStatus(1);

            Core.PageWasDownloaded += this.Event1;
            Core.ItemWasDetected += this.Event2;
            Core.HeaderWasParsed += this.Event3;
            Core.CoverWasAcquired += this.Event4;
            Core.WorkIsDone += this.Event5;
            Core.OnException += OnFailed;
            this.gb_FooterButtons.Enabled = false;

            this.Text = this.GetProgramName();
        }

        /// <summary>
        /// 1 - ready; 2 - processing; 3 - completed/success; 4 - failed; 5 - some errors; 6 - stopped/interrupted; 7 - cancelling
        /// </summary>
        /// <param name="StatusCode"></param>
        private void UpdateStatus(Byte StatusCode)
        {
            if (this.InvokeRequired == false)
            {
                switch (StatusCode)
                {
                    case 1:
                        this.pb_ProcStatusImage.Image = Properties.Resources.ImageReady;
                        this.lbl_ProcStatusText.Text = "Ready";
                        this.lbl_ProcStatusText.ForeColor = Color.BurlyWood;
                        break;
                    case 2:
                        this.pb_ProcStatusImage.Image = Properties.Resources.ImageSpinnerProcessing;
                        this.lbl_ProcStatusText.Text = "Wait...";
                        this.lbl_ProcStatusText.ForeColor = Color.CornflowerBlue;
                        break;
                    case 3:
                        this.pb_ProcStatusImage.Image = Properties.Resources.ImageFinished;
                        this.lbl_ProcStatusText.Text = "Success";
                        this.lbl_ProcStatusText.ForeColor = Color.Aquamarine;
                        break;
                    case 4:
                        this.pb_ProcStatusImage.Image = Properties.Resources.ImageFailed;
                        this.lbl_ProcStatusText.Text = "Error";
                        this.lbl_ProcStatusText.ForeColor = Color.OrangeRed;
                        break;
                    case 5:
                        this.pb_ProcStatusImage.Image = Properties.Resources.ImageSomeErrors;
                        this.lbl_ProcStatusText.Text = "Some errors";
                        this.lbl_ProcStatusText.ForeColor = Color.Coral;
                        break;
                    case 6:
                        this.pb_ProcStatusImage.Image = Properties.Resources.ImageStopped;
                        this.lbl_ProcStatusText.Text = "Stopped";
                        this.lbl_ProcStatusText.ForeColor = Color.Blue;
                        break;
                    case 7:
                        this.pb_ProcStatusImage.Image = Properties.Resources.ImageSpinnerCancelling;
                        this.lbl_ProcStatusText.Text = "Cancelling";
                        this.lbl_ProcStatusText.ForeColor = Color.DarkKhaki;
                        break;
                }
            }
            else
            {
                this.Invoke((Action<Byte>) UpdateStatus, StatusCode);
            }
        }

        /// <summary>
        /// true - enable; false - disable; null - negate status
        /// </summary>
        /// <param name="NewStatus"></param>
        private void SwitchStopButtonStatus(Nullable<Boolean> NewStatus)
        {
            if (this.btn_StopProcess.InvokeRequired == false)
            {
                switch (NewStatus)
                {
                    case true:
                        this.btn_StopProcess.Enabled = true;
                        break;
                    case false:
                        this.btn_StopProcess.Enabled = false;
                        break;
                    case null:
                        this.btn_StopProcess.Enabled = !this.btn_StopProcess.Enabled;
                        break;
                }
            }
            else
            {
                this.btn_StopProcess.Invoke((Action<Nullable<Boolean>>) this.SwitchStopButtonStatus, NewStatus);
            }
        }

        private void OnFormClosed(Object sender, FormClosedEventArgs e)
        {
            if (this._parsedItem != null)
            {
                this._parsedItem.Dispose();
                this._parsedItem = null;
            }
            if (this._cancelGrabbingPage != null)
            {
                this._cancelGrabbingPage.Dispose();
                this._cancelGrabbingPage = null;
            }
            if (this._cancelSongsDownload != null)
            {
                this._cancelSongsDownload.Dispose();
                this._cancelSongsDownload = null;
            }
        }

        private void btn_Options_Click(object sender, EventArgs e)
        {
            frm_Options options = new frm_Options();
            options.ShowDialog(this);
        }

        private void btn_Exit_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Добавляет новую запись в лог. Потоконезависим.
        /// </summary>
        /// <param name="Message"></param>
        private void AddToLog(String Message)
        {
            if (this.lb_Log.InvokeRequired == false)
            {
                String text = String.Format("{0} UTC: {1}", DateTime.UtcNow.ToString("yyyy-MM-dd HH.mm.ss.fff"), Message);
                this.lb_Log.TopIndex = this.lb_Log.Items.Add(text);
            }
            else
            {
                Action<String> deleg = AddToLog;
                this.lb_Log.Invoke(deleg, Message);
            }
        }

        private void ResetProgressBarAndCounter()
        {
            if (this.InvokeRequired == false)
            {
                this.prbr_Processing.Minimum = 0;
                this.prbr_Processing.Value = 0;
                this.prbr_Processing.Enabled = false;

                this.lbl_ProcessedCount.Text = empty;
                this.lbl_SelectedCount.Text = empty;
            }
            else
            {
                this.Invoke((Action) this.ResetProgressBarAndCounter);
            }
        }

        private void IncrementProgressBarAndCounter()
        {
            if (this.InvokeRequired == false)
            {
                this.prbr_Processing.PerformStep();
                Byte current_count = this.lbl_ProcessedCount.Text.TryParseNumber<Byte>(NumberStyles.None, null, 255);
                if (current_count == 255)
                {
                    throw new InvalidOperationException("Произошла внутренняя ошибка - невозможно получить из Label'а "+
                        "текущее количество обработанных песен '" + this.lbl_ProcessedCount.Text+"'");
                }
                this.lbl_ProcessedCount.Text = (current_count + 1).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                this.Invoke((Action) this.IncrementProgressBarAndCounter);
            }
        }

        #region Selection
        private void btn_InverseSelected_Click(object sender, EventArgs e)
        {
            if (this.dgv_List.Rows.Count < 1) { return; }
            foreach (DataGridViewRow one_row in dgv_List.Rows)
            {
                DataGridViewCell cell = one_row.Cells["col_Download"];
                if (cell.Value == null || (Boolean)cell.Value == false)
                { cell.Value = true; }
                else
                {
                    cell.Value = false;
                }
            }
        }

        private void btn_DeselectAll_Click(object sender, EventArgs e)
        {
            if (this.dgv_List.Rows.Count < 1) { return; }
            foreach (DataGridViewRow one_row in dgv_List.Rows)
            {
                DataGridViewCell cell = one_row.Cells["col_Download"];
                cell.Value = false;
            }
        }

        private void btn_SelectAll_Click(object sender, EventArgs e)
        {
            if (this.dgv_List.Rows.Count < 1) { return; }
            foreach (DataGridViewRow one_row in dgv_List.Rows)
            {
                DataGridViewCell cell = one_row.Cells["col_Download"];
                cell.Value = true;
            }
        }
        #endregion

        private async void btn_Grab_Click(object sender, EventArgs e)
        {
            this.SetOrAppendMessage(true, null);
            this.ResetProgressBarAndCounter();
            String error_message;
            Uri input_URI = Core.TryParseURI(this.tb_InputURI.Text, out error_message);
            if (input_URI == null)
            {
                this.lbl_InputFailed.Text = error_message;
                this.AddToLog("Введённая URI недопустима: " + error_message);
                return;
            }
            else
            {
                this.lbl_InputFailed.Text = "";
                this.AddToLog("Введённая URI корректна");
            }
            this.UpdateStatus(2);
            this.LockOrUnlockInterface(true);

            this.SwitchStopButtonStatus(true);
            this._cancelGrabbingPage = new CancellationTokenSource(TimeSpan.FromSeconds(60));

            Task<CommonDataBase> t = 
                Core.TryGrabAndParsePageAsync(input_URI, ProgramSettings.Instance.UserAgent, true, true, this._cancelGrabbingPage.Token);
            Task t1 = t.ContinueWith(this.Cancelled, TaskContinuationOptions.OnlyOnCanceled);
            Task t2 = t.ContinueWith(this.Finished, TaskContinuationOptions.NotOnCanceled);
            try
            {
                await t;
            }
            catch (Exception ex)
            {
                if (ex is OperationCanceledException || ex.InnerException is OperationCanceledException)
                {
                    //MessageBox.Show(ex.TotalMessage());
                }
                else
                {
                    this.AddToLog("Возникло необработанное неожиданное исключение при скачивании парсинге страницы: "+ex.TotalMessage());
                }
            }
        }

        private void Finished(Task<CommonDataBase> data)
        {
            this.SwitchStopButtonStatus(false);
            this._parsedItem = data.Result;
            data.Dispose();
            data = null;
            
            this.LockOrUnlockInterface(false);
            this._cancelGrabbingPage.Dispose();
            this._cancelGrabbingPage = null;
            if (this._parsedItem.IsNull() == true)
            {
                this.CleanLayout();
                this.UpdateStatus(4);
                return;
            }
            else if (this._parsedItem is ParsedAlbum)
            {
                this.UpdateStatus(3);
                this.RenderAlbum((ParsedAlbum)this._parsedItem);
                this.AddToLog("Данные альбома загружены");
            }
            else
            {
                this.UpdateStatus(3);
                this.RenderSong((ParsedSong)this._parsedItem);
                this.AddToLog("Данные песни загружены");
            }
            Action a = () =>
            {
                this.tb_InputURI.Text = this._parsedItem.ItemLink.ToString();
                if (this._URI_history.Count > 0)
                {
                    this.btn_Back.Enabled = true;
                }

            };
            this.Invoke(a);
            
            this._URI_history.Push(this._parsedItem.ItemLink);
        }

        private void Cancelled(Task<CommonDataBase> data)
        {
            this.SwitchStopButtonStatus(false);
            this.LockOrUnlockInterface(false);
            this.AddToLog("Задание по скачиванию и парсингу страницы было отменено");
            this.SetOrAppendMessage(true, "Процесс скачивания и парсинга страницы был успешно отменён");
            this.UpdateStatus(6);
            this.LockOrUnlockFooterButtons(true);
            data.Dispose();
            data = null;
        }

        #region Events
        private void Event1(System.Text.Encoding encoding, Int32 checksum)
        {
            this.AddToLog(String.Format("Этап 1: Страница скачана и успешно преобразована в HTML-документ с контрольной суммой {0} и кодировкой {1}", 
                checksum, encoding.ToString()));
        }
        private void Event2(ParsedItemType type)
        {
            this.AddToLog("Этап 2: Определён тип страницы — " + type.ToString());
        }
        private void Event3(ICommonHeader header)
        {
            this.AddToLog("Этап 3: Выполнен успешный парсинг хидера. Исполнитель - " + header.Artist + ", название - " + header.Title);
        }
        private void Event4(DownloadedFile file, Bitmap bmp)
        {
            this.AddToLog(String.Format(
                "Этап 4: Обложка успешно скачана и преобразована в изображение. "+
                "Название - {0}, размер - {1} байт, тип - {2}, линейные размеры - {3}.",
                file.Filename, file.Contentlength, ImageTools.GetImageFormat(bmp).ToString(), bmp.Size.ToString()));
        }
        private void Event5(CommonDataBase data)
        {
            this.AddToLog("Этап 5: Все данные получены успешно");
        }
        private void OnFailed(Exception ex)
        {
            this.AddToLog(ex.TotalMessage());
            this.SetOrAppendMessage(true, "Произошла ошибка при скачивании или парсинге страницы: " + ex.TotalMessage());
        }
        #endregion

        /// <summary>
        /// true - lock interface; false - unlock interface
        /// </summary>
        /// <param name="Lock"></param>
        private void LockOrUnlockInterface(Boolean Lock)
        {
            if (this.InvokeRequired == false)
            {
                this.gb_FooterButtons.Enabled = !Lock;
                this.btn_Grab.Enabled = !Lock;
                this.tb_InputURI.ReadOnly = Lock;
                if (Lock == true)
                {
                    this.btn_Back.Enabled = false;
                }
                else
                {
                    if (this._URI_history.Count > 1) {this.btn_Back.Enabled = true;}
                }
            }
            else
            {
                this.Invoke((Action<Boolean>)LockOrUnlockInterface, Lock);
            }
        }

        /// <summary>
        /// true - lock; false - unlock; null - negate
        /// </summary>
        /// <param name="Lock"></param>
        private void LockOrUnlockFooterButtons(Nullable<Boolean> Lock)
        {
            if (this.gb_FooterButtons.InvokeRequired == false)
            {
                switch (Lock)
                {
                    case true:
                        this.gb_FooterButtons.Enabled = false;
                        break;
                    case false:
                        this.gb_FooterButtons.Enabled = true;
                        break;
                    case null:
                        this.gb_FooterButtons.Enabled = !this.gb_FooterButtons.Enabled;
                        break;
                }
            }
            else
            {
                this.gb_FooterButtons.Invoke((Action<Nullable<Boolean>>) this.LockOrUnlockFooterButtons, Lock);
            }
        }

        /// <summary>
        /// Устанавливает новое или дополняет существующее текстовое сообщение о выполнении процесса
        /// </summary>
        /// <param name="SetOrAppend">true - set; false - append</param>
        /// <param name="Message"></param>
        private void SetOrAppendMessage(Boolean SetOrAppend, String Message)
        {
            if (this.rtb_DownloadFailed.InvokeRequired == false)
            {
                if (SetOrAppend == true)
                {
                    this.rtb_DownloadFailed.Text = Message.IsStringNullEmptyWhiteSpace() == true ? "" : Message;
                }
                else
                {
                    if (Message.HasVisibleChars() == false) {return;}
                    this.rtb_DownloadFailed.Text = this.rtb_DownloadFailed.Text + "\r\n" + Message;
                }
            }
            else
            {
                this.rtb_DownloadFailed.Invoke((Action<Boolean, String>) this.SetOrAppendMessage, SetOrAppend, Message);
            }
        }

        private void CleanLayout()
        {
            if (this.InvokeRequired == false)
            {
                this.pb_ItemImage.Image = null;
                this.btn_SaveImage.Enabled = false;
                this.lbl_ImageInfo.Text = empty;

                this.tb_Title.Text = empty;
                this.tb_RO_Artist.Text = empty;
                this.tb_RO_Genre.Text = empty;
                this.tb_RO_Uploader.Text = empty;
                this.tb_RO_Format.Text = empty;

                this.gb_SongHeader.Visible = false;
                this.btn_GoToAlbumPage.Enabled = false;
                this.tb_RO_AlbumURI.Text = empty;

                this.gb_Album_Header.Visible = false;
                this.tb_RO_Count_Album.Text = empty;
                this.tb_RO_Type_Album.Text = empty;
                this.tb_RO_Updater_Album.Text = empty;
                this.tb_RO_Date_Album.Text = empty;
                this.tb_RO_Description_Album.Text = empty;
                this.tb_RO_Label_Album.Text = empty;

                this.dgv_List.Rows.Clear();

                this.SwitchStopButtonStatus(false);

                this.LockOrUnlockFooterButtons(true);

                this.ResetProgressBarAndCounter();
            }
            else
            {
                this.Invoke((Action)this.CleanLayout);
            }
        }

        private void RenderAlbum(ParsedAlbum album)
        {
            if (this.InvokeRequired == false)
            {
                this.gb_SongHeader.Visible = false;
                this.gb_Album_Header.Visible = true;
                
                this.tb_Title.Text = album.Header.Title;
                this.tb_RO_Genre.Text = album.Genre;
                this.tb_RO_Artist.Text = album.Artist;
                this.tb_RO_Format.Text = album.Header.Format;
                this.tb_RO_Uploader.Text = album.Uploader;

                this.tb_RO_Date_Album.Text = album.Header.ReleaseDate;
                this.tb_RO_Type_Album.Text = album.Header.Type;
                this.tb_RO_Count_Album.Text = album.Header.SongsCount.ToString();
                this.tb_RO_Description_Album.Text = album.Header.Description;
                this.tb_RO_Updater_Album.Text = album.Header.Updater;
                this.tb_RO_Label_Album.Text = album.Header.Label;

                this.RenderImage(album);

                this.dgv_List.Rows.Clear();

                Int32 failed_count = 0;
                for (Int32 i = 0; i < album.Songs.Count; i++)
                {
                    OneSongHeader song = album.Songs[i];
                    this.dgv_List.Rows.Add
                        (true, song.Number, song.Artist, song.Title, song.Duration, song.Size, song.Bitrate);
                    if (song.IsAvailableForDownload == false)
                    {
                        DataGridViewRow drvr = this.dgv_List.Rows[i];
                        drvr.DefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.LightPink };
                        drvr.Cells["col_Download"].ReadOnly = true;
                        drvr.Cells["col_Download"].Value = false;
                        failed_count++;
                    }
                }
                if (failed_count > 0)
                {
                    if (failed_count == album.Songs.Count)
                    {
                        this.gb_FooterButtons.Enabled = false;
                        this.SetOrAppendMessage(true, String.Format(
                            "Все {0} песни в данном альбоме помечены на сайте как таковые, файлы которых утеряны",
                            album.Songs.Count));
                    }
                    else
                    {
                        this.SetOrAppendMessage(true, 
                            String.Format("Из {0} песен {1} помечены на сайте как таковые, файл которых утерян",
                                album.Songs.Count, failed_count));
                    }
                    this.UpdateStatus(5);
                }
            }
            else
            {
                Action<ParsedAlbum> a = RenderAlbum;
                this.Invoke(a, album);
            }
        }

        private void RenderSong(ParsedSong song)
        {
            if (this.InvokeRequired == false)
            {
                this.gb_Album_Header.Visible = false;
                this.gb_SongHeader.Visible = true;
                this.btn_GoToAlbumPage.Enabled = true;

                this.tb_Title.Text = song.Header.Album;
                this.tb_RO_Genre.Text = song.Genre;
                this.tb_RO_Artist.Text = song.Artist;
                this.tb_RO_Format.Text = song.Header.Format;
                this.tb_RO_Uploader.Text = song.Uploader;

                this.tb_RO_AlbumURI.Text = song.AlbumLink.ToString();

                this.RenderImage(song);

                this.dgv_List.Rows.Clear();

                this.dgv_List.Rows.Add(true, song.Header.Number, song.Header.Artist, song.Header.Name, song.Header.Duration,
                    song.Header.Size, song.Header.Bitrate);
                if (song.Header.IsAvailableForDownload == false)
                {
                    this.gb_FooterButtons.Enabled = false;
                    DataGridViewRow drvr = this.dgv_List.Rows[0];
                    drvr.DefaultCellStyle = new DataGridViewCellStyle() { BackColor = Color.LightPink };
                    drvr.Cells["col_Download"].ReadOnly = true;
                    drvr.Cells["col_Download"].Value = false;
                    this.SetOrAppendMessage(true, String.Format(
                        "Песня '{0}. {1}' помечена на сайте как таковая, файл которой утерян", song.Header.Number, song.Header.Name));
                    this.UpdateStatus(5);
                }
            }
            else
            {
                this.Invoke((Action<ParsedSong>)this.RenderSong, song);
            }
        }

        private void RenderImage(CommonDataBase Data)
        {
            if (this.InvokeRequired == false)
            {
                if (Data.CoverImage == null)
                {
                    this.pb_ItemImage.Image = null;
                    this.btn_SaveImage.Enabled = false;
                    this.lbl_ImageInfo.Text = "";
                }
                else
                {
                    this.pb_ItemImage.Image = Data.CoverImage;
                    this.btn_SaveImage.Enabled = true;
                    this.lbl_ImageInfo.Text = String.Format("{0} | {1} | {2} x {3} px",
                        ImageTools.GetImageFormat(Data.CoverImage),
                        KlotosLib.ByteQuantity.FromBytes(Data.CoverFile.Contentlength).ToStringWithBinaryPrefix(2, ByteQuantity.DecimalSeparatorSign.Point),
                        Data.CoverImage.Size.Width,
                        Data.CoverImage.Size.Height
                    );
                    
                }
            }
            else
            {
                this.Invoke((Action<CommonDataBase>)this.RenderImage, Data);
            }
        }

        private void btn_SaveImage_Click(object sender, EventArgs e)
        {
            if (this._parsedItem.CoverFile == null) { return; }

            String new_filename = null;
            if (ProgramSettings.Instance.UseServerFilenames == false)
            {
                new_filename = this._parsedItem.GenerateExternalCoverFilename();
                FilePathTools.TryCleanFilename(new_filename, out new_filename);
            }
            if (new_filename == null)
            {
                new_filename = this._parsedItem.CoverFile.Filename;
            }
            if(KlotosLib.FilePathTools.IsValidFilename(this.tb_Title.Text))
            {
                this._lastSavePath = ProgramSettings.PrepareSavePath(this.tb_Title.Text);
                
            }
            else
            {
                this._lastSavePath = ProgramSettings.PrepareSavePath(this._parsedItem.Album);
            }
            String finalSaveFolderPath = this._lastSavePath.FullName;
            Task<String> result = Core.TrySaveDownloadedFileToDiskAsync
                (this._parsedItem.CoverFile, finalSaveFolderPath, new_filename);
            result.ContinueWith(
                (Task<String> a) =>
                {
                    String res = a.Result;
                    if (res != null)
                    {
                        String err_mes = "Произошла ошибка при сохранении обложки на диск: " + res;
                        this.SetOrAppendMessage(true, err_mes);
                        this.AddToLog(err_mes);
                    }
                    else
                    {
                        this.AddToLog("Обложка успешно сохранена");
                    }
                }
            );
        }

        private void btn_DownloadSelected_Click(object sender, EventArgs e)
        {
            if(this._parsedItem == null) { return; }
            String updatedAlbumName = this.tb_Title.Text;
            String finalAlbumName;
            if (updatedAlbumName.Equals(this._parsedItem.Album, StringComparison.OrdinalIgnoreCase) == false && 
                KlotosLib.FilePathTools.IsValidFilename(updatedAlbumName))
            {
                finalAlbumName = updatedAlbumName;
            }
            else
            {
                finalAlbumName = this._parsedItem.Album;
            }
            this._lastSavePath = ProgramSettings.PrepareSavePath(finalAlbumName);
            
            this.LockOrUnlockInterface(true);

            if (this._parsedItem is ParsedSong)
            {
                this.SwitchStopButtonStatus(false);
                ParsedSong song = (ParsedSong)this._parsedItem;
                Object raw = this.dgv_List.Rows[0].Cells["col_Download"].Value;
                if (raw.IsNull() == true || (Boolean) raw == false)
                {
                    String message = String.Format("Песня {0} не была выбрана для скачивания", song.Header.Name);
                    this.AddToLog(message);
                    this.SetOrAppendMessage(true, message);
                    this.LockOrUnlockInterface(false);
                    return;
                }

                this.UpdateStatus(2);
                this.AddToLog("Выполнение задания по скачиванию и сохранению песни '"+song.Header.Name+"' началось");

                Task<DownloadedFile> task_song_file = Core.DownloadOneSongAsync(song, ProgramSettings.Instance.UserAgent);
                task_song_file.ContinueWith(
                    (Task<DownloadedFile> already_downloaded) =>
                    {
                        DownloadedFile file = already_downloaded.Result;
                        if (file == null)
                        {
                            this.LockOrUnlockInterface(false);
                            const string message = "Невозможно скачать файл песни с сервера";
                            this.SetOrAppendMessage(true, message);
                            this.AddToLog(message);
                            this.UpdateStatus(4);
                            return;
                        }
                        this.AddToLog(String.Format("Файл песни {0} размером {1} скачан", file.Filename, 
                            ByteQuantity.FromBytes(file.Contentlength).ToStringWithBinaryPrefix(3, ByteQuantity.DecimalSeparatorSign.Comma)));
                        String songCompleteFilename;
                        if (ProgramSettings.Instance.UseServerFilenames == true)
                        {
                            songCompleteFilename = file.Filename;
                        }
                        else
                        {
                            OverridableSongMetadata overridings = new OverridableSongMetadata()
                            {
                                AlbumName = this.tb_Title.Text,
                                SongName = this.dgv_List.Rows[0].Cells["col_Title"].Value.ToString(),
                                ArtistName = this.dgv_List.Rows[0].Cells["col_Artist"].Value.ToString(),
                            };
                            songCompleteFilename = song.Header.GenerateSongFilename(file.Filename, ProgramSettings.Instance.FilenameTemplate, overridings);
                        }
                        Task<String> res = Core.TrySaveDownloadedFileToDiskAsync(file, this._lastSavePath.FullName, songCompleteFilename);
                        res.ContinueWith(
                            (Task<String> res2) =>
                            {
                                this.LockOrUnlockInterface(false);
                                String message = res2.Result;
                                this.SetOrAppendMessage(true, message);
                                if (message == null)
                                {
                                    this.AddToLog("Песня успешно сохранена");
                                    this.SetOrAppendMessage(true, null);
                                    this.UpdateStatus(3);
                                }
                                else
                                {
                                    this.SetOrAppendMessage(true, "Произошла ошибка при сохранении скачанной песни на диск. " + message);
                                    this.AddToLog("Произошла ошибка при сохранении скачанной песни на диск. " + message);
                                    this.UpdateStatus(4);
                                }
                                return;
                            }
                        );
                    }
                );
            }
            else
            {
                ParsedAlbum album = this._parsedItem as ParsedAlbum;
                if (album == null)
                {
                    this.SetOrAppendMessage(true, "Произошла внутренняя ошибка");
                    this.AddToLog("Внутренняя ошибка - распарсенная ранее сущность не может быть успешно преобразована в тип "+
                        typeof(ParsedAlbum).FullName);
                    this.LockOrUnlockInterface(false);
                    return;
                }
                List<OneSongHeader> selected_for_download = new List<OneSongHeader>(album.Songs.Count);
                foreach (DataGridViewRow row in this.dgv_List.Rows)
                {
                    Boolean selected = row.Cells["col_Download"].Value.IsNull() != true && (Boolean)row.Cells["col_Download"].Value;
                    if (selected == true)
                    {
                        Byte number = (Byte)row.Cells["col_Number"].Value;
                        OneSongHeader selectedSong = album.Songs.Single(
                            (OneSongHeader song) => song.Number == number);
                        selectedSong.Album = this.tb_Title.Text.HasAlphaNumericChars() 
                            ? this.tb_Title.Text 
                            : selectedSong.Album;
                        selectedSong.Artist = ((string) row.Cells["col_Artist"].Value).HasAlphaNumericChars()
                            ? (string) row.Cells["col_Artist"].Value
                            : selectedSong.Artist;
                        selectedSong.Title = ((string)row.Cells["col_Title"].Value).HasAlphaNumericChars()
                            ? (string)row.Cells["col_Title"].Value
                            : selectedSong.Title;
                        selected_for_download.Add(selectedSong);
                    }
                }
                if (selected_for_download.Count == 0)
                {
                    String message = String.Format("Не было выбрано ни одной из {0} доступных песен", album.Songs.Count);
                    this.AddToLog(message);
                    this.SetOrAppendMessage(true, message);
                    this.LockOrUnlockInterface(false);
                    return;
                }
                this.UpdateStatus(2);
                this.SwitchStopButtonStatus(true);
                this.DownloadAndSaveSelectedSongs(selected_for_download, this._lastSavePath.FullName);
                return;
            }
        }

        private void btn_Open_Click(object sender, EventArgs e)
        {
            String openFolderPath;
            if (this._lastSavePath != null && this._lastSavePath.Exists)
            {
                openFolderPath = this._lastSavePath.FullName;
            }
            else
            {
                openFolderPath = ProgramSettings.PrepareSavePath(this._parsedItem.Album).FullName;
            }
            DirectoryInfo dir = new DirectoryInfo(openFolderPath);
            bool exists = dir.Exists;
            while (exists == false)
            {
                dir = dir.Parent;
                exists = dir.Exists;
            }
            System.Diagnostics.Process.Start(dir.FullName);
        }

        private void DownloadAndSaveSelectedSongs(List<OneSongHeader> songsForDownload, String downloadFolderPath)
        {
            songsForDownload.ThrowIfNullOrEmpty();
            this._cancelSongsDownload = new CancellationTokenSource();

            this.AddToLog("Выполнение задания по скачиванию и сохранению "+songsForDownload.Count+" песен началось");
            this.prbr_Processing.Enabled = true;
            this.prbr_Processing.Minimum = 0;
            this.prbr_Processing.Maximum = songsForDownload.Count;
            this.prbr_Processing.Value = 0;
            this.prbr_Processing.Step = 1;
            this.lbl_SelectedCount.Text = "/" + songsForDownload.Count;
            this.lbl_ProcessedCount.Text = "0";

            ReactiveDownloader rd = ReactiveDownloader.CreateTask
                (songsForDownload, ProgramSettings.Instance.UserAgent, downloadFolderPath, 
                !ProgramSettings.Instance.UseServerFilenames, ProgramSettings.Instance.FilenameTemplate);

            rd.OnCancellation += delegate(Int32 i, Int32 j)
            {
                this.AddToLog("Результат запроса на отмену скачивания: успело скачаться "+i +" песен из "+j+" запрошенных");
            };
            rd.OnComplete += delegate(IDictionary<OneSongHeader, Exception> res)
            {
                if (res.Any(elem => elem.Value == null))
                {
                    this.AddToLog("Скачивание всех " + res.Count + " песен успешно завершено");
                }
            };
            rd.OnNext += delegate(OneSongHeader header, Exception exception)
            {
                this.AddToLog(String.Format("Получен результат песни '{0}.{1}' - {2}", 
                    header.Number, header.Name, exception == null ? "Ok" : exception.TotalMessage()));
                this.IncrementProgressBarAndCounter();
            };

            Task<IDictionary<OneSongHeader, Exception>> res_task = rd.StartAsync
                (this._cancelSongsDownload.Token, ProgramSettings.Instance.MaxDownloadThreads);
            res_task.ContinueWith(
                (Task<IDictionary<OneSongHeader, Exception>> r) =>
                {
                    IDictionary<OneSongHeader, Exception> result = r.Result;
                    Dictionary<OneSongHeader, Exception> failed =
                        result.Where(kvp => kvp.Value != null).ConvertToDictionary();
                    String message;
                    if (failed.IsNullOrEmpty() == true)
                    {
                        message = String.Format("Скачивание и сохранение на диск всех {0} песен успешно завершено",
                            result.Count);
                        this.UpdateStatus(3);
                    }
                    else if (failed.Count == result.Count)
                    {
                        message = String.Format(
                            "Не удалось скачать или сохранить на диск ни одну из {0} песен. Список ошибок: \r\n{1}",
                            result.Count,
                            failed.ConcatToString
                                (key => "'" + key.Number.ToString() + ". " + key.Name + "'",
                                    value => value.TotalMessage(), " --> ", ";\r\n "));
                        this.UpdateStatus(4);
                    }
                    else
                    {
                        message = String.Format(
                            "Из {0} выбранных песен не удалось скачать {1}. Список ошибок: \r\n{2}",
                            result.Count, failed.Count, failed.ConcatToString
                                (key => "'" + key.Number + ". " + key.Name + "'",
                                    value => value.TotalMessage(), " --> ", ";\r\n "));
                        this.UpdateStatus(5);
                    }
                    this.AddToLog(message);
                    this.SetOrAppendMessage(true, message);
                    this.LockOrUnlockInterface(false);
                    this.SwitchStopButtonStatus(false);
                    this._cancelSongsDownload.Dispose();
                    this._cancelSongsDownload = null;
                }, TaskContinuationOptions.NotOnCanceled);

            res_task.ContinueWith(
                (Task<IDictionary<OneSongHeader, Exception>> r) =>
                {
                    this.LockOrUnlockInterface(false);
                    this.SwitchStopButtonStatus(false);
                    this.UpdateStatus(6);
                    String message = "Отмена операции скачивания и сохранения успешно завершена";
                    this.AddToLog(message);
                    this.SetOrAppendMessage(true, message);
                }, TaskContinuationOptions.OnlyOnCanceled);
            
        }

        private void btn_GoToAlbumPage_Click(object sender, EventArgs e)
        {
            ParsedSong song = this._parsedItem as ParsedSong;
            if (song != null)
            {
                this.tb_InputURI.Text = song.AlbumLink.ToString();
                this.btn_GoToAlbumPage.Enabled = false;
                this.UpdateStatus(1);
                this.btn_Grab_Click(null, EventArgs.Empty);
            }
        }

        private void btn_StopProcess_Click(object sender, EventArgs e)
        {
            if (this._cancelGrabbingPage != null)
            {
                this._cancelGrabbingPage.Cancel();
                this._cancelGrabbingPage.Dispose();
                this._cancelGrabbingPage = null;
            }
            else if (this._cancelSongsDownload != null)
            {
                this._cancelSongsDownload.Cancel();
                this._cancelSongsDownload.Dispose();
                this._cancelSongsDownload = null;
                {
                    this.UpdateStatus(7);
                    this.AddToLog("Запрошена отмена операции скачивания и сохранения песен");
                    this.SetOrAppendMessage(true, "Выполняется отмена операции скачивания и сохранения песен. Те песни, которые уже начали скачиваться или сохраняться на диск, будут докачаны и сохранены.");
                }
            }
            this.SwitchStopButtonStatus(false);
        }

        private void btn_About_Click(object sender, EventArgs e)
        {
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
            IEnumerable<String> ass_info = Tools.GetAssemblyDescription(assemblies);
            
            Int32 i = 0;
            string ass_info_final = ass_info.ConcatToString((String x) => { i++; return i + ". " + x; }, "", "", "\r\n", true);
            
            String about_text = String.Format(this.GetProgramName()+"\r\nCopyright © Klotos\r\n" +
                "Created with C# 5.0, .NET Framework 4.5, VS 2012\r\n\r\nEnvironment information\r\n"+
                "Logical processors count: {0} \r\nMachine name: {1}\r\nOperating system: {2}"+
                "\r\nRunning as 64-bit application: {3}\r\nCurrent folder: {4}\r\n\r\nAssemblies info:\r\n{5}",
                Environment.ProcessorCount, Environment.MachineName,
                Environment.OSVersion.ToString()+" "+(Environment.Is64BitOperatingSystem == true ? "64-bit" : "32-bit"), 
                Environment.Is64BitProcess == true ? "Yes" : "No", ProgramSettings.Default.SavedFilesPath,
                ass_info_final);

            MessageBox.Show(about_text, "About", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private String GetProgramName()
        {
            return String.Format("Myzuka.ru Grabber ver.{0}.{1}",
                this.GetType().Assembly.GetName().Version.Major, this.GetType().Assembly.GetName().Version.Minor);
        }
        
        private void btn_Back_Click(object sender, EventArgs e)
        {
            if (this._URI_history.IsNullOrEmpty() == true || this._URI_history.HasSingle()==true)
            {
                this.btn_Back.Enabled = false;
            }
            else
            {
                this._URI_history.Pop();
                this.tb_InputURI.Text = this._URI_history.Pop().ToString();
                this.btn_Grab_Click(null, EventArgs.Empty);
            }
        }
    }
}
