using MisterProtypoParser.Helpers;
using RtelLibrary.Enums;
using RtelLibrary.TableModels;
using RtelLogException;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Reflection;
using System.Security.Principal;
using System.ServiceProcess;
using System.Threading;
using System.Windows.Forms;

namespace MisterProtypoParser
{
    public partial class MainForm : Form
    {
        private const string ServiceName = "MisterLDAPParserService";
        readonly System.Windows.Forms.Timer serviceCheckTimer = new ();
        //TODO: double code
        private Dictionary<SysApplicationProcess, List<SysParameters>> processParameters = new();
        private Dictionary<SysApplicationProcess, List<SysParametersDetails>> processParameterDetails = new();

        public MainForm()
        {
            InitializeComponent();
            LogsListBox.DisplayMember = "Text";
            LogsListBox.ValueMember = "EventLevel";
            serviceCheckTimer.Interval = (60 * 1000);
            serviceCheckTimer.Tick += ServiceCheckTimer_Tick;
            serviceCheckTimer.Enabled = false;
            ApplicationsProcesses.SetMainForm(this);
        }

        public void WriteErrors(string text)
        {
            LogsListBox.Items.Add(new ListBoxItem() { EventLevel = SysEventLevel.Error, Text = $"{DateTime.Now}: {text}" });
            LogsListBox.SelectedIndex = LogsListBox.Items.Count - 1;
            Application.DoEvents();
        }

        public void WriteLogs(SysEventLevel eventLevel, string text)
        {
            LogsListBox.Items.Add(new ListBoxItem() { EventLevel = eventLevel, Text = $"{DateTime.Now}: {text}" });
            LogsListBox.SelectedIndex = LogsListBox.Items.Count - 1;
            Application.DoEvents();
        }

        public void WriteLogs(SysEventLevel eventLevel, string[] texts)
        {
            foreach (string text in texts) WriteLogs(eventLevel, text);
        }

        private void ServiceCheckTimer_Tick(object sender, EventArgs e)
        {
            CheckServiceStatus();
            serviceCheckTimer.Start();
        }

        private void CheckServiceStatus()
        {
            try
            {
                ServiceController sc = new (ServiceName);
                switch (sc.Status)
                {
                    case ServiceControllerStatus.Running:
                        serviceStatusLabel.ForeColor = System.Drawing.Color.Green;
                        break;
                    case ServiceControllerStatus.Stopped:
                    case ServiceControllerStatus.Paused:
                    case ServiceControllerStatus.StopPending:
                    case ServiceControllerStatus.StartPending:
                        serviceStatusLabel.ForeColor = System.Drawing.Color.Orange;
                        break;
                    default:
                        break;
                }
                serviceStatusLabel.Text = sc.Status.ToString();
                uninstallServiceButton.Enabled = true;
                installServiceButton.Enabled = false;
            }
            catch (Exception ex)
            {
                uninstallServiceButton.Enabled = false;
                installServiceButton.Enabled = true;
                WriteErrors(ex.Message);
                serviceStatusLabel.Text = "Not Installed";
                serviceStatusLabel.ForeColor = System.Drawing.Color.Red;
            }
        }

        private string GetServiceCreateCommand()
        {
            return $"{(AdministratorCheckBox.Checked ? "/user:Administrator" : string.Empty)} \"cmd /{(closeWindowCheckBox.Checked ?"C":"K")} sc create {ServiceName} binpath=\"{Assembly.GetExecutingAssembly().Location.Replace("dll", "exe")}\" start= auto\"";
        }

        private string GetServiceDeleteCommand()
        {
            return $"{(AdministratorCheckBox.Checked ? "/user:Administrator" : string.Empty)} \"cmd /{(closeWindowCheckBox.Checked ? "C" : "K")} sc delete {ServiceName}";
        }

