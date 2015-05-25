// Decompiled with JetBrains decompiler
// Type: Xamarin.Forms.Maps.Pin
// Assembly: Xamarin.Forms.Maps, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4AF1B23F-6E32-4C15-9B75-1B873B08868C
// Assembly location: D:\source\PegasusMission\Pegasus.Phone\packages\Xamarin.Forms.Maps.1.4.2.6359\lib\MonoAndroid10\Xamarin.Forms.Maps.dll

using System;
using Xamarin.Forms;

namespace Xamarin.Forms.Maps
{
  /// <summary>
  /// A marker on a <see cref="T:Xamarin.Forms.Maps.Map"/>.
  /// </summary>
  /// 
  /// <remarks>
  /// 
  /// <para>
  /// A <see cref="T:Xamarin.Forms.Maps.Pin"/> must have its <see cref="P:Xamarin.Forms.Maps.Pin.Label"/> property assigned before it is added to a map. If not, a <see cref="T:System.ArgumentException"/> is thrown.
  /// </para>
  /// 
  /// <example>
  /// 
  /// <code lang="C#">
  /// <![CDATA[
  /// public static Page GetMapPage ()
  /// {
  /// 	var map = new Map (MapSpan.FromCenterAndRadius (new Position (37, -122), Distance.FromMiles (10)));
  /// 
  /// //If Label is not set, runtime exception
  /// 	var pin = new Pin () {
  /// 		Position = new Position (37, -122),
  /// 		Label = "Some Pin!"
  /// 	};
  /// 	map.Pins.Add (pin);
  /// 
  /// 	var cp = new ContentPage {
  /// 		Content = map,
  /// 	};
  /// 
  /// 	return cp;
  /// }
  /// 
  ///           ]]>
  /// </code>
  /// 
  /// </example>
  /// 
  /// </remarks>
  public sealed class Pin : BindableObject
  {
    /// <summary>
    /// Identifies the <see cref="T:Xamarin.Forms.Maps.Pin.Type"/> bindable property.
    /// </summary>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static readonly BindableProperty TypeProperty = BindableProperty.Create("Type", typeof (PinType), typeof (Pin), (object) PinType.Generic, (BindingMode) 2, (BindableProperty.ValidateValueDelegate) null, (BindableProperty.BindingPropertyChangedDelegate) null, (BindableProperty.BindingPropertyChangingDelegate) null, (BindableProperty.CoerceValueDelegate) null, (BindableProperty.CreateDefaultValueDelegate) null);
    /// <summary>
    /// Identifies the <see cref="P:Xamarin.Forms.Maps.Pin.Position"/> bindable property.
    /// </summary>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static readonly BindableProperty PositionProperty = BindableProperty.Create("Position", typeof (Position), typeof (Pin), (object) new Position(), (BindingMode) 2, (BindableProperty.ValidateValueDelegate) null, (BindableProperty.BindingPropertyChangedDelegate) null, (BindableProperty.BindingPropertyChangingDelegate) null, (BindableProperty.CoerceValueDelegate) null, (BindableProperty.CreateDefaultValueDelegate) null);
    /// <summary>
    /// Identifies the <see cref="P:Xamarin.Forms.Maps.Pin.Address"/> bindable property.
    /// </summary>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static readonly BindableProperty AddressProperty = BindableProperty.Create("Address", typeof (string), typeof (Pin), (object) null, (BindingMode) 2, (BindableProperty.ValidateValueDelegate) null, (BindableProperty.BindingPropertyChangedDelegate) null, (BindableProperty.BindingPropertyChangingDelegate) null, (BindableProperty.CoerceValueDelegate) null, (BindableProperty.CreateDefaultValueDelegate) null);
    private object id;
    private string label;

    internal object Id
    {
      get
      {
        return this.id;
      }
      set
      {
        this.id = value;
      }
    }

    /// <summary>
    /// The kind of pin.
    /// </summary>
    /// 
    /// <value>
    /// To be added.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public PinType Type
    {
      get
      {
        return (PinType) this.GetValue(Pin.TypeProperty);
      }
      set
      {
        this.SetValue(Pin.TypeProperty, (object) value);
      }
    }

    /// <summary>
    /// The latitude and longitude of the <see cref="T:Xamarin.Forms.Maps.Pin"/>.
    /// </summary>
    /// 
    /// <value>
    /// The <see cref="T:Xamarin.Forms.Maps.Position"/> of the <see cref="T:Xamarin.Forms.Maps.Pin"/>.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public Position Position
    {
      get
      {
        return (Position) this.GetValue(Pin.PositionProperty);
      }
      set
      {
        this.SetValue(Pin.PositionProperty, (object) value);
      }
    }

