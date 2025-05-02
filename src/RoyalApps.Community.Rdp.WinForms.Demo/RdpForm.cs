using System.Drawing;
using System.Windows.Forms;
using RoyalApps.Community.Rdp.WinForms.Configuration;
using RoyalApps.Community.Rdp.WinForms.Controls;

namespace RoyalApps.Community.Rdp.Demo;

public partial class RdpForm : Form
{
    private readonly Form _form;
    private readonly RdpControl _rdpControl;

    public RdpForm()
    {
        InitializeComponent();

        _form = new Form
        {
            Size = new Size(800, 1000),
            Text = "Settings",
            FormBorderStyle = FormBorderStyle.SizableToolWindow,
            StartPosition = FormStartPosition.CenterParent
        };
        _form.Closing += (_, args) =>
        {
            _form.Hide();
            args.Cancel = true;
        };

        var propertyGrid = new PropertyGrid
        {
            PropertySort = PropertySort.NoSort,
            Dock = DockStyle.Fill,
            Parent = _form
        };

        _rdpControl = new RdpControl
        {
            Dock = DockStyle.Fill,
            Parent = this,
        };

        var connect = new ToolStripMenuItem("Connect");
        connect.Click += (_, _) => _rdpControl.Connect();

        var disconnect = new ToolStripMenuItem("Disconnect");
        disconnect.Click += (_, _) => _rdpControl.Disconnect();

        var settings = new ToolStripMenuItem("Settings...");
        settings.Click += (_, _) =>
        {
            propertyGrid.SelectedObject = _rdpControl.RdpConfiguration;
            _form.Show(this);
        };

        var updateDisplay = new ToolStripMenuItem("Update Display");
        updateDisplay.Click += (_, _) => _rdpControl.UpdateClientSize();

        var scrollbars = new ToolStripMenuItem("Scrollbars");
        var smartSizing = new ToolStripMenuItem("Smart Sizing");
        var smartReconnect = new ToolStripMenuItem("Smart Reconnect");

        scrollbars.Click += (_, _) =>
        {
            scrollbars.Checked = true;
            smartSizing.Checked = false;
            smartReconnect.Checked = false;
            _rdpControl.SetResizeBehavior(ResizeBehavior.Scrollbars);
        };
        smartSizing.Click += (_, _) =>
        {
            scrollbars.Checked = false;
            smartSizing.Checked = true;
            smartReconnect.Checked = false;
            _rdpControl.SetResizeBehavior(ResizeBehavior.SmartSizing);
        };
        smartReconnect.Click += (_, _) =>
        {
            scrollbars.Checked = false;
            smartSizing.Checked = false;
            smartReconnect.Checked = true;
            _rdpControl.SetResizeBehavior(ResizeBehavior.SmartReconnect);
        };

        var connectionInfo = new ToolStripMenuItem("Show Connection Info");
        connectionInfo.Click += (_, _) =>
        {
            _rdpControl.ShowConnectionInfo();
        };

        var zoomIn = new ToolStripMenuItem("Zoom In");
        zoomIn.Click += (_, _) => _rdpControl.ZoomIn();
        var zoomOut = new ToolStripMenuItem("Zoom Out");
        zoomOut.Click += (_, _) => _rdpControl.ZoomOut();

        var remoteDesktopMenu = new ToolStripMenuItem("Remote Desktop")
        {
            DropDownItems =
            {
                connect,
                disconnect,
                new ToolStripSeparator(),
                connectionInfo,
                new ToolStripSeparator(),
                settings
            }
        };

        var changeMenu = new ToolStripMenuItem("Change")
        {
            Enabled = false,
            DropDownItems =
            {
                updateDisplay,
                new ToolStripSeparator(),
                new ToolStripLabel("Resize Behavior") { Enabled = false },
                scrollbars,
                smartSizing,
                smartReconnect,
                new ToolStripSeparator(),
                new ToolStripLabel("Zoom") { Enabled = false },
                zoomIn,
                zoomOut,
            }
        };

        var _ = new MenuStrip
        {
            Dock = DockStyle.Top,
            Parent = this,
            Items = { remoteDesktopMenu, changeMenu }
        };

        // Sample Connection
        // _rdpControl.RdpConfiguration.Server = "10.0.0.200";
        // _rdpControl.RdpConfiguration.Credentials.Username = "Administrator";
        // _rdpControl.RdpConfiguration.Credentials.Password = new SensitiveString("Administrator");


        _rdpControl.OnConnected += (_, _) =>
        {
            changeMenu.Enabled = true;
            scrollbars.Checked = _rdpControl.RdpConfiguration.Display.ResizeBehavior == ResizeBehavior.Scrollbars;
            smartSizing.Checked = _rdpControl.RdpConfiguration.Display.ResizeBehavior == ResizeBehavior.SmartSizing;
            smartReconnect.Checked = _rdpControl.RdpConfiguration.Display.ResizeBehavior == ResizeBehavior.SmartReconnect;
        };
        _rdpControl.OnDisconnected += (_, _) =>
        {
            changeMenu.Enabled = false;
        };
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }
}
