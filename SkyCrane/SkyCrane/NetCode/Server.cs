﻿using System;
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
    public static class NetTest
    {
        //RawServer s;
        //RawClient c;
        //public NetTest(int port)
        //{
        //    s = new RawServer(port);
        //    c = new RawClient();
        //    c.connect("127.0.0.1", port);
        //}

        public static void Main(string[] args)
        {
            //RawClient c = new RawClient();
            RawServer s = new RawServer(9999);
            while (true)
            {
                if (s.getCMD().Count > 0)
                {
                    foreach (Command c in s.getCMD())
                    {
                        Console.WriteLine(c.ct);
                        Console.WriteLine(c.entity_id);
                        Console.WriteLine(c.direction.X);
                        Console.WriteLine(c.direction.Y);
                        Console.WriteLine(c.position.X);
                        Console.WriteLine(c.position.Y);
                        Console.WriteLine("===========");
                    }
                }
                List<StateChange> l = new List<StateChange>();
                StateChange st = new StateChange();
                st.type = StateChangeType.DELETE_ENTITY;
                st.intProperties[StateProperties.FRAME_WIDTH] = 123;
                st.stringProperties[StateProperties.SPRITE_NAME] = "I'm the Baconator";
                l.Add(st);
                s.broadcastSC(l);
                Thread.Sleep(2000);
            }
            //c.connect("127.0.0.1", 9999);
        }

        //public void exit()
        //{
        //    s.exit();
        //    c.exit();
        //}
    }

    class RawServer
    {
        private Thread serverThread;
        private bool go = true;
        private NetworkWorker nw;

        //Connection state
        Dictionary<IPEndPoint, ConnectionID> connections = new Dictionary<IPEndPoint, ConnectionID>();
        List<Command> commandQ = new List<Command>();

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
                        Console.WriteLine("Server - Receivd State Change from client... who do they think they are?");
                        Environment.Exit(1);
                        break;

                    case Packet.PacketType.SYNC:
                        if (connections.ContainsKey(p.Dest))
                        {
                            Console.WriteLine("Server - SYNC Reply from: " + connections[p.Dest].ID);
                        }
                        else
                        {
                            Console.WriteLine("Server - ERROR Unregistered SYNC");
                        }
                        break;

                    case Packet.PacketType.PING:
                        if (connections.ContainsKey(p.Dest))
                        {
                            Console.WriteLine("Server - Ping from connection: " + connections[p.Dest].ID);
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
                        Console.WriteLine("Server - Got CMD from: " + connections[p.Dest].ID);
                        Command cmd = new Command(p.data);
                        lock (commandQ)
                            this.commandQ.Add(cmd);
                        break;
                }
            }
        }

        public List<Command> getCMD()
        {
            List<Command> ret;
            lock (commandQ)
            {
                ret = new List<Command>(commandQ);
                //commandQ.Clear();
            }
            return ret;
        }

        public void broadcastSC(List<StateChange> list)
        {
            foreach (StateChange sc in list)
            {
                foreach (KeyValuePair<IPEndPoint, ConnectionID> d in connections)
                {
                    Console.WriteLine("Server - Sent StateChange to: " + d.Value.ID);
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
            }
        }
    }

    public class ConnectionID
    {
        private static short ids = 1;
        public short ID;
        public IPEndPoint endpt;
        public System.Diagnostics.Stopwatch stopwatch = new System.Diagnostics.Stopwatch();
        public long lastSYNC = -1;

        public static ConnectionID newConnectionID(IPEndPoint ep)
        {
            ConnectionID c = new ConnectionID(ep);
            c.ID = ids++;
            return c;
        }

        public ConnectionID(IPEndPoint ep)
        {
            this.endpt = ep;
        }
    }
}
