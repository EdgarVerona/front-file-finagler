using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace FrontFileFinagler
{
    public partial class Form1 : Form
    {

        public Form1()
        {
            InitializeComponent();

            listFiles.DragDrop += new DragEventHandler(listFiles_DragDrop);
            listFiles.DragEnter += new DragEventHandler(listFiles_DragEnter);
            listFiles.KeyDown += new KeyEventHandler(listFiles_KeyDown);
        }

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

                List<FileDetail> fileDetails = new List<FileDetail>();

                for (int index = 0; index < files.Length; index++)
                {
                    string fileName = files[index];

                    FileDetail detail = new FileDetail(fileName);

                    fileDetails.Add(detail);
                }

                IEnumerable<FileDetail> sortedFileDetails = fileDetails.OrderBy(detail => detail.CreationTime);

                foreach (FileDetail fileDetail in sortedFileDetails)
                {
                    ListViewItem item = new ListViewItem(fileDetail.FileName);
                    item.Tag = fileDetail;

                    listFiles.Items.Add(item);
                }

            }   
        }

        private void btnChangeDates_Click(object sender, EventArgs e)
        {
            btnChangeDates.Enabled = false;
            listFiles.Enabled = false;
            DateTime timeIterator = DateTime.Now.AddMinutes(-listFiles.Items.Count);

            foreach (ListViewItem item in listFiles.Items)
            {
                FileDetail detail = (FileDetail)item.Tag;

                File.SetCreationTime(detail.OriginalPath, timeIterator);
                File.SetLastAccessTime(detail.OriginalPath, timeIterator);
                File.SetLastWriteTime(detail.OriginalPath, timeIterator);

                timeIterator = timeIterator.AddMinutes(1);
            }

            btnChangeDates.Enabled = true;
            listFiles.Enabled = true;
        }


    }
}
