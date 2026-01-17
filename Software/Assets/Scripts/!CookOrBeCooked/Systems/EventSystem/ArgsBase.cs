using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CookOrBeCooked.Systems.EventSystem
{
    public class ArgsBase
    {
    }

    public class IntArgs : ArgsBase
    {
        public int value { get; set; }

        public IntArgs(int value)
        {
            this.value = value;
        }
    }

    public class FloatArgs : ArgsBase
    {
        public float value { get; set; }

        public FloatArgs(float value)
        {
            this.value = value;
        }
    }

    public class DoubleArgs : ArgsBase
    {
        public double value { get; set; }

        public DoubleArgs(double value)
        {
            this.value = value;
        }
    }

    public class StringArgs : ArgsBase
    {
        public string value { get; set; }

        public StringArgs(string value)
        {
            this.value = value;
        }
    }

    public class BoolArgs : ArgsBase
    {
        public bool value { get; set; }

        public BoolArgs(bool value)
        {
            this.value = value;
        }
    }

    public class CustomEventArgs : ArgsBase
    {
        public Dictionary<string, object> values { get; set; }

        public CustomEventArgs(Dictionary<string, object> values)
        {
            this.values = values;
        }
    }

    public class GenericEventArgs<T> : ArgsBase
    {
        public T value { get; private set; }
        public GenericEventArgs(T value) : base()
        {
            this.value = value;
        }
    }
}
