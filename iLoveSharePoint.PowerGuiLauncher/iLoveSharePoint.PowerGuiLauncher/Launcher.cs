using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;
using System.IO;
using System.Diagnostics;
using System.Reflection;
using Microsoft.Win32;
using System.Text.RegularExpressions;
using WindowsInstaller;
using System.Windows.Forms;

namespace iLoveSharePoint.PowerGuiLauncher
{
    [Guid("EC98BBC7-71B5-4dd0-B0CF-269AA5F2081E")]
    [ClassInterface(ClassInterfaceType.AutoDual)]
    [ProgId("iLoveSharePoint.PowerGuiLauncher")]
    public class Launcher : ActiveXBase, IDisposable
    {
        private FileSystemWatcher watcher;
        private int changeCounter = 0;
        private string scriptEditorPath = null;
        private string scriptPath = "";
        public object OnScriptChanged { get; set; }
        public string Script { get; set; }
        public string ScriptName { get; set; }
        
        public bool IsPowerGuiInstalled
        {
            get
            {
                return scriptEditorPath != null;
            }
        }

        public string ScriptPath
        {
            get { return scriptPath; }
        }

        public Launcher()
        {
            scriptEditorPath = Helper.GetPowerGuiScriptEditorPath();
        }

        public void StartPowerGui(string script)
        {
            if (IsPowerGuiInstalled == false)
                throw new NullReferenceException("PowerGUI isn't installed!");

            watcher.EnableRaisingEvents = false;
            File.WriteAllText(scriptPath, script, Encoding.UTF8);
            Process.Start(scriptEditorPath, "\"" + scriptPath + "\"");
            watcher.EnableRaisingEvents = true;
            
        }

        public void Initialize()
        {
            if (String.IsNullOrEmpty(ScriptName))
                scriptPath = Path.GetTempFileName() + ".ps1";
            else
            {
                scriptPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.InternetCache), ScriptName);
                if (Path.HasExtension(scriptPath) == false)
                {
                    scriptPath += ".ps1";
                }
            }

            string dir = Path.GetDirectoryName(scriptPath);
            watcher = new FileSystemWatcher(dir);
            watcher.Filter = Path.GetFileName(scriptPath);
            watcher.NotifyFilter = NotifyFilters.LastWrite;
            watcher.Changed += new FileSystemEventHandler(watcher_Changed);
            watcher.EnableRaisingEvents = true;        
        }

       
        void watcher_Changed(object sender, FileSystemEventArgs e)
        {
            //PowerGUI fires the change event twice. To avoid crashes ignores the first change event.
            changeCounter++;
            if (changeCounter % 2 == 0)
            {
                try
                {
                    this.Script = File.ReadAllText(scriptPath, Encoding.UTF8);

                    if (OnScriptChanged != null)
                        OnScriptChanged.GetType().InvokeMember("", BindingFlags.InvokeMethod, null, OnScriptChanged, new object[] { this, Script });
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
            }
        }

        public void Dispose()
        {
            if (watcher != null)
            {
                watcher.Changed -= watcher_Changed;
                watcher.Dispose();
            }
        }
    }
}
