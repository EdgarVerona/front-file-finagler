using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace FrontFileFinagler
{
    public class FileListItemServiceResult
    {
        public List<ListViewItem> Items { get; set; }
        public List<string> ErrorFiles { get; set; }


        public FileListItemServiceResult()
        {
            this.Items = new List<ListViewItem>();
            this.ErrorFiles = new List<string>();
        }

    }
}
