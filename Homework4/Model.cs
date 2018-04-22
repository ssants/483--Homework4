using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace Homework4
{
    partial class Model
    {
        public ObservableCollection<Tile> TileCollection;
        private static UInt32 numTiles = 9;

        // xpressed keeps track of X's tiles
        private bool[] xpressed = new bool[9];
        // opressed keeps track of O's tiles
        private bool[] opressed = new bool[9];
        // win# used to highlight the winning tiles
        private int win1, win2, win3;
        // true = x, false = o, start on x
        private bool turn;
        // true = player x, false = player o
        private bool player;
        // used to check for tie
        private int count;

        UdpClient datasock;
        private static int localport;
        private static string localip;
        private static int remoteport;
        private static string remoteip;

        private Thread receiveThread;
        private Thread syncThread;

        #region General stuff
        public Model()
        {
            
            TileCollection = new ObservableCollection<Tile>();
            for (int i = 0; i < numTiles; i++)
            {
                TileCollection.Add(new Tile()
                {
                    tBrush = Brushes.Black,
                    tLabel = "",
                    tName = i.ToString(),
                    tBackground = Brushes.LightGray,
                    tEnabled = false
                });
            }
            
            MainBackground = Brushes.White;
            
            Start = "Play";
            StartEnabled = false;

            StatusHeader = "Status";
            StatusColor = Brushes.DarkOrange;
            Status = "Press ? to setup sockets";
        }

        public void cleanup()
        {
            if (datasock != null) datasock.Close();
            if (syncThread != null) syncThread.Abort();
            if (receiveThread != null) receiveThread.Abort();
        }

        public void localTilePressed(int which)
        {
            // Does various checks before actually making a move
            Tile t = TileCollection[which];

            // Check  if game over
            if (count == 9) return;

            // Check if it is the player's turn
            if (turn != player)
            {
                StatusColor = Brushes.Red;
                Status = "It isn't your turn yet";
                return;
            }

            // Check if tile has already been claimed
            if (t.tLabel != "")
            {
                StatusColor = Brushes.Red;
                Status = "That tile is already claimed!";
                return;
            }

            if (turn)
            {
                t.tLabel = "X";
                t.tBrush = Brushes.Red;
            }
            else
            {
                t.tLabel = "O";
                t.tBrush = Brushes.RoyalBlue;
            }

            TinyTile tt = new TinyTile(t, which);

            sendTile(tt);

            updateGame(tt);
        }

        private void updateGame(TinyTile tt)
        {
            int which = tt.num;
            Tile newtile = tt.getTile();
            count++;

            // Change Start button text after first turn
            if (count == 1) Start = "Restart";

            // Update data for X or O player
            if (turn)
            {
                xpressed[which] = true;

                StatusColor = Brushes.Black;
                Status = "O's turn";
            }
            else
            {
                opressed[which] = true;

                StatusColor = Brushes.Black;
                Status = "X's turn";
            }
            TileCollection[which] = newtile;

            // Check for winner
            if (isWinner())
            {
                 if (turn == player)
                    win();
                else
                    lose();
                return;
            }

            // If all tiles claimed, tie game
            if (count == 9) { tie(); return; }

            // Else, change turns
            turn = !turn;
        }

        private bool isWinner()
        {
            // Check winning conditions for X
            // If won, save winning tiles in win#
            if (turn)
            {
                if (xpressed[0] && xpressed[1] && xpressed[2])
                {
                    win1 = 0; win2 = 1; win3 = 2;
                    return true;
                }
                if (xpressed[3] && xpressed[4] && xpressed[5])
                {
                    win1 = 3; win2 = 4; win3 = 5;
                    return true;
                }
                if (xpressed[6] && xpressed[7] && xpressed[8])
                {
                    win1 = 6; win2 = 7; win3 = 8;
                    return true;
                }
                if (xpressed[0] && xpressed[3] && xpressed[6])
                {
                    win1 = 0; win2 = 3; win3 = 6;
                    return true;
                }
                if (xpressed[1] && xpressed[4] && xpressed[7])
                {
                    win1 = 1; win2 = 4; win3 = 7;
                    return true;
                }
                if (xpressed[2] && xpressed[5] && xpressed[8])
                {
                    win1 = 2; win2 = 5; win3 = 8;
                    return true;
                }
                if (xpressed[0] && xpressed[4] && xpressed[8])
                {
                    win1 = 0; win2 = 4; win3 = 8;
                    return true;
                }
                if (xpressed[2] && xpressed[4] && xpressed[6])
                {
                    win1 = 2; win2 = 4; win3 = 6;
                    return true;
                }
            }
            // Check winning conditions for O
            // If won, save winning tiles in win#
            else
            {
                if (opressed[0] && opressed[1] && opressed[2])
                {
                    win1 = 0; win2 = 1; win3 = 2;
                    return true;
                }
                if (opressed[3] && opressed[4] && opressed[5])
                {
                    win1 = 3; win2 = 4; win3 = 5;
                    return true;
                }
                if (opressed[6] && opressed[7] && opressed[8])
                {
                    win1 = 6; win2 = 7; win3 = 8;
                    return true;
                }
                if (opressed[0] && opressed[3] && opressed[6])
                {
                    win1 = 0; win2 = 3; win3 = 6;
                    return true;
                }
                if (opressed[1] && opressed[4] && opressed[7])
                {
                    win1 = 1; win2 = 4; win3 = 7;
                    return true;
                }
                if (opressed[2] && opressed[5] && opressed[8])
                {
                    win1 = 2; win2 = 5; win3 = 8;
                    return true;
                }
                if (opressed[0] && opressed[4] && opressed[8])
                {
                    win1 = 0; win2 = 4; win3 = 8;
                    return true;
                }
                if (opressed[2] && opressed[4] && opressed[6])
                {
                    win1 = 2; win2 = 4; win3 = 6;
                    return true;
                }
            }

            return false;
        }

        private void win()
        {
            // Highlight winning tiles
            TileCollection[win1].tBackground = Brushes.GreenYellow;
            TileCollection[win2].tBackground = Brushes.GreenYellow;
            TileCollection[win3].tBackground = Brushes.GreenYellow;
            
            MainBackground = Brushes.DarkGreen;

            Start = "Play Again";
            StatusColor = Brushes.White;
            Status = "Winner Winner! Congradulations";

            count = 9;
            return;
        }

        private void lose()
        {
            // Highlight winning tiles
            TileCollection[win1].tBackground = Brushes.DarkOrange;
            TileCollection[win2].tBackground = Brushes.DarkOrange;
            TileCollection[win3].tBackground = Brushes.DarkOrange;
            
            MainBackground = Brushes.DarkRed;

            Start = "Play Again";
            StatusColor = Brushes.White;
            Status = "You lost, better luck next time";

            count = 9;
            return;
        }

        private void tie()
        {
            // Diable all tiles after tie
            for (int i = 0; i < numTiles; i++)
            {
                TileCollection[i].tEnabled = false;
            }

            MainBackground = Brushes.DarkOrange;

            Start = "Play Again";

            StatusColor = Brushes.White;
            Status = "It's a tie!\nPress Play to begin";
        }
        
        public void startPressed()
        {
            clear();
            sendTile(new TinyTile(-1));
        }

        public void clear()
        {
            // Reset game
            for (int i = 0; i < numTiles; i++)
            {
                TileCollection[i].tBrush = Brushes.Black;
                TileCollection[i].tLabel = "";
                TileCollection[i].tBackground = Brushes.LightGray;
                TileCollection[i].tEnabled = true;

                xpressed[i] = false;
                opressed[i] = false;
            }

            MainBackground = Brushes.White;

            Start = "Restart";
            StartEnabled = true;

            StatusHeader = " You are " + (player ? "X" : "O");
            StatusColor = Brushes.Black;
            Status = "X's turn";

            win1 = -1; win2 = -1; win3 = -1;
            turn = true;
            count = 0;
        }

        #endregion

        #region Network Stuff

        private bool initModel()
        {
            try
            {
                datasock = new UdpClient(localport);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.ToString());
                return false;
            }

            //waitingThread = new Thread(new ThreadStart(waitingForPlayer));
            //waiting = true;
            syncThread = new Thread(new ThreadStart(syncPlayers));

            //waitingThread.Start();
            syncThread.Start();

            StatusHeader = "Waiting...";
            Status = "Searching for player 2";
            return true;
        }

        public void setNetworkSettings(int lport, string lip, int rport, string rip)
        {
            if (lport == 0 || rport == 0 || lip == "" || rip == "")
                return;
            localport = lport;
            remoteport = rport;
            localip = lip;
            remoteip = rip;

            initModel();
        }

        private void syncPlayers()
        {
            Console.WriteLine("Sync Thread: Starting");
            Byte[] data = new Byte[1];
            IPEndPoint sendEP = new IPEndPoint(IPAddress.Parse(remoteip), remoteport);
            IPEndPoint receiveEP = new IPEndPoint(IPAddress.Any, 0);

            UdpClient syncsock = new UdpClient(localport + 10);

            datasock.Client.ReceiveTimeout = 1000; // 1 sec

            while (true)
            {
                try
                {
                    syncsock.Send(data, data.Length, sendEP);
                    datasock.Receive(ref receiveEP);
                    break;
                }
                catch (SocketException se)
                {
                    if (se.ErrorCode == (int)SocketError.TimedOut)
                    {
                        Console.WriteLine("Sync Thread: Player sync timed out");
                    }
                    else
                    {
                        syncsock.Close();
                        Status = Status + "Socket exception: Unable to sync\n" + se.ToString();
                        //waiting = false;
                    }
                }
                catch (System.ObjectDisposedException ode)
                {
                    Console.WriteLine(ode.ToString());
                    syncsock.Close();
                    //waiting = false;
                    Status = "Error occured. Uable to sync\n";
                }
            }
            //waiting = false;
            Console.WriteLine("Sync Thread: Players synced");
            syncsock.Send(data, data.Length, sendEP);
            syncsock.Close();
            datasock.Client.ReceiveTimeout = 0;

            StatusColor = Brushes.Black;
            Status = "Player 2 has joined!\n";
            Thread.Sleep(1000);

            setupGame();
            Console.WriteLine("Sync Thread: Closing");
        }

        private void setupGame()
        { 
            // Whichever is smaller is x
            // Player = true is X, X goes first
            player = localport < remoteport;
            Console.WriteLine("Sync Thread: Local = {0}, Remote = {1}", localport, remoteport);
            if (player)
                Status += "You are X\n";
            else
                Status += "You are O\n";
            turn = player;

            Console.WriteLine("Sync Thread: Starting Receive Thread");
            receiveThread = new Thread(new ThreadStart(receiveData));
            receiveThread.Start();

            // Auto start game on connect
            clear();

            Console.WriteLine("Sync Thread: Setup the game");
        }

        private void receiveData()
        {
            Console.WriteLine("Receive Thread: Starting");
            IPEndPoint ep = new IPEndPoint(IPAddress.Any, 0);
            Console.WriteLine("Receive Thread: Listeing");
            while (true)
            {
                try
                {
                    Byte[] receivedData = datasock.Receive(ref ep);

                    if (receivedData.Length < 2)
                        continue;

                    Console.WriteLine("Receive Thread: Received a tile");
                    TinyTile tt;
                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream stream = new MemoryStream();

                    stream = new MemoryStream(receivedData);
                    tt = (TinyTile)formatter.Deserialize(stream);
                    if (tt.num < 0)
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            clear();
                        }));
                    }
                    else
                    {
                        Application.Current.Dispatcher.Invoke(new Action(() =>
                        {
                            updateGame(tt);
                        }));
                    }

                }
                catch (SocketException se)
                {
                    Console.WriteLine("Receive Thread: Failed to receive \n" + se.Message);
                    return;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.ToString());
                }

            }
        }

        private void sendTile(TinyTile tt)
        {
            BinaryFormatter formatter = new BinaryFormatter();
            MemoryStream stream = new MemoryStream();
            Byte[] sendthis;

            formatter.Serialize(stream, tt);
            sendthis = stream.ToArray();

            IPEndPoint remotehost = new IPEndPoint(IPAddress.Parse(remoteip), remoteport);
            try
            {
                datasock.Send(sendthis, sendthis.Length, remotehost);
            }
            catch (SocketException se)
            {
                Console.WriteLine(se.ToString());
                Status = "Error: Tile failed to send";
                return;
            }
        }

        #endregion
    }
}