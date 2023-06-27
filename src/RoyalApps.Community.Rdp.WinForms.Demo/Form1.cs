using System;
using System.Windows.Forms;
using RoyalApps.Community.Rdp.WinForms;

namespace RoyalApps.Community.Rdp.Demo;

public partial class Form1 : Form
{
    private readonly RdpControl _rdpControl;
    
    public Form1()
    {
        InitializeComponent();

        var connect = new ToolStripMenuItem("Connect");
        var disconnect = new ToolStripMenuItem("Disconnect");
        var remoteDesktopMenu = new ToolStripMenuItem("Remote Desktop")
        {
            DropDownItems = { connect, disconnect }
        };

        _rdpControl = new RdpControl
        {
            Dock = DockStyle.Fill,
            Parent = this,
        };

        new MenuStrip
        {
            Dock = DockStyle.Top,
            Parent = this,
            Items = { remoteDesktopMenu }
        };
        
        connect.Click += Connect_Click;
        disconnect.Click += Disconnect_Click;
        
        _rdpControl.OnConnected += RdpControl_OnConnected;
    }

    private void RdpControl_OnConnected(object? sender, ConnectedEventArgs e)
    {
        
    }

    /// <summary>
    ///  Clean up any resources being used.
    /// </summary>
    /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
    protected override void Dispose(bool disposing)
    {
        if (disposing && components != null)
        {
            components.Dispose();
        }

        base.Dispose(disposing);
    }

    private void Connect_Click(object? sender, EventArgs e)
    {
        _rdpControl.RdpConfiguration.Server = "10.0.0.200";
        _rdpControl.RdpConfiguration.Credentials.Username = "Administrator";
        _rdpControl.RdpConfiguration.Credentials.Password = "Administrator";
        _rdpControl.Connect();
    }
    
    private void Disconnect_Click(object? sender, EventArgs e)
    {
        _rdpControl.Disconnect();
    }
}