using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Vertisec.FileIO;

namespace VertisecTests.FileIO
{
    [TestClass]
    public class ReadFileTest : ReadFile
    {
        public ReadFileTest(string filePath) : base(filePath)
        {
        }

        public ReadFileTest() : base()
        {
        }

        [TestMethod]
        public void ValidFileExtension()
        {
            ReadFileTest file = new ReadFileTest();
            Assert.IsTrue(file.ValidFileExtension("file.txt"));
            Assert.IsTrue(file.ValidFileExtension("file.sql"));
            Assert.IsFalse(file.ValidFileExtension("file.ext"));
            Assert.IsFalse(file.ValidFileExtension("file"));
        }

        [TestMethod]
        [DeploymentItem("sample.txt")]
        public void ReadTxt()
        {
            ReadFile txtFile = new ReadFile("sample.txt");
            string[] lines = txtFile.Read();
            Assert.AreEqual(lines[0], "select");
            Assert.AreEqual(lines[1], "\twh_id,");
            Assert.AreEqual(lines[2], "\tlocation_id");
            Assert.AreEqual(lines[3], "from");
            Assert.AreEqual(lines[4], "\ttable");

            lines = txtFile.Read("sample.txt");
            Assert.AreEqual(lines[0], "select");
            Assert.AreEqual(lines[1], "\twh_id,");
            Assert.AreEqual(lines[2], "\tlocation_id");
            Assert.AreEqual(lines[3], "from");
            Assert.AreEqual(lines[4], "\ttable");
        }

        [TestMethod]
        [DeploymentItem("sample.sql")]
        public void ReadSQL()
        {
            ReadFile readSQL = new ReadFile("sample.sql");
            string[] lines = readSQL.Read();
            Assert.AreEqual(lines[0], "select");
            Assert.AreEqual(lines[1], "\twh_id,");
            Assert.AreEqual(lines[2], "\tlocation_id");
            Assert.AreEqual(lines[3], "from");
            Assert.AreEqual(lines[4], "\ttable");

            lines = readSQL.Read("sample.sql");
            Assert.AreEqual(lines[0], "select");
            Assert.AreEqual(lines[1], "\twh_id,");
            Assert.AreEqual(lines[2], "\tlocation_id");
            Assert.AreEqual(lines[3], "from");
            Assert.AreEqual(lines[4], "\ttable");
        }

        [TestMethod]
        [DeploymentItem("sample.ext")]
        public void ReadNonvalidExtension()
        {
            ReadFile readExt = new ReadFile("sample.ext");
            Assert.ThrowsException<FileNotFoundException>(() => readExt.Read());

        }

        [TestMethod]
        public void ReadNonExistentFile()
        {
            ReadFile nonExistent = new ReadFile("nonexistent");
            Assert.ThrowsException<FileNotFoundException>(() => nonExistent.Read());
        }
    }
}
