namespace MyzukaRuGrabberGUI
{
    partial class frm_Main
    {
        /// <summary>
        /// Требуется переменная конструктора.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Освободить все используемые ресурсы.
        /// </summary>
        /// <param name="disposing">истинно, если управляемый ресурс должен быть удален; иначе ложно.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Код, автоматически созданный конструктором форм Windows

        /// <summary>
        /// Обязательный метод для поддержки конструктора - не изменяйте
        /// содержимое данного метода при помощи редактора кода.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(frm_Main));
            this.lbl_InputURI = new System.Windows.Forms.Label();
            this.lbl_InputFailed = new System.Windows.Forms.Label();
            this.btn_Grab = new System.Windows.Forms.Button();
            this.tb_InputURI = new System.Windows.Forms.TextBox();
            this.btn_SaveImage = new System.Windows.Forms.Button();
            this.pb_ItemImage = new System.Windows.Forms.PictureBox();
            this.tb_RO_Format = new System.Windows.Forms.TextBox();
            this.tb_RO_Genre = new System.Windows.Forms.TextBox();
            this.lbl_D_Format = new System.Windows.Forms.Label();
            this.lbl__Uploader = new System.Windows.Forms.Label();
            this.lbl_D_Genre = new System.Windows.Forms.Label();
            this.tb_RO_Uploader = new System.Windows.Forms.TextBox();
            this.lbl_D_Artist = new System.Windows.Forms.Label();
            this.tb_RO_Artist = new System.Windows.Forms.TextBox();
            this.tb_RO_Title = new System.Windows.Forms.TextBox();
            this.lbl_D_Albun = new System.Windows.Forms.Label();
            this.gb_Album_Header = new System.Windows.Forms.GroupBox();
            this.tb_RO_Description_Album = new System.Windows.Forms.RichTextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_RO_Count_Album = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.tb_RO_Updater_Album = new System.Windows.Forms.TextBox();
            this.tb_RO_Type_Album = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.tb_RO_Date_Album = new System.Windows.Forms.TextBox();
            this.dgv_List = new System.Windows.Forms.DataGridView();
            this.col_Number = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Artist = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Title = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Length = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Size = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Bitrate = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.col_Download = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.gb_FooterButtons = new System.Windows.Forms.GroupBox();
            this.btn_DownloadSelected = new System.Windows.Forms.Button();
            this.btn_SelectAll = new System.Windows.Forms.Button();
            this.btn_DeselectAll = new System.Windows.Forms.Button();
            this.btn_InverseSelected = new System.Windows.Forms.Button();
            this.btn_Options = new System.Windows.Forms.Button();
            this.btn_Exit = new System.Windows.Forms.Button();
            this.lb_Log = new System.Windows.Forms.ListBox();
            this.rtb_DownloadFailed = new System.Windows.Forms.RichTextBox();
            this.gb_SongHeader = new System.Windows.Forms.GroupBox();
            this.btn_GoToAlbumPage = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.tb_RO_AlbumURI = new System.Windows.Forms.TextBox();
            this.pb_ProcStatusImage = new System.Windows.Forms.PictureBox();
            this.lbl_ProcStatusText = new System.Windows.Forms.Label();
            this.btn_StopProcess = new System.Windows.Forms.Button();
            this.gb_MainHeader = new System.Windows.Forms.GroupBox();
            this.lbl_ImageInfo = new System.Windows.Forms.Label();
            this.btn_About = new System.Windows.Forms.Button();
            this.prbr_Processing = new System.Windows.Forms.ProgressBar();
            this.lbl_SelectedCount = new System.Windows.Forms.Label();
            this.lbl_ProcessedCount = new System.Windows.Forms.Label();
            this.btn_Back = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.pb_ItemImage)).BeginInit();
            this.gb_Album_Header.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_List)).BeginInit();
            this.gb_FooterButtons.SuspendLayout();
            this.gb_SongHeader.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_ProcStatusImage)).BeginInit();
            this.gb_MainHeader.SuspendLayout();
            this.SuspendLayout();
            // 
            // lbl_InputURI
            // 
            this.lbl_InputURI.AutoSize = true;
            this.lbl_InputURI.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_InputURI.Location = new System.Drawing.Point(64, 8);
            this.lbl_InputURI.Name = "lbl_InputURI";
            this.lbl_InputURI.Size = new System.Drawing.Size(139, 16);
            this.lbl_InputURI.TabIndex = 8;
            this.lbl_InputURI.Text = "Input URI to Myzuka.ru";
            // 
            // lbl_InputFailed
            // 
            this.lbl_InputFailed.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_InputFailed.ForeColor = System.Drawing.Color.Red;
            this.lbl_InputFailed.Location = new System.Drawing.Point(219, 8);
            this.lbl_InputFailed.Name = "lbl_InputFailed";
            this.lbl_InputFailed.Size = new System.Drawing.Size(467, 22);
            this.lbl_InputFailed.TabIndex = 7;
            // 
            // btn_Grab
            // 
            this.btn_Grab.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_Grab.Location = new System.Drawing.Point(711, 8);
            this.btn_Grab.Name = "btn_Grab";
            this.btn_Grab.Size = new System.Drawing.Size(61, 46);
            this.btn_Grab.TabIndex = 6;
            this.btn_Grab.Text = "Grab";
            this.btn_Grab.UseVisualStyleBackColor = true;
            this.btn_Grab.Click += new System.EventHandler(this.btn_Grab_Click);
            // 
            // tb_InputURI
            // 
            this.tb_InputURI.Location = new System.Drawing.Point(64, 34);
            this.tb_InputURI.Name = "tb_InputURI";
            this.tb_InputURI.Size = new System.Drawing.Size(622, 20);
            this.tb_InputURI.TabIndex = 5;
            // 
            // btn_SaveImage
            // 
            this.btn_SaveImage.Enabled = false;
            this.btn_SaveImage.Location = new System.Drawing.Point(58, 295);
            this.btn_SaveImage.Name = "btn_SaveImage";
            this.btn_SaveImage.Size = new System.Drawing.Size(87, 23);
            this.btn_SaveImage.TabIndex = 32;
            this.btn_SaveImage.Text = "Save image";
            this.btn_SaveImage.UseVisualStyleBackColor = true;
            this.btn_SaveImage.Click += new System.EventHandler(this.btn_SaveImage_Click);
            // 
            // pb_ItemImage
            // 
            this.pb_ItemImage.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.pb_ItemImage.Location = new System.Drawing.Point(9, 60);
            this.pb_ItemImage.Name = "pb_ItemImage";
            this.pb_ItemImage.Size = new System.Drawing.Size(200, 200);
            this.pb_ItemImage.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.pb_ItemImage.TabIndex = 31;
            this.pb_ItemImage.TabStop = false;
            // 
            // tb_RO_Format
            // 
            this.tb_RO_Format.Location = new System.Drawing.Point(392, 74);
            this.tb_RO_Format.Name = "tb_RO_Format";
            this.tb_RO_Format.ReadOnly = true;
            this.tb_RO_Format.Size = new System.Drawing.Size(130, 20);
            this.tb_RO_Format.TabIndex = 41;
            // 
            // tb_RO_Genre
            // 
            this.tb_RO_Genre.Location = new System.Drawing.Point(392, 46);
            this.tb_RO_Genre.Name = "tb_RO_Genre";
            this.tb_RO_Genre.ReadOnly = true;
            this.tb_RO_Genre.Size = new System.Drawing.Size(130, 20);
            this.tb_RO_Genre.TabIndex = 43;
            // 
            // lbl_D_Format
            // 
            this.lbl_D_Format.AutoSize = true;
            this.lbl_D_Format.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_D_Format.Location = new System.Drawing.Point(341, 75);
            this.lbl_D_Format.Name = "lbl_D_Format";
            this.lbl_D_Format.Size = new System.Drawing.Size(50, 16);
            this.lbl_D_Format.TabIndex = 40;
            this.lbl_D_Format.Text = "Format";
            // 
            // lbl__Uploader
            // 
            this.lbl__Uploader.AutoSize = true;
            this.lbl__Uploader.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl__Uploader.Location = new System.Drawing.Point(7, 74);
            this.lbl__Uploader.Name = "lbl__Uploader";
            this.lbl__Uploader.Size = new System.Drawing.Size(65, 16);
            this.lbl__Uploader.TabIndex = 38;
            this.lbl__Uploader.Text = "Uploader";
            // 
            // lbl_D_Genre
            // 
            this.lbl_D_Genre.AutoSize = true;
            this.lbl_D_Genre.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_D_Genre.Location = new System.Drawing.Point(341, 47);
            this.lbl_D_Genre.Name = "lbl_D_Genre";
            this.lbl_D_Genre.Size = new System.Drawing.Size(45, 16);
            this.lbl_D_Genre.TabIndex = 42;
            this.lbl_D_Genre.Text = "Genre";
            // 
            // tb_RO_Uploader
            // 
            this.tb_RO_Uploader.Location = new System.Drawing.Point(69, 72);
            this.tb_RO_Uploader.Name = "tb_RO_Uploader";
            this.tb_RO_Uploader.ReadOnly = true;
            this.tb_RO_Uploader.Size = new System.Drawing.Size(269, 20);
            this.tb_RO_Uploader.TabIndex = 39;
            // 
            // lbl_D_Artist
            // 
            this.lbl_D_Artist.AutoSize = true;
            this.lbl_D_Artist.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_D_Artist.Location = new System.Drawing.Point(9, 47);
            this.lbl_D_Artist.Name = "lbl_D_Artist";
            this.lbl_D_Artist.Size = new System.Drawing.Size(37, 16);
            this.lbl_D_Artist.TabIndex = 44;
            this.lbl_D_Artist.Text = "Artist";
            // 
            // tb_RO_Artist
            // 
            this.tb_RO_Artist.Location = new System.Drawing.Point(69, 46);
            this.tb_RO_Artist.Name = "tb_RO_Artist";
            this.tb_RO_Artist.ReadOnly = true;
            this.tb_RO_Artist.Size = new System.Drawing.Size(269, 20);
            this.tb_RO_Artist.TabIndex = 45;
            // 
            // tb_RO_Title
            // 
            this.tb_RO_Title.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_RO_Title.ForeColor = System.Drawing.SystemColors.Info;
            this.tb_RO_Title.Location = new System.Drawing.Point(69, 14);
            this.tb_RO_Title.Name = "tb_RO_Title";
            this.tb_RO_Title.ReadOnly = true;
            this.tb_RO_Title.Size = new System.Drawing.Size(453, 26);
            this.tb_RO_Title.TabIndex = 37;
            // 
            // lbl_D_Albun
            // 
            this.lbl_D_Albun.AutoSize = true;
            this.lbl_D_Albun.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_D_Albun.Location = new System.Drawing.Point(6, 17);
            this.lbl_D_Albun.Name = "lbl_D_Albun";
            this.lbl_D_Albun.Size = new System.Drawing.Size(54, 20);
            this.lbl_D_Albun.TabIndex = 46;
            this.lbl_D_Albun.Text = "Album";
            // 
            // gb_Album_Header
            // 
            this.gb_Album_Header.Controls.Add(this.tb_RO_Description_Album);
            this.gb_Album_Header.Controls.Add(this.label1);
            this.gb_Album_Header.Controls.Add(this.tb_RO_Count_Album);
            this.gb_Album_Header.Controls.Add(this.label6);
            this.gb_Album_Header.Controls.Add(this.tb_RO_Updater_Album);
            this.gb_Album_Header.Controls.Add(this.tb_RO_Type_Album);
            this.gb_Album_Header.Controls.Add(this.label5);
            this.gb_Album_Header.Controls.Add(this.label4);
            this.gb_Album_Header.Controls.Add(this.tb_RO_Date_Album);
            this.gb_Album_Header.Location = new System.Drawing.Point(232, 173);
            this.gb_Album_Header.Name = "gb_Album_Header";
            this.gb_Album_Header.Size = new System.Drawing.Size(540, 145);
            this.gb_Album_Header.TabIndex = 47;
            this.gb_Album_Header.TabStop = false;
            this.gb_Album_Header.Visible = false;
            // 
            // tb_RO_Description_Album
            // 
            this.tb_RO_Description_Album.Location = new System.Drawing.Point(197, 15);
            this.tb_RO_Description_Album.Name = "tb_RO_Description_Album";
            this.tb_RO_Description_Album.ReadOnly = true;
            this.tb_RO_Description_Album.Size = new System.Drawing.Size(326, 123);
            this.tb_RO_Description_Album.TabIndex = 23;
            this.tb_RO_Description_Album.Text = "";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(11, 97);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(57, 16);
            this.label1.TabIndex = 24;
            this.label1.Text = "Updater";
            // 
            // tb_RO_Count_Album
            // 
            this.tb_RO_Count_Album.Location = new System.Drawing.Point(73, 71);
            this.tb_RO_Count_Album.Name = "tb_RO_Count_Album";
            this.tb_RO_Count_Album.ReadOnly = true;
            this.tb_RO_Count_Album.Size = new System.Drawing.Size(109, 20);
            this.tb_RO_Count_Album.TabIndex = 16;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label6.Location = new System.Drawing.Point(11, 72);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(42, 16);
            this.label6.TabIndex = 15;
            this.label6.Text = "Count";
            // 
            // tb_RO_Updater_Album
            // 
            this.tb_RO_Updater_Album.Location = new System.Drawing.Point(73, 97);
            this.tb_RO_Updater_Album.Name = "tb_RO_Updater_Album";
            this.tb_RO_Updater_Album.ReadOnly = true;
            this.tb_RO_Updater_Album.Size = new System.Drawing.Size(109, 20);
            this.tb_RO_Updater_Album.TabIndex = 25;
            // 
            // tb_RO_Type_Album
            // 
            this.tb_RO_Type_Album.Location = new System.Drawing.Point(73, 45);
            this.tb_RO_Type_Album.Name = "tb_RO_Type_Album";
            this.tb_RO_Type_Album.ReadOnly = true;
            this.tb_RO_Type_Album.Size = new System.Drawing.Size(109, 20);
            this.tb_RO_Type_Album.TabIndex = 14;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(11, 46);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(40, 16);
            this.label5.TabIndex = 13;
            this.label5.Text = "Type";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(11, 20);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(37, 16);
            this.label4.TabIndex = 11;
            this.label4.Text = "Date";
            // 
            // tb_RO_Date_Album
            // 
            this.tb_RO_Date_Album.Location = new System.Drawing.Point(73, 19);
            this.tb_RO_Date_Album.Name = "tb_RO_Date_Album";
            this.tb_RO_Date_Album.ReadOnly = true;
            this.tb_RO_Date_Album.Size = new System.Drawing.Size(109, 20);
            this.tb_RO_Date_Album.TabIndex = 12;
            // 
            // dgv_List
            // 
            this.dgv_List.AllowUserToAddRows = false;
            this.dgv_List.AllowUserToDeleteRows = false;
            this.dgv_List.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
            this.dgv_List.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.dgv_List.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.col_Number,
            this.col_Artist,
            this.col_Title,
            this.col_Length,
            this.col_Size,
            this.col_Bitrate,
            this.col_Download});
            this.dgv_List.Location = new System.Drawing.Point(9, 421);
            this.dgv_List.Name = "dgv_List";
            this.dgv_List.Size = new System.Drawing.Size(763, 177);
            this.dgv_List.TabIndex = 48;
            // 
            // col_Number
            // 
            this.col_Number.Frozen = true;
            this.col_Number.HeaderText = "No";
            this.col_Number.Name = "col_Number";
            this.col_Number.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_Number.Width = 46;
            // 
            // col_Artist
            // 
            this.col_Artist.Frozen = true;
            this.col_Artist.HeaderText = "Artist";
            this.col_Artist.Name = "col_Artist";
            this.col_Artist.Width = 55;
            // 
            // col_Title
            // 
            this.col_Title.Frozen = true;
            this.col_Title.HeaderText = "Title";
            this.col_Title.Name = "col_Title";
            this.col_Title.Width = 52;
            // 
            // col_Length
            // 
            this.col_Length.Frozen = true;
            this.col_Length.HeaderText = "Length";
            this.col_Length.Name = "col_Length";
            this.col_Length.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_Length.Width = 65;
            // 
            // col_Size
            // 
            this.col_Size.Frozen = true;
            this.col_Size.HeaderText = "Size";
            this.col_Size.Name = "col_Size";
            this.col_Size.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_Size.Width = 52;
            // 
            // col_Bitrate
            // 
            this.col_Bitrate.Frozen = true;
            this.col_Bitrate.HeaderText = "Bitrate";
            this.col_Bitrate.Name = "col_Bitrate";
            this.col_Bitrate.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_Bitrate.Width = 62;
            // 
            // col_Download
            // 
            this.col_Download.Frozen = true;
            this.col_Download.HeaderText = "Download";
            this.col_Download.Name = "col_Download";
            this.col_Download.Resizable = System.Windows.Forms.DataGridViewTriState.False;
            this.col_Download.Width = 61;
            // 
            // gb_FooterButtons
            // 
            this.gb_FooterButtons.Controls.Add(this.btn_DownloadSelected);
            this.gb_FooterButtons.Controls.Add(this.btn_SelectAll);
            this.gb_FooterButtons.Controls.Add(this.btn_DeselectAll);
            this.gb_FooterButtons.Controls.Add(this.btn_InverseSelected);
            this.gb_FooterButtons.Location = new System.Drawing.Point(9, 684);
            this.gb_FooterButtons.Name = "gb_FooterButtons";
            this.gb_FooterButtons.Size = new System.Drawing.Size(478, 45);
            this.gb_FooterButtons.TabIndex = 49;
            this.gb_FooterButtons.TabStop = false;
            // 
            // btn_DownloadSelected
            // 
            this.btn_DownloadSelected.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_DownloadSelected.ForeColor = System.Drawing.SystemColors.HotTrack;
            this.btn_DownloadSelected.Location = new System.Drawing.Point(6, 14);
            this.btn_DownloadSelected.Name = "btn_DownloadSelected";
            this.btn_DownloadSelected.Size = new System.Drawing.Size(168, 23);
            this.btn_DownloadSelected.TabIndex = 25;
            this.btn_DownloadSelected.Text = "Download selected";
            this.btn_DownloadSelected.UseVisualStyleBackColor = true;
            this.btn_DownloadSelected.Click += new System.EventHandler(this.btn_DownloadSelected_Click);
            // 
            // btn_SelectAll
            // 
            this.btn_SelectAll.Location = new System.Drawing.Point(194, 14);
            this.btn_SelectAll.Name = "btn_SelectAll";
            this.btn_SelectAll.Size = new System.Drawing.Size(75, 23);
            this.btn_SelectAll.TabIndex = 26;
            this.btn_SelectAll.Text = "Select all";
            this.btn_SelectAll.UseVisualStyleBackColor = true;
            this.btn_SelectAll.Click += new System.EventHandler(this.btn_SelectAll_Click);
            // 
            // btn_DeselectAll
            // 
            this.btn_DeselectAll.Location = new System.Drawing.Point(275, 14);
            this.btn_DeselectAll.Name = "btn_DeselectAll";
            this.btn_DeselectAll.Size = new System.Drawing.Size(75, 23);
            this.btn_DeselectAll.TabIndex = 27;
            this.btn_DeselectAll.Text = "Deselect all";
            this.btn_DeselectAll.UseVisualStyleBackColor = true;
            this.btn_DeselectAll.Click += new System.EventHandler(this.btn_DeselectAll_Click);
            // 
            // btn_InverseSelected
            // 
            this.btn_InverseSelected.Location = new System.Drawing.Point(356, 14);
            this.btn_InverseSelected.Name = "btn_InverseSelected";
            this.btn_InverseSelected.Size = new System.Drawing.Size(110, 23);
            this.btn_InverseSelected.TabIndex = 28;
            this.btn_InverseSelected.Text = "Inverse selected";
            this.btn_InverseSelected.UseVisualStyleBackColor = true;
            this.btn_InverseSelected.Click += new System.EventHandler(this.btn_InverseSelected_Click);
            // 
            // btn_Options
            // 
            this.btn_Options.Location = new System.Drawing.Point(603, 698);
            this.btn_Options.Name = "btn_Options";
            this.btn_Options.Size = new System.Drawing.Size(61, 23);
            this.btn_Options.TabIndex = 51;
            this.btn_Options.Text = "Options";
            this.btn_Options.UseVisualStyleBackColor = true;
            this.btn_Options.Click += new System.EventHandler(this.btn_Options_Click);
            // 
            // btn_Exit
            // 
            this.btn_Exit.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_Exit.Location = new System.Drawing.Point(697, 698);
            this.btn_Exit.Name = "btn_Exit";
            this.btn_Exit.Size = new System.Drawing.Size(75, 23);
            this.btn_Exit.TabIndex = 50;
            this.btn_Exit.Text = "Exit";
            this.btn_Exit.UseVisualStyleBackColor = true;
            this.btn_Exit.Click += new System.EventHandler(this.btn_Exit_Click);
            // 
            // lb_Log
            // 
            this.lb_Log.FormattingEnabled = true;
            this.lb_Log.HorizontalScrollbar = true;
            this.lb_Log.Location = new System.Drawing.Point(9, 604);
            this.lb_Log.Name = "lb_Log";
            this.lb_Log.Size = new System.Drawing.Size(763, 82);
            this.lb_Log.TabIndex = 52;
            // 
            // rtb_DownloadFailed
            // 
            this.rtb_DownloadFailed.Location = new System.Drawing.Point(232, 360);
            this.rtb_DownloadFailed.Name = "rtb_DownloadFailed";
            this.rtb_DownloadFailed.ReadOnly = true;
            this.rtb_DownloadFailed.Size = new System.Drawing.Size(540, 55);
            this.rtb_DownloadFailed.TabIndex = 53;
            this.rtb_DownloadFailed.Text = "";
            // 
            // gb_SongHeader
            // 
            this.gb_SongHeader.Controls.Add(this.btn_GoToAlbumPage);
            this.gb_SongHeader.Controls.Add(this.label2);
            this.gb_SongHeader.Controls.Add(this.tb_RO_AlbumURI);
            this.gb_SongHeader.Location = new System.Drawing.Point(232, 173);
            this.gb_SongHeader.Name = "gb_SongHeader";
            this.gb_SongHeader.Size = new System.Drawing.Size(540, 101);
            this.gb_SongHeader.TabIndex = 54;
            this.gb_SongHeader.TabStop = false;
            this.gb_SongHeader.Visible = false;
            // 
            // btn_GoToAlbumPage
            // 
            this.btn_GoToAlbumPage.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_GoToAlbumPage.Location = new System.Drawing.Point(160, 48);
            this.btn_GoToAlbumPage.Name = "btn_GoToAlbumPage";
            this.btn_GoToAlbumPage.Size = new System.Drawing.Size(180, 39);
            this.btn_GoToAlbumPage.TabIndex = 28;
            this.btn_GoToAlbumPage.Text = "Go to the Album page";
            this.btn_GoToAlbumPage.UseVisualStyleBackColor = true;
            this.btn_GoToAlbumPage.Click += new System.EventHandler(this.btn_GoToAlbumPage_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(6, 23);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(72, 16);
            this.label2.TabIndex = 26;
            this.label2.Text = "Album URI";
            // 
            // tb_RO_AlbumURI
            // 
            this.tb_RO_AlbumURI.Location = new System.Drawing.Point(84, 22);
            this.tb_RO_AlbumURI.Name = "tb_RO_AlbumURI";
            this.tb_RO_AlbumURI.ReadOnly = true;
            this.tb_RO_AlbumURI.Size = new System.Drawing.Size(439, 20);
            this.tb_RO_AlbumURI.TabIndex = 27;
            // 
            // pb_ProcStatusImage
            // 
            this.pb_ProcStatusImage.Location = new System.Drawing.Point(9, 339);
            this.pb_ProcStatusImage.Name = "pb_ProcStatusImage";
            this.pb_ProcStatusImage.Size = new System.Drawing.Size(76, 76);
            this.pb_ProcStatusImage.TabIndex = 55;
            this.pb_ProcStatusImage.TabStop = false;
            // 
            // lbl_ProcStatusText
            // 
            this.lbl_ProcStatusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_ProcStatusText.Location = new System.Drawing.Point(91, 339);
            this.lbl_ProcStatusText.Name = "lbl_ProcStatusText";
            this.lbl_ProcStatusText.Size = new System.Drawing.Size(118, 34);
            this.lbl_ProcStatusText.TabIndex = 56;
            this.lbl_ProcStatusText.Text = "Some errors";
            this.lbl_ProcStatusText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // btn_StopProcess
            // 
            this.btn_StopProcess.Enabled = false;
            this.btn_StopProcess.Font = new System.Drawing.Font("Microsoft Sans Serif", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_StopProcess.Location = new System.Drawing.Point(131, 380);
            this.btn_StopProcess.Name = "btn_StopProcess";
            this.btn_StopProcess.Size = new System.Drawing.Size(78, 35);
            this.btn_StopProcess.TabIndex = 57;
            this.btn_StopProcess.Text = "STOP";
            this.btn_StopProcess.UseVisualStyleBackColor = true;
            this.btn_StopProcess.Click += new System.EventHandler(this.btn_StopProcess_Click);
            // 
            // gb_MainHeader
            // 
            this.gb_MainHeader.Controls.Add(this.lbl_D_Albun);
            this.gb_MainHeader.Controls.Add(this.tb_RO_Title);
            this.gb_MainHeader.Controls.Add(this.tb_RO_Artist);
            this.gb_MainHeader.Controls.Add(this.lbl_D_Artist);
            this.gb_MainHeader.Controls.Add(this.tb_RO_Uploader);
            this.gb_MainHeader.Controls.Add(this.lbl_D_Genre);
            this.gb_MainHeader.Controls.Add(this.lbl__Uploader);
            this.gb_MainHeader.Controls.Add(this.lbl_D_Format);
            this.gb_MainHeader.Controls.Add(this.tb_RO_Genre);
            this.gb_MainHeader.Controls.Add(this.tb_RO_Format);
            this.gb_MainHeader.Location = new System.Drawing.Point(232, 60);
            this.gb_MainHeader.Name = "gb_MainHeader";
            this.gb_MainHeader.Size = new System.Drawing.Size(540, 105);
            this.gb_MainHeader.TabIndex = 58;
            this.gb_MainHeader.TabStop = false;
            // 
            // lbl_ImageInfo
            // 
            this.lbl_ImageInfo.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_ImageInfo.Location = new System.Drawing.Point(9, 269);
            this.lbl_ImageInfo.Name = "lbl_ImageInfo";
            this.lbl_ImageInfo.Size = new System.Drawing.Size(200, 23);
            this.lbl_ImageInfo.TabIndex = 59;
            // 
            // btn_About
            // 
            this.btn_About.Location = new System.Drawing.Point(507, 698);
            this.btn_About.Name = "btn_About";
            this.btn_About.Size = new System.Drawing.Size(75, 23);
            this.btn_About.TabIndex = 60;
            this.btn_About.Text = "About";
            this.btn_About.UseVisualStyleBackColor = true;
            this.btn_About.Click += new System.EventHandler(this.btn_About_Click);
            // 
            // prbr_Processing
            // 
            this.prbr_Processing.Location = new System.Drawing.Point(301, 322);
            this.prbr_Processing.Name = "prbr_Processing";
            this.prbr_Processing.Size = new System.Drawing.Size(471, 23);
            this.prbr_Processing.TabIndex = 61;
            // 
            // lbl_SelectedCount
            // 
            this.lbl_SelectedCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_SelectedCount.Location = new System.Drawing.Point(261, 322);
            this.lbl_SelectedCount.Name = "lbl_SelectedCount";
            this.lbl_SelectedCount.Size = new System.Drawing.Size(39, 23);
            this.lbl_SelectedCount.TabIndex = 62;
            // 
            // lbl_ProcessedCount
            // 
            this.lbl_ProcessedCount.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_ProcessedCount.Location = new System.Drawing.Point(233, 322);
            this.lbl_ProcessedCount.Name = "lbl_ProcessedCount";
            this.lbl_ProcessedCount.Size = new System.Drawing.Size(32, 23);
            this.lbl_ProcessedCount.TabIndex = 63;
            // 
            // btn_Back
            // 
            this.btn_Back.Enabled = false;
            this.btn_Back.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_Back.Location = new System.Drawing.Point(9, 8);
            this.btn_Back.Name = "btn_Back";
            this.btn_Back.Size = new System.Drawing.Size(49, 46);
            this.btn_Back.TabIndex = 64;
            this.btn_Back.Text = "Back";
            this.btn_Back.UseVisualStyleBackColor = true;
            this.btn_Back.Click += new System.EventHandler(this.btn_Back_Click);
            // 
            // frm_Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(784, 730);
            this.Controls.Add(this.btn_Back);
            this.Controls.Add(this.lbl_ProcessedCount);
            this.Controls.Add(this.lbl_SelectedCount);
            this.Controls.Add(this.prbr_Processing);
            this.Controls.Add(this.btn_About);
            this.Controls.Add(this.lbl_ImageInfo);
            this.Controls.Add(this.gb_MainHeader);
            this.Controls.Add(this.btn_StopProcess);
            this.Controls.Add(this.lbl_ProcStatusText);
            this.Controls.Add(this.pb_ProcStatusImage);
            this.Controls.Add(this.gb_SongHeader);
            this.Controls.Add(this.rtb_DownloadFailed);
            this.Controls.Add(this.lb_Log);
            this.Controls.Add(this.btn_Options);
            this.Controls.Add(this.btn_Exit);
            this.Controls.Add(this.gb_FooterButtons);
            this.Controls.Add(this.dgv_List);
            this.Controls.Add(this.gb_Album_Header);
            this.Controls.Add(this.btn_SaveImage);
            this.Controls.Add(this.pb_ItemImage);
            this.Controls.Add(this.lbl_InputURI);
            this.Controls.Add(this.lbl_InputFailed);
            this.Controls.Add(this.btn_Grab);
            this.Controls.Add(this.tb_InputURI);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.Name = "frm_Main";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Load += new System.EventHandler(this.frm_Main_Load);
            ((System.ComponentModel.ISupportInitialize)(this.pb_ItemImage)).EndInit();
            this.gb_Album_Header.ResumeLayout(false);
            this.gb_Album_Header.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.dgv_List)).EndInit();
            this.gb_FooterButtons.ResumeLayout(false);
            this.gb_SongHeader.ResumeLayout(false);
            this.gb_SongHeader.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pb_ProcStatusImage)).EndInit();
            this.gb_MainHeader.ResumeLayout(false);
            this.gb_MainHeader.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lbl_InputURI;
        private System.Windows.Forms.Label lbl_InputFailed;
        private System.Windows.Forms.Button btn_Grab;
        private System.Windows.Forms.TextBox tb_InputURI;
        private System.Windows.Forms.Button btn_SaveImage;
        private System.Windows.Forms.PictureBox pb_ItemImage;
        private System.Windows.Forms.TextBox tb_RO_Format;
        private System.Windows.Forms.TextBox tb_RO_Genre;
        private System.Windows.Forms.Label lbl_D_Format;
        private System.Windows.Forms.Label lbl__Uploader;
        private System.Windows.Forms.Label lbl_D_Genre;
        private System.Windows.Forms.TextBox tb_RO_Uploader;
        private System.Windows.Forms.Label lbl_D_Artist;
        private System.Windows.Forms.TextBox tb_RO_Artist;
        private System.Windows.Forms.TextBox tb_RO_Title;
        private System.Windows.Forms.Label lbl_D_Albun;
        private System.Windows.Forms.GroupBox gb_Album_Header;
        private System.Windows.Forms.RichTextBox tb_RO_Description_Album;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tb_RO_Count_Album;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox tb_RO_Updater_Album;
        private System.Windows.Forms.TextBox tb_RO_Type_Album;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox tb_RO_Date_Album;
        private System.Windows.Forms.DataGridView dgv_List;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Number;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Artist;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Title;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Length;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Size;
        private System.Windows.Forms.DataGridViewTextBoxColumn col_Bitrate;
        private System.Windows.Forms.DataGridViewCheckBoxColumn col_Download;
        private System.Windows.Forms.GroupBox gb_FooterButtons;
        private System.Windows.Forms.Button btn_DownloadSelected;
        private System.Windows.Forms.Button btn_SelectAll;
        private System.Windows.Forms.Button btn_DeselectAll;
        private System.Windows.Forms.Button btn_InverseSelected;
        private System.Windows.Forms.Button btn_Options;
        private System.Windows.Forms.Button btn_Exit;
        private System.Windows.Forms.ListBox lb_Log;
        private System.Windows.Forms.RichTextBox rtb_DownloadFailed;
        private System.Windows.Forms.GroupBox gb_SongHeader;
        private System.Windows.Forms.Button btn_GoToAlbumPage;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox tb_RO_AlbumURI;
        private System.Windows.Forms.PictureBox pb_ProcStatusImage;
        private System.Windows.Forms.Label lbl_ProcStatusText;
        private System.Windows.Forms.Button btn_StopProcess;
        private System.Windows.Forms.GroupBox gb_MainHeader;
        private System.Windows.Forms.Label lbl_ImageInfo;
        private System.Windows.Forms.Button btn_About;
        private System.Windows.Forms.ProgressBar prbr_Processing;
        private System.Windows.Forms.Label lbl_SelectedCount;
        private System.Windows.Forms.Label lbl_ProcessedCount;
        private System.Windows.Forms.Button btn_Back;
    }
}

