﻿using System.Globalization;

namespace MomNKidStore_BE.Business.VNPay
{
    public class VNPayCompare : IComparer<string>
    {
        public int Compare(string? x, string? y)
        {
            if (x == y) return 0;
            if (x == null) return -1;
            if (y == null) return 1;
            var vnpayCompare = CompareInfo.GetCompareInfo("en-Us");
            return vnpayCompare.Compare(x, y, CompareOptions.Ordinal);
        }
    }
}
