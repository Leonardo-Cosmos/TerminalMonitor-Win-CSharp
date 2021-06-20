/* 2021/6/19 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;
using TerminalMonitor.Execution;
using TerminalMonitor.Models;
using TerminalMonitor.Parsers;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for TerminalTabControl.xaml
    /// </summary>
    public partial class TerminalTabControl : UserControl, ITerminalLineSupervisor
    {
        private ITerminalLineProducer lineProducer;

        private DispatcherTimer readTerminalTimer;

        private readonly List<TerminalLineDto> terminalLineDtos = new();

        private readonly List<TerminalView> terminalViews = new();

        private readonly DispatcherTimer selectTabTimer;

        private int selectTabIndex = 0;

        private bool addingNewTab = false;

        public TerminalTabControl()
        {
            InitializeComponent();

            var defaultTab = tbCtrl.Items[0] as TabItem;
            (defaultTab.Content as TerminalView).LineSupervisor = this;

            //AddTab(new()
            //{
            //    Id = Guid.NewGuid().ToString(),
            //    Name = "Default",
            //});

            selectTabTimer = new();
            selectTabTimer.Tick += (sender, e) =>
            {
                tbCtrl.SelectedIndex = selectTabIndex;
                addingNewTab = false;
                selectTabTimer.Stop();
            };
            selectTabTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }

        private void TbCtrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!addingNewTab && tbCtrl.SelectedIndex == tbCtrl.Items.Count - 1)
            {
                TerminalConfig config = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "New",
                };

                addingNewTab = true;
                AddTab(config);

                // Set selected tab after "SelectionChanged" event.
                selectTabIndex = tbCtrl.Items.Count - 2;
                selectTabTimer.Start();
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            var self = sender as Button;

            RemoveTab(self.Tag as string);
        }

        private void MnRename_Click(object sender, RoutedEventArgs e)
        {

        }

        private void StartTimer(ITerminalLineProducer producer)
        {
            readTerminalTimer = new();
            readTerminalTimer.Tick += (sender, e) =>
            {
                var lines = producer.ReadTerminalLines();
                foreach (var line in lines)
                {
                    ParseTerminalLine(line);
                }

                if (producer.IsCompleted)
                {
                    readTerminalTimer.Stop();
                }
            };
            readTerminalTimer.Interval = new TimeSpan(0, 0, 1);
            readTerminalTimer.Start();
        }

        private void StopTimer()
        {
            readTerminalTimer.Stop();
            readTerminalTimer = null;
        }

        private void PauseTimer()
        {
            if (readTerminalTimer == null)
            {
                return;
            }
            readTerminalTimer.Stop();
        }

        private void ResumeTimer()
        {
            if (readTerminalTimer == null)
            {
                return;
            }
            readTerminalTimer.Start();
        }

        private void ParseTerminalLine(string text)
        {
            var terminalLineDto = JsonParser.ParseTerminalLineToVO(text);
            terminalLineDtos.Add(terminalLineDto);

            OnTerminalLineAdded(terminalLineDto);
        }

        private void AddTab(TerminalConfig config)
        {
            TabItem tab = new();
            tab.Tag = config.Id;
            tab.Header = config.Name;

            TerminalView terminalView = new();
            terminalView.VisibleFields = config.VisibleFields;
            terminalView.FilterConditions = config.FilterConditions;
            terminalView.LineSupervisor = this;
            tab.Content = terminalView;

            tbCtrl.Items.Insert(tbCtrl.Items.Count - 1, tab);
        }

        private void RemoveTab(string id)
        {
            if (tbCtrl.Items.Count == 2)
            {
                // Cannot remove last tab.
                return;
            }

            TabItem removedItem = null;
            foreach (var item in tbCtrl.Items)
            {
                if (item is TabItem tab && (tab.Tag as string) == id)
                {
                    removedItem = tab;
                    break;
                }
            }

            if (removedItem != null)
            {
                var removedIndex = tbCtrl.Items.IndexOf(removedItem);
                if (removedIndex == tbCtrl.Items.Count - 2)
                {
                    // Select the tab before the removed one when it is the one previous to "new" tab.
                    tbCtrl.SelectedIndex = tbCtrl.Items.Count - 3;
                }

                tbCtrl.Items.Remove(removedItem);
            }
        }

        protected void OnTerminalLineAdded(TerminalLineDto terminalLineDto)
        {
            TerminalLineEventArgs e = new()
            {
                TerminalLine = terminalLineDto,
            };

            TerminalLineAdded?.Invoke(this, e);
        }

        public IEnumerable<TerminalLineDto> TerminalLines
        {
            get
            {
                var lines = terminalLineDtos.AsEnumerable();
                return lines;
            }
        }

        public event TerminalLineEventHandler TerminalLineAdded;

        private void LineProducer_Started(object sender, EventArgs e)
        {
            StartTimer(lineProducer);
        }

        public ITerminalLineProducer LineProducer
        {
            get => lineProducer;
            set
            {
                if (lineProducer != value && lineProducer != null)
                {
                    StopTimer();
                    lineProducer.Started -= LineProducer_Started;
                }

                lineProducer = value;

                if (lineProducer != null)
                {
                    lineProducer.Started += LineProducer_Started;
                }
            }
        }

        public TerminalConfig Terminals
        {
            get; set;
        }
    }
}
