using System;

namespace Wdpr_Groep_E.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Text { get; set; }
        public DateTime Date { get; set; }

        public int ChatId { get; set; }
    }
}
