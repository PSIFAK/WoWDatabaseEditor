﻿using System.Collections.Generic;
using System.Globalization;
using System.Threading;

namespace WDE.Common.Parameters
{
    public class Parameter : GenericBaseParameter<long>
    {
        public override string ToString(long key)
        {
            if (Items != null && Items.TryGetValue(key, out var option))
                return option.Name;// + " (" + key + ")";
            return key.ToString();
        }

        public static Parameter Instance { get; } = new Parameter();
    }
    
    public class ParameterNumbered : GenericBaseParameter<long>
    {
        public override string ToString(long key, ToStringOptions options)
        {
            if (options.WithNumber)
                return ToString(key);

            if (Items != null && Items.TryGetValue(key, out var option))
                return option.Name;
            return key.ToString();
        }
        
        public override string ToString(long key)
        {
            if (Items != null && Items.TryGetValue(key, out var option))
                return option.Name + " (" + key + ")";
            return key.ToString();
        }
    }
    
    public class FloatIntParameter : Parameter
    {
        private readonly float divider;

        public FloatIntParameter(float divider)
        {
            this.divider = divider;
        }
        
        public override string ToString(long value)
        {
            return (value / divider).ToString(CultureInfo.InvariantCulture);
        }
    }

    public class FloatParameter : GenericBaseParameter<float>
    {
        public static FloatParameter Instance { get; } = new FloatParameter();
        
        public override string ToString(float value)
        {
            return value.ToString(CultureInfo.InvariantCulture);
        }
    }
    
    public class StringParameter : GenericBaseParameter<string>
    {
        public static StringParameter Instance { get; } = new StringParameter();
        
        public override string ToString(string value) => value;
    }

    public class SwitchStringParameter : GenericBaseParameter<string>
    {
        public SwitchStringParameter(Dictionary<string, SelectOption> options, string? prefix = null)
        {
            Items = options;
            Prefix = prefix;
        }

        public override string ToString(string value)
        {
            if (Items != null && Items.TryGetValue(value, out var option))
                return option.Name;
            return "unknown (" + value + ")";
        }
    }

    public class MultiSwitchStringParameter : SwitchStringParameter
    {
        public MultiSwitchStringParameter(Dictionary<string, SelectOption> options) : base(options)
        {
        }

        public override string ToString(string value)
        {
            return value;
        }
    }
    
    public class FlagParameter : Parameter
    {
        private static ThreadLocal<List<string>> temp = new ThreadLocal<List<string>>(() => new List<string>());
        
        public override string ToString(long value)
        {
            if (Items == null)
                return value.ToString();
            
            if (value == 0 && Items.TryGetValue(0, out var zero))
                return zero.Name;
            else if (value == 0)
                return "";
            
            var flags = temp.Value!;
            flags.Clear();
            
            foreach (var item in Items)
            {
                if (item.Key > 0 && (item.Key & value) == item.Key)
                {
                    flags.Add(item.Value.Name);
                    value &= ~item.Key;
                }
            }

            if (value > 0)
            {
                for (var i = 0; i < 31; ++i)
                {
                    if (((1 << i) & value) > 0)
                    {
                        flags.Add("Flag unknown " + (1 << i));
                    }
                }
            }

            return string.Join(", ", flags);
        }
    }
}