// Decompiled with JetBrains decompiler
// Type: Xamarin.Forms.Maps.MapSpan
// Assembly: Xamarin.Forms.Maps, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4AF1B23F-6E32-4C15-9B75-1B873B08868C
// Assembly location: D:\source\PegasusMission\Pegasus.Phone\packages\Xamarin.Forms.Maps.1.4.2.6359\lib\MonoAndroid10\Xamarin.Forms.Maps.dll

using System;

namespace Xamarin.Forms.Maps
{
  /// <summary>
  /// A circular region on a <see cref="T:Xamarin.Forms.Maps.Map"/>.
  /// </summary>
  /// 
  /// <remarks>
  /// To be added.
  /// </remarks>
  public sealed class MapSpan
  {
    private const double EarthRadiusKm = 6371.0;
    private const double EarthCircumferenceKm = 40030.1735920411;
    private const double MinimumRangeDegrees = 8.99321605918731E-06;
    private readonly Position center;
    private readonly double longitudeDegrees;
    private readonly double latitudeDegrees;

    /// <summary>
    /// The <see cref="T:Xamarin.Forms.Maps.Position"/> in the geographical center of the <see cref="T:Xamarin.Forms.Maps.MapSpan"/>.
    /// </summary>
    /// 
    /// <value>
    /// To be added.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public Position Center
    {
      get
      {
        return this.center;
      }
    }

    /// <summary>
    /// The degrees of longitude that are spanned by the <see cref="T:Xamarin.Forms.Maps.MapSpan"/>.
    /// </summary>
    /// 
    /// <value>
    /// The number of degrees of longitude that are spanned by the <see cref="T:Xamarin.Forms.Maps.MapSpan"/>.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public double LongitudeDegrees
    {
      get
      {
        return this.longitudeDegrees;
      }
    }

    /// <summary>
    /// The degrees of latitude that are spanned by the <see cref="T:Xamarin.Forms.Maps.MapSpan"/>.
    /// </summary>
    /// 
    /// <value>
    /// The number of degrees of latitude that are spanned by the <see cref="T:Xamarin.Forms.Maps.MapSpan"/>.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public double LatitudeDegrees
    {
      get
      {
        return this.latitudeDegrees;
      }
    }

    /// <summary>
    /// Gets the smallest distance across the <see cref="T:Xamarin.Forms.Maps.MapSpan"/>.
    /// </summary>
    /// 
    /// <value>
    /// The smallest distance across the <see cref="T:Xamarin.Forms.Maps.MapSpan"/>.
    /// </value>
    /// 
    /// <remarks>
    /// Application developers can get the distance in their desired units from the returned <see cref="T:Xamarin.Forms.Distance"/> struct.
    /// </remarks>
    public Distance Radius
    {
      get
      {
        return new Distance(1000.0 * Math.Min(MapSpan.LatitudeDegreesToKm(this.LatitudeDegrees), MapSpan.LongitudeDegreesToKm(this.Center, this.LongitudeDegrees)) / 2.0);
      }
    }

    public MapSpan(Position center, double latitudeDegrees, double longitudeDegrees)
    {
      this.center = center;
      this.latitudeDegrees = Math.Min(Math.Max(latitudeDegrees, 8.99321605918731E-06), 90.0);
      this.longitudeDegrees = Math.Min(Math.Max(longitudeDegrees, 8.99321605918731E-06), 180.0);
    }

    /// <param name="left">A <see cref="T:Xamarin.Forms.Maps.MapSpan"/> to be compared.</param><param name="right">A <see cref="T:Xamarin.Forms.Maps.MapSpan"/> to be compared.</param>
    /// <summary>
    /// Whether two <see cref="T:Xamarin.Forms.Maps.MapSpan"/> have equivalent values.
    /// </summary>
    /// 
    /// <returns>
    /// <see langword="true"/> if the two <see cref="T:Xamarin.Forms.Maps.MapSpan"/>s have equivalent <see cref="P:Xamarin.Forms.Maps.MapSpan.Center"/> and <see cref="P:Xamarin.Forms.Maps.MapSpan.Radius"/> values.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static bool operator ==(MapSpan left, MapSpan right)
    {
      return object.Equals((object) left, (object) right);
    }

    /// <param name="left">A <see cref="T:Xamarin.Forms.Maps.MapSpan"/> to be compared.</param><param name="right">A <see cref="T:Xamarin.Forms.Maps.MapSpan"/> to be compared.</param>
    /// <summary>
    /// Whether two <see cref="T:Xamarin.Forms.Maps.MapSpan"/> have different values.
    /// </summary>
    /// 
    /// <returns>
    /// <see langword="true"/> if the two <see cref="T:Xamarin.Forms.Maps.MapSpan"/>s have different values for either <see cref="P:Xamarin.Forms.Maps.MapSpan.Center"/> or <see cref="P:Xamarin.Forms.Maps.MapSpan.Radius"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static bool operator !=(MapSpan left, MapSpan right)
    {
      return !object.Equals((object) left, (object) right);
    }

