using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Diagnostics;
using System.Management;
using System.IO;

namespace VpnClientApp
{
    public partial class MainForm : Form
    {
        private TextBox txtVpnName;
        private TextBox txtServerAddress;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private ComboBox cmbConnectionType;
        private Button btnCreateVpn;
        private Button btnConnectVpn;
        private Button btnDeleteVpn;
        private CheckBox chkSaveCredentials;
        private Label lblStatus;
        private VpnConfig config;

        public MainForm()
        {
            InitializeComponent();
            LoadConfig();
        }

        private void InitializeComponent()
        {
            this.Text = "VPN Client";
            this.Size = new Size(500, 400);
            this.StartPosition = FormStartPosition.CenterScreen;
            
            this.FormClosing += MainForm_FormClosing;

            Label lblVpnName = new Label
            {
                Text = "VPN Name:",
                Location = new Point(20, 20),
                Size = new Size(120, 20)
            };

            Label lblServerAddress = new Label
            {
                Text = "Server Address:",
                Location = new Point(20, 50),
                Size = new Size(120, 20)
            };

            Label lblConnectionType = new Label
            {
                Text = "Connection Type:",
                Location = new Point(20, 80),
                Size = new Size(120, 20)
            };

            Label lblUsername = new Label
            {
                Text = "Username:",
                Location = new Point(20, 110),
                Size = new Size(120, 20)
            };

            Label lblPassword = new Label
            {
                Text = "Password:",
                Location = new Point(20, 140),
                Size = new Size(120, 20)
            };

            txtVpnName = new TextBox
            {
                Text = "MyVPN",
                Location = new Point(150, 20),
                Size = new Size(200, 20)
            };

            txtServerAddress = new TextBox
            {
                Location = new Point(150, 50),
                Size = new Size(200, 20)
            };

            cmbConnectionType = new ComboBox
            {
                Location = new Point(150, 80),
                Size = new Size(200, 20),
                DropDownStyle = ComboBoxStyle.DropDownList
            };
            cmbConnectionType.Items.Add("PPTP");
            cmbConnectionType.Items.Add("L2TP/IPSec");
            cmbConnectionType.SelectedIndex = 1; // Default to L2TP

            txtUsername = new TextBox
            {
                Location = new Point(150, 110),
                Size = new Size(200, 20)
            };

            txtPassword = new TextBox
            {
                Location = new Point(150, 140),
                Size = new Size(200, 20),
                PasswordChar = '*'
            };

            chkSaveCredentials = new CheckBox
            {
                Text = "Save credentials",
                Location = new Point(150, 170),
                Size = new Size(200, 20),
                Checked = true
            };

            btnCreateVpn = new Button
            {
                Text = "Create VPN",
                Location = new Point(50, 210),
                Size = new Size(120, 30)
            };
            btnCreateVpn.Click += BtnCreateVpn_Click;

            btnConnectVpn = new Button
            {
                Text = "Connect",
                Location = new Point(190, 210),
                Size = new Size(120, 30)
            };
            btnConnectVpn.Click += BtnConnectVpn_Click;

            btnDeleteVpn = new Button
            {
                Text = "Delete VPN",
                Location = new Point(330, 210),
                Size = new Size(120, 30)
            };
            btnDeleteVpn.Click += BtnDeleteVpn_Click;

            lblStatus = new Label
            {
                Text = "Ready",
                Location = new Point(20, 260),
                Size = new Size(460, 100),
                BorderStyle = BorderStyle.FixedSingle,
                BackColor = Color.White,
                AutoSize = false
            };

            this.Controls.Add(lblVpnName);
            this.Controls.Add(lblServerAddress);
            this.Controls.Add(lblConnectionType);
            this.Controls.Add(lblUsername);
            this.Controls.Add(lblPassword);
            this.Controls.Add(txtVpnName);
            this.Controls.Add(txtServerAddress);
            this.Controls.Add(cmbConnectionType);
            this.Controls.Add(txtUsername);
            this.Controls.Add(txtPassword);
            this.Controls.Add(chkSaveCredentials);
            this.Controls.Add(btnCreateVpn);
            this.Controls.Add(btnConnectVpn);
            this.Controls.Add(btnDeleteVpn);
            this.Controls.Add(lblStatus);
        }

