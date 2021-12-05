using System;
using System.Drawing;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.AspNetCore.SignalR.Client;
using Shared;

namespace Client
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private async void Form1_Load(object sender, EventArgs e)
        {
            var hubConnection = new HubConnectionBuilder()
                .WithUrl(Shared.Strings.HubUrl)
                .Build();

            // Call "Cancel" on this CancellationTokenSource to send a cancellation message to
            // the server, which will trigger the corresponding token in the hub method.
            var cancellationTokenSource = new CancellationTokenSource();

            // Loop is here to wait until the server is running
            while (true)
            {
                try
                {
                    await hubConnection.StartAsync(cancellationTokenSource.Token);
                    break;
                }
                catch
                {
                    await Task.Delay(1000, cancellationTokenSource.Token);
                }
            }

            var stream = hubConnection.StreamAsync<Frame>(
                "Frames", cancellationTokenSource.Token);

            await foreach (var data in stream.WithCancellation(cancellationTokenSource.Token))
            {
                await using (var memoryStream = new MemoryStream(data.Bytes))
                {
                    pictureBox1.Image = Image.FromStream(memoryStream);
                }
            }
        }
    }
}