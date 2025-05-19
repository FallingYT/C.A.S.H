using System;
using System.Collections.Generic;
using System.IO;
using System.Media;
using System.Text.Json;
using System.Windows.Forms;
using System.Threading.Tasks;
using System.Security.Cryptography;
using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using Excel = Microsoft.Office.Interop.Excel;

namespace C.A.S.H
{
    public partial class Form1 : Form
    {
        private System.Windows.Forms.Timer autoSyncTimer;
        private readonly string configDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "CASH");
        private readonly string configFile;
        private NotifyIcon trayIcon;
        private ContextMenuStrip trayMenu;
        private NotifyIcon notifyIcon1;

        public Form1()
        {
            InitializeComponent();

            lstSources.AllowDrop = true;
            lstSources.DragEnter += LstSources_DragEnter;
            lstSources.DragDrop += LstSources_DragDrop;


            notifyIcon1 = new NotifyIcon();
            notifyIcon1.Icon = SystemIcons.Information; // Use built-in icon or your custom .ico
            notifyIcon1.Visible = true;

            lblUser.Text = $"User: {Environment.UserName}";

            configFile = Path.Combine(configDir, "config.json");
            SetupAutoSyncTimer();

            LoadOrCreateConfig();

            // Setup tray icon and menu
            trayMenu = new ContextMenuStrip();
            trayMenu.Items.Add("Open", null, TrayMenu_Open);
            trayMenu.Items.Add("Exit", null, TrayMenu_Exit);

            trayIcon = new NotifyIcon();
            trayIcon.Text = "C.A.S.H - Continuous Automated Syncing Helper";
            trayIcon.Icon = SystemIcons.Application; // Or provide your own icon resource
            trayIcon.ContextMenuStrip = trayMenu;
            trayIcon.Visible = true;

            trayIcon.DoubleClick += TrayIcon_DoubleClick;

            // Handle form closing event
            this.FormClosing += Form1_FormClosing;

        }

       

// Add this method to check if a file is Excel and try to save it if opened:
 
        private void LstSources_DragEnter(object sender, DragEventArgs e)
        {
            // Check if the data being dragged is a file or folder
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

                // Check if at least one path is a directory
                foreach (string path in paths)
                {
                    if (Directory.Exists(path))
                    {
                        e.Effect = DragDropEffects.Copy;
                        return;
                    }
                }
            }

