using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FrontFileFinagler
{
    public class FileDetail
    {

        public string OriginalPath { get; private set; }
        public string FileName { get; private set; }


        public DateTime CreationTime 
        {
            get
            {
                return File.GetCreationTime(this.OriginalPath);
            }
            set
            {
                File.SetCreationTime(this.OriginalPath, value);
            }
        }


        public DateTime AccessTime
        {
            get
            {
                return File.GetLastAccessTime(this.OriginalPath);
            }
            set
            {
                File.SetLastAccessTime(this.OriginalPath, value);
            }
        }

        public DateTime WriteTime
        {
            get
            {
                return File.GetLastWriteTime(this.OriginalPath);
            }
            set
            {
                File.SetLastWriteTime(this.OriginalPath, value);
            }
        }

        
        public FileDetail(string filePath)
        {
            this.OriginalPath = filePath;
            this.FileName = filePath.Substring(filePath.LastIndexOf(@"\") + 1);
        }


        
    }
}
