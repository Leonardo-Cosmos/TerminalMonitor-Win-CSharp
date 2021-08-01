﻿/* 2021/6/19 */
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
            GetTabTerminalView(defaultTab).LineSupervisor = this;

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

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var self = sender as Border;

            if ((self.Tag as string) == "+")
            {
                e.Handled = true;
            }
        }

        private void MnRename_Click(object sender, RoutedEventArgs e)
        {
            var tab = GetMenuTab(sender as MenuItem);

            InputWindow window = new()
            {
                Title = "Rename tab",
                Message = "New tab name: ",
                Text = tab.Header as string,
            };

            if (window.ShowDialog() ?? false)
            {
                tab.Header = window.Text;
            }
        }

        private void MnMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            var tab = GetMenuTab(sender as MenuItem);

            var index = tbCtrl.Items.IndexOf(tab);
            if (index > 0)
            {
                changingTab = true;

                tbCtrl.Items.Remove(tab);
                tbCtrl.Items.Insert(index - 1, tab);

                changingTab = false;
            }
        }

        private void MnMoveRight_Click(object sender, RoutedEventArgs e)
        {
            var tab = GetMenuTab(sender as MenuItem);

            var index = tbCtrl.Items.IndexOf(tab);
            if (index < tbCtrl.Items.Count - 2)
            {
                changingTab = true;

                tbCtrl.Items.Remove(tab);
                tbCtrl.Items.Insert(index + 1, tab);
                tbCtrl.SelectedIndex = index + 1;

                changingTab = false;
            }
        }

        private void MnDuplicate_Click(object sender, RoutedEventArgs e)
        {
            var tab = GetMenuTab(sender as MenuItem);
            var terminalConfig = GetTabConfig(tab);
            var index = tbCtrl.Items.IndexOf(tab);

            TerminalConfig config = (TerminalConfig) terminalConfig.Clone();
            config.Name = $"{terminalConfig.Name} (Copy)";
            config.Id = Guid.NewGuid().ToString();

            InsertTab(index + 1, config);
        }

        private TabItem GetMenuTab(MenuItem menuItem)
        {
            var menu = menuItem.Parent as ContextMenu;
            var border = menu.PlacementTarget as Border;
            return GetTab(border.Tag as string);
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

        private static TerminalConfig GetTabConfig(TabItem tab)
        {
            var terminalView = GetTabTerminalView(tab);
            return new TerminalConfig()
            {
                Id = tab.Tag as string,
                Name = tab.Header as string,
                VisibleFields = terminalView.VisibleFields,
                FilterCondition = terminalView.FilterCondition,
            };
        }

        private static TerminalView GetTabTerminalView(TabItem tab)
        {
            return tab.Content as TerminalView;
        }

        private TabItem GetTab(string id)
        {
            TabItem matchedItem = null;
            foreach (var item in tbCtrl.Items)
            {
                if (item is TabItem tab && (tab.Tag as string) == id)
                {
                    matchedItem = tab;
                    break;
                }
            }
            return matchedItem;
        }

        private TabItem CreateTab(TerminalConfig config)
        {
            TabItem tab = new();
            tab.Tag = config.Id ?? Guid.NewGuid().ToString();
            tab.Header = config.Name ?? "Unknown View";

            TerminalView terminalView = new();
            terminalView.VisibleFields = config.VisibleFields;
            terminalView.FilterCondition = config.FilterCondition;
            terminalView.LineSupervisor = this;
            tab.Content = terminalView;

            return tab;
        }

        private void AddTab(TerminalConfig config)
        {
            var tab = CreateTab(config);

            tbCtrl.Items.Insert(tbCtrl.Items.Count - 1, tab);
        }

        private void InsertTab(int index, TerminalConfig config)
        {
            var tab = CreateTab(config);

            if (index < 0 || index > tbCtrl.Items.Count -1)
            {
                index = tbCtrl.Items.Count - 1;
            }

            tbCtrl.Items.Insert(index, tab);
        }

        private void RemoveTab(TabItem tab)
        {
            var terminalView = GetTabTerminalView(tab);
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

            TabItem removedItem = GetTab(id);

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

                    terminalConfigs.Add(GetTabConfig(tab));
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
