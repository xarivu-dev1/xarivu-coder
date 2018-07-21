using Xarivu.Coder.Component.ObjectView.Converter;
using Xarivu.Coder.Component.ObjectView.ViewModel;
using Xarivu.Coder.Utilities;
using Xarivu.Coder.ViewModel.Dialog;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;

namespace Xarivu.Coder.Component.ObjectView.View
{
    /// <summary>
    /// Interaction logic for REPropertiesGrid.xaml
    /// </summary>
    public partial class ObjectGrid : UserControl
    {
        DialogServiceViewModel dialogServiceViewModel;

        object[] rootDataArray;

        /// <summary>
        /// Represents the current set of items (row data) displayed on the object grid.
        /// This is the un-filtered view. If an item type from the combo box is selected or defaulted, then the grid items source
        /// is bound to a subset of this.
        /// When drill operation is used the current data array elements will be different from the root data array.
        /// </summary>
        object[] currentDataArray;

        string drillPath;

        public ObjectGrid()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                this.dialogServiceViewModel = DependencyContainer.Get<DialogServiceViewModel>();
            }

            this.GridItemTypesComboBox.SelectionChanged += GridItemTypesComboBox_SelectionChanged;
            this.DrillUpButton.Click += DrillUpButton_Click;

            UpdateRootData();
            this.DataContextChanged += ObjectGrid_DataContextChanged;

