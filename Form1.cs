using System;
using System.Drawing;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;

namespace AAC_Optimizer
{
    public partial class Form1 : Form
    {
        // UI Componenten
        private Label lblStatus;
        private Button btnOptimize;
        private Button btnRestore;
        private CheckBox chkAutoOptimize;
        private Timer tmrLauncherWatch;

        // Logica & State Management
        private readonly string launcherProcessName = "ArcheAge Classic Launcher";
        private bool hasAutoOptimizedThisSession = false;

        public Form1()
        {
            // Initialiseer alle UI-elementen
            InitializeComponent();
        }

        /// <summary>
        /// Handmatige initialisatie van de UI-componenten voor een clean en minimalistisch design.
        /// </summary>
        private void InitializeComponent()
        {
            this.SuspendLayout();

            // DPI scaling en AutoScale instellingen
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;

            // Formulier instellingen (Form1)
            this.Text = "AAC Raid Smoother";
            this.ClientSize = new Size(380, 200); // Slightly larger for a modern look and better text scaling
            this.StartPosition = FormStartPosition.CenterScreen;
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.BackColor = Color.FromArgb(245, 245, 245); // Minimalistic light gray

            // TableLayoutPanel voor de hoofd-layout
            TableLayoutPanel mainLayout = new TableLayoutPanel();
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.ColumnCount = 1;
            mainLayout.RowCount = 3;
            mainLayout.Padding = new Padding(20);
            
            // Definieer de rij-verhoudingen
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));  // Status label
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Absolute, 45F));  // Knop containers
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 35F));  // Checkbox

            // Status Label (lblStatus)
            lblStatus = new Label();
            lblStatus.Name = "lblStatus";
            lblStatus.Text = "Status: Not optimized";
            lblStatus.Font = new Font("Segoe UI", 10F, FontStyle.Regular);
            lblStatus.ForeColor = Color.FromArgb(50, 50, 50);
            lblStatus.Dock = DockStyle.Fill;
            lblStatus.TextAlign = ContentAlignment.MiddleLeft;

            // TableLayoutPanel voor de knoppen (twee kolommen)
            TableLayoutPanel buttonLayout = new TableLayoutPanel();
            buttonLayout.Dock = DockStyle.Fill;
            buttonLayout.Margin = new Padding(0);
            buttonLayout.ColumnCount = 2;
            buttonLayout.RowCount = 1;
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));

            // Optimaliseer Knop (btnOptimize)
            btnOptimize = new Button();
            btnOptimize.Name = "btnOptimize";
            btnOptimize.Text = "Optimize";
            btnOptimize.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnOptimize.BackColor = Color.FromArgb(0, 120, 215); // Accent blue
            btnOptimize.ForeColor = Color.White;
            btnOptimize.FlatStyle = FlatStyle.Flat;
            btnOptimize.FlatAppearance.BorderSize = 0;
            btnOptimize.Dock = DockStyle.Fill;
            btnOptimize.Margin = new Padding(0, 0, 10, 0); // 10px spacing right
            btnOptimize.Click += new EventHandler(btnOptimize_Click);

            // Herstel Knop (btnRestore)
            btnRestore = new Button();
            btnRestore.Name = "btnRestore";
            btnRestore.Text = "Restore";
            btnRestore.Font = new Font("Segoe UI", 9.5F, FontStyle.Regular);
            btnRestore.BackColor = Color.FromArgb(225, 225, 225);
            btnRestore.ForeColor = Color.FromArgb(50, 50, 50);
            btnRestore.FlatStyle = FlatStyle.Flat;
            btnRestore.FlatAppearance.BorderSize = 0;
            btnRestore.Dock = DockStyle.Fill;
            btnRestore.Margin = new Padding(10, 0, 0, 0); // 10px spacing left
            btnRestore.Click += new EventHandler(btnRestore_Click);

            buttonLayout.Controls.Add(btnOptimize, 0, 0);
            buttonLayout.Controls.Add(btnRestore, 1, 0);

            // CheckBox (chkAutoOptimize)
            chkAutoOptimize = new CheckBox();
            chkAutoOptimize.Name = "chkAutoOptimize";
            chkAutoOptimize.Text = "Automatically optimize when launcher starts";
            chkAutoOptimize.Font = new Font("Segoe UI", 9F, FontStyle.Regular);
            chkAutoOptimize.ForeColor = Color.FromArgb(50, 50, 50);
            chkAutoOptimize.Dock = DockStyle.Fill;
            chkAutoOptimize.TextAlign = ContentAlignment.MiddleLeft;
            chkAutoOptimize.CheckedChanged += new EventHandler(chkAutoOptimize_CheckedChanged);

            // Timer (tmrLauncherWatch)
            tmrLauncherWatch = new Timer();
            tmrLauncherWatch.Interval = 2000;
            tmrLauncherWatch.Tick += new EventHandler(tmrLauncherWatch_Tick);

            // Voeg componenten toe aan de layouts en het formulier
            mainLayout.Controls.Add(lblStatus, 0, 0);
            mainLayout.Controls.Add(buttonLayout, 0, 1);
            mainLayout.Controls.Add(chkAutoOptimize, 0, 2);

            this.Controls.Add(mainLayout);

            // Registreer Load event
            this.Load += new EventHandler(Form1_Load);

            this.ResumeLayout(false);
            this.PerformLayout();
        }

        /// <summary>
        /// Event handler voor chkAutoOptimize check status verandering.
        /// </summary>
        private void chkAutoOptimize_CheckedChanged(object sender, EventArgs e)
        {
            if (chkAutoOptimize.Checked)
            {
                tmrLauncherWatch.Enabled = true;
                lblStatus.Text = "Status: Waiting for ArcheAge Classic Launcher...";
            }
            else
            {
                tmrLauncherWatch.Enabled = false;
                lblStatus.Text = "Status: Manual mode active.";
            }
        }

        /// <summary>
        /// Timer tick event handler die controleert of de launcher actief is en zo ja, optimalisatie start.
        /// </summary>
        private async void tmrLauncherWatch_Tick(object sender, EventArgs e)
        {
            if (hasAutoOptimizedThisSession)
            {
                return;
            }

            // Voer de processen-controle asynchroon uit op een threadpool thread om de UI niet te blokkeren
            bool isRunning = await System.Threading.Tasks.Task.Run(() =>
            {
                try
                {
                    var processes = Process.GetProcessesByName(launcherProcessName);
                    return processes.Length > 0;
                }
                catch
                {
                    return false;
                }
            });

            if (isRunning && !hasAutoOptimizedThisSession)
            {
                hasAutoOptimizedThisSession = true;
                btnOptimize_Click(this, EventArgs.Empty);
            }
        }

        /// <summary>
        /// Voert een command-line applicatie onzichtbaar op de achtergrond uit.
        /// </summary>
        private int RunCommand(string filename, string arguments)
        {
            using (var process = new Process())
            {
                process.StartInfo = new ProcessStartInfo
                {
                    FileName = filename,
                    Arguments = arguments,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    UseShellExecute = false
                };
                process.Start();
                process.WaitForExit();
                return process.ExitCode;
            }
        }

        /// <summary>
        /// Controleert of een directory een symbolische link (Reparse Point) is.
        /// </summary>
        private bool IsSymbolicLink(string path)
        {
            if (!Directory.Exists(path))
                return false;

            FileAttributes attributes = File.GetAttributes(path);
            return (attributes & FileAttributes.ReparsePoint) == FileAttributes.ReparsePoint;
        }

        /// <summary>
        /// Event handler voor de optimalisatieknop.
        /// </summary>
        private void btnOptimize_Click(object sender, EventArgs e)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string userPath = Path.Combine(documentsPath, "AAClassic", "USER", "shaders");
            string backupPath = Path.Combine(documentsPath, "AAClassic", "USER", "shaders_Backup");

            try
            {
                // a. Controleer of "R:" bestaat en de "shaders"-map al een symlink is
                if (Directory.Exists("R:\\") && IsSymbolicLink(userPath))
                {
                    MessageBox.Show("The game is already optimized!", "Optimization", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                // Controleer of de originele "shaders"-map bestaat voordat we beginnen
                if (!Directory.Exists(userPath))
                {
                    MessageBox.Show("Error: The 'shaders' folder could not be found in the Documents/AAClassic/USER directory.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // b. Een veilige back-up maken van de "USER"-map naar "USER_Backup"
                if (Directory.Exists(backupPath))
                {
                    Directory.Delete(backupPath, true);
                }
                Directory.Move(userPath, backupPath);

                // c. De RAM-disk aanmaken via ImDisk (2 GB, NTFS, Quiet)
                if (!Directory.Exists("R:\\"))
                {
                    int imdiskExitCode = RunCommand("imdisk.exe", "-a -s 2G -m R: -p \"/fs:ntfs /q /y\"");
                    if (!Directory.Exists("R:\\"))
                    {
                        // Herstel back-up bij falen
                        Directory.Move(backupPath, userPath);
                        MessageBox.Show("Error: Could not create RAM disk (R:) via ImDisk. Check if ImDisk is correctly installed and the application is run as Administrator.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        return;
                    }
                }

                // d. De profielinstellingen en shaders verplaatsen naar de RAM-disk via Robocopy
                string robocopyArgs = string.Format("\"{0}\" \"R:\" /E /MOVE /R:1 /W:1 /NP /NFL /NDL", backupPath);
                int robocopyExitCode = RunCommand("robocopy.exe", robocopyArgs);

                // Robocopy exitcode >= 8 betekent een mislukte overdracht
                if (robocopyExitCode >= 8)
                {
                    // Herstel back-up bij falen
                    if (Directory.Exists(backupPath) && !Directory.Exists(userPath))
                    {
                        Directory.Move(backupPath, userPath);
                    }
                    MessageBox.Show("Error: File transfer via Robocopy failed. Error code: " + robocopyExitCode, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // e. Een directory symbolic link aanmaakt via: cmd.exe /c mklink /d
                string mklinkArgs = string.Format("/c mklink /d \"{0}\" \"R:\"", userPath);
                int mklinkExitCode = RunCommand("cmd.exe", mklinkArgs);

                if (mklinkExitCode != 0 || !IsSymbolicLink(userPath))
                {
                    // Herstelactie indien mogelijk
                    MessageBox.Show("Error: Creating symbolic link failed. Make sure you have administrator privileges.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                lblStatus.Text = "Status: Optimized";
                MessageBox.Show("Optimization successfully completed! The 'shaders' folder has been moved to the RAM disk (R:).", "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Er is een onverwachte fout opgetreden: " + ex.Message, "Fout", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        /// <summary>
        /// Event handler voor de herstelknop.
        /// </summary>
        private void btnRestore_Click(object sender, EventArgs e)
        {
            RestoreDefault(false);
        }

        /// <summary>
        /// Herstelt de standaardinstellingen door de RAM-disk te ontkoppelen en de USER-map terug te zetten naar de SSD.
        /// </summary>
        private bool RestoreDefault(bool silent = false)
        {
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string userPath = Path.Combine(documentsPath, "AAClassic", "USER", "shaders");
            string backupPath = Path.Combine(documentsPath, "AAClassic", "USER", "shaders_Backup");

            try
            {
                // a. Controleren of de RAM-disk ("R:") actief is.
                if (!Directory.Exists("R:\\"))
                {
                    if (!silent)
                    {
                        MessageBox.Show("The RAM disk (R:) is not active or already unmounted.", "Restore", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    return false;
                }

                // b. De symbolic link van de "USER"-map veilig verwijderen zonder de bestanden op de RAM-disk te beschadigen.
                if (IsSymbolicLink(userPath))
                {
                    Directory.Delete(userPath);
                }
                else if (Directory.Exists(userPath))
                {
                    Directory.Delete(userPath, true);
                }

                // c. Een nieuwe, fysieke "USER"-map aanmaken op de opslagschijf.
                Directory.CreateDirectory(userPath);

                // d. Alle gewijzigde instellingen en keybinds van de RAM-disk terugschrijven naar de SSD via Robocopy.
                // Uitsluiten van systeemmappen (System Volume Information, $RECYCLE.BIN) om Robocopy-fouten te voorkomen.
                string robocopyArgs = string.Format("\"R:\" \"{0}\" /E /XD \"System Volume Information\" \"$RECYCLE.BIN\" /R:1 /W:1 /NP /NFL /NDL", userPath);
                int robocopyExitCode = RunCommand("robocopy.exe", robocopyArgs);

                if (robocopyExitCode >= 8)
                {
                    if (!silent)
                    {
                        MessageBox.Show("Error writing back settings via Robocopy. Error code: " + robocopyExitCode, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                    return false;
                }

                // e. De RAM-disk ontkoppelen en vernietigen met imdisk.exe -D -m R:. Indien dit mislukt, noodontkoppeling met imdisk.exe -R -m R:.
                int imdiskExitCode = RunCommand("imdisk.exe", "-D -m R:");
                if (imdiskExitCode != 0 || Directory.Exists("R:\\"))
                {
                    RunCommand("imdisk.exe", "-R -m R:");
                }

                // f. De "USER_Backup" map opruimen.
                if (Directory.Exists(backupPath))
                {
                    Directory.Delete(backupPath, true);
                }

                lblStatus.Text = "Status: Not optimized";
                hasAutoOptimizedThisSession = false;

                if (!silent)
                {
                    MessageBox.Show("Restore successfully completed! The RAM disk has been unmounted and the shaders folder has been restored to the storage drive.", "Restore Successful", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
                return true;
            }
            catch (Exception ex)
            {
                if (!silent)
                {
                    MessageBox.Show("An error occurred during restore: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                return false;
            }
        }

        /// <summary>
        /// Overschrijft het sluiten van het formulier om dataverlies bij systeemafsluiting te voorkomen.
        /// </summary>
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.WindowsShutDown)
            {
                // Systeemafsluiting: direct en geruisloos herstellen om binnen de Windows shutdown-timeout te blijven.
                RestoreDefault(true);
            }
            else if (e.CloseReason == CloseReason.UserClosing)
            {
                // Regular close by user: ask to restore settings or leave RAM disk active
                if (Directory.Exists("R:\\"))
                {
                    DialogResult result = MessageBox.Show(
                        "The game is currently optimized (RAM disk is active). Would you like to restore the settings to the SSD before closing?\n\nClick 'Yes' to restore and exit.\nClick 'No' to exit and keep the RAM disk active.\nClick 'Cancel' to stay in the application.",
                        "Exit",
                        MessageBoxButtons.YesNoCancel,
                        MessageBoxIcon.Question
                    );

                    if (result == DialogResult.Yes)
                    {
                        if (!RestoreDefault(false))
                        {
                            e.Cancel = true; // Cancel exit if restore failed
                        }
                    }
                    else if (result == DialogResult.Cancel)
                    {
                        e.Cancel = true;
                    }
                }
            }

            base.OnFormClosing(e);
        }

        /// <summary>
        /// Pak ingebedde driverbestanden (imdisk.exe, imdisk.sys, imdisk.inf) uit naar de applicatiemap.
        /// </summary>
        private void DeployEmbeddedAssets()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string[] filesToDeploy = { "imdisk.exe", "imdisk.sys", "imdisk.inf" };
            var assembly = System.Reflection.Assembly.GetExecutingAssembly();
            string[] resourceNames = assembly.GetManifestResourceNames();

            foreach (string file in filesToDeploy)
            {
                string targetPath = Path.Combine(baseDir, file);
                if (!File.Exists(targetPath))
                {
                    // Zoek de resource die eindigt op de bestandsnaam (ongeacht namespace)
                    string resourceName = Array.Find(resourceNames, name => name.EndsWith(file, StringComparison.OrdinalIgnoreCase));
                    if (string.IsNullOrEmpty(resourceName))
                    {
                        Console.WriteLine("[DEPLOY] Embedded resource for " + file + " not found in assembly manifest.");
                        continue;
                    }

                    try
                    {
                        using (Stream resourceStream = assembly.GetManifestResourceStream(resourceName))
                        {
                            if (resourceStream == null)
                            {
                                Console.WriteLine("[DEPLOY] Could not open resource stream for " + file + ".");
                                continue;
                            }

                            using (FileStream fileStream = new FileStream(targetPath, FileMode.Create, FileAccess.Write))
                            {
                                resourceStream.CopyTo(fileStream);
                            }
                        }
                        Console.WriteLine("[DEPLOY] File successfully extracted to: " + targetPath);
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("[DEPLOY] Error extracting " + file + ": " + ex.Message);
                    }
                }
                else
                {
                    Console.WriteLine("[DEPLOY] File already exists: " + targetPath);
                }
            }
        }

        /// <summary>
        /// Installeert en laadt de ImDisk driver via opeenvolgende fallback-strategieën.
        /// </summary>
        private bool InstallImDiskDriver()
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            string infPath = Path.Combine(baseDir, "imdisk.inf");
            string sysPath = Path.Combine(baseDir, "imdisk.sys");

            Console.WriteLine("[INSTALL] Starting ImDisk driver installation...");

            // Check if the service is already installed and running
            int checkInitialCode = RunCommand("sc.exe", "start imdisk");
            if (checkInitialCode == 0 || checkInitialCode == 1056)
            {
                Console.WriteLine("[INSTALL] ImDisk driver is already installed and active.");
                return true;
            }

            // Strategy A: Rundll32 setupapi.dll,InstallHinfSection
            Console.WriteLine("[INSTALL] Strategy A: Installing via SetupAPI (rundll32)...");
            string rundllArgs = string.Format("setupapi.dll,InstallHinfSection DefaultInstall 132 {0}", infPath);
            int setupApiExitCode = RunCommand("rundll32.exe", rundllArgs);

            // Check if the service exists and can be started after Strategy A.
            int checkStartExitCode = RunCommand("sc.exe", "start imdisk");
            if (checkStartExitCode == 0 || checkStartExitCode == 1056) // 1056 = service is already running
            {
                Console.WriteLine("[INSTALL] Strategy A successful. Driver is active.");
                return true;
            }

            Console.WriteLine("[INSTALL] Strategy A failed or driver does not start (Exit code start: " + checkStartExitCode + "). Installing manually via Strategy B...");

            // Strategy B: sc.exe create and sc.exe start
            Console.WriteLine("[INSTALL] Strategy B: Manually registering kernel service via Service Controller...");
            string systemRoot = Environment.GetEnvironmentVariable("SystemRoot") ?? "C:\\Windows";
            string destSysPath = Path.Combine(systemRoot, "system32\\drivers\\imdisk.sys");

            try
            {
                // Copy the sys file to system32\drivers if it's not already there
                if (!File.Exists(destSysPath))
                {
                    File.Copy(sysPath, destSysPath, true);
                    Console.WriteLine("[INSTALL] sys file copied to " + destSysPath);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("[INSTALL] Error copying driver file to drivers directory: " + ex.Message);
            }

            string createArgs = "create imdisk binPath= \"%SystemRoot%\\system32\\drivers\\imdisk.sys\" type= kernel start= auto DisplayName= \"ImDisk Driver\"";
            int createExitCode = RunCommand("sc.exe", createArgs);
            Console.WriteLine("[INSTALL] sc create exit code: " + createExitCode);

            int startExitCode = RunCommand("sc.exe", "start imdisk");
            Console.WriteLine("[INSTALL] sc start exit code: " + startExitCode);

            if (startExitCode == 0 || startExitCode == 1056)
            {
                Console.WriteLine("[INSTALL] Strategy B successful. Driver manually loaded.");
                return true;
            }

            Console.WriteLine("[INSTALL] Both installation strategies failed.");
            return false;
        }

        /// <summary>
        /// Wordt aangeroepen bij het laden van het formulier. Zorgt voor de installatie en activatie van de ImDisk driver.
        /// </summary>
        private void Form1_Load(object sender, EventArgs e)
        {
            lblStatus.Text = "Status: Initializing ImDisk driver...";
            Console.WriteLine("[LOAD] Application starting. Initializing driver...");

            try
            {
                DeployEmbeddedAssets();

                if (InstallImDiskDriver())
                {
                    lblStatus.Text = "Status: Driver loaded. Ready.";
                    Console.WriteLine("[LOAD] ImDisk driver successfully loaded.");
                }
                else
                {
                    lblStatus.Text = "Status: Error loading driver.";
                    Console.WriteLine("[LOAD] Error: ImDisk driver could not be loaded.");
                    MessageBox.Show("Error: The ImDisk driver could not be automatically installed or started. Please run the application as Administrator.", "Driver Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
            catch (Exception ex)
            {
                lblStatus.Text = "Status: Error during initialization.";
                Console.WriteLine("[LOAD] Unexpected error during initialization: " + ex.Message);
                MessageBox.Show("An error occurred during driver initialization: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}
