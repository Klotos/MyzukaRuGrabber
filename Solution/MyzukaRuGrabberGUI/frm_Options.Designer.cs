namespace MyzukaRuGrabberGUI
{
    partial class frm_Options
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.gb_FilenamesSelector = new System.Windows.Forms.GroupBox();
            this.rb_External = new System.Windows.Forms.RadioButton();
            this.rb_Internal = new System.Windows.Forms.RadioButton();
            this.gb_AdvancedSettings = new System.Windows.Forms.GroupBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tb_MaxDownloadThreads = new System.Windows.Forms.TextBox();
            this.lbl_MaxDownloadThreads = new System.Windows.Forms.Label();
            this.tb_UserAgent = new System.Windows.Forms.TextBox();
            this.lbl_UserAgent = new System.Windows.Forms.Label();
            this.btn_Apply = new System.Windows.Forms.Button();
            this.btn_Cancel = new System.Windows.Forms.Button();
            this.btn_OK = new System.Windows.Forms.Button();
            this.btn_BrowseFilePath = new System.Windows.Forms.Button();
            this.tb_SaveFilePath = new System.Windows.Forms.TextBox();
            this.lbl_SavePath = new System.Windows.Forms.Label();
            this.folderBrowserDialog1 = new System.Windows.Forms.FolderBrowserDialog();
            this.gb_DistinctFolders = new System.Windows.Forms.GroupBox();
            this.rb_DistinctFolder = new System.Windows.Forms.RadioButton();
            this.rb_CommonFolder = new System.Windows.Forms.RadioButton();
            this.btn_SetDefault = new System.Windows.Forms.Button();
            this.lbl_FilenameTemplate = new System.Windows.Forms.Label();
            this.tb_FilenameTemplate = new System.Windows.Forms.TextBox();
            this.textBox2 = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.textBox4 = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.textBox5 = new System.Windows.Forms.TextBox();
            this.gb_FilenamesSelector.SuspendLayout();
            this.gb_AdvancedSettings.SuspendLayout();
            this.gb_DistinctFolders.SuspendLayout();
            this.SuspendLayout();
            // 
            // gb_FilenamesSelector
            // 
            this.gb_FilenamesSelector.Controls.Add(this.rb_External);
            this.gb_FilenamesSelector.Controls.Add(this.rb_Internal);
            this.gb_FilenamesSelector.Location = new System.Drawing.Point(16, 46);
            this.gb_FilenamesSelector.Name = "gb_FilenamesSelector";
            this.gb_FilenamesSelector.Size = new System.Drawing.Size(244, 78);
            this.gb_FilenamesSelector.TabIndex = 18;
            this.gb_FilenamesSelector.TabStop = false;
            this.gb_FilenamesSelector.Text = "filenames selector";
            // 
            // rb_External
            // 
            this.rb_External.AutoSize = true;
            this.rb_External.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rb_External.Location = new System.Drawing.Point(15, 54);
            this.rb_External.Name = "rb_External";
            this.rb_External.Size = new System.Drawing.Size(204, 20);
            this.rb_External.TabIndex = 1;
            this.rb_External.TabStop = true;
            this.rb_External.Text = "Use own generated filenames";
            this.rb_External.UseVisualStyleBackColor = true;
            // 
            // rb_Internal
            // 
            this.rb_Internal.AutoSize = true;
            this.rb_Internal.Checked = true;
            this.rb_Internal.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rb_Internal.Location = new System.Drawing.Point(15, 28);
            this.rb_Internal.Name = "rb_Internal";
            this.rb_Internal.Size = new System.Drawing.Size(214, 20);
            this.rb_Internal.TabIndex = 0;
            this.rb_Internal.TabStop = true;
            this.rb_Internal.Text = "Use server-delivered filenames";
            this.rb_Internal.UseVisualStyleBackColor = true;
            this.rb_Internal.CheckedChanged += new System.EventHandler(this.rb_Internal_CheckedChanged);
            // 
            // gb_AdvancedSettings
            // 
            this.gb_AdvancedSettings.Controls.Add(this.label1);
            this.gb_AdvancedSettings.Controls.Add(this.tb_MaxDownloadThreads);
            this.gb_AdvancedSettings.Controls.Add(this.lbl_MaxDownloadThreads);
            this.gb_AdvancedSettings.Controls.Add(this.tb_UserAgent);
            this.gb_AdvancedSettings.Controls.Add(this.lbl_UserAgent);
            this.gb_AdvancedSettings.Location = new System.Drawing.Point(16, 206);
            this.gb_AdvancedSettings.Name = "gb_AdvancedSettings";
            this.gb_AdvancedSettings.Size = new System.Drawing.Size(649, 124);
            this.gb_AdvancedSettings.TabIndex = 17;
            this.gb_AdvancedSettings.TabStop = false;
            this.gb_AdvancedSettings.Text = "Advanced settings - use them if you exactly understand what you are doing";
            // 
            // label1
            // 
            this.label1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label1.Location = new System.Drawing.Point(193, 62);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(391, 35);
            this.label1.TabIndex = 10;
            this.label1.Text = "Number of concurrent requests to server while downloading songs from album; \r\n0 -" +
    " unlimited/unspecified threads count; Max - 99";
            // 
            // tb_MaxDownloadThreads
            // 
            this.tb_MaxDownloadThreads.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_MaxDownloadThreads.Location = new System.Drawing.Point(605, 62);
            this.tb_MaxDownloadThreads.MaxLength = 2;
            this.tb_MaxDownloadThreads.Name = "tb_MaxDownloadThreads";
            this.tb_MaxDownloadThreads.Size = new System.Drawing.Size(38, 26);
            this.tb_MaxDownloadThreads.TabIndex = 9;
            // 
            // lbl_MaxDownloadThreads
            // 
            this.lbl_MaxDownloadThreads.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_MaxDownloadThreads.Location = new System.Drawing.Point(11, 65);
            this.lbl_MaxDownloadThreads.Name = "lbl_MaxDownloadThreads";
            this.lbl_MaxDownloadThreads.Size = new System.Drawing.Size(191, 23);
            this.lbl_MaxDownloadThreads.TabIndex = 8;
            this.lbl_MaxDownloadThreads.Text = "Max Download Threads";
            // 
            // tb_UserAgent
            // 
            this.tb_UserAgent.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_UserAgent.Location = new System.Drawing.Point(124, 27);
            this.tb_UserAgent.Name = "tb_UserAgent";
            this.tb_UserAgent.Size = new System.Drawing.Size(519, 26);
            this.tb_UserAgent.TabIndex = 7;
            // 
            // lbl_UserAgent
            // 
            this.lbl_UserAgent.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_UserAgent.Location = new System.Drawing.Point(11, 30);
            this.lbl_UserAgent.Name = "lbl_UserAgent";
            this.lbl_UserAgent.Size = new System.Drawing.Size(107, 23);
            this.lbl_UserAgent.TabIndex = 6;
            this.lbl_UserAgent.Text = "User-Agent";
            // 
            // btn_Apply
            // 
            this.btn_Apply.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_Apply.Location = new System.Drawing.Point(593, 352);
            this.btn_Apply.Name = "btn_Apply";
            this.btn_Apply.Size = new System.Drawing.Size(75, 28);
            this.btn_Apply.TabIndex = 16;
            this.btn_Apply.Text = "Apply";
            this.btn_Apply.UseVisualStyleBackColor = true;
            this.btn_Apply.Click += new System.EventHandler(this.btn_Apply_Click);
            // 
            // btn_Cancel
            // 
            this.btn_Cancel.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_Cancel.Location = new System.Drawing.Point(496, 352);
            this.btn_Cancel.Name = "btn_Cancel";
            this.btn_Cancel.Size = new System.Drawing.Size(75, 28);
            this.btn_Cancel.TabIndex = 15;
            this.btn_Cancel.Text = "Cancel";
            this.btn_Cancel.UseVisualStyleBackColor = true;
            this.btn_Cancel.Click += new System.EventHandler(this.btn_Cancel_Click);
            // 
            // btn_OK
            // 
            this.btn_OK.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_OK.Location = new System.Drawing.Point(405, 352);
            this.btn_OK.Name = "btn_OK";
            this.btn_OK.Size = new System.Drawing.Size(75, 28);
            this.btn_OK.TabIndex = 14;
            this.btn_OK.Text = "OK";
            this.btn_OK.UseVisualStyleBackColor = true;
            this.btn_OK.Click += new System.EventHandler(this.btn_OK_Click);
            // 
            // btn_BrowseFilePath
            // 
            this.btn_BrowseFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_BrowseFilePath.Location = new System.Drawing.Point(593, 12);
            this.btn_BrowseFilePath.Name = "btn_BrowseFilePath";
            this.btn_BrowseFilePath.Size = new System.Drawing.Size(75, 31);
            this.btn_BrowseFilePath.TabIndex = 13;
            this.btn_BrowseFilePath.Text = "Browse";
            this.btn_BrowseFilePath.UseVisualStyleBackColor = true;
            this.btn_BrowseFilePath.Click += new System.EventHandler(this.btn_BrowseFilePath_Click);
            // 
            // tb_SaveFilePath
            // 
            this.tb_SaveFilePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_SaveFilePath.Location = new System.Drawing.Point(106, 14);
            this.tb_SaveFilePath.Name = "tb_SaveFilePath";
            this.tb_SaveFilePath.ReadOnly = true;
            this.tb_SaveFilePath.Size = new System.Drawing.Size(481, 26);
            this.tb_SaveFilePath.TabIndex = 12;
            // 
            // lbl_SavePath
            // 
            this.lbl_SavePath.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_SavePath.Location = new System.Drawing.Point(12, 17);
            this.lbl_SavePath.Name = "lbl_SavePath";
            this.lbl_SavePath.Size = new System.Drawing.Size(88, 23);
            this.lbl_SavePath.TabIndex = 11;
            this.lbl_SavePath.Text = "Save path";
            // 
            // folderBrowserDialog1
            // 
            this.folderBrowserDialog1.Description = "Choose directory, where saved files will be placed";
            // 
            // gb_DistinctFolders
            // 
            this.gb_DistinctFolders.Controls.Add(this.rb_DistinctFolder);
            this.gb_DistinctFolders.Controls.Add(this.rb_CommonFolder);
            this.gb_DistinctFolders.Location = new System.Drawing.Point(292, 46);
            this.gb_DistinctFolders.Name = "gb_DistinctFolders";
            this.gb_DistinctFolders.Size = new System.Drawing.Size(373, 78);
            this.gb_DistinctFolders.TabIndex = 19;
            this.gb_DistinctFolders.TabStop = false;
            this.gb_DistinctFolders.Text = "new folder requirement";
            // 
            // rb_DistinctFolder
            // 
            this.rb_DistinctFolder.AutoSize = true;
            this.rb_DistinctFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rb_DistinctFolder.Location = new System.Drawing.Point(15, 54);
            this.rb_DistinctFolder.Name = "rb_DistinctFolder";
            this.rb_DistinctFolder.Size = new System.Drawing.Size(293, 20);
            this.rb_DistinctFolder.TabIndex = 1;
            this.rb_DistinctFolder.TabStop = true;
            this.rb_DistinctFolder.Text = "Create and use distinct folder for every album";
            this.rb_DistinctFolder.UseVisualStyleBackColor = true;
            // 
            // rb_CommonFolder
            // 
            this.rb_CommonFolder.AutoSize = true;
            this.rb_CommonFolder.Checked = true;
            this.rb_CommonFolder.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.rb_CommonFolder.Location = new System.Drawing.Point(15, 28);
            this.rb_CommonFolder.Name = "rb_CommonFolder";
            this.rb_CommonFolder.Size = new System.Drawing.Size(272, 20);
            this.rb_CommonFolder.TabIndex = 0;
            this.rb_CommonFolder.TabStop = true;
            this.rb_CommonFolder.Text = "Save all files from all albums to one folder";
            this.rb_CommonFolder.UseVisualStyleBackColor = true;
            // 
            // btn_SetDefault
            // 
            this.btn_SetDefault.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.btn_SetDefault.Location = new System.Drawing.Point(269, 352);
            this.btn_SetDefault.Name = "btn_SetDefault";
            this.btn_SetDefault.Size = new System.Drawing.Size(113, 28);
            this.btn_SetDefault.TabIndex = 20;
            this.btn_SetDefault.Text = "Set Default";
            this.btn_SetDefault.UseVisualStyleBackColor = true;
            this.btn_SetDefault.Click += new System.EventHandler(this.btn_SetDefault_Click);
            // 
            // lbl_FilenameTemplate
            // 
            this.lbl_FilenameTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.lbl_FilenameTemplate.Location = new System.Drawing.Point(16, 140);
            this.lbl_FilenameTemplate.Name = "lbl_FilenameTemplate";
            this.lbl_FilenameTemplate.Size = new System.Drawing.Size(145, 23);
            this.lbl_FilenameTemplate.TabIndex = 21;
            this.lbl_FilenameTemplate.Text = "Filename template";
            // 
            // tb_FilenameTemplate
            // 
            this.tb_FilenameTemplate.Font = new System.Drawing.Font("Microsoft Sans Serif", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.tb_FilenameTemplate.Location = new System.Drawing.Point(160, 137);
            this.tb_FilenameTemplate.Name = "tb_FilenameTemplate";
            this.tb_FilenameTemplate.Size = new System.Drawing.Size(505, 26);
            this.tb_FilenameTemplate.TabIndex = 22;
            // 
            // textBox2
            // 
            this.textBox2.Location = new System.Drawing.Point(16, 167);
            this.textBox2.Name = "textBox2";
            this.textBox2.ReadOnly = true;
            this.textBox2.Size = new System.Drawing.Size(64, 20);
            this.textBox2.TabIndex = 23;
            this.textBox2.Text = "%number%";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label2.Location = new System.Drawing.Point(82, 168);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(92, 16);
            this.label2.TabIndex = 24;
            this.label2.Text = "- track number";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label3.Location = new System.Drawing.Point(236, 168);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(67, 16);
            this.label3.TabIndex = 26;
            this.label3.Text = "- track title";
            // 
            // textBox3
            // 
            this.textBox3.Location = new System.Drawing.Point(183, 167);
            this.textBox3.Name = "textBox3";
            this.textBox3.ReadOnly = true;
            this.textBox3.Size = new System.Drawing.Size(52, 20);
            this.textBox3.TabIndex = 25;
            this.textBox3.Text = "%title%";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label4.Location = new System.Drawing.Point(368, 168);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(75, 16);
            this.label4.TabIndex = 28;
            this.label4.Text = "- track artist";
            // 
            // textBox4
            // 
            this.textBox4.Location = new System.Drawing.Point(314, 167);
            this.textBox4.Name = "textBox4";
            this.textBox4.ReadOnly = true;
            this.textBox4.Size = new System.Drawing.Size(52, 20);
            this.textBox4.TabIndex = 27;
            this.textBox4.Text = "%artist%";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(204)));
            this.label5.Location = new System.Drawing.Point(512, 168);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(89, 16);
            this.label5.TabIndex = 30;
            this.label5.Text = "- album name";
            // 
            // textBox5
            // 
            this.textBox5.Location = new System.Drawing.Point(458, 167);
            this.textBox5.Name = "textBox5";
            this.textBox5.ReadOnly = true;
            this.textBox5.Size = new System.Drawing.Size(52, 20);
            this.textBox5.TabIndex = 29;
            this.textBox5.Text = "%album%";
            // 
            // frm_Options
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(684, 400);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.textBox5);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.textBox4);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.textBox2);
            this.Controls.Add(this.tb_FilenameTemplate);
            this.Controls.Add(this.lbl_FilenameTemplate);
            this.Controls.Add(this.btn_SetDefault);
            this.Controls.Add(this.gb_DistinctFolders);
            this.Controls.Add(this.gb_FilenamesSelector);
            this.Controls.Add(this.gb_AdvancedSettings);
            this.Controls.Add(this.btn_Apply);
            this.Controls.Add(this.btn_Cancel);
            this.Controls.Add(this.btn_OK);
            this.Controls.Add(this.btn_BrowseFilePath);
            this.Controls.Add(this.tb_SaveFilePath);
            this.Controls.Add(this.lbl_SavePath);
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frm_Options";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Options";
            this.Load += new System.EventHandler(this.frm_Options_Load);
            this.gb_FilenamesSelector.ResumeLayout(false);
            this.gb_FilenamesSelector.PerformLayout();
            this.gb_AdvancedSettings.ResumeLayout(false);
            this.gb_AdvancedSettings.PerformLayout();
            this.gb_DistinctFolders.ResumeLayout(false);
            this.gb_DistinctFolders.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.GroupBox gb_FilenamesSelector;
        private System.Windows.Forms.RadioButton rb_External;
        private System.Windows.Forms.RadioButton rb_Internal;
        private System.Windows.Forms.GroupBox gb_AdvancedSettings;
        private System.Windows.Forms.TextBox tb_UserAgent;
        private System.Windows.Forms.Label lbl_UserAgent;
        private System.Windows.Forms.Button btn_Apply;
        private System.Windows.Forms.Button btn_Cancel;
        private System.Windows.Forms.Button btn_OK;
        private System.Windows.Forms.Button btn_BrowseFilePath;
        private System.Windows.Forms.TextBox tb_SaveFilePath;
        private System.Windows.Forms.Label lbl_SavePath;
        private System.Windows.Forms.FolderBrowserDialog folderBrowserDialog1;
        private System.Windows.Forms.GroupBox gb_DistinctFolders;
        private System.Windows.Forms.RadioButton rb_DistinctFolder;
        private System.Windows.Forms.RadioButton rb_CommonFolder;
        private System.Windows.Forms.Button btn_SetDefault;
        private System.Windows.Forms.TextBox tb_MaxDownloadThreads;
        private System.Windows.Forms.Label lbl_MaxDownloadThreads;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label lbl_FilenameTemplate;
        private System.Windows.Forms.TextBox tb_FilenameTemplate;
        private System.Windows.Forms.TextBox textBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox textBox4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox textBox5;
    }
}