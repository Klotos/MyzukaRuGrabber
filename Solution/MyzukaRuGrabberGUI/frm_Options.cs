using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using KlotosLib;

namespace MyzukaRuGrabberGUI
{
    /// <summary>
    /// Форма для меню настроек
    /// </summary>
    public partial class frm_Options : Form
    {
        /// <summary>
        /// Инициализирует содержимое формы
        /// </summary>
        public frm_Options()
        {
            InitializeComponent();

            this.tb_SaveFilePath.Text = ProgramSettings.Instance.SavedFilesPath;
            this.folderBrowserDialog1.SelectedPath = ProgramSettings.Instance.SavedFilesPath;
            this.tb_UserAgent.Text = ProgramSettings.Instance.UserAgent;
            this.rb_Internal.Checked = ProgramSettings.Instance.UseServerFilenames;
            this.rb_External.Checked = !ProgramSettings.Instance.UseServerFilenames;
            this.rb_CommonFolder.Checked = !ProgramSettings.Instance.UseDistinctFolder;
            this.rb_DistinctFolder.Checked = ProgramSettings.Instance.UseDistinctFolder;
            this.tb_MaxDownloadThreads.Text = ProgramSettings.Instance.MaxDownloadThreads.ToString(CultureInfo.InvariantCulture);
            this.tb_FilenameTemplate.Enabled = !ProgramSettings.Instance.UseServerFilenames;
            this.tb_FilenameTemplate.Text = ProgramSettings.Instance.FilenameTemplate;
        }
        
        private void btn_BrowseFilePath_Click(object sender, EventArgs e)
        {
            DialogResult dr = this.folderBrowserDialog1.ShowDialog();
            if (dr == DialogResult.OK)
            {
                String new_path = folderBrowserDialog1.SelectedPath;
                this.tb_SaveFilePath.Text = new_path;
            }
        }

        private Boolean TransactionalApply()
        {
            Dictionary<String, String> err = ProgramSettings.TransactionalApply
                (this.rb_DistinctFolder.Checked, this.rb_Internal.Checked, this.tb_MaxDownloadThreads.Text, 
                this.tb_SaveFilePath.Text, this.tb_UserAgent.Text, this.tb_FilenameTemplate.Text);
            if (err == null)
            {
                return true;
            }
            String message = String.Format("Some data are invalid: \r\n{0}.",
                err.ConcatToString(key => key, value => value, ": ", "; \r\n")
                );
            MessageBox.Show(message, "Error while saving data", MessageBoxButtons.OK);
            return false;
        }

        private void btn_OK_Click(object sender, EventArgs e)
        {
            Boolean result = this.TransactionalApply();
            this.Close();
        }

        private void btn_Cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void btn_Apply_Click(object sender, EventArgs e)
        {

            Boolean result = this.TransactionalApply();

        }

        private void btn_SetDefault_Click(object sender, EventArgs e)
        {
            this.tb_SaveFilePath.Text = ProgramSettings.Default.SavedFilesPath;
            this.tb_UserAgent.Text = ProgramSettings.Default.UserAgent;
            this.rb_CommonFolder.Checked = !ProgramSettings.Default.UseDistinctFolder;
            this.rb_DistinctFolder.Checked = ProgramSettings.Default.UseDistinctFolder;
            this.rb_Internal.Checked = ProgramSettings.Default.UseServerFilenames;
            this.rb_External.Checked = !ProgramSettings.Default.UseServerFilenames;
            this.tb_MaxDownloadThreads.Text = ProgramSettings.Default.MaxDownloadThreads.ToString(CultureInfo.InvariantCulture);
            this.tb_FilenameTemplate.Enabled = !ProgramSettings.Default.UseServerFilenames;
            this.tb_FilenameTemplate.Text = ProgramSettings.Default.FilenameTemplate;
        }

        private void frm_Options_Load(object sender, EventArgs e)
        {

        }

        private void rb_Internal_CheckedChanged(object sender, EventArgs e)
        {
            this.tb_FilenameTemplate.Enabled = !this.tb_FilenameTemplate.Enabled;
        }
    }
}
