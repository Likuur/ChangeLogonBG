using System;
using System.Runtime.InteropServices;
using Microsoft.Win32;

namespace ChangeLogonBG
{
    class Program
    {
        [DllImport("kernel32.dll")]
        static extern IntPtr GetConsoleWindow();

        [DllImport("user32.dll")]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        
        static void Main(string[] args)
        {
            string logonnewbgpath = string.Empty;
            bool hideConsole = false;

            for (int i = 0; i < args.Length; i++)
            {
                if (args[i] == "--bgpath" && i + 1 < args.Length)
                    logonnewbgpath = args[i + 1];

                else if (args[i] == "--noconsolewindow")
                    hideConsole = true;
            }

            if (hideConsole)
                ShowWindow(GetConsoleWindow(), 0); //0 - SW_HIDE

            if (IsStringEmpty(logonnewbgpath))
                Environment.Exit(1);

            using (RegistryKey personalizationcsp = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\PersonalizationCSP"))
            using (RegistryKey personalization = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Policies\Microsoft\Windows\Personalization"))
            {
                personalization.SetValue("LockScreenImage", logonnewbgpath, RegistryValueKind.String); //set logon bg
                personalization.SetValue("NoChangingLockScreen", 1, RegistryValueKind.DWord); //disallow changing bg

                personalizationcsp.SetValue("LockScreenImageStatus", 1, RegistryValueKind.DWord); //set img status (1 - active, 0 - inactive)
                personalizationcsp.SetValue("LockScreenImagePath", logonnewbgpath, RegistryValueKind.String);
                personalizationcsp.SetValue("LockScreenImageUrl", logonnewbgpath, RegistryValueKind.String);
            }

            Environment.Exit(0);
        }
        static bool IsStringEmpty(string str) => string.IsNullOrEmpty(str) && string.IsNullOrWhiteSpace(str);
    }
}
