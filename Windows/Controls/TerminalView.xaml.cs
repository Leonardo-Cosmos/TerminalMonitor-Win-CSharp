﻿/* 2021/5/9 */
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Interaction logic for TerminalView.xaml
    /// </summary>
    public partial class TerminalView : UserControl
    {
        private readonly List<TerminalTextVO> allTerminalTextVOs = new();
        private readonly ObservableCollection<TerminalTextVO> visibleTerminalTextVOs = new();

        private DispatcherTimer timer;

        private IReadOnlyList<FilterCondition> filterCondtions = new List<FilterCondition>(0);


        public TerminalView()
        {
            InitializeComponent();

            listTerminal.ItemsSource = visibleTerminalTextVOs;
        }

        private void ButtonFilter_Click(object sender, RoutedEventArgs e)
        {
            filterCondtions = filterView.FilterConditions;
            FilterTerminal(filterCondtions);
        }

        private void MenuItemClear_Click(object sender, RoutedEventArgs e)
        {
            ClearTerminal();
        }

        public void AddExecution(IExecution execution)
        {
            StartTimer(execution);
        }

        private void StartTimer(IExecution execution)
        {
            timer = new();
            timer.Tick += (sender, e) =>
            {
                var lines = execution.ReadTerminalLines();
                foreach (var line in lines)
                {
                    AddTerminalLine(line);
                }

                if (execution.IsCompleted)
                {
                    timer.Stop();
                    AddTerminalLine("Task is completed");
                }

            };
            timer.Interval = new TimeSpan(0, 0, 1);
            timer.Start();
        }

        private void PauseTimer()
        {
            timer.Stop();
        }

        private void ResumeTimer()
        {
            timer.Start();
        }

        private void AddTerminalLine(string text)
        {
            var terminalTextVO = JsonParser.ParseTerminalLine(text);
            allTerminalTextVOs.Add(terminalTextVO);

            if (terminalTextVO.IsMatch(filterCondtions))
            {
                visibleTerminalTextVOs.Add(terminalTextVO);
                listTerminal.ScrollIntoView(terminalTextVO);
            }
        }

        private void ClearTerminal()
        {
            PauseTimer();

            allTerminalTextVOs.Clear();
            visibleTerminalTextVOs.Clear();

            ResumeTimer();
        }

        private void FilterTerminal(IEnumerable<FilterCondition> filterConditions)
        {
            PauseTimer();

            visibleTerminalTextVOs.Clear();
            foreach (var terminalTextVO in allTerminalTextVOs)
            {
                if (terminalTextVO.IsMatch(filterConditions))
                {
                    visibleTerminalTextVOs.Add(terminalTextVO);
                }
            }

            ResumeTimer();
        }
    }
}
