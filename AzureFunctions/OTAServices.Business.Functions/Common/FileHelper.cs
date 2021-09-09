using System.IO;
using OTAServices.Business.Functions.Helpers;

namespace OTAServices.Business.Functions.Common
{
    public static class FileHelper
    {
        public static string GetFileNameWithSourcePath(string sourcePathFolder, string fileName)
        {
            string sourcePath = string.Concat(sourcePathFolder, fileName);
            return sourcePath;
        }

        public static string GetProcessedFileNameWithTimeStamp(string fileName, IDateTimeProvider dateTimeProvider)
        {
            string name = Path.GetFileNameWithoutExtension(fileName);
            string extension = Path.GetExtension(fileName);
            string processedFileNameWithTimeStamp = string.Concat(
                name,
                "_processedAt_",
                dateTimeProvider.GetCurrentDateTime().Year,
                "_",
                dateTimeProvider.GetCurrentDateTime().Month,
                "_",
                dateTimeProvider.GetCurrentDateTime().Day,
                "_",
                dateTimeProvider.GetCurrentDateTime().Hour,
                "_",
                dateTimeProvider.GetCurrentDateTime().Second,
                extension
                );
            return processedFileNameWithTimeStamp;
        }

        public static string GetFileNameWithDestinationPath(string destinationPathFolder, string fileName)
        {
            string destinationPath = string.Concat(destinationPathFolder,fileName);
            return destinationPath;
        }
    }
}