        private void InstallService()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                System.Diagnostics.Process process = new ();
                System.Diagnostics.ProcessStartInfo startInfo = new ();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = GetServiceCreateCommand();
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
            }
            else
            {
                System.Diagnostics.ProcessStartInfo startInfo = new();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Application.ExecutablePath;

                startInfo.Verb = "runas";
                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                    Application.Exit();

                }
                catch (Exception ex)
                {
                    WriteErrors(ex.Message);
                }
            }
        }

        private void UnistallService()
        {
            var identity = WindowsIdentity.GetCurrent();
            var principal = new WindowsPrincipal(identity);
            if (principal.IsInRole(WindowsBuiltInRole.Administrator))
            {
                System.Diagnostics.Process process = new ();
                System.Diagnostics.ProcessStartInfo startInfo = new();
                startInfo.WindowStyle = System.Diagnostics.ProcessWindowStyle.Hidden;
                startInfo.FileName = "cmd.exe";
                startInfo.Arguments = GetServiceDeleteCommand();
                startInfo.Verb = "runas";
                process.StartInfo = startInfo;
                process.Start();
            }
            else
            {
                System.Diagnostics.ProcessStartInfo startInfo = new();
                startInfo.UseShellExecute = true;
                startInfo.WorkingDirectory = Environment.CurrentDirectory;
                startInfo.FileName = Application.ExecutablePath;

                startInfo.Verb = "runas";
                try
                {
                    System.Diagnostics.Process.Start(startInfo);
                    Application.Exit();

                }
                catch (Exception ex)
                {
                    WriteErrors(ex.Message);
                }
            }
        }

        private void CopyInstallCommandToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread t = new((ThreadStart)(() => { System.Windows.Forms.Clipboard.SetText($"sc create {ServiceName} binpath=\"{Assembly.GetExecutingAssembly().Location.Replace("dll", "exe")}\" start= auto\""); }));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        private void CopyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Thread t = new((ThreadStart)(() => { System.Windows.Forms.Clipboard.SetText($"sc delete {ServiceName}"); }));
            t.SetApartmentState(ApartmentState.STA);
            t.Start();
            t.Join();
        }

        private void InstallServiceButton_Click(object sender, EventArgs e)
        {
            InstallService();
            CheckServiceStatus();
        }

        private void UninstallServiceButton_Click(object sender, EventArgs e)
        {
            UnistallService();
            CheckServiceStatus();
        }

        private void LogsListBox_DrawItem(object sender, DrawItemEventArgs e)
        {
            e.DrawBackground();
            e.DrawFocusRectangle();
            if (LogsListBox.Items[e.Index] is ListBoxItem item)
            {
                switch (item.EventLevel)
                {
                    case SysEventLevel.Error:
                        e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(Color.Red), e.Bounds);
                        break;
                    case SysEventLevel.Information:
                        e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(Color.Blue), e.Bounds);
                        break;
                    case SysEventLevel.Warning:
                        e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(Color.Orange), e.Bounds);
                        break;
                    case SysEventLevel.Unknown:
                        e.Graphics.DrawString(item.Text, e.Font, new SolidBrush(Color.Black), e.Bounds);
                        break;
                }
            }
            else e.Graphics.DrawString(LogsListBox.Items[e.Index].ToString(), e.Font, new SolidBrush(e.ForeColor), e.Bounds);
        }

        private void LogsListBox_MeasureItem(object sender, MeasureItemEventArgs e)
        {
            int stringWidth;
            int stringHeight;
            if (LogsListBox.Items[e.Index] is ListBoxItem item)
            {
                stringWidth = (int)e.Graphics.MeasureString(item.Text, LogsListBox.Font).Width;
                stringHeight = (int)e.Graphics.MeasureString(item.Text, LogsListBox.Font).Height;
            }
            else
            {
                stringWidth = (int)e.Graphics.MeasureString(LogsListBox.Items[e.Index].ToString(), LogsListBox.Font).Width;
                stringHeight = (int)e.Graphics.MeasureString(LogsListBox.Items[e.Index].ToString(), LogsListBox.Font).Height;
            }
            int dividedWidth = stringWidth / LogsListBox.Width;
            e.ItemHeight = (LogsListBox.ItemHeight * ++dividedWidth) + stringHeight;
        }

        private void CheckNowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (serviceCheckTimer.Enabled)
            {
                serviceCheckTimer.Stop();
                CheckServiceStatus();
                serviceCheckTimer.Start();
            }
            else
            {
                CheckServiceStatus();
            }
            
        }

        private void StopstartAutoCheckToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (serviceCheckTimer.Enabled)
            {
                serviceCheckTimer.Stop();
                stopstartAutoCheckToolStripMenuItem.Text = "Start auto check";
            }
            else
            {
                serviceCheckTimer.Start();
                stopstartAutoCheckToolStripMenuItem.Text = "Stop auto check";
            }
        }

        private void MainForm_Shown(object sender, EventArgs e)
        {
            serviceCheckTimer.Start();
        }

        private void ReplicateLDAPDataButton_Click(object sender, EventArgs e)
        {
            string folderPath = AppDomain.CurrentDomain.BaseDirectory;
            string Logspath = $"{folderPath}Service.Logs.txt";
        }
    }
}
