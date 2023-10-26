﻿using System.Collections.Generic;
using System.Linq;

namespace SqlSensei.Core
{
    public static class IEnumerableExtensions
    {
        public static IEnumerable<(T item, int index)> WithIndex<T>(this IEnumerable<T> self) => self.Select((item, index) => (item, index));
    }
}
