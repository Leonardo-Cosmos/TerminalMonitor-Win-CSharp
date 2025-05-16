/* 2021/11/8 */
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;
using TerminalMonitor.Models;
using TerminalMonitor.Models.Settings;

namespace TerminalMonitor.Windows.Controls
{
    class TerminalViewColumnSettingHelper
    {
        /// <summary>
        /// A dictionary between original field ID and cloned field ID.
        /// </summary>
        private readonly Dictionary<string, string> fieldIdDict = [];

        /// <summary>
        /// A dictionary between cloned field ID and original field ID.
        /// </summary>
        private readonly Dictionary<string, string> fieldCloneIdDict = [];

        /// <summary>
        /// A dictionary between cloned field ID and grid view column.
        /// </summary>
        private readonly Dictionary<string, GridViewColumn> gridViewColumnDict = [];

        public List<FieldDisplayDetail> Init(IEnumerable<FieldDisplayDetail> fieldDetails)
        {
            fieldIdDict.Clear();
            fieldCloneIdDict.Clear();
            List<FieldDisplayDetail> fieldDetailClones = [];

            foreach (var fieldDetail in fieldDetails)
            {
                var fieldDetailClone = (FieldDisplayDetail)fieldDetail.Clone();

                fieldIdDict[fieldDetail.Id] = fieldDetailClone.Id;
                fieldCloneIdDict[fieldDetailClone.Id] = fieldDetail.Id;
                
                fieldDetailClones.Add(fieldDetailClone);
            }

            gridViewColumnDict.Clear();
            return fieldDetailClones;
        }

        public void AddColumn(string fieldId, GridViewColumn viewColumn)
        {
            gridViewColumnDict[fieldId] = viewColumn;
        }

        public IEnumerable<GridViewColumnSetting> GetGridViewColumnSettings()
        {
            return [.. gridViewColumnDict.Select(keyValuePair =>
            {
                string fieldCloneId = keyValuePair.Key;
                var column = keyValuePair.Value;
                return new GridViewColumnSetting(
                    FieldId: fieldCloneIdDict[fieldCloneId],
                    Width: column.ActualWidth
                    );
            })];
        }

        public void SetGridViewColumnSettings(IEnumerable<GridViewColumnSetting> gridViewColumnSettings)
        {
            foreach (var columnSetting in gridViewColumnSettings)
            {
                if (fieldIdDict.TryGetValue(columnSetting.FieldId, out string? fieldCloneId))
                {
                    if (gridViewColumnDict.TryGetValue(fieldCloneId, out GridViewColumn? column))
                    {
                        column.Width = columnSetting.Width;
                    }
                }
            }
        }
    }
}
