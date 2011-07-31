using System;
using System.Drawing;
using System.Collections;
using System.Windows.Forms;
using System.Drawing.Drawing2D;
using System.Collections.Generic;
using System.Diagnostics;

namespace FrontFileControls
{
    public class ListViewReorderable : ListView
    {
        private bool allowRowReorder = true;
        private ListViewItem _lastHoveredItem = null;

        private List<IDragDropListHandler> _dragDropHandlers;


        private int _hoverIndex = -1;

        
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

        public ListViewReorderable()
            : base()
        {
            this.AllowRowReorder = true;
            this.DoubleBuffered = true;
            //+++ Set this to true when we want to give another shot at the "line indicator" feature.
            this.OwnerDraw = false;
            _dragDropHandlers = new List<IDragDropListHandler>();
            resetDraggingState();
        }


        public void RegisterDragDropHandler(IDragDropListHandler handler)
        {
            _dragDropHandlers.Add(handler);
        }

        public void UnregisterDragDropHandler(IDragDropListHandler handler)
        {
            if (_dragDropHandlers.Contains(handler))
            {
                _dragDropHandlers.Remove(handler);
            }
        }

        public int GetItemInsertIndexFromScreenPoint(Point screenPoint)
        {
            ListViewItem dragToItem = GetListViewItemAtScreenPoint(screenPoint);

            if (dragToItem == null)
            {
                return -1;
            }

            int dropIndex = dragToItem.Index;

            System.Drawing.Point cp = this.PointToClient(new System.Drawing.Point(screenPoint.X, screenPoint.Y));
            if ((dragToItem.Bounds.Location.Y + (dragToItem.Bounds.Height / 2)) <= cp.Y)
            {
                dropIndex++;
            }

            return dropIndex;
        }

        public ListViewItem GetListViewItemAtScreenPoint(Point screenPoint)
        {
            // Get the item we're currently dropping onto, if any.
            Point cp = this.PointToClient(new Point(screenPoint.X, screenPoint.Y));
            ListViewItem dragToItem = this.GetItemAt(cp.X, cp.Y);
            return dragToItem;
        }

        public bool ReorderItemsAtScreenPoint(Point screenPoint)
        {
            if (!this.AllowRowReorder)
            {
                return false;
            }

            int dropIndex = GetItemInsertIndexFromScreenPoint(screenPoint);

            ArrayList insertItems = new ArrayList(this.SelectedItems.Count);

            foreach (ListViewItem item in this.SelectedItems)
            {
                insertItems.Add(item.Clone());
            }

            for (int i = insertItems.Count - 1; i >= 0; i--)
            {
                ListViewItem insertItem = (ListViewItem)insertItems[i];
                this.Items.Insert(dropIndex, insertItem);
            }

            foreach (ListViewItem removeItem in this.SelectedItems)
            {
                this.Items.Remove(removeItem);
            }

            return true;
        }

        public bool AddItemsAtScreenPoint(List<ListViewItem> items, Point screenPoint)
        {
            int dropIndex = GetItemInsertIndexFromScreenPoint(screenPoint);

            if (dropIndex == -1)
            {
                foreach (ListViewItem item in items)
                {
                    this.Items.Add(item);                        
                }
            }
            else
            {
                foreach (ListViewItem item in items)
                {
                    // Insert after the selected item.
                    ListViewItem newItem = this.Items.Insert(dropIndex, item);
                    dropIndex++;
                }
            }

            return true;
        }

        public bool AddItemsAtFirstSelected(List<ListViewItem> items)
        {
            int dropIndex = -1;

            if (this.SelectedItems.Count > 0)
            {
                dropIndex = this.SelectedItems[0].Index + 1;
            }

            if (dropIndex == -1)
            {
                foreach (ListViewItem item in items)
                {
                    this.Items.Add(item);
                }
            }
            else
            {
                foreach (ListViewItem item in items)
                {
                    // Insert after the selected item.
                    ListViewItem newItem = this.Items.Insert(dropIndex, item);
                    dropIndex++;
                }
            }

            return true;
        }


        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------
        //-- DRAG AND DROP EVENT HANDLERS
        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------





        protected override void OnDragDrop(DragEventArgs e)
        {
            base.OnDragDrop(e);

            foreach (IDragDropListHandler handler in _dragDropHandlers)
            {
                DragDropResult handlerResult = handler.HandleDragDrop(this, e);

                if (handlerResult == DragDropResult.Cancel)
                {
                    resetDraggingState();
                    return;
                }
            }

            resetDraggingState();
        }

