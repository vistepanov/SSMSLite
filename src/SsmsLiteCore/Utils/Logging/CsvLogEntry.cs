namespace SsmsLite.Core.Utils.Logging
{
    public class CsvLogEntry
    {
        //[Index(0)]
        //[Name("LocalDate               ")]
        public string LocalDate { get; set; }

        //[Index(1)]
        //[Name("Level         ")]
        public string Level { get; set; }

        //[Index(2)]
        //[Name("Category                                                    ")]
        public string Category { get; set; }

        //[Index(3)]
        public string Text { get; set; }

        //[Index(4)]
        //public string Properties { get; set; }
    }
}
