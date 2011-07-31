using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FrontFileFinagler
{
    public interface IPlaylistLoader
    {
        List<string> GetFilePaths(FileInfo playlistFileInfo);
    }
}
