// (c) Copyright Microsoft Corporation.
// This source is subject to the Microsoft Public License (Ms-PL).
// Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
// All other rights reserved.

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace System.Windows.Controls.Data.Design.Silverlight
{
    /// <summary>
    /// This class contains helper functions that are too cumbersome to write in a late bound fashion and require 
    /// SL specific types
    /// </summary>
    internal class SilverlightHelpers
    {
        private const int DefaultColumnDisplayOrder = 10000;

        internal static IEnumerable<SilverlightColumnInfo> GetColumnGenerationInfo(object dataSource)
        {
            List<SilverlightColumnInfo> columnGenerationInfos = new List<SilverlightColumnInfo>();
            PropertyInfo[] dataProperties = GetItemsSourceProperties(dataSource);
            if (dataProperties != null && dataProperties.Length > 0)
            {
                List<KeyValuePair<int, SilverlightColumnInfo>> columnOrderPairs = new List<KeyValuePair<int, SilverlightColumnInfo>>();

                // Generate the columns
                foreach (PropertyInfo propertyInfo in dataProperties)
                {
                    string columnHeader = null;
                    int columnOrder = DefaultColumnDisplayOrder;

                    // If Browsable exists in SL, consider filtering out the BrowsableAttribute.No properties here

                    // Check if DisplayAttribute is defined on the property
                    object[] attributes = propertyInfo.GetCustomAttributes(typeof(DisplayAttribute), true);
                    if (attributes != null && attributes.Length > 0)
                    {
                        DisplayAttribute displayAttribute = attributes[0] as DisplayAttribute;
                        Debug.Assert(displayAttribute != null);

                        // We want to return all properties even if they are marked AutoGenerateField == false
                        // because in the Designer, we're not generating the columns.  We're allowing the user
                        // to choose columns based on properties

                        string header = displayAttribute.GetShortName();
                        if (header != null)
                        {
                            columnHeader = header;
                        }

                        int? order = displayAttribute.GetOrder();
                        if (order.HasValue)
                        {
                            columnOrder = order.Value;
                        }
                    }

                    // Generate a single column and determine its relative order
                    int insertIndex = 0;
                    if (columnOrder == int.MaxValue)
                    {
                        insertIndex = columnOrderPairs.Count;
                    }
                    else
                    {
                        foreach (KeyValuePair<int, SilverlightColumnInfo> columnOrderPair in columnOrderPairs)
                        {
                            if (columnOrderPair.Key > columnOrder)
                            {
                                break;
                            }
                            insertIndex++;
                        }
                    }

                    SilverlightColumnInfo columnGenerationInfo = new SilverlightColumnInfo(columnHeader, propertyInfo);

                    columnOrderPairs.Insert(insertIndex, new KeyValuePair<int, SilverlightColumnInfo>(columnOrder, columnGenerationInfo));
                }

                // Add the ColumnGenerationInfo's in the correct order and set IsReadOnly
                foreach (KeyValuePair<int, SilverlightColumnInfo> columnOrderPair in columnOrderPairs)
                {
                    columnGenerationInfos.Add(columnOrderPair.Value);

                }
            }
            return columnGenerationInfos;
        }

        private static Type GetEnumerableItemType(Type enumerableType)
        {
            Type type = FindGenericType(typeof(IEnumerable<>), enumerableType);
            if (type != null)
            {
                return type.GetGenericArguments()[0];
            }
            return enumerableType;
        }

        /// <summary>
        ///     Get the properties on the ItemSource or if the ItemsSource is an IEnumerable then the properties on 
        ///     the items in the IEnumerable. Lifted from the DataGrid runtime
        /// </summary>
        private static PropertyInfo[] GetItemsSourceProperties(object dataSource)
        {
            if (dataSource == null)
            {
                return new PropertyInfo[] { };
            }
            Type itemType = dataSource.GetType();

            //We need to get the actual source collection rather than the IList exposed by CollectionViewSource and ICollectionView
            //in order to get the type of the collection when it is a generic collection - IList<T> etc.
            CollectionViewSource cvs = dataSource as CollectionViewSource;
            if (cvs != null)
            {
                dataSource = cvs.Source;
            }
            else
            {
                ICollectionView cv = dataSource as ICollectionView;
                if (cv != null)
                {
                    dataSource = cv.SourceCollection;
                }
            }

            IEnumerable list = dataSource as IEnumerable;
            if (list != null)
            {
                itemType = GetItemType(list);
            }

            return itemType.GetProperties(BindingFlags.Public | BindingFlags.Instance);
        }

        private static Type GetItemType(System.Collections.IEnumerable list)
        {
            Type enumerableType = list.GetType();
            Type enumerableItemType = null;
            if (IsEnumerableType(enumerableType))
            {
                enumerableItemType = GetEnumerableItemType(enumerableType);
            }
            if ((enumerableItemType == null) || (enumerableItemType == typeof(object)))
            {
                IEnumerator enumerator = list.GetEnumerator();
                if (enumerator.MoveNext() && (enumerator.Current != null))
                {
                    return enumerator.Current.GetType();
                }
            }
            return enumerableItemType;
        }

        /// <summary>
        ///     Initialize a DataGridColumn created in the collection editor
        /// </summary>
        /// <param name="gridColumn"></param>
        internal static void IntializeDataGridColumn(object gridColumn)
        {
            DataGridColumn slGridColumn = gridColumn as DataGridColumn;
            if (slGridColumn != null)
            {
                DataGridTemplateColumn slTemplateGridColumn = slGridColumn as DataGridTemplateColumn;

                if (slTemplateGridColumn != null)
                {
                    slTemplateGridColumn.CellTemplate = new DataTemplate();
                    slTemplateGridColumn.CellEditingTemplate = new DataTemplate();
                }
            }
        }

        private static bool IsEnumerableType(Type enumerableType)
        {
            return (FindGenericType(typeof(System.Collections.Generic.IEnumerable<>), enumerableType) != null);
        }

        private static Type FindGenericType(Type definition, Type type)
        {
            while ((type != null) && (type != typeof(object)))
            {
                if (type.IsGenericType && (type.GetGenericTypeDefinition() == definition))
                {
                    return type;
                }
                if (definition.IsInterface)
                {
                    foreach (Type type2 in type.GetInterfaces())
                    {
                        Type type3 = FindGenericType(definition, type2);
                        if (type3 != null)
                        {
                            return type3;
                        }
                    }
                }
                type = type.BaseType;
            }
            return null;
        }
    }
}
