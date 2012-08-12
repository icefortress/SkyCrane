using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.IO;

namespace SkyCrane
{
    class NetworkWorker : UdpClient
    {
        private Thread rcvThread, sendThread;
        private bool go = true;

        private Queue<Packet> buffer = new Queue<Packet>();
        private Queue<Packet> readBuffer = new Queue<Packet>();

        private static int id = 0;
        private int myID;

        private Semaphore sendSem = new Semaphore(0, 100);
        private Semaphore nextSem = new Semaphore(0, 100);

        private static int TIMEOUT = 5000;

        //This is the server side
        public NetworkWorker(int port = 0)
            : base(port)
        {
            Console.WriteLine("Started NW-Server on port: " + this.Client.LocalEndPoint);
            this.rcvThread = new Thread(thread_do_recv);
            this.sendThread = new Thread(thread_do_send);
            rcvThread.Name = "Receive Thread ID: " + id;
            sendThread.Name = "Send Thread ID: " + id;
            this.rcvThread.Start();
            this.sendThread.Start();
            this.myID = id;
            NetworkWorker.id++;
        }

        //This init's so we only communicate with one
        //It's the Client init (so port is any)
        public NetworkWorker(IPEndPoint endpt)
            : base(0)
        {
            Console.WriteLine("Started NW-Client on port: " + this.Client.LocalEndPoint);
            this.rcvThread = new Thread(thread_do_recv);
            this.sendThread = new Thread(thread_do_send);
            rcvThread.Name = "Receive Thread ID: " + id;
            sendThread.Name = "Send Thread ID: " + id;
            this.rcvThread.Start();
            this.sendThread.Start();
            this.myID = id;
            NetworkWorker.id++;
        }

        public void commitPacket(Packet p)
        {
            lock (this.buffer)
            {
                this.buffer.Enqueue(p);
            }
            this.sendSem.Release();
        }

        public Packet getNext()
        {
            if (this.nextSem.WaitOne(TIMEOUT))
            {
                Packet ret;
                lock (readBuffer)
                {
                    ret = readBuffer.Dequeue();
                }

                return ret;
            }
            else
            {
                return null;
            }
        }

        private void thread_do_recv()
        {
            Thread.CurrentThread.IsBackground = true;
            IPEndPoint srv = new IPEndPoint(IPAddress.Any, 0);
            MemoryStream ms;
            while (this.go)
            {
                Console.WriteLine("waiting..." + Thread.CurrentThread.Name);
                byte[] data = this.Receive(ref srv);
                Console.WriteLine("NW-" + myID + " Recv: " + data.Length + " bytes");
                Packet p = new Packet();
                p.Dest = srv;
                ms = new MemoryStream(data);
                p.ptype = (Packet.PacketType)ms.ReadByte();
                ms.Read(p.data, 1, (int)ms.Length - 1);
                lock (readBuffer)
                {
                    readBuffer.Enqueue(p);
                }
                this.nextSem.Release();
            }
        }

        private void thread_do_send()
        {
            Thread.CurrentThread.IsBackground = true;
            while (this.go)
            {
                this.sendSem.WaitOne(); //Get Semaphore
                lock (this.buffer)
                {
                    Console.WriteLine("NW-" + myID + " Send");
                    Packet pkt = this.buffer.Dequeue();
                    int i = this.Send(pkt.data, pkt.data.Length, pkt.Dest);
                    Console.WriteLine(i);
                }
            }
        }
    }

    public class Packet
    {
        public enum PacketType { HANDSHAKE, CMD, STC, SYNC, PING };
        public PacketType ptype;
        public IPEndPoint Dest = null;
        public byte[] data = new byte[200];
        private MemoryStream ms = new MemoryStream();

        protected void addHeader(PacketType p)
        {
            ms.WriteByte((byte)p);
        }

        protected void addContent(byte[] content)
        {
            ms.Write(content, 0, content.Length);
        }

        protected void finalize()
        {
            this.data = ms.ToArray();
        }

        public void setDest(IPEndPoint p)
        {
            this.Dest = p;
        }
    }

    public class CMDPacket : Packet
    {
        public CMDPacket(Command s)
        {
            this.ptype = PacketType.CMD;
            this.addHeader(ptype);
            this.addContent(s.getPacketData());
            this.finalize();
        }
    }

    public class STCPacket : Packet
    {
        public STCPacket(StateChange s)
        {
            this.ptype = PacketType.STC;
            this.addHeader(ptype);
            this.addContent(s.getPacketData());
            this.finalize();
        }
    }

    public class HandshakePacket : Packet
    {
        public HandshakePacket()
        {
            this.ptype = PacketType.HANDSHAKE;
            this.addHeader(ptype);
            this.finalize();
        }
    }

    public class SYNCPacket : Packet
    {
        public SYNCPacket()
        {
            this.ptype = PacketType.SYNC;
            this.addHeader(ptype);
            this.finalize();
        }
    }

    public class PingPacket : Packet
    {
        public PingPacket()
        {
            this.ptype = PacketType.PING;
            this.addHeader(ptype);
            this.finalize();
        }
    }

    public interface Marshable
    {
        byte[] getPacketData();
    }
}
