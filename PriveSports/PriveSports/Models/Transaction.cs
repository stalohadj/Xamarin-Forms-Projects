using System;
namespace PriveSports.Models
{
    
        public class Transaction
        {
            public string Details { get; set; }
            public bool Isvisible { get; set; }
            public string Date { get; set; }
            public string refer { get; set; }
            public string[] passthrough { get; set; }
            public string ItemDet { get; set; }

        }
    
}
