﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DataDog.Tracing
{
    internal static class Util
    {
        [ThreadStatic]
        static Random _random;

        static Random Random => _random ?? (_random = new Random(Guid.NewGuid().GetHashCode()));

        public static long NewTraceId() => (long)(Random.Next() << 32) | (long)Random.Next();

        public static long NewSpanId() => (long)(Random.Next() << 32) | (long)Random.Next();

        public static long GetTimestamp() => DateTime.UtcNow.Ticks * 100; // 100 nanoseconds in a tick
    }
}