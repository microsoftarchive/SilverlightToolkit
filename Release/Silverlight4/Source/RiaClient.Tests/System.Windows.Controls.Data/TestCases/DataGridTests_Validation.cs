// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows.Controls.Data.Test.DataClasses;
using System.Windows.Controls.Primitives;
using System.Windows.Controls.Test;
using System.Windows.Data;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace System.Windows.Controls.Data.Test
{
    /// <summary>
    /// This class runs the unit tests for DataGrid editing events
    /// </summary>
    [TestClass]
    public class DataGridTests_Validation : DataGridUnitTest<Customer>
    {
        #region Row Validation

        #region Data

        private int _currentCellChangedCount;
        private int _selectionChangedCount;
        private SelectionChangedEventArgs _selectionChangedEventArgs = null;

        #endregion

        #region Helper Methods

        private void dataGrid_CellEditEnded(object sender, DataGridCellEditEndedEventArgs e)
        {
            ((DataGrid)sender).CommitEdit();
        }

        void dataGrid_CurrentCellChanged(object sender, EventArgs e)
        {
            this._currentCellChangedCount++;
        }

        private void dataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            this._selectionChangedCount++;
            this._selectionChangedEventArgs = e;
        }

        private ValidationSummaryItem FindErrorWithContext(ValidationSummary validationSummary, object context)
        {
            foreach (ValidationSummaryItem validationSummaryItem in validationSummary.Errors)
            {
                if (validationSummaryItem.Context == context)
                {
                    return validationSummaryItem;
                }
            }
            return null;
        }

        private CustomerList CreateCustomerListForRowValidation()
        {
            CustomerList customerList = new CustomerList(5);
            for (int customerIndex = 0; customerIndex < customerList.Count; customerIndex++)
            {
                customerList[customerIndex].Validate = true;
                if (customerIndex % 2 == 0)
                {
                    customerList[customerIndex].IsRegistered = true;
                }
                else
                {
                    customerList[customerIndex].IsRegistered = false;
                    customerList[customerIndex].IsValid = false;
                }
            }
            return customerList;
        }

        private DataGrid CreateDataGridForRowValidation()
        {
            DataGrid dataGrid = new DataGrid();
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            dataGrid.Width = 650;
            dataGrid.Height = 250;
            dataGrid.AutoGenerateColumns = false;

            string cellXaml = @"
                <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <TextBlock Text=""{Binding FirstName}""/>
                </DataTemplate>";
            DataTemplate cellTemplate = XamlReader.Load(cellXaml) as DataTemplate;
            Assert.IsNotNull(cellTemplate);

            string cellEditingXaml = @"
                <DataTemplate xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"">
                    <TextBox Text=""{Binding FirstName, Mode=TwoWay, ValidatesOnExceptions=True, NotifyOnValidationError=True}""/>
                </DataTemplate>";
            DataTemplate cellEditingTemplate = XamlReader.Load(cellEditingXaml) as DataTemplate;
            Assert.IsNotNull(cellEditingTemplate);

            DataGridTemplateColumn templateColumn = new DataGridTemplateColumn();
            templateColumn.Header = "FirstName";
            templateColumn.CellTemplate = cellTemplate;
            templateColumn.CellEditingTemplate = cellEditingTemplate;
            templateColumn.DisplayIndex = 0;

            dataGrid.Columns.Add(templateColumn);
            dataGrid.Columns.Add(new DataGridTextColumn() { Header = "LastName", Binding = new Binding("LastName") { Mode = BindingMode.TwoWay } });
            dataGrid.Columns.Add(new DataGridTextColumn() { Header = "Rating", Binding = new Binding("Rating") { Mode = BindingMode.TwoWay } });
            dataGrid.Columns.Add(new DataGridCheckBoxColumn() { Header = "IsValid", Binding = new Binding("IsValid") { Mode = BindingMode.TwoWay } });
            dataGrid.Columns.Add(new DataGridCheckBoxColumn() { Header = "IsRegistered", Binding = new Binding("IsRegistered") { Mode = BindingMode.TwoWay } });
            dataGrid.Columns.Add(new DataGridTextColumn() { Header = "FullAddress", Binding = new Binding("FullAddress") { Mode = BindingMode.TwoWay } });

            return dataGrid;
        }

        /// <summary>
        /// Simulates clicking on an ValidationSummaryItem within the ValidationSummary, by calling the
        /// ValidationSummary_FocusingInvalidControl event with the specified ValidationSummaryItemSource.
        /// </summary>
        /// <param name="sourceIndex">index into ValidationSummaryItem.Sources</param>
        internal void ValidationSummaryClick(DataGrid dataGrid, int sourceIndex)
        {
            ListBox errorsListBox = GetErrorsListBox(dataGrid.TestHook.ValidationSummary);
            if (errorsListBox != null && errorsListBox.SelectedItem != null)
            {
                ValidationSummaryItem item = errorsListBox.SelectedItem as ValidationSummaryItem;
                if (item == null)
                {
                    return;
                }
                ValidationSummaryItemSource source = null;
                if (sourceIndex < item.Sources.Count)
                {
                    source = item.Sources[sourceIndex];
                }
                dataGrid.TestHook.ValidationSummary_FocusingInvalidControl(dataGrid.TestHook.ValidationSummary, new FocusingInvalidControlEventArgs(item, source));
            }
        }

        /// <summary>
        /// Walks the visual tree to find the ValidationSummary's internal ListBox.
        /// </summary>
        /// <returns>ErrorsListBox if found, null otherwise</returns>
        internal ListBox GetErrorsListBox(ValidationSummary validationSummary)
        {
            Queue<DependencyObject> children = new Queue<DependencyObject>();
            children.Enqueue(validationSummary);
            while (children.Count > 0)
            {
                DependencyObject child = children.Dequeue();
                ListBox listBox = child as ListBox;
                if (listBox != null)
                {
                    return listBox;
                }
                int childrenCount = VisualTreeHelper.GetChildrenCount(child);
                for (int childIndex = 0; childIndex < childrenCount; childIndex++)
                {
                    children.Enqueue(VisualTreeHelper.GetChild(child, childIndex));
                }
            }
            return null;
        }

        #endregion Helper Methods

        [TestMethod]
        [Asynchronous]
        [Description("Ensures that ending cell edit on a cell related to a row error will run row validation.")]
        public virtual void AddRelatedError()
        {
            CustomerList customerList = CreateCustomerListForRowValidation();
            DataGrid dataGrid = CreateDataGridForRowValidation();
            dataGrid.ItemsSource = customerList;
            dataGrid.SelectedItem = customerList[0];

            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                bool success = dataGrid.BeginEdit();
                Assert.IsTrue(success, "BeginEdit was not successful");

                // Assert that the ValidationSummary was initialized correctly
                Assert.IsNotNull(dataGrid.TestHook.ValidationSummary);
                Assert.AreEqual(0, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsTrue(dataGrid.IsValid);

                // Input some values that will cause the IsRatingValid validation error,
                // which does not specify any related MemberNames
                customerList[0].Rating = -1;
                customerList[0].IsRegistered = true;
                customerList[0].IsValid = true;

                // Try to commit the invalid values
                dataGrid.CommitEdit();
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Assert that the ValidationSummary updated correctly
                Assert.AreEqual(1, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsRatingValid_ValidationResult));
                Assert.IsFalse(dataGrid.IsValid);

                // Begin editing the IsRegistered column
                dataGrid.CurrentColumn = dataGrid.Columns[4];
                dataGrid.BeginEdit();
                Assert.AreEqual(4, dataGrid.EditingColumnIndex);
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Change the bool in the IsRegistered checkbox in order to cause a new entity error
                CheckBox editingCheckBox = dataGrid.Columns[dataGrid.EditingColumnIndex].GetCellContent(dataGrid.EditingRow) as CheckBox;
                Assert.IsNotNull(editingCheckBox);
                Assert.AreEqual(true, editingCheckBox.IsChecked);
                editingCheckBox.IsChecked = false;
                bool success = dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                Assert.IsTrue(success);

                // The ValidationSummary should show the new error because IsRatingValid does not specify
                // any related MemberNames.  If nothing is specified, we assume that the cell could
                // be related, so we should be triggering row validation
                Assert.AreEqual(2, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsRatingValid_ValidationResult));
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsRegisteredValid_ValidationResult));
                Assert.IsFalse(dataGrid.IsValid);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Ensures that adding a new error to a valid cell will not run row validation when ending cell edit.")]
        public virtual void AddUnrelatedError()
        {
            CustomerList customerList = CreateCustomerListForRowValidation();
            DataGrid dataGrid = CreateDataGridForRowValidation();
            dataGrid.ItemsSource = customerList;
            dataGrid.SelectedItem = customerList[0];

            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                bool success = dataGrid.BeginEdit();
                Assert.IsTrue(success, "BeginEdit was not successful");

                // Assert that the ValidationSummary was initialized correctly
                Assert.IsNotNull(dataGrid.TestHook.ValidationSummary);
                Assert.AreEqual(0, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsTrue(dataGrid.IsValid);

                // Input some values that will cause the IsFullNameValidLength validation error
                customerList[0].FirstName = "ExtraordinarilyLongFirstName";
                customerList[0].IsRegistered = true;
                customerList[0].IsValid = true;

                // Try to commit the invalid values
                dataGrid.CommitEdit();
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Assert that the ValidationSummary updated correctly
                Assert.AreEqual(1, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsFullNameValidLength_ValidationResult));
                Assert.IsFalse(dataGrid.IsValid);

                // Begin editing the IsRegistered column
                dataGrid.CurrentColumn = dataGrid.Columns[4];
                dataGrid.BeginEdit();
                Assert.AreEqual(4, dataGrid.EditingColumnIndex);
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Change the bool in the IsRegistered checkbox in order to cause a new entity error
                CheckBox editingCheckBox = dataGrid.Columns[dataGrid.EditingColumnIndex].GetCellContent(dataGrid.EditingRow) as CheckBox;
                Assert.IsNotNull(editingCheckBox);
                Assert.AreEqual(true, editingCheckBox.IsChecked);
                editingCheckBox.IsChecked = false;
                bool success = dataGrid.CommitEdit(DataGridEditingUnit.Cell, true);
                Assert.IsTrue(success);

                // The ValidationSummary shouldn't show the new error yet because IsRegisteredValid was not related
                // to any pre-existing validation error, and we should only validate the row when we
                // finish editing a cell related to an error in the list
                Assert.AreEqual(1, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsFullNameValidLength_ValidationResult));
                Assert.IsFalse(dataGrid.IsValid);

                // Committing the row, however, will trigger validation which should find the new error
                success = dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                Assert.IsFalse(success);
                Assert.AreEqual(2, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsRegisteredValid_ValidationResult));
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsFullNameValidLength_ValidationResult));
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Ensures that cancelling an edit when there are entity errors returns the DataGrid to a normal state.")]
        public virtual void CancelError()
        {
            CustomerList customerList = CreateCustomerListForRowValidation();
            DataGrid dataGrid = CreateDataGridForRowValidation();
            dataGrid.ItemsSource = customerList;
            dataGrid.SelectedItem = customerList[0];

            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                bool success = dataGrid.BeginEdit();
                Assert.IsTrue(success, "BeginEdit was not successful");

                // Assert that the ValidationSummary was initialized correctly
                Assert.IsNotNull(dataGrid.TestHook.ValidationSummary);
                Assert.AreEqual(0, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsTrue(dataGrid.IsValid);

                // Input some values that will cause the IsFullNameValidLength validation error
                customerList[0].FirstName = "ExtraordinarilyLongFirstName";
                customerList[0].IsRegistered = true;
                customerList[0].IsValid = true;

                // Try to commit the invalid values
                dataGrid.CommitEdit();
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Assert that the ValidationSummary updated correctly
                Assert.AreEqual(1, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsFullNameValidLength_ValidationResult));
                Assert.IsFalse(dataGrid.IsValid);

                // Canceling the edit should remove the errors
                dataGrid.CancelEdit();
                Assert.AreEqual(-1, dataGrid.EditingColumnIndex);
                Assert.AreEqual(0, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsTrue(dataGrid.IsValid);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Ensures that we successfully cancel edit when we change the Page of a PagedCollectionView.")]
        public virtual void ChangePageWithError()
        {
            CustomerList customerList = CreateCustomerListForRowValidation();
            PagedCollectionView pagedCollectionView = new PagedCollectionView(customerList);
            pagedCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IsRegistered"));
            pagedCollectionView.PageSize = 2;
            DataGrid dataGrid = CreateDataGridForRowValidation();
            dataGrid.ItemsSource = pagedCollectionView;
            dataGrid.SelectedIndex = 0;

            string originalFirstName = customerList[0].FirstName;

            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                bool success = dataGrid.BeginEdit();
                Assert.IsTrue(success, "BeginEdit was not successful");

                // Assert that the ValidationSummary was initialized correctly
                Assert.IsNotNull(dataGrid.TestHook.ValidationSummary);
                Assert.AreEqual(0, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsTrue(dataGrid.IsValid);

                // Input some values that will cause the IsFullNameValidLength validation error
                ((Customer)pagedCollectionView[0]).FirstName = "ExtraordinarilyLongFirstName";

                // Try to commit the invalid values
                dataGrid.CommitEdit();
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Assert that there is still an editing row and it is invalid
                Assert.IsNotNull(dataGrid.EditingRow);
                Assert.AreEqual(pagedCollectionView[0], dataGrid.EditingRow.DataContext);
                Assert.IsFalse(dataGrid.EditingRow.IsValid);
                Assert.IsFalse(dataGrid.IsValid);

                // Cause a collection reset by changing the page
                pagedCollectionView.MoveToNextPage();
                pagedCollectionView.MoveToPreviousPage();
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Assert that there is still an editing row and it is invalid 
                // since pcv does not page while editing.
                Assert.IsNotNull(dataGrid.EditingRow);
                Assert.AreEqual(pagedCollectionView[0], dataGrid.EditingRow.DataContext);
                Assert.IsFalse(dataGrid.EditingRow.IsValid);
                Assert.IsFalse(dataGrid.IsValid);

                pagedCollectionView.CancelEdit();
                pagedCollectionView.MoveToNextPage();
                pagedCollectionView.MoveToPreviousPage();
            });

            EnqueueCallback(delegate
            {
                // The DataGrid should no longer be in editing mode and the invalid value should be reverted
                Assert.IsNull(dataGrid.EditingRow);
                Assert.AreEqual(-1, dataGrid.EditingColumnIndex);
                Assert.AreEqual(originalFirstName, ((Customer)pagedCollectionView[0]).FirstName);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Ensures that you are unable to collapse a group that has a row validation error.")]
        public virtual void CollapseGroupWithError()
        {
            // 
            CustomerList customerList = CreateCustomerListForRowValidation();
            PagedCollectionView pagedCollectionView = new PagedCollectionView(customerList);
            pagedCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IsRegistered"));
            DataGrid dataGrid = CreateDataGridForRowValidation();
            dataGrid.ItemsSource = pagedCollectionView;
            dataGrid.SelectedItem = customerList[0];

            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                bool success = dataGrid.BeginEdit();
                Assert.IsTrue(success, "BeginEdit was not successful");

                // Assert that the ValidationSummary was initialized correctly
                Assert.IsNotNull(dataGrid.TestHook.ValidationSummary);
                Assert.AreEqual(0, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsTrue(dataGrid.IsValid);

                // Input some values that will cause the IsFullNameValidLength validation error
                customerList[0].FirstName = "ExtraordinarilyLongFirstName";
                customerList[0].IsRegistered = true;
                customerList[0].IsValid = true;

                // Try to commit the invalid values
                dataGrid.CommitEdit();
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Assert that the ValidationSummary updated correctly
                Assert.AreEqual(1, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsFullNameValidLength_ValidationResult));
                Assert.IsFalse(dataGrid.IsValid);

                // Check the initial state of the item's owning group
                DataGridRowGroupHeader groupHeader = dataGrid.DisplayData.GetDisplayedElement(0) as DataGridRowGroupHeader;
                Assert.IsNotNull(groupHeader, "Item[0] has no corresponding RowGroupHeader");
                DataGridRowGroupInfo groupInfo = groupHeader.RowGroupInfo;
                Assert.IsNotNull(groupInfo, "Item[0] has no corresponding RowGroupInfo");
                CollectionViewGroup group = groupInfo.CollectionViewGroup;
                Assert.IsNotNull(group, "Item[0] has no corresponding CollectionViewGroup");
                Assert.AreEqual(Visibility.Visible, groupInfo.Visibility, "Group is not visible by default");

                // Find the group's ExpanderButton and verify the value of IsChecked
                Stack<DependencyObject> visuals = new Stack<DependencyObject>();
                ToggleButton expanderButton = null;
                visuals.Push(groupHeader);
                while (visuals.Count > 0)
                {
                    DependencyObject visual = visuals.Pop();
                    expanderButton = visual as ToggleButton;
                    if (expanderButton != null)
                    {
                        break;
                    }

                    int childCount = VisualTreeHelper.GetChildrenCount(visual);
                    for (int childIndex = 0; childIndex < childCount; childIndex++)
                    {
                        visuals.Push(VisualTreeHelper.GetChild(visual, childIndex));
                    }
                }
                Assert.IsNotNull(expanderButton, "Group did not contain ExpanderButton");
                Assert.IsTrue(expanderButton.IsChecked.HasValue, "ExpanderButton.IsChecked is incorrect");
                Assert.IsTrue(expanderButton.IsChecked.Value, "ExpanderButton.IsChecked is incorrect");

                // Trying to collapse the group should fail if there is a validation error
                expanderButton.IsChecked = false;
                Assert.AreEqual(Visibility.Visible, groupInfo.Visibility, "Group was collapsed even though there was a validation error");
                Assert.IsTrue(expanderButton.IsChecked.HasValue, "ExpanderButton.IsChecked is incorrect");
                Assert.IsTrue(expanderButton.IsChecked.Value, "ExpanderButton.IsChecked is incorrect");
                Assert.AreEqual(1, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsFullNameValidLength_ValidationResult));
                Assert.IsFalse(dataGrid.IsValid);

                // Trying to collapse after fixing the error should work correctly
                customerList[0].FirstName = "ShortFirstName";
                expanderButton.IsChecked = false;
                Assert.AreEqual(Visibility.Collapsed, groupInfo.Visibility, "Group should be collapsed");
                Assert.IsTrue(expanderButton.IsChecked.HasValue, "ExpanderButton.IsChecked is incorrect");
                Assert.IsFalse(expanderButton.IsChecked.Value, "ExpanderButton.IsChecked is incorrect");
                Assert.AreEqual(-1, dataGrid.EditingColumnIndex);
                Assert.AreEqual(0, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsTrue(dataGrid.IsValid);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Ensures that committing edit on the only item in a group updates selection correctly.")]
        public virtual void CommitEditOnOnlyItemInGroup()
        {
            CustomerList customerList = new CustomerList(2);
            for (int customerIndex = 0; customerIndex < customerList.Count; customerIndex++)
            {
                customerList[customerIndex].Validate = true;
                if (customerIndex == 0)
                {
                    customerList[customerIndex].IsRegistered = true;
                }
                else
                {
                    customerList[customerIndex].IsRegistered = false;
                    customerList[customerIndex].IsValid = false;
                }
            }
            PagedCollectionView pagedCollectionView = new PagedCollectionView(customerList);
            pagedCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IsRegistered"));
            DataGrid dataGrid = CreateDataGridForRowValidation();
            dataGrid.ItemsSource = pagedCollectionView;
            dataGrid.SelectedItem = pagedCollectionView[0];
            dataGrid.SelectionChanged += new SelectionChangedEventHandler(dataGrid_SelectionChanged);
            dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                bool success = dataGrid.BeginEdit();
                Assert.IsTrue(success, "BeginEdit was not successful");
                this._currentCellChangedCount = 0;
                this._selectionChangedCount = 0;
                this._selectionChangedEventArgs = null;

                // Commit edit by pressing the up key
                dataGrid.ProcessUpKey();
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                Assert.AreEqual(0, dataGrid.CurrentSlot, "Incorrect CurrentSlot after committing edit");
                Assert.AreEqual(-1, dataGrid.SelectedIndex, "Incorrect SelectedIndex after committing edit");
                Assert.IsNull(dataGrid.SelectedItem, "Incorrect SelectedItem after committing edit");
                Assert.AreEqual(1, this._currentCellChangedCount, "CurrentCellChanged should have been raised once");
                Assert.AreEqual(1, this._selectionChangedCount, "SelectionChanged should have been raised once");
                Assert.IsNotNull(this._selectionChangedEventArgs, "SelectionChangedEventArgs is null");
                Assert.AreEqual(0, this._selectionChangedEventArgs.AddedItems.Count, "No items should have been selected by pressing up");
                Assert.AreEqual(1, this._selectionChangedEventArgs.RemovedItems.Count, "An item should have been de-selected by pressing up");
                Assert.AreEqual(pagedCollectionView[0], this._selectionChangedEventArgs.RemovedItems[0], "Wrong item de-selected");

                DataGridRowGroupHeader rowGroupHeader = dataGrid.DisplayData.GetDisplayedElement(0) as DataGridRowGroupHeader;
                Assert.IsNotNull(rowGroupHeader, "Missing RowGroupHeader after committing edit");
                Assert.IsNotNull(rowGroupHeader.RowGroupInfo, "Missing RowGroupInfo after committing edit");
                Assert.AreEqual(0, rowGroupHeader.RowGroupInfo.Slot, "RowGroupInfo has incorrect Index after committing edit");

                DataGridRow row = dataGrid.DisplayData.GetDisplayedRow(0);
                Assert.IsNotNull(row, "Missing row after committing edit");
                Assert.AreEqual(0, row.Index, "Row has incorrect Index after committing edit");
                Assert.AreEqual(1, row.Slot, "Row has incorrect Slot after committing edit");

                row = dataGrid.DisplayData.GetDisplayedRow(1);
                Assert.IsNotNull(row, "Missing row after committing edit");
                Assert.AreEqual(1, row.Index, "Row has incorrect Index after committing edit");
                Assert.AreEqual(3, row.Slot, "Row has incorrect Slot after committing edit");

                dataGrid.SelectionChanged -= new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                dataGrid.CurrentCellChanged -= new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Ensures that the Tab key is handled correctly when CommitEdit is called during CellEditEnded.")]
        public virtual void CommitEditOnTabPress()
        {
            CustomerList customerList = this.CreateCustomerListForRowValidation();
            PagedCollectionView pagedCollectionView = new PagedCollectionView(customerList);
            pagedCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IsRegistered"));
            DataGrid dataGrid = CreateDataGridForRowValidation();
            dataGrid.ItemsSource = pagedCollectionView;
            dataGrid.SelectedItem = pagedCollectionView[0];
            dataGrid.SelectionChanged += new SelectionChangedEventHandler(dataGrid_SelectionChanged);
            dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                // Begin edit on the IsRegistered column
                dataGrid.CurrentColumn = dataGrid.Columns[4];
                bool success = dataGrid.BeginEdit();
                Assert.IsTrue(success, "BeginEdit was not successful");
                this._currentCellChangedCount = 0;
                this._selectionChangedCount = 0;
                this._selectionChangedEventArgs = null;

                // Change a value that will cause the item to change groups
                Customer customer = dataGrid.SelectedItem as Customer;
                Assert.IsNotNull(customer);
                customer.Validate = false;
                customer.IsRegistered = !customer.IsRegistered;

                // Pressing the Tab key will cause CellEditEnded to be raised, where we will CommitEdit
                dataGrid.CellEditEnded += new EventHandler<DataGridCellEditEndedEventArgs>(dataGrid_CellEditEnded);
                dataGrid.TestHook.ProcessTabKey(null);
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                Assert.AreEqual(dataGrid.Columns[5], dataGrid.CurrentColumn, "Incorrect CurrentColumn after tabbing");
                Assert.AreEqual(4, dataGrid.CurrentSlot, "Incorrect CurrentSlot after committing edit");
                Assert.AreEqual(2, dataGrid.SelectedIndex, "Incorrect SelectedIndex after committing edit");
                Assert.AreEqual(pagedCollectionView[2], dataGrid.SelectedItem, "Incorrect SelectedItem after committing edit");
                Assert.AreEqual(1, this._currentCellChangedCount, "CurrentCellChanged should have been raised once");
                Assert.AreEqual(0, this._selectionChangedCount, "SelectionChanged should have been raised once");
                Assert.IsNull(this._selectionChangedEventArgs, "SelectionChangedEventArgs should be null");

                dataGrid.CellEditEnded -= new EventHandler<DataGridCellEditEndedEventArgs>(dataGrid_CellEditEnded);
                dataGrid.SelectionChanged -= new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                dataGrid.CurrentCellChanged -= new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Ensures that committing edit will run validation regardless of whether or not we are exiting edit mode.")]
        public virtual void CommitEditWithoutExitingEditingMode()
        {
            CustomerList customerList = CreateCustomerListForRowValidation();
            DataGrid dataGrid = CreateDataGridForRowValidation();
            dataGrid.ItemsSource = customerList;
            dataGrid.SelectedItem = customerList[0];
            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                bool success = dataGrid.BeginEdit();
                Assert.IsTrue(success, "BeginEdit was not successful");

                // Assert that the ValidationSummary was initialized correctly
                Assert.IsNotNull(dataGrid.TestHook.ValidationSummary);
                Assert.AreEqual(0, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsTrue(dataGrid.IsValid);

                // Input some values that will cause the IsFullNameValidLength validation error
                customerList[0].FirstName = "ExtraordinarilyLongFirstName";
                customerList[0].IsRegistered = true;
                customerList[0].IsValid = true;

                // Commit the cell edit without exiting edit mode
                dataGrid.CommitEdit(DataGridEditingUnit.Cell, false /*exitEditingMode*/);
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Assert that the ValidationSummary has not changed because all we've done is commit the cell
                Assert.IsNotNull(dataGrid.TestHook.ValidationSummary);
                Assert.AreEqual(0, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsTrue(dataGrid.IsValid);

                // Now commit the row, but remain in editing mode
                dataGrid.CommitEdit(DataGridEditingUnit.Row, false /*exitEditingMode*/);
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Assert that the ValidationSummary updated correctly because validation was run
                Assert.AreEqual(1, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsFullNameValidLength_ValidationResult));
                Assert.IsFalse(dataGrid.IsValid);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Ensures that filtering an item out of the DataGrid correctly selects the next item.")]
        public virtual void FilterItemOutOfDataGrid()
        {
            CustomerList customerList = CreateCustomerListForRowValidation();
            PagedCollectionView pagedCollectionView = new PagedCollectionView(customerList);
            pagedCollectionView.GroupDescriptions.Add(new PropertyGroupDescription("IsRegistered"));
            pagedCollectionView.Filter = customer => customer is Customer && ((Customer)customer).FirstName != "Filter";
            DataGrid dataGrid = CreateDataGridForRowValidation();
            dataGrid.ItemsSource = pagedCollectionView;
            dataGrid.SelectedIndex = 0;
            object originalItem = null;
            object expectedItem = null;

            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                bool success = dataGrid.BeginEdit();
                Assert.IsTrue(success, "BeginEdit was not successful");
                originalItem = ((CollectionViewGroup)pagedCollectionView.Groups[0]).Items[0];
                expectedItem = ((CollectionViewGroup)pagedCollectionView.Groups[0]).Items[1];
                dataGrid.SelectionChanged += new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                dataGrid.CurrentCellChanged += new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
                this._currentCellChangedCount = 0;
                this._selectionChangedCount = 0;

                // Input a value that will cause the item to be filtered out and commit edit
                ((Customer)pagedCollectionView[0]).FirstName = "Filter";
                dataGrid.CommitEdit();
            });
            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                Assert.AreEqual(1, dataGrid.CurrentSlot, "Incorrect CurrentSlot after committing edit");
                Assert.AreEqual(0, dataGrid.SelectedIndex, "Incorrect SelectedIndex after committing edit");
                Assert.AreEqual(expectedItem, dataGrid.SelectedItem, "Incorrect SelectedItem after committing edit");
                Assert.AreEqual(1, this._currentCellChangedCount, "CurrentCellChanged should have been raised once");
                Assert.AreEqual(1, this._selectionChangedCount, "SelectionChanged should have been raised once");
                Assert.IsNotNull(this._selectionChangedEventArgs, "SelectionChangedEventArgs is null");
                Assert.AreEqual(1, this._selectionChangedEventArgs.AddedItems.Count, "No items should have been selected by pressing up");
                Assert.AreEqual(1, this._selectionChangedEventArgs.RemovedItems.Count, "An item should have been de-selected by pressing up");
                Assert.AreEqual(originalItem, this._selectionChangedEventArgs.RemovedItems[0], "Wrong item de-selected");
                Assert.AreEqual(expectedItem, this._selectionChangedEventArgs.AddedItems[0], "Wrong item selected");

                dataGrid.SelectionChanged -= new SelectionChangedEventHandler(dataGrid_SelectionChanged);
                dataGrid.CurrentCellChanged -= new EventHandler<EventArgs>(dataGrid_CurrentCellChanged);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Ensures that fixing an entity error in the DataGrid behaves correctly.")]
        public virtual void FixError()
        {
            CustomerList customerList = CreateCustomerListForRowValidation();
            DataGrid dataGrid = CreateDataGridForRowValidation();
            dataGrid.ItemsSource = customerList;
            dataGrid.SelectedItem = customerList[0];
            TextBox editingTextBox = null;

            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                bool success = dataGrid.BeginEdit();
                Assert.IsTrue(success, "BeginEdit was not successful");

                // Assert that the ValidationSummary was initialized correctly
                Assert.IsNotNull(dataGrid.TestHook.ValidationSummary);
                Assert.AreEqual(0, dataGrid.TestHook.ValidationSummary.Errors.Count);

                // Input some values that will cause the IsFullNameValidLength validation error
                customerList[0].FirstName = "ExtraordinarilyLongFirstName";
                customerList[0].IsRegistered = true;
                customerList[0].IsValid = true;

                // Try to commit the invalid values
                dataGrid.CommitEdit();
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Assert that the ValidationSummary updated correctly
                Assert.AreEqual(1, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsFullNameValidLength_ValidationResult));

                // Selecting the IsFullNameValid error should set IsValid to false for all related cells
                ListBox errorsListBox = this.GetErrorsListBox(dataGrid.TestHook.ValidationSummary);
                Assert.IsNotNull(errorsListBox, "ValidationSummary did not have a ListBox");
                errorsListBox.SelectedItem = FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsFullNameValidLength_ValidationResult);
                Assert.IsFalse(dataGrid.EditingRow.Cells[0].IsValid);
                Assert.IsFalse(dataGrid.EditingRow.Cells[1].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[2].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[3].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[4].IsValid);

                // Clicking on the IsFullNameValid error should begin editing on the first related cell
                this.ValidationSummaryClick(dataGrid, 0);
                Assert.AreEqual(0, dataGrid.EditingColumnIndex);
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Change the text in the FirstName cell to another invalid value
                editingTextBox = dataGrid.Columns[dataGrid.EditingColumnIndex].GetCellContent(dataGrid.EditingRow) as TextBox;
                Assert.IsNotNull(editingTextBox);
                Assert.AreEqual("ExtraordinarilyLongFirstName", editingTextBox.Text);
                editingTextBox.Text = "AnEvenMoreExtraordinarilyLongFirstName";

                // Cycle back through the related cells
                this.ValidationSummaryClick(dataGrid, 1);
                Assert.AreEqual(1, dataGrid.EditingColumnIndex);
                this.ValidationSummaryClick(dataGrid, 0);
                Assert.AreEqual(0, dataGrid.EditingColumnIndex);

                // The error should not have been removed
                Assert.AreEqual(1, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsFullNameValidLength_ValidationResult));
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Now change the text in the FirstName cell to a valid value
                editingTextBox = dataGrid.Columns[dataGrid.EditingColumnIndex].GetCellContent(dataGrid.EditingRow) as TextBox;
                Assert.IsNotNull(editingTextBox);
                Assert.AreEqual("AnEvenMoreExtraordinarilyLongFirstName", editingTextBox.Text);
                editingTextBox.Text = "ShortName";

                // We should have ended edit for the cell and removed the error
                this.ValidationSummaryClick(dataGrid, 0);
                Assert.AreEqual(-1, dataGrid.EditingColumnIndex);
                Assert.AreEqual(0, dataGrid.TestHook.ValidationSummary.Errors.Count);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Ensures that the ValidationSummary updates correctly when toggling between errors.")]
        public virtual void SwitchError()
        {
            CustomerList customerList = CreateCustomerListForRowValidation();
            DataGrid dataGrid = CreateDataGridForRowValidation();
            isLoaded = false;
            dataGrid.Loaded += new RoutedEventHandler(dataGrid_Loaded);
            dataGrid.ItemsSource = customerList;
            dataGrid.SelectedItem = customerList[0];

            TestPanel.Children.Add(dataGrid);
            EnqueueConditional(delegate { return isLoaded; });

            this.EnqueueYieldThread();
            EnqueueCallback(delegate
            {
                bool success = dataGrid.BeginEdit();
                Assert.IsTrue(success, "BeginEdit was not successful");

                // Assert that the DataGrid, its editing row and all the cells are valid
                Assert.IsTrue(dataGrid.IsValid);
                Assert.IsNotNull(dataGrid.EditingRow);
                Assert.IsTrue(dataGrid.EditingRow.IsValid);
                foreach (DataGridCell cell in dataGrid.EditingRow.Cells)
                {
                    Assert.IsTrue(cell.IsValid);
                }

                // Assert that the ValidationSummary was initialized correctly
                Assert.IsNotNull(dataGrid.TestHook.ValidationSummary);
                Assert.IsNotNull(dataGrid.TestHook.ValidationSummary.Errors);
                Assert.AreEqual(0, dataGrid.TestHook.ValidationSummary.Errors.Count);

                // Input some values that will cause entity-level validation errors
                customerList[0].FirstName = "ExtraordinarilyLongFirstName";
                customerList[0].IsRegistered = false;
                customerList[0].IsValid = true;
                customerList[0].Rating = -1;

                // Try to commit the invalid values
                dataGrid.CommitEdit();
            });
            this.EnqueueYieldThread();

            EnqueueCallback(delegate
            {
                // Assert that the DataGrid and its editing row are invalid but the cells are still all valid
                Assert.IsFalse(dataGrid.IsValid);
                Assert.IsNotNull(dataGrid.EditingRow);
                Assert.IsFalse(dataGrid.EditingRow.IsValid);
                foreach (DataGridCell cell in dataGrid.EditingRow.Cells)
                {
                    Assert.IsTrue(cell.IsValid);
                }

                // Assert that the ValidationSummary updated correctly
                Assert.AreEqual(3, dataGrid.TestHook.ValidationSummary.Errors.Count);
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsRatingValid_ValidationResult));
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsRegisteredValid_ValidationResult));
                Assert.IsNotNull(FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsFullNameValidLength_ValidationResult));

                // Trying to sort while there's invalid data should fail
                dataGrid.Columns[0].HeaderCell.InvokeProcessSort();
                Assert.IsNull(dataGrid.Columns[0].HeaderCell.CurrentSortingState);

                // Trying to change selection while there's an entity-level validation error should fail
                dataGrid.SelectedIndex = -1;
                dataGrid.SelectedItem = null;
                Assert.AreEqual(0, dataGrid.SelectedIndex);
                Assert.AreEqual(customerList[0], dataGrid.SelectedItem);

                // Trying to change ICollectionView currency should also fail
                if (dataGrid.DataConnection.CollectionView != null)
                {
                    dataGrid.DataConnection.CollectionView.MoveCurrentToNext();
                    Assert.AreEqual(customerList[0], dataGrid.SelectedItem);
                }

                // Selecting the IsFullNameValid error should set IsValid to false for all related cells
                ListBox errorsListBox = this.GetErrorsListBox(dataGrid.TestHook.ValidationSummary);
                Assert.IsNotNull(errorsListBox, "ValidationSummary did not have a ListBox");
                errorsListBox.SelectedItem = FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsFullNameValidLength_ValidationResult);
                Assert.IsFalse(dataGrid.EditingRow.Cells[0].IsValid);
                Assert.IsFalse(dataGrid.EditingRow.Cells[1].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[2].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[3].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[4].IsValid);

                // Clicking on the IsFullNameValid error should cycle editing between the related cells
                this.ValidationSummaryClick(dataGrid, 0);
                Assert.AreEqual(0, dataGrid.EditingColumnIndex);
                this.ValidationSummaryClick(dataGrid, 1);
                Assert.AreEqual(1, dataGrid.EditingColumnIndex);
                this.ValidationSummaryClick(dataGrid, 0);
                Assert.AreEqual(0, dataGrid.EditingColumnIndex);

                // Selecting the IsRegisteredValid error should set IsValid to false on related cells, and true on the others
                Assert.IsNotNull(errorsListBox, "ValidationSummary did not have a ListBox");
                errorsListBox.SelectedItem = FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsRegisteredValid_ValidationResult);
                Assert.IsTrue(dataGrid.EditingRow.Cells[0].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[1].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[2].IsValid);
                Assert.IsFalse(dataGrid.EditingRow.Cells[3].IsValid);
                Assert.IsFalse(dataGrid.EditingRow.Cells[4].IsValid);

                // Clicking on the IsRegisteredValid error should cycle editing between the related cells
                this.ValidationSummaryClick(dataGrid, 0);
                Assert.AreEqual(3, dataGrid.EditingColumnIndex);
                this.ValidationSummaryClick(dataGrid, 1);
                Assert.AreEqual(4, dataGrid.EditingColumnIndex);
                this.ValidationSummaryClick(dataGrid, 0);
                Assert.AreEqual(3, dataGrid.EditingColumnIndex);

                // Selecting the IsRatingValid error should set IsValid to true on all cells because the
                // ValidationResult did not specify any related MemberNames
                Assert.IsNotNull(errorsListBox, "ValidationSummary did not have a ListBox");
                errorsListBox.SelectedItem = FindErrorWithContext(dataGrid.TestHook.ValidationSummary, CustomerValidator.IsRatingValid_ValidationResult);
                Assert.IsTrue(dataGrid.EditingRow.Cells[0].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[1].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[2].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[3].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[4].IsValid);

                // Clicking on the IsRatingValid error should not begin editing because there are no related cells
                this.ValidationSummaryClick(dataGrid, 0);
                Assert.AreEqual(-1, dataGrid.EditingColumnIndex);
            });
            EnqueueTestComplete();
        }

        #endregion Row Validation

        #region Validation Util

        private class DerivedButton : Button { }

        [TestMethod]
        [Description("Uses ValidationUtil helper classes to search for bindings according to specific criteria.")]
        public virtual void SearchForBindings()
        {
            List<BindingInfo> bindings = null;
            Customer targetCustomer = new Customer();
            Customer nonTargetCustomer = new Customer();

            string xaml = @"
                <Grid xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
                      xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
                    <Grid.ColumnDefinitions>
                        <ColumnDefinition/>
                        <ColumnDefinition/>
                    </Grid.ColumnDefinitions>
                    <Grid.RowDefinitions>
                        <RowDefinition/>
                        <RowDefinition/>
                    </Grid.RowDefinitions>
                    <StackPanel x:Name=""stackPanel"" Background=""{Binding PreferredColor}"">
                        <TextBox Text=""{Binding LastName, Mode=TwoWay}""/>
                        <TextBlock Text=""{Binding LastName}""/>
                        <Rectangle Height=""10"" Width=""{Binding Rating}"" Fill=""{Binding PreferredColor}""/>
                    </StackPanel>
                    <Button Content=""{Binding FirstName}"" Grid.Column=""1""/>
                    <TextBox Text=""{Binding FullAddress, Mode=TwoWay}"" Grid.Row=""1""/>
                    <Border BorderThickness=""{Binding Age}"" Grid.Row=""1"" Grid.Column=""1"">
                        <CheckBox IsChecked=""{Binding IsValid, Mode=TwoWay}"" Foreground=""{Binding PreferredColor}""/>
                    </Border>
                </Grid>";

            Grid grid = XamlReader.Load(xaml) as Grid;
            Assert.IsNotNull(grid);

            StackPanel stackPanel = grid.FindName("stackPanel") as StackPanel;
            Assert.IsNotNull(stackPanel);

            DerivedButton derivedButton = new DerivedButton();
            derivedButton.SetBinding(DerivedButton.ContentProperty, new Binding("FirstName"));
            stackPanel.Children.Add(derivedButton);

            // Search for all bindings in the grid
            bindings = grid.GetBindingInfo(null /*dataItem*/, false /*twoWayOnly*/, false /*useBlockList*/, true /*searchChildren*/);
            Assert.AreEqual(11, bindings.Count);

            // Search for bindings on the stack panel, but not its children
            bindings = stackPanel.GetBindingInfo(null /*dataItem*/, false /*twoWayOnly*/, false /*useBlockList*/, false /*searchChildren*/);
            Assert.AreEqual(1, bindings.Count);

            // Search for all bindings in the stack panel, including its children
            bindings = stackPanel.GetBindingInfo(null /*dataItem*/, false /*twoWayOnly*/, false /*useBlockList*/, true /*searchChildren*/);
            Assert.AreEqual(6, bindings.Count);

            // Search only for TwoWay bindings
            bindings = grid.GetBindingInfo(null /*dataItem*/, true /*twoWayOnly*/, false /*useBlockList*/, true /*searchChildren*/);
            Assert.AreEqual(3, bindings.Count);

            // Search for all bindings, but use the block list (which ignores elements not typically used for input)
            bindings = grid.GetBindingInfo(null /*dataItem*/, false /*twoWayOnly*/, true /*useBlockList*/, true /*searchChildren*/);
            Assert.AreEqual(4, bindings.Count);

            // Exclude StackPanels (and their children) from the search
            bindings = grid.GetBindingInfo(null /*dataItem*/, false /*twoWayOnly*/, false /*useBlockList*/, true /*searchChildren*/, typeof(StackPanel) /*excludedTypes*/);
            Assert.AreEqual(5, bindings.Count);

            // Exclude TextBoxes and TextBlocks from the search
            bindings = grid.GetBindingInfo(null /*dataItem*/, false /*twoWayOnly*/, false /*useBlockList*/, true /*searchChildren*/, typeof(TextBlock), typeof(TextBox) /*excludedTypes*/);
            Assert.AreEqual(8, bindings.Count);

            // Exclude Buttons from the search (including classes that derive from button)
            bindings = grid.GetBindingInfo(null /*dataItem*/, false /*twoWayOnly*/, false /*useBlockList*/, true /*searchChildren*/, typeof(Button) /*excludedTypes*/);
            Assert.AreEqual(9, bindings.Count);

            // Setting the root's DomainContext and searching for it explicitly should still find all bindings
            grid.DataContext = targetCustomer;
            bindings = grid.GetBindingInfo(targetCustomer /*dataItem*/, false /*twoWayOnly*/, false /*useBlockList*/, true /*searchChildren*/);
            Assert.AreEqual(11, bindings.Count);

            // Setting a child's DomainContext to something other than the target should exclude any bindings contained
            // within that child (including its own children) from the list of bindings
            stackPanel.DataContext = nonTargetCustomer;
            bindings = grid.GetBindingInfo(targetCustomer /*dataItem*/, false /*twoWayOnly*/, false /*useBlockList*/, true /*searchChildren*/);
            Assert.AreEqual(5, bindings.Count);

            // Setting a specific grandchild's DomainContext back to the target should re-include it in the list
            derivedButton.DataContext = targetCustomer;
            bindings = grid.GetBindingInfo(targetCustomer /*dataItem*/, false /*twoWayOnly*/, false /*useBlockList*/, true /*searchChildren*/);
            Assert.AreEqual(6, bindings.Count);
        }

        [TestMethod]
        [Description("Ensures that the block list works when searching for dependency properties.")]
        public virtual void SearchForDependencyProperties()
        {
            List<DependencyProperty> dependencyProperties = null;
            DerivedButton derivedButton = new DerivedButton();
            TextBlock textBlock = new TextBlock();
            TextBox textBox = new TextBox();

            // An element not in the block list should return dependency properties if useBlockList is false
            dependencyProperties = textBox.GetDependencyProperties(false);
            Assert.IsNotNull(dependencyProperties);
            Assert.IsTrue(dependencyProperties.Count > 0);

            // An element not in the block list should still return dependency properties even if useBlockList is true
            dependencyProperties = textBox.GetDependencyProperties(true);
            Assert.IsNotNull(dependencyProperties);
            Assert.IsTrue(dependencyProperties.Count > 0);

            // An element in the block list should return dependency properties if useBlockList is false
            dependencyProperties = textBlock.GetDependencyProperties(false);
            Assert.IsNotNull(dependencyProperties);
            Assert.IsTrue(dependencyProperties.Count > 0);

            // An element in the block list should not return dependency properties if useBlockList is true
            dependencyProperties = textBlock.GetDependencyProperties(true);
            Assert.IsNotNull(dependencyProperties);
            Assert.IsTrue(dependencyProperties.Count == 0);

            // An element that is derived from an element in the block list should return dependency properties if useBlockList is false
            dependencyProperties = derivedButton.GetDependencyProperties(false);
            Assert.IsNotNull(dependencyProperties);
            Assert.IsTrue(dependencyProperties.Count > 0);

            // An element that is derived from an element in the block list should not return dependency properties if useBlockList is true
            dependencyProperties = derivedButton.GetDependencyProperties(true);
            Assert.IsNotNull(dependencyProperties);
            Assert.IsTrue(dependencyProperties.Count == 0);
        }

        #endregion Validation Util
    }
}
