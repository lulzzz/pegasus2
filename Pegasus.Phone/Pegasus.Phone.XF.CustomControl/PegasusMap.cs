using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace Pegasus.Phone.XF.CustomControl
{
    public struct Position
    {
        public double Latitude;
        public double Longitude;
    }

    public class PegasusMap : View
    {
        public static readonly BindableProperty CenterPositionProperty =
            BindableProperty.Create<PegasusMap, Position> (
                p => p.CenterPosition, new Position());
        public Position CenterPosition
        {
            get { return (Position)GetValue(CenterPositionProperty); }
            set { SetValue(CenterPositionProperty, value); }
        }

    }
}
