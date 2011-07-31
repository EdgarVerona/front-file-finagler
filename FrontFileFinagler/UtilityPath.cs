using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FrontFileFinagler
{
    public class UtilityPath
    {


        public static string CreateFullPath(FileInfo playlistFileInfo, string trimmedLine)
        {
            if (!trimmedLine.Contains(@"\\") && !(trimmedLine.Contains(@":\")))
            {
                trimmedLine = string.Format(@"{0}\{1}", playlistFileInfo.Directory.FullName, trimmedLine);
            }
            return trimmedLine;
        }

    }
}
