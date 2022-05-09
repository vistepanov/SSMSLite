﻿namespace SsmsLite.Core.Ui.Utils
{
    public class InvertableBool
    {
        public bool Value { get; } = false;
        public bool Invert { get { return !Value; } }

        public InvertableBool(bool b)
        {
            Value = b;
        }

        public static implicit operator InvertableBool(bool b)
        {
            return new InvertableBool(b);
        }

        public static implicit operator bool(InvertableBool b)
        {
            return b.Value;
        }

    }
}
