// UI/Views/Prosesİzleme_Control.cs
using System;
using System.Collections.Generic;
using System.Windows.Forms;
using TekstilScada.Models;
using TekstilScada.Services;
using TekstilScada.UI.Controls;

namespace TekstilScada.UI.Views
{
    public partial class Prosesİzleme_Control : UserControl
    {
        public event EventHandler<int> MachineDetailsRequested;
        public event EventHandler<int> MachineVncRequested;

        private PlcPollingService _pollingService;
        private readonly Dictionary<int, MachineCard_Control> _machineCards = new Dictionary<int, MachineCard_Control>();

        public Prosesİzleme_Control()
        {
            InitializeComponent();
            this.DoubleBuffered = true;
        }

        public void InitializeView(List<Machine> machines, PlcPollingService service)
        {
            ClearView();
            _pollingService = service;

            _pollingService.OnMachineDataRefreshed += PollingService_OnMachineDataRefreshed;
            _pollingService.OnMachineConnectionStateChanged += PollingService_OnMachineConnectionStateChanged;
            
            int displayCounter = 1;
            foreach (var machine in machines)
            {
                var card = new MachineCard_Control(machine.Id, machine.MachineUserDefinedId, machine.MachineName, machine.Id);
                card.DetailsRequested += Card_DetailsRequested;
                card.VncRequested += Card_VncRequested;
                _machineCards.Add(machine.Id, card);
                flowLayoutPanelMachines.Controls.Add(card);
            }
        }

        private void Card_DetailsRequested(object sender, EventArgs e)
        {
            var card = sender as MachineCard_Control;
            if (card != null)
            {
                MachineDetailsRequested?.Invoke(this, card.MachineId);
            }
        }

        private void Card_VncRequested(object sender, EventArgs e)
        {
            var card = sender as MachineCard_Control;
            if (card != null)
            {
                MachineVncRequested?.Invoke(this, card.MachineId);
            }
        }

        private void ClearView()
        {
            if (_pollingService != null)
            {
                _pollingService.OnMachineDataRefreshed -= PollingService_OnMachineDataRefreshed;
                _pollingService.OnMachineConnectionStateChanged -= PollingService_OnMachineConnectionStateChanged;
            }

            foreach (var card in _machineCards.Values)
            {
                card.Dispose();
            }
            _machineCards.Clear();
            flowLayoutPanelMachines.Controls.Clear();
        }

        // DÜZELTME: Metot imzası, olayın tanımıyla eşleşecek şekilde güncellendi.
        private void PollingService_OnMachineConnectionStateChanged(int machineId, FullMachineStatus status)
        {
            if (_machineCards.TryGetValue(machineId, out var card))
            {
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.BeginInvoke(new Action(() => {
                        if (!card.IsDisposed)
                        {
                            card.UpdateView(status);
                        }
                    }));
                }
            }
        }

        // DÜZELTME: Metot imzası, olayın tanımıyla eşleşecek şekilde güncellendi.
        private void PollingService_OnMachineDataRefreshed(int machineId, FullMachineStatus status)
        {
            if (_machineCards.TryGetValue(machineId, out var card))
            {
                if (this.IsHandleCreated && !this.IsDisposed)
                {
                    this.BeginInvoke(new Action(() => {
                        if (!card.IsDisposed)
                        {
                            card.UpdateView(status);
                        }
                    }));
                }
            }
        }
    }
}
