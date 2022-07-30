using System;
namespace ESAOK.Models
{
    public class EventModel
    {
        public string Name { get; set; }
        public string Details { get; set; }

        internal void Add(EventModel eventModel)
        {
            throw new NotImplementedException();
        }
    }
}
