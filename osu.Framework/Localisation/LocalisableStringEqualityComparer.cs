// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System;
using System.Collections.Generic;

namespace osu.Framework.Localisation
{
    /// <summary>
    /// An equality comparer for the <see cref="LocalisableString"/> type.
    /// </summary>
    public class LocalisableStringEqualityComparer : IEqualityComparer<LocalisableString>
    {
        // ReSharper disable once InconsistentNaming (follows EqualityComparer<T>.Default)
        public static readonly LocalisableStringEqualityComparer Default = new LocalisableStringEqualityComparer();

        public bool Equals(LocalisableString x, LocalisableString y)
        {
            var xData = x.Data;
            var yData = y.Data;

            if (ReferenceEquals(null, xData) != ReferenceEquals(null, yData))
                return false;

            if (ReferenceEquals(null, xData))
                return true;

            if (x.Casing == y.Casing)
                return true;

            if (xData.GetType() != yData.GetType())
                return EqualityComparer<object>.Default.Equals(xData, yData);

            switch (xData)
            {
                case string strX:
                    return strX.Equals((string)yData, StringComparison.Ordinal);

                case ILocalisableStringData dataX:
                    return dataX.Equals((ILocalisableStringData)yData);
            }

            return false;
        }

        public int GetHashCode(LocalisableString obj) => HashCode.Combine(obj.Data?.GetType(), obj.Data);
    }
}
