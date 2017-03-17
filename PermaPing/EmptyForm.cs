using System;
using System.ComponentModel;
using System.Net.NetworkInformation;
using System.Windows.Forms;
using Properties;

public partial class EmptyForm : Form
{
    private readonly NotifyIcon _trayIcon = new NotifyIcon();
    private PingState _state;
    private BackgroundWorker _worker = new BackgroundWorker();

    public EmptyForm()
    {
        InitializeComponent();
        Initialize();
    }

    private void Initialize()
    {
        WindowState = FormWindowState.Minimized;
        LoadTitle();
        InitializeTrayIcon();
        InitializeBackgroundWorker();
    }


    private void InitializeTrayIcon()
    {
        _trayIcon.Text = Resources.MainName + Environment.NewLine + Resources.Connected;
        _trayIcon.Visible = true;
        _trayIcon.Icon = Resources.ConnectedIcon;
    }

    private void InitializeBackgroundWorker()
    {
        _worker = new BackgroundWorker();
        _worker.DoWork += Ping;
        _worker.RunWorkerAsync();
    }

    private void Ping(object sender, DoWorkEventArgs doWorkEventArgs)
    {
        while (true)
            Ping();
        // ReSharper disable once FunctionNeverReturns
    }

    protected override void SetVisibleCore(bool value)
    {
        base.SetVisibleCore(false);
    }

    private void LoadTitle()
    {
        Text = Application.ProductName + @" " + Application.ProductVersion;
    }

    private void Ping()
    {
        var pingSender = new Ping();
        var reply = pingSender.Send("8.8.8.8");

        if (reply != null && reply.Status == IPStatus.Success)
            AnalyseState(PingState.Success);
        else
            AnalyseState(PingState.Fail);
    }

    private void SetIconSuccess()
    {
        _trayIcon.Icon = Resources.ConnectedIcon;
        _trayIcon.BalloonTipTitle = Resources.InternetConnectionReestablished;
        _trayIcon.BalloonTipText = Resources.InternetConnectionReestablished;
        _trayIcon.Text = Resources.MainName + Environment.NewLine + Resources.Connected;
        _trayIcon.ShowBalloonTip(1000);
    }

    private void SetIconFail()
    {
        _trayIcon.Icon = Resources.NotConnectedIcon;
        _trayIcon.BalloonTipTitle = Resources.InternetConnectionLost;
        _trayIcon.BalloonTipText = Resources.InternetConnectionLost;
        _trayIcon.Text = Resources.MainName + Environment.NewLine + Resources.NotConnected;
        _trayIcon.ShowBalloonTip(1000);
    }

    private void AnalyseState(PingState state)
    {
        if (_state == state) return;
        switch (state)
        {
            case PingState.Success:
                _state = state;
                SetIconSuccess();
                break;
            case PingState.Fail:
                _state = state;
                SetIconFail();
                break;
        }
    }
}