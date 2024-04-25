using System;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Windows.Forms;

namespace Shadowrun_Launcher
{
    public partial class MainForm : Form
    {
        private string rootPath;
        private string gameZip;
        private string gameExe;
        private LauncherStatus _status;

        public MainForm()
        {
            InitializeComponent();
            rootPath = Directory.GetCurrentDirectory();
            gameZip = Path.Combine(rootPath, "Build.zip");
            gameExe = Path.Combine(rootPath, "Build", "Pirate Game.exe");
        }

        private void PlayButton_Click(object sender, System.EventArgs e)
        {
            if (File.Exists(gameExe))
            {
                ProcessStartInfo startInfo = new ProcessStartInfo(gameExe);
                startInfo.WorkingDirectory = Path.Combine(rootPath, "Build");
                Process.Start(startInfo);

                Close();
            }
            else
            {
                CheckForUpdates();
            }
        }
        private void CheckForUpdates()
        {
            string version = Path.Combine(rootPath, "Version.txt");
            if (File.Exists(version))
            {
                Version localVersion = new Version(File.ReadAllText(version));
                try
                {
                    WebClient webClient = new WebClient();
                    Version onlineVersion = new Version(webClient.DownloadString("https://drive.google.com/uc?export=download&id=1R3GT_VINzmNoXKtvnvuJw6C86-k3Jr5s"));

                    if (onlineVersion > localVersion)
                    {
                        InstallGameFiles(true, onlineVersion);
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error checking for game updates: {ex}");
                }
            }
            else
            {
                InstallGameFiles(false, Version.Parse(version));
            }
        }

        private void InstallGameFiles(bool _isUpdate, Version _onlineVersion)
        {
            try
            {
                WebClient webClient = new WebClient();
                if (!_isUpdate)
                {
                    _onlineVersion = new Version(webClient.DownloadString("https://drive.google.com/uc?export=download&id=1R3GT_VINzmNoXKtvnvuJw6C86-k3Jr5s"));
                }
                webClient.DownloadFileCompleted += new AsyncCompletedEventHandler(DownloadGameCompletedCallback);
                webClient.DownloadFileAsync(new Uri("https://drive.google.com/uc?export=download&id=1SNA_3P5wVp4tZi5NKhiGAAD6q4ilbaaf"), gameZip, _onlineVersion);
            }
            catch (Exception ex)
            {
                Status = LauncherStatus.failed;
                MessageBox.Show($"Error installing game files: {ex}");
            }
        }
        private void DownloadGameCompletedCallback(object sender, AsyncCompletedEventArgs e)
        {

            try
            {
                string onlineVersion = ((Version)e.UserState).ToString();
                ZipFile.ExtractToDirectory(gameZip, rootPath);
                File.Delete(gameZip);

                File.WriteAllText(Path.Combine(rootPath, "Version.txt"), onlineVersion);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error finishing download: {ex}");
            }
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            CheckForUpdates();
        }
        internal LauncherStatus Status
        {
            get => _status;
            set
            {
                _status = value;
                switch (_status)
                {
                    case LauncherStatus.ready:
                        button1.Text = "Play";
                        break;
                    case LauncherStatus.failed:
                        button1.Text = "Update Failed - Retry";
                        break;
                    case LauncherStatus.downloadingGame:
                        button1.Text = "Downloading Game";
                        break;
                    case LauncherStatus.downloadingUpdate:
                        button1.Text = "Downloading Update";
                        break;
                    default:
                        break;
                }
            }
        }
    }

    enum LauncherStatus
    {
        ready, failed, downloadingGame, downloadingUpdate
    }
}
