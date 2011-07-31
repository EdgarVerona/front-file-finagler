using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrontFileControls
{
    public interface IDragDropListHandler
    {
        DragDropResult HandleDragDrop(ListViewReorderable sender, DragEventArgs e);
        DragDropResult HandleDragOver(ListViewReorderable sender, DragEventArgs e);
        DragDropResult HandleDragEnter(ListViewReorderable sender, DragEventArgs e);
        DragDropResult HandleDragItem(ListViewReorderable sender, ItemDragEventArgs e);
        DragDropResult HandleDragLeave(ListViewReorderable sender, EventArgs e);
    }
}
