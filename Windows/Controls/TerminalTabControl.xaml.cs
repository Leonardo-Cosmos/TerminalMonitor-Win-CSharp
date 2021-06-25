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

        private bool changingTab = false;

        public TerminalTabControl()
        {
            InitializeComponent();

            var defaultTab = tbCtrl.Items[0] as TabItem;
            (defaultTab.Content as TerminalView).LineSupervisor = this;

            selectTabTimer = new();
            selectTabTimer.Tick += (sender, e) =>
            {
                tbCtrl.SelectedIndex = selectTabIndex;
                changingTab = false;
                selectTabTimer.Stop();
            };
            selectTabTimer.Interval = new TimeSpan(0, 0, 0, 0, 1);
        }

        private void TbCtrl_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (!changingTab && tbCtrl.SelectedIndex == tbCtrl.Items.Count - 1)
            {
                // Set selected tab after "SelectionChanged" event.
                selectTabIndex = tbCtrl.Items.Count - 2;
                selectTabTimer.Start();
                tbCtrl.SelectedItem = selectTabIndex;

                TerminalConfig config = new()
                {
                    Id = Guid.NewGuid().ToString(),
                    Name = "New View",
                };

                changingTab = true;
                AddTab(config);
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
                var terminalLines = producer.ReadTerminalLines();
                foreach (var terminalLine in terminalLines)
                {
                    ParseTerminalLine(terminalLine.Text, terminalLine.ExecutionName);
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

        private void ParseTerminalLine(string text, string executionName)
        {
            TerminalLineDto terminalLineDto =
                TerminalLineParser.ParseTerminalLine(text, executionName);

            terminalLineDtos.Add(terminalLineDto);

            OnTerminalLineAdded(terminalLineDto);
        }

        private static TerminalConfig GetConfig(TabItem tab)
        {
            var terminalView = tab.Content as TerminalView;
            return new TerminalConfig()
            {
                Id = tab.Tag as string,
                Name = tab.Header as string,
                VisibleFields = terminalView.VisibleFields,
                FilterConditions = terminalView.FilterConditions,
            };
        }

        private void AddTab(TerminalConfig config)
        {
            TabItem tab = new();
            tab.Tag = config.Id ?? Guid.NewGuid().ToString();
            tab.Header = config.Name ?? "Unknown View";

            TerminalView terminalView = new();
            terminalView.VisibleFields = config.VisibleFields;
            terminalView.FilterConditions = config.FilterConditions;
            terminalView.LineSupervisor = this;
            tab.Content = terminalView;

            tbCtrl.Items.Insert(tbCtrl.Items.Count - 1, tab);
        }

        private void RemoveTab(TabItem tab)
        {
            var terminalView = tab.Content as TerminalView;
            terminalView.LineSupervisor = null;

            tbCtrl.Items.Remove(tab);
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

                RemoveTab(removedItem);
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

        public TerminalLineCollection TerminalLines
        {
            get
            {
                return new TerminalLineCollection(terminalLineDtos.AsEnumerable());
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

        public IEnumerable<TerminalConfig> Terminals
        {
            get
            {
                List<TerminalConfig> terminalConfigs = new();
                var tabCount = tbCtrl.Items.Count;
                for (var i = 0; i < tabCount - 1; i++)
                {
                    var tab = tbCtrl.Items[i] as TabItem;

                    terminalConfigs.Add(GetConfig(tab));
                }
                return terminalConfigs.AsEnumerable();
            }

            set
            {
                changingTab = true;

                /*
                 * Remove existing tabs.
                 */
                List<TabItem> removeTabs = new();
                var tabCount = tbCtrl.Items.Count;
                for (var i = 0; i < tabCount - 1; i++)
                {
                    var tab = tbCtrl.Items[i] as TabItem;

                    removeTabs.Add(tab);
                }
                foreach (var tab in removeTabs)
                {
                    RemoveTab(tab);
                }


                if (value == null)
                {
                    // Add default tab.
                    AddTab(new()
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = "Default",
                    });

                    tbCtrl.SelectedIndex = 0;

                    return;
                }

                /*
                 * Add new tabs.
                 */
                foreach (var terminalConfig in value)
                {
                    AddTab(terminalConfig);
                }

                tbCtrl.SelectedIndex = 0;

                changingTab = false;
            }
        }
    }
}
