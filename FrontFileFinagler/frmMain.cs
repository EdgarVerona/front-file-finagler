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

            listFiles.KeyDown += new KeyEventHandler(listFiles_KeyDown);

            FileDragDropHandler fileHandler = new FileDragDropHandler();
            listFiles.RegisterDragDropHandler(fileHandler);
            ReorderDragDropHandler reorderHandler = new ReorderDragDropHandler();
            listFiles.RegisterDragDropHandler(reorderHandler);
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

                FileListItemServiceResult fileItems = FileListItemService.GetSortedFileListViewItems(files);

                listFiles.AddItemsAtFirstSelected(fileItems.Items);
                ShowErrors(fileItems);

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

        private static void ShowErrors(FileListItemServiceResult result)
        {
            if (result.ErrorFiles.Count > 0)
            {
                StringBuilder error = new StringBuilder();
                error.Append("Some files could not be found, and failed to load:\n\n");
                foreach (string errorFile in result.ErrorFiles)
                {
                    error.AppendLine(errorFile);
                }

                MessageBox.Show(error.ToString());
            }
        }

    }
}
