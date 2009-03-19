// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections.Generic;
using System.Windows.Controls.Data.Test.DataClasses;
using System.Windows.Controls.Test;
using System.Windows.Data;
using System.Windows.Markup;
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

        #region Helper Methods

        private CustomerList CreateCustomerListForRowValidation()
        {
            CustomerList customerList = new CustomerList(5);
            foreach (Customer customer in customerList)
            {
                customer.Validate = true;
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

            dataGrid.Columns.Add(templateColumn);
            dataGrid.Columns.Add(new DataGridTextColumn() { Header = "LastName", Binding = new Binding("LastName") { Mode = BindingMode.TwoWay } });
            dataGrid.Columns.Add(new DataGridTextColumn() { Header = "Rating", Binding = new Binding("Rating") { Mode = BindingMode.TwoWay } });
            dataGrid.Columns.Add(new DataGridCheckBoxColumn() { Header = "IsValid", Binding = new Binding("IsValid") { Mode = BindingMode.TwoWay } });
            dataGrid.Columns.Add(new DataGridCheckBoxColumn() { Header = "IsRegistered", Binding = new Binding("IsRegistered") { Mode = BindingMode.TwoWay } });

            return dataGrid;
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

                // Assert that the ErrorsListBox was initialized correctly
                Assert.IsNotNull(dataGrid.TestHook.ErrorsListBox);
                Assert.AreEqual(Visibility.Collapsed, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(0, dataGrid.TestHook.ErrorsListBox.Items.Count);
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
                // Assert that the ErrorsListBox updated correctly
                Assert.AreEqual(Visibility.Visible, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(1, dataGrid.TestHook.ErrorsListBox.Items.Count);
                Assert.IsTrue(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsRatingValid_ValidationResult));
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

                // The ErrorsListBox should show the new error because IsRatingValid does not specify
                // any related MemberNames.  If nothing is specified, we assume that the cell could
                // be related, so we should be triggering row validation
                Assert.AreEqual(Visibility.Visible, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(2, dataGrid.TestHook.ErrorsListBox.Items.Count);
                Assert.IsTrue(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsRatingValid_ValidationResult));
                Assert.IsTrue(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsRegisteredValid_ValidationResult));
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

                // Assert that the ErrorsListBox was initialized correctly
                Assert.IsNotNull(dataGrid.TestHook.ErrorsListBox);
                Assert.AreEqual(Visibility.Collapsed, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(0, dataGrid.TestHook.ErrorsListBox.Items.Count);
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
                // Assert that the ErrorsListBox updated correctly
                Assert.AreEqual(Visibility.Visible, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(1, dataGrid.TestHook.ErrorsListBox.Items.Count);
                Assert.IsTrue(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsFullNameValidLength_ValidationResult));
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

                // The ErrorsListBox shouldn't show the new error yet because IsRegisteredValid was not related
                // to any pre-existing validation error, and we should only validate the row when we
                // finish editing a cell related to an error in the list
                Assert.AreEqual(Visibility.Visible, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(1, dataGrid.TestHook.ErrorsListBox.Items.Count);
                Assert.IsTrue(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsFullNameValidLength_ValidationResult));
                Assert.IsFalse(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsRegisteredValid_ValidationResult));
                Assert.IsFalse(dataGrid.IsValid);

                // Committing the row, however, will trigger validation which should find the new error
                success = dataGrid.CommitEdit(DataGridEditingUnit.Row, true);
                Assert.IsFalse(success);
                Assert.AreEqual(2, dataGrid.TestHook.ErrorsListBox.Items.Count);
                Assert.IsTrue(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsFullNameValidLength_ValidationResult));
                Assert.IsTrue(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsRegisteredValid_ValidationResult));
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

                // Assert that the ErrorsListBox was initialized correctly
                Assert.IsNotNull(dataGrid.TestHook.ErrorsListBox);
                Assert.AreEqual(Visibility.Collapsed, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(0, dataGrid.TestHook.ErrorsListBox.Items.Count);
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
                // Assert that the ErrorsListBox updated correctly
                Assert.AreEqual(Visibility.Visible, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(1, dataGrid.TestHook.ErrorsListBox.Items.Count);
                Assert.IsTrue(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsFullNameValidLength_ValidationResult));
                Assert.IsFalse(dataGrid.IsValid);

                // Canceling the edit should remove the errors
                dataGrid.CancelEdit();
                Assert.AreEqual(-1, dataGrid.EditingColumnIndex);
                Assert.AreEqual(Visibility.Collapsed, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(0, dataGrid.TestHook.ErrorsListBox.Items.Count);
                Assert.IsTrue(dataGrid.IsValid);
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

                // Assert that the ErrorsListBox was initialized correctly
                Assert.IsNotNull(dataGrid.TestHook.ErrorsListBox);
                Assert.AreEqual(Visibility.Collapsed, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(0, dataGrid.TestHook.ErrorsListBox.Items.Count);

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
                // Assert that the ErrorsListBox updated correctly
                Assert.AreEqual(Visibility.Visible, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(1, dataGrid.TestHook.ErrorsListBox.Items.Count);
                Assert.IsTrue(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsFullNameValidLength_ValidationResult));

                // Selecting the IsFullNameValid error should set IsValid to false for all related cells
                dataGrid.TestHook.ErrorsListBox.SelectedItem = CustomerValidator.IsFullNameValidLength_ValidationResult;
                Assert.IsFalse(dataGrid.EditingRow.Cells[0].IsValid);
                Assert.IsFalse(dataGrid.EditingRow.Cells[1].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[2].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[3].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[4].IsValid);

                // Clicking on the IsFullNameValid error should begin editing on the first related cell
                dataGrid.ErrorsListBox_Click();
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
                dataGrid.ErrorsListBox_Click();
                Assert.AreEqual(1, dataGrid.EditingColumnIndex);
                dataGrid.ErrorsListBox_Click();
                Assert.AreEqual(0, dataGrid.EditingColumnIndex);

                // The error should not have been removed
                Assert.AreEqual(Visibility.Visible, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(1, dataGrid.TestHook.ErrorsListBox.Items.Count);
                Assert.IsTrue(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsFullNameValidLength_ValidationResult));
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
                dataGrid.ErrorsListBox_Click();
                Assert.AreEqual(-1, dataGrid.EditingColumnIndex);
                Assert.AreEqual(Visibility.Collapsed, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(0, dataGrid.TestHook.ErrorsListBox.Items.Count);
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        [Description("Ensures that the ErrorsListBox updates correctly when toggling between errors.")]
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

                // Assert that the ErrorsListBox was initialized correctly
                Assert.IsNotNull(dataGrid.TestHook.ErrorsListBox);
                Assert.IsNotNull(dataGrid.TestHook.ErrorsListBox.ItemsSource);
                Assert.AreEqual(System.Windows.Controls.SelectionMode.Single, dataGrid.TestHook.ErrorsListBox.SelectionMode);
                Assert.AreEqual(Visibility.Collapsed, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(0, dataGrid.TestHook.ErrorsListBox.Items.Count);

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

                // Assert that the ErrorsListBox updated correctly
                Assert.AreEqual(Visibility.Visible, dataGrid.TestHook.ErrorsListBox.Visibility);
                Assert.AreEqual(3, dataGrid.TestHook.ErrorsListBox.Items.Count);
                Assert.IsTrue(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsFullNameValidLength_ValidationResult));
                Assert.IsTrue(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsRegisteredValid_ValidationResult));
                Assert.IsTrue(dataGrid.TestHook.ErrorsListBox.Items.Contains(CustomerValidator.IsRatingValid_ValidationResult));

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
                dataGrid.TestHook.ErrorsListBox.SelectedItem = CustomerValidator.IsFullNameValidLength_ValidationResult;
                Assert.IsFalse(dataGrid.EditingRow.Cells[0].IsValid);
                Assert.IsFalse(dataGrid.EditingRow.Cells[1].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[2].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[3].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[4].IsValid);

                // Clicking on the IsFullNameValid error should cycle editing between the related cells
                dataGrid.ErrorsListBox_Click();
                Assert.AreEqual(0, dataGrid.EditingColumnIndex);
                dataGrid.ErrorsListBox_Click();
                Assert.AreEqual(1, dataGrid.EditingColumnIndex);
                dataGrid.ErrorsListBox_Click();
                Assert.AreEqual(0, dataGrid.EditingColumnIndex);

                // Selecting the IsRegisteredValid error should set IsValid to false on related cells, and true on the others
                dataGrid.TestHook.ErrorsListBox.SelectedItem = CustomerValidator.IsRegisteredValid_ValidationResult;
                Assert.IsTrue(dataGrid.EditingRow.Cells[0].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[1].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[2].IsValid);
                Assert.IsFalse(dataGrid.EditingRow.Cells[3].IsValid);
                Assert.IsFalse(dataGrid.EditingRow.Cells[4].IsValid);

                // Clicking on the IsRegisteredValid error should cycle editing between the related cells
                dataGrid.ErrorsListBox_Click();
                Assert.AreEqual(3, dataGrid.EditingColumnIndex);
                dataGrid.ErrorsListBox_Click();
                Assert.AreEqual(4, dataGrid.EditingColumnIndex);
                dataGrid.ErrorsListBox_Click();
                Assert.AreEqual(3, dataGrid.EditingColumnIndex);

                // Selecting the IsRatingValid error should set IsValid to true on all cells because the
                // ValidationResult did not specify any related MemberNames
                dataGrid.TestHook.ErrorsListBox.SelectedItem = CustomerValidator.IsRatingValid_ValidationResult;
                Assert.IsTrue(dataGrid.EditingRow.Cells[0].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[1].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[2].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[3].IsValid);
                Assert.IsTrue(dataGrid.EditingRow.Cells[4].IsValid);

                // Clicking on the IsRatingValid error should not begin editing because there are no related cells
                dataGrid.ErrorsListBox_Click();
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
