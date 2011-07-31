using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FrontFileFinagler
{
    class FPLPlaylistLoader : IPlaylistLoader
    {
        
        public List<string> GetFilePaths(FileInfo playlistFileInfo)
        {
            List<string> results = new List<string>();

            char NullCharacter = Convert.ToChar(0x0);

            string file;

            using (StreamReader reader = new StreamReader(playlistFileInfo.FullName))
            {
                file = reader.ReadToEnd();
            }

            string[] lines = file.Split(NullCharacter);

            IEnumerable<string> fileLines = lines.Where(line => line.StartsWith("file://"));

            foreach (string line in fileLines)
            {
                string trimmedLine = line.Replace("file://", "");

                trimmedLine = UtilityPath.CreateFullPath(playlistFileInfo, trimmedLine);

                results.Add(trimmedLine);
            }

            return results;
        }

    }
}
