using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Threading;
using System.Net;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

namespace SkyCrane
{
    class RawClient
    {
        private Thread clientThread;
        private NetworkWorker nw;
        private IPEndPoint endPt;
        private bool go = true;
        public enum cState { DISCONNECTED, CONNECTED, TRYCONNECT,SEND,RECV,SYNC };
        cState curState = cState.DISCONNECTED;

        //Bounded buffer for producer consumer
        private Queue<byte[]> buffer = new Queue<byte[]>();

        public RawClient()
        {
            System.Console.WriteLine("Client Started");

            clientThread = new Thread(this.ClientstartFunc);
            clientThread.Name = "mainClientThread";
            clientThread.Start();
        }
         
        public bool connect(string host, int port)
        {
            this.endPt = new IPEndPoint(IPAddress.Parse(host), port);

            this.curState = cState.TRYCONNECT;
            while (this.curState == cState.TRYCONNECT) ;
            return true;
        }

        public void exit()
        {
            this.go = false;
        }

        //Main routine, this does all the processing
        private void ClientstartFunc()
        {
            //Do I want another busy loop that just polls the connection?
            //Event loop
            while (this.go)
            {
                if (curState == cState.TRYCONNECT)
                {
                    //Spawn the client reader/writer threads
                    this.nw = new NetworkWorker(endPt);
                    this.curState = cState.CONNECTED;
                }
                if (curState == cState.CONNECTED)
                {
                    StateChange sc = new StateChange();
                    sc.type = StateChangeType.MOVED;
                    sc.intProperties.Add(StateProperties.POSITION_X,10);
                    nw.commitPacket(endPt,sc.getPacketData());
                }
                Thread.Sleep(1000);
            }
        }

        private bool thread_connect()
        {
            System.Console.WriteLine("Attempting to connect to "+endPt);
            return true;
        }

        //OPERATORS
        public void sendCMD(List<Command> cmds)
        {
        }

        public List<StateChange> rcvUPD()
        {
            return new List<StateChange>();
        }
    }
}
