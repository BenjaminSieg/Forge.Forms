﻿using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Bindables;
using Forge.Forms.Annotations;
using Forge.Forms.DynamicExpressions;
using Humanizer;

namespace Forge.Forms.Collections.Controls
{
    [TemplatePart(Name = "PART_CrudEditButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_DataGrid", Type = typeof(Grid))]
    public class CrudControl : ContentControl
    {
        private const string ButtonCrudKey = "ButtonCrud";

        static CrudControl()
        {
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary
            {
                Source = new Uri("/Forge.Forms.Collections;component/Themes/Generic.xaml", UriKind.Relative)
            });
            ButtonCrudTemplate = Application.Current.TryFindResource(ButtonCrudKey) as DataTemplate;
        }

        private static DataTemplate ButtonCrudTemplate { get; }

        [DependencyProperty(Options = FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
            OnPropertyChanged = nameof(ItemsSourceChanged))]
        public object ItemsSource { get; set; }

        private CrudCollection Collection => ItemsSource as CrudCollection;

        private DataGrid DataGrid { get; } = new DataGrid{Name = "PART_DataGrid" };

        public ICommand AddItemCommand { get; set; }

        public ICommand EditItemCommand { get; set; }

        public ICommand RemoveItemCommand { get; set; }

        public CrudControl()
        {
            DefaultCommands();
            InitializeDataGrid();
            Content = DataGrid;
        }

        private void InitializeDataGrid()
        {
            DataGrid.AutoGenerateColumns = true;
            DataGrid.AutoGeneratingColumn += DataGridOnAutoGeneratingColumn;
            DataGrid.AutoGeneratedColumns += DataGridOnAutoGeneratedColumns;
            DataGrid.SetBinding(ItemsControl.ItemsSourceProperty, new Binding("ItemsSource")
            {
                Source = this
            });
        }

        private childItem FindVisualChild<childItem>(DependencyObject obj)
            where childItem : DependencyObject
        {
            for (int i = 0; i < VisualTreeHelper.GetChildrenCount(obj); i++)
            {
                DependencyObject child = VisualTreeHelper.GetChild(obj, i);
                if (child != null && child is childItem)
                    return (childItem)child;
                else
                {
                    childItem childOfChild = FindVisualChild<childItem>(child);
                    if (childOfChild != null)
                        return childOfChild;
                }
            }
            return null;
        }

        private void DataGridOnAutoGeneratedColumns(object sender, EventArgs eventArgs)
        {


            var dataGridTemplateColumn = new DataGridTemplateColumn
            {
                CellTemplate = ButtonCrudTemplate,
                IsReadOnly = false
            };
            DataGrid.Columns.Add(dataGridTemplateColumn);

            ApplyTemplate();
            DataGrid.ApplyTemplate();

            var xxxx11= Template.FindName("PART_CrudEditButton", DataGrid) as Button;
            var xxxxa11= Template.FindName("PART_DataGrid", this) as Grid;
        }

        private void DefaultCommands()
        {
            AddItemCommand = new RelayCommand(_ =>
            {
                var newItem = Activator.CreateInstance(Collection.First().GetType());
                Show.Dialog().For(newItem);
            });

            EditItemCommand = new RelayCommand(_ => { Show.Dialog().For(_); });

            RemoveItemCommand = new RelayCommand(_ =>
            {
                //TODO: Implement
            });
        }

        private static void DataGridOnAutoGeneratingColumn(object o, DataGridAutoGeneratingColumnEventArgs e)
        {
            if (e.PropertyDescriptor is PropertyDescriptor descriptor)
            {
                SetDatagridHeader(e, descriptor);
            }
        }

        private static void SetDatagridHeader(DataGridAutoGeneratingColumnEventArgs e,
            MemberDescriptor propertyDescriptor)
        {
            var fieldAttribute = propertyDescriptor.Attributes.OfType<FieldAttribute>().FirstOrDefault();
            if (fieldAttribute != null)
            {
                e.Column.Header = fieldAttribute.Name;
                if (fieldAttribute.IsVisible is bool b)
                    e.Column.Visibility = b ? Visibility.Visible : Visibility.Collapsed;
            }

            e.Column.Header = (e.Column.Header as string ?? propertyDescriptor.Name).Humanize();
        }

        private static void ItemsSourceChanged(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            if (!(obj is CrudControl crudControl) || !(crudControl.ItemsSource is IEnumerable enumerable) ||
                crudControl.ItemsSource is CrudCollection)
            {
                return;
            }

            var itemsList = new CrudCollection(enumerable.OfType<object>());
            crudControl.ItemsSource = itemsList;
        }
    }
}
