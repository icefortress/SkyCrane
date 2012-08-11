using System;
using System.Threading;
using System.Net;
using System.Net.Sockets;
using System.Collections.Generic;
using System.Linq;
using System.Text;

/*What do I want to achieve here...
    I need to specify the transport layer
 * I require a constant interface which is exported to the game logic
 * We want to be able to send game information
 * Need a way to specify who to connnect to... probably via direct connection at first
 * Need to be able to receive information in a new thread
 * Probably have some sort of obeservable object, which notifies interested parties of new events.
 * I'll probably supply interfaces for player centric activity and game world.
 * We'll likely send updates in a single packet -> Extract them and notify the required sub components.
 * -> the subcomponents will be inhereted by game logic (so a component for players [subtype of character, which could be a monster or player])
 * 
 * Latency and connectivity measurements
 */

namespace SkyCrane
{
    class NetTest
    {
        RawServer s;
        RawClient c;
        public NetTest(int port)
        {
            s = new RawServer(port);
            c = new RawClient();
            c.connect("127.0.0.1", port);
        }

        public void exit()
        {
            s.exit();
            c.exit();
        }
    }

    class RawServer
    {
        private Thread serverThread;
        private bool go = true;
        private NetworkWorker nw;

        public RawServer(int port)
        {
            serverThread = new Thread(runThis);
            serverThread.Name = "Main Server";

            System.Console.WriteLine("Starting test ");
            this.nw = new NetworkWorker(port);
            serverThread.Start();
        }

        public void exit()
        {
            this.go = false;
        }

        private void runThis() {
            Packet p;
            while (this.go)
            {
                if (nw.hasNext())
                {
                    p = nw.getNext();

                    Console.WriteLine(p.data.Length);
                    StateChange t = StateChange.getStateData(p.data);
                    foreach (var element in t.intProperties)
                    {
                        Console.WriteLine(element);
                    }
                }
            }
        }

        public List<Command> getCMD()
        {
        }

        public void broadcastSC(List<StateChange> list)
        {
        }

        public void signalSC(List<StateChange> list, ConnectionID cid){

        }
    }

    public class ConnectionID
    {
        private static short ids = 0;
        public short ID;

        public static ConnectionID newConnectionID(){
            ConnectionID c = new ConnectionID();
            c.ID = ids++;
            return c;
        }
    }
}
