
namespace DrukEtykietAdv
{
    public class Config
    {
        public string ConnectionString { get; set; }
        public Paths Paths { get; set; }
        public Printers Printers { get; set; }
    }

    public class Paths
    {
        public string DostawaCsv { get; set; }
        public string TowarEtykietyCsv { get; set; }
        public string TowarBezEtykietyCsv { get; set; }
        public string ZapisWydrukuTxt { get; set; }
        public string LabelPdf { get; set; }
    }

    public class Printers
    {
        public string LabelPrinter { get; set; }
        public string DefaultPrinter { get; set; }
        public string DefaultPrinter2 { get; set; }

    }

}
