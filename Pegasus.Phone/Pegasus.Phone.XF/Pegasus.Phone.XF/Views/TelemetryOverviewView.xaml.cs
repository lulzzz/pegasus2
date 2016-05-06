using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace Pegasus.Phone.XF
{
	public partial class TelemetryOverviewView : ContentView
	{
        private double lastWidth, lastHeight;

		public TelemetryOverviewView ()
		{
			InitializeComponent ();
            PressureAltitude.FontSize *= 2;
		}

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (width <= 0 || height <= 0 || (width == lastWidth && height == lastHeight))
            {
                return;
            }

            lastWidth = width;
            lastHeight = height;

            double h = this.PressureAltitude.Height;
            double grid = this.DetailsGrid.Height;
            double ratio = grid / h;
            //Debug.WriteLine("    " + h + ", " + grid + ", r: " + ratio);

            // If Grid:TimeStack is under than 5.5, there's not enough space, flip it
            if (height > width || ratio < 5.5)
            {
                foreach (var child in this.DetailsGrid.Children)
                {
                    var layout = child as StackLayout;
                    if (layout != null)
                    {
                        if (width > height)
                        {
                            layout.Orientation = StackOrientation.Horizontal;
                            layout.Children[0].HorizontalOptions = LayoutOptions.Start;
                            layout.Children[1].HorizontalOptions = LayoutOptions.CenterAndExpand;
                            layout.Children[2].HorizontalOptions = LayoutOptions.End;
                        }
                        else
                        {
                            layout.Orientation = StackOrientation.Vertical;
                            layout.Children[0].HorizontalOptions = LayoutOptions.Center;
                            layout.Children[1].HorizontalOptions = LayoutOptions.CenterAndExpand;
                            layout.Children[2].HorizontalOptions = LayoutOptions.Center;

                        }
                    }
                }
            }
        }
    }
}