            this.REPropertiesDataGrid.SelectionChanged += REPropertiesDataGrid_SelectionChanged;
            this.REPropertiesDataGrid.SelectedCellsChanged += REPropertiesDataGrid_SelectedCellsChanged;
        }

        private void ObjectGrid_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var oldVm = e.OldValue as ObjectListViewModel;
            var newVm = e.NewValue as ObjectListViewModel;
            RegisterVMPropertyChangedEventHandlers(oldVm, newVm);
        }

        private void REPropertiesDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var vm = (this.DataContext as ObjectListViewModel);
            if (vm != null)
            {
                RowDataHolder rowDataHolder = this.REPropertiesDataGrid.SelectedItem as RowDataHolder;
                if (rowDataHolder != null)
                {
                    vm.SelectedItem = rowDataHolder.Data;
                }
            }
        }

        private void REPropertiesDataGrid_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var vm = (this.DataContext as ObjectListViewModel);
            if (vm == null) return;

            var selectedCells = this.REPropertiesDataGrid.SelectedCells;
            if (selectedCells == null || selectedCells.Count == 0)
            {
                vm.SelectedItem = null;
            }

            var selectedItem = selectedCells[0].Item;
            RowDataHolder rowDataHolder = selectedItem as RowDataHolder;
            if (rowDataHolder != null)
            {
                vm.SelectedItem = rowDataHolder.Data;
            }
        }

        public string DrillPath
        {
            get
            {
                return this.drillPath;
            }

            private set
            {
                if (this.drillPath != value)
                {
                    this.drillPath = value;
                    this.DrillPathTextBox.Text = this.drillPath;
                }
            }
        }

        void RegisterVMPropertyChangedEventHandlers(ObjectListViewModel oldVm, ObjectListViewModel newVm)
        {
            if (oldVm != null)
            {
                oldVm.PropertyChanged -= ObjectViewModel_PropertyChanged;
            }

            if (newVm != null)
            {
                UpdateRootData();
                newVm.PropertyChanged += ObjectViewModel_PropertyChanged;
            }
        }

        void ObjectViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ObjectListViewModel.ObjectList))
            {
                UpdateRootData();
            }
        }

        void UpdateRootData()
        {
            var vm = this.DataContext as ObjectListViewModel;
            if (vm != null)
            {
                var data = new object[0];
                if (vm.ObjectList != null)
                {
                    data = vm.ObjectList.ToArray();
                }

                SetData(data);
            }
        }

        private void GridItemTypesComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GetDataAndBind();
        }

        private void DrillUpButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(this.DrillPath)) return;

            int i = this.DrillPath.LastIndexOf("[");
            if (i != -1)
            {
                this.DrillPath = this.DrillPath.Substring(0, i);
            }
            else
            {
                this.DrillPath = string.Empty;
            }

            GetDataAndBind();
        }

        public void SetData<T>(T[] dataArray)
        {
            this.rootDataArray = dataArray?.Cast<object>().ToArray();
            this.DrillPath = string.Empty;

            GetDataAndBind();
        }

        void GetDataAndBind()
        {
            // The data array for the data grid items. Each element represents a row.
            this.currentDataArray = this.rootDataArray;

            if (this.currentDataArray != null)
            {
                if (!string.IsNullOrEmpty(DrillPath))
                {
                    // Drill down to a specific path in the object and show values from that property in the cells.
                    // For example property.SaleFeatures.SalePricingInfo
                    var pathProperties = DrillPath.Split(new[] { '[' }, StringSplitOptions.RemoveEmptyEntries);
                    foreach (string pathCompWithIndex in pathProperties)
                    {
                        var bracketEnd = pathCompWithIndex.IndexOf(']');
                        var dotIndex = pathCompWithIndex.IndexOf('.');
                        if (bracketEnd == -1 || dotIndex == -1 || bracketEnd + 1 != dotIndex)
                        {
                            throw new Exception($"Invalid drill path component '{pathCompWithIndex}'.");
                        }

                        var pathComp = pathCompWithIndex.Substring(dotIndex + 1);
                        var rowStr = pathCompWithIndex.Substring(0, bracketEnd);
                        var row = int.Parse(rowStr);

                        var selectedRowItemType = this.currentDataArray[row].GetType();

                        var propInfos = selectedRowItemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
                        var propInfo = propInfos.FirstOrDefault(pi => pi.Name == pathComp);
                        if (propInfo == null)
                        {
                            break;
                        }

                        object[] nextDataArray = null;
                        var value = propInfo.GetValue(this.currentDataArray[row]);
                        if (propInfo.PropertyType.IsArray)
                        {
                            nextDataArray = (object[])value;
                            //currentType = propInfo.PropertyType.GetElementType();
                        }
                        else if (propInfo.PropertyType.GetInterface(nameof(IEnumerable)) != null)
                        {
                            nextDataArray = ((IEnumerable)value).Cast<object>().ToArray();
                            //currentType = propInfo.PropertyType.GetGenericArguments().Single();
                        }
                        else
                        {
                            nextDataArray = new object[1] { value };
                            //currentType = value.GetType();
                        }

                        this.currentDataArray = nextDataArray;
                    }
                }
            }

            // Check if data array needs ot be filtered based on selected item type in combo box.
            Type currentType = this.GridItemTypesComboBox.SelectedItem as Type;
            if (currentType == null)
            {
                currentType = this.currentDataArray?.FirstOrDefault()?.GetType();
            }

            FilterAndBindToData(currentType);

            // Get all types in the current data array.
            // The data array may be a polymorphic collection with different types derived from a base type.
            List<Type> allTypes = new List<Type>();
            if (this.currentDataArray != null)
            {
                // Get derived types
                foreach (var e in this.currentDataArray)
                {
                    var type = e.GetType();
                    if (!allTypes.Contains(type))
                    {
                        allTypes.Add(type);
                    }
                }
            }

            CheckandUpdateGridItemTypes(allTypes);
        }

        void FilterAndBindToData(Type dataType)
        {
            this.REPropertiesDataGrid.Columns.Clear();
            this.REPropertiesDataGrid.ItemsSource = null;

            if (dataType == null)
            {
                return;
            }

            var propInfos = dataType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (var propInfo in propInfos)
            {
                DataGridTextColumn col = new DataGridTextColumn()
                {
                    Header = propInfo.Name,
                    Binding = new Binding()
                    {
                        Converter = new DataGridTextColumnPropertyInfoConverter(),
                        ConverterParameter = propInfo
                    }
                };

                this.REPropertiesDataGrid.Columns.Add(col);
            }

            if (this.currentDataArray != null)
            {
                // Filter by dataType. Only show those items which are of type DataType or can be drived from it.
                var filteredDataArray = this.currentDataArray.Where(d => dataType.IsAssignableFrom(d.GetType())).ToList();

                //var filteredDataArray = dataArray;
                this.REPropertiesDataGrid.ItemsSource = filteredDataArray.Select(d => new RowDataHolder() { Data = d }).ToArray();
            }
        }

        private void REPropertiesDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var grid = sender as DataGrid;
            var selectedCells = grid.SelectedCells;
            if (selectedCells != null && selectedCells.Count > 0)
            {
                var selectedColumn = selectedCells[0].Column;
                var selectedItem = selectedCells[0].Item;
                object selectedItemData = null;

                // Get the items source collection (data grid row collection)
                var items = grid.ItemsSource;
                bool found = false;
                foreach (var item in items)
                {
                    if (item == selectedItem)
                    {
                        selectedItemData = (item as RowDataHolder)?.Data;
                        found = true;
                        break;
                    }
                }

                if (!found || selectedItemData == null) return;

                // Get the row number from currentDataArray since the itemsSource may be filtered.
                found = false;
                int row = 0;
                foreach (var data in this.currentDataArray)
                {
                    if (data == selectedItemData)
                    {
                        found = true;
                        break;
                    }

                    ++row;
                }

                if (!found) return;

                var textColumn = selectedColumn as DataGridTextColumn;
                if (textColumn != null)
                {
                    var binding = textColumn.Binding as Binding;
                    if (binding != null)
                    {
                        var propInfo = binding.ConverterParameter as PropertyInfo;
                        if (propInfo != null && propInfo.PropertyType != typeof(string))
                        {
                            // If the value double-clicked is null then cannot drill down.
                            if (propInfo.GetValue(selectedItemData) == null) return;

                            if (string.IsNullOrWhiteSpace(DrillPath))
                            {
                                DrillPath = string.Empty;
                            }

                            DrillPath += $"[{row}].{propInfo.Name}";

                            GetDataAndBind();
                        }
                    }
                }
            }
        }

        void CheckandUpdateGridItemTypes(List<Type> allTypesList)
        {
            var allTypes = allTypesList.Distinct().OrderBy(t => t.Name).ToArray();

            // Check if the types in the combo-box are different.
            var currentTypes = this.GridItemTypesComboBox.ItemsSource?.Cast<Type>().ToArray();

            var different = (allTypes == null && currentTypes != null) || (allTypes != null && currentTypes == null);
            if (allTypes != null && currentTypes != null)
            {
                if (allTypes.Length != currentTypes.Length)
                {
                    different = true;
                }
                else
                {
                    for (int i = 0; i < allTypes.Length; ++i)
                    {
                        if (allTypes[i] != currentTypes[i])
                        {
                            different = true;
                            break;
                        }
                    }
                }
            }

            if (different)
            {
                this.GridItemTypesComboBox.ItemsSource = allTypes;
                this.GridItemTypesComboBox.SelectedItem = null;
            }
        }
    }
}
