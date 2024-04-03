using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MailApp.Models.Events
{
    public class WebView2WheelEventArgs : EventArgs
    {
        public WebView2WheelEventArgs(double deltaX, double deltaY)
        {
            DeltaX = deltaX;
            DeltaY = deltaY;
        }

        public double DeltaX { get; }
        public double DeltaY { get; }
    }
}
