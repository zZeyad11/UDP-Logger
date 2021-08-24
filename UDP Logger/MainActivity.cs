using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V7.App;
using Android.Text;
using Android.Views;
using Android.Widget;
using Video_Record_Stream;

namespace UDP_Logger
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity
    {
        const int Port = 1234;
        EditText Message;
        TextView Log;
        Button Send;
        UdpSocketReceiver receiver;


        string SenderIP;
        string SenderPort;



        protected async override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);

            //Starting
            Message = FindViewById<EditText>(Resource.Id.editText1);
            Log = FindViewById<TextView>(Resource.Id.textView1);
            Send = FindViewById<Button>(Resource.Id.button1);
            receiver = new UdpSocketReceiver();
            FindViewById<ScrollView>(Resource.Id.scrollView1).ScrollbarFadingEnabled = false;

            receiver.MessageReceived += Receiver_MessageReceived;

            // listen for udp traffic on listenPort
            await receiver.StartListeningAsync(Port);
           
            LogMessage("Started Recieving Incoming Connections on "+ GetLocalIPAddress()+":"+Port);
            Send.Enabled = true;
            Message.Enabled = true;

            Send.Click += Send_Click;



        }

        private async void Send_Click(object sender, EventArgs e)
        {
            await receiver.SendToAsync(Encoding.UTF8.GetBytes(Message.Text),SenderIP, int.Parse(SenderPort));
            Message.Text = "";
        }

        [Obsolete]
        void LogMessage(string MSG,string Sender="")
        {


            RunOnUiThread(() => { Log.Text += Html.FromHtml($"<b>{ DateTime.Now.ToString("dd:mm:yyyy:HH:mm:ss") + (Sender == "" ? " :" : (", " + Sender + " :"))}</b>") + MSG + "\n\n";
            FindViewById<ScrollView>(Resource.Id.scrollView1).ScrollTo(0, FindViewById<ScrollView>(Resource.Id.scrollView1).Bottom);



            });
            
        }

        public static string GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (var ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
            throw new Exception("No network adapters with an IPv4 address in the system!");
        }

        private void Receiver_MessageReceived(object sender, UdpSocketMessageReceivedEventArgs e)
        {
            SenderIP = e.RemoteAddress;
            SenderPort = e.RemotePort;
            LogMessage(Encoding.UTF8.GetString(e.ByteData, 0, e.ByteData.Length)); //LogMessage(Encoding.UTF8.GetString(e.ByteData, 0, e.ByteData.Length), SenderIP);

        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

     

        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }
	}
}
