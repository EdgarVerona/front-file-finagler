using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace FrontFileFinagler
{

    public class PlaylistLoadStrategy
    {

        public List<string> GetFilePaths(string playlistFilePath)
        {

            FileInfo info = new FileInfo(playlistFilePath);
            IPlaylistLoader loader;

            switch (info.Extension.ToLower())
            {
                case PlaylistConstants.FPL:
                    loader = new FPLPlaylistLoader();
                    break;
                default:
                    // M3U and M3U8 *should* be loadable in the same way, given MS's encoding abstractions... hopefully. =)
                    loader = new M3UPlaylistLoader();
                    break;
            }

            return loader.GetFilePaths(info);

        }

    }


}