    private bool Equals(MapSpan other)
    {
      if (this.center.Equals((object) other.center) && this.longitudeDegrees.Equals(other.longitudeDegrees))
        return this.latitudeDegrees.Equals(other.latitudeDegrees);
      return false;
    }

    /// <param name="obj">To be added.</param>
    /// <summary>
    /// Whether <paramref name="obj"/> is a <see cref="T:Xamarin.Forms.MapsSpan"/> with identical position and radius values as this <see cref="T:Xamarin.Forms.Maps.MapSpan"/>.
    /// </summary>
    /// 
    /// <returns>
    /// <see langword="true"/> if <paramref name="obj"/> is a <see cref="T:Xamarin.Forms.MapsSpan"/> with identical position and radius values as this <see cref="T:Xamarin.Forms.Maps.MapSpan"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj))
        return false;
      if (object.ReferenceEquals((object) this, obj))
        return true;
      if (obj is MapSpan)
        return this.Equals((MapSpan) obj);
      return false;
    }

    /// <summary>
    /// The hash value for this <see cref="T:Xamarin.Forms.Maps.MapSpan"/>, based on the position and radius.
    /// </summary>
    /// 
    /// <returns>
    /// A value optimized for insertion and retrieval in hash-based data structures.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public override int GetHashCode()
    {
      return (this.center.GetHashCode() * 397 ^ this.longitudeDegrees.GetHashCode()) * 397 ^ this.latitudeDegrees.GetHashCode();
    }

    private static double LatitudeCircumferenceKm(Position position)
    {
      return 40030.1735920411 * Math.Cos(position.Latitude * 3.14159265358979 / 180.0);
    }

    private static double DistanceToLatitudeDegrees(Distance distance)
    {
      return distance.Kilometers / 40030.1735920411 * 360.0;
    }

    private static double LatitudeDegreesToKm(double latitudeDegrees)
    {
      return 40030.1735920411 * latitudeDegrees / 360.0;
    }

    private static double DistanceToLongitudeDegrees(Position position, Distance distance)
    {
      double num = MapSpan.LatitudeCircumferenceKm(position);
      return distance.Kilometers / num * 360.0;
    }

    private static double LongitudeDegreesToKm(Position position, double longitudeDegrees)
    {
      return MapSpan.LatitudeCircumferenceKm(position) * longitudeDegrees / 360.0;
    }

    /// <param name="center">To be added.</param><param name="radius">To be added.</param>
    /// <summary>
    /// Returns a <see cref="Xamarin.Forms.Maps.MapSpan"/> that displays the area that is defined by <paramref name="center"/> and <paramref name="radius"/>.
    /// </summary>
    /// 
    /// <returns>
    /// To be added.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static MapSpan FromCenterAndRadius(Position center, Distance radius)
    {
      return new MapSpan(center, 2.0 * MapSpan.DistanceToLatitudeDegrees(radius), 2.0 * MapSpan.DistanceToLongitudeDegrees(center, radius));
    }

    /// <param name="zoomFactor">The factor by which to enlarge or decrease <c>this</c>.</param>
    /// <summary>
    /// Creates a new <see cref="T:Xamarin.Forms.Maps.MapSpan"/> with the same <see cref="P:Xamarin.Forms.Maps.MapSpan.Center"/> as <c>this</c> but with a <see cref="P:Xamarin.Forms.Maps.MapSpan.Radius"/> multiplied by <paramref name="zoomFactor"/>.
    /// </summary>
    /// 
    /// <returns>
    /// To be added.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public MapSpan WithZoom(double zoomFactor)
    {
      double val2 = Math.Min(90.0 - this.center.Latitude, 90.0 + this.center.Latitude) * 2.0;
      return new MapSpan(this.Center, Math.Min(this.latitudeDegrees / zoomFactor, val2), this.longitudeDegrees / zoomFactor);
    }

    /// <param name="north">The northern limit of the new <see cref="T:Xamarin.Forms.Maps.MapSpan"/>.</param><param name="south">The southern limit of the new <see cref="T:Xamarin.Forms.Maps.MapSpan"/>.</param>
    /// <summary>
    /// Creates a new <see cref="T:Xamarin.Forms.Maps.MapSpan"/> with the same <see cref="P:Xamarin.Forms.Maps.MapSpan.LongitudeDegrees"/> as <c>this</c> and a radius defined by <paramref name="north"/> and <paramref name="south"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A new <see cref="T:Xamarin.Forms.Maps.MapSpan"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public MapSpan ClampLatitude(double north, double south)
    {
      north = Math.Min(Math.Max(north, 0.0), 90.0);
      south = Math.Max(Math.Min(south, 0.0), -90.0);
      double latitude = Math.Max(Math.Min(this.center.Latitude, north), south);
      double val2 = Math.Min(north - latitude, -south + latitude) * 2.0;
      return new MapSpan(new Position(latitude, this.center.Longitude), Math.Min(this.latitudeDegrees, val2), this.longitudeDegrees);
    }
  }
}
