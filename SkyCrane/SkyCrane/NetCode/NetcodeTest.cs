using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace SkyCrane.NetCode
{
    public static class NetTest
    {
        public static RawClient c = new RawClient();
        public static RawClient c1 = new RawClient();
        public static RawClient c2 = new RawClient();
        public static RawClient c3 = new RawClient();
        public static RawServer s = new RawServer(9999);
        //Netcode testing suite... or just a template
        public static void Main(string[] args)
        {
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
            c.connect("127.0.0.1", 9999);
            c1.connect("127.0.0.1", 9999);
            c2.connect("127.0.0.1", 9999);
            c3.connect("127.0.0.1", 9999);

            Timer t = new Timer(NetTest.doPing, new AutoResetEvent(false), 0, 2000);

            while (true)
            {
                List<Command> l = new List<Command>();
                Command cm = new Command();
                cm.ct = CommandType.ATTACK;
                cm.direction.X = 3.14159F;
                cm.direction.Y = 3.14159F;
                cm.position.X = 3.14159F;
                cm.position.Y = 3.14159F;
                l.Add(cm);
                c.sendCMD(l);
                c1.sendCMD(l);
                c2.sendCMD(l);
                c3.sendCMD(l);
                Thread.Sleep(10);
            }
        }

        public static void doPing(Object st)
        {
            Console.WriteLine("Ping for 1: {0}", c.getPing());
            Console.WriteLine("Ping for 2: {0}", c1.getPing());
            Console.WriteLine("Ping for 3: {0}", c2.getPing());
            Console.WriteLine("Ping for 4: {0}", c3.getPing());
            Console.WriteLine("Ping for 4: {0}", s.getStats().rcvdPkts);
        }

        //public void exit()
        //{
        //    s.exit();
        //    c.exit();
        //}
    }
}
