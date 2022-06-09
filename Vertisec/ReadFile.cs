using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Vertisec
{
    internal class ReadFile
    {
        private string _filePath;

        private bool validFileExtension(string filePath)
        {
            string fileType = Path.GetExtension(@filePath);
            return (fileType == ".sql" || fileType == ".txt");
        }

        private bool fileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        private void validateFile(string filePath)
        {
            if (!fileExists(@filePath))
                InternalErrorMessage.PrintError("File not found.");
            else if (!validFileExtension(@filePath))
                InternalErrorMessage.PrintError("Invalid file extension. Must be one of '.sql' or '.txt'");
        }

        public ReadFile(string filePath)
        {
            this._filePath = @filePath;
        }

        public string[] read(string filePath)
        {
            validateFile(filePath);
            return System.IO.File.ReadAllLines(@filePath);
        }

        public string[] read()
        {
            validateFile(this._filePath);
            return System.IO.File.ReadAllLines(this._filePath);
        }
    }
}
