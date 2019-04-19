using NUnit.Framework;
using One_Sgp4;
using System.Linq;

namespace OneSgp4_Tests
{
    class FileParserTests
    {

        [Test]
        public void test2LineFile()
        {
            string testpath = TestContext.CurrentContext.TestDirectory + @"\sat42784_1.txt";
            var list = ParserTLE.ParseFile(testpath, "Pegasus");

            Assert.That(list.Count(), Is.EqualTo(3));
            var names = list.Select((t) => t.getName()).Distinct().ToList();

            Assert.That(names.Count, Is.EqualTo(1));
            Assert.That(names[0], Is.EqualTo("Pegasus"));
        }


        [Test]
        public void test3LineFile()
        {
            string testpath = TestContext.CurrentContext.TestDirectory + @"\somesats.txt";
            var list = ParserTLE.ParseFile(testpath);

            Assert.That(list.Count(), Is.EqualTo(3));
            var names = list.Select((t) => t.getName()).Distinct().ToList();

            Assert.That(names.Count, Is.EqualTo(3));
            Assert.That(names, Is.EquivalentTo(new string[] { "Sat123 and Something", "Anotherone", "TheLast" }));
        }

    }
}
