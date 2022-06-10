using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Vertisec.Util;

namespace Vertisec.FileIO
{
    internal class ReadFile
    {
        private string _filePath;

        private bool validFileExtension(string filePath)
        {
            string fileType = Path.GetExtension(@filePath);
            return fileType == ".sql" || fileType == ".txt";
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
            _filePath = @filePath;
        }

        public string[] read(string filePath)
        {
            validateFile(filePath);
            return File.ReadAllLines(@filePath);
        }

        public string[] read()
        {
            validateFile(_filePath);
            return File.ReadAllLines(_filePath);
        }
    }
}
