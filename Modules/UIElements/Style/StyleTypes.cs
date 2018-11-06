// Unity C# reference source
// Copyright (c) Unity Technologies. For terms of use, see
// https://unity3d.com/legal/licenses/Unity_Reference_Only_License

using System;
using System.Collections.Generic;
using UnityEngine.UIElements.StyleSheets;

namespace UnityEngine.UIElements
{
    internal interface IStyleValue<T>
    {
        T value { get; set; }
        int specificity { get; set; }
        StyleKeyword keyword { get; set; }

        bool Apply<U>(U otherValue, StylePropertyApplyMode mode) where U : IStyleValue<T>;
    }

    public enum StyleKeyword
    {
        Undefined, // No keyword defined
        Null, // No inline style value
        Auto,
        None
    }

    internal static class StyleValueExtensions
    {
        internal static readonly int UndefinedSpecificity = 0;
        internal static readonly int InlineSpecificity = int.MaxValue;

        // Convert StyleLength to StyleFloat for IResolvedStyle
        internal static StyleFloat ToStyleFloat(this StyleLength styleLength)
        {
            return new StyleFloat(styleLength.value.value, styleLength.keyword) { specificity = styleLength.specificity };
        }

        // Convert StyleInt to StyleEnum for IComputedStyle
        internal static StyleEnum<T> ToStyleEnum<T>(this StyleInt styleInt, T value) where T : struct, IConvertible
        {
            return new StyleEnum<T>(value, styleInt.keyword) {specificity = styleInt.specificity };
        }

        internal static string DebugString<T>(this IStyleValue<T> styleValue)
        {
            return styleValue.keyword != StyleKeyword.Undefined ? $"{styleValue.keyword}" : $"{styleValue.value}";
        }

        internal static U GetSpecifiedValueOrDefault<T, U>(this T styleValue, U defaultValue) where T : IStyleValue<U>
        {
            if (styleValue.specificity > UndefinedSpecificity)
                return styleValue.value;

            return defaultValue;
        }

        internal static T GetSpecifiedValueOrDefault<T, U>(this T styleValue, T defaultValue) where T : IStyleValue<U>
        {
            if (styleValue.specificity > UndefinedSpecificity)
                return styleValue;

            return defaultValue;
        }

        internal static bool CanApply<T>(this IStyleValue<T> styleValue, int otherSpecificity, StylePropertyApplyMode mode)
        {
            switch (mode)
            {
                case StylePropertyApplyMode.Copy:
                    return true;
                case StylePropertyApplyMode.CopyIfEqualOrGreaterSpecificity:
                    return otherSpecificity >= styleValue.specificity;
                case StylePropertyApplyMode.CopyIfNotInline:
                    return styleValue.specificity < InlineSpecificity;
                default:
                    Debug.Assert(false, "Invalid mode " + mode);
                    return false;
            }
        }
    }
}