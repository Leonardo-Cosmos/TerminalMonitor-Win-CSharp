/* 2021/6/19 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
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
using TerminalMonitor.Clipboard;
using TerminalMonitor.Execution;
using TerminalMonitor.Matchers.Models;
using TerminalMonitor.Models;
using TerminalMonitor.Models.Settings;
using TerminalMonitor.Parsers;
using TerminalMonitor.Terminal;
using Condition = TerminalMonitor.Matchers.Models.Condition;

namespace TerminalMonitor.Windows.Controls
{
    /// <summary>
    /// Interaction logic for TerminalTabControl.xaml
    /// </summary>
    public partial class TerminalTabControl : UserControl
    {
        private ITerminalLineProducer? terminalLineProducer;

        private DispatcherTimer? readTerminalTimer;

        private readonly TerminalSupervisor terminalLineSupervisor = new();

        private readonly DispatcherTimer selectTabTimer;

        private int selectTabIndex = 0;

        private bool changingTab = false;

        private readonly ItemClipboard<FieldDisplayDetail> fieldClipboard = new();

        private readonly ItemClipboard<TextStyleCondition> styleConditionClipboard = new();

        private readonly ItemClipboard<Condition> conditionListClipboard = new();

        private readonly ItemClipboard<Condition> conditionTreeClipboard = new();

        public TerminalTabControl()
        {
            InitializeComponent();

            var defaultTab = (tbCtrl.Items[0] as TabItem)!;
            GetTabTerminalView(defaultTab).TerminalLineSupervisor = terminalLineSupervisor;

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
            var self = (sender as Button)!;

            var tab = GetTab((self.Tag as string)!);
            var tabConfig = GetTabConfig(tab)!;

            var result = MessageBox.Show($"Do you want to close tab \"{tabConfig.Name}\"?\nAll settings in this tab will be deleted.", "Close Terminal Tab",
                MessageBoxButton.YesNo, MessageBoxImage.None);

            if (result == MessageBoxResult.Yes)
            {
                RemoveTab((self.Tag as string)!);
            }
        }

        private void ContextMenu_ContextMenuOpening(object sender, ContextMenuEventArgs e)
        {
            var self = (sender as Border)!;

            if ((self.Tag as string) == "+")
            {
                e.Handled = true;
            }
        }

        private void MnRename_Click(object sender, RoutedEventArgs e)
        {
            var tab = GetMenuTab((sender as MenuItem)!);

            InputWindow window = new()
            {
                Title = "Rename tab",
                Message = "New tab name: ",
                Text = (tab.Header as string)!,
            };

            if (window.ShowDialog() ?? false)
            {
                tab.Header = window.Text;
            }
        }

        private void MnMoveLeft_Click(object sender, RoutedEventArgs e)
        {
            var tab = GetMenuTab((sender as MenuItem)!);

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
            var tab = GetMenuTab((sender as MenuItem)!);

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
            var tab = GetMenuTab((sender as MenuItem)!);
            var terminalConfig = GetTabConfig(tab)!;
            var index = tbCtrl.Items.IndexOf(tab);

            TerminalConfig config = (TerminalConfig)terminalConfig.Clone();
            config.Name = $"{terminalConfig.Name} (Copy)";
            config.Id = Guid.NewGuid().ToString();

            InsertTab(index + 1, config);
        }

        private TabItem GetMenuTab(MenuItem menuItem)
        {
            var menu = (menuItem.Parent as ContextMenu)!;
            var border = (menu.PlacementTarget as Border)!;
            return GetTab((border.Tag as string)!);
        }

        private void StartTimer(ITerminalLineProducer producer)
        {
            readTerminalTimer = new();
            readTerminalTimer.Tick += (sender, e) =>
            {
                var readTerminalLines = producer.ReadTerminalLines();

                terminalLineSupervisor.AddTerminalLines(readTerminalLines);

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
            if (readTerminalTimer == null)
            {
                return;
            }
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

        private static TerminalConfig? GetTabConfig(TabItem tab)
        {
            var terminalView = GetTabTerminalView(tab);
            return new TerminalConfig()
            {
                Id = (tab.Tag as string)!,
                Name = (tab.Header as string)!,
                VisibleFields = terminalView.VisibleFields,
                FilterCondition = terminalView.FilterCondition,
                FindCondition = terminalView.FindCondition,
            };
        }

        private static TerminalView GetTabTerminalView(TabItem tab)
        {
            return (tab.Content as TerminalView)!;
        }

        private TabItem GetTab(string id)
        {
            TabItem matchedItem;
            foreach (var item in tbCtrl.Items)
            {
                if (item is TabItem tab && (tab.Tag as string) == id)
                {
                    matchedItem = tab;
                    return matchedItem;
                }
            }
            throw new ArgumentException($"Invalid {nameof(id)} {id}");
        }

        private TabItem CreateTab(TerminalConfig config)
        {
            TabItem tab = new()
            {
                Tag = config.Id ?? Guid.NewGuid().ToString(),
                Header = config.Name ?? "Unknown View"
            };

            TerminalView terminalView = new()
            {
                VisibleFields = config.VisibleFields?.ToList() ?? [],
                FilterCondition = config.FilterCondition ?? GroupCondition.Empty,
                FindCondition = config.FindCondition ?? GroupCondition.Empty,
                FieldClipboard = fieldClipboard,
                StyleConditionClipboard = styleConditionClipboard,
                FilterListClipboard = conditionListClipboard,
                FilterTreeClipboard = conditionTreeClipboard,
                FindListClipboard = conditionListClipboard,
                FindTreeClipboard = conditionTreeClipboard,
                TerminalLineSupervisor = terminalLineSupervisor
            };
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

            if (index < 0 || index > tbCtrl.Items.Count - 1)
            {
                index = tbCtrl.Items.Count - 1;
            }

            tbCtrl.Items.Insert(index, tab);
        }

        private void RemoveTab(TabItem tab)
        {
            var terminalView = GetTabTerminalView(tab);
            terminalView.TerminalLineSupervisor = null;

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

        private IEnumerable<TerminalConfig> GetTerminalConfigs()
        {
            var columnSettingDict = JsonSerializer.Deserialize<Dictionary<string, GridViewColumnSetting[]>>(
                Properties.TerminalSettings.Default.GridViewColumns)!;

            List<TerminalConfig> terminalConfigs = [];
            var tabCount = tbCtrl.Items.Count;
            for (var i = 0; i < tabCount - 1; i++)
            {
                var tab = (tbCtrl.Items[i] as TabItem)!;

                var terminalConfig = GetTabConfig(tab)!;
                terminalConfigs.Add(terminalConfig);

                /*
                 * Update terminal view UI settings when it is valid.
                 */
                var terminalView = GetTabTerminalView(tab);
                var columnSettings = terminalView.ColumnSettings;
                var isValidColumnSettings = columnSettings.Any(columnSetting => columnSetting.Width > 0);
                if (isValidColumnSettings)
                {
                    columnSettingDict[terminalConfig.Id] = [.. columnSettings];
                }
            }

            Properties.TerminalSettings.Default.GridViewColumns =
                JsonSerializer.Serialize<Dictionary<string, GridViewColumnSetting[]>>(columnSettingDict);

            return terminalConfigs.AsEnumerable();
        }

        private void SetTerminalConfigs(IEnumerable<TerminalConfig>? value)
        {
            var columnSettingDict = JsonSerializer.Deserialize<Dictionary<string, GridViewColumnSetting[]>>(
                Properties.TerminalSettings.Default.GridViewColumns)!;

            changingTab = true;

            /*
             * Remove existing tabs.
             */
            List<TabItem> removeTabs = [];
            var tabCount = tbCtrl.Items.Count;
            for (var i = 0; i < tabCount - 1; i++)
            {
                var tab = (tbCtrl.Items[i] as TabItem)!;

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

                /*
                 * Set terminal view UI settings if it was saved.
                 */
                if (columnSettingDict.TryGetValue(terminalConfig.Id, out GridViewColumnSetting[]? columnSetting))
                {
                    var tab = GetTab(terminalConfig.Id);
                    var terminalView = GetTabTerminalView(tab);
                    terminalView.ColumnSettings = columnSetting;
                }
            }

            tbCtrl.SelectedIndex = 0;

            changingTab = false;
        }

        private void LineProducer_Started(object? sender, EventArgs e)
        {
            if (terminalLineProducer != null)
            {
                StartTimer(terminalLineProducer);
            }
        }

        public ITerminalLineProducer? TerminalLineProducer
        {
            get => terminalLineProducer;
            set
            {
                if (terminalLineProducer != value && terminalLineProducer != null)
                {
                    StopTimer();
                    terminalLineProducer.Started -= LineProducer_Started;
                }

                terminalLineProducer = value;

                if (terminalLineProducer != null)
                {
                    terminalLineProducer.Started += LineProducer_Started;
                }
            }
        }

        public IEnumerable<TerminalConfig>? Terminals
        {
            get => GetTerminalConfigs();
            set => SetTerminalConfigs(value);
        }
    }
}
