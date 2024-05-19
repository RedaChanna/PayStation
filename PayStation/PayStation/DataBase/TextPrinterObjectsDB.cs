using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.IO.Ports;

namespace PayStationSW.DataBase
{
    public class TextPrinterObjectsDB
    {
        [Key]
        public int Id { get; set; }

        [Column("Name")]
        public string? Name { get; set; }

        [Column("Type")]
        public int Type { get; set; }

        [Column("Text")]
        public string? Text { get; set; }

        [Column("Highlighting")]
        public bool Highlighting { get; set; }

        [Column("TypeFont")]
        public int TypeFont { get; set; }

        [Column("SizeWidth")]
        public int SizeWidth { get; set; }

        [Column("SizeHeight")]
        public int SizeHeight { get; set; }

        [Column("Underline")]
        public bool Underline { get; set; }
        
        [Column("Bold")]
        public bool Bold { get; set; }

        [Column("Overlapping")]
        public bool Overlapping { get; set; }

        [Column("UpsideDown")]
        public bool UpsideDown { get; set; }

        [Column("Revolving")]
        public bool Revolving { get; set; }

        [Column("LeftMargin")]
        public int LeftMargin { get; set; }
        
        [Column("Alligment")]
        public int Alligment { get; set; }

        [Column("DistanceBeforeObj")]
        public int DistanceBeforeObj { get; set; }

        [Column("DistanceAfterObj")]
        public int DistanceAfterObj { get; set; }

    }
}