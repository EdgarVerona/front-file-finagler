using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrontFileControls;
using System.Windows.Forms;
using System.Drawing;

namespace FrontFileFinagler
{

    public class FileDragDropHandler : IDragDropListHandler
    {



        public DragDropResult HandleDragDrop(ListViewReorderable sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);

                FileListItemServiceResult fileListItems = FileListItemService.GetSortedFileListViewItems(files);

                sender.AddItemsAtScreenPoint(fileListItems.Items, new Point(e.X, e.Y));

                return DragDropResult.Handled;
            }

            return DragDropResult.None;
        }


        public DragDropResult HandleDragOver(ListViewReorderable sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                return DragDropResult.Handled;
            }

            e.Effect = DragDropEffects.None;
            return DragDropResult.None;
        }

        public DragDropResult HandleDragEnter(ListViewReorderable sender, System.Windows.Forms.DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effect = DragDropEffects.Copy;
                return DragDropResult.Handled;
            }

            e.Effect = DragDropEffects.None;
            return DragDropResult.None;
        }

        public DragDropResult HandleDragItem(ListViewReorderable sender, System.Windows.Forms.ItemDragEventArgs e)
        {
            return DragDropResult.None;
        }

        public DragDropResult HandleDragLeave(ListViewReorderable sender, EventArgs e)
        {
            return DragDropResult.None;
        }
    }

}
