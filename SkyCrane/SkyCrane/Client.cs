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
        public enum cState { DISCONNECTED, CONNECTED, TRYCONNECT };
        cState curState = cState.DISCONNECTED;

        // Queue of state changes to be passed off the the UI
        private Queue<StateChange> buffer = new Queue<StateChange>();

        public RawClient()
        {
            System.Console.WriteLine("Client Started");

            clientThread = new Thread(this.ClientstartFunc);
            clientThread.Name = "mainClientThread";
            clientThread.Start();
        }

        public bool connect(string host, int port)
        {
            // Store server info
            this.server = new IPEndPoint(IPAddress.Parse(host), port);

            // Client may now try to connect
            this.curState = cState.TRYCONNECT;

            //Spawn the client reader/writer threads
            this.nw = new NetworkWorker(server);

            // Send the handshake request to the server
            this.handshake();

            // Wait for the connection to be established
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
            // Hold here until the server information has been provided
            // TODO

            // Event Loop
            // Pull packets out of the network layer and handle them
            while (this.go)
            {
                Packet newPacket = nw.getNext(); // This is a blocking call! 

                // Handle timeout
                if (newPacket == null)
                {
                    Console.WriteLine("Timeout on receive");
                    switch (curState)
                    {
                        case cState.TRYCONNECT:
                            break;
                        case cState.CONNECTED:
                            break;
                        case cState.DISCONNECTED:
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    // Handle the new packet 
                    switch (newPacket.ptype)
                    {
                        case Packet.PacketType.CMD:
                            Console.WriteLine("Should not be getting CMD packets from the server...");
                            Environment.Exit(1);
                            break;
                        case Packet.PacketType.HANDSHAKE:
                            Console.WriteLine("Handshake received from the server");
                            break;
                        case Packet.PacketType.STC:
                            Console.WriteLine("STC received from the server");
                            // Marshall the state change packet into an object
                            StateChange newSTC = new StateChange(newPacket.data);

                            // Add the state change object to the buffer for the UI
                            lock (this.buffer)
                            {
                                buffer.Enqueue(newSTC);
                            }
                            break;
                        case Packet.PacketType.SYNC:
                            Console.WriteLine("SYNC received from the server");
                            break;
                        default:
                            Console.WriteLine("Unknown packet type from the server...");
                            Environment.Exit(1);
                            break;
                    }
                }
            }
        }

        private void handshake()
        {
            HandshakePacket hs = new HandshakePacket();
            hs.setDest(server);
            this.nw.commitPacket(hs);
        }

        //OPERATORS
        public void sendCMD(List<Command> cmds)
        {
            foreach (Command c in cmds)
            {
                // Create the CMD Packet
                CMDPacket newCMD = new CMDPacket(c);
                newCMD.Dest = server;

                // Add the CMD packet to the network worker's send queue
                this.nw.commitPacket(newCMD);
            }
        }

        // Called by the UI to acquire the latest state from the server
        public List<StateChange> rcvUPD()
        {
            List<StateChange> newStates = new List<StateChange>();

            // Acquire a the buffer lock well emptying the buffer

            lock (this.buffer)
            {
                // Iterate over the buffer of states that have been acquired from the server
                while (buffer.Count > 0)
                {
                    newStates.Add(buffer.Dequeue());
                }
            }

            return newStates;
        }
    }
}
