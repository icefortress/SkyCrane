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
        private IPEndPoint server;
        private bool go = true;
        public enum cState { DISCONNECTED, CONNECTED, TRYCONNECT,SEND,RECV,SYNC };
        cState curState = cState.DISCONNECTED;

        // Queue of state changes to be passed off the the UI
        private Queue<StateChange> buffer = new Queue<StateChange>();
        // Lock protecting the buffer queue

        public RawClient()
        {
            System.Console.WriteLine("Client Started");

            clientThread = new Thread(this.ClientstartFunc);
            clientThread.Name = "mainClientThread";
            clientThread.Start();
        }
         
        public bool connect(string host, int port)
        {
            this.server = new IPEndPoint(IPAddress.Parse(host), port);

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
                    this.nw = new NetworkWorker(server);
                    this.handshake();
                    this.curState = cState.CONNECTED;
                }
                if (curState == cState.CONNECTED)
                {

                }
            }
        }

        private bool handshake()
        {
            StateChange s = new StateChange();
            s.stringProperties[StateProperties.POSITION_X] = "Bacon";
            HandshakePacket hs = new HandshakePacket();
            SYNCPacket sp = new SYNCPacket();
            STCPacket stcp = new STCPacket(s);
            stcp.setDest(server);
            sp.setDest(server);
            hs.setDest(server);
                this.nw.commitPacket(sp);
                this.nw.commitPacket(stcp);
                this.nw.commitPacket(hs);
            return true;
        }

        //OPERATORS
        public void sendCMD(List<Command> cmds)
        {
        }

        // Called by the UI to acquire the latest state from the server
        public List<StateChange> rcvUPD()
        {
            List<StateChange> newStates = new List<StateChange>();

            // TODO: Acquire a the buffer lock well emptying the buffer
            // Iterate over the buffer of states that have been acquired from the server
            while (buffer.Count > 0)
            {
                newStates.Add(buffer.Dequeue());
            }

            return newStates;
        }
    }
}