            e.Effect = DragDropEffects.None;
        }

        private void LstSources_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] paths = (string[])e.Data.GetData(DataFormats.FileDrop);

                foreach (string path in paths)
                {
                    if (Directory.Exists(path))
                    {
                        if (!lstSources.Items.Contains(path))
                        {
                            lstSources.Items.Add(path);
                            UpdateStatus($"Added source folder by drag-drop: {path}");
                        }
                        else
                        {
                            UpdateStatus($"Source folder already exists: {path}");
                        }
                    }
                    else
                    {
                        UpdateStatus($"Skipped non-folder dropped item: {path}");
                    }
                }
            }
        }


        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowMainWindow();
        }

        private void TrayMenu_Open(object sender, EventArgs e)
        {
            ShowMainWindow();
        }

        private void TrayMenu_Exit(object sender, EventArgs e)
        {
            // Actually exit app
            trayIcon.Visible = false;
            Application.Exit();
        }

        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                e.Cancel = true;   // Cancel the close
                Hide();           // Hide the window
                trayIcon.ShowBalloonTip(1000, "C.A.S.H", "App is still running in the background.", ToolTipIcon.Info);
            }
        }

        private void ShowMainWindow()
        {
            Show();
            WindowState = FormWindowState.Normal;
            Activate();
        }

        private void SetupAutoSyncTimer()
        {
            autoSyncTimer = new System.Windows.Forms.Timer();
            autoSyncTimer.Interval = 5 * 60 * 1000; // 3 minutes
            autoSyncTimer.Tick += AutoSyncTimer_Tick;
        }

        private void AutoSyncTimer_Tick(object sender, EventArgs e)
        {
            btnStart_Click(null, null);
        }

        private bool IsFileLocked(string filePath)
        {
            FileStream stream = null;
            try
            {
                stream = File.Open(filePath, FileMode.Open, FileAccess.ReadWrite, FileShare.None);
                UpdateStatusThreadSafe($"{Path.GetFileName(filePath)} is not locked.");
                return false; // not locked
            }
            catch (IOException)
            {
                UpdateStatusThreadSafe($"{Path.GetFileName(filePath)} is locked.");
                return true; // locked
            }
            finally
            {
                stream?.Close();
            }
        }


        private async void btnStart_Click(object sender, EventArgs e)
        {
            string dest = txtDestination.Text;

            if (!Directory.Exists(dest))
            {
                MessageBox.Show("Destination folder does not exist!");
                return;
            }

            btnStart.Enabled = false;
            UpdateStatus("Starting copy...");

            bool anyCopied = false;

            foreach (string source in lstSources.Items)
            {
                if (Directory.Exists(source))
                {
                    bool copied = await Task.Run(() => CopyDirectoryRecursive(source, dest));
                    if (copied) anyCopied = true;
                }
                else
                {
                    UpdateStatus($"Skipped missing source: {source}");
                }
            }

            UpdateStatus("Copy completed!");

            if (anyCopied)
            {
                SystemSounds.Asterisk.Play();
                notifyIcon1.BalloonTipTitle = "C.A.S.H";
                notifyIcon1.BalloonTipText = "Copy operation finished! New files copied.";
                notifyIcon1.BalloonTipIcon = ToolTipIcon.Info;
                notifyIcon1.ShowBalloonTip(3000); // Show for 3 seconds


            }
            else
            {
                UpdateStatus("No new files copied.");
            }

            btnStart.Enabled = true;
        }

        private bool FilesAreEqual(string file1, string file2)
        {
            try
            {
                using (var fs1 = File.Open(file1, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                using (var fs2 = File.Open(file2, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    if (fs1.Length != fs2.Length)
                        return false;

                    int byte1;
                    int byte2;
                    do
                    {
                        byte1 = fs1.ReadByte();
                        byte2 = fs2.ReadByte();
                        if (byte1 != byte2)
                            return false;
                    }
                    while (byte1 != -1);
                    return true;
                }
            }
            catch (IOException ioEx)
            {
                UpdateStatusThreadSafe($"[SKIP] Cannot compare {Path.GetFileName(file1)} because it is locked or unavailable: {ioEx.Message}");
                return false; // Treat locked files as different to trigger copying or skipping based on your logic
            }
            catch (Exception ex)
            {
                UpdateStatusThreadSafe($"[ERROR] Comparing files failed for {Path.GetFileName(file1)}: {ex.Message}");
                return false;
            }
        }






        private bool CopyDirectoryRecursive(string sourceDir, string destDir)
        {
            bool copiedAny = false;

            if (!Directory.Exists(destDir))
                Directory.CreateDirectory(destDir);

            foreach (var filePath in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(filePath);
                string destFilePath = Path.Combine(destDir, fileName);

                bool shouldCopy = false;
                UpdateStatusThreadSafe($"Comparing:\n Source: {filePath}\n Dest: {destFilePath}");

                if (!File.Exists(destFilePath))
                {
                    UpdateStatusThreadSafe($"[NEW FILE] {fileName} - copying.");
                    shouldCopy = true;
                }
                else
                {
                    var srcInfo = new FileInfo(filePath);
                    var destInfo = new FileInfo(destFilePath);

                    if (srcInfo.Length != destInfo.Length)
                    {
                        UpdateStatusThreadSafe($"[SIZE DIFF] {fileName} - copying.");
                        shouldCopy = true;
                    }
                    else if (Math.Abs((srcInfo.LastWriteTimeUtc - destInfo.LastWriteTimeUtc).TotalSeconds) > 1)
                    {
                        UpdateStatusThreadSafe($"[TIME DIFF] {fileName} - copying.");
                        shouldCopy = true;
                    }
                    else if (!FilesAreEqual(filePath, destFilePath))
                    {
                        UpdateStatusThreadSafe($"[HASH DIFF] {fileName} - copying.");
                        shouldCopy = true;
                    }
                    else
                    {
                        UpdateStatusThreadSafe($"[UNCHANGED] {fileName} - skipping.");
                    }
                }

                if (shouldCopy)
                {
                    // --- Locked-file skip logic ---
                    if (IsFileLocked(filePath))
                    {
                        UpdateStatusThreadSafe($"[SKIP] {fileName} is locked. Skipping copy.");
                        continue;
                    }

                    try
                    {
                        File.Copy(filePath, destFilePath, true);
                        UpdateStatusThreadSafe($"Copied: {fileName}");
                        copiedAny = true;
                    }
                    catch (IOException ioEx)
                    {
                        UpdateStatusThreadSafe($"[WARNING] Copy failed for {fileName}: {ioEx.Message}");
                    }
                    catch (Exception ex)
                    {
                        UpdateStatusThreadSafe($"[ERROR] Failed to copy {fileName}: {ex.Message}");
                    }
                }
            }

            foreach (var subDir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(subDir);
                string destSubDir = Path.Combine(destDir, dirName);
                if (CopyDirectoryRecursive(subDir, destSubDir))
                {
                    copiedAny = true;
                }
            }

            return copiedAny;
        }




        private void UpdateStatusThreadSafe(string message)
        {
            if (txtStatus.InvokeRequired)
            {
                txtStatus.Invoke(new Action(() =>
                {
                    txtStatus.AppendText(message + Environment.NewLine);
                }));
            }
            else
            {
                txtStatus.AppendText(message + Environment.NewLine);
            }
        }

        private void UpdateStatus(string message)
        {
            txtStatus.AppendText(message + Environment.NewLine);
        }

        private void btnToggleAutoSync_Click(object sender, EventArgs e)
        {
            if (autoSyncTimer.Enabled)
            {
                autoSyncTimer.Stop();
                btnToggleAutoSync.Text = "Enable Auto-Sync";
                UpdateStatus("Auto-sync disabled.");
            }
            else
            {
                autoSyncTimer.Start();
                btnToggleAutoSync.Text = "Disable Auto-Sync";
                UpdateStatus("Auto-sync enabled.");
            }
        }

        private void btnAddSource_Click(object sender, EventArgs e)
        {
            if (PickFolder("Select Source Folder", out string path))
            {
                lstSources.Items.Add(path);
            }
        }

        private void btnBrowseDest_Click(object sender, EventArgs e)
        {
            if (PickFolder("Select Destination Folder", out string path))
            {
                txtDestination.Text = path;
            }
        }

        private void btnResetConfig_Click(object sender, EventArgs e)
        {
            var result = MessageBox.Show("This will delete your saved configuration and require a new setup. Continue?", "Reset Config", MessageBoxButtons.YesNo);
            if (result == DialogResult.Yes)
            {
                try
                {
                    if (File.Exists(configFile))
                        File.Delete(configFile);

                    lstSources.Items.Clear();
                    txtDestination.Text = "";

                    UpdateStatus("Config reset.");
                    PromptSetupConfig();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to reset config: " + ex.Message);
                }
            }
        }

        private void LoadOrCreateConfig()
        {
            if (File.Exists(configFile))
            {
                try
                {
                    var config = JsonSerializer.Deserialize<UserConfig>(File.ReadAllText(configFile)) ?? new UserConfig();

                    if (config.SourcePaths == null || config.SourcePaths.Count == 0)
                    {
                        MessageBox.Show("No source paths found. Please reconfigure.");
                        PromptSetupConfig();
                        return;
                    }

                    if (string.IsNullOrWhiteSpace(config.DestinationPath))
                    {
                        MessageBox.Show("No destination found. Please reconfigure.");
                        PromptSetupConfig();
                        return;
                    }

                    txtDestination.Text = config.DestinationPath;
                    lstSources.Items.AddRange(config.SourcePaths.ToArray());

                    UpdateStatus("Config loaded.");
                    autoSyncTimer.Start();
                    btnToggleAutoSync.Text = "Disable Auto-Sync";
                    UpdateStatus("Auto-sync started.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Failed to load config: " + ex.Message);
                    PromptSetupConfig();
                }
            }
            else
            {
                PromptSetupConfig();
            }
        }

        private void PromptSetupConfig()
        {
            MessageBox.Show("Welcome! Let's set up your copy folders.");

            lstSources.Items.Clear();

            while (true)
            {
                if (!PickFolder("Select Source Folder (Cancel to finish)", out string src))
                    break;

                lstSources.Items.Add(src);
            }

            if (PickFolder("Select Destination Folder", out string dest))
                txtDestination.Text = dest;

            var config = new UserConfig
            {
                DestinationPath = txtDestination.Text
            };

            foreach (var item in lstSources.Items)
                config.SourcePaths.Add(item.ToString());

            try
            {
                Directory.CreateDirectory(configDir);
                File.WriteAllText(configFile, JsonSerializer.Serialize(config));
                UpdateStatus("Config saved.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Failed to save config: " + ex.Message);
            }
        }

        private bool PickFolder(string description, out string path)
        {
            path = null;
            using var dlg = new FolderBrowserDialog();
            dlg.Description = description;
            if (dlg.ShowDialog() == DialogResult.OK)
            {
                path = dlg.SelectedPath;
                return true;
            }
            return false;
        }
    }

    public class UserConfig
    {
        public List<string> SourcePaths { get; set; } = new List<string>();
        public string DestinationPath { get; set; }
    }
}