        protected override void OnDragOver(DragEventArgs e)
        {
            base.OnDragOver(e);

            if (this.OwnerDraw)
            {
                if (this.GetItemInsertIndexFromScreenPoint(new Point(e.X, e.Y)) != _hoverIndex)
                {
                    _hoverIndex = this.GetItemInsertIndexFromScreenPoint(new Point(e.X, e.Y));
                    this.RedrawItems(Math.Max(0, _hoverIndex - 1), Math.Min(this.Items.Count - 1, _hoverIndex + 1), false);
                }
            }
            

            foreach (IDragDropListHandler handler in _dragDropHandlers)
            {
                DragDropResult handlerResult = handler.HandleDragOver(this, e);

                if (handlerResult == DragDropResult.Cancel)
                {
                    resetDraggingState();
                    return;
                }
                if (handlerResult == DragDropResult.Handled)
                {
                    break;
                }
            }

            ListViewItem hoverItem = GetHoveredItem(e);

            if (hoverItem == null)
            {
                resetDraggingState();
                return;
            }
            else
            {
                if (hoverItem != _lastHoveredItem)
                {
                    resetItemColors();
                    setCurrentItem(hoverItem);
                }
            }

        }

        
        protected override void OnDragEnter(DragEventArgs e)
        {
            base.OnDragEnter(e);

            foreach (IDragDropListHandler handler in _dragDropHandlers)
            {
                DragDropResult handlerResult = handler.HandleDragEnter(this, e);

                if (handlerResult == DragDropResult.Cancel)
                {
                    resetDraggingState();
                    return;
                }
                if (handlerResult == DragDropResult.Handled)
                {
                    break;
                }
            }
        }



        protected override void OnItemDrag(ItemDragEventArgs e)
        {
            base.OnItemDrag(e);

            // ItemDrag is strictly for list items: if we don't allow reordering of this nature, stop now.
            if (!this.AllowRowReorder)
            {
                return;
            }

            foreach (IDragDropListHandler handler in _dragDropHandlers)
            {
                DragDropResult handlerResult = handler.HandleDragItem(this, e);

                if (handlerResult == DragDropResult.Cancel)
                {
                    resetDraggingState();
                    return;
                }
                if (handlerResult == DragDropResult.Handled)
                {
                    break;
                }
            }
        }

        protected override void OnDragLeave(EventArgs e)
        {
            base.OnDragLeave(e);

            foreach (IDragDropListHandler handler in _dragDropHandlers)
            {
                DragDropResult handlerResult = handler.HandleDragLeave(this, e);

                if (handlerResult == DragDropResult.Cancel)
                {
                    resetDraggingState();
                    return;
                }
                if (handlerResult == DragDropResult.Handled)
                {
                    break;
                }
            }
        }


        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------
        //-- DRAWING OVERRIDES
        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------


        protected override void OnDrawColumnHeader(DrawListViewColumnHeaderEventArgs e)
        {
            base.OnDrawColumnHeader(e);

            e.DrawDefault = true;
        }


        protected override void OnDrawItem(DrawListViewItemEventArgs e)
        {
            base.OnDrawItem(e);

            if (_hoverIndex > 0)
            {
                e.DrawBackground();
                e.DrawText();
                e.DrawFocusRectangle();

                Pen pen = new Pen(Color.Black);

                if (_hoverIndex >= this.Items.Count)
                {
                    Rectangle rect = this.GetItemRect(_hoverIndex - 1);
                    e.Graphics.DrawLine(pen, rect.Left, rect.Bottom + 1, rect.Right, rect.Bottom + 1);
                }
                else
                {
                    Rectangle rect = this.GetItemRect(_hoverIndex);
                    e.Graphics.DrawLine(pen, rect.Left, rect.Top - 1, rect.Right, rect.Top - 1);
                }
            }
            else
            {
                e.DrawBackground();
                e.DrawText();
                e.DrawFocusRectangle();
            }
            
        }

        protected override void OnDrawSubItem(DrawListViewSubItemEventArgs e)
        {
            base.OnDrawSubItem(e);
        }


        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------
        //-- PRIVATE METHODS
        //-------------------------------------------------------------------------------------------
        //-------------------------------------------------------------------------------------------


        private ListViewItem GetHoveredItem(DragEventArgs e)
        {
            Point cp = base.PointToClient(new Point(e.X, e.Y));
            ListViewItem hoverItem = base.GetItemAt(cp.X, cp.Y);
            return hoverItem;
        }


        private void resetDraggingState()
        {
            resetItemColors();
            _hoverIndex = -1;
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