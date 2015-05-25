// Decompiled with JetBrains decompiler
// Type: Xamarin.Forms.Maps.Geocoder
// Assembly: Xamarin.Forms.Maps, Version=1.4.0.0, Culture=neutral, PublicKeyToken=null
// MVID: 4AF1B23F-6E32-4C15-9B75-1B873B08868C
// Assembly location: D:\source\PegasusMission\Pegasus.Phone\packages\Xamarin.Forms.Maps.1.4.2.6359\lib\MonoAndroid10\Xamarin.Forms.Maps.dll

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Xamarin.Forms.Maps
{
  /// <summary>
  /// Converts between string addresses and <see cref="T:Xamarin.Forms.Maps.Position"/>s.
  /// </summary>
  /// 
  /// <remarks>
  /// To be added.
  /// </remarks>
  public class Geocoder
  {
    internal static Func<string, Task<IEnumerable<Position>>> GetPositionsForAddressAsyncFunc;
    internal static Func<Position, Task<IEnumerable<string>>> GetAddressesForPositionFuncAsync;

    /// <param name="address">To be added.</param>
    /// <summary>
    /// Returns a list of positions for <paramref name="address"/>.
    /// </summary>
    /// 
    /// <returns>
    /// To be added.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public Task<IEnumerable<Position>> GetPositionsForAddressAsync(string address)
    {
      if (Geocoder.GetPositionsForAddressAsyncFunc == null)
        throw new InvalidOperationException("You MUST call Xamarin.FormsMaps.Init (); prior to using it.");
      return Geocoder.GetPositionsForAddressAsyncFunc(address);
    }

    /// <param name="position">To be added.</param>
    /// <summary>
    /// Returns the addresses near <paramref name="position"/>, asynchronously.
    /// </summary>
    /// 
    /// <returns>
    /// To be added.
    /// </returns>
    /// 
    /// <remarks>
    /// To be added.
    /// </remarks>
    public Task<IEnumerable<string>> GetAddressesForPositionAsync(Position position)
    {
      if (Geocoder.GetAddressesForPositionFuncAsync == null)
        throw new InvalidOperationException("You MUST call Xamarin.FormsMaps.Init (); prior to using it.");
      return Geocoder.GetAddressesForPositionFuncAsync(position);
    }
  }
}
