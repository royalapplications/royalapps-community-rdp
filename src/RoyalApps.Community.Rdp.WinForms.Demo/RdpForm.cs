using System;
using System.Drawing;
using System.IO;
using System.Text.Json;
using System.Windows.Forms;
using RoyalApps.Community.Rdp.WinForms;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Configuration.Display;

namespace RoyalApps.Community.Rdp.Demo;

public partial class RdpForm : Form
{
    private readonly Form _settingsForm;
    private readonly PropertyGrid _propertyGrid;
    private readonly RdpControl _rdpControl;

    private readonly ComboBox _sessionModeComboBox;
    private readonly ComboBox _clientVersionComboBox;
    private readonly CheckBox _useMsRdcCheckBox;
    private readonly TextBox _serverTextBox;
    private readonly TextBox _domainTextBox;
    private readonly TextBox _usernameTextBox;
    private readonly TextBox _passwordTextBox;

    private readonly CheckBox _remoteAppEnabledCheckBox;
    private readonly TextBox _remoteAppProgramTextBox;
    private readonly TextBox _remoteAppNameTextBox;
    private readonly TextBox _remoteAppArgumentsTextBox;
    private readonly TextBox _remoteAppWorkingDirectoryTextBox;
    private readonly TextBox _remoteAppFileTextBox;
    private readonly CheckBox _disableCapabilitiesCheckBox;

    private readonly ListBox _eventLogListBox;
    private readonly Label _profileSummaryLabel;
    private readonly ToolStripStatusLabel _statusLabel;
    private readonly ToolStripMenuItem _changeMenu;
    private readonly ToolStripMenuItem _scrollbarsMenuItem;
    private readonly ToolStripMenuItem _smartSizingMenuItem;
    private readonly ToolStripMenuItem _smartReconnectMenuItem;
    private string _currentProfileName = "Custom";
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true
    };

    public RdpForm()
    {
        InitializeComponent();
        FormClosing += RdpForm_FormClosing;

        _settingsForm = new Form
        {
            Size = new Size(800, 1000),
            Text = "Advanced Settings",
            FormBorderStyle = FormBorderStyle.SizableToolWindow,
            StartPosition = FormStartPosition.CenterParent
        };
        _settingsForm.FormClosing += (_, args) =>
        {
            _settingsForm.Hide();
            args.Cancel = true;
        };

        _propertyGrid = new PropertyGrid
        {
            PropertySort = PropertySort.NoSort,
            Dock = DockStyle.Fill,
            Parent = _settingsForm
        };
        _propertyGrid.PropertyValueChanged += (_, _) => SyncHarnessInputsFromConfiguration();

        _rdpControl = new RdpControl
        {
            Dock = DockStyle.Fill
        };

        _sessionModeComboBox = CreateDropDownList();
        _sessionModeComboBox.Items.AddRange(
        [
            RdpSessionMode.Embedded,
            RdpSessionMode.External
        ]);

        _clientVersionComboBox = CreateDropDownList();
        _clientVersionComboBox.Items.AddRange(
        [
            "Auto",
            "12",
            "11",
            "10",
            "9",
            "8",
            "7",
            "2"
        ]);

        _useMsRdcCheckBox = new CheckBox
        {
            AutoSize = true,
            Text = "Use rdclientax.dll when available"
        };

        _serverTextBox = CreateTextBox("rdp.example.test");
        _domainTextBox = CreateTextBox("LAB");
        _usernameTextBox = CreateTextBox("alice");
        _passwordTextBox = CreateTextBox("password", true);

        _remoteAppEnabledCheckBox = new CheckBox
        {
            AutoSize = true,
            Text = "Enable RemoteApp mode"
        };
        _remoteAppProgramTextBox = CreateTextBox("Published alias or app path, for example ||Notepad");
        _remoteAppNameTextBox = CreateTextBox("Display name shown by the server/client");
        _remoteAppArgumentsTextBox = CreateTextBox("Optional command line");
        _remoteAppWorkingDirectoryTextBox = CreateTextBox("%USERPROFILE%\\Documents");
        _remoteAppFileTextBox = CreateTextBox("Optional file to open");
        _disableCapabilitiesCheckBox = new CheckBox
        {
            AutoSize = true,
            Text = "Disable RemoteApp capabilities check"
        };

        _eventLogListBox = new ListBox
        {
            Dock = DockStyle.Fill,
            HorizontalScrollbar = true
        };

        _profileSummaryLabel = new Label
        {
            AutoSize = true,
            MaximumSize = new Size(360, 0)
        };

        _statusLabel = new ToolStripStatusLabel();

        _scrollbarsMenuItem = new ToolStripMenuItem("Scrollbars");
        _smartSizingMenuItem = new ToolStripMenuItem("Smart Sizing");
        _smartReconnectMenuItem = new ToolStripMenuItem("Smart Reconnect");
        _changeMenu = BuildChangeMenu();

        BuildMainLayout();
        ConfigureDemoDefaults();
        RegisterControlEvents();
        SyncHarnessInputsFromConfiguration();

        AppendLog("Loaded RDP test harness. Choose a profile or fill in the connection details, then connect.");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
            components.Dispose();

        base.Dispose(disposing);
    }

    private void BuildMainLayout()
    {
        var connectMenuItem = new ToolStripMenuItem("Connect");
        connectMenuItem.Click += (_, _) => ConnectUsingHarness();

        var disconnectMenuItem = new ToolStripMenuItem("Disconnect");
        disconnectMenuItem.Click += (_, _) => DisconnectUsingHarness();

        var settingsMenuItem = new ToolStripMenuItem("Advanced Settings...");
        settingsMenuItem.Click += (_, _) => OpenSettings();

        var loadExternalRemoteAppProfileMenuItem = new ToolStripMenuItem("External RemoteApp");
        loadExternalRemoteAppProfileMenuItem.Click += (_, _) => LoadExternalRemoteAppProfile();

        var loadEmbeddedDesktopProfileMenuItem = new ToolStripMenuItem("Embedded Desktop Baseline");
        loadEmbeddedDesktopProfileMenuItem.Click += (_, _) => LoadEmbeddedDesktopProfile();

        var connectionInfoMenuItem = new ToolStripMenuItem("Show Connection Info");
        connectionInfoMenuItem.Click += (_, _) => _rdpControl.ShowConnectionInfo();

        var zoomInMenuItem = new ToolStripMenuItem("Zoom In");
        zoomInMenuItem.Click += (_, _) => _rdpControl.ZoomIn();

        var zoomOutMenuItem = new ToolStripMenuItem("Zoom Out");
        zoomOutMenuItem.Click += (_, _) => _rdpControl.ZoomOut();

        var updateDisplayMenuItem = new ToolStripMenuItem("Update Display");
        updateDisplayMenuItem.Click += (_, _) => _rdpControl.UpdateClientSize();

        _scrollbarsMenuItem.Click += (_, _) =>
        {
            SetResizeBehavior(ResizeBehavior.Scrollbars);
        };
        _smartSizingMenuItem.Click += (_, _) =>
        {
            SetResizeBehavior(ResizeBehavior.SmartSizing);
        };
        _smartReconnectMenuItem.Click += (_, _) =>
        {
            SetResizeBehavior(ResizeBehavior.SmartReconnect);
        };

        var remoteDesktopMenu = new ToolStripMenuItem("Remote Desktop")
        {
            DropDownItems =
            {
                connectMenuItem,
                disconnectMenuItem,
                new ToolStripSeparator(),
                connectionInfoMenuItem,
                new ToolStripSeparator(),
                settingsMenuItem
            }
        };

        var profilesMenu = new ToolStripMenuItem("Profiles")
        {
            DropDownItems =
            {
                loadExternalRemoteAppProfileMenuItem,
                loadEmbeddedDesktopProfileMenuItem
            }
        };

        _changeMenu.DropDownItems.Insert(0, updateDisplayMenuItem);
        _changeMenu.DropDownItems.Insert(1, new ToolStripSeparator());
        _changeMenu.DropDownItems.Insert(2, new ToolStripLabel("Resize Behavior") { Enabled = false });
        _changeMenu.DropDownItems.Insert(3, _scrollbarsMenuItem);
        _changeMenu.DropDownItems.Insert(4, _smartSizingMenuItem);
        _changeMenu.DropDownItems.Insert(5, _smartReconnectMenuItem);
        _changeMenu.DropDownItems.Insert(6, new ToolStripSeparator());
        _changeMenu.DropDownItems.Insert(7, new ToolStripLabel("Zoom") { Enabled = false });
        _changeMenu.DropDownItems.Insert(8, zoomInMenuItem);
        _changeMenu.DropDownItems.Insert(9, zoomOutMenuItem);

        var menuStrip = new MenuStrip
        {
            Dock = DockStyle.Top,
            Items = { remoteDesktopMenu, profilesMenu, _changeMenu }
        };

        var statusStrip = new StatusStrip
        {
            Dock = DockStyle.Bottom,
            Items = { _statusLabel }
        };

        var splitContainer = new SplitContainer
        {
            Dock = DockStyle.Fill,
            FixedPanel = FixedPanel.Panel2,
            SplitterDistance = 1140
        };
        splitContainer.Panel1.Controls.Add(_rdpControl);
        splitContainer.Panel2.Controls.Add(CreateHarnessPanel());

        MainMenuStrip = menuStrip;
        Controls.Add(splitContainer);
        Controls.Add(statusStrip);
        Controls.Add(menuStrip);
    }

    private ToolStripMenuItem BuildChangeMenu()
    {
        return new ToolStripMenuItem("Change")
        {
            Enabled = false
        };
    }

    private Control CreateHarnessPanel()
    {
        var panel = new Panel
        {
            Dock = DockStyle.Fill,
            AutoScroll = true,
            Padding = new Padding(8)
        };

        var layout = new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 1,
            Dock = DockStyle.Top
        };

        layout.Controls.Add(CreateInstructionsGroup());
        layout.Controls.Add(CreateConnectionGroup());
        layout.Controls.Add(CreateRemoteAppGroup());
        layout.Controls.Add(CreateActionsGroup());
        layout.Controls.Add(CreateLogGroup());

        panel.Controls.Add(layout);
        return panel;
    }

    private Control CreateInstructionsGroup()
    {
        var panel = new Panel
        {
            AutoSize = true,
            Dock = DockStyle.Top
        };

        panel.Controls.Add(_profileSummaryLabel);
        _profileSummaryLabel.Dock = DockStyle.Top;

        var instructionLabel = new Label
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            MaximumSize = new Size(360, 0),
            Text = "1. Choose a profile or enter connection details manually. 2. Verify server, username, and optional RemoteApp settings. 3. Enter the password if needed. 4. Connect and use Activate External Window if an external session opens behind other windows."
        };
        panel.Controls.Add(instructionLabel);

        return new GroupBox
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top,
            Padding = new Padding(10),
            Text = "How to use this harness",
            Controls = { panel }
        };
    }

    private Control CreateConnectionGroup()
    {
        var table = CreateEditorTable();
        AddRow(table, "Session mode", _sessionModeComboBox);
        AddRow(table, "Client version", _clientVersionComboBox);
        AddRow(table, "Server", _serverTextBox);
        AddRow(table, "Domain", _domainTextBox);
        AddRow(table, "Username", _usernameTextBox);
        AddRow(table, "Password", _passwordTextBox);
        AddRow(table, string.Empty, _useMsRdcCheckBox);

        return CreateGroupBox("Connection", table);
    }

    private Control CreateRemoteAppGroup()
    {
        var table = CreateEditorTable();
        AddRow(table, string.Empty, _remoteAppEnabledCheckBox);
        AddRow(table, "Program / alias", _remoteAppProgramTextBox);
        AddRow(table, "Display name", _remoteAppNameTextBox);
        AddRow(table, "Arguments", _remoteAppArgumentsTextBox);
        AddRow(table, "Working directory", _remoteAppWorkingDirectoryTextBox);
        AddRow(table, "File", _remoteAppFileTextBox);
        AddRow(table, string.Empty, _disableCapabilitiesCheckBox);

        return CreateGroupBox("RemoteApp", table);
    }

    private Control CreateActionsGroup()
    {
        var profileFlow = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };

        var externalRemoteAppButton = new Button
        {
            AutoSize = true,
            Text = "External RemoteApp"
        };
        externalRemoteAppButton.Click += (_, _) => LoadExternalRemoteAppProfile();

        var desktopBaselineButton = new Button
        {
            AutoSize = true,
            Text = "Desktop Baseline"
        };
        desktopBaselineButton.Click += (_, _) => LoadEmbeddedDesktopProfile();

        profileFlow.Controls.Add(externalRemoteAppButton);
        profileFlow.Controls.Add(desktopBaselineButton);

        var actionFlow = new FlowLayoutPanel
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            FlowDirection = FlowDirection.LeftToRight,
            WrapContents = true
        };

        var applyButton = new Button
        {
            AutoSize = true,
            Text = "Apply Inputs"
        };
        applyButton.Click += (_, _) => ApplyHarnessInputs();

        var connectButton = new Button
        {
            AutoSize = true,
            Text = "Connect"
        };
        connectButton.Click += (_, _) => ConnectUsingHarness();

        var disconnectButton = new Button
        {
            AutoSize = true,
            Text = "Disconnect"
        };
        disconnectButton.Click += (_, _) => DisconnectUsingHarness();

        var activateExternalWindowButton = new Button
        {
            AutoSize = true,
            Text = "Activate External Window"
        };
        activateExternalWindowButton.Click += (_, _) => ActivateExternalWindow();

        var settingsButton = new Button
        {
            AutoSize = true,
            Text = "Advanced Settings..."
        };
        settingsButton.Click += (_, _) => OpenSettings();

        var clearLogButton = new Button
        {
            AutoSize = true,
            Text = "Clear Log"
        };
        clearLogButton.Click += (_, _) => _eventLogListBox.Items.Clear();

        actionFlow.Controls.Add(applyButton);
        actionFlow.Controls.Add(connectButton);
        actionFlow.Controls.Add(disconnectButton);
        actionFlow.Controls.Add(activateExternalWindowButton);
        actionFlow.Controls.Add(settingsButton);
        actionFlow.Controls.Add(clearLogButton);

        var panel = new Panel
        {
            AutoSize = true,
            Dock = DockStyle.Top
        };
        panel.Controls.Add(actionFlow);
        panel.Controls.Add(profileFlow);

        return CreateGroupBox("Actions", panel);
    }

    private Control CreateLogGroup()
    {
        var groupBox = new GroupBox
        {
            Dock = DockStyle.Top,
            Height = 300,
            Text = "Event Log"
        };
        groupBox.Controls.Add(_eventLogListBox);
        return groupBox;
    }

    private void RegisterControlEvents()
    {
        _rdpControl.OnConnected += (_, args) =>
        {
            RunOnUiThread(() =>
            {
                _changeMenu.Enabled = true;
                UpdateResizeMenuChecks(_rdpControl.RdpConfiguration.Display.ResizeBehavior);
                AppendLog(
                    $"Connected. Mode={args.SessionMode}, ProcessId={args.ExternalProcessId?.ToString() ?? "<none>"}, MultiMonFullScreen={args.MultiMonFullScreen}, Bounds={args.Bounds}.");
            });
        };

        _rdpControl.OnDisconnected += (_, args) =>
        {
            RunOnUiThread(() =>
            {
                _changeMenu.Enabled = false;
                AppendLog(
                    $"Disconnected. Mode={args.SessionMode}, ProcessId={args.ExternalProcessId?.ToString() ?? "<none>"}, Code={args.DisconnectCode}, UserInitiated={args.UserInitiated}, Description={args.Description}");
            });
        };

        _rdpControl.ExternalSessionWindowChanged += (_, args) =>
        {
            RunOnUiThread(() =>
            {
                AppendLog(
                    args.HasWindow
                        ? $"External window tracked. ProcessId={args.ExternalProcessId?.ToString() ?? "<none>"}, Handle={args.WindowHandle:X}."
                        : $"External window no longer tracked. ProcessId={args.ExternalProcessId?.ToString() ?? "<none>"}.");
            });
        };

        _rdpControl.OnRequestContainerMinimize += (_, _) => RunOnUiThread(() => AppendLog("Container minimize requested by the client."));
        _rdpControl.OnRequestLeaveFullScreen += (_, _) => RunOnUiThread(() => AppendLog("Leave full screen requested by the client."));
        _rdpControl.OnClientAreaClicked += (_, _) => RunOnUiThread(() => AppendLog("Client area clicked."));
        _rdpControl.RemoteDesktopSizeChanged += (_, _) => RunOnUiThread(() => AppendLog($"Remote desktop size changed. Local size={_rdpControl.Size}."));
        _rdpControl.RdpClientConfigured += (_, _) =>
        {
            RunOnUiThread(() =>
            {
                var rdpClient = _rdpControl.RdpClient;
                AppendLog(
                    $"Embedded client configured. Wrapper={rdpClient?.GetType().Name}, RemoteAppProgram={_rdpControl.RdpConfiguration.RemoteApp.Program ?? "<empty>"}");
            });
        };
    }

    private void ConfigureDemoDefaults()
    {
        var configuration = _rdpControl.RdpConfiguration;
        var savedState = LoadHarnessState();

        configuration.Server = savedState?.Server ?? string.Empty;
        configuration.Credentials.Domain = savedState?.Domain;
        configuration.Credentials.Username = savedState?.Username ?? string.Empty;
        configuration.Credentials.NetworkLevelAuthentication = savedState?.NetworkLevelAuthentication ?? true;
        configuration.UseMsRdc = savedState?.UseMsRdc ?? false;
        configuration.ClientVersion = savedState?.ClientVersion ?? 0;
        configuration.Display.ResizeBehavior = ResizeBehavior.Scrollbars;
        configuration.Display.FullScreen = false;
        configuration.Display.UseMultimon = false;
        configuration.External.KillProcessOnHostExit = true;

        configuration.RemoteApp.Enabled = savedState?.RemoteAppEnabled ?? false;
        configuration.RemoteApp.Program = savedState?.RemoteAppProgram;
        configuration.RemoteApp.Name = savedState?.RemoteAppName;
        configuration.RemoteApp.CommandLine = savedState?.RemoteAppCommandLine;
        configuration.RemoteApp.WorkingDirectory = savedState?.RemoteAppWorkingDirectory;
        configuration.RemoteApp.File = savedState?.RemoteAppFile;
        configuration.RemoteApp.DisableCapabilitiesCheck = savedState?.DisableCapabilitiesCheck ?? false;

        var savedSessionMode = savedState?.SessionMode ?? RdpSessionMode.Embedded;
        configuration.SessionMode = configuration.RemoteApp.Enabled && savedSessionMode == RdpSessionMode.Embedded
            ? RdpSessionMode.External
            : savedSessionMode;
        configuration.Program.StartProgram = string.Empty;
        configuration.Program.WorkDir = string.Empty;
        configuration.External.SelectedMonitors = string.Empty;

        var currentProfileName = savedState?.CurrentProfileName;
        if (configuration.RemoteApp.Enabled &&
            string.Equals(currentProfileName, "Embedded RemoteApp", StringComparison.Ordinal))
        {
            currentProfileName = "External RemoteApp";
        }

        SetCurrentProfile(currentProfileName ?? "Custom");
    }

    private void LoadExternalRemoteAppProfile(bool writeLog = true)
    {
        ApplyHarnessInputs(false);

        var configuration = _rdpControl.RdpConfiguration;
        configuration.SessionMode = RdpSessionMode.External;
        configuration.RemoteApp.Enabled = true;
        configuration.Display.FullScreen = false;
        configuration.Display.UseMultimon = false;
        configuration.Display.ResizeBehavior = ResizeBehavior.Scrollbars;
        configuration.Program.StartProgram = string.Empty;
        configuration.Program.WorkDir = string.Empty;
        configuration.External.SelectedMonitors = string.Empty;

        SyncHarnessInputsFromConfiguration();
        if (writeLog)
            AppendLog("Loaded profile: External RemoteApp.");
        SetCurrentProfile("External RemoteApp");
    }

    private void LoadEmbeddedDesktopProfile(bool writeLog = true)
    {
        ApplyHarnessInputs(false);

        var configuration = _rdpControl.RdpConfiguration;
        configuration.SessionMode = RdpSessionMode.Embedded;
        configuration.RemoteApp.Enabled = false;
        configuration.Display.ResizeBehavior = ResizeBehavior.Scrollbars;

        SyncHarnessInputsFromConfiguration();
        if (writeLog)
            AppendLog("Loaded profile: Embedded desktop baseline.");
        SetCurrentProfile("Embedded Desktop Baseline");
    }

    private void ApplyHarnessInputs(bool writeLog = true)
    {
        var configuration = _rdpControl.RdpConfiguration;

        configuration.SessionMode = GetSelectedSessionMode();
        configuration.ClientVersion = GetSelectedClientVersion();
        configuration.UseMsRdc = _useMsRdcCheckBox.Checked;

        configuration.Server = NormalizeText(_serverTextBox.Text) ?? string.Empty;
        configuration.Credentials.Domain = NormalizeText(_domainTextBox.Text);
        configuration.Credentials.Username = NormalizeText(_usernameTextBox.Text);
        configuration.Credentials.Password = string.IsNullOrWhiteSpace(_passwordTextBox.Text)
            ? null
            : new SensitiveString(_passwordTextBox.Text);

        configuration.RemoteApp.Enabled = _remoteAppEnabledCheckBox.Checked;
        configuration.RemoteApp.Program = NormalizeText(_remoteAppProgramTextBox.Text);
        configuration.RemoteApp.Name = NormalizeText(_remoteAppNameTextBox.Text);
        configuration.RemoteApp.CommandLine = NormalizeText(_remoteAppArgumentsTextBox.Text);
        configuration.RemoteApp.WorkingDirectory = NormalizeText(_remoteAppWorkingDirectoryTextBox.Text);
        configuration.RemoteApp.File = NormalizeText(_remoteAppFileTextBox.Text);
        configuration.RemoteApp.DisableCapabilitiesCheck = _disableCapabilitiesCheckBox.Checked;

        if (configuration.RemoteApp.Enabled)
        {
            configuration.Program.StartProgram = string.Empty;
            configuration.Program.WorkDir = string.Empty;
        }

        _propertyGrid.SelectedObject = configuration;
        _propertyGrid.Refresh();
        SetCurrentProfile("Custom");

        if (writeLog)
        {
            AppendLog(
                $"Applied inputs. Mode={configuration.SessionMode}, ClientVersion={(configuration.ClientVersion == 0 ? "Auto" : configuration.ClientVersion)}, UseMsRdc={configuration.UseMsRdc}, RemoteAppEnabled={configuration.RemoteApp.Enabled}, Program={configuration.RemoteApp.Program ?? "<empty>"}");
        }
    }

    private void SyncHarnessInputsFromConfiguration()
    {
        var configuration = _rdpControl.RdpConfiguration;

        _sessionModeComboBox.SelectedItem = configuration.SessionMode;
        SelectClientVersion(configuration.ClientVersion);
        _useMsRdcCheckBox.Checked = configuration.UseMsRdc;

        _serverTextBox.Text = configuration.Server;
        _domainTextBox.Text = configuration.Credentials.Domain ?? string.Empty;
        _usernameTextBox.Text = configuration.Credentials.Username ?? string.Empty;
        _passwordTextBox.Text = configuration.Credentials.Password?.GetValue() ?? string.Empty;

        _remoteAppEnabledCheckBox.Checked = configuration.RemoteApp.Enabled;
        _remoteAppProgramTextBox.Text = configuration.RemoteApp.Program ?? string.Empty;
        _remoteAppNameTextBox.Text = configuration.RemoteApp.Name ?? string.Empty;
        _remoteAppArgumentsTextBox.Text = configuration.RemoteApp.CommandLine ?? string.Empty;
        _remoteAppWorkingDirectoryTextBox.Text = configuration.RemoteApp.WorkingDirectory ?? string.Empty;
        _remoteAppFileTextBox.Text = configuration.RemoteApp.File ?? string.Empty;
        _disableCapabilitiesCheckBox.Checked = configuration.RemoteApp.DisableCapabilitiesCheck;

        _propertyGrid.SelectedObject = configuration;
        _propertyGrid.Refresh();
        UpdateResizeMenuChecks(configuration.Display.ResizeBehavior);
        UpdateStatusSummary();
    }

    private void ConnectUsingHarness()
    {
        try
        {
            ApplyHarnessInputs();
            AppendLog("Connecting...");
            _rdpControl.Connect();
        }
        catch (Exception ex)
        {
            AppendLog($"Connect failed: {ex.Message}");
            MessageBox.Show(this, ex.ToString(), "Connect failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void ActivateExternalWindow()
    {
        if (_rdpControl.RdpConfiguration.SessionMode != RdpSessionMode.External)
        {
            AppendLog("Activate External Window is only meaningful in external mode.");
            return;
        }

        if (_rdpControl.ActivateExternalSessionWindow())
        {
            var hasWindow = _rdpControl.TryGetExternalSessionWindowHandle(out var windowHandle);
            AppendLog($"Activated external window. Handle={(hasWindow ? windowHandle.ToString("X") : "n/a")}.");
            return;
        }

        AppendLog("Could not activate an external session window. The RemoteApp window may not exist yet or may not be owned by the launcher process.");
    }

    private void DisconnectUsingHarness()
    {
        try
        {
            AppendLog("Disconnect requested.");
            _rdpControl.Disconnect();
        }
        catch (Exception ex)
        {
            AppendLog($"Disconnect failed: {ex.Message}");
            MessageBox.Show(this, ex.ToString(), "Disconnect failed", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    private void OpenSettings()
    {
        _propertyGrid.SelectedObject = _rdpControl.RdpConfiguration;
        _propertyGrid.Refresh();
        _settingsForm.Show(this);
    }

    private void SetResizeBehavior(ResizeBehavior resizeBehavior)
    {
        _rdpControl.SetResizeBehavior(resizeBehavior);
        UpdateResizeMenuChecks(resizeBehavior);
        AppendLog($"Resize behavior set to {resizeBehavior}.");
    }

    private void UpdateResizeMenuChecks(ResizeBehavior resizeBehavior)
    {
        _scrollbarsMenuItem.Checked = resizeBehavior == ResizeBehavior.Scrollbars;
        _smartSizingMenuItem.Checked = resizeBehavior == ResizeBehavior.SmartSizing;
        _smartReconnectMenuItem.Checked = resizeBehavior == ResizeBehavior.SmartReconnect;
    }

    private void AppendLog(string message)
    {
        if (InvokeRequired)
        {
            RunOnUiThread(() => AppendLog(message));
            return;
        }

        var entry = $"[{DateTime.Now:HH:mm:ss.fff}] {message}";
        _eventLogListBox.Items.Add(entry);
        _eventLogListBox.TopIndex = _eventLogListBox.Items.Count - 1;
    }

    private void SetCurrentProfile(string profileName)
    {
        _currentProfileName = profileName;
        UpdateStatusSummary();
    }

    private void UpdateStatusSummary()
    {
        if (InvokeRequired)
        {
            RunOnUiThread(UpdateStatusSummary);
            return;
        }

        var configuration = _rdpControl.RdpConfiguration;
        var clientVersion = configuration.ClientVersion == 0 ? "Auto" : configuration.ClientVersion.ToString();
        var remoteAppProgram = string.IsNullOrWhiteSpace(configuration.RemoteApp.Program)
            ? "<empty>"
            : configuration.RemoteApp.Program;

        _profileSummaryLabel.Text =
            $"Current profile: {_currentProfileName}\r\nEffective mode: {configuration.SessionMode}\r\nRemoteApp enabled: {configuration.RemoteApp.Enabled}\r\nRemoteApp program: {remoteAppProgram}";

        _statusLabel.Text =
            $"Profile: {_currentProfileName} | Mode: {configuration.SessionMode} | Client: {clientVersion} | RemoteApp: {remoteAppProgram}";
    }

    private RdpSessionMode GetSelectedSessionMode()
    {
        return _sessionModeComboBox.SelectedItem is RdpSessionMode sessionMode
            ? sessionMode
            : RdpSessionMode.Embedded;
    }

    private int GetSelectedClientVersion()
    {
        return _clientVersionComboBox.SelectedItem?.ToString() switch
        {
            "12" => 12,
            "11" => 11,
            "10" => 10,
            "9" => 9,
            "8" => 8,
            "7" => 7,
            "2" => 2,
            _ => 0
        };
    }

    private void SelectClientVersion(int clientVersion)
    {
        var itemText = clientVersion == 0 ? "Auto" : clientVersion.ToString();
        _clientVersionComboBox.SelectedItem = itemText;
        if (_clientVersionComboBox.SelectedIndex < 0)
            _clientVersionComboBox.SelectedItem = "Auto";
    }

    private static ComboBox CreateDropDownList()
    {
        return new ComboBox
        {
            Dock = DockStyle.Top,
            DropDownStyle = ComboBoxStyle.DropDownList,
            Width = 240
        };
    }

    private static TextBox CreateTextBox(string placeholderText, bool usePasswordChar = false)
    {
        return new TextBox
        {
            Dock = DockStyle.Top,
            PlaceholderText = placeholderText,
            UseSystemPasswordChar = usePasswordChar,
            Width = 240
        };
    }

    private static GroupBox CreateGroupBox(string text, Control content)
    {
        var groupBox = new GroupBox
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            Dock = DockStyle.Top,
            Padding = new Padding(10),
            Text = text
        };
        groupBox.Controls.Add(content);
        return groupBox;
    }

    private static TableLayoutPanel CreateEditorTable()
    {
        return new TableLayoutPanel
        {
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            ColumnCount = 2,
            Dock = DockStyle.Top,
            Padding = new Padding(0),
            Width = 360
        };
    }

    private static void AddRow(TableLayoutPanel table, string labelText, Control editor)
    {
        var rowIndex = table.RowCount++;
        table.RowStyles.Add(new RowStyle(SizeType.AutoSize));

        var label = new Label
        {
            AutoSize = true,
            Dock = DockStyle.Top,
            Margin = new Padding(0, 6, 8, 0),
            Text = labelText,
            TextAlign = ContentAlignment.MiddleLeft
        };

        editor.Margin = new Padding(0, 3, 0, 3);

        table.Controls.Add(label, 0, rowIndex);
        table.Controls.Add(editor, 1, rowIndex);
    }

    private static string? NormalizeText(string? value)
    {
        return string.IsNullOrWhiteSpace(value) ? null : value.Trim();
    }

    private void RunOnUiThread(Action action)
    {
        if (IsDisposed)
            return;

        if (!IsHandleCreated)
        {
            action();
            return;
        }

        if (InvokeRequired)
        {
            BeginInvoke(action);
            return;
        }

        action();
    }

    private void RdpForm_FormClosing(object? sender, FormClosingEventArgs e)
    {
        SaveHarnessState();
    }

    private void SaveHarnessState()
    {
        try
        {
            var currentProfileName = _currentProfileName;
            ApplyHarnessInputs(false);
            _currentProfileName = currentProfileName;
            UpdateStatusSummary();

            var configuration = _rdpControl.RdpConfiguration;
            var state = new DemoHarnessState(
                currentProfileName,
                configuration.SessionMode,
                configuration.ClientVersion,
                configuration.UseMsRdc,
                configuration.Server ?? string.Empty,
                configuration.Credentials.Domain,
                configuration.Credentials.Username,
                configuration.Credentials.NetworkLevelAuthentication,
                configuration.RemoteApp.Enabled,
                configuration.RemoteApp.Program,
                configuration.RemoteApp.Name,
                configuration.RemoteApp.CommandLine,
                configuration.RemoteApp.WorkingDirectory,
                configuration.RemoteApp.File,
                configuration.RemoteApp.DisableCapabilitiesCheck);

            var directoryPath = Path.GetDirectoryName(GetHarnessStatePath());
            if (!string.IsNullOrWhiteSpace(directoryPath))
                Directory.CreateDirectory(directoryPath);

            File.WriteAllText(
                GetHarnessStatePath(),
                JsonSerializer.Serialize(state, JsonSerializerOptions));
        }
        catch (Exception ex)
        {
            AppendLog($"Failed to save harness state: {ex.Message}");
        }
    }

    private DemoHarnessState? LoadHarnessState()
    {
        try
        {
            var path = GetHarnessStatePath();
            if (!File.Exists(path))
                return null;

            var json = File.ReadAllText(path);
            return JsonSerializer.Deserialize<DemoHarnessState>(json, JsonSerializerOptions);
        }
        catch (Exception ex)
        {
            AppendLog($"Failed to load harness state: {ex.Message}");
            return null;
        }
    }

    private static string GetHarnessStatePath()
    {
        return Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "RoyalApps.Community.Rdp.WinForms.Demo",
            "demo-harness-state.json");
    }

    private sealed record DemoHarnessState(
        string CurrentProfileName,
        RdpSessionMode SessionMode,
        int ClientVersion,
        bool UseMsRdc,
        string Server,
        string? Domain,
        string? Username,
        bool NetworkLevelAuthentication,
        bool RemoteAppEnabled,
        string? RemoteAppProgram,
        string? RemoteAppName,
        string? RemoteAppCommandLine,
        string? RemoteAppWorkingDirectory,
        string? RemoteAppFile,
        bool DisableCapabilitiesCheck);
}
