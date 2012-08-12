using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;

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

namespace SkyCrane.NetCode
{
    public class RawServer
    {
        private Thread serverThread;
        private bool go = true;
        private NetworkWorker nw;

        //Connection state
        Dictionary<IPEndPoint, ConnectionID> connections = new Dictionary<IPEndPoint, ConnectionID>();
        List<Command> commandQ = new List<Command>();
        Queue<Tuple<ConnectionID, MenuState>> mscQ = new Queue<Tuple<ConnectionID, MenuState>>();

        public RawServer(int port)
        {
            serverThread = new Thread(runThis);
            serverThread.Name = "Main Server";
            serverThread.Priority = ThreadPriority.AboveNormal;
            serverThread.IsBackground = true;

            System.Console.WriteLine("Starting Server");
            this.nw = new NetworkWorker(port);
            serverThread.Start();
        }

        public void exit()
        {
            this.go = false;
        }

        private void runThis()
        {
            Packet p;
            while (this.go)
            {
                p = nw.getNext();
                if (p == null)
                {
                    foreach (IPEndPoint ep in connections.Keys)
                    {
                        SYNCPacket ps = new SYNCPacket();
                        ps.Dest = ep;
                        this.nw.commitPacket(ps);
                    }
                    continue;
                }
                //Console.WriteLine(p.ptype);

                switch (p.ptype)
                {
                    case Packet.PacketType.HANDSHAKE:
                        if (!connections.ContainsKey(p.Dest))
                        {
                            Console.WriteLine("Server - New connection from: " + p.Dest);
                            connections[p.Dest] = new ConnectionID(p.Dest);
                            Console.WriteLine("Server - Added Connection: " + connections[p.Dest].ID);
                            HandshakePacket hs = new HandshakePacket();
                            hs.Dest = p.Dest;
                            nw.commitPacket(hs);
                        }
                        break;

                    case Packet.PacketType.STC:
                        //Console.WriteLine("Server - Receivd State Change from client... who do they think they are?");
                        Environment.Exit(1);
                        break;

                    case Packet.PacketType.SYNC:
                        if (connections.ContainsKey(p.Dest))
                        {
                            //Console.WriteLine("Server - SYNC Reply from: " + connections[p.Dest].ID);
                        }
                        else
                        {
                            Console.WriteLine("Server - ERROR Unregistered SYNC");
                        }
                        break;

                    case Packet.PacketType.PING:
                        if (connections.ContainsKey(p.Dest))
                        {
                            //Console.WriteLine("Server - Ping from connection: " + connections[p.Dest].ID);
                            PingPacket ps = new PingPacket();
                            ps.Dest = p.Dest;
                            nw.commitPacket(ps); //ACK the ping
                        }
                        else
                        {
                            Console.WriteLine("Server ERROR - Unregistered PING");
                        }
                        break;

                    case Packet.PacketType.CMD:
                        //Actually handle this
                        //Console.WriteLine("Server - Got CMD from: " + connections[p.Dest].ID);
                        Command cmd = new Command(p.data);
                        lock (commandQ)
                            this.commandQ.Add(cmd);
                        break;
                    case Packet.PacketType.MSC:
                        //Actually handle this
                        //Console.WriteLine("Server - Got MSC from: " + connections[p.Dest].ID);
                        MenuState msc = new MenuState(p.data);
                        if (connections.ContainsKey(p.Dest))
                        {
                            ConnectionID cid = connections[p.Dest];
                            Tuple<ConnectionID, MenuState> newMQ = new Tuple<ConnectionID, MenuState>(cid, msc);
                            lock (mscQ)
                                this.mscQ.Enqueue(newMQ);
                        }
                        break;
                }
            }
        }

        public List<Command> getCMD()
        {
            List<Command> ret=new List<Command>();
            lock (commandQ)
            {
                foreach (Command c in this.commandQ)
                    ret.Add(c);
                commandQ.Clear();
            }
            return ret;
        }

        public List<Tuple<ConnectionID, MenuState>> getMSC()
        {
            List<Tuple<ConnectionID, MenuState>> ret = new List<Tuple<ConnectionID, MenuState>>();

            lock (mscQ)
            {
                while (mscQ.Count > 0)
                {
                    ret.Add(mscQ.Dequeue());
                }
            }

            return ret;
        }

        public void broadcastSC(List<StateChange> list)
        {
            //Console.WriteLine("Sending # of SCs: {0}", list.Count);
            foreach (StateChange sc in list)
            {
                foreach (KeyValuePair<IPEndPoint, ConnectionID> d in connections)
                {
                    //Console.WriteLine("Server - Sent StateChange to: " + d.Value.ID);
                    STCPacket p = new STCPacket(sc);
                    p.Dest = d.Key;
                    this.nw.commitPacket(p);
                }
            }
        }

        public void signalSC(List<StateChange> list, ConnectionID cid)
        {
            foreach (StateChange sc in list)
            {
                STCPacket p = new STCPacket(sc);
                p.Dest = cid.endpt;
                nw.commitPacket(p);
            }
        }

        public void broadcastMSC(List<MenuState> list)
        {
            foreach (MenuState msc in list)
            {
                broadcastMSC(msc);
            }
            return;
        }

        public void broadcastMSC(MenuState m)
        {
            foreach (KeyValuePair<IPEndPoint, ConnectionID> d in connections)
            {
                //Console.WriteLine("Server - Sent MenuState to: " + d.Value.ID);
                MSCPacket p = new MSCPacket(m);
                p.Dest = d.Key;
                this.nw.commitPacket(p);
            }
            return;
        }

        public void signalMSC(List<MenuState> list, ConnectionID cid)
        {
            foreach (MenuState m in list)
            {
                signalMSC(m, cid);
            }
        }

        public void signalMSC(MenuState m, ConnectionID cid)
        {
            MSCPacket p = new MSCPacket(m);
            p.Dest = cid.endpt;
            nw.commitPacket(p);
        }
    }

    public class ConnectionID
    {
        private static short ids = 0;
        public short ID;
        public IPEndPoint endpt;
        public long lastSYNC = -1;

        public ConnectionID(IPEndPoint ep)
        {
            ID = ids++;
            this.endpt = ep;
        }
    }
}
