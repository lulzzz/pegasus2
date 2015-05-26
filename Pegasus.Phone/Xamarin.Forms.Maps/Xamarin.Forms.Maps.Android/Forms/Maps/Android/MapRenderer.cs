// Decompiled with JetBrains decompiler
// Type: Xamarin.Forms.Maps.Android.MapRenderer
// Assembly: Xamarin.Forms.Maps.Android, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 3C53046C-F87C-472B-9B76-6FA6E84409D4
// Assembly location: D:\source\PegasusMission\Pegasus.Phone\packages\Xamarin.Forms.Maps.1.4.2.6359\lib\MonoAndroid10\Xamarin.Forms.Maps.Android.dll

using Android.Gms.Maps;
using Android.Gms.Maps.Model;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Java.Lang;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using Xamarin.Forms;
using Xamarin.Forms.Maps;
using Xamarin.Forms.Platform.Android;
using Android.Graphics;
using AGPoint = Android.Graphics.Point;

namespace Xamarin.Forms.Maps.Android
{
  public class MapRenderer : ViewRenderer, GoogleMap.IOnCameraChangeListener, IJavaObject, IDisposable
  {
    private const string MoveMessageName = "MapMoveToRegion";
    private static Bundle bundle;
    private List<Marker> markers;
    private bool init;
    private bool disposed;

    internal static Bundle Bundle
    {
      set
      {
        MapRenderer.bundle = value;
      }
    }

    protected GoogleMap NativeMap
    {
      get
      {
        return ((MapView) this.Control).Map;
      }
    }

    protected Map Map
    {
      get
      {
        return (Map) this.Element;
      }
    }

    public MapRenderer()
    {
      this.AutoPackage = (false);
    }

    public override SizeRequest GetDesiredSize(int widthConstraint, int heightConstraint)
    {
      return new SizeRequest(new Size((double) ContextExtensions.ToPixels(this.Context, 40.0), (double) ContextExtensions.ToPixels(this.Context, 40.0)));
    }

    protected override void OnElementChanged(ElementChangedEventArgs<View> e)
    {
      base.OnElementChanged(e);
      MapView mapView1 = (MapView)this.Control;
      MapView mapView2 = new MapView(this.Context);
      mapView2.OnCreate(MapRenderer.bundle);
      mapView2.OnResume();
      this.SetNativeControl(mapView2);
      if (e.OldElement != null)
      {
        ((ObservableCollection<Pin>) ((Map) e.OldElement).Pins).CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
        MessagingCenter.Unsubscribe<Map, MapSpan>((object) this, "MapMoveToRegion");
        if (mapView1.Map != null)
        {
          mapView1.Map.SetOnCameraChangeListener((GoogleMap.IOnCameraChangeListener) null);
          this.NativeMap.InfoWindowClick -= (new EventHandler<GoogleMap.InfoWindowClickEventArgs>(this.MapOnMarkerClick));
        }
        ((Java.Lang.Object) mapView1).Dispose();
      }
      GoogleMap nativeMap = this.NativeMap;
      if (nativeMap != null)
      {
        nativeMap.SetOnCameraChangeListener((GoogleMap.IOnCameraChangeListener) this);
        this.NativeMap.InfoWindowClick += (new EventHandler<GoogleMap.InfoWindowClickEventArgs>(this.MapOnMarkerClick));
        nativeMap.UiSettings.ZoomControlsEnabled = (this.Map.HasZoomEnabled);
        nativeMap.UiSettings.ZoomGesturesEnabled = (this.Map.HasZoomEnabled);
        nativeMap.UiSettings.ScrollGesturesEnabled = (this.Map.HasScrollEnabled);
        GoogleMap googleMap = nativeMap;
        bool isShowingUser;
        nativeMap.UiSettings.MyLocationButtonEnabled = (isShowingUser = this.Map.IsShowingUser);
        int num = isShowingUser ? 1 : 0;
        googleMap.MyLocationEnabled = (num != 0);
        this.SetMapType();
      }
      MessagingCenter.Subscribe<Map, MapSpan>(this, "MapMoveToRegion", new Action<Map, MapSpan>(this.OnMoveToRegionMessage), this.Map);
      INotifyCollectionChanged collectionChanged = this.Map.Pins as INotifyCollectionChanged;
      if (collectionChanged == null)
        return;
      collectionChanged.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
    }

    private void OnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
    {
      this.UpdatePins();
    }

    private void OnMoveToRegionMessage(Map s, MapSpan a)
    {
      this.MoveToRegion(a, true);
    }

    private void MoveToRegion(MapSpan span, bool animate)
    {
      GoogleMap nativeMap = this.NativeMap;
      if (nativeMap == null)
        return;
      span = span.ClampLatitude(85.0, -85.0);
      LatLng latLng = new LatLng(span.Center.Latitude + span.LatitudeDegrees / 2.0, span.Center.Longitude + span.LongitudeDegrees / 2.0);
      CameraUpdate cameraUpdate = CameraUpdateFactory.NewLatLngBounds(new LatLngBounds(new LatLng(span.Center.Latitude - span.LatitudeDegrees / 2.0, span.Center.Longitude - span.LongitudeDegrees / 2.0), latLng), 0);
      try
      {
        if (animate)
          nativeMap.AnimateCamera(cameraUpdate);
        else
          nativeMap.MoveCamera(cameraUpdate);
      }
      catch (IllegalStateException)
      {
      }
    }

    protected override void OnLayout(bool changed, int l, int t, int r, int b)
    {
      base.OnLayout(changed, l, t, r, b);
      if (!this.init)
        return;
      this.MoveToRegion(((Map) this.Element).LastMoveToRegion, false);
      this.UpdatePins();
      this.init = false;
    }

    protected override void OnElementPropertyChanged(object sender, PropertyChangedEventArgs e)
    {
      base.OnElementPropertyChanged(sender, e);
      if (e.PropertyName == Map.MapTypeProperty.PropertyName)
      {
        this.SetMapType();
      }
      else
      {
        GoogleMap nativeMap = this.NativeMap;
        if (nativeMap == null)
          return;
        if (e.PropertyName == Map.IsShowingUserProperty.PropertyName)
        {
          GoogleMap googleMap = nativeMap;
          bool isShowingUser;
          nativeMap.UiSettings.MyLocationButtonEnabled = (isShowingUser = this.Map.IsShowingUser);
          int num = isShowingUser ? 1 : 0;
          googleMap.MyLocationEnabled = (num != 0);
        }
        else if (e.PropertyName == Map.HasScrollEnabledProperty.PropertyName)
        {
          nativeMap.UiSettings.ScrollGesturesEnabled = (this.Map.HasScrollEnabled);
        }
        else
        {
          if (!(e.PropertyName == Map.HasZoomEnabledProperty.PropertyName))
            return;
          nativeMap.UiSettings.ZoomControlsEnabled = (this.Map.HasZoomEnabled);
          nativeMap.UiSettings.ZoomGesturesEnabled = (this.Map.HasZoomEnabled);
        }
      }
    }

    private void SetMapType()
    {
      GoogleMap nativeMap = this.NativeMap;
      if (nativeMap == null)
        return;
      switch (this.Map.MapType)
      {
        case MapType.Street:
          nativeMap.MapType = (1);
          break;
        case MapType.Satellite:
          nativeMap.MapType = (2);
          break;
        case MapType.Hybrid:
          nativeMap.MapType = (4);
          break;
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public void OnCameraChange(CameraPosition pos)
    {
      GoogleMap nativeMap = this.NativeMap;
      if (nativeMap == null)
        return;
      Projection projection = nativeMap.Projection;
      int width = Control.Width;
      int height = Control.Height;
      LatLng latLng1 = projection.FromScreenLocation(new AGPoint(0, 0));
      LatLng latLng2 = projection.FromScreenLocation(new AGPoint(width, 0));
      LatLng latLng3 = projection.FromScreenLocation(new AGPoint(0, height));
      LatLng latLng4 = projection.FromScreenLocation(new AGPoint(width, height));
      double latitudeDegrees = System.Math.Max(System.Math.Abs(latLng1.Latitude - latLng4.Latitude), System.Math.Abs(latLng2.Latitude - latLng3.Latitude));
      double longitudeDegrees = System.Math.Max(System.Math.Abs(latLng1.Longitude - latLng4.Longitude), System.Math.Abs(latLng2.Longitude - latLng3.Longitude));
      ((Map) Element).VisibleRegion = new MapSpan(new Position(pos.Target.Latitude, pos.Target.Longitude), latitudeDegrees, longitudeDegrees);
    }

    private void UpdatePins()
    {
      GoogleMap map = this.NativeMap;
      if (map == null)
        return;
      if (this.markers != null)
        this.markers.ForEach((Action<Marker>) (m => m.Remove()));
      this.markers = Enumerable.ToList<Marker>(Enumerable.Select<Pin, Marker>((IEnumerable<Pin>) this.Map.Pins, (Func<Pin, Marker>) (p =>
      {
        MarkerOptions markerOptions = new MarkerOptions();
        markerOptions.SetPosition(new LatLng(p.Position.Latitude, p.Position.Longitude));
        markerOptions.SetTitle(p.Label);
        markerOptions.SetSnippet(p.Address);
        Marker marker = map.AddMarker(markerOptions);
        p.Id = (object) marker.Id;
        return marker;
      })));
    }

    private void MapOnMarkerClick(object sender, GoogleMap.InfoWindowClickEventArgs eventArgs)
    {
      Marker marker = eventArgs.Marker;
      Pin pin1 = (Pin) null;
      for (int index = 0; index < this.Map.Pins.Count; ++index)
      {
        Pin pin2 = this.Map.Pins[index];
        if (!((string) pin2.Id != marker.Id))
        {
          pin1 = pin2;
          break;
        }
      }
      if (pin1 == (Pin) null)
        return;
      pin1.SendTap();
    }

    protected override void Dispose(bool disposing)
    {
      if (disposing && !this.disposed)
      {
        this.disposed = true;
        Map map = this.Element as Map;
        if (map != null)
        {
          MessagingCenter.Unsubscribe<Map, MapSpan>((object) this, "MapMoveToRegion");
          ((ObservableCollection<Pin>) map.Pins).CollectionChanged -= new NotifyCollectionChangedEventHandler(this.OnCollectionChanged);
        }
        GoogleMap nativeMap = this.NativeMap;
        if (nativeMap == null)
          return;
        nativeMap.MyLocationEnabled = (false);
        nativeMap.InfoWindowClick -= (new EventHandler<GoogleMap.InfoWindowClickEventArgs>(this.MapOnMarkerClick));
        ((Java.Lang.Object) nativeMap).Dispose();
      }
      this.Dispose(disposing);
    }
  }
}
