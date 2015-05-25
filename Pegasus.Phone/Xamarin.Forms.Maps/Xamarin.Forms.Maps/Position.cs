// Decompiled with JetBrains decompiler
// Type: Xamarin.Forms.Maps.Position
// Assembly: Xamarin.Forms.Maps, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4AF1B23F-6E32-4C15-9B75-1B873B08868C
// Assembly location: D:\source\PegasusMission\Pegasus.Phone\packages\Xamarin.Forms.Maps.1.4.2.6359\lib\MonoAndroid10\Xamarin.Forms.Maps.dll

using System;

namespace Xamarin.Forms.Maps
{
  /// <summary>
  /// A struct that has a latitude and longitude, stored as doubles.
  /// </summary>
  /// 
  /// <remarks>
  /// To be added.
  /// </remarks>
  public struct Position
  {
    private readonly double latitude;
    private readonly double longitude;

    /// <summary>
    /// Gets the latitude of this position in decimal degrees.
    /// </summary>
    /// 
    /// <value>
    /// The latitude of this position in degrees, as a double. The returned value will be between -90.0 and 90.0 degrees, inclusive.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public double Latitude
    {
      get
      {
        return this.latitude;
      }
    }

    /// <summary>
    /// Gets the longitude of this position in decimal degrees.
    /// </summary>
    /// 
    /// <value>
    /// The longitude of this position in degrees, as a double. The returned value will be between -180.0 and 180.0 degrees, inclusive.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public double Longitude
    {
      get
      {
        return this.longitude;
      }
    }

    public Position(double latitude, double longitude)
    {
      this.latitude = Math.Min(Math.Max(latitude, -90.0), 90.0);
      this.longitude = Math.Min(Math.Max(longitude, -180.0), 180.0);
    }

    /// <param name="left">A <see cref="T:Xamarin.Forms.Maps.Position"/> to compare.</param><param name="right">A <see cref="T:Xamarin.Forms.Maps.Position"/> to compare.</param>
    /// <summary>
    /// Returns a Boolean value that indicates whether or not <paramref name="left"/> represents exactly the same latitude and longitude as <paramref name="right"/>.
    /// </summary>
    /// 
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> represents exactly the same latitude and longitude as <paramref name="right"/>. Otherwise, <see langword="false"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static bool operator ==(Position left, Position right)
    {
      return object.Equals((object) left, (object) right);
    }

    /// <param name="left">A <see cref="T:Xamarin.Forms.Maps.Position"/> to compare.</param><param name="right">A <see cref="T:Xamarin.Forms.Maps.Position"/> to compare.</param>
    /// <summary>
    /// Returns a Boolean value that indicates whether or not <paramref name="left"/> and <paramref name="right"/> represent different latitudes or longitudes.
    /// </summary>
    /// 
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> represent different latitudes or longitudes. Otherwise, <see langword="false"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static bool operator !=(Position left, Position right)
    {
      return !object.Equals((object) left, (object) right);
    }

    /// <param name="obj">To be added.</param>
    /// <summary>
    /// Returns a Boolean value that indicates whether or not <paramref name="obj"/> is another <see cref="T:Xamarin.Forms.Position"/> structure and represents exactly the same latitude and longitude as <see langword="this"/> one.
    /// </summary>
    /// 
    /// <returns>
    /// <see langword="true"/> if <paramref name="obj"/> is another <see cref="T:Xamarin.Forms.Position"/> structure and represents exactly the same latitude and longitude as <see langword="this"/> one. Otherwise, <see langword="false"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj) || obj.GetType() != this.GetType())
        return false;
      Position position = (Position) obj;
      if (this.latitude == position.latitude)
        return this.longitude == position.longitude;
      return false;
    }

    /// <summary>
    /// Returns the hash code for this instance.
    /// </summary>
    /// 
    /// <returns>
    /// The hash code for this instance.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public override int GetHashCode()
    {
      return this.latitude.GetHashCode() * 397 ^ this.longitude.GetHashCode();
    }
  }
}
