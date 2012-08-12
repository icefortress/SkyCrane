using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SkyCrane.NetCode
{
    public static class NetTest
    {
        //Netcode testing suite... or just a template
        public static void Main(string[] args)
        {
            RawClient c = new RawClient();
            RawServer s = new RawServer(9999);
            //while (true)
            //{
            //    if (s.getCMD().Count > 0)
            //    {
            //        foreach (Command c in s.getCMD())
            //        {
            //            Console.WriteLine(c.ct);
            //            Console.WriteLine(c.entity_id);
            //            Console.WriteLine(c.direction.X);
            //            Console.WriteLine(c.direction.Y);
            //            Console.WriteLine(c.position.X);
            //            Console.WriteLine(c.position.Y);
            //            Console.WriteLine("===========");
            //        }
            //    }
            //    List<StateChange> l = new List<StateChange>();
            //    StateChange st = new StateChange();
            //    st.type = StateChangeType.DELETE_ENTITY;
            //    st.intProperties[StateProperties.FRAME_WIDTH] = 123;
            //    st.stringProperties[StateProperties.SPRITE_NAME] = "I'm the Baconator";
            //    l.Add(st);
            //    s.broadcastSC(l);
            //    Thread.Sleep(2000);
            //}
            c.connect("192.168.0.28", 9999);

            while (true)
            {
                Thread.Sleep(5000);
                Console.WriteLine(c.getPing());
            }
        }

        //public void exit()
        //{
        //    s.exit();
        //    c.exit();
        //}
    }
}