    /// <summary>
    /// A user-readable <langword see="string"/> associated with the <see cref="T:Xamarin.Forms.Maps.Pin"/>.
    /// </summary>
    /// 
    /// <value>
    /// To be added.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public string Label
    {
      get
      {
        return this.label;
      }
      set
      {
        if (this.label == value)
          return;
        this.label = value;
        this.OnPropertyChanged("Label");
      }
    }

    /// <summary>
    /// A <langword see="string"/> describing the street address.
    /// </summary>
    /// 
    /// <value>
    /// To be added.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public string Address
    {
      get
      {
        return (string) this.GetValue(Pin.AddressProperty);
      }
      set
      {
        this.SetValue(Pin.AddressProperty, (object) value);
      }
    }

    /// <summary>
    /// Event that is raised when the pin is clicked.
    /// </summary>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public event EventHandler Clicked;

    public Pin()
    {
      base.\u002Ector();
    }

    /// <param name="left">A <see cref="T:Xamarin.Forms.Maps.Pin"/> to be compared.</param><param name="right">A <see cref="T:Xamarin.Forms.Maps.Pin"/> to be compared.</param>
    /// <summary>
    /// Whether <paramref name="left"/> and <paramref name="right"/> have equivalent <see cref="P:Xamarin.Forms.Maps.Pin.Type"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Position"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Label"/> and <see cref="P:Xamarin.Forms.Maps.Pin.Address"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have equivalent <see cref="P:Xamarin.Forms.Maps.Pin.Type"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Position"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Label"/> and <see cref="P:Xamarin.Forms.Maps.Pin.Address"/> values.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static bool operator ==(Pin left, Pin right)
    {
      return object.Equals((object) left, (object) right);
    }

    /// <param name="left">A <see cref="T:Xamarin.Forms.Maps.Pin"/> to be compared.</param><param name="right">A <see cref="T:Xamarin.Forms.Maps.Pin"/> to be compared.</param>
    /// <summary>
    /// Whether <paramref name="left"/> and <paramref name="right"/> differ in any of their <see cref="P:Xamarin.Forms.Maps.Pin.Type"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Position"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Label"/> and <see cref="P:Xamarin.Forms.Maps.Pin.Address"/> values.
    /// </summary>
    /// 
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have any differences between their <see cref="P:Xamarin.Forms.Maps.Pin.Type"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Position"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Label"/> and <see cref="P:Xamarin.Forms.Maps.Pin.Address"/> values.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static bool operator !=(Pin left, Pin right)
    {
      return !object.Equals((object) left, (object) right);
    }

    private bool Equals(Pin other)
    {
      if (string.Equals(this.Label, other.Label) && object.Equals((object) this.Position, (object) other.Position) && this.Type == other.Type)
        return string.Equals(this.Address, other.Address);
      return false;
    }

    /// <param name="obj">To be added.</param>
    /// <summary>
    /// Whether <paramref name="obj"/> with equivalent <see cref="P:Xamarin.Forms.Maps.Pin.Type"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Position"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Label"/> and <see cref="P:Xamarin.Forms.Maps.Pin.Address"/>.
    /// </summary>
    /// 
    /// <returns>
    /// <see langword="true"/> if <paramref name="obj"/> is a <see cref="T:Xamarin.Forms.Maps.Pin"/> with equivalent <see cref="P:Xamarin.Forms.Maps.Pin.Type"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Position"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Label"/> and <see cref="P:Xamarin.Forms.Maps.Pin.Address"/> values.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public virtual bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      if (obj.GetType() != ((object) this).GetType())
        return false;
      return this.Equals((Pin) obj);
    }

    /// <summary>
    /// The hash value for the <see cref="T:Xamarin.Forms.Maps.Pin"/>
    /// </summary>
    /// 
    /// <returns>
    /// A value optimized for fast insertion and retrieval in a hash-based data structure.
    /// </returns>
    /// 
    /// <remarks>
    /// 
    /// <para>
    /// A <see cref="T:Xamarin.Forms.Maps.Pin"/> hash-value is based on its <see cref="P:Xamarin.Forms.Maps.Pin.Type"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Position"/>, <see cref="P:Xamarin.Forms.Maps.Pin.Label"/> and <see cref="P:Xamarin.Forms.Maps.Pin.Address"/> properties.
    /// </para>
    /// 
    /// </remarks>
    public virtual int GetHashCode()
    {
      return (int) ((PinType) (((this.Label != null ? this.Label.GetHashCode() : 0) * 397 ^ this.Position.GetHashCode()) * 397) ^ this.Type) * 397 ^ (this.Address != null ? this.Address.GetHashCode() : 0);
    }

    internal bool SendTap()
    {
      EventHandler eventHandler = this.Clicked;
      if (eventHandler == null)
        return false;
      eventHandler((object) this, EventArgs.Empty);
      return true;
    }
  }
}
