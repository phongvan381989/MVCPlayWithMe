using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.OpenPlatform.Model.TikiApp.Event
{
    public class Event_Response
    {
        public List<TikiEvent> events { get; set; }

        public string ack_id { get; set; }
    }
}