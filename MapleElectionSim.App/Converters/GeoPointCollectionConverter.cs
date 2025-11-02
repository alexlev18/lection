using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace MapleElectionSim.App.Converters;

/// <summary>
/// Converts latitude/longitude tuples into a <see cref="PointCollection"/> for rendering simple polygons.
/// </summary>
public class GeoPointCollectionConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not IEnumerable<(double Latitude, double Longitude)> points)
        {
            return null;
        }

        var collection = new PointCollection(points.Select(p => new Point((p.Longitude + 150) * 5, (70 - p.Latitude) * 5)));
        return collection;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotSupportedException();
}
