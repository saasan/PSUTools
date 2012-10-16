using System;
using System.Collections;
using System.Windows.Forms;

namespace PSUTools
{
    public class ListViewItemComparer : IComparer
    {
        private int column;
        private SortOrder order;

        public ListViewItemComparer(int column, SortOrder order)
        {
            this.column = column;
            this.order = order;
        }

        public int Compare(object x, object y)
        {
            ListViewItem itemx = (ListViewItem)x;
            ListViewItem itemy = (ListViewItem)y;
            int result = String.Compare(itemx.SubItems[column].Text, itemy.SubItems[column].Text, true);

            if (order != SortOrder.Descending)
            {
                return result;
            }
            else
            {
                return -result;
            }
        }
    }
}
