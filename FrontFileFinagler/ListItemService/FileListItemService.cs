using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Configuration;
using System.Windows.Forms;
using System.IO;

namespace FrontFileFinagler
{
    public class FileListItemService
    {
        

        public static FileListItemServiceResult GetSortedFileListViewItems(IEnumerable<string> files)
        {
            FileListItemServiceResult result = new FileListItemServiceResult();
            
            List<FileDetail> fileDetails = new List<FileDetail>();
            
            foreach (string fileName in files)
            {
                if (File.Exists(fileName))
                {
                    FileDetail detail = new FileDetail(fileName);

                    fileDetails.Add(detail);
                }
                else
                {
                    result.ErrorFiles.Add(fileName);
                }
            }

            IEnumerable<FileDetail> sortedFileDetails = SortIncomingFiles(fileDetails);

            foreach (FileDetail fileDetail in sortedFileDetails)
            {
                ListViewItem item = new ListViewItem(fileDetail.FileName);
                item.Tag = fileDetail;

                result.Items.Add(item);
            }

            return result;
        }


        private static IEnumerable<FileDetail> SortIncomingFiles(List<FileDetail> fileDetails)
        {
            IEnumerable<FileDetail> sortedFileDetails;
            string sortBy = ConfigurationManager.AppSettings["sortby"];

            switch (sortBy)
            {
                case FileDateConstants.Created:
                    sortedFileDetails = fileDetails.OrderBy(detail => detail.CreationTime);
                    break;
                case FileDateConstants.Accessed:
                    sortedFileDetails = fileDetails.OrderBy(detail => detail.AccessTime);
                    break;
                default:
                    // By default, let's use Modified.
                    sortedFileDetails = fileDetails.OrderBy(detail => detail.WriteTime);
                    break;
            }
            return sortedFileDetails;
        }

    }
}
