using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Shared;
using Size = System.Drawing.Size;

namespace Server
{
    public class DesktopHub : Hub<IDesktopHub>
    {
        public async IAsyncEnumerable<Frame> Frames([EnumeratorCancellation]CancellationToken cancellationToken)
        {
            var stopwatch = new Stopwatch();

            while (true)
            {
                stopwatch.Restart();

                cancellationToken.ThrowIfCancellationRequested();
                
                var data = new Frame {Bytes = Capture()};

                yield return data;

                await Task.CompletedTask;

                var frameRate = 1000 / stopwatch.ElapsedMilliseconds;
                Console.WriteLine($"Frame rate: {frameRate}");
            }
        }

        public byte[] Capture()
        {
            // TODO: Determine desktop size programmatically
            var imageSize = new Size(1000, 1000);

            using var bmp = new Bitmap(imageSize.Width, imageSize.Height, PixelFormat.Format32bppArgb);
            {
                using (var graphics = Graphics.FromImage(bmp))
                {
                    graphics.CopyFromScreen(0, 0, 0, 0, imageSize);
                }

                using (var memoryStream = new MemoryStream())
                {
                    bmp.Save(memoryStream, ImageFormat.Jpeg);
                    return memoryStream.ToArray();
                }
            }
        }
    }
}