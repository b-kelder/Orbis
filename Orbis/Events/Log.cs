using System;

namespace Orbis.Events
{
    class Log
    {
        public string Item { get; set; }
        public string Type { get; set; }
        public string Timestamp { get; set; }

        public Log(string item, string type)
        {
            Item        = item;
            Type        = type;
            Timestamp   = DateTime.Now.ToString();
        }

        public override string ToString()
        {
            return Timestamp + ":" + " (" + Type + ") > " + Item;
        }
    }
}
