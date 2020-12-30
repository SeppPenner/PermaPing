// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EmptyForm.cs" company="Hämmer Electronics">
//   Copyright (c) All rights reserved.
// </copyright>
// <summary>
//   The main form.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace PermaPing
{
    using System;
    using System.ComponentModel;
    using System.Net.NetworkInformation;
    using System.Windows.Forms;

    using Properties;

    /// <summary>
    /// The main form.
    /// </summary>
    public partial class EmptyForm : Form
    {
        /// <summary>
        /// The tray icon.
        /// </summary>
        private readonly NotifyIcon trayIcon = new NotifyIcon();

        /// <summary>
        /// The state.
        /// </summary>
        private PingState state;

        /// <summary>
        /// The background worker.
        /// </summary>
        private BackgroundWorker worker = new BackgroundWorker();

        /// <summary>
        /// Initializes a new instance of the <see cref="EmptyForm"/> class.
        /// </summary>
        public EmptyForm()
        {
            this.InitializeComponent();
            this.Initialize();
        }

        /// <summary>
        /// Sets the visible core.
        /// </summary>
        /// <param name="value">A value indicating whether the application is shown or not.</param>
        protected override void SetVisibleCore(bool value)
        {
            base.SetVisibleCore(false);
        }

        /// <summary>
        /// Initializes the data.
        /// </summary>
        private void Initialize()
        {
            this.WindowState = FormWindowState.Minimized;
            this.LoadTitle();
            this.InitializeTrayIcon();
            this.InitializeBackgroundWorker();
        }

        /// <summary>
        /// Initializes the tray icon.
        /// </summary>
        private void InitializeTrayIcon()
        {
            this.trayIcon.Text = Resources.MainName + Environment.NewLine + Resources.Connected;
            this.trayIcon.Visible = true;
            this.trayIcon.Icon = Resources.ConnectedIcon;
        }

        /// <summary>
        /// Initializes the background worker.
        /// </summary>
        private void InitializeBackgroundWorker()
        {
            this.worker = new BackgroundWorker();
            this.worker.DoWork += this.Ping;
            this.worker.RunWorkerAsync();
        }

        /// <summary>
        /// Runs a permanent ping.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="doWorkEventArgs">The event args.</param>
        private void Ping(object sender, DoWorkEventArgs doWorkEventArgs)
        {
            while (true)
            {
                this.Ping();
            }
            // ReSharper disable once FunctionNeverReturns
        }

        /// <summary>
        /// Loads the title.
        /// </summary>
        private void LoadTitle()
        {
            this.Text = Application.ProductName + @" " + Application.ProductVersion;
        }

        /// <summary>
        /// Runs one ping.
        /// </summary>
        private void Ping()
        {
            var pingSender = new Ping();
            var reply = pingSender.Send("8.8.8.8");

            if (reply != null && reply.Status == IPStatus.Success)
            {
                this.AnalyzeState(PingState.Success);
            }
            else
            {
                this.AnalyzeState(PingState.Fail);
            }
        }

        /// <summary>
        /// Sets the success icon.
        /// </summary>
        private void SetIconSuccess()
        {
            this.trayIcon.Icon = Resources.ConnectedIcon;
            this.trayIcon.BalloonTipTitle = Resources.InternetConnectionReestablished;
            this.trayIcon.BalloonTipText = Resources.InternetConnectionReestablished;
            this.trayIcon.Text = Resources.MainName + Environment.NewLine + Resources.Connected;
            this.trayIcon.ShowBalloonTip(1000);
        }

        /// <summary>
        /// Sets the fail icon.
        /// </summary>
        private void SetIconFail()
        {
            this.trayIcon.Icon = Resources.NotConnectedIcon;
            this.trayIcon.BalloonTipTitle = Resources.InternetConnectionLost;
            this.trayIcon.BalloonTipText = Resources.InternetConnectionLost;
            this.trayIcon.Text = Resources.MainName + Environment.NewLine + Resources.NotConnected;
            this.trayIcon.ShowBalloonTip(1000);
        }

        /// <summary>
        /// Analyzes the state.
        /// </summary>
        /// <param name="state">The state.</param>
        private void AnalyzeState(PingState state)
        {
            if (this.state == state)
            {
                return;
            }

            switch (state)
            {
                case PingState.Success:
                    this.state = state;
                    this.SetIconSuccess();
                    break;
                case PingState.Fail:
                    this.state = state;
                    this.SetIconFail();
                    break;
            }
        }
    }
}