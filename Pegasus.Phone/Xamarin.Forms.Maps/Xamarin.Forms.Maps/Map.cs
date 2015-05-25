// Decompiled with JetBrains decompiler
// Type: Xamarin.Forms.Maps.Map
// Assembly: Xamarin.Forms.Maps, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4AF1B23F-6E32-4C15-9B75-1B873B08868C
// Assembly location: D:\source\PegasusMission\Pegasus.Phone\packages\Xamarin.Forms.Maps.1.4.2.6359\lib\MonoAndroid10\Xamarin.Forms.Maps.dll

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Xamarin.Forms;

namespace Xamarin.Forms.Maps
{
  /// <summary>
  /// A <see cref="T:Xamarin.Forms.View"/> that shows a map provided by a platform-specific service.
  /// </summary>
  /// 
  /// <remarks>
  /// 
  /// <para>
  /// Here is a very simple example of a <see cref="T:Xamarin.Forms.Map"/>:
  /// </para>
  /// 
  /// <example>
  /// 
  /// <code lang="C#">
  /// <![CDATA[
  /// var map = new Map (MapSpan.FromCenterAndRadius (new Position (37, -122), Distance.FromMiles (10)));
  /// 
  /// var pin = new Pin () {
  /// 	Position = new Position (37, -122),
  /// 	Label = "Some Pin!"
  /// };
  /// map.Pins.Add (pin);
  /// 
  /// var cp = new ContentPage {
  /// 	Content = map,
  /// };
  ///           ]]>
  /// </code>
  /// 
  /// </example>
  /// 
  /// <para>
  /// <img href="Pin.Label.png"/>
  /// </para>
  /// 
  /// </remarks>
  public class Map : View, IEnumerable<Pin>, IEnumerable
  {
    /// <summary>
    /// Identifies the <see cref="P:Xamarin.Forms.Maps.Map.MapType"/> bindable property.
    /// </summary>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static readonly BindableProperty MapTypeProperty = BindableProperty.Create("MapType", typeof (MapType), typeof (Map), (object) MapType.Street, (BindingMode) 2, (BindableProperty.ValidateValueDelegate) null, (BindableProperty.BindingPropertyChangedDelegate) null, (BindableProperty.BindingPropertyChangingDelegate) null, (BindableProperty.CoerceValueDelegate) null, (BindableProperty.CreateDefaultValueDelegate) null);
    /// <summary>
    /// Identifies the <see cref="P:Xamarin.Forms.Maps.Map.IsShowingUser"/> bindable property.
    /// </summary>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static readonly BindableProperty IsShowingUserProperty = BindableProperty.Create("IsShowingUser", typeof (bool), typeof (Map), (object) false, (BindingMode) 2, (BindableProperty.ValidateValueDelegate) null, (BindableProperty.BindingPropertyChangedDelegate) null, (BindableProperty.BindingPropertyChangingDelegate) null, (BindableProperty.CoerceValueDelegate) null, (BindableProperty.CreateDefaultValueDelegate) null);
    /// <summary>
    /// Identifies the <see cref="P:Xamarin.Forms.Maps.Map.HasScrollEnabled"/> bindable property.
    /// </summary>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static readonly BindableProperty HasScrollEnabledProperty = BindableProperty.Create("HasScrollEnabled", typeof (bool), typeof (Map), (object) true, (BindingMode) 2, (BindableProperty.ValidateValueDelegate) null, (BindableProperty.BindingPropertyChangedDelegate) null, (BindableProperty.BindingPropertyChangingDelegate) null, (BindableProperty.CoerceValueDelegate) null, (BindableProperty.CreateDefaultValueDelegate) null);
    /// <summary>
    /// Identifies the <see cref="P:Xamarin.Forms.Maps.Map.HasZoomEnabled"/> bindable property.
    /// </summary>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static readonly BindableProperty HasZoomEnabledProperty = BindableProperty.Create("HasZoomEnabled", typeof (bool), typeof (Map), (object) true, (BindingMode) 2, (BindableProperty.ValidateValueDelegate) null, (BindableProperty.BindingPropertyChangedDelegate) null, (BindableProperty.BindingPropertyChangingDelegate) null, (BindableProperty.CoerceValueDelegate) null, (BindableProperty.CreateDefaultValueDelegate) null);
    private readonly ObservableCollection<Pin> pins;
    private MapSpan visibleRegion;

    internal MapSpan LastMoveToRegion { get; private set; }

    /// <summary>
    /// An <see cref="T:System.Collections.IList`1"/> of the <see cref="T:Xamarin.Forms.Maps.Pin"/>s on this <see cref="T:Xamarin.Forms.Maps.Map"/>.
    /// </summary>
    /// 
    /// <value>
    /// To be added.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public IList<Pin> Pins
    {
      get
      {
        return (IList<Pin>) this.pins;
      }
    }

    /// <summary>
    /// The currently visible <see cref="T:Xamarin.Forms.Maps.MapSpan"/> of this <see cref="T:Xamarin.Forms.Maps.Map"/>.
    /// </summary>
    /// 
    /// <value>
    /// To be added.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public MapSpan VisibleRegion
    {
      get
      {
        return this.visibleRegion;
      }
      internal set
      {
        if (this.visibleRegion == value)
          return;
        if (value == (MapSpan) null)
          throw new ArgumentNullException("value");
        ((BindableObject) this).OnPropertyChanging("VisibleRegion");
        this.visibleRegion = value;
        ((BindableObject) this).OnPropertyChanged("VisibleRegion");
      }
    }

    /// <summary>
    /// The <see cref="T:Xamarin.Forms.Maps.MapType"/> display style of this <see cref="T:Xamarin.Forms.Maps.Map"/>.
    /// </summary>
    /// 
    /// <value>
    /// To be added.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public MapType MapType
    {
      get
      {
        return (MapType) ((BindableObject) this).GetValue(Map.MapTypeProperty);
      }
      set
      {
        ((BindableObject) this).SetValue(Map.MapTypeProperty, (object) value);
      }
    }

    /// <summary>
    /// Whether this <see cref="T:Xamarin.Forms.Maps.Map"/> is showing the user's current location.
    /// </summary>
    /// 
    /// <value>
    /// <see langword="true"/> if the user's current location is being displayed.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public bool IsShowingUser
    {
      get
      {
        return (bool) ((BindableObject) this).GetValue(Map.IsShowingUserProperty);
      }
      set
      {
        ((BindableObject) this).SetValue(Map.IsShowingUserProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    /// <summary>
    /// Whether this <see cref="T:Xamarin.Forms.Maps.Map"/> is allowed to scroll.
    /// </summary>
    /// 
    /// <value>
    /// <see langword="true"/> if scrolling is enabled.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public bool HasScrollEnabled
    {
      get
      {
        return (bool) ((BindableObject) this).GetValue(Map.HasScrollEnabledProperty);
      }
      set
      {
        ((BindableObject) this).SetValue(Map.HasScrollEnabledProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    /// <summary>
    /// Whether this <see cref="T:Xamarin.Forms.Maps.Map"/> is allowed to zoom.
    /// </summary>
    /// 
    /// <value>
    /// <see langword="true"/> if zooming is enabled.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public bool HasZoomEnabled
    {
      get
      {
        return (bool) ((BindableObject) this).GetValue(Map.HasZoomEnabledProperty);
      }
      set
      {
        ((BindableObject) this).SetValue(Map.HasZoomEnabledProperty, (object) (bool) (value ? 1 : 0));
      }
    }

    public Map(MapSpan region)
    {
      base.\u002Ector();
      this.LastMoveToRegion = region;
      LayoutOptions layoutOptions;
      this.set_HorizontalOptions(layoutOptions = (LayoutOptions) LayoutOptions.FillAndExpand);
      this.set_VerticalOptions(layoutOptions);
      this.pins.CollectionChanged += new NotifyCollectionChangedEventHandler(this.PinsOnCollectionChanged);
    }

    public Map()
      : this(new MapSpan(new Position(41.890202, 12.492049), 0.1, 0.1))
    {
    }

    private void PinsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
    {
      if (e.NewItems != null && Enumerable.Any<Pin>(Enumerable.Cast<Pin>((IEnumerable) e.NewItems), (Func<Pin, bool>) (pin => pin.Label == null)))
        throw new ArgumentException("Pin must have a Label to be added to a map");
    }

    /// <param name="mapSpan">The <see cref="T:Xamarin.Forms.Maps.MapSpan"/> to make visible.</param>
    /// <summary>
    /// Moves the map so that it displays the specified <paramref name="mapSpan"/>.
    /// </summary>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public void MoveToRegion(MapSpan mapSpan)
    {
      if (mapSpan == (MapSpan) null)
        throw new ArgumentNullException("mapSpan");
      this.LastMoveToRegion = mapSpan;
      MessagingCenter.Send<Map, MapSpan>((M0) this, "MapMoveToRegion", (M1) mapSpan);
    }

    /// <summary>
    /// An <see cref="T:System.Collections.Generic.IEnumerator`1"/> of the <see cref="T:Xamarin.Forms.Maps.Pin"/>s on this <see cref="T:Xamarin.Forms.Maps.Map"/>.
    /// </summary>
    /// 
    /// <returns>
    /// To be added.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public IEnumerator<Pin> GetEnumerator()
    {
      return this.pins.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return (IEnumerator) this.GetEnumerator();
    }
  }
}
