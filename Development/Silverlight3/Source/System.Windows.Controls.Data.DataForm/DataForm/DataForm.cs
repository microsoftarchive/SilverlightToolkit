//-----------------------------------------------------------------------
// <copyright company="Microsoft">
//      (c) Copyright Microsoft Corporation.
//      This source is subject to the Microsoft Public License (Ms-PL).
//      Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//      All other rights reserved.
// </copyright>
//-----------------------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Windows.Automation.Peers;
using System.Windows.Controls.Common;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Media;

namespace System.Windows.Controls
{
    /// <summary>
    /// Presents the specified contents of an Entity for viewing and editing.
    /// </summary>
    /// <QualityBand>Preview</QualityBand>
    [TemplatePart(Name = DATAFORM_elementHeaderElement, Type = typeof(ContentControl))]
    [TemplatePart(Name = DATAFORM_elementFieldsPresenter, Type = typeof(ContentPresenter))]
    [TemplatePart(Name = DATAFORM_elementErrorSummary, Type = typeof(ErrorSummary))]
    [TemplatePart(Name = DATAFORM_elementFirstItemButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementPreviousItemButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementNextItemButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementLastItemButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementButtonSeparator, Type = typeof(UIElement))]
    [TemplatePart(Name = DATAFORM_elementNewItemButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementDeleteItemButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementEditButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementCommitButton, Type = typeof(ButtonBase))]
    [TemplatePart(Name = DATAFORM_elementCancelButton, Type = typeof(ButtonBase))]
    [TemplateVisualState(Name = DATAFORM_stateNormal, GroupName = DATAFORM_groupCommon)]
    [TemplateVisualState(Name = DATAFORM_stateDisabled, GroupName = DATAFORM_groupCommon)]
    [TemplateVisualState(Name = DATAFORM_stateReadOnly, GroupName = DATAFORM_groupCommon)]
    [TemplateVisualState(Name = DATAFORM_stateDisplay, GroupName = DATAFORM_groupMode)]
    [TemplateVisualState(Name = DATAFORM_stateEmpty, GroupName = DATAFORM_groupMode)]
    [TemplateVisualState(Name = DATAFORM_stateEdit, GroupName = DATAFORM_groupMode)]
    [TemplateVisualState(Name = DATAFORM_stateInsert, GroupName = DATAFORM_groupMode)]
    [TemplateVisualState(Name = DATAFORM_stateValid, GroupName = DATAFORM_groupValidation)]
    [TemplateVisualState(Name = DATAFORM_stateInvalid, GroupName = DATAFORM_groupValidation)]
    [TemplateVisualState(Name = DATAFORM_stateCommitted, GroupName = DATAFORM_groupCommitted)]
    [TemplateVisualState(Name = DATAFORM_stateUncommitted, GroupName = DATAFORM_groupCommitted)]
    [TemplateVisualState(Name = DATAFORM_stateEntity, GroupName = DATAFORM_groupScope)]
    [TemplateVisualState(Name = DATAFORM_stateCollection, GroupName = DATAFORM_groupScope)]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1724:TypeNamesShouldNotMatchNamespaces", Justification = "The DataForm is the main control that lives in the namespace.")]
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "The class is necessarily complicated because of what it does.")]
    public class DataForm : Control
    {
        #region Constants

        private const int BottomYPosition = 3;
        private const int ColumnSpanThreeItems = 5;
        private const int ColumnSpanTwoItems = 3;
        private const int ColumnsPerField = 6;
        private const int FieldElementSpacing = 6; // px
        private const int LeftXPosition = 1;
        private const int MiddleXPosition = 3;
        private const int MiddleYPosition = 2;
        private const int RightXPosition = 5;
        private const int RowsPerField = 4;
        private const int TopYPosition = 1;
        private const int XEdgeSpacing = 5; // px
        private const int YEdgeSpacing = 3; // px

        private const string DATAFORM_elementHeaderElement = "HeaderElement";
        private const string DATAFORM_elementFieldsPresenter = "FieldsPresenter";
        private const string DATAFORM_elementErrorSummary = "ErrorSummary";
        private const string DATAFORM_elementDataForm = "DataForm";

        private const string DATAFORM_elementFirstItemButton = "FirstItemButton";
        private const string DATAFORM_elementPreviousItemButton = "PreviousItemButton";
        private const string DATAFORM_elementNextItemButton = "NextItemButton";
        private const string DATAFORM_elementLastItemButton = "LastItemButton";

        private const string DATAFORM_elementButtonSeparator = "ButtonSeparator";

        private const string DATAFORM_elementNewItemButton = "NewItemButton";
        private const string DATAFORM_elementDeleteItemButton = "DeleteItemButton";

        private const string DATAFORM_elementEditButton = "EditButton";
        private const string DATAFORM_elementCommitButton = "CommitButton";
        private const string DATAFORM_elementCancelButton = "CancelButton";

        private const string DATAFORM_groupCommon = "CommonStates";
        private const string DATAFORM_stateNormal = "Normal";
        private const string DATAFORM_stateDisabled = "Disabled";
        private const string DATAFORM_stateReadOnly = "ReadOnly";

        private const string DATAFORM_groupMode = "ModeStates";
        private const string DATAFORM_stateDisplay = "Display";
        private const string DATAFORM_stateEmpty = "Empty";
        private const string DATAFORM_stateEdit = "Edit";
        private const string DATAFORM_stateInsert = "Insert";

        private const string DATAFORM_groupValidation = "ValidationStates";
        private const string DATAFORM_stateValid = "Valid";
        private const string DATAFORM_stateInvalid = "Invalid";

        private const string DATAFORM_groupCommitted = "CommittedStates";
        private const string DATAFORM_stateCommitted = "Committed";
        private const string DATAFORM_stateUncommitted = "Uncommitted";

        private const string DATAFORM_groupScope = "ScopeStates";
        private const string DATAFORM_stateEntity = "Entity";
        private const string DATAFORM_stateCollection = "Collection";
        
        #endregion Constants

        #region Dependency Properties

        /// <summary>
        /// Identifies the AutoCommit dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoCommitProperty =
            DependencyProperty.Register(
                "AutoCommit",
                typeof(bool),
                typeof(DataForm),
                new PropertyMetadata(OnAutoCommitPropertyChanged));

        /// <summary>
        /// AutoCommit property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its AutoCommit value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnAutoCommitPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            SetAllCanPropertiesAndUpdate(dataForm, false /* onlyUpdateStates */);
        }

        /// <summary>
        /// Identifies the AutoEdit dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoEditProperty =
            DependencyProperty.Register(
                "AutoEdit",
                typeof(bool),
                typeof(DataForm),
                new PropertyMetadata(OnAutoEditPropertyChanged));

        /// <summary>
        /// AutoEdit property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its AutoEdit value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnAutoEditPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm != null)
            {
                if (!dataForm.IsEditing && dataForm.AutoEdit)
                {
                    dataForm.BeginEdit();
                }
            }
        }

        /// <summary>
        /// Identifies the AutoGenerateFields dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoGenerateFieldsProperty =
            DependencyProperty.Register(
                "AutoGenerateFields",
                typeof(bool),
                typeof(DataForm),
                new PropertyMetadata(OnAutoGenerateFieldsPropertyChanged));

        /// <summary>
        /// AutoGenerateFields property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its AutoGenerateFields value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnAutoGenerateFieldsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.UpdateButtonsAndStates();

            if (dataForm.AutoGenerateFields)
            {
                dataForm.GenerateFields();
            }
            else
            {
                dataForm.RemoveGeneratedFields();
            }

            dataForm.GenerateUI();
        }

        /// <summary>
        /// Identifies the CancelButtonContent dependency property.
        /// </summary>
        public static readonly DependencyProperty CancelButtonContentProperty =
            DependencyProperty.Register(
                "CancelButtonContent",
                typeof(object),
                typeof(DataForm),
                new PropertyMetadata(OnCancelButtonContentPropertyChanged));

        /// <summary>
        /// CancelButtonContent property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its CancelButtonContent value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnCancelButtonContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm._cancelButton != null)
            {
                dataForm._cancelButton.Content = dataForm.CancelButtonContent;
            }
        }

        /// <summary>
        /// Identifies the CanUserAddItems dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserAddItemsProperty =
            DependencyProperty.Register(
                "CanUserAddItems",
                typeof(bool),
                typeof(DataForm),
                new PropertyMetadata(OnCanUserAddItemsPropertyChanged));

        /// <summary>
        /// CanUserAddItems property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its CanUserAddItems value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnCanUserAddItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (!dataForm.CanUserAddItems)
            {
                dataForm._isUserAbleToAddItemsAfterEdit = false;
            }

            dataForm.UpdateButtonsAndStates();
        }

        /// <summary>
        /// Identifies the CanUserDeleteItems dependency property.
        /// </summary>
        public static readonly DependencyProperty CanUserDeleteItemsProperty =
            DependencyProperty.Register(
                "CanUserDeleteItems",
                typeof(bool),
                typeof(DataForm),
                new PropertyMetadata(OnCanUserDeleteItemsPropertyChanged));

        /// <summary>
        /// CanUserDeleteItems property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its CanUserDeleteItems value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnCanUserDeleteItemsPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (!dataForm.CanUserDeleteItems)
            {
                dataForm._isUserAbleToDeleteItemsAfterEdit = false;
            }

            dataForm.UpdateButtonsAndStates();
        }

        /// <summary>
        /// Identifies the CommandButtonsVisibility dependency property.
        /// </summary>
        public static readonly DependencyProperty CommandButtonsVisibilityProperty =
            DependencyProperty.Register(
                "CommandButtonsVisibility",
                typeof(DataFormCommandButtonsVisibility),
                typeof(DataForm),
                new PropertyMetadata(OnCommandButtonsVisibilityPropertyChanged));

        /// <summary>
        /// CommandButtonsVisibility property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its CommandButtonsVisibility value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnCommandButtonsVisibilityPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm._buttonSeparator != null)
            {
                dataForm.SetButtonSeparatorVisibility();
            }

            if (dataForm._newItemButton != null)
            {
                dataForm.SetNewItemButtonVisibility();
            }

            if (dataForm._deleteItemButton != null)
            {
                dataForm.SetDeleteItemButtonVisibility();
            }

            if (dataForm._editButton != null)
            {
                dataForm.SetEditItemButtonVisibility();
            }

            if (dataForm._commitButton != null)
            {
                dataForm.SetCommitButtonVisibility();
            }

            if (dataForm._cancelButton != null)
            {
                dataForm.SetCancelButtonVisibility();
            }
        }

        /// <summary>
        /// Identifies the CommitButtonContent dependency property.
        /// </summary>
        public static readonly DependencyProperty CommitButtonContentProperty =
            DependencyProperty.Register(
                "CommitButtonContent",
                typeof(object),
                typeof(DataForm),
                new PropertyMetadata(OnCommitButtonContentPropertyChanged));

        /// <summary>
        /// CommitButtonContent property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its CommitButtonContent value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnCommitButtonContentPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm._commitButton != null)
            {
                dataForm._commitButton.Content = dataForm.CommitButtonContent;
            }
        }

        /// <summary>
        /// Identifies the CurrentIndex dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentIndexProperty =
            DependencyProperty.Register(
                "CurrentIndex",
                typeof(int),
                typeof(DataForm),
                new PropertyMetadata(OnCurrentIndexPropertyChanged));

        /// <summary>
        /// CurrentIndex property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its CurrentItem value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnCurrentIndexPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm != null && !dataForm.AreHandlersSuspended())
            {
                // Don't let the CurrentIndex be set if there's no collection
                // or if the value is invalid.
                if (dataForm._collectionView == null)
                {
                    if ((dataForm.CurrentItem == null && dataForm.CurrentIndex != -1) ||
                        (dataForm.CurrentItem != null && dataForm.CurrentIndex != 0))
                    {
                        dataForm.SetValueNoCallback(e.Property, e.OldValue);
                    }
                }
                else
                {
                    if (dataForm.CurrentIndex < -1 || dataForm.CurrentIndex >= dataForm.ItemsCount)
                    {
                        dataForm.SetValueNoCallback(e.Property, e.OldValue);
                    }
                    else
                    {
                        dataForm._collectionView.MoveCurrentToPosition(dataForm.CurrentIndex);

                        if (dataForm.CurrentIndex != dataForm._collectionView.CurrentPosition)
                        {
                            dataForm.SetValueNoCallback(e.Property, e.OldValue);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Identifies the CurrentItem dependency property.
        /// </summary>
        public static readonly DependencyProperty CurrentItemProperty =
            DependencyProperty.Register(
                "CurrentItem",
                typeof(object),
                typeof(DataForm),
                new PropertyMetadata(OnCurrentItemPropertyChanged));

        /// <summary>
        /// CurrentItem property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its CurrentItem value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnCurrentItemPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm != null && !dataForm.AreHandlersSuspended())
            {
                if (((dataForm.AutoCommit || !dataForm.IsEditing) && dataForm.IsItemValid) &&
                    (e.NewValue == null ||
                    dataForm._collectionView == null ||
                    dataForm._collectionView.Contains(dataForm.CurrentItem)))
                {
                    if (dataForm._collectionView != null)
                    {
                        if (e.NewValue != null || dataForm._collectionView.Contains(dataForm.CurrentItem))
                        {
                            dataForm._collectionView.MoveCurrentTo(dataForm.CurrentItem);
                        }
                        else
                        {
                            dataForm._collectionView.MoveCurrentToPosition(-1);
                        }
                    }

                    if (dataForm.CurrentItemNotifyPropertyChanged != null)
                    {
                        dataForm.CurrentItemNotifyPropertyChanged.PropertyChanged -= new PropertyChangedEventHandler(dataForm.OnCurrentItemPropertyChanged);
                    }

                    dataForm.ForceEndEdit();

                    dataForm._rootField.IsDataFormCurrentItemNull = dataForm.CurrentItem == null;

                    if (dataForm.CurrentItemNotifyPropertyChanged != null)
                    {
                        dataForm.CurrentItemNotifyPropertyChanged.PropertyChanged += new PropertyChangedEventHandler(dataForm.OnCurrentItemPropertyChanged);
                    }

                    if (dataForm.AutoGenerateFields && dataForm._lastItemType != dataForm.CurrentItemType)
                    {
                        dataForm.GenerateFields();
                    }

                    dataForm.GenerateUI();
                    dataForm.UpdateCurrentItem();
                    SetAllCanPropertiesAndUpdate(dataForm, false /* onlyUpdateStates */);

                    if (dataForm.CurrentItem != null && dataForm.AutoEdit)
                    {
                        dataForm.BeginEdit();
                    }

                    dataForm._lastItem = dataForm.CurrentItem;
                    dataForm._lastItemType = dataForm.CurrentItemType;
                    dataForm.OnCurrentItemChanged(EventArgs.Empty);
                }
                else
                {
                    dataForm.SetValueNoCallback(e.Property, e.OldValue);

                    dataForm.Dispatcher.BeginInvoke(() =>
                        {
                            BindingExpression be = dataForm.GetBindingExpression(DataForm.CurrentItemProperty);

                            if (be != null)
                            {
                                be.UpdateSource();
                            }
                        });
                }
            }
        }

        /// <summary>
        /// Identifies the DescriptionViewerPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionViewerPositionProperty =
            DependencyProperty.Register(
                "DescriptionViewerPosition",
                typeof(DataFormDescriptionViewerPosition),
                typeof(DataForm),
                new PropertyMetadata(OnDescriptionViewerPositionPropertyChanged));

        /// <summary>
        /// DescriptionViewerPosition property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its DescriptionViewerPosition value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDescriptionViewerPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.GenerateUI();
        }

        /// <summary>
        /// Identifies the DescriptionViewerStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty DescriptionViewerStyleProperty =
            DependencyProperty.Register(
                "DescriptionViewerStyle",
                typeof(Style),
                typeof(DataForm),
                new PropertyMetadata(OnDescriptionViewerStylePropertyChanged));

        /// <summary>
        /// DescriptionViewerStyle property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its DescriptionViewerStyle value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDescriptionViewerStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.GenerateUI();
        }

        /// <summary>
        /// Identifies the DisplayTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayTemplateProperty =
            DependencyProperty.Register(
                "DisplayTemplate",
                typeof(DataTemplate),
                typeof(DataForm),
                new PropertyMetadata(OnDisplayTemplatePropertyChanged));

        /// <summary>
        /// DisplayTemplate property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its DisplayTemplate value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnDisplayTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            SetAllCanPropertiesAndUpdate(dataForm, true /* onlyUpdateStates */);
        }

        /// <summary>
        /// Identifies the EditTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty EditTemplateProperty =
            DependencyProperty.Register(
                "EditTemplate",
                typeof(DataTemplate),
                typeof(DataForm),
                new PropertyMetadata(OnEditTemplatePropertyChanged));

        /// <summary>
        /// EditTemplate property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its EditTemplate value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnEditTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            SetAllCanPropertiesAndUpdate(dataForm, true /* onlyUpdateStates */);
        }

        /// <summary>
        /// Identifies the ErrorSummaryStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty ErrorSummaryStyleProperty =
            DependencyProperty.Register(
                "ErrorSummaryStyle",
                typeof(Style),
                typeof(DataForm),
                new PropertyMetadata(OnErrorSummaryStylePropertyChanged));

        /// <summary>
        /// ErrorSummaryStyle property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its ErrorSummaryStyle value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnErrorSummaryStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;

            if (dataForm._errorSummary != null)
            {
                dataForm._errorSummary.Style = dataForm.ErrorSummaryStyle;
            }
        }

        /// <summary>
        /// Identifies the FieldLabelPosition dependency property.
        /// </summary>
        public static readonly DependencyProperty FieldLabelPositionProperty =
            DependencyProperty.Register(
                "FieldLabelPosition",
                typeof(DataFormFieldLabelPosition),
                typeof(DataForm),
                new PropertyMetadata(OnFieldLabelPositionPropertyChanged));

        /// <summary>
        /// FieldLabelPosition property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its FieldLabelPosition value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnFieldLabelPositionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.GenerateUI();
        }

        /// <summary>
        /// Identifies the FieldLabelStyle dependency property.
        /// </summary>
        public static readonly DependencyProperty FieldLabelStyleProperty =
            DependencyProperty.Register(
                "FieldLabelStyle",
                typeof(Style),
                typeof(DataForm),
                new PropertyMetadata(OnFieldLabelStylePropertyChanged));

        /// <summary>
        /// FieldLabelStyle property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its FieldLabelStyle value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnFieldLabelStylePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.GenerateUI();
        }

        /// <summary>
        /// Identifies the Header dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderProperty =
            DependencyProperty.Register(
                "Header",
                typeof(object),
                typeof(DataForm),
                null);

        /// <summary>
        /// Identifies the HeaderTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty HeaderTemplateProperty =
            DependencyProperty.Register(
                "HeaderTemplate",
                typeof(DataTemplate),
                typeof(DataForm),
                null);

        /// <summary>
        /// Identifies the InsertTemplate dependency property.
        /// </summary>
        public static readonly DependencyProperty InsertTemplateProperty =
            DependencyProperty.Register(
                "InsertTemplate",
                typeof(DataTemplate),
                typeof(DataForm),
                new PropertyMetadata(OnInsertTemplatePropertyChanged));

        /// <summary>
        /// InsertTemplate property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its InsertTemplate value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnInsertTemplatePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            SetAllCanPropertiesAndUpdate(dataForm, true /* onlyUpdateStates */);
        }

        /// <summary>
        /// Identifies the IsReadOnly dependency property.
        /// </summary>
        public static readonly DependencyProperty IsReadOnlyProperty =
            DependencyProperty.Register(
                "IsReadOnly",
                typeof(bool),
                typeof(DataForm),
                new PropertyMetadata(OnIsReadOnlyPropertyChanged));

        /// <summary>
        /// IsReadOnly property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its IsReadOnly value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnIsReadOnlyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;

            if (dataForm != null && dataForm.IsEditing)
            {
                dataForm.CancelItemEdit();
            }

            SetAllCanPropertiesAndUpdate(dataForm, false /* onlyUpdateStates */);
        }

        /// <summary>
        /// Identifies the ItemsSource dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsSourceProperty =
            DependencyProperty.Register(
                "ItemsSource",
                typeof(IEnumerable),
                typeof(DataForm),
                new PropertyMetadata(OnItemsSourcePropertyChanged));

        /// <summary>
        /// ItemsSource property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its ItemsSource value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnItemsSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            if (dataForm == null)
            {
                return;
            }

            if (dataForm._collectionView != null)
            {
                dataForm._collectionView.CurrentChanging -= new CurrentChangingEventHandler(dataForm.OnCollectionViewCurrentChanging);
                dataForm._collectionView.CurrentChanged -= new EventHandler(dataForm.OnCollectionViewCurrentChanged);
                dataForm._collectionView.CollectionChanged -= new NotifyCollectionChangedEventHandler(dataForm.OnCollectionViewCollectionChanged);
            }

            dataForm.ForceEndEdit();
            dataForm._collectionView = null;

            IEnumerable newItemsSource = e.NewValue as IEnumerable;
            dataForm._originalItemsSource = newItemsSource;

            bool collectionViewCreated = false;
            ICollectionView collectionView = newItemsSource as ICollectionView;

            if (collectionView != null)
            {
                dataForm._collectionView = collectionView;
            }
            else if (newItemsSource != null)
            {
                dataForm._collectionView = new PagedCollectionView(newItemsSource);
                collectionViewCreated = true;
            }

            if (dataForm._collectionView != null)
            {
                dataForm._collectionView.CurrentChanging += new CurrentChangingEventHandler(dataForm.OnCollectionViewCurrentChanging);
                dataForm._collectionView.CurrentChanged += new EventHandler(dataForm.OnCollectionViewCurrentChanged);
                dataForm._collectionView.CollectionChanged += new NotifyCollectionChangedEventHandler(dataForm.OnCollectionViewCollectionChanged);

                if (collectionViewCreated || dataForm._collectionView.IsCurrentBeforeFirst)
                {
                    if (dataForm._collectionView.CurrentPosition == 0 || !dataForm._collectionView.MoveCurrentToFirst())
                    {
                        // This move can fail in the case of an empty set, which will then require that
                        // the below happen manually.
                        dataForm.UpdateCurrentItem();
                    }
                }
                else
                {
                    // The above will not cause a CurrentChanged event to be fired when the position
                    // is initialized to the first item, so change DataForm.CurrentItem manually
                    // in that case.
                    dataForm.UpdateCurrentItem();
                }
            }
            else
            {
                dataForm.CurrentItem = dataForm.DataContext;
                dataForm.UpdateCurrentItem();
            }

            dataForm.SetAllCanProperties();

            if (dataForm.AutoGenerateFields)
            {
                dataForm.GenerateFields();
            }

            dataForm.GenerateUI();

            if (dataForm._buttonSeparator != null)
            {
                dataForm.SetButtonSeparatorVisibility();
            }
        }

        /// <summary>
        /// Identifies the Orientation dependency property.
        /// </summary>
        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                "Orientation",
                typeof(Orientation),
                typeof(DataForm),
                new PropertyMetadata(OnOrientationPropertyChanged));

        /// <summary>
        /// Orientation property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its Orientation value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnOrientationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.GenerateUI();
        }

        /// <summary>
        /// Identifies the WrapAfter dependency property.
        /// </summary>
        public static readonly DependencyProperty WrapAfterProperty =
            DependencyProperty.Register(
                "WrapAfter",
                typeof(int),
                typeof(DataForm),
                new PropertyMetadata(OnWrapAfterPropertyChanged));

        /// <summary>
        /// WrapAfter property changed handler.
        /// </summary>
        /// <param name="d">DataForm that changed its WrapAfter value.</param>
        /// <param name="e">The DependencyPropertyChangedEventArgs for this event.</param>
        private static void OnWrapAfterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            DataForm dataForm = d as DataForm;
            dataForm.GenerateUI();
        }

        #endregion Dependency Properties

        #region Fields

        /// <summary>
        /// Private accessor to ButtonSeparator.
        /// </summary>
        private UIElement _buttonSeparator;

        /// <summary>
        /// Private accessor to CanAddItems.
        /// </summary>
        private bool _canAddItems;

        /// <summary>
        /// Private accessor to CanBeginEdit.
        /// </summary>
        private bool _canBeginEdit;

        /// <summary>
        /// Private accessor to CanCancelEdit.
        /// </summary>
        private bool _canCancelEdit;

        /// <summary>
        /// Private accessor to CancelButton.
        /// </summary>
        private ButtonBase _cancelButton;

        /// <summary>
        /// Private accessor to CanDeleteItems.
        /// </summary>
        private bool _canDeleteItems;

        /// <summary>
        /// Private accessor to CanCommitEdit.
        /// </summary>
        private bool _canCommitEdit;

        /// <summary>
        /// Private accessor to CanMoveToFirstItem.
        /// </summary>
        private bool _canMoveToFirstItem;

        /// <summary>
        /// Private accessor to CanMoveToLastItem.
        /// </summary>
        private bool _canMoveToLastItem;

        /// <summary>
        /// Private accessor to CanMoveToNextItem.
        /// </summary>
        private bool _canMoveToNextItem;

        /// <summary>
        /// Private accessor to CanMoveToPreviousItem.
        /// </summary>
        private bool _canMoveToPreviousItem;

        /// <summary>
        /// Represents the items source as an ICollectionView.
        /// </summary>
        private ICollectionView _collectionView;

        /// <summary>
        /// Private accessor to DeleteItemButton.
        /// </summary>
        private ButtonBase _deleteItemButton;

        /// <summary>
        /// Private accessor to EditButton.
        /// </summary>
        private ButtonBase _editButton;

        /// <summary>
        /// Dictionary tying elements to their labels.
        /// </summary>
        private IDictionary<FrameworkElement, FieldLabel> _elementFieldLabelDictionary = new Dictionary<FrameworkElement, FieldLabel>();

        /// <summary>
        /// Dictionary tying elements to their error viewers.
        /// </summary>
        private IDictionary<FrameworkElement, DescriptionViewer> _elementDescriptionViewerDictionary = new Dictionary<FrameworkElement, DescriptionViewer>();

        /// <summary>
        /// Holds the entity-level validation errors.
        /// </summary>
        private ValidationErrorCollection _entityLevelErrors;

        /// <summary>
        /// Private accessor to ErrorSummary.
        /// </summary>
        private ErrorSummary _errorSummary;

        /// <summary>
        /// Holds the developer-specified collection of fields.
        /// </summary>
        private ObservableCollection<DataFormField> _fields;

        /// <summary>
        /// Private accessor to the fields presenter.
        /// </summary>
        private ContentPresenter _fieldsPresenter;

        /// <summary>
        /// Private accessor to FirstItemButton.
        /// </summary>
        private ButtonBase _firstItemButton;

        /// <summary>
        /// Holds a value indicating whether or not to ignore a change in the fields
        /// collection.
        /// </summary>
        private bool _ignoreFieldsCollectionChange;

        /// <summary>
        /// Holds the collection of invalid fields.
        /// </summary>
        private ICollection<DataFormField> _invalidFields;

        /// <summary>
        /// Holds the collection of invalid template elements.
        /// </summary>
        private ICollection<FrameworkElement> _invalidElements;

        /// <summary>
        /// Private accessor to a value indicating whether or not the user is appending a new item.
        /// </summary>
        private bool _isAddingNew;

        /// <summary>
        /// Private accessor to a value indicating whether or not the user is editing the current item.
        /// </summary>
        private bool _isEditing;

        /// <summary>
        /// Private accessor to a value indicating whether or not the control is displaying an item.
        /// </summary>
        private bool _isEmpty;

        /// <summary>
        /// Private accessor to a value indicating whether or not the current item has been changed.
        /// </summary>
        private bool _isItemChanged;

        /// <summary>
        /// Private accessor to a value indicating whether or not the current item is valid.
        /// </summary>
        private bool _isItemValid;

        /// <summary>
        /// The value of IsUserAbleToAddItems prior to entering edit mode.
        /// </summary>
        private bool _isUserAbleToAddItemsAfterEdit;

        /// <summary>
        /// The value of IsUserAbleToDeleteItems prior to entering edit mode.
        /// </summary>
        private bool _isUserAbleToDeleteItemsAfterEdit;

        /// <summary>
        /// Holds the index of the last value of CurrentPosition on the PagedCollectionView.
        /// </summary>
        private int _lastCurrentPosition;

        /// <summary>
        /// The last value of CurrentItem, for use when auto-committing an edit.
        /// </summary>
        private object _lastItem;

        /// <summary>
        /// Private accessor to LastItemButton.
        /// </summary>
        private ButtonBase _lastItemButton;

        /// <summary>
        /// The last type of CurrentItem, for use when regenerating fields when the
        /// current item changes.
        /// </summary>
        private object _lastItemType;

        /// <summary>
        /// Private accessor to NewItemButton.
        /// </summary>
        private ButtonBase _newItemButton;

        /// <summary>
        /// Private accessor to NextItemButton.
        /// </summary>
        private ButtonBase _nextItemButton;

        /// <summary>
        /// Holds the next tab index for generated fields.
        /// </summary>
        private int _nextTabIndex;

        /// <summary>
        /// Represents the original items source prior to wrapping.
        /// </summary>
        private IEnumerable _originalItemsSource;

        /// <summary>
        /// Private accessor to PreviousItemButton.
        /// </summary>
        private ButtonBase _previousItemButton;

        /// <summary>
        /// The root group field.
        /// </summary>
        private DataFormFieldGroup _rootField;

        /// <summary>
        /// Private accessor to CommitButton.
        /// </summary>
        private ButtonBase _commitButton;

        /// <summary>
        /// Holds the binding expressions retrieved from the current template.
        /// </summary>
        private IList<BindingExpression> _templateBindingExpressions;

        #endregion Fields

        #region Constructors

        /// <summary>
        /// Constructs a new instance of DataForm.
        /// </summary>
        public DataForm()
        {
            this.DefaultStyleKey = typeof(DataForm);
            this._fields = new ObservableCollection<DataFormField>();
            this._fields.CollectionChanged += new NotifyCollectionChangedEventHandler(this.OnFieldsCollectionChanged);
            this._isItemValid = true;

            this._rootField = new DataFormFieldGroup();
            this._invalidFields = new Collection<DataFormField>();
            this._entityLevelErrors = new ValidationErrorCollection();
            this._invalidElements = new Collection<FrameworkElement>();

            this.Labels = new List<FrameworkElement>();
            this.Descriptions = new List<FrameworkElement>();
            this.InputControls = new List<FrameworkElement>();
        }

        #endregion Constructors

        #region Events

        /// <summary>
        /// Event handler for when an item is being added.
        /// </summary>
        public event EventHandler<CancelEventArgs> AddingItem;

        /// <summary>
        /// Event handler for when the fields have been auto-generated.
        /// </summary>
        public event EventHandler<EventArgs> AutoGeneratedFields;

        /// <summary>
        /// Event handler for when a field is being auto-generated.
        /// </summary>
        public event EventHandler<DataFormAutoGeneratingFieldEventArgs> AutoGeneratingField;

        /// <summary>
        /// Event handler for when an edit is beginning.
        /// </summary>
        public event EventHandler<CancelEventArgs> BeginningEdit;

        /// <summary>
        /// Event handler for when content is loaded.
        /// </summary>
        public event EventHandler<DataFormContentLoadedEventArgs> ContentLoaded;

        /// <summary>
        /// Event handler for when the current item has changed.
        /// </summary>
        public event EventHandler<EventArgs> CurrentItemChanged;

        /// <summary>
        /// Event handler for when an item is being deleted.
        /// </summary>
        public event EventHandler<CancelEventArgs> DeletingItem;

        /// <summary>
        /// Event handler for when a field edit has ended
        /// </summary>
        public event EventHandler<DataFormFieldEditEndedEventArgs> FieldEditEnded;

        /// <summary>
        /// Event handler for when a field edit is ending.
        /// </summary>
        public event EventHandler<DataFormFieldEditEndingEventArgs> FieldEditEnding;

        /// <summary>
        /// Event handler for when an item edit has ended
        /// </summary>
        public event EventHandler<DataFormItemEditEndedEventArgs> ItemEditEnded;

        /// <summary>
        /// Event handler for when an item edit is ending.
        /// </summary>
        public event EventHandler<DataFormItemEditEndingEventArgs> ItemEditEnding;

        /// <summary>
        /// Event handler for when the item is being validated.
        /// </summary>
        public event EventHandler<CancelEventArgs> ValidatingItem;

        #endregion Events

        #region Properties

        #region Public Properties

        /// <summary>
        /// Gets or sets a value indicating whether or not to auto-commit items when the
        /// current item is changed while the user is editing an item.
        /// </summary>
        public bool AutoCommit
        {
            get
            {
                return (bool)GetValue(AutoCommitProperty);
            }

            set
            {
                this.SetValue(AutoCommitProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the DataForm should be
        /// permanently in edit mode.
        /// </summary>
        public bool AutoEdit
        {
            get
            {
                return (bool)GetValue(AutoEditProperty);
            }

            set
            {
                this.SetValue(AutoEditProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not to automatically generate
        /// the Fields collection.
        /// </summary>
        public bool AutoGenerateFields
        {
            get
            {
                return (bool)GetValue(AutoGenerateFieldsProperty);
            }

            set
            {
                this.SetValue(AutoGenerateFieldsProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not an edit can be begun.
        /// </summary>
        public bool CanBeginEdit
        {
            get
            {
                return this._canBeginEdit;
            }

            private set
            {
                if (value != this._canBeginEdit)
                {
                    this._canBeginEdit = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not an edit can be canceled.
        /// </summary>
        public bool CanCancelEdit
        {
            get
            {
                return this._canCancelEdit;
            }

            private set
            {
                if (value != this._canCancelEdit)
                {
                    this._canCancelEdit = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets or sets the cancel button content.
        /// </summary>
        public object CancelButtonContent
        {
            get
            {
                return this.GetValue(CancelButtonContentProperty);
            }

            set
            {
                this.SetValue(CancelButtonContentProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not you can to the first item.
        /// </summary>
        public bool CanMoveToFirstItem
        {
            get
            {
                return this._canMoveToFirstItem;
            }

            private set
            {
                if (value != this._canMoveToFirstItem)
                {
                    this._canMoveToFirstItem = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not you can to the last item.
        /// </summary>
        public bool CanMoveToLastItem
        {
            get
            {
                return this._canMoveToLastItem;
            }

            private set
            {
                if (value != this._canMoveToLastItem)
                {
                    this._canMoveToLastItem = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not you can to the next item.
        /// </summary>
        public bool CanMoveToNextItem
        {
            get
            {
                return this._canMoveToNextItem;
            }

            private set
            {
                if (value != this._canMoveToNextItem)
                {
                    this._canMoveToNextItem = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not you can to the previous item.
        /// </summary>
        public bool CanMoveToPreviousItem
        {
            get
            {
                return this._canMoveToPreviousItem;
            }

            private set
            {
                if (value != this._canMoveToPreviousItem)
                {
                    this._canMoveToPreviousItem = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not users can add a new item.
        /// </summary>
        public bool CanUserAddItems
        {
            get
            {
                return (bool)this.GetValue(CanUserAddItemsProperty);
            }

            set
            {
                this.SetValue(CanUserAddItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not users can delete an item.
        /// </summary>
        public bool CanUserDeleteItems
        {
            get
            {
                return (bool)this.GetValue(CanUserDeleteItemsProperty);
            }

            set
            {
                this.SetValue(CanUserDeleteItemsProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the command buttons visibility.
        /// </summary>
        public DataFormCommandButtonsVisibility CommandButtonsVisibility
        {
            get
            {
                return (DataFormCommandButtonsVisibility)GetValue(CommandButtonsVisibilityProperty);
            }

            set
            {
                this.SetValue(CommandButtonsVisibilityProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the commit button content.
        /// </summary>
        public object CommitButtonContent
        {
            get
            {
                return this.GetValue(CommitButtonContentProperty);
            }

            set
            {
                this.SetValue(CommitButtonContentProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current index.
        /// </summary>
        public int CurrentIndex
        {
            get
            {
                return (int)this.GetValue(CurrentIndexProperty);
            }

            set
            {
                this.SetValue(CurrentIndexProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the current item.
        /// </summary>
        public object CurrentItem
        {
            get
            {
                return this.GetValue(CurrentItemProperty);
            }

            set
            {
                this.SetValue(CurrentItemProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the developer-specified location to place descriptions.
        /// </summary>
        public DataFormDescriptionViewerPosition DescriptionViewerPosition
        {
            get
            {
                return (DataFormDescriptionViewerPosition)this.GetValue(DescriptionViewerPositionProperty);
            }

            set
            {
                this.SetValue(DescriptionViewerPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the developer-specified style for descriptions.
        /// </summary>
        public Style DescriptionViewerStyle
        {
            get
            {
                return this.GetValue(DescriptionViewerStyleProperty) as Style;
            }

            set
            {
                this.SetValue(DescriptionViewerStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the item template used when not editing.
        /// </summary>
        public DataTemplate DisplayTemplate
        {
            get
            {
                return this.GetValue(DisplayTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(DisplayTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the item template used when editing.
        /// </summary>
        public DataTemplate EditTemplate
        {
            get
            {
                return this.GetValue(EditTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(EditTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets the error summary.
        /// </summary>
        public ErrorSummary ErrorSummary
        {
            get
            {
                return this._errorSummary;
            }
        }

        /// <summary>
        /// Gets or sets the error summary style.
        /// </summary>
        public Style ErrorSummaryStyle
        {
            get
            {
                return this.GetValue(ErrorSummaryStyleProperty) as Style;
            }

            set
            {
                this.SetValue(ErrorSummaryStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the developer-specified location to place labels.
        /// </summary>
        public DataFormFieldLabelPosition FieldLabelPosition
        {
            get
            {
                return (DataFormFieldLabelPosition)this.GetValue(FieldLabelPositionProperty);
            }

            set
            {
                this.SetValue(FieldLabelPositionProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the developer-specified style for labels.
        /// </summary>
        public Style FieldLabelStyle
        {
            get
            {
                return this.GetValue(FieldLabelStyleProperty) as Style;
            }

            set
            {
                this.SetValue(FieldLabelStyleProperty, value);
            }
        }

        /// <summary>
        /// Gets the field collection.
        /// </summary>
        public ObservableCollection<DataFormField> Fields
        {
            get
            {
                return this._fields;
            }
        }

        /// <summary>
        /// Gets or sets the content of the header.
        /// </summary>
        public object Header
        {
            get
            {
                return this.GetValue(HeaderProperty);
            }

            set
            {
                this.SetValue(HeaderProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the template of the header.
        /// </summary>
        public DataTemplate HeaderTemplate
        {
            get
            {
                return this.GetValue(HeaderTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(HeaderTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the item template used when inserting.
        /// </summary>
        public DataTemplate InsertTemplate
        {
            get
            {
                return this.GetValue(InsertTemplateProperty) as DataTemplate;
            }

            set
            {
                this.SetValue(InsertTemplateProperty, value);
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the user is appending a new item.
        /// </summary>
        public bool IsAddingNew
        {
            get
            {
                return this._isAddingNew;
            }

            private set
            {
                if (value != this._isAddingNew)
                {
                    this._isAddingNew = value;
                    this.SetAllCanProperties();
                    this.IsEditing = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the user is editing the current item.
        /// </summary>
        public bool IsEditing
        {
            get
            {
                return this._isEditing;
            }

            private set
            {
                if (value != this._isEditing)
                {
                    this._isEditing = value;
                    this.SetAllCanProperties();
                    this.GenerateUI();
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the control is displaying an item.
        /// </summary>
        public bool IsEmpty
        {
            get
            {
                return this._isEmpty;
            }

            private set
            {
                if (value != this._isEmpty)
                {
                    this._isEmpty = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the current item has been changed.
        /// </summary>
        public bool IsItemChanged
        {
            get
            {
                return this._isItemChanged;
            }

            private set
            {
                if (value != this._isItemChanged)
                {
                    this._isItemChanged = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the current item is valid.
        /// </summary>
        public bool IsItemValid
        {
            get
            {
                return this._isItemValid;
            }

            private set
            {
                if (value != this._isItemValid)
                {
                    this._isItemValid = value;
                    this.SetAllCanProperties();
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether or not the control is in read-only mode.
        /// </summary>
        public bool IsReadOnly
        {
            get
            {
                return (bool)GetValue(IsReadOnlyProperty);
            }

            set
            {
                this.SetValue(IsReadOnlyProperty, value);
            }
        }

        /// <summary>
        /// Gets the number of items in the items source.
        /// </summary>
        public int ItemsCount
        {
            get
            {
                // If the items source implements IList, use its Count property.
                IList listItemsSource = this._originalItemsSource as IList;

                if (listItemsSource != null)
                {
                    return listItemsSource.Count;
                }

                // If the items source implements IIndexableCollection, use its Count property.
                IIndexableCollection indexableCollectionItemsSource = this._originalItemsSource as IIndexableCollection;

                if (indexableCollectionItemsSource != null)
                {
                    return indexableCollectionItemsSource.Count;
                }

                // If the items source implements IEnumerable, use the extension method count on IEnumerable<T>.
                IEnumerable enumerableItemsSource = this._originalItemsSource;

                if (enumerableItemsSource != null)
                {
                    // This will always succeed since any IEnumerable is an IEnumerable with generic type object.
                    IEnumerable<object> genericEnumerableItemsSource = enumerableItemsSource.Cast<object>();

                    return genericEnumerableItemsSource.Count();
                }

                // If it's not even an IEnumerable, we've got either a singleton or nothing.
                return this.CurrentItem != null ? 1 : 0;
            }
        }

        /// <summary>
        /// Gets or sets the items source.
        /// </summary>
        public IEnumerable ItemsSource
        {
            get
            {
                return this.GetValue(ItemsSourceProperty) as IEnumerable;
            }

            set
            {
                this.SetValue(ItemsSourceProperty, value);
            }
        }

        /// <summary>
        /// Gets the current mode of the DataForm.
        /// </summary>
        public DataFormMode Mode
        {
            get
            {
                if (this._isAddingNew)
                {
                    return DataFormMode.Insert;
                }
                else if (this._isEditing)
                {
                    return DataFormMode.Edit;
                }
                else
                {
                    return DataFormMode.Display;
                }
            }
        }

        /// <summary>
        /// Gets or sets the orientation.
        /// </summary>
        public Orientation Orientation
        {
            get
            {
                return (Orientation)this.GetValue(OrientationProperty);
            }

            set
            {
                this.SetValue(OrientationProperty, value);
            }
        }

        /// <summary>
        /// Gets or sets the number of fields to wrap after.
        /// </summary>
        public int WrapAfter
        {
            get
            {
                return (int)this.GetValue(WrapAfterProperty);
            }

            set
            {
                this.SetValue(WrapAfterProperty, value);
            }
        }

        #endregion Public Properties

        #region Internal Properties

        /// <summary>
        /// Gets a value indicating whether or not an edit can be committed.
        /// </summary>
        internal bool CanCommitEdit
        {
            get
            {
                return this._canCommitEdit;
            }

            private set
            {
                if (value != this._canCommitEdit)
                {
                    this._canCommitEdit = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets the type of the current item.
        /// </summary>
        internal Type CurrentItemType
        {
            get
            {
                if (this.CurrentItem == null)
                {
                    return null;
                }
                else
                {
                    return this.CurrentItem.GetType();
                }
            }
        }

        /// <summary>
        /// Gets the descriptions being displayed in the DataForm.  For use only for unit tests.
        /// </summary>
        internal List<FrameworkElement> Descriptions
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether or not the DataForm is effectively read-only,
        /// taking into account both IsReadOnly and the presence of templates.
        /// </summary>
        internal bool EffectiveIsReadOnly
        {
            get
            {
                if (this.DisplayTemplate != null && this.EditTemplate == null && this.InsertTemplate == null)
                {
                    return true;
                }
                else
                {
                    return this.IsReadOnly;
                }
            }
        }

        /// <summary>
        /// Gets the input controls being displayed in the DataForm.  For use only for unit tests.
        /// </summary>
        internal List<FrameworkElement> InputControls
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether or not users are able to add
        /// a new item.  This takes into account the values both of
        /// CanUserAddItems and CanAddItems.
        /// </summary>
        internal bool IsUserAbleToAddItems
        {
            get
            {
                return this.CanAddItems && this.CanUserAddItems;
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not users are able to add
        /// a new item.  This takes into account the values both of
        /// CanUserDeleteItems and CanDeleteItems.
        /// </summary>
        internal bool IsUserAbleToDeleteItems
        {
            get
            {
                return this.CanDeleteItems && this.CanUserDeleteItems;
            }
        }

        /// <summary>
        /// Gets the labels being displayed in the DataForm.  For use only for unit tests.
        /// </summary>
        internal List<FrameworkElement> Labels
        {
            get;
            set;
        }

        /// <summary>
        /// Gets a value indicating whether or not the UI has been generated.
        /// </summary>
        internal bool UIGenerated
        {
            get;
            set;
        }

        #endregion Internal Properties

        #region Private Properties

        /// <summary>
        /// Gets a value indicating whether or not the collection allows the
        /// addition of items.
        /// </summary>
        private bool CanAddItems
        {
            get
            {
                return this._canAddItems;
            }

            set
            {
                if (value != this._canAddItems)
                {
                    this._canAddItems = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets a value indicating whether or not the collection allows the
        /// deletion of items.
        /// </summary>
        private bool CanDeleteItems
        {
            get
            {
                return this._canDeleteItems;
            }

            set
            {
                if (value != this._canDeleteItems)
                {
                    this._canDeleteItems = value;
                    this.UpdateButtonsAndStates();
                }
            }
        }

        /// <summary>
        /// Gets the current item as an IChangeTracking.
        /// </summary>
        private IChangeTracking CurrentItemChangeTracking
        {
            get
            {
                return this.CurrentItem as IChangeTracking;
            }
        }

        /// <summary>
        /// Gets the current item as an INotifyPropertyChanged.
        /// </summary>
        private INotifyPropertyChanged CurrentItemNotifyPropertyChanged
        {
            get
            {
                return this.CurrentItem as INotifyPropertyChanged;
            }
        }

        /// <summary>
        /// Gets the generated content from the display template.
        /// </summary>
        private FrameworkElement DisplayTemplateContent
        {
            get
            {
                if (this.DisplayTemplate != null)
                {
                    return this.DisplayTemplate.LoadContent() as FrameworkElement;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the entity collection view as an IEditableCollectionView.
        /// </summary>
        private IEditableCollectionView EditableCollectionView
        {
            get
            {
                return this._collectionView as IEditableCollectionView;
            }
        }

        /// <summary>
        /// Gets the items source as an IEditableCollection.
        /// </summary>
        private IEditableCollection EditableItemsSource
        {
            get
            {
                return this.ItemsSource as IEditableCollection;
            }
        }

        /// <summary>
        /// Gets the generated content from the edit template.
        /// </summary>
        private FrameworkElement EditTemplateContent
        {
            get
            {
                if (this.EditTemplate != null)
                {
                    return this.EditTemplate.LoadContent() as FrameworkElement;
                }

                return null;
            }
        }

        /// <summary>
        /// Gets the generated content from the insert template.
        /// </summary>
        private FrameworkElement InsertTemplateContent
        {
            get
            {
                if (this.InsertTemplate != null)
                {
                    return this.InsertTemplate.LoadContent() as FrameworkElement;
                }

                return null;
            }
        }

        #endregion Private Properties

        #endregion Properties

        #region Methods

        #region Public Methods

        /// <summary>
        /// Adds a new item.
        /// </summary>
        /// <returns>Whether or not a new item was added.</returns>
        public bool AddItem()
        {
            if (this.IsEditing)
            {
                if (this._isUserAbleToAddItemsAfterEdit)
                {
                    this.ForceEndEdit();
                }
                else
                {
                    // Don't end the edit if we won't be able to start an insert operation immediately afterwards.
                    return false;
                }
            }

            if (!this.IsUserAbleToAddItems)
            {
                return false;
            }

            CancelEventArgs e = new CancelEventArgs();
            this.OnAddingItem(e);

            if (e.Cancel)
            {
                return false;
            }

            if (this.EditableCollectionView != null && this.EditableCollectionView.CanAddNew)
            {
                this._lastCurrentPosition = this._collectionView.CurrentPosition;
                this.EditableCollectionView.AddNew();
                this.IsAddingNew = this.EditableCollectionView.IsAddingNew;
                return true;
            }

            return false;
        }

        /// <summary>
        /// Begins the editing of the current item.
        /// </summary>
        /// <returns>Whether or not editing was begun.</returns>
        public bool BeginEdit()
        {
            if (!this.CanBeginEdit)
            {
                return false;
            }

            CancelEventArgs e = new CancelEventArgs();
            this.OnBeginningEdit(e);

            if (e.Cancel)
            {
                return false;
            }

            if (this.EditableCollectionView != null)
            {
                this._isUserAbleToAddItemsAfterEdit = this.IsUserAbleToAddItems;
                this._isUserAbleToDeleteItemsAfterEdit = this.IsUserAbleToDeleteItems;
                this.EditableCollectionView.EditItem(this.CurrentItem);
                this.IsEditing = this.EditableCollectionView.IsEditingItem;
            }
            else
            {
                IEditableObject ieo = this.CurrentItem as IEditableObject;

                if (ieo != null)
                {
                    ieo.BeginEdit();
                }

                this.IsEditing = true;
            }

            if (this.IsEditing)
            {
                this.UpdateButtonsAndStates();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Cancels the editing of the current item.
        /// </summary>
        /// <returns>Whether or not the cancellation was successful.</returns>
        public bool CancelItemEdit()
        {
            if (!this.CanCancelEdit)
            {
                return false;
            }

            DataFormItemEditEndingEventArgs e = new DataFormItemEditEndingEventArgs(DataFormEditAction.Cancel);
            this.OnItemEditEnding(e);

            if (e.Cancel)
            {
                return false;
            }

            if (!this.IsAddingNew)
            {
                if (this.EditableCollectionView != null && this.EditableCollectionView.CanCancelEdit)
                {
                    this.EditableCollectionView.CancelEdit();
                }
                else
                {
                    IEditableObject currentEditableObject = this._lastItem as IEditableObject;

                    if (currentEditableObject != null)
                    {
                        currentEditableObject.CancelEdit();
                    }
                }
            }
            else
            {
                this.CancelAppend();
            }

            this._invalidElements.Clear();
            this._invalidFields.Clear();
            this._entityLevelErrors.Clear();

            if (this._errorSummary != null)
            {
                Debug.Assert(this._errorSummary.Errors != null, "ErrorSummary.Errors should never be null.");
                this._errorSummary.Errors.Clear();
            }

            this.EndEdit();
            this.IsItemValid = true;
            this.UpdateButtonsAndStates();

            this.OnItemEditEnded(new DataFormItemEditEndedEventArgs(DataFormEditAction.Cancel));
            return true;
        }

        /// <summary>
        /// Commits the edit of the property on the current item bound to a field.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <returns>True if the commit succeeds; false otherwise.</returns>
        public bool CommitFieldEdit(DataFormField field)
        {
            if (field == null || field.EffectiveIsReadOnly)
            {
                return false;
            }

            DataFormFieldEditEndingEventArgs e = new DataFormFieldEditEndingEventArgs(field, field.Element, DataFormEditAction.Commit);
            this.OnFieldEditEnding(e);

            if (e.Cancel)
            {
                return false;
            }

            DataFormFieldGroup group = field as DataFormFieldGroup;

            if (group != null)
            {
                foreach (DataFormField childField in group.Fields)
                {
                    this.CommitFieldEdit(childField);
                }
            }
            else
            {
                if (field.BindingExpressions != null)
                {
                    foreach (BindingExpression bindingExpression in field.BindingExpressions)
                    {
                        bindingExpression.UpdateSource();
                    }
                }

                if (field.IsValid && this._invalidFields.Contains(field))
                {
                    this._invalidFields.Remove(field);
                    this.SetIsItemValid();
                }
                else if (!field.IsValid && !this._invalidFields.Contains(field))
                {
                    this._invalidFields.Add(field);
                    this.SetIsItemValid();
                }
            }

            // Validate the form when validating an individual field if there are entity-level errors.
            if (this._entityLevelErrors.Count > 0)
            {
                this.ValidateItem();
            }

            this.OnFieldEditEnded(new DataFormFieldEditEndedEventArgs(field, DataFormEditAction.Commit));
            return field.IsValid;
        }

        /// <summary>
        /// Commits the edit of the current item and exits editing mode.
        /// </summary>
        /// <returns>True if the commit succeeds; false otherwise.</returns>
        public bool CommitItemEdit()
        {
            return this.CommitItemEdit(true /* exitEditingMode */);
        }

        /// <summary>
        /// Commits the edit of the current item.
        /// </summary>
        /// <param name="exitEditingMode">Whether or not editing mode should be exited after committing the edit.</param>
        /// <returns>True if the commit succeeds; false otherwise.</returns>
        public bool CommitItemEdit(bool exitEditingMode)
        {
            if (!this.CanCommitEdit)
            {
                return false;
            }

            DataFormItemEditEndingEventArgs e = new DataFormItemEditEndingEventArgs(DataFormEditAction.Commit);
            this.OnItemEditEnding(e);

            if (e.Cancel)
            {
                return false;
            }

            if (!this.ValidateItem())
            {
                return false;
            }

            if (!this.IsAddingNew)
            {
                if (this.EditableCollectionView != null)
                {
                    this.EditableCollectionView.CommitEdit();
                }
                else
                {
                    IEditableObject currentEditableObject = this._lastItem as IEditableObject;

                    if (currentEditableObject != null)
                    {
                        currentEditableObject.EndEdit();
                    }
                }
            }
            else
            {
                this.CommitAppend();
            }

            this.EndEdit();
            this.UpdateButtonsAndStates();
            this.OnItemEditEnded(new DataFormItemEditEndedEventArgs(DataFormEditAction.Commit));

            if (!exitEditingMode)
            {
                this.BeginEdit();
            }

            return true;
        }

        /// <summary>
        /// Deletes the current item.
        /// </summary>
        /// <returns>Whether or not the item was deleted.</returns>
        public bool DeleteItem()
        {
            if (this.IsEditing)
            {
                if (this._isUserAbleToDeleteItemsAfterEdit)
                {
                    this.ForceEndEdit();
                }
                else
                {
                    // Don't end the edit if we won't be able to start a delete operation immediately afterwards.
                    return false;
                }
            }

            if (!this.IsUserAbleToDeleteItems)
            {
                return false;
            }

            CancelEventArgs e = new CancelEventArgs();
            this.OnDeletingItem(e);

            if (e.Cancel)
            {
                return false;
            }

            if (this.EditableCollectionView != null && this._collectionView.CurrentPosition >= 0 && this.EditableCollectionView.CanRemove)
            {
                this._lastCurrentPosition = this._collectionView.CurrentPosition;
                this.EditableCollectionView.RemoveAt(this._collectionView.CurrentPosition);

                if (this._lastCurrentPosition < this.ItemsCount - 1)
                {
                    this._collectionView.MoveCurrentToPosition(this._lastCurrentPosition);
                }
                else
                {
                    this._collectionView.MoveCurrentToLast();
                }
            }

            return true;
        }

        /// <summary>
        /// Gets the field element for a field.
        /// </summary>
        /// <param name="field">The field to retrieve the element from.</param>
        /// <returns>The element from the field.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Justification = "Making the method static would be a discoverability problem.")]
        public FrameworkElement GetFieldElement(DataFormField field)
        {
            if (field != null)
            {
                return field.Element;
            }

            return null;
        }

        /// <summary>
        /// Applies the template for this control.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            this._fieldsPresenter = GetTemplateChild(DATAFORM_elementFieldsPresenter) as ContentPresenter;

            this._firstItemButton = GetTemplateChild(DATAFORM_elementFirstItemButton) as ButtonBase;

            if (this._firstItemButton != null)
            {
                this._firstItemButton.Click -= new RoutedEventHandler(this.OnMoveToFirstItemButtonClick);
                this._firstItemButton.Click += new RoutedEventHandler(this.OnMoveToFirstItemButtonClick);
            }

            this._errorSummary = GetTemplateChild(DATAFORM_elementErrorSummary) as ErrorSummary;
            this._previousItemButton = GetTemplateChild(DATAFORM_elementPreviousItemButton) as ButtonBase;

            if (this._previousItemButton != null)
            {
                this._previousItemButton.Click -= new RoutedEventHandler(this.OnMoveToPreviousItemButtonClick);
                this._previousItemButton.Click += new RoutedEventHandler(this.OnMoveToPreviousItemButtonClick);
            }

            this._nextItemButton = GetTemplateChild(DATAFORM_elementNextItemButton) as ButtonBase;

            if (this._nextItemButton != null)
            {
                this._nextItemButton.Click -= new RoutedEventHandler(this.OnMoveToNextItemButtonClick);
                this._nextItemButton.Click += new RoutedEventHandler(this.OnMoveToNextItemButtonClick);
            }

            this._lastItemButton = GetTemplateChild(DATAFORM_elementLastItemButton) as ButtonBase;

            if (this._lastItemButton != null)
            {
                this._lastItemButton.Click -= new RoutedEventHandler(this.OnMoveToLastItemButtonClick);
                this._lastItemButton.Click += new RoutedEventHandler(this.OnMoveToLastItemButtonClick);
            }

            this._buttonSeparator = GetTemplateChild(DATAFORM_elementButtonSeparator) as UIElement;

            if (this._buttonSeparator != null)
            {
                this.SetButtonSeparatorVisibility();
            }

            this._newItemButton = GetTemplateChild(DATAFORM_elementNewItemButton) as ButtonBase;

            if (this._newItemButton != null)
            {
                this._newItemButton.Click -= new RoutedEventHandler(this.OnAppendItemButtonClick);
                this._newItemButton.Click += new RoutedEventHandler(this.OnAppendItemButtonClick);

                this.SetNewItemButtonVisibility();
            }

            this._deleteItemButton = GetTemplateChild(DATAFORM_elementDeleteItemButton) as ButtonBase;

            if (this._deleteItemButton != null)
            {
                this._deleteItemButton.Click -= new RoutedEventHandler(this.OnDeleteItemButtonClick);
                this._deleteItemButton.Click += new RoutedEventHandler(this.OnDeleteItemButtonClick);

                this.SetDeleteItemButtonVisibility();
            }

            this._editButton = GetTemplateChild(DATAFORM_elementEditButton) as ButtonBase;

            if (this._editButton != null)
            {
                this._editButton.Click -= new RoutedEventHandler(this.OnBeginEditButtonClick);
                this._editButton.Click += new RoutedEventHandler(this.OnBeginEditButtonClick);

                this.SetEditItemButtonVisibility();
            }

            this._commitButton = GetTemplateChild(DATAFORM_elementCommitButton) as ButtonBase;

            if (this._commitButton != null)
            {
                this._commitButton.Click -= new RoutedEventHandler(this.OnEndEditButtonClick);
                this._commitButton.Click += new RoutedEventHandler(this.OnEndEditButtonClick);

                if (this.CommitButtonContent != null)
                {
                    this._commitButton.Content = this.CommitButtonContent;
                }

                this.SetCommitButtonVisibility();
            }

            this._cancelButton = GetTemplateChild(DATAFORM_elementCancelButton) as ButtonBase;

            if (this._cancelButton != null)
            {
                this._cancelButton.Click -= new RoutedEventHandler(this.OnCancelEditButtonClick);
                this._cancelButton.Click += new RoutedEventHandler(this.OnCancelEditButtonClick);

                if (this.CancelButtonContent != null)
                {
                    this._cancelButton.Content = this.CancelButtonContent;
                }

                this.SetCancelButtonVisibility();
            }

            this._errorSummary = GetTemplateChild(DATAFORM_elementErrorSummary) as ErrorSummary;

            if (this._errorSummary != null)
            {
                this._errorSummary.Style = this.ErrorSummaryStyle;
            }

            this.UpdateCurrentItem();
            this.GenerateUI();
        }

        /// <summary>
        /// Validates the current item.
        /// </summary>
        /// <returns>Whether or not the current item is valid.</returns>
        public bool ValidateItem()
        {
            CancelEventArgs e = new CancelEventArgs();
            this.OnValidatingItem(e);

            if (e.Cancel)
            {
                return this.IsItemValid;
            }

            // Clear entity errors both from the ErrorSummary and the entity errors list.
            this.ClearEntityErrors();

            // Check all input controls and validate them.
            this.CommitAllFields();

            // Check to see if there's any errors at this point, and if so, break out early,
            // since it does not make sense to continue with cross field validation.
            this.SetIsItemValid();

            if (!this.IsItemValid)
            {
                return this.IsItemValid;
            }

            if (this.CurrentItem != null)
            {
                // Check the rest of the parameters on the entity object.
                ValidationContext context = new ValidationContext(this.CurrentItem, null, null);
                List<ValidationResult> validationResults = new List<ValidationResult>();
                bool valid = Validator.TryValidateObject(this.CurrentItem, context, validationResults, true /* validateAllProperties */);

                if (!valid)
                {
                    Debug.Assert(validationResults.Count > 0, "Entity is not valid, but there are no errors.");
                    foreach (ValidationResult result in validationResults)
                    {
                        string errorMessage = result.ErrorMessage;
                        ErrorSummaryItem newError = new ErrorSummaryItem(errorMessage, ErrorType.EntityError);

                        this._entityLevelErrors.Add(newError);

                        if (this.ErrorSummary != null)
                        {
                            Debug.Assert(this.ErrorSummary.Errors != null, "ErrorSummary.Errors should never be null.");

                            if (!this.ErrorSummary.Errors.Contains(newError))
                            {
                                this.ErrorSummary.Errors.Add(newError);
                            }
                        }
                    }

                    this.SetIsItemValid();
                    return false;
                }

                validationResults.Clear();
            }

            this.SetIsItemValid();
            return true;
        }

        #endregion Public Methods

        #region Internal Methods

        /// <summary>
        /// Cancels the appending of a new item.
        /// </summary>
        internal void CancelAppend()
        {
            if (this.EditableCollectionView != null)
            {
                this.IsAddingNew = false;
                this.EditableCollectionView.CancelNew();
                this._collectionView.MoveCurrentToPosition(this._lastCurrentPosition);
                SetAllCanPropertiesAndUpdate(this, false /* onlyUpdateStates */);
            }
        }

        /// <summary>
        /// Commits the appending of a new item.
        /// </summary>
        internal void CommitAppend()
        {
            if (this.EditableCollectionView != null)
            {
                this.IsAddingNew = false;
                this.EditableCollectionView.CommitNew();
                SetAllCanPropertiesAndUpdate(this, false /* onlyUpdateStates */);
            }
        }

        /// <summary>
        /// Forces the ending of the editing of the current item.
        /// </summary>
        internal void ForceEndEdit()
        {
            if (this.IsEditing)
            {
                if (this.AutoCommit)
                {
                    if (!this.CommitItemEdit(true /* exitEditingMode */))
                    {
                        this.CancelItemEdit();
                    }
                }
                else
                {
                    this.CancelItemEdit();
                }
            }
        }

        /// <summary>
        /// Generates the UI from the Fields collection.
        /// </summary>
        /// <param name="mode">The mode to be used.</param>
        /// <returns>The generated UI.</returns>
        internal FrameworkElement GeneratePanelUI(DataFormMode mode)
        {
            this._rootField.OwningForm = this;
            this._rootField.Orientation = this.Orientation;
            this._rootField.WrapAfter = this.WrapAfter;

            this.Labels.Clear();
            this.InputControls.Clear();
            this.Descriptions.Clear();
            this._elementFieldLabelDictionary.Clear();
            this._elementDescriptionViewerDictionary.Clear();
            this.ResetNextTabIndex();

            return this.GeneratePanelUI(mode, this._rootField.Fields, this._rootField.Orientation, this._rootField.WrapAfter);
        }

        /// <summary>
        /// Fills the items panel with all of the field containers holding the generated items from the fields collection.
        /// </summary>
        /// <param name="mode">The mode to be used.</param>
        /// <param name="fields">The list of fields.</param>
        /// <param name="orientation">The orientation in which to lay the fields out.</param>
        /// <param name="wrapAfter">The number of items to wrap after.</param>
        /// <returns>The generated UI.</returns>
        internal FrameworkElement GeneratePanelUI(DataFormMode mode, IList<DataFormField> fields, Orientation orientation, int wrapAfter)
        {
            Grid grid = new Grid();

            if (fields.Count <= 0)
            {
                return grid;
            }

            // Not wrapping is the same as wrapping after the number of fields.
            // Similarly, wrapping after more than the number of fields is also
            // the same as wrapping after the number of fields.
            // This is necessary to make the following loop work.
            if (wrapAfter <= 0 || wrapAfter > fields.Count)
            {
                wrapAfter = fields.Count;
            }

            int numRowsOrColumns = (int)Math.Ceiling((double)fields.Count / wrapAfter);

            grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(YEdgeSpacing, GridUnitType.Pixel) });
            grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(XEdgeSpacing, GridUnitType.Pixel) });

            for (int i = 0; i < numRowsOrColumns; i++)
            {
                if (orientation == Orientation.Horizontal)
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

                    if (i < numRowsOrColumns - 1)
                    {
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(FieldElementSpacing, GridUnitType.Pixel) });
                    }
                    else
                    {
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(YEdgeSpacing, GridUnitType.Pixel) });
                    }
                }
                else
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(FieldElementSpacing, GridUnitType.Pixel) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(FieldElementSpacing, GridUnitType.Pixel) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                    if (i < numRowsOrColumns - 1)
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(FieldElementSpacing, GridUnitType.Pixel) });
                    }
                    else
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(XEdgeSpacing, GridUnitType.Pixel) });
                    }
                }
            }

            for (int i = 0; i < wrapAfter; i++)
            {
                if (orientation == Orientation.Horizontal)
                {
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(FieldElementSpacing, GridUnitType.Pixel) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition());
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(FieldElementSpacing, GridUnitType.Pixel) });
                    grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });

                    if (i < wrapAfter - 1)
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(FieldElementSpacing, GridUnitType.Pixel) });
                    }
                    else
                    {
                        grid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(XEdgeSpacing, GridUnitType.Pixel) });
                    }
                }
                else
                {
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                    grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });

                    if (i < wrapAfter - 1)
                    {
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(FieldElementSpacing, GridUnitType.Pixel) });
                    }
                    else
                    {
                        grid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(YEdgeSpacing, GridUnitType.Pixel) });
                    }
                }
            }

            for (int i = 0; i < numRowsOrColumns; i++)
            {
                int numColumnsOrRows = Math.Min((i + 1) * wrapAfter, fields.Count);

                bool areLeftLabels = false;

                for (int j = i * wrapAfter; j < numColumnsOrRows; j++)
                {
                    DataFormLabeledField labeledField = fields[j] as DataFormLabeledField;

                    if (labeledField != null)
                    {
                        if (labeledField.OwningForm != null)
                        {
                            labeledField.ParentDescriptionViewerPosition = labeledField.OwningForm.DescriptionViewerPosition;
                            labeledField.ParentFieldLabelPosition = labeledField.OwningForm.FieldLabelPosition;
                        }

                        if (labeledField.EffectiveFieldLabelPosition == DataFormFieldLabelPosition.Auto ||
                            labeledField.EffectiveFieldLabelPosition == DataFormFieldLabelPosition.Left)
                        {
                            areLeftLabels = true;
                        }
                    }
                }

                for (int j = i * wrapAfter; j < numColumnsOrRows; j++)
                {
                    DataFormFieldGroup childGroupField = fields[j] as DataFormFieldGroup;

                    if (childGroupField != null)
                    {
                        childGroupField.Mode = mode;
                    }

                    FieldLabel label = null;
                    FrameworkElement element = null;
                    DescriptionViewer description = null;

                    int xPosition = orientation == Orientation.Horizontal ? j % wrapAfter : i;
                    int yPosition = orientation == Orientation.Vertical ? j % wrapAfter : i;
                    bool isLastXPosition =
                        orientation == Orientation.Vertical ||
                        j == Math.Min(i + wrapAfter, fields.Count) - 1;

                    this.PlaceFieldInGrid(
                        fields[j],
                        out label,
                        out element,
                        out description,
                        grid,
                        xPosition,
                        yPosition,
                        isLastXPosition,
                        areLeftLabels,
                        mode);

                    fields[j].OwningForm.Labels.Add(label);
                    fields[j].OwningForm.InputControls.Add(element);
                    fields[j].OwningForm.Descriptions.Add(description);
                }
            }

            return grid;
        }

        /// <summary>
        /// Generates the UI for this control.
        /// </summary>
        internal void GenerateUI()
        {
            this.UIGenerated = false;

            if (this._fieldsPresenter == null)
            {
                return;
            }

            this._invalidElements.Clear();
            this._invalidFields.Clear();
            this._entityLevelErrors.Clear();

            if (this.ErrorSummary != null)
            {
                Debug.Assert(this.ErrorSummary.Errors != null, "ErrorSummary.Errors should never be null.");
                this.ErrorSummary.Errors.Clear();
            }

            this.SetIsItemValid();
            this.UpdateButtonsAndStates();

            this._templateBindingExpressions = null;

            if (!this.EffectiveIsReadOnly && this._isEditing)
            {
                if (this._isAddingNew)
                {
                    if (this.InsertTemplate != null && this.InsertTemplateContent != null)
                    {
                        this.PrepareTemplateContent(this.InsertTemplateContent);
                    }
                    else if (this.EditTemplate != null && this.EditTemplateContent != null)
                    {
                        this.PrepareTemplateContent(this.EditTemplateContent);
                    }
                    else
                    {
                        this._fieldsPresenter.Content = this.GeneratePanelUI(this.Mode);
                    }
                }
                else
                {
                    if (this.EditTemplate != null && this.EditTemplateContent != null)
                    {
                        this.PrepareTemplateContent(this.EditTemplateContent);
                    }
                    else if (this.InsertTemplate != null && this.InsertTemplateContent != null)
                    {
                        this.PrepareTemplateContent(this.InsertTemplateContent);
                    }
                    else
                    {
                        this._fieldsPresenter.Content = this.GeneratePanelUI(this.Mode);
                    }
                }
            }
            else
            {
                if (this.DisplayTemplate != null && this.DisplayTemplateContent != null)
                {
                    this._fieldsPresenter.Content = this.DisplayTemplateContent;
                }
                else if (this.EditTemplate != null && this.EditTemplateContent != null)
                {
                    this.PrepareTemplateContent(this.EditTemplateContent);
                }
                else if (this.InsertTemplate != null && this.InsertTemplateContent != null)
                {
                    this.PrepareTemplateContent(this.InsertTemplateContent);
                }
                else
                {
                    this._fieldsPresenter.Content = this.GeneratePanelUI(DataFormMode.Display);
                }
            }

            FrameworkElement rootElement = this._fieldsPresenter.Content as FrameworkElement;

            if (rootElement != null)
            {
                rootElement.Loaded += new RoutedEventHandler(this.OnRootElementLoaded);
            }

            this.UIGenerated = true;
            this.UpdateCurrentItem();
        }

        /// <summary>
        /// Gets the next tab index for generated controls.
        /// </summary>
        /// <returns>The next tab index.</returns>
        internal int GetNextTabIndex()
        {
            this._nextTabIndex++;
            return this._nextTabIndex;
        }

        /// <summary>
        /// Goes to the first item.
        /// </summary>
        internal void MoveToFirstItem()
        {
            if (this._collectionView != null && this.CanMoveToFirstItem)
            {
                this._collectionView.MoveCurrentToFirst();
            }
        }

        /// <summary>
        /// Goes to the last item.
        /// </summary>
        internal void MoveToLastItem()
        {
            if (this._collectionView != null && this.CanMoveToLastItem)
            {
                this._collectionView.MoveCurrentToLast();
            }
        }

        /// <summary>
        /// Goes to the next item.
        /// </summary>
        internal void MoveToNextItem()
        {
            if (this._collectionView != null && this.CanMoveToNextItem)
            {
                this._collectionView.MoveCurrentToNext();
            }
        }

        /// <summary>
        /// Goes to the previous item.
        /// </summary>
        internal void MoveToPreviousItem()
        {
            if (this._collectionView != null && this.CanMoveToPreviousItem)
            {
                this._collectionView.MoveCurrentToPrevious();
            }
        }

        /// <summary>
        /// Updates the enabled state of the buttons within the DataForm.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity", Justification = "There is no need to make the method less complex.")]
        internal void UpdateButtons()
        {
            if (this._newItemButton != null)
            {
                this._newItemButton.IsEnabled = this.IsUserAbleToAddItems && this.IsEnabled;
                this.SetNewItemButtonVisibility();
            }

            if (this._deleteItemButton != null)
            {
                this._deleteItemButton.IsEnabled = this.IsUserAbleToDeleteItems && this.IsEnabled;
                this.SetDeleteItemButtonVisibility();
            }

            if (this._editButton != null)
            {
                this._editButton.IsEnabled = this.CanBeginEdit && this.IsEnabled;
            }

            if (this._cancelButton != null)
            {
                this._cancelButton.IsEnabled = this.CanCancelEdit && this.IsEnabled;
                this.SetCancelButtonVisibility();
            }

            if (this._commitButton != null)
            {
                this._commitButton.IsEnabled = this.CanCommitEdit && this.IsEnabled;
                this.SetCommitButtonVisibility();
            }

            if (this._firstItemButton != null)
            {
                this._firstItemButton.IsEnabled = this.CanMoveToFirstItem && this.IsEnabled;
            }

            if (this._lastItemButton != null)
            {
                this._lastItemButton.IsEnabled = this.CanMoveToLastItem && this.IsEnabled;
            }

            if (this._nextItemButton != null)
            {
                this._nextItemButton.IsEnabled = this.CanMoveToNextItem && this.IsEnabled;
            }

            if (this._previousItemButton != null)
            {
                this._previousItemButton.IsEnabled = this.CanMoveToPreviousItem && this.IsEnabled;
            }
        }

        /// <summary>
        /// Finds any bindings on an element and updates the ones in which Mode is TwoWay
        /// to set the two Boolean properties to true and to set UpdateSourceTrigger
        /// to Explicit.  Used both for TemplateFields and for template mode.
        /// </summary>
        /// <param name="element">The element.</param>
        internal void UpdateBindingsOnElement(FrameworkElement element)
        {
            if (this.CurrentItem == null)
            {
                return;
            }

            List<DependencyProperty> dependencyProperties = ValidationUtil.GetDependencyPropertiesForElement(element);
            Debug.Assert(dependencyProperties != null, "GetDependencyPropertiesForElement() should never return null.");

            foreach (DependencyProperty dependencyProperty in dependencyProperties)
            {
                if (dependencyProperty != null)
                {
                    BindingExpression bindingExpression = element.GetBindingExpression(dependencyProperty);

                    if (bindingExpression != null && bindingExpression.ParentBinding != null)
                    {
                        Binding binding = ValidationUtil.CopyBinding(bindingExpression.ParentBinding);

                        if (binding.Path != null && !String.IsNullOrEmpty(binding.Path.Path) && binding.Mode == BindingMode.TwoWay)
                        {
                            PropertyInfo propertyInfo = this.CurrentItemType.GetPropertyInfo(binding.Path.Path);

                            if (propertyInfo == null || !propertyInfo.CanWrite)
                            {
                                // Ignore bindings to nonexistent or read-only properties.
                                continue;
                            }

                            binding.ValidatesOnExceptions = true;
                            binding.NotifyOnValidationError = true;
                            binding.UpdateSourceTrigger = UpdateSourceTrigger.Explicit;
                        }

                        if (binding.Converter == null)
                        {
                            binding.Converter = new DataFormValueConverter();
                        }

                        element.SetBinding(dependencyProperty, binding);

                        element.LostFocus -= new RoutedEventHandler(this.OnElementLostFocus);
                        element.LostFocus += new RoutedEventHandler(this.OnElementLostFocus);

                        TextBox textBox = element as TextBox;

                        if (textBox != null)
                        {
                            textBox.TextChanged -= new TextChangedEventHandler(this.OnTextBoxTextChanged);
                            textBox.TextChanged += new TextChangedEventHandler(this.OnTextBoxTextChanged);
                        }
                    }
                }
            }

            int childrenCount = VisualTreeHelper.GetChildrenCount(element);

            for (int i = 0; i < childrenCount; i++)
            {
                FrameworkElement childElement = VisualTreeHelper.GetChild(element, i) as FrameworkElement;

                // Stop if we've found a child DataForm.
                if (childElement != null && childElement.GetType() != typeof(DataForm))
                {
                    this.UpdateBindingsOnElement(childElement);
                }
            }
        }

        /// <summary>
        /// Updates the enabled state of the buttons and the visual states within the DataForm.
        /// </summary>
        internal void UpdateButtonsAndStates()
        {
            this.UpdateButtons();
            this.UpdateStates();
        }

        /// <summary>
        /// Updates whether or not the given element is valid.
        /// </summary>
        /// <param name="element">The element to update.</param>
        internal void UpdateElementValidity(FrameworkElement element)
        {
            ValidationUtil.UpdateSourceOnElementBindings(element);

            if (this._entityLevelErrors.Count > 0)
            {
                this.ValidateItem();
            }

            if (!ValidationUtil.ElementHasErrors(element) && this._invalidElements.Contains(element))
            {
                this._invalidElements.Remove(element);
                this.SetIsItemValid();
            }
            else if (ValidationUtil.ElementHasErrors(element) && !this._invalidElements.Contains(element))
            {
                this._invalidElements.Add(element);
                this.SetIsItemValid();
            }

            // Validate the form when validating an individual field if there are entity-level errors.
            if (this._entityLevelErrors.Count > 0)
            {
                this.ValidateItem();
            }
        }

        /// <summary>
        /// Updates the visual states within the DataForm.
        /// </summary>
        internal void UpdateStates()
        {
            if (this.IsEnabled)
            {
                VisualStateManager.GoToState(this, DATAFORM_stateDisabled, true);
            }
            else if (this.EffectiveIsReadOnly)
            {
                VisualStateManager.GoToState(this, DATAFORM_stateReadOnly, true);
            }
            else
            {
                VisualStateManager.GoToState(this, DATAFORM_stateNormal, true);
            }

            if (this.IsAddingNew)
            {
                if (!VisualStateManager.GoToState(this, DATAFORM_stateInsert, true))
                {
                    if (!VisualStateManager.GoToState(this, DATAFORM_stateEdit, true))
                    {
                        VisualStateManager.GoToState(this, DATAFORM_stateDisplay, true);
                    }
                }
            }
            else if (this.IsEditing)
            {
                if (!VisualStateManager.GoToState(this, DATAFORM_stateEdit, true))
                {
                    VisualStateManager.GoToState(this, DATAFORM_stateDisplay, true);
                }
            }
            else if (this.IsEmpty)
            {
                if (!VisualStateManager.GoToState(this, DATAFORM_stateEmpty, true))
                {
                    VisualStateManager.GoToState(this, DATAFORM_stateDisplay, true);
                }
            }
            else
            {
                VisualStateManager.GoToState(this, DATAFORM_stateDisplay, true);
            }

            if (this.IsItemValid)
            {
                VisualStateManager.GoToState(this, DATAFORM_stateValid, true);
            }
            else
            {
                VisualStateManager.GoToState(this, DATAFORM_stateInvalid, true);
            }

            if (this.IsItemChanged)
            {
                VisualStateManager.GoToState(this, DATAFORM_stateUncommitted, true);
            }
            else
            {
                VisualStateManager.GoToState(this, DATAFORM_stateCommitted, true);
            }

            if (this.ItemsSource == null)
            {
                VisualStateManager.GoToState(this, DATAFORM_stateEntity, true);
            }
            else
            {
                VisualStateManager.GoToState(this, DATAFORM_stateCollection, true);
            }
        }

        #endregion Internal Methods

        #region Protected Methods

        /// <summary>
        /// Fires the AddingItem event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnAddingItem(CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> handler = this.AddingItem;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the AutoGeneratedFields event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnAutoGeneratedFields(EventArgs e)
        {
            EventHandler<EventArgs> handler = this.AutoGeneratedFields;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the AutoGeneratingField event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnAutoGeneratingField(DataFormAutoGeneratingFieldEventArgs e)
        {
            EventHandler<DataFormAutoGeneratingFieldEventArgs> handler = this.AutoGeneratingField;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the BeginningEdit event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnBeginningEdit(CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> handler = this.BeginningEdit;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the ContentLoaded event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnContentLoaded(DataFormContentLoadedEventArgs e)
        {
            EventHandler<DataFormContentLoadedEventArgs> handler = this.ContentLoaded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Returns an automation peer for this DataForm.
        /// </summary>
        /// <returns>The automation peer.</returns>
        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return base.OnCreateAutomationPeer();
        }

        /// <summary>
        /// Fires the CurrentItemChanged event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnCurrentItemChanged(EventArgs e)
        {
            EventHandler<EventArgs> handler = this.CurrentItemChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the DeletingItem event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnDeletingItem(CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> handler = this.DeletingItem;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the FieldEditEnded event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnFieldEditEnded(DataFormFieldEditEndedEventArgs e)
        {
            EventHandler<DataFormFieldEditEndedEventArgs> handler = this.FieldEditEnded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the FieldEditEnding event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnFieldEditEnding(DataFormFieldEditEndingEventArgs e)
        {
            EventHandler<DataFormFieldEditEndingEventArgs> handler = this.FieldEditEnding;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the ItemEditEnded event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnItemEditEnded(DataFormItemEditEndedEventArgs e)
        {
            EventHandler<DataFormItemEditEndedEventArgs> handler = this.ItemEditEnded;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the ItemEditEnding event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnItemEditEnding(DataFormItemEditEndingEventArgs e)
        {
            EventHandler<DataFormItemEditEndingEventArgs> handler = this.ItemEditEnding;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        /// <summary>
        /// Fires the ValidatingItem event.
        /// </summary>
        /// <param name="e">The event args.</param>
        protected virtual void OnValidatingItem(CancelEventArgs e)
        {
            EventHandler<CancelEventArgs> handler = this.ValidatingItem;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        #endregion Protected Methods

        #region Private Static Methods

        /// <summary>
        /// Returns a DataFormField for a type.
        /// </summary>
        /// <param name="type">The type for which to generate the DataFormField.</param>
        /// <returns>The DataFormField.</returns>
        private static DataFormBoundField GetDataFormFieldFromType(Type type)
        {
            Debug.Assert(type != null, "The type must not be null.");

            if (type == typeof(bool))
            {
                return new DataFormCheckBoxField();
            }
            if (type == typeof(bool?))
            {
                DataFormCheckBoxField field = new DataFormCheckBoxField();
                field.IsThreeState = true;
                return field;
            }
            else if (type == typeof(DateTime) || type == typeof(DateTime?))
            {
                return new DataFormDateField();
            }
            else
            {
                return new DataFormTextField();
            }
        }

        /// <summary>
        /// Returns whether or not a framework element is keyboard focusable.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns>Whether or not the element is keyboard focusable.</returns>
        private static bool IsKeyboardFocusable(FrameworkElement element)
        {
            if (element == null)
            {
                return false;
            }

            FrameworkElementAutomationPeer elementPeer = FrameworkElementAutomationPeer.CreatePeerForElement(element) as FrameworkElementAutomationPeer;

            if (elementPeer != null)
            {
                return elementPeer.IsKeyboardFocusable();
            }

            return false;
        }

        /// <summary>
        /// Places the description into the grid.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="element">The element.</param>
        /// <param name="description">The description.</param>
        /// <param name="grid">The grid to be used.</param>
        /// <param name="labelGrid">The grid to be used for the label.</param>
        private static void PlaceDescriptionInGrid(DataFormField field, FrameworkElement element, DescriptionViewer description, Grid grid, Grid labelGrid)
        {
            DataFormLabeledField labeledField = field as DataFormLabeledField;

            if (labeledField != null && description != null && element != null)
            {
                Panel oldPanel = VisualTreeHelper.GetParent(description) as Panel;

                if (oldPanel != null)
                {
                    oldPanel.Children.Remove(description);
                }

                switch (labeledField.EffectiveDescriptionViewerPosition)
                {
                    case DataFormDescriptionViewerPosition.Auto:
                    case DataFormDescriptionViewerPosition.BesideContent:
                        Grid.SetColumn(description, Grid.GetColumn(element) + Grid.GetColumnSpan(element) + 1);
                        Grid.SetRow(description, Grid.GetRow(element));
                        break;

                    case DataFormDescriptionViewerPosition.BesideLabel:
                        // When the description position is BesideLabel, insert the description
                        // into the grid for the label instead of into the outer grid.
                        labelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(FieldElementSpacing, GridUnitType.Pixel) });
                        labelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                        Grid.SetColumn(description, 2);
                        labelGrid.Children.Add(description);
                        break;

                    default:
                        Debug.Assert(false, "Unexpected description position.");
                        break;
                }

                if (labeledField.EffectiveDescriptionViewerPosition != DataFormDescriptionViewerPosition.BesideLabel)
                {
                    grid.Children.Add(description);
                }
            }
        }

        /// <summary>
        /// Places the label into the grid.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="label">The label.</param>
        /// <param name="element">The element.</param>
        /// <param name="grid">The grid to be used.</param>
        /// <param name="labelGrid">The grid to be used for the label.</param>
        /// <param name="yPosition">The y-position in the grid at which to insert the field.</param>
        private static void PlaceLabelInGrid(DataFormField field, FieldLabel label, FrameworkElement element, Grid grid, Grid labelGrid, int yPosition)
        {
            DataFormLabeledField labeledField = field as DataFormLabeledField;

            if (labeledField != null && label != null && element != null)
            {
                switch (labeledField.EffectiveFieldLabelPosition)
                {
                    case DataFormFieldLabelPosition.Auto:
                    case DataFormFieldLabelPosition.Left:
                        Grid.SetColumn(labelGrid, Grid.GetColumn(element) - 2);
                        Grid.SetRow(labelGrid, Grid.GetRow(element));
                        break;

                    case DataFormFieldLabelPosition.Top:
                        Grid.SetColumn(labelGrid, Grid.GetColumn(element));
                        Grid.SetRow(labelGrid, (yPosition * RowsPerField) + TopYPosition);
                        Grid.SetColumnSpan(labelGrid, Grid.GetColumnSpan(element));
                        break;

                    default:
                        Debug.Assert(false, "Unexpected label position.");
                        break;
                }

                grid.Children.Add(labelGrid);
            }
        }

        /// <summary>
        /// Sets focus to the first focusable control (with respect to TabIndex) within the hierarchy of a given dependency object.
        /// </summary>
        /// <param name="dependencyObject">The dependency object.</param>
        private static void SetFocusToFirstFocusableControl(DependencyObject dependencyObject)
        {
            if (dependencyObject == null)
            {
                return;
            }

            int lowestTabIndex = int.MaxValue;
            Control controlWithLowestTabIndex = null;

            Stack<DependencyObject> dependencyObjectStack = new Stack<DependencyObject>();
            dependencyObjectStack.Push(dependencyObject);

            while (dependencyObjectStack.Count > 0 && lowestTabIndex > 0)
            {
                DependencyObject currentDependencyObject = dependencyObjectStack.Pop();

                Control control = currentDependencyObject as Control;

                if (control != null && control.TabIndex < lowestTabIndex && IsKeyboardFocusable(control))
                {
                    lowestTabIndex = control.TabIndex;
                    controlWithLowestTabIndex = control;
                }
                else
                {
                    int numChildren = VisualTreeHelper.GetChildrenCount(currentDependencyObject);

                    for (int i = 0; i < numChildren; i++)
                    {
                        DependencyObject childDependencyObject = VisualTreeHelper.GetChild(currentDependencyObject, i);
                        dependencyObjectStack.Push(childDependencyObject);
                    }
                }
            }

            if (controlWithLowestTabIndex != null)
            {
                controlWithLowestTabIndex.Focus();
            }
        }

        /// <summary>
        /// Sorts the generated field paths in GenerateFields() according to the order specified
        /// in the DisplayAttributes.
        /// </summary>
        /// <param name="generatedFieldPaths">The generated field paths.</param>
        /// <param name="orders">The orders.</param>
        /// <returns>The sorted list of generated field paths.</returns>
        private static IList<string> SortGeneratedFieldPaths(IList<string> generatedFieldPaths, IDictionary<string, int> orders)
        {
            IList<PathOrderPair> pathOrderPairList = new List<PathOrderPair>();

            foreach (string path in generatedFieldPaths)
            {
                PathOrderPair pathOrderPair = new PathOrderPair();
                pathOrderPair.Path = path;
                pathOrderPair.Order = orders[path];
                pathOrderPairList.Add(pathOrderPair);
            }

            pathOrderPairList = new List<PathOrderPair>(pathOrderPairList.OrderBy<PathOrderPair, int>(PathOrderPair.GetOrder));

            IList<string> sortedGeneratedFieldPaths = new List<string>();

            foreach (PathOrderPair pathOrderPair in pathOrderPairList)
            {
                sortedGeneratedFieldPaths.Add(pathOrderPair.Path);
            }

            return sortedGeneratedFieldPaths;
        }

        /// <summary>
        /// Calls SetAllCanProperties() and either UpdateStates() or UpdateButtonsAndStates()
        /// on the given DataForm.
        /// </summary>
        /// <param name="dataForm">The DataForm.</param>
        /// <param name="onlyUpdateStates">Whether to call UpdateStates() or UpdateButtonsAndStates()</param>
        private static void SetAllCanPropertiesAndUpdate(DataForm dataForm, bool onlyUpdateStates)
        {
            if (dataForm != null)
            {
                dataForm.SetAllCanProperties();

                if (onlyUpdateStates)
                {
                    dataForm.UpdateStates();
                }
                else
                {
                    dataForm.UpdateButtonsAndStates();
                }
            }
        }

        #endregion Private Static Methods

        #region Private Methods

        /// <summary>
        /// Clears all EntityErrors before ValidateForm is performed.
        /// </summary>
        private void ClearEntityErrors()
        {
            if (this.ErrorSummary != null)
            {
                Debug.Assert(this.ErrorSummary.Errors != null, "Unexpected null ErrorSummary.Errors collection");
                Debug.Assert(this._entityLevelErrors != null, "Unexpected null value for _entityLevelErrors");
                List<ErrorSummaryItem> entityErrors = new List<ErrorSummaryItem>();

                foreach (ErrorSummaryItem esi in this.ErrorSummary.Errors)
                {
                    Debug.Assert(esi != null, "Unexpected null ErrorSummaryItem");
                    if (esi.ErrorType == ErrorType.EntityError)
                    {
                        entityErrors.Add(esi);
                    }
                }

                foreach (ErrorSummaryItem esi in entityErrors)
                {
                    this.ErrorSummary.Errors.Remove(esi);
                }
            }

            this._entityLevelErrors.Clear();
        }

        /// <summary>
        /// Commits all the fields.
        /// </summary>
        private void CommitAllFields()
        {
            if (this._templateBindingExpressions != null)
            {
                foreach (BindingExpression bindingExpression in this._templateBindingExpressions)
                {
                    bindingExpression.UpdateSource();
                }
            }
            else
            {
                Debug.Assert(this._rootField != null, "The root field should never be null.");
                this.CommitFieldEdit(this._rootField);
            }
        }

        /// <summary>
        /// Moves out of edit mode.
        /// </summary>
        private void EndEdit()
        {
             this._isUserAbleToAddItemsAfterEdit = false;
             this._isUserAbleToDeleteItemsAfterEdit = false;
             this.IsEditing = false;
        }

        /// <summary>
        /// Generates the DataFormFields from the properties with [Bindable(true)] applied to them.
        /// </summary>
        private void GenerateFields()
        {
            this.RemoveGeneratedFields();

            if (!this.AutoGenerateFields || this.CurrentItemType == null)
            {
                return;
            }

            // If it's a primitive type, generate a binding to the current item itself
            // instead of a binding to its properties.
            if (TypeHelper.TypeIsPrimitive(this.CurrentItemType))
            {
                this.GenerateField(this.CurrentItemType, string.Empty, BindingDirection.TwoWay, null);
                return;
            }

            PropertyInfo[] propertyInfos = this.CurrentItemType.GetProperties();

            if (propertyInfos == null)
            {
                return;
            }

            BindableAttribute currentItemBindableAttribute =
                this.CurrentItemType.GetCustomAttributes(typeof(BindableAttribute), true /* inherit */)
                .Cast<BindableAttribute>()
                .FirstOrDefault();

            if (currentItemBindableAttribute != null && !currentItemBindableAttribute.Bindable)
            {
                // If the current item is flagged as not bindable, do nothing.
                return;
            }

            IList<string> generatedFieldPathsNoGroup = new List<string>();
            IDictionary<string, BindingDirection> bindingDirections = new Dictionary<string, BindingDirection>();
            IDictionary<string, int> orders = new Dictionary<string, int>();

            foreach (PropertyInfo propertyInfo in propertyInfos)
            {
                if (propertyInfo.GetIndexParameters().Length > 0)
                {
                    // Don't generate anything for indexed properties.
                    continue;
                }

                BindableAttribute bindableAttribute =
                    propertyInfo.GetCustomAttributes(typeof(BindableAttribute), true /* inherit */)
                    .Cast<BindableAttribute>()
                    .FirstOrDefault();

                DisplayAttribute displayAttribute =
                    propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true /* inherit */)
                    .Cast<DisplayAttribute>()
                    .FirstOrDefault();

                bool propertyGenerated = false;

                if (bindableAttribute != null)
                {
                    if (bindableAttribute.Bindable)
                    {
                        bindingDirections.Add(propertyInfo.Name, bindableAttribute.Direction);
                        propertyGenerated = true;
                    }
                }
                else
                {
                    if (currentItemBindableAttribute != null)
                    {
                        bindingDirections.Add(propertyInfo.Name, currentItemBindableAttribute.Direction);
                    }
                    else
                    {
                        if (propertyInfo.CanWrite)
                        {
                            bindingDirections.Add(propertyInfo.Name, BindingDirection.TwoWay);
                        }
                        else
                        {
                            bindingDirections.Add(propertyInfo.Name, BindingDirection.OneWay);
                        }
                    }

                    propertyGenerated = true;
                }

                if (propertyGenerated)
                {
                    if (displayAttribute != null)
                    {
                        generatedFieldPathsNoGroup.Add(propertyInfo.Name);
                        orders.Add(propertyInfo.Name, displayAttribute.Order);
                    }
                    else
                    {
                        generatedFieldPathsNoGroup.Add(propertyInfo.Name);
                        orders.Add(propertyInfo.Name, 0);
                    }
                }
            }

            generatedFieldPathsNoGroup = SortGeneratedFieldPaths(generatedFieldPathsNoGroup, orders);

            // If we have at least one grouped field, put the non-grouped fields into a
            // group as well with no header.
            DataFormFieldGroup groupNonGroupedFields = null;

            Debug.Assert(generatedFieldPathsNoGroup != null, "The ungrouped generated field path list should never be null.");

            foreach (string generatedFieldPath in generatedFieldPathsNoGroup)
            {
                PropertyInfo propertyInfo = this.CurrentItemType.GetProperty(generatedFieldPath);
                Debug.Assert(propertyInfo != null, "Property info should never be null.");
                this.GenerateField(propertyInfo.PropertyType, propertyInfo.Name, bindingDirections[generatedFieldPath], groupNonGroupedFields);
            }

            this.OnAutoGeneratedFields(EventArgs.Empty);
        }

        /// <summary>
        /// Generates a field based on a property type and a property name.
        /// </summary>
        /// <param name="propertyType">The type of the property.</param>
        /// <param name="propertyName">The name of the property.</param>
        /// <param name="bindingDirection">The binding direction.</param>
        /// <param name="group">The group to insert the field into (field added to DataForm.Fields if null).</param>
        /// <returns>A value indicating whether or not the field was generated.</returns>
        private bool GenerateField(Type propertyType, string propertyName, BindingDirection bindingDirection, DataFormFieldGroup group)
        {
            // Create a new DataFormBoundColumn for the property.
            DataFormBoundField newField = GetDataFormFieldFromType(propertyType);

            newField.IsAutoGenerated = true;

            if (bindingDirection == BindingDirection.TwoWay && !string.IsNullOrEmpty(propertyName))
            {
                newField.Binding = new Binding(propertyName) { Mode = BindingMode.TwoWay, ValidatesOnExceptions = true, NotifyOnValidationError = true, UpdateSourceTrigger = UpdateSourceTrigger.Explicit };
            }
            else
            {
                newField.Binding = new Binding(propertyName) { Mode = BindingMode.OneWay };
                newField.IsReadOnly = true;
            }

            // Raise the AutoGeneratingField event in case the user wants to cancel or replace the
            // field being generated.
            DataFormAutoGeneratingFieldEventArgs e = new DataFormAutoGeneratingFieldEventArgs(propertyName, propertyType, newField);
            this.OnAutoGeneratingField(e);

            if (e.Cancel)
            {
                return false;
            }
            else
            {
                this._ignoreFieldsCollectionChange = true;

                if (group != null)
                {
                    group.Fields.Add(e.Field);
                }
                else
                {
                    Debug.Assert(this.Fields != null, "The fields collection should never be null.");
                    this.Fields.Add(e.Field);
                }

                return true;
            }
        }

        /// <summary>
        /// Returns whether or not the specified type of command button is visible.
        /// </summary>
        /// <param name="commandButtonVisibility">The type of command button.</param>
        /// <returns>Whether or not the type of command button is visible.</returns>
        private bool IsCommandButtonVisibile(DataFormCommandButtonsVisibility commandButtonVisibility)
        {
            return (this.CommandButtonsVisibility & commandButtonVisibility) == commandButtonVisibility;
        }

        /// <summary>
        /// Handles the situation where the collection changed on the ICollectionView.
        /// </summary>
        /// <param name="sender">The ICollectionView.</param>
        /// <param name="e">The event args.</param>
        private void OnCollectionViewCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            this.SetAllCanProperties();
        }

        /// <summary>
        /// Handles the situation where the current item changed on the ICollectionView.
        /// </summary>
        /// <param name="sender">The ICollectionView.</param>
        /// <param name="e">The event args.</param>
        private void OnCollectionViewCurrentChanged(object sender, EventArgs e)
        {
            this.UpdateCurrentItem();
            this.SetAllCanProperties();
        }

        /// <summary>
        /// Handles the situation where the current item is changing on the ICollectionView.
        /// </summary>
        /// <param name="sender">The ICollectionView.</param>
        /// <param name="e">The event args.</param>
        private void OnCollectionViewCurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            if (e.IsCancelable && ((!this.AutoCommit && this.IsEditing) || !this.IsItemValid))
            {
                e.Cancel = true;
            }
        }

        /// <summary>
        /// Handles the situation where a property on the current item changes.
        /// </summary>
        /// <param name="sender">The current item.</param>
        /// <param name="e">The event args.</param>
        private void OnCurrentItemPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsChanged":
                    if (this.CurrentItemChangeTracking != null)
                    {
                        this.IsItemChanged = this.CurrentItemChangeTracking.IsChanged;
                    }
                    break;
            }
        }

        /// <summary>
        /// Handles the situation where the fields collection has changed.
        /// </summary>
        /// <param name="sender">The fields collection.</param>
        /// <param name="e">The event args.</param>
        private void OnFieldsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                    for (int i = 0; i < e.NewItems.Count; i++)
                    {
                        DataFormField field = e.NewItems[i] as DataFormField;

                        if (field != null)
                        {
                            this._rootField.Fields.Add(field);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Remove:
                    for (int i = 0; i < e.OldItems.Count; i++)
                    {
                        DataFormField field = e.OldItems[i] as DataFormField;

                        if (field != null)
                        {
                            this._rootField.Fields.Remove(field);
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Replace:
                    for (int i = 0; i < Math.Min(e.OldItems.Count, e.NewItems.Count); i++)
                    {
                        DataFormField oldField = e.OldItems[i] as DataFormField;
                        DataFormField newField = e.NewItems[i] as DataFormField;

                        if (oldField != null && newField != null)
                        {
                            int index = this._rootField.Fields.IndexOf(oldField);

                            if (index > -1)
                            {
                                this._rootField.Fields[index] = newField;
                            }
                        }
                    }

                    break;

                case NotifyCollectionChangedAction.Reset:
                    this._rootField.Fields.Clear();

                    foreach (DataFormField field in this.Fields)
                    {
                        this._rootField.Fields.Add(field);
                    }

                    break;
            }

            if (this._ignoreFieldsCollectionChange)
            {
                this._ignoreFieldsCollectionChange = false;
            }
            else
            {
                this.GenerateUI();
            }
        }

        /// <summary>
        /// Handles the situation where the collection of validation errors changes.
        /// </summary>
        /// <param name="sender">The ValidationErrorCollection.</param>
        /// <param name="e">The event args.</param>
        private void OnValidationErrorsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            ValidationErrorCollection vec = sender as ValidationErrorCollection;

            if (vec != null)
            {
                this.IsItemValid = vec.Count == 0;
            }

            this.UpdateButtonsAndStates();
        }

        /// <summary>
        /// Places the element into the grid.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="label">The label.</param>
        /// <param name="element">The element.</param>
        /// <param name="description">The description.</param>
        /// <param name="grid">The grid to be used.</param>
        /// <param name="xPosition">The x-position in the grid at which to insert the field.</param>
        /// <param name="yPosition">The y-position in the grid at which to insert the field.</param>
        /// <param name="isLastXPosition">Whether or not this is the rightmost x-position.</param>
        /// <param name="areLeftLabels">Whether or not there exist other fields in this group with labels at the left.</param>
        private void PlaceElementInGrid(DataFormField field, FieldLabel label, FrameworkElement element, DescriptionViewer description, Grid grid, int xPosition, int yPosition, bool isLastXPosition, bool areLeftLabels)
        {
            if (element != null)
            {
                element.VerticalAlignment = VerticalAlignment.Center;

                if (field is DataFormLabeledField)
                {
                    if (!areLeftLabels)
                    {
                        Grid.SetColumn(element, (xPosition * ColumnsPerField) + LeftXPosition);
                        Grid.SetColumnSpan(element, ColumnSpanTwoItems);
                    }
                    else
                    {
                        Grid.SetColumn(element, (xPosition * ColumnsPerField) + MiddleXPosition);
                    }
                }
                else
                {
                    Grid.SetColumn(element, xPosition * ColumnsPerField);

                    // The last item in a row needs to take up the extra space at the right side of the space.
                    Grid.SetColumnSpan(element, isLastXPosition ? ColumnsPerField + 1 : ColumnsPerField);
                }

                Grid.SetRow(element, (yPosition * RowsPerField) + MiddleYPosition);

                if (label != null || description != null)
                {
                    if (label != null)
                    {
                        if (this._elementFieldLabelDictionary.ContainsKey(element))
                        {
                            this._elementFieldLabelDictionary[element] = label;
                        }
                        else
                        {
                            this._elementFieldLabelDictionary.Add(element, label);
                        }
                    }

                    if (description != null)
                    {
                        if (this._elementDescriptionViewerDictionary.ContainsKey(element))
                        {
                            this._elementDescriptionViewerDictionary[element] = description;
                        }
                        else
                        {
                            this._elementDescriptionViewerDictionary.Add(element, description);
                        }
                    }

                    RoutedEventHandler handler = new RoutedEventHandler(this.OnElementLoaded);
                    element.Loaded -= handler;
                    element.Loaded += handler;
                }

                Panel oldPanel = VisualTreeHelper.GetParent(element) as Panel;

                if (oldPanel != null)
                {
                    oldPanel.Children.Remove(element);
                }

                grid.Children.Add(element);
            }
        }

        /// <summary>
        /// Places the field into the grid.
        /// </summary>
        /// <param name="field">The field.</param>
        /// <param name="label">The resulting label.</param>
        /// <param name="element">The resulting element.</param>
        /// <param name="description">The resulting description.</param>
        /// <param name="grid">The grid to be used.</param>
        /// <param name="xPosition">The x-position in the grid at which to insert the field.</param>
        /// <param name="yPosition">The y-position in the grid at which to insert the field.</param>
        /// <param name="isLastXPosition">Whether or not this is the rightmost x-position.</param>
        /// <param name="areLeftLabels">Whether or not there exist other fields in this group with labels at the left.</param>
        /// <param name="mode">The mode to be used.</param>
        private void PlaceFieldInGrid(DataFormField field, out FieldLabel label, out FrameworkElement element, out DescriptionViewer description, Grid grid, int xPosition, int yPosition, bool isLastXPosition, bool areLeftLabels, DataFormMode mode)
        {
            if (mode != field.Mode)
            {
                field.ShouldGenerateNewElement = true;
                field.Mode = mode;
            }

            Grid labelGrid = null;
            label = null;
            element = null;
            description = null;
            DataFormLabeledField labeledField = field as DataFormLabeledField;

            if (labeledField != null)
            {
                label = labeledField.GenerateLabelInternal();

                if (this.Mode == DataFormMode.Display)
                {
                    label.IsRequired = false;
                }
                else
                {
                    label.Refresh();

                    if (labeledField.FieldLabelContent != null)
                    {
                        label.Content = labeledField.FieldLabelContent;
                    }
                }

                Panel oldPanel = VisualTreeHelper.GetParent(label) as Panel;

                if (oldPanel != null)
                {
                    oldPanel.Children.Remove(label);
                }

                // We put the label into a Grid in order to simplify the case where the description position
                // is set to be BesideLabel.
                labelGrid = new Grid() { VerticalAlignment = VerticalAlignment.Center };
                labelGrid.RowDefinitions.Add(new RowDefinition() { Height = new GridLength(1, GridUnitType.Auto) });
                labelGrid.ColumnDefinitions.Add(new ColumnDefinition() { Width = new GridLength(1, GridUnitType.Auto) });
                labelGrid.Children.Add(label);
                labelGrid.HorizontalAlignment = label.HorizontalAlignment;
            }

            switch (mode)
            {
                case DataFormMode.Display:
                    element = field.GenerateElementInternal();
                    break;

                case DataFormMode.Edit:
                    element = field.GenerateEditingElementInternal();
                    break;

                case DataFormMode.Insert:
                    element = field.GenerateInsertElementInternal();
                    break;

                default:
                    Debug.Assert(false, "Unexpected mode.");
                    break;
            }

            if (labeledField != null)
            {
                description = labeledField.GenerateDescriptionInternal();
            }

            this.PlaceElementInGrid(field, label, element, description, grid, xPosition, yPosition, isLastXPosition, areLeftLabels);
            PlaceLabelInGrid(field, label, element, grid, labelGrid, yPosition);
            PlaceDescriptionInGrid(field, element, description, grid, labelGrid);
        }

        /// <summary>
        /// Prepares and applies the given template content.
        /// </summary>
        /// <param name="templateContent">The template content.</param>
        private void PrepareTemplateContent(FrameworkElement templateContent)
        {
            this._fieldsPresenter.Content = templateContent;
            this.UpdateBindingsOnElement(templateContent);
            this._templateBindingExpressions = ValidationUtil.GetBindingExpressionsForElement(templateContent);
        }

        /// <summary>
        /// Removes fields generated by GeneratedFields().
        /// </summary>
        private void RemoveGeneratedFields()
        {
            for (int i = this.Fields.Count - 1; i >= 0; i--)
            {
                if (this.Fields[i].IsAutoGenerated)
                {
                    this._ignoreFieldsCollectionChange = true;
                    this.Fields.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Resets the next tab index.
        /// </summary>
        private void ResetNextTabIndex()
        {
            this._nextTabIndex = -1;
        }

        /// <summary>
        /// Sets all of the properties of the form "CanXXXX".
        /// </summary>
        private void SetAllCanProperties()
        {
            this.SetCanAddItems();
            this.SetCanBeginEdit();
            this.SetCanCancelEdit();
            this.SetCanDeleteItems();
            this.SetCanCommitEdit();
            this.SetCanMoveToFirstItem();
            this.SetCanMoveToLastItem();
            this.SetCanMoveToNextItem();
            this.SetCanMoveToPreviousItem();
        }

        /// <summary>
        /// Sets the visibility of the button separator.
        /// </summary>
        private void SetButtonSeparatorVisibility()
        {
            Debug.Assert(this._buttonSeparator != null, "The button separator should never be null when this method is called.");

            if (this.CommandButtonsVisibility != DataFormCommandButtonsVisibility.None && this.ItemsSource != null)
            {
                this._buttonSeparator.Visibility = Visibility.Visible;
            }
            else
            {
                this._buttonSeparator.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets whether or not the user can add a new item.
        /// </summary>
        private void SetCanAddItems()
        {
            this.CanAddItems =
                this.EditableCollectionView != null &&
                (this.EditableCollectionView.CanAddNew || (this.IsEditing && this._isUserAbleToAddItemsAfterEdit)) &&
                !this.EffectiveIsReadOnly &&
                (!this.IsEditing || this.AutoCommit) &&
                !this.IsAddingNew;
        }

        /// <summary>
        /// Sets whether or not the user can begin an edit.
        /// </summary>
        private void SetCanBeginEdit()
        {
            this.CanBeginEdit =
                (this.EditableItemsSource == null || this.EditableItemsSource.CanEdit) &&
                !this.IsEditing &&
                !this.EffectiveIsReadOnly &&
                this.CurrentItem != null;
        }

        /// <summary>
        /// Sets whether or not the user can cancel an edit.
        /// </summary>
        private void SetCanCancelEdit()
        {
            this.CanCancelEdit =
                ((this.EditableCollectionView != null && this.EditableCollectionView.CanCancelEdit) ||
                (this._lastItem is IEditableObject)) &&
                (this.IsAddingNew ||
                this.IsEditing);
        }

        /// <summary>
        /// Sets the visibility of the cancel button.
        /// </summary>
        private void SetCancelButtonVisibility()
        {
            Debug.Assert(this._cancelButton != null, "The cancel button should never be null when this method is called.");

            if (this.Mode != DataFormMode.Display &&
                ((this.EditableCollectionView != null && this.EditableCollectionView.CanCancelEdit) ||
                this.CurrentItem is IEditableObject) &&
                this.IsCommandButtonVisibile(DataFormCommandButtonsVisibility.Edit))
            {
                this._cancelButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._cancelButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets whether or not the user can delete an item.
        /// </summary>
        private void SetCanDeleteItems()
        {
            this.CanDeleteItems =
                this._collectionView != null &&
                this.EditableCollectionView != null &&
                !this._collectionView.IsCurrentBeforeFirst &&
                !this._collectionView.IsCurrentAfterLast &&
                (this.EditableCollectionView.CanRemove || (this.IsEditing && this._isUserAbleToDeleteItemsAfterEdit)) &&
                !this.EffectiveIsReadOnly &&
                (!this.IsEditing || this.AutoCommit) &&
                !this.IsAddingNew;
        }

        /// <summary>
        /// Sets whether or not the user can commit an edit.
        /// </summary>
        private void SetCanCommitEdit()
        {
            this.CanCommitEdit =
                this.IsEditing &&
                this.IsItemValid;
        }

        /// <summary>
        /// Sets whether or not the user can move to the first item.
        /// </summary>
        private void SetCanMoveToFirstItem()
        {
            this.CanMoveToFirstItem =
                this._collectionView != null &&
                this._collectionView.CurrentPosition > 0 &&
                (!this.IsEditing || this.AutoCommit) &&
                !this.IsAddingNew &&
                this.IsItemValid;
        }

        /// <summary>
        /// Sets whether or not the user can move to the last item.
        /// </summary>
        private void SetCanMoveToLastItem()
        {
            this.CanMoveToLastItem =
                this._collectionView != null &&
                this._collectionView.CurrentPosition < this.ItemsCount - 1 &&
                (!this.IsEditing || this.AutoCommit) &&
                !this.IsAddingNew &&
                this.IsItemValid;
        }

        /// <summary>
        /// Sets whether or not the user can move to the next item.
        /// </summary>
        private void SetCanMoveToNextItem()
        {
            this.CanMoveToNextItem =
                this._collectionView != null &&
                this._collectionView.CurrentPosition < this.ItemsCount - 1 &&
                (!this.IsEditing || this.AutoCommit) &&
                !this.IsAddingNew &&
                this.IsItemValid;
        }

        /// <summary>
        /// Sets whether or not the user can move to the previous item.
        /// </summary>
        private void SetCanMoveToPreviousItem()
        {
            this.CanMoveToPreviousItem =
                this._collectionView != null &&
                this._collectionView.CurrentPosition > 0 &&
                (!this.IsEditing || this.AutoCommit) &&
                !this.IsAddingNew &&
                this.IsItemValid;
        }

        /// <summary>
        /// Sets the visibility of the commit button.
        /// </summary>
        private void SetCommitButtonVisibility()
        {
            Debug.Assert(this._commitButton != null, "The commit button should never be null when this method is called.");

            if (this.Mode != DataFormMode.Display &&
                this.IsCommandButtonVisibile(DataFormCommandButtonsVisibility.Edit))
            {
                this._commitButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._commitButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets the visibility of the delete item button.
        /// </summary>
        private void SetDeleteItemButtonVisibility()
        {
            Debug.Assert(this._deleteItemButton != null, "The button separator should never be null when this method is called.");

            if (this.IsUserAbleToDeleteItems &&
                this.IsCommandButtonVisibile(DataFormCommandButtonsVisibility.Delete))
            {
                this._deleteItemButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._deleteItemButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets the visibility of the delete item button.
        /// </summary>
        private void SetEditItemButtonVisibility()
        {
            Debug.Assert(this._editButton != null, "The button separator should never be null when this method is called.");

            if (this.IsCommandButtonVisibile(DataFormCommandButtonsVisibility.Edit))
            {
                this._editButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._editButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Sets whether or not the item is valid.
        /// </summary>
        private void SetIsItemValid()
        {
            this.IsItemValid =
                this._invalidFields.Count == 0 &&
                this._invalidElements.Count == 0 &&
                this._entityLevelErrors.Count == 0;
        }

        /// <summary>
        /// Sets the visibility of the new item button.
        /// </summary>
        private void SetNewItemButtonVisibility()
        {
            Debug.Assert(this._newItemButton != null, "The button separator should never be null when this method is called.");

            if (this.IsUserAbleToAddItems &&
                this.IsCommandButtonVisibile(DataFormCommandButtonsVisibility.Add))
            {
                this._newItemButton.Visibility = Visibility.Visible;
            }
            else
            {
                this._newItemButton.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// Updates the current item.
        /// </summary>
        private void UpdateCurrentItem()
        {
            if (this._collectionView != null)
            {
                this.CurrentItem = this._collectionView.CurrentItem;
                this.CurrentIndex = this._collectionView.CurrentPosition;
            }
            else
            {
                this.CurrentIndex = this.CurrentItem != null ? 0 : -1;
            }

            this.IsEmpty = this.CurrentItem == null;

            if (this.CurrentItemChangeTracking != null)
            {
                this.IsItemChanged = this.CurrentItemChangeTracking.IsChanged;
            }

            if (this._fieldsPresenter != null && this._fieldsPresenter.Content != null)
            {
                FrameworkElement fieldsPresenterContent = this._fieldsPresenter.Content as FrameworkElement;

                if (fieldsPresenterContent != null)
                {
                    fieldsPresenterContent.DataContext = this.CurrentItem;
                    if (this._errorSummary != null)
                    {
                        this._errorSummary.ErrorsSource = fieldsPresenterContent;
                    }
                }
            }

            this.UpdateButtonsAndStates();
        }

        /// <summary>
        /// Handles the click of the "append item" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnAppendItemButtonClick(object sender, RoutedEventArgs e)
        {
            this.AddItem();
        }

        /// <summary>
        /// Handles the click of the "begin edit" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnBeginEditButtonClick(object sender, RoutedEventArgs e)
        {
            this.BeginEdit();
        }

        /// <summary>
        /// Handles the click of the "cancel edit" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnCancelEditButtonClick(object sender, RoutedEventArgs e)
        {
            this.CancelItemEdit();
        }

        /// <summary>
        /// Handles the click of the "delete item" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnDeleteItemButtonClick(object sender, RoutedEventArgs e)
        {
            this.DeleteItem();
        }

        /// <summary>
        /// Handles the click of the "end edit" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnEndEditButtonClick(object sender, RoutedEventArgs e)
        {
            this.CommitItemEdit(true /* exitEditingMode */);
        }

        /// <summary>
        /// Handles the click of the "move to first item" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnMoveToFirstItemButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveToFirstItem();
        }

        /// <summary>
        /// Handles the click of the "move to last item" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnMoveToLastItemButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveToLastItem();
        }

        /// <summary>
        /// Handles the click of the "move to previous item" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnMoveToPreviousItemButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveToPreviousItem();
        }

        /// <summary>
        /// Handles the click of the "move to next item" button.
        /// </summary>
        /// <param name="sender">The button.</param>
        /// <param name="e">The event args.</param>
        private void OnMoveToNextItemButtonClick(object sender, RoutedEventArgs e)
        {
            this.MoveToNextItem();
        }

        /// <summary>
        /// Handles the case where an element has loaded.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnElementLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            element.Loaded -= new RoutedEventHandler(this.OnElementLoaded);

            FieldLabel fieldLabel = null;
            DescriptionViewer descriptionViewer = null;

            if (this._elementFieldLabelDictionary.ContainsKey(element))
            {
                fieldLabel = this._elementFieldLabelDictionary[element];
            }

            if (this._elementDescriptionViewerDictionary.ContainsKey(element))
            {
                descriptionViewer = this._elementDescriptionViewerDictionary[element];
            }

            if (fieldLabel != null)
            {
                fieldLabel.Target = element;
            }

            if (descriptionViewer != null)
            {
                descriptionViewer.Target = element;
            }

            this._elementFieldLabelDictionary.Remove(element);
            this._elementDescriptionViewerDictionary.Remove(element);
        }

        /// <summary>
        /// Handles the case where an element lost focus.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnElementLostFocus(object sender, RoutedEventArgs e)
        {
            FrameworkElement element = sender as FrameworkElement;

            if (element != null)
            {
                this.UpdateElementValidity(element);
            }
        }

        /// <summary>
        /// Handles the case where the root element in the fields presenter has loaded.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnRootElementLoaded(object sender, RoutedEventArgs e)
        {
            FrameworkElement rootElement = sender as FrameworkElement;
            rootElement.Loaded -= new RoutedEventHandler(this.OnRootElementLoaded);

            if (this.Mode != DataFormMode.Display)
            {
                SetFocusToFirstFocusableControl(rootElement);
            }

            this.OnContentLoaded(new DataFormContentLoadedEventArgs(this._fieldsPresenter.Content as FrameworkElement));
        }

        /// <summary>
        /// Handles the case where a text box's text changed.
        /// </summary>
        /// <param name="sender">The element.</param>
        /// <param name="e">The event args.</param>
        private void OnTextBoxTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (textBox != null && ValidationUtil.ElementHasErrors(textBox))
            {
                this.UpdateElementValidity(textBox);
            }
        }

        #endregion Private Methods

        #endregion Methods
    }
}
