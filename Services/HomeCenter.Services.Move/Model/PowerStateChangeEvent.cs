﻿using HomeCenter.ComponentModel.Capabilities.Constants;

namespace HomeCenter.Motion.Model
{
    public class PowerStateChangeEvent
    {
        public static string ManualSource = "Manual";
        public static string AutoSource = "Auto";

        public PowerStateChangeEvent(bool value, string eventSource)
        {
            Value = value;
            EventSource = eventSource;
        }

        public bool Value { get;}
        public string EventSource { get; }
    }
}