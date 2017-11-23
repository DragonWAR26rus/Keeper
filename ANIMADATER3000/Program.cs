using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Microsoft.Win32;
using System.Text;
using System.Collections;
using System.Security;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Runtime.InteropServices;
using System.IO;
using System.Reflection;
using System.Diagnostics;
using IWshRuntimeLibrary;
using System.Security.Principal;

namespace ANIMADATER3000
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            string icoName = "C:\\Keeper's directory\\icon.ico";
            string stdDirectory = "C:\\Keeper's directory";
            string stdAppDirectory = "C:\\Keeper's directory\\" + Application.ProductName + ".exe";
            string tmpfile = "C:\\Keeper's directory\\ls.kfls";
            string stdOldVerDirectory = "C:\\Keeper's directory\\Old version\\";
            string[] arguments = Environment.GetCommandLineArgs();
            bool argIsNoApp = true;
            bool InstallProcession = true;
            bool oldVersionInFile = false;

            WindowsPrincipal pricipal = new WindowsPrincipal(WindowsIdentity.GetCurrent());
            bool hasAdministrativeRight = pricipal.IsInRole(WindowsBuiltInRole.Administrator);

            if (hasAdministrativeRight == false)
            {
                ProcessStartInfo processInfo = new ProcessStartInfo(); //создаем новый процесс
                processInfo.Verb = "runas"; //в данном случае указываем, что процесс должен быть запущен с правами администратора
                processInfo.FileName = Application.ExecutablePath; //указываем исполняемый файл (программу) для запуска
                try
                {
                    Process.Start(processInfo); //пытаемся запустить процесс
                }
                catch (Win32Exception)
                {
                    //Ничего не делаем, потому что пользователь, возможно, нажал кнопку "Нет" в ответ на вопрос о запуске программы в окне предупреждения UAC (для Windows 7)
                }
                return;
                //Application.Exit(); //закрываем текущую копию программы (в любом случае, даже если пользователь отменил запуск с правами администратора в окне UAC)
            }
                // Проверка существующего файла и его версии
                if (System.IO.File.Exists(stdAppDirectory) && (Application.ExecutablePath != stdAppDirectory ))
            {
                string oldFileVersionInfo;
                string thatFileVersionInfo;
                oldFileVersionInfo = FileVersionInfo.GetVersionInfo(stdAppDirectory).ProductVersion;
                thatFileVersionInfo = Application.ProductVersion;
                int i = 0;
                do
                {
                    if (oldFileVersionInfo[i] < thatFileVersionInfo[i])
                    {
                        oldVersionInFile = true;
                        break;
                    }
                    i++;
                } while ((i < oldFileVersionInfo.Length) && (i < thatFileVersionInfo.Length));
            }

            // Проверка на первый запуск приложения

            if (!System.IO.File.Exists(tmpfile) || oldVersionInFile)
            {
                Directory.CreateDirectory(stdDirectory);
                if(!System.IO.File.Exists(tmpfile))
                    System.IO.File.Create(tmpfile);
                if (!oldVersionInFile)
                { 
                    if (MessageBox.Show("Установить приложение?" + Environment.NewLine + "(Создастся ярлык на рабочем столе и программа переместится в следующую директорию: " + stdDirectory + ")", "Установка", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        InstallProcession = false;
                }
                else
                {
                    if (MessageBox.Show("Обнаружена старая версия приложения." + Environment.NewLine + 
                        "Обновить приложение?" + Environment.NewLine +
                        " Старая версия: " + FileVersionInfo.GetVersionInfo(stdAppDirectory).ProductVersion + Environment.NewLine +
                        " Новая  версия: " + Application.ProductVersion,
                        "Установка", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.No)
                        InstallProcession = false;
                }
            } 
            else InstallProcession = false;

            // Создание папки
            if (!Directory.Exists(stdDirectory)) Directory.CreateDirectory(stdDirectory);
            // Выгрузка иконки
            if (!System.IO.File.Exists(icoName))
            {
                Stream stream = System.IO.File.Create(icoName);
                Properties.Resources.anime.Save(stream);
                stream.Close();
            }
            if (InstallProcession)
            {
                if (!System.IO.File.Exists(stdAppDirectory))
                {
                    try
                    {
                        // Перемещение .exe файл в папку
                        System.IO.File.Copy(Application.ExecutablePath, stdAppDirectory, true);
                        // Копирование .exe файла в папку
                        //System.IO.File.Copy(Application.ExecutablePath, stdAppDirectory, false);
                        // Создание ярлыка
                        WshShell shell = new WshShell();
                        string shortcutPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + @"\Keeper.lnk";
                        IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);
                        shortcut.Description = "Ярлык для Keeper";
                        shortcut.TargetPath = stdAppDirectory;
                        shortcut.Save();
                        Process.Start(stdAppDirectory, Application.ExecutablePath);
                        return;
                        // Конец создания ярлыка
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка" + Environment.NewLine + ex.Message, "Ошбика", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }
                else
                {
                    // Создание папки для старой версии
                    if(!Directory.Exists(stdOldVerDirectory)) Directory.CreateDirectory(stdOldVerDirectory);
                    // Копирование старой версии
                    System.IO.File.Copy(stdAppDirectory, stdOldVerDirectory + Application.ProductName + ".exe", true);
                    // Копирование с перезаписью
                    System.IO.File.Copy(Application.ExecutablePath, stdAppDirectory, true);
                    // Запуск .exe из папки
                    Process.Start(stdAppDirectory, Application.ExecutablePath);
                    return;
                }
            }
            
            
            if ((arguments.Length > 1))
            {
                if (arguments[1].Contains(Application.ProductName))
                {
                    try
                    {
                        Process[] myProcesses = Process.GetProcessesByName("Keeper");
                        while (myProcesses.Length > 1) ;
                        System.IO.File.Delete(arguments[1]);
                        MessageBox.Show("Внимание! Программа находится в следующей дериктории: " + Application.ExecutablePath, "Внимение!", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show("Ошибка!" + Environment.NewLine + ex.Message, "Ошибка", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    argIsNoApp = false;
                }
            }
                

            if (!FileAssociation.IsAssociated) FileAssociation.Associate("Keeper's file", icoName);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            if (arguments.Length > 1 && argIsNoApp)
                Application.Run(new Form1(arguments[1]));
            else
                Application.Run(new Form1());
        }
    }


    // Ассоциация файлов
    public class FileAssociation
    {
        private const string FILE_EXTENSION   = ".kfls";
        private const long SHCNE_ASSOCCHANGED = 0x8000000L;
        private const uint SHCNF_IDLIST       = 0x0U;
 
        public static void Associate(string description, string icon)
        {
            Registry.ClassesRoot.CreateSubKey(FILE_EXTENSION).SetValue("", Application.ProductName);
 
            if (Application.ProductName != null && Application.ProductName.Length > 0)
            {
                using (RegistryKey key = Registry.ClassesRoot.CreateSubKey(Application.ProductName))
                {
                    if (description != null)
                        key.SetValue("", description);
                        
                    if (icon != null)
                        key.CreateSubKey("DefaultIcon").SetValue("", ToShortPathName(icon));
 
                    key.CreateSubKey(@"Shell\Open\Command").SetValue("", ToShortPathName(Application.ExecutablePath) + " \"%1\"");
                }
            }
 
            SHChangeNotify(SHCNE_ASSOCCHANGED, SHCNF_IDLIST, IntPtr.Zero, IntPtr.Zero);
        }
 
        public static bool IsAssociated
        {
            get { return (Registry.ClassesRoot.OpenSubKey(FILE_EXTENSION, false) != null); }
        }
 
        public static void Remove()
        {
            Registry.ClassesRoot.DeleteSubKeyTree(FILE_EXTENSION);
            Registry.ClassesRoot.DeleteSubKeyTree(Application.ProductName);
        }
        
        [DllImport("shell32.dll", SetLastError = true)]
        private static extern void SHChangeNotify(long wEventId, uint uFlags, IntPtr dwItem1, IntPtr dwItem2);
 
        [DllImport("Kernel32.dll")]
        private static extern uint GetShortPathName(string lpszLongPath, [Out]StringBuilder lpszShortPath, uint cchBuffer);
 
        private static string ToShortPathName(string longName)
        {
            StringBuilder s = new StringBuilder(1000);
            uint iSize = (uint)s.Capacity;
            uint iRet = GetShortPathName(longName, s, iSize);
            return s.ToString();
        }
    }
}
