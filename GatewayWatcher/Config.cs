using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Win32;
using System.Security.AccessControl;
using System.Diagnostics;

namespace GatewayWatcher
{
    class Config
    {
        public static bool IsZavod = false;
        public static int PingCheckCount = 3;
        public static int PingTimeout = 100;

        public static bool Pause;
        public static bool NeedToStop;

        public static void Open()
        {
            RegistryKey reg = Registry.LocalMachine.OpenSubKey("SOFTWARE", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
            reg = reg.OpenSubKey("GatewayWatcher");
            if (reg == null)
                return;



            reg.Close();
        }
        static T ReadRegistry<T>(T DefaultValue, RegistryKey reg, string ValueName)
        {
            object Value;
            try
            {
                Value = reg.GetValue(ValueName);
                if (typeof(T) == typeof(bool))
                    Value = Convert.ToBoolean(Value);
                if (Value == null)
                    Value = DefaultValue;
            }
            catch
            {
                Value = DefaultValue;
            }

            return (T)Value;
        }

        public static void Save()
        {
            RegistryKey reg = Registry.LocalMachine.OpenSubKey("SOFTWARE", RegistryKeyPermissionCheck.ReadWriteSubTree, RegistryRights.FullControl);
            reg = reg.CreateSubKey("micSpy");



            reg.Close();
        }
        static void WriteRegistry<T>(RegistryKey reg, string ValueName, T Value)
        {
            if (typeof(T) == typeof(bool))
                reg.SetValue(ValueName, Convert.ToInt32(Value), RegistryValueKind.DWord);
            else if (typeof(T) == typeof(int))
                reg.SetValue(ValueName, Value, RegistryValueKind.DWord);
            else if (typeof(T) == typeof(long))
                reg.SetValue(ValueName, Value, RegistryValueKind.QWord);
            else if (typeof(T) == typeof(long?))
                reg.SetValue(ValueName, Value, RegistryValueKind.QWord);
            else if (typeof(T) == typeof(string))
                reg.SetValue(ValueName, Value, RegistryValueKind.String);
            else
                throw new Exception("Unknown value type " + typeof(T));

        }
    }
}
