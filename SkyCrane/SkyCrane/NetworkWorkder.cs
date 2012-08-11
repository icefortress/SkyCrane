using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;

namespace SkyCrane
{
    class NetworkWorker : UdpClient
    {
        private Thread rcvThread, sendThread;
        private bool go = true;

        private IPEndPoint echo;
        private Queue<Packet> buffer = new Queue<Packet>();
        private Queue<Packet> readBuffer = new Queue<Packet>();

        private static int id = 0;
        private int myID;

        //This is the server side
        public NetworkWorker(int port = 0)
            : base(port)
        {
            Console.WriteLine("Started NW on port: " + this.Client.LocalEndPoint);
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
            Console.WriteLine("Started NW on port: " + this.Client.LocalEndPoint);
            this.rcvThread = new Thread(thread_do_recv);
            this.sendThread = new Thread(thread_do_send);
            rcvThread.Name = "Receive Thread ID: " + id;
            sendThread.Name = "Send Thread ID: " + id;
            this.rcvThread.Start();
            this.sendThread.Start();
            this.myID = id;
            NetworkWorker.id++;
        }

        public void commitPacket(IPEndPoint dest, byte[] data)
        {
            Packet p = new Packet();
            p.data = data;
            p.Dest = dest;
            lock (this)
            {
                this.buffer.Enqueue(p);
            }
        }

        public Packet getNext()
        {
            if (readBuffer.Count > 0)
                return readBuffer.Dequeue();
            else
                return null;
        }

        public bool hasNext()
        {
            return (this.readBuffer.Count > 0) ? true : false;
        }

        private void thread_do_recv()
        {
            Thread.CurrentThread.IsBackground = true;
            IPEndPoint srv = new IPEndPoint(IPAddress.Any, 0);
            while (this.go)
            {
                byte[] data = this.Receive(ref srv);
                echo = srv;
                Console.WriteLine("NW-" + myID + " Recv: "+data.Length+" bytes");
                lock (this)
                {
                    Packet p = new Packet();
                    p.Dest = srv;
                    p.data = data;
                    readBuffer.Enqueue(p);
                }
            }
        }

        private void thread_do_send()
        {
            Thread.CurrentThread.IsBackground = true;
            while (this.go)
            {
                lock (this)
                {
                    if (buffer.Count > 0)
                    {
                        Console.WriteLine("NW-" + myID + " Send");
                        Packet pkt = buffer.Dequeue();
                        this.Send(pkt.data, pkt.data.Length, pkt.Dest);
                    }
                }
            }
        }
    }

    public class Packet
    {
        public IPEndPoint Dest = null;
        public byte[] data = null;
    }
}
