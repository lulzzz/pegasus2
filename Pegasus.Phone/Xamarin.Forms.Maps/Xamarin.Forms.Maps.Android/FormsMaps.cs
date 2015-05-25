// Decompiled with JetBrains decompiler
// Type: Xamarin.FormsMaps
// Assembly: Xamarin.Forms.Maps.Android, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C53046C-F87C-472B-9B76-6FA6E84409D4
// Assembly location: D:\source\PegasusMission\Pegasus.Phone\packages\Xamarin.Forms.Maps.1.4.2.6359\lib\MonoAndroid10\Xamarin.Forms.Maps.Android.dll

using Android.App;
using Android.Content;
using Android.Gms.Common;
using Android.Gms.Maps;
using Android.OS;
using System;
using Xamarin.Forms.Maps.Android;

namespace Xamarin
{
  public static class FormsMaps
  {
    public static bool IsInitialized { get; private set; }

    public static Context Context { get; private set; }

    public static void Init(Activity activity, Bundle bundle)
    {
      if (FormsMaps.IsInitialized)
        return;
      FormsMaps.IsInitialized = true;
      FormsMaps.Context = (Context) activity;
      MapRenderer.Bundle = bundle;
      if (GooglePlayServicesUtil.IsGooglePlayServicesAvailable(FormsMaps.Context) == 0)
      {
        try
        {
          MapsInitializer.Initialize(FormsMaps.Context);
        }
        catch (Exception ex)
        {
          Console.WriteLine("Google Play Services Not Found");
          Console.WriteLine("Exception: {0}", (object) ex.ToString());
        }
      }
      GeocoderBackend.Register(FormsMaps.Context);
    }
  }
}
