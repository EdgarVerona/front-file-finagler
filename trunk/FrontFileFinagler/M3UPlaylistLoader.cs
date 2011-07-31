using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FrontFileFinagler
{
    class M3UPlaylistLoader : IPlaylistLoader
    {
        public List<string> GetFilePaths(FileInfo playlistFileInfo)
        {
            List<string> results = new List<string>();

            using (StreamReader sr = new StreamReader(playlistFileInfo.FullName))
            {
                String line;
                
                while ((line = sr.ReadLine()) != null)
                {
                    results.Add( UtilityPath.CreateFullPath(playlistFileInfo, line));
                }
            }

            return results;
        }
    }
}
