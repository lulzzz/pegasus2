// Decompiled with JetBrains decompiler
// Type: Xamarin.Forms.Maps.Distance
// Assembly: Xamarin.Forms.Maps, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4AF1B23F-6E32-4C15-9B75-1B873B08868C
// Assembly location: D:\source\PegasusMission\Pegasus.Phone\packages\Xamarin.Forms.Maps.1.4.2.6359\lib\MonoAndroid10\Xamarin.Forms.Maps.dll

namespace Xamarin.Forms.Maps
{
  /// <summary>
  /// Struct that encapsulates a distance (natively stored as a double of meters).
  /// </summary>
  /// 
  /// <remarks>
  /// To be added.
  /// </remarks>
  public struct Distance
  {
    private const double MetersPerMile = 1609.34;
    private const double MetersPerKilometer = 1000.0;
    private readonly double meters;

    /// <summary>
    /// Gets the distance in meters that is spanned by <c>this</c><see cref="T:Xamarin.Forms.Maps.Distance"/>.
    /// </summary>
    /// 
    /// <value>
    /// Toe distance in meters that is spanned by <c>this</c><see cref="T:Xamarin.Forms.Maps.Distance"/>.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public double Meters
    {
      get
      {
        return this.meters;
      }
    }

    /// <summary>
    /// Gets the distance in miles that is spanned by <c>this</c><see cref="T:Xamarin.Forms.Maps.Distance"/>.
    /// </summary>
    /// 
    /// <value>
    /// The distance in miles that is spanned by <c>this</c><see cref="T:Xamarin.Forms.Maps.Distance"/>.
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public double Miles
    {
      get
      {
        return this.meters / 1609.34;
      }
    }

    /// <summary>
    /// Gets the distance in kilometers that is spanned by <c>this</c><see cref="T:Xamarin.Forms.Maps.Distance"/>.
    /// </summary>
    /// 
    /// <value>
    /// The distance in kilometers that is spanned by <c>this</c><see cref="T:Xamarin.Forms.Maps.Distance"/>
    /// </value>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public double Kilometers
    {
      get
      {
        return this.meters / 1000.0;
      }
    }

    public Distance(double meters)
    {
      this.meters = meters;
    }

    /// <param name="left">A <see cref="T:Xamarin.Forms.Maps.Distance"/> to compare to another <see cref="T:Xamarin.Forms.Maps.Distance"/>.</param><param name="right">A <see cref="T:Xamarin.Forms.Maps.Distance"/> to compare to <paramref name="left"/>.</param>
    /// <summary>
    /// Whether two <see cref="T:Xamarin.Forms.Maps.Distance"/>s have <see cref="P:Xamarin.Forms.Maps.Distance.Meters"/> properties that are exactly the same.
    /// </summary>
    /// 
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have identical <see cref="P:Xamarin.Forms.Maps.Distance.Meters"/> properties.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static bool operator ==(Distance left, Distance right)
    {
      return left.Equals(right);
    }

    /// <param name="left">A <see cref="T:Xamarin.Forms.Maps.Distance"/> to compare to another <see cref="T:Xamarin.Forms.Maps.Distance"/>.</param><param name="right">A <see cref="T:Xamarin.Forms.Maps.Distance"/> to compare to <paramref name="left"/>.</param>
    /// <summary>
    /// Whether two <see cref="T:Xamarin.Forms.Maps.Distance"/>s have differing <see cref="P:Xamarin.Forms.Maps.Distance.Meters"/> properties.
    /// </summary>
    /// 
    /// <returns>
    /// <see langword="true"/> if <paramref name="left"/> and <paramref name="right"/> have different <see cref="P:Xamarin.Forms.Maps.Distance.Meters"/> properties.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static bool operator !=(Distance left, Distance right)
    {
      return !left.Equals(right);
    }

    /// <param name="miles">A double value representing the distance, in miles, for the <see cref="T:Xamarin.Forms.Maps.Distance"/> to create.</param>
    /// <summary>
    /// Factory method to create a <see cref="T:Xamarin.Forms.Maps.Distance"/> from a value provided in miles.
    /// </summary>
    /// 
    /// <returns>
    /// To be added.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static Distance FromMiles(double miles)
    {
      if (miles < 0.0)
        miles = 0.0;
      return new Distance(miles * 1609.34);
    }

    /// <param name="meters">A double value representing the distance, in meters, for the <see cref="T:Xamarin.Forms.Maps.Distance"/> to create.</param>
    /// <summary>
    /// Factory method to create a <see cref="T:Xamarin.Forms.Maps.Distance"/> from a value provided in meters.
    /// </summary>
    /// 
    /// <returns>
    /// To be added.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static Distance FromMeters(double meters)
    {
      if (meters < 0.0)
        meters = 0.0;
      return new Distance(meters);
    }

    /// <param name="kilometers">A double value representing the distance, in kilometers, for the <see cref="T:Xamarin.Forms.Maps.Distance"/> to create.</param>
    /// <summary>
    /// Factory method to create a <see cref="T:Xamarin.Forms.Maps.Distance"/> from a value provided in kilometers.
    /// </summary>
    /// 
    /// <returns>
    /// To be added.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public static Distance FromKilometers(double kilometers)
    {
      if (kilometers < 0.0)
        kilometers = 0.0;
      return new Distance(kilometers * 1000.0);
    }

    /// <param name="other">The <see cref="T:Xamarin.Forms.Maps.Distance"/> being compared to <c>this</c><see cref="T:Xamarin.Forms.Maps.Distance"/>.</param>
    /// <summary>
    /// Whether a <see cref="T:Xamaring.Forms.Maps.Distance"/> has exactly the same values as <c>this </c><see cref="T:Xamaring.Forms.Maps.Distance"/>.
    /// </summary>
    /// 
    /// <returns>
    /// <see langword="true"/> if <paramref name="other"/> has exactly the same value as <c>this</c><see cref="T:Xamaring.Forms.Maps.Distance"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public bool Equals(Distance other)
    {
      return this.meters.Equals(other.meters);
    }

    /// <param name="obj">An Object to compare to <c>this</c>.</param>
    /// <summary>
    /// Whether an <see cref="T:System.Object"/> is a <see cref="T:Xamarin.Forms.Maps.Distance"/> and has exactly the same values as <c>this</c><see cref="T:Xamarin.Forms.Maps.Distance"/>.
    /// </summary>
    /// 
    /// <returns>
    /// <see langword="true"/> if <paramref name="obj"/> is a <see cref="T:Xamarin.Forms.Maps.Distance"/> and has exactly the same values as <c>this</c><see cref="T:Xamarin.Forms.Maps.Distance"/>.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public override bool Equals(object obj)
    {
      if (object.ReferenceEquals((object) null, obj) || !(obj is Distance))
        return false;
      return this.Equals((Distance) obj);
    }

    /// <summary>
    /// The hashcode for the <see cref="T:Xamarin.Forms.Maps.Distance"/>.
    /// </summary>
    /// 
    /// <returns>
    /// A value optimized for fast insertion and retrieval in a hash-based data structure.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public override int GetHashCode()
    {
      return this.meters.GetHashCode();
    }
  }
}