        private void BtnCreateVpn_Click(object sender, EventArgs e)
        {
            try
            {
                string vpnName = txtVpnName.Text;
                string serverAddress = txtServerAddress.Text;
                string connectionType = cmbConnectionType.SelectedItem.ToString();
                string username = txtUsername.Text;
                string password = txtPassword.Text;
                bool saveCredentials = chkSaveCredentials.Checked;

                if (string.IsNullOrEmpty(vpnName) || string.IsNullOrEmpty(serverAddress))
                {
                    MessageBox.Show("VPN Name and Server Address are required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string tunnelType = connectionType == "PPTP" ? "PPTP" : "L2TP";
                string psCommand = $"Add-VpnConnection -Name \"{vpnName}\" -ServerAddress \"{serverAddress}\" -TunnelType {tunnelType} -EncryptionLevel \"Optional\" -AuthenticationMethod MSChapv2 -RememberCredential ${saveCredentials.ToString().ToLower()} -Force";
                
                if (saveCredentials && !string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    psCommand += $" -Username \"{username}\" -Password \"{password}\"";
                }

                RunPowerShellCommand(psCommand);

                if (connectionType == "L2TP/IPSec")
                {
                    string l2tpCommand = $"Set-VpnConnection -Name \"{vpnName}\" -AuthenticationMethod MSChapv2 -EncryptionLevel \"Optional\" -UseWinlogonCredential $false -Force";
                    RunPowerShellCommand(l2tpCommand);
                }

                SaveConfig();
                
                UpdateStatus($"VPN connection '{vpnName}' created successfully.");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error creating VPN: {ex.Message}");
            }
        }

        private void BtnConnectVpn_Click(object sender, EventArgs e)
        {
            try
            {
                string vpnName = txtVpnName.Text;
                string username = txtUsername.Text;
                string password = txtPassword.Text;

                if (string.IsNullOrEmpty(vpnName))
                {
                    MessageBox.Show("VPN Name is required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string psCommand = $"rasdial \"{vpnName}\"";
                
                if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
                {
                    psCommand += $" {username} {password}";
                }

                string result = RunPowerShellCommand(psCommand);

                if (result.Contains("Successfully connected"))
                {
                    UpdateStatus($"Successfully connected to '{vpnName}'");
                }
                else
                {
                    UpdateStatus($"Failed to connect to '{vpnName}'. Error: {result}");
                }
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error connecting to VPN: {ex.Message}");
            }
        }

        private void BtnDeleteVpn_Click(object sender, EventArgs e)
        {
            try
            {
                string vpnName = txtVpnName.Text;

                if (string.IsNullOrEmpty(vpnName))
                {
                    MessageBox.Show("VPN Name is required.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                string psCommand = $"Remove-VpnConnection -Name \"{vpnName}\" -Force -Confirm:$false";
                RunPowerShellCommand(psCommand);

                UpdateStatus($"VPN connection '{vpnName}' deleted successfully.");
            }
            catch (Exception ex)
            {
                UpdateStatus($"Error deleting VPN: {ex.Message}");
            }
        }

        private string RunPowerShellCommand(string command)
        {
            try
            {
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = "powershell.exe",
                    Arguments = $"-NoProfile -ExecutionPolicy Bypass -Command \"{command}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                };

                using (Process process = new Process { StartInfo = startInfo })
                {
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    string error = process.StandardError.ReadToEnd();
                    process.WaitForExit();

                    if (!string.IsNullOrEmpty(error))
                    {
                        throw new Exception(error);
                    }

                    return output;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Error executing PowerShell command: {ex.Message}");
            }
        }

        private void UpdateStatus(string message)
        {
            lblStatus.Text = $"{DateTime.Now}: {message}";
        }
        
        private void LoadConfig()
        {
            config = VpnConfig.Load();
            
            txtVpnName.Text = config.VpnName;
            txtServerAddress.Text = config.ServerAddress;
            
            if (cmbConnectionType.Items.Contains(config.ConnectionType))
            {
                cmbConnectionType.SelectedItem = config.ConnectionType;
            }
            
            txtUsername.Text = config.Username;
            txtPassword.Text = config.Password;
            chkSaveCredentials.Checked = config.SaveCredentials;
        }

        private void SaveConfig()
        {
            config.VpnName = txtVpnName.Text;
            config.ServerAddress = txtServerAddress.Text;
            config.ConnectionType = cmbConnectionType.SelectedItem?.ToString() ?? "L2TP/IPSec";
            config.Username = txtUsername.Text;
            config.Password = chkSaveCredentials.Checked ? txtPassword.Text : "";
            config.SaveCredentials = chkSaveCredentials.Checked;
            
            config.Save();
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            SaveConfig();
        }
    }
}
