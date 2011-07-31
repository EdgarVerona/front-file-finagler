using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FrontFileControls;
using System.Windows.Forms;
using System.Collections;
using System.Drawing;

namespace FrontFileFinagler
{
    public class ReorderDragDropHandler : IDragDropListHandler
    {
        private const string REORDER = "Reorder";



        public DragDropResult HandleDragDrop(ListViewReorderable sender, DragEventArgs e)
        {
            if (!IsReorderEvent(e))
            {
                return DragDropResult.None;
            }

            if (!sender.AllowRowReorder)
            {
                return DragDropResult.Cancel;
            }

            if (sender.SelectedItems.Count == 0)
            {
                return DragDropResult.Cancel;
            }

            bool result = sender.ReorderItemsAtScreenPoint(new Point(e.X, e.Y));

            if (result)
            {
                return DragDropResult.Handled;
            }
            else
            {
                return DragDropResult.None;
            }
        }


        public DragDropResult HandleDragOver(ListViewReorderable sender, DragEventArgs e)
        {
            if (!IsReorderEvent(e))
            {
                return DragDropResult.None;
            }

            if (!sender.AllowRowReorder)
            {
                return DragDropResult.Cancel;
            }

            return DoHover(sender, e);
        }




        public DragDropResult HandleDragEnter(ListViewReorderable sender, DragEventArgs e)
        {
            if (!IsReorderEvent(e))
            {
                return DragDropResult.None;
            }

            if (!sender.AllowRowReorder)
            {
                return DragDropResult.Cancel;
            }

            return DoHover(sender, e);
        }

        public DragDropResult HandleDragItem(ListViewReorderable sender, ItemDragEventArgs e)
        {
            sender.DoDragDrop(REORDER, DragDropEffects.Move);

            return DragDropResult.Handled;
        }

        public DragDropResult HandleDragLeave(ListViewReorderable sender, EventArgs e)
        {
            return DragDropResult.None;
        }



        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------
        //-- PRIVATE METHODS
        //---------------------------------------------------------------------------------------------------
        //---------------------------------------------------------------------------------------------------



        private bool IsReorderEvent(DragEventArgs e)
        {
            if (!e.Data.GetDataPresent(DataFormats.Text))
            {
                return false;
            }

            String text = (String)e.Data.GetData(REORDER.GetType());
            return (text.Equals(REORDER));
        }



        private DragDropResult DoHover(ListViewReorderable sender, DragEventArgs e)
        {

            ListViewItem hoverItem = sender.GetListViewItemAtScreenPoint(new Point(e.X, e.Y));

            if (hoverItem == null)
            {
                return DragDropResult.None;
            }

            foreach (ListViewItem moveItem in sender.SelectedItems)
            {
                if (moveItem.Index == hoverItem.Index)
                {
                    e.Effect = DragDropEffects.None;
                    hoverItem.EnsureVisible();

                    return DragDropResult.Handled;
                }
            }

            e.Effect = DragDropEffects.Move;
            hoverItem.EnsureVisible();
            return DragDropResult.Handled;
        }

    }
}
