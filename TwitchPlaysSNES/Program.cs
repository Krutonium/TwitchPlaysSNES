using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using WindowsInput;
using WindowsInput.Native;
using TwitchLib.Client;
using TwitchLib.Client.Enums;
using TwitchLib.Client.Events;
using TwitchLib.Client.Extensions;
using TwitchLib.Client.Models;
namespace TwitchPlaysSNES
{

    class Program
    {
        [DllImport("user32.dll")]
        public static extern int SetForegroundWindow(IntPtr hWnd);
        static InputSimulator sim = new InputSimulator();
        static TwitchClient client = new TwitchClient();
        static Controller cont = new Controller();


        [STAThread]
        static void Main(string[] args)
        {
            string token = File.ReadAllText("./token.txt");
            ConnectionCredentials credentials = new ConnectionCredentials("PFCKrutonium", token);
            client.Initialize(credentials, "PFCKrutonium");
            client.OnConnected += Client_OnConnected;
            client.OnMessageReceived += Client_OnMessageReceived;
            client.Connect();
            while (true)
            {
                System.Threading.Thread.Sleep(1000); //How often to press it
                var button = cont.buttons.Max(p => p.Value);
                int currmax = 0;
                string keyToPress = null;
                foreach(var butt in cont.buttons)
                {
                    if(butt.Value > currmax)
                    {
                        currmax = butt.Value;
                        keyToPress = butt.Key;
                    }
                }
                if(keyToPress == null)
                {
                    Console.WriteLine("No Buttons Pressed");
                }
                else
                {
                    Console.WriteLine(keyToPress);
                    SendButton(cont.keyTokey[keyToPress]);
                }
                cont = new Controller();
            }
        }



        private static void Client_OnMessageReceived(object sender, OnMessageReceivedArgs e)
        {
            //Recieve Messages and use them to decide which button to press.
            Console.WriteLine(e.ChatMessage.Message);
            string Message = e.ChatMessage.Message.ToUpper();
            if (cont.buttons.ContainsKey(Message)){
                cont.buttons[Message] += 1;
            }
        }

        public class Controller
        {
            //public int A,B,X,Y,L,R,UP,DOWN,LEFT,RIGHT,START,SELECT = 0;
            public Dictionary<string, int> buttons = new Dictionary<string, int>()
        {
            { "A", 0 },
            { "B", 0 },
            { "X", 0 },
            { "Y", 0 },
            {"LEFT", 0 },
            {"RIGHT", 0 },
            {"UP", 0},
            {"DOWN", 0 },
            {"START", 0 },
            {"SELECT", 0 },
            {"L", 0},
            {"R", 0 }
        };
            public readonly Dictionary<String, VirtualKeyCode> keyTokey = new Dictionary<string, VirtualKeyCode>()
            {
                {"A", VirtualKeyCode.VK_A },
                {"B", VirtualKeyCode.VK_B },
                {"X", VirtualKeyCode.VK_X },
                {"Y", VirtualKeyCode.VK_Y },
                {"LEFT", VirtualKeyCode.LEFT },
                {"RIGHT", VirtualKeyCode.RIGHT },
                {"UP", VirtualKeyCode.UP },
                {"DOWN", VirtualKeyCode.DOWN },
                {"START", VirtualKeyCode.RETURN },
                {"SELECT", VirtualKeyCode.RSHIFT },
                {"L", VirtualKeyCode.VK_L },
                {"R", VirtualKeyCode.VK_R }
            };
        }
        private static void Client_OnConnected(object sender, OnConnectedArgs e)
        {
            Console.WriteLine("Connected");
        }

        static void SendButton(VirtualKeyCode key)
        {
            Process[] process = Process.GetProcessesByName("higan");
            foreach (Process proc in process)
            {
                SetForegroundWindow(proc.MainWindowHandle);
                sim.Keyboard.KeyDown(key);
                System.Threading.Thread.Sleep(300); //How long to hold the key
                sim.Keyboard.KeyUp(key);
                Console.WriteLine("Pressed a button");
            }
        }
    }
}
