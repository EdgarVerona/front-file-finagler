using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Drawing.Drawing2D;

namespace FrontFileFinagler
{
    public class ListViewRecordable : ListView
    {
        private const string REORDER = "Reorder";

        private bool allowRowReorder = true;
        private bool _draggingItem = false;
        private ListViewItem _lastHoveredItem = null;
        private Point _lastPoint;

        public bool AllowRowReorder
        {
            get
            {
                return this.allowRowReorder;
            }
            set
            {
                this.allowRowReorder = value;
                base.AllowDrop = value;
            }
        }

        public new SortOrder Sorting
        {
            get
            {
                return SortOrder.None;
            }
            set
            {
                base.Sorting = SortOrder.None;
            }
        }

        public ListViewRecordable()
            : base()
        {
            this.AllowRowReorder = true;
            this.DoubleBuffered = true;
            resetDraggingState();
        }

        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------
        //-- DRAG AND DROP EVENT HANDLERS
        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------



        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            if (!_draggingItem)
            {
                return;
            }
            resetDraggingState();

            if (!this.AllowRowReorder)
            {
                return;
            }
            if (base.SelectedItems.Count == 0)
            {
                return;
            }
            Point cp = base.PointToClient(new Point(e.X, e.Y));
            _lastPoint = cp;
            ListViewItem dragToItem = base.GetItemAt(cp.X, cp.Y);
            if (dragToItem == null)
            {
                return;
            }
            int dropIndex = dragToItem.Index;
            if (dropIndex > base.SelectedItems[0].Index)
            {
                dropIndex++;
            }
            ArrayList insertItems =
            new ArrayList(base.SelectedItems.Count);
            foreach (ListViewItem item in base.SelectedItems)
            {
                insertItems.Add(item.Clone());
            }
            for (int i = insertItems.Count - 1; i >= 0; i--)
            {
                ListViewItem insertItem =
                (ListViewItem)insertItems[i];
                base.Items.Insert(dropIndex, insertItem);
            }
            foreach (ListViewItem removeItem in base.SelectedItems)
            {
                base.Items.Remove(removeItem);
            }
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            if (!this.AllowRowReorder)
            {
                e.Effect = DragDropEffects.None;
                resetDraggingState();
                return;
            }
            if (!e.Data.GetDataPresent(DataFormats.Text))
            {
                resetDraggingState();
                return;
            }
            Point cp = base.PointToClient(new Point(e.X, e.Y));
            ListViewItem hoverItem = base.GetItemAt(cp.X, cp.Y);
            if (hoverItem == null)
            {
                e.Effect = DragDropEffects.None;
                resetDraggingState();
                return;
            }
            foreach (ListViewItem moveItem in base.SelectedItems)
            {
                if (moveItem.Index == hoverItem.Index)
                {
                    e.Effect = DragDropEffects.None;
                    
                    hoverItem.EnsureVisible();
                    resetDraggingState();
                    return;
                }
            }
            
            String text = (String)e.Data.GetData(REORDER.GetType());
            if (text.CompareTo(REORDER) == 0)
            {
                e.Effect = DragDropEffects.Move;
                if (hoverItem != _lastHoveredItem)
                {
                    resetItemColors();
                    setCurrentItem(hoverItem);
                }

                hoverItem.EnsureVisible();
                _draggingItem = true;
            }
            else
            {
                e.Effect = DragDropEffects.None;
                resetDraggingState();
            }
        }

        
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            if (!this.AllowRowReorder)
            {
                e.Effect = DragDropEffects.None;
                resetDraggingState();
                return;
            }
            if (!e.Data.GetDataPresent(DataFormats.Text))
            {
                resetDraggingState();
                return;
            }
            
            String text = (String)e.Data.GetData(REORDER.GetType());
            if (text.CompareTo(REORDER) == 0)
            {
                e.Effect = DragDropEffects.Move;
                _draggingItem = true;
            }
            else
            {
                e.Effect = DragDropEffects.None;
                resetDraggingState();
            }
        }

        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            base.OnItemDrag(e);
            if (!this.AllowRowReorder)
            {
                return;
            }
            base.DoDragDrop(REORDER, DragDropEffects.Move);
        }



        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------
        //-- PRIVATE METHODS
        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------


        private void resetDraggingState()
        {
            resetItemColors();
            _draggingItem = false;
        }

        private void resetItemColors()
        {
            if (_lastHoveredItem != null)
            {
                _lastHoveredItem.BackColor = this.BackColor;
                _lastHoveredItem = null;
            }
        }

        private void setCurrentItem(ListViewItem item)
        {
            item.BackColor = Color.Aqua;
            _lastHoveredItem = item;
        }


    }
}