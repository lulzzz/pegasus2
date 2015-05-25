﻿// Decompiled with JetBrains decompiler
// Type: Xamarin.Forms.Maps.Android.GeocoderBackend
// Assembly: Xamarin.Forms.Maps.Android, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C53046C-F87C-472B-9B76-6FA6E84409D4
// Assembly location: D:\source\PegasusMission\Pegasus.Phone\packages\Xamarin.Forms.Maps.1.4.2.6359\lib\MonoAndroid10\Xamarin.Forms.Maps.Android.dll

using Android.Content;
using Android.Locations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Maps;

namespace Xamarin.Forms.Maps.Android
{
  internal class GeocoderBackend
  {
    public static void Register(Context context)
    {
      Xamarin.Forms.Maps.Geocoder.GetPositionsForAddressAsyncFunc = new Func<string, Task<IEnumerable<Position>>>(GeocoderBackend.GetPositionsForAddressAsync);
      Xamarin.Forms.Maps.Geocoder.GetAddressesForPositionFuncAsync = new Func<Position, Task<IEnumerable<string>>>(GeocoderBackend.GetAddressesForPositionAsync);
    }

    public static async Task<IEnumerable<Position>> GetPositionsForAddressAsync(string address)
    {
      Android.Locations.Geocoder geocoder = new Android.Locations.Geocoder(Forms.get_Context());
      IList<Address> addresses = await geocoder.GetFromLocationNameAsync(address, 5);
      return Enumerable.Select<Address, Position>((IEnumerable<Address>) addresses, (Func<Address, Position>) (p => new Position(p.Latitude, p.Longitude)));
    }

    public static async Task<IEnumerable<string>> GetAddressesForPositionAsync(Position position)
    {
      Android.Locations.Geocoder geocoder = new Android.Locations.Geocoder(Forms.get_Context());
      IList<Address> addresses = await geocoder.GetFromLocationAsync(position.Latitude, position.Longitude, 5);
      return Enumerable.Select<Address, string>((IEnumerable<Address>) addresses, (Func<Address, string>) (p => string.Join("\n", Enumerable.Select<int, string>(Enumerable.Range(0, p.MaxAddressLineIndex + 1), new Func<int, string>(p.GetAddressLine)))));
    }
  }
}