using DataImport.Conversion;

namespace DataImport.UnitTests
{
    [TestFixture]
    public class StringDataConverterTests
    {
        [Test]
        public void CoreTest()
        {
            IDataConverter converter = DataConverterFactory.CreateStringImporter();

            string fileContents = TestData.TestData1.TestData1String;
            
            var records = converter.ImportData(fileContents, out var errors);
            
            Assert.Multiple(() =>
            {
                Assert.That(records, Is.Not.Empty);
                Assert.That(!errors.Any());
            });
        }
        
        [Test]
        public void ImportDataAsync_ValidFileContents_ReturnsExpectedRecords()
        {
            IDataConverter converter = DataConverterFactory.CreateStringImporter();

            const string fileContents = """
                                        Tyndall Centre grim file created on 22.01.2004 at 17:57 by Dr. Tim Mitchell
                                        .pre = precipitation (mm)
                                        CRU TS 2.1
                                        [Long=-180.00, 180.00] [Lati= -90.00,  90.00] [Grid X,Y= 720, 360]
                                        [Boxes=   67420] [Years=1991-2000] [Multi=    0.1000] [Missing=-999]
                                        Grid Ref= 1,2
                                        10 20 30 40 50 60 70 80 90 100 110 120
                                        Grid Ref= 3,4
                                        5 15 25 35 45 55 65 75 85 95 105 115
                                        """;

            var records = converter.ImportData(fileContents, out var errors);

            Assert.Multiple(() =>
            {
                Assert.That(!errors.Any());
                Assert.That(records, Has.Count.EqualTo(24)); // 2 grid refs * 12 months
                Assert.That(records.Any(r => r is { Xref: 1, Yref: 2, Date: { Year: 1991, Month: 1 }, Value: 10 }));
                Assert.That(records.Any(r => r is { Xref: 3, Yref: 4, Date: { Year: 1991, Month: 12 }, Value: 115 }));
            });
        }

        [Test]
        public void ImportDataAsync_InvalidValues_ReturnsErrors()
        {
            IDataConverter converter = DataConverterFactory.CreateStringImporter();
            
            const string fileContents = """
                                        Tyndall Centre grim file created on 22.01.2004 at 17:57 by Dr. Tim Mitchell
                                        .pre = precipitation (mm)
                                        CRU TS 2.1
                                        [Long=-180.00, 180.00] [Lati= -90.00,  90.00] [Grid X,Y= 720, 360]
                                        [Boxes=   67420] [Years=1991-2000] [Multi=    0.1000] [Missing=-999]
                                        Grid Ref= 1,2
                                        10 20 XX 40 50 60 70 80 90 100 110 120
                                        """;

            var records = converter.ImportData(fileContents, out IEnumerable<string> errors);

            Assert.Multiple(() =>
            {
                Assert.That(errors.Any());
                Assert.That(records, Has.Count.EqualTo(11));
            });
        }
        
        [Test]
        public void ImportDataAsync_EmptyFileContents_ReturnsEmptyRecords()
        {
            IDataConverter converter = DataConverterFactory.CreateStringImporter();

            const string fileContents = """
                                        Tyndall Centre grim file created on 22.01.2004 at 17:57 by Dr. Tim Mitchell
                                        .pre = precipitation (mm)
                                        CRU TS 2.1
                                        [Long=-180.00, 180.00] [Lati= -90.00,  90.00] [Grid X,Y= 720, 360]
                                        [Boxes=   67420] [Years=1991-2000] [Multi=    0.1000] [Missing=-999]
                                        """;

            var records = converter.ImportData(fileContents, out var errors);

            Assert.Multiple(() =>
            {
                Assert.That(!errors.Any());
                Assert.That(records, Is.Empty);
            });
        }
        
        [Test]
        public void ImportDataAsync_InvalidFileFormat_ThrowsFormatException()
        {
            IDataConverter converter = DataConverterFactory.CreateStringImporter();

            const string fileContents = "Invalid format data";

            Assert.Throws<System.FormatException>(() => converter.ImportData(fileContents, out _));
        }
        
        [Test]
        public void ImportDataAsync_MissingStartYear_ThrowsFormatException()
        {
            IDataConverter converter = DataConverterFactory.CreateStringImporter();

            const string fileContents = """
                                        Tyndall Centre grim file created on 22.01.2004 at 17:57 by Dr. Tim Mitchell
                                        .pre = precipitation (mm)
                                        CRU TS 2.1
                                        [Long=-180.00, 180.00] [Lati= -90.00,  90.00] [Grid X,Y= 720, 360]
                                        [Boxes=   67420][Multi=    0.1000] [Missing=-999]
                                        Grid Ref= 1,2
                                        10 20 30 40 50 60 70 80 90 100 110 120
                                        """;

            Assert.Throws<System.FormatException>(() => converter.ImportData(fileContents, out _));
        }
    }
}