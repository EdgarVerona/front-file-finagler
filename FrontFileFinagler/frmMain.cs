using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Configuration;

namespace FrontFileFinagler
{
    public partial class frmMain : Form
    {

        public frmMain()
        {
            InitializeComponent();

            listFiles.DragDrop += new DragEventHandler(listFiles_DragDrop);
            listFiles.DragEnter += new DragEventHandler(listFiles_DragEnter);
            listFiles.KeyDown += new KeyEventHandler(listFiles_KeyDown);
        }


        //--------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------
        //-- OVERRIDES FOR LIST BEHAVIOR
        //--------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------


        void listFiles_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Delete || e.KeyCode == Keys.Back)
            {
                ListView.SelectedListViewItemCollection selected = listFiles.SelectedItems;
                foreach (ListViewItem item in selected)
                {
                    listFiles.Items.Remove(item);
                }
            }
        }

        void listFiles_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
                e.Effect = DragDropEffects.Copy;
            else
                e.Effect = DragDropEffects.None;
        }

        void listFiles_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                LoadFiles(files);

            }   
        }


        //--------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------
        //-- BUTTON EVENT HANDLERS
        //--------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------


        private void btnChangeDates_Click(object sender, EventArgs e)
        {
            btnChangeDates.Enabled = false;
            listFiles.Enabled = false;
            DateTime timeIterator = DateTime.Now.AddMinutes(-listFiles.Items.Count);

            foreach (ListViewItem item in listFiles.Items)
            {
                FileDetail detail = (FileDetail)item.Tag;

                UpdateDate(timeIterator, detail);

                timeIterator = timeIterator.AddMinutes(1);
            }

            btnChangeDates.Enabled = true;
            listFiles.Enabled = true;
        }


        //--------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------
        //-- MENU ITEM EVENT HANDLERS
        //--------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------



        private void importPlaylistToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new OpenFileDialog();

            dialog.Filter = "";
            dialog.CheckFileExists = true;
            dialog.SupportMultiDottedExtensions = true;

            DialogResult result = dialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                string file = dialog.FileName;

                PlaylistLoadStrategy loader = new PlaylistLoadStrategy();

                List<string> files = loader.GetFilePaths(file);

                LoadFiles(files);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            this.Dispose();
        }



        //--------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------
        //-- PRIVATE METHODS
        //--------------------------------------------------------------------------------------
        //--------------------------------------------------------------------------------------

        private static void UpdateDate(DateTime timeIterator, FileDetail detail)
        {
            string sortBy = ConfigurationManager.AppSettings["update"];

            if (string.IsNullOrEmpty(sortBy))
            {
                sortBy = FileDateConstants.Modified;
            }
            string[] sortByList = sortBy.Split('|');

            foreach (string sortByItem in sortByList)
            {
                switch (sortByItem)
                {
                    case FileDateConstants.Accessed:
                        File.SetLastAccessTime(detail.OriginalPath, timeIterator);
                        break;
                    case FileDateConstants.Created:
                        File.SetCreationTime(detail.OriginalPath, timeIterator);
                        break;
                    default:
                        // Again, by default choose Modified.
                        File.SetLastWriteTime(detail.OriginalPath, timeIterator);
                        break;
                }
            }
        }



        private static IEnumerable<FileDetail> SortIncomingFiles(List<FileDetail> fileDetails)
        {
            IEnumerable<FileDetail> sortedFileDetails;
            string sortBy = ConfigurationManager.AppSettings["sortby"];

            switch (sortBy)
            {
                case FileDateConstants.Created:
                    sortedFileDetails = fileDetails.OrderBy(detail => detail.CreationTime);
                    break;
                case FileDateConstants.Accessed:
                    sortedFileDetails = fileDetails.OrderBy(detail => detail.AccessTime);
                    break;
                default:
                    // By default, let's use Modified.
                    sortedFileDetails = fileDetails.OrderBy(detail => detail.WriteTime);
                    break;
            }
            return sortedFileDetails;
        }


        private void LoadFiles(IEnumerable<string> files)
        {
            List<FileDetail> fileDetails = new List<FileDetail>();
            List<string> errorFiles = new List<string>();

            foreach (string fileName in files)
            {
                if (File.Exists(fileName))
                {
                    FileDetail detail = new FileDetail(fileName);

                    fileDetails.Add(detail);
                }
                else
                {
                    errorFiles.Add(fileName);
                }
            }

            IEnumerable<FileDetail> sortedFileDetails = SortIncomingFiles(fileDetails);

            foreach (FileDetail fileDetail in sortedFileDetails)
            {
                ListViewItem item = new ListViewItem(fileDetail.FileName);
                item.Tag = fileDetail;

                listFiles.Items.Add(item);
            }

            if (errorFiles.Count > 0)
            {
                StringBuilder error = new StringBuilder();
                error.Append("Some files could not be found, and failed to load:\n\n");
                foreach(string errorFile in errorFiles)
                {
                    error.AppendLine(errorFile);
                }
                MessageBox.Show(error.ToString());
            }

        }


    }
}
