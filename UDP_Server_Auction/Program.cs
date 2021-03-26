using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace UDP_Server_Auction
{
    class Program
    {
        static void Main(string[] args)
        {
            UdpClient udpClient = new UdpClient(11125); //socket di ricezione dati
            IPEndPoint IpRemoto = new IPEndPoint(IPAddress.Any, 0); //accetta datagram da chiunque
            List<IPEndPoint> clients = new List<IPEndPoint>();

            try
            {
                while (true)
                {
                    Console.WriteLine("In attesa di datagram...\n");
                    Byte[] receiveBytes = udpClient.Receive(ref IpRemoto);

                    //chiamata bloccante. attesa datagram
                    string returnData = Encoding.ASCII.GetString(receiveBytes);
                    //Console.WriteLine("Datagram ricevuto: " + returnData.ToString());
                    //Console.WriteLine("Questo datagram è stato inviato dall'Ip " +
                    //IpRemoto.Address.ToString() + " e porta " + IpRemoto.Port.ToString());

                    if(returnData == "HELO")
                    {
                        // add ip and port to list
                        if (!clients.Contains(IpRemoto))
                            clients.Add(IpRemoto);
                    }
                    else if(returnData == "BYE")
                    {
                        if (clients.Contains(IpRemoto))
                            clients.Remove(IpRemoto);
                    }
                    // wants to add bid
                    else
                    {
                        // verify user 
                        List<string> data = returnData.Split(' ').ToList(); // format: ITEM_ID AMOUNT USER_ID PASSWORD
                        string itemId = data[0];
                        string amount = data[1];
                        string userId = data[2];
                        string password = data[3];

                        // verify the password is correct
                        if (DB.UserLogIn(userId, password))
                        {
                            DB.AddBid(itemId, amount, userId);
                        }

                        // send new bid to all clients
                        foreach (var client in clients)
                        {
                            if (IpRemoto.Address.Equals(client.Address) && IpRemoto.Port.Equals(client.Port)) continue;
                            Byte[] sendBytes = Encoding.ASCII.GetBytes("NEW_BID " + amount + " " + itemId);
                            udpClient.Send(sendBytes, sendBytes.Length, client);
                        }
                    }



                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
            }
        }
    }
}
