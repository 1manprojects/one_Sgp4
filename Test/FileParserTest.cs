using System;
using System.Linq;
using NUnit.Framework;
using One_Sgp4;

namespace Test
{
    class FileParserTest
    {
        [Test]
        public void test2LineFile()
        {
            string testpath = TestContext.CurrentContext.TestDirectory + @"\resources\sat42784_1.txt";
            var list = ParserTLE.ParseFile(testpath, "Pegasus");

            Assert.That(list.Count(), Is.EqualTo(3));
            var names = list.Select((t) => t.getName()).Distinct().ToList();

            Assert.That(names.Count, Is.EqualTo(1));
            Assert.That(names[0], Is.EqualTo("Pegasus"));
        }


        [Test]
        public void test3LineFile()
        {
            string testpath = TestContext.CurrentContext.TestDirectory + @"\resources\somesats.txt";
            var list = ParserTLE.ParseFile(testpath);

            Assert.That(list.Count(), Is.EqualTo(3));
            var names = list.Select((t) => t.getName()).Distinct().ToList();

            Assert.That(names.Count, Is.EqualTo(3));
            Assert.That(names, Is.EquivalentTo(new string[] { "Sat123 and Something", "Anotherone", "TheLast" }));
        }
    }
}
