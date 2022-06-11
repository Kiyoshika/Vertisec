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

        private bool ValidFileExtension(string filePath)
        {
            string fileType = Path.GetExtension(@filePath);
            return fileType == ".sql" || fileType == ".txt";
        }

        private bool FileExists(string filePath)
        {
            return File.Exists(filePath);
        }

        private void ValidateFile(string filePath)
        {
            if (!FileExists(@filePath))
                InternalErrorMessage.PrintError("File not found.");
            else if (!ValidFileExtension(@filePath))
                InternalErrorMessage.PrintError("Invalid file extension. Must be one of '.sql' or '.txt'");
        }

        public ReadFile(string filePath)
        {
            _filePath = @filePath;
        }

        public string[] Read(string filePath)
        {
            ValidateFile(filePath);
            return File.ReadAllLines(@filePath);
        }

        public string[] Read()
        {
            ValidateFile(_filePath);
            return File.ReadAllLines(_filePath);
        }
    }
}
