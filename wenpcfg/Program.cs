using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Diagnostics;
using Microsoft.Win32;
using System.Runtime.InteropServices;


namespace Sinn.Wenpcfg
{
    class Program
    {
        private static bool is64bit = false;

        private const string acl_full = "D:PAR(A;CI;KA;;;BA)(A;CIIO;KA;;;CO)(A;CI;KA;;;SY)(A;CI;KR;;;BU)";
        private const string acl_read = "D:PAR(A;CIIO;KA;;;CO)(A;CI;KR;;;SY)(A;CI;KR;;;BA)(A;CI;KR;;;BU)";

        private const string libraries_hkcr = @"CLASSES_ROOT\CLSID\{031E4825-7B94-4dc3-B131-E946B44C8DD5}\ShellFolder";
        private const string libraries_hkcr_syswow64 = @"CLASSES_ROOT\Wow6432Node\CLSID\{031E4825-7B94-4dc3-B131-E946B44C8DD5}\ShellFolder";
        private const string libraries_hklm = @"SOFTWARE\Classes\CLSID\{031E4825-7B94-4dc3-B131-E946B44C8DD5}\ShellFolder";
        private const string libraries_hklm_classes_syswow64 = @"SOFTWARE\Classes\Wow6432Node\CLSID\{031E4825-7B94-4dc3-B131-E946B44C8DD5}\ShellFolder";
        private const string libraries_hklm_syswow64 = @"SOFTWARE\Wow6432Node\Classes\CLSID\{031E4825-7B94-4dc3-B131-E946B44C8DD5}\ShellFolder";
        private const uint libraries_attribute_on = 2961178893;  // b080010d
        private const uint libraries_attribute_off = 2962227469; // b090010d

        private const string computer_hkcr = @"CLASSES_ROOT\CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\ShellFolder";
        private const string computer_hkcr_syswow64 = @"CLASSES_ROOT\Wow6432Node\CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\ShellFolder";
        private const string computer_hklm = @"SOFTWARE\Classes\CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\ShellFolder";
        private const string computer_hklm_classes_syswow64 = @"SOFTWARE\Classes\Wow6432Node\CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\ShellFolder";
        private const string computer_hklm_syswow64 = @"SOFTWARE\Wow6432Node\Classes\CLSID\{20D04FE0-3AEA-1069-A2D8-08002B30309D}\ShellFolder";
        private const uint computer_attribute_on = 0; // DELETE
        private const uint computer_attribute_off = 2962489612; //B094010C

        private const string favorites_hkcr = @"CLASSES_ROOT\CLSID\{323CA680-C24D-4099-B94D-446DD2D7249E}\ShellFolder";
        private const string favorites_hkcr_syswow64 = @"CLASSES_ROOT\Wow6432Node\CLSID\{323CA680-C24D-4099-B94D-446DD2D7249E}\ShellFolder";
        private const string favorites_hklm = @"SOFTWARE\Classes\CLSID\{323CA680-C24D-4099-B94D-446DD2D7249E}\ShellFolder";
        private const string favorites_hklm_classes_syswow64 = @"SOFTWARE\Classes\Wow6432Node\CLSID\{323CA680-C24D-4099-B94D-446DD2D7249E}\ShellFolder";
        private const string favorites_hklm_syswow64 = @"SOFTWARE\Wow6432Node\Classes\CLSID\{323CA680-C24D-4099-B94D-446DD2D7249E}\ShellFolder";
        private const uint favorites_attribute_on = 2693792000;  // a0900100
        private const uint favorites_attribute_off = 2839544064; // a9400100
        
        private const string network_hkcr = @"CLASSES_ROOT\CLSID\{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}\ShellFolder";
        private const string network_hkcr_syswow64 = @"CLASSES_ROOT\Wow6432Node\CLSID\{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}\ShellFolder";
        private const string network_hklm = @"SOFTWARE\Classes\CLSID\{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}\ShellFolder";
        private const string network_hklm_classes_syswow64 = @"SOFTWARE\Classes\Wow6432Node\CLSID\{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}\ShellFolder";
        private const string network_hklm_syswow64 = @"SOFTWARE\Wow6432Node\Classes\CLSID\{F02C1A0D-BE21-4350-88B0-7367FC96EF3C}\ShellFolder";
        private const uint network_attribute_on = 2953052260;    // b0040064
        private const uint network_attribute_off = 2962489444;   // b0940064

        private const string homegroup_hkcr = @"CLASSES_ROOT\CLSID\{B4FB3F98-C1EA-428d-A78A-D1F5659CBA93}\ShellFolder";
        private const string homegroup_hkcr_syswow64 = @"CLASSES_ROOT\Wow6432Node\CLSID\{B4FB3F98-C1EA-428d-A78A-D1F5659CBA93}\ShellFolder";
        private const string homegroup_hklm = @"SOFTWARE\Classes\CLSID\{B4FB3F98-C1EA-428d-A78A-D1F5659CBA93}\ShellFolder";
        private const string homegroup_hklm_classes_syswow64 = @"SOFTWARE\Classes\Wow6432Node\CLSID\{B4FB3F98-C1EA-428d-A78A-D1F5659CBA93}\ShellFolder";
        private const string homegroup_hklm_syswow64 = @"SOFTWARE\Wow6432Node\Classes\CLSID\{B4FB3F98-C1EA-428d-A78A-D1F5659CBA93}\ShellFolder";
        private const uint homegroup_attribute_on = 2961441036;    // b084010c
        private const uint homegroup_attribute_off = 2962489612;   // b094010c

        private const string userfiles_hkcr = @"CLASSES_ROOT\CLSID\{59031a47-3f72-44a7-89c5-5595fe6b30ee}\ShellFolder";
        private const string userfiles_hkcr_syswow64 = @"CLASSES_ROOT\Wow6432Node\CLSID\{59031a47-3f72-44a7-89c5-5595fe6b30ee}\ShellFolder";
        private const string userfiles_hkcr_subkey = @"CLSID\{59031a47-3f72-44a7-89c5-5595fe6b30ee}\ShellFolder";
        private const string userfiles_hkcr_syswow64_subkey = @"Wow6432Node\CLSID\{59031a47-3f72-44a7-89c5-5595fe6b30ee}\ShellFolder";
        private const uint userfiles_attribute_on = 4035182893;      // f084012d
        private const uint userfiles_attribute_off = 4036231469;     // f094012d


        // Windows 10
        private const string onedrive_hkcr = @"CLASSES_ROOT\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}\ShellFolder";
        private const string onedrive_hkcr_syswow64 = @"CLASSES_ROOT\Wow6432Node\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}\ShellFolder";
        private const string onedrive_hkcr_subkey = @"CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}\ShellFolder";
        private const string onedrive_hkcr_syswow64_subkey = @"Wow6432Node\CLSID\{018D5C66-4533-4307-9B53-224DE2ED1FE6}\ShellFolder";
        private const uint onedrive_attribute_on = 4034920525;      // f080004d
        private const uint onedrive_attribute_off = 4035969101;      // f090004d

        private const string quickaccess_hkcr = @"CLASSES_ROOT\CLSID\{679f85cb-0220-4080-b29b-5540cc05aab6}\ShellFolder";
        private const string quickaccess_hkcr_syswow64 = @"CLASSES_ROOT\Wow6432Node\CLSID\{679f85cb-0220-4080-b29b-5540cc05aab6}\ShellFolder";
        private const string quickaccess_hkcr_subkey = @"CLSID\{679f85cb-0220-4080-b29b-5540cc05aab6}\ShellFolder";
        private const string quickaccess_hkcr_syswow64_subkey = @"Wow6432Node\CLSID\{679f85cb-0220-4080-b29b-5540cc05aab6}\ShellFolder";
        private const uint quickaccess_attribute_on = 2685403136;      // a0100000
        private const uint quickaccess_attribute_off = 2690646016;     // a0600000
       
        static void Main(string[] args)
        {
            bool bInvalidOption = false;
            bool bHelp = false;
            bool bReboot = false;
            bool bLogoff = false;
            bool bHideLibraries = false;
            bool bHideFavorites = false;
            bool bHideNetwork = false;
            bool bHideComputer = false;
            bool bHideHomeGroup = false;
            bool bHideUserFiles = false;
            bool bHideOneDrive = false;
            bool bHideQuickAccess = false;
            bool bShowLibraries = false;
            bool bShowFavorites = false;
            bool bShowNetwork = false;
            bool bShowComputer = false;
            bool bShowHomeGroup = false;
            bool bShowUserFiles = false;
            bool bShowOneDrive = false;
            bool bShowQuickAccess = false;
                        
            if (args.Length > 0)
            {
                try
                {
                    foreach (string arg in args)
                    {
                        string cmd = arg.ToLower();
                        cmd = cmd.Replace("-", "");
                        cmd = cmd.Replace("/", "");

                        switch (cmd)
                        {
                            case "hidelibraries":
                                bHideLibraries = true;
                                Console.WriteLine("Config to hide libraries icon: \tSET");
                                break;

                            case "hidefavorites":
                                bHideFavorites = true;
                                Console.WriteLine("Config to hide favorites icon: \tSET");
                                break;

                            case "hidenetwork":
                                bHideNetwork = true;
                                Console.WriteLine("Config to hide network icon: \tSET");
                                break;

                            case "hidecomputer":
                                bHideComputer = true;
                                Console.WriteLine("Config to hide computer icon: \tSET");
                                break;

                            case "hidehomegroup":
                                bHideHomeGroup = true;
                                Console.WriteLine("Config to hide home group icon: \tSET");
                                break;

                            case "hideuserfiles":
                                bHideUserFiles = true;
                                Console.WriteLine("Config to hide user files icon: \tSET");
                                break;

                            case "hideonedrive":
                                bHideOneDrive = true;
                                Console.WriteLine("Config to hide onedrive icon: \tSET");
                                break;

                            case "hidequickaccess":
                                bHideQuickAccess = true;
                                Console.WriteLine("Config to hide quick access icon: \tSET");
                                break;

                            case "showlibraries":
                                bShowLibraries = true;
                                Console.WriteLine("Config to show libraries icon: \tSET");
                                break;

                            case "showfavorites":
                                bShowFavorites = true;
                                Console.WriteLine("Config to show favorites icon: \tSET");
                                break;

                            case "shownetwork":
                                bShowNetwork = true;
                                Console.WriteLine("Config to show network icon: \tSET");
                                break;

                            case "showcomputer":
                                bShowComputer = true;
                                Console.WriteLine("Config to show computer icon: \tSET");                                
                                break;

                            case "showhomegroup":
                                bShowHomeGroup = true;
                                Console.WriteLine("Config to show home group icon: \tSET");
                                break;

                            case "showuserfiles":
                                bShowUserFiles = true;
                                Console.WriteLine("Config to show user files icon: \tSET");
                                break;

                            case "showonedrive":
                                bShowOneDrive = true;
                                Console.WriteLine("Config to show onedrive icon: \tSET");
                                break;

                            case "showquickaccess":
                                bShowQuickAccess = true;
                                Console.WriteLine("Config to show quick access icon: \tSET");
                                break;

                            case "reboot":
                                bReboot = true;
                                Console.WriteLine("Set to reboot immediately ...");
                                break;

                            case "logoff":
                                bLogoff = true;
                                Console.WriteLine("Set to logoff immediately ...");
                                break;
                                
                            case "?":
                                bHelp = true;
                                break;
                            case "h":
                                bHelp = true;
                                break;
                            case "help":
                                bHelp = true;
                                break;

                            default:
                                bInvalidOption = true;
                                Console.WriteLine("");
                                Console.WriteLine("ERROR: Invalid option: " + arg);
                                Console.WriteLine("");
                                Console.WriteLine("Use /? oder /help to display usage information.");                                
                                break;

                        }
                    }

                    if (bInvalidOption)
                        return;
                    if (bHelp)
                    {
                        DisplayHelp();
                        return;
                    }


                    SetOSInfo();
                    if (is64bit)
                    {
                        Console.WriteLine("Running in 64-bit process...");
                    }
                    else
                    {
                        Console.WriteLine("Running in 32-bit process...");
                    }



                    if (bHideLibraries)
                    {
                        HideLibraries();
                    }
                    if (bHideFavorites)
                    {
                        HideFavorites();
                    }
                    if (bHideNetwork)
                    {
                        HideNetwork();
                    }
                    if (bHideComputer)
                    {
                        HideComputer();
                    }
                    if (bHideHomeGroup)
                    {
                        HideHomeGroup();                        
                    }
                    if (bHideUserFiles)
                    {
                        HideUserFiles();
                    }
                    if (bHideOneDrive)
                    {
                        HideOneDrive();
                    }
                    if (bHideQuickAccess)
                    {
                        HideQuickAccess();
                    }

                    if (bShowLibraries)
                    {
                        ShowLibraries();
                    }
                    if (bShowFavorites)
                    {
                        ShowFavorites();
                    }
                    if (bShowNetwork)
                    {
                        ShowNetwork();
                    }
                    if (bShowComputer)
                    {
                        ShowComputer();
                    }
                    if (bShowHomeGroup)
                    {
                        ShowHomeGroup();
                    }
                    if (bShowUserFiles)
                    {
                        ShowUserFiles();
                    }
                    if (bShowOneDrive)
                    {
                        ShowOneDrive();
                    }
                    if (bShowQuickAccess)
                    {
                        ShowQuickAccess();
                    }

                    if (bLogoff && !bReboot)
                    {
                        LogoffComputer();
                    }

                    if (bReboot)
                    {
                        RebootComputer();
                    }
                    else
                    {
                        Console.WriteLine("######################################################################");
                        Console.WriteLine("NOTE: In order to changes take effect you have to logoff an re-logon.");
                        Console.WriteLine("######################################################################");
                    }
                    

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            }
            else
            {
                DisplayHelp();
            }
        }

        private static void SetOSInfo()
        {
            // http://stackoverflow.com/questions/336633/how-to-detect-windows-64-bit-platform-with-net

            if (IntPtr.Size == 8)
            {
                is64bit = true;
            }
        }

        private static void DisplayHelp()
        {
            Console.WriteLine("");
            Console.WriteLine("Hides or shows the icons of libraries, favorites, network and computer in the navigation pane of Windows Explorer and also in Open- and SaveFileDialogs introduced in Windows 7 and Server 2008 R2.");
            Console.WriteLine("After applying configuration changes you have to restart Explorer shell process (not only an explorer window).  Simply logoff and logon your windows session.");   

            Console.WriteLine("");
            Console.WriteLine("USAGE:");
            Console.WriteLine("\tWENPCFG [option [option] ... ]");
            Console.WriteLine("");
            Console.WriteLine("options:");
            Console.WriteLine("\t/HideLibraries \tHide libraries icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer.");

            Console.WriteLine("\t/HideFavorites \tHide favorites icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer.");

            Console.WriteLine("\t/HideNetwork \tHide network icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer.");

            Console.WriteLine("\t/HideComputer \tHide computer icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer.");

            Console.WriteLine("\t/HideHomeGroup \tHide home group icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer.");

            Console.WriteLine("\t/HideUserFiles \tHide user files icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer.");

            Console.WriteLine("\t/HideOneDrive \tHide OnDrive icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer (Experimental for Windows 10).");

            Console.WriteLine("\t/HideQuickAccess \tHide quick access icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer (Experimental for Windows 10).");

            Console.WriteLine("\t/ShowLibraries \tShow libraries icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer.");

            Console.WriteLine("\t/ShowFavorites \tShow favorites icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer.");

            Console.WriteLine("\t/ShowNetwork \tShow network icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer.");

            Console.WriteLine("\t/ShowComputer \tShow computer icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer.");

            Console.WriteLine("\t/ShowHomeGroup \tShow home group icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer.");

            Console.WriteLine("\t/ShowUserFiles \tShow user files icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer.");

            Console.WriteLine("\t/ShowOneDrive \tShow OneDrive icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer (Experimental for Windows 10).");

            Console.WriteLine("\t/ShowQuickAccess \tShow quick access icon in the navigation pane");
            Console.WriteLine("\t\t\tof Windows Explorer (Experimental for Windows 10).");

            Console.WriteLine("\t/Logoff \tIn order to changes take effect the explorer");
            Console.WriteLine("\t\t\tshell process needs to be restarted. Specifying this");
            Console.WriteLine("\t\t\toption causes your windows session to logoff immediately.");

            Console.WriteLine("\t/Reboot \tIn order to changes take effect the explorer");
            Console.WriteLine("\t\t\tshell process needs to be restarted. Specifying this");
            Console.WriteLine("\t\t\toption causes the computer to reboot immediately.");

            Console.WriteLine("\t/Help \t\tDisplays this usage information.");
            Console.WriteLine("");
            Console.WriteLine("Samples:");
            Console.WriteLine("");
            Console.WriteLine("\tHide libraries and network icon, keep other icons untouched:");
            Console.WriteLine("");
            Console.WriteLine("\t> WENPCFG /HideLibraries /HideNetwork");
            Console.WriteLine("");
            Console.WriteLine("\tShow computer icon, hide network icon and favorites, keep");
            Console.WriteLine("\tlibraries untouched and immediately reboot:");
            Console.WriteLine("");
            Console.WriteLine("\t> WENPCFG /ShowComputer /HideNetwork /HideFavorites /Logoff");
            Console.WriteLine("");
            Console.WriteLine("");
            Console.WriteLine("Author:");
            Console.WriteLine("\tAndreas Fleischmann (andreas.fleischmann@prianto.com)");
            Console.WriteLine("");


        }

        private static void HideLibraries()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();
            regkeyAclFull.Add(libraries_hkcr, acl_full);
            regkeyAclRead.Add(libraries_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(libraries_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(libraries_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkey(libraries_hklm, libraries_attribute_off);
            if (is64bit)
            {
                SetRegkey(libraries_hklm_classes_syswow64, libraries_attribute_off);
                SetRegkey(libraries_hklm_syswow64, libraries_attribute_off);
            }
            ExecSecedit(regkeyAclRead);
                        
        }
        private static void HideFavorites()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();
            regkeyAclFull.Add(favorites_hkcr, acl_full);
            regkeyAclRead.Add(favorites_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(favorites_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(favorites_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkey(favorites_hklm, favorites_attribute_off);
            if (is64bit)
            {
                SetRegkey(favorites_hklm_classes_syswow64, favorites_attribute_off);
                SetRegkey(favorites_hklm_syswow64, favorites_attribute_off);
            }
            ExecSecedit(regkeyAclRead);
        }
        private static void HideNetwork()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();
            regkeyAclFull.Add(network_hkcr, acl_full);
            regkeyAclRead.Add(network_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(network_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(network_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkey(network_hklm, network_attribute_off);
            if (is64bit)
            {
                SetRegkey(network_hklm_classes_syswow64, network_attribute_off);
                SetRegkey(network_hklm_syswow64, network_attribute_off);
            }
            ExecSecedit(regkeyAclRead);
        }
        private static void HideComputer()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();
            regkeyAclFull.Add(computer_hkcr, acl_full);
            regkeyAclRead.Add(computer_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(computer_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(computer_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkey(computer_hklm, computer_attribute_off);
            if (is64bit)
            {
                SetRegkey(computer_hklm_classes_syswow64, computer_attribute_off);
                SetRegkey(computer_hklm_syswow64, computer_attribute_off);
            }
            ExecSecedit(regkeyAclRead);
        }       
        private static void HideHomeGroup()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();
                        
            regkeyAclFull.Add(homegroup_hkcr, acl_full);
            regkeyAclRead.Add(homegroup_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(homegroup_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(homegroup_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkey(homegroup_hklm, homegroup_attribute_off);
            if (is64bit)
            {
                SetRegkey(homegroup_hklm_classes_syswow64, homegroup_attribute_off);
                SetRegkey(homegroup_hklm_syswow64, homegroup_attribute_off);
            }
            ExecSecedit(regkeyAclRead);
        }
        private static void HideUserFiles()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();

            regkeyAclFull.Add(userfiles_hkcr, acl_full);
            regkeyAclRead.Add(userfiles_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(userfiles_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(userfiles_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkeyClassesRoot(userfiles_hkcr_subkey, userfiles_attribute_off);
            if (is64bit)
            {
                SetRegkeyClassesRoot(userfiles_hkcr_syswow64_subkey, userfiles_attribute_off);
            }
            ExecSecedit(regkeyAclRead);
        }

        #region Windows 10
        // Windows 10
        private static void HideOneDrive()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();

            regkeyAclFull.Add(onedrive_hkcr, acl_full);
            regkeyAclRead.Add(onedrive_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(onedrive_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(onedrive_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkeyClassesRoot(onedrive_hkcr_subkey, onedrive_attribute_off);
            if (is64bit)
            {
                SetRegkeyClassesRoot(onedrive_hkcr_syswow64_subkey, onedrive_attribute_off);
            }
            ExecSecedit(regkeyAclRead);
        }

        private static void HideQuickAccess()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();

            regkeyAclFull.Add(quickaccess_hkcr, acl_full);
            regkeyAclRead.Add(quickaccess_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(quickaccess_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(quickaccess_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkeyClassesRoot(quickaccess_hkcr_subkey, quickaccess_attribute_off);
            if (is64bit)
            {
                SetRegkeyClassesRoot(quickaccess_hkcr_syswow64_subkey, quickaccess_attribute_off);
            }
            ExecSecedit(regkeyAclRead);
        }

        private static void SetExplorerLaunchTo_Computer()
        {
            RegistryKey usersKey = Registry.Users;
            string[] userKeyNames = usersKey.GetSubKeyNames();
            foreach (string userKeyName in userKeyNames)
            {
                RegistryKey explorerKey = usersKey.OpenSubKey(userKeyName + @"\SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", true);

                if (explorerKey != null)
                {
                    explorerKey.SetValue("LaunchTo", 1, RegistryValueKind.DWord);
                    explorerKey.Close();
                }
                else
                {
                    Console.WriteLine("Warning: " + userKeyName + " not found.");
                }
            }                        
        }
        #endregion



        private static void ShowLibraries()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();
            regkeyAclFull.Add(libraries_hkcr, acl_full);
            regkeyAclRead.Add(libraries_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(libraries_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(libraries_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkey(libraries_hklm, libraries_attribute_on);
            if (is64bit)
            {
                SetRegkey(libraries_hklm_classes_syswow64, libraries_attribute_on);
                SetRegkey(libraries_hklm_syswow64, libraries_attribute_on);
            }
            ExecSecedit(regkeyAclRead);
        }
        private static void ShowFavorites()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();
            regkeyAclFull.Add(favorites_hkcr, acl_full);
            regkeyAclRead.Add(favorites_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(favorites_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(favorites_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkey(favorites_hklm, favorites_attribute_on);
            if (is64bit)
            {
                SetRegkey(favorites_hklm_classes_syswow64, favorites_attribute_on);
                SetRegkey(favorites_hklm_syswow64, favorites_attribute_on);
            }
            ExecSecedit(regkeyAclRead);
        }
        private static void ShowNetwork()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();
            regkeyAclFull.Add(network_hkcr, acl_full);
            regkeyAclRead.Add(network_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(network_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(network_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkey(network_hklm, network_attribute_on);
            if (is64bit)
            {
                SetRegkey(network_hklm_classes_syswow64, network_attribute_on);
                SetRegkey(network_hklm_syswow64, network_attribute_on);
            }
            ExecSecedit(regkeyAclRead);
        }
        private static void ShowComputer()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();
            regkeyAclFull.Add(computer_hkcr, acl_full);
            regkeyAclRead.Add(computer_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(computer_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(computer_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            DeleteRegValue(computer_hklm);
            if (is64bit)
            {
                DeleteRegValue(computer_hklm_classes_syswow64);
                DeleteRegValue(computer_hklm_syswow64);
            }
            ExecSecedit(regkeyAclRead);
        }
        private static void ShowHomeGroup()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();
            regkeyAclFull.Add(homegroup_hkcr, acl_full);
            regkeyAclRead.Add(homegroup_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(homegroup_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(homegroup_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkey(homegroup_hklm, homegroup_attribute_on);
            if (is64bit)
            {
                SetRegkey(homegroup_hklm_classes_syswow64, homegroup_attribute_on);
                SetRegkey(homegroup_hklm_syswow64, homegroup_attribute_on);
            }
            ExecSecedit(regkeyAclRead);
        }
        private static void ShowUserFiles()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();
            regkeyAclFull.Add(userfiles_hkcr, acl_full);
            regkeyAclRead.Add(userfiles_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(userfiles_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(userfiles_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkeyClassesRoot(userfiles_hkcr_subkey, userfiles_attribute_on);
            if (is64bit)
            {
                SetRegkeyClassesRoot(userfiles_hkcr_syswow64_subkey, userfiles_attribute_on);
            }
            ExecSecedit(regkeyAclRead);
        }

        #region Windows 10
        private static void ShowOneDrive()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();
            regkeyAclFull.Add(onedrive_hkcr, acl_full);
            regkeyAclRead.Add(onedrive_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(onedrive_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(onedrive_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkeyClassesRoot(onedrive_hkcr_subkey, onedrive_attribute_on);
            if (is64bit)
            {
                SetRegkeyClassesRoot(onedrive_hkcr_syswow64_subkey, onedrive_attribute_on);
            }
            ExecSecedit(regkeyAclRead);
        }
        private static void ShowQuickAccess()
        {
            Dictionary<string, string> regkeyAclFull = new Dictionary<string, string>();
            Dictionary<string, string> regkeyAclRead = new Dictionary<string, string>();
            regkeyAclFull.Add(quickaccess_hkcr, acl_full);
            regkeyAclRead.Add(quickaccess_hkcr, acl_read);
            if (is64bit)
            {
                regkeyAclFull.Add(quickaccess_hkcr_syswow64, acl_full);
                regkeyAclRead.Add(quickaccess_hkcr_syswow64, acl_read);
            }
            ExecSecedit(regkeyAclFull);
            SetRegkeyClassesRoot(quickaccess_hkcr_subkey, quickaccess_attribute_on);
            if (is64bit)
            {
                SetRegkeyClassesRoot(quickaccess_hkcr_syswow64_subkey, quickaccess_attribute_on);
            }
            ExecSecedit(regkeyAclRead);
        }
        #endregion

        private static void ExecSecedit(Dictionary<string, string> regkeyAclList)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("[Unicode]");
            sb.AppendLine("Unicode=yes");
            sb.AppendLine("[Version]");
            sb.AppendLine("signature=\"$CHICAGO$\"");
            sb.AppendLine("Revision=1");
            sb.AppendLine("[Registry Keys]");

            foreach (KeyValuePair<string, string> kvp in regkeyAclList)
            {
                string regkey = kvp.Key;
                string acl = kvp.Value;
                sb.AppendLine("\"" + regkey + "\"" + ",0," + "\"" + acl + "\"");
            }
            
            string infFile = Environment.GetEnvironmentVariable("SystemDrive") + "\\wenc.inf";
            string dummyDb = Environment.GetEnvironmentVariable("SystemDrive") + "\\wenc.db";
            File.WriteAllText(infFile, sb.ToString());

            
            Process seceditProc = new Process();
            seceditProc.StartInfo.FileName = Environment.GetEnvironmentVariable("SystemRoot") + "\\system32\\secedit.exe";
            seceditProc.StartInfo.Arguments = "/CONFIGURE /CFG " + infFile + " /DB " + dummyDb + " /OVERWRITE /AREAS REGKEYS /QUIET";
            seceditProc.Start();

            seceditProc.WaitForExit();
            seceditProc.Close();
                        
            File.Delete(infFile);
            File.Delete(dummyDb);
                      
        }

        private static void SetRegkey(string subkey, uint attributesValue)
        {
            RegistryKey shellFolderKey = Registry.LocalMachine.OpenSubKey(subkey, true);
            if (shellFolderKey != null)
            {
                shellFolderKey.SetValue("Attributes", unchecked((int)attributesValue), RegistryValueKind.DWord);
                shellFolderKey.Close();
            }
            else
            {
                Console.WriteLine("Warning: " + subkey + " not found.");
            }
        }

        private static void SetRegkeyClassesRoot(string subkey, uint attributesValue)
        {
            RegistryKey shellFolderKey = Registry.ClassesRoot.OpenSubKey(subkey, true);
            if (shellFolderKey != null)
            {
                shellFolderKey.SetValue("Attributes", unchecked((int)attributesValue), RegistryValueKind.DWord);
                shellFolderKey.Close();
            }
            else
            {
                Console.WriteLine("Warning: " + subkey + " not found.");
            }
        }

        private static void DeleteRegValue(string subkey)
        {
            RegistryKey shellFolderKey = Registry.LocalMachine.OpenSubKey(subkey,true);
            if (shellFolderKey != null)
            {
                shellFolderKey.DeleteValue("Attributes", false);
                shellFolderKey.Close();
            }
        }

        private static void DeleteRegistryClassesRootValue(string subkey)
        {
            RegistryKey shellFolderKey = Registry.ClassesRoot.OpenSubKey(subkey, true);
            if (shellFolderKey != null)
            {
                shellFolderKey.DeleteValue("Attributes", false);
                shellFolderKey.Close();
            }
        }

        private static void RebootComputer()
        {
            Org.Mentalis.Utilities.WindowsController.ExitWindows(
                Org.Mentalis.Utilities.RestartOptions.Reboot, false);
            
        }
        private static void LogoffComputer()
        {
            Org.Mentalis.Utilities.WindowsController.ExitWindows(
                Org.Mentalis.Utilities.RestartOptions.LogOff, false);

        }
    }
}
