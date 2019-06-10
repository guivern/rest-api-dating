using System;

namespace RestApiDating.Models
{
    public class Value
    {
        public int Id {get; set;}
        public string Name { get; set; }
        public DateTime Date {get; set;} = DateTime.Now;
    }
}