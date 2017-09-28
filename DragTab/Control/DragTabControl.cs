using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Data;

namespace DragTab.Control
{
    [TemplatePart(Name = HeaderItemsControlPartName, Type = typeof(DragItemsControl))]
    [TemplatePart(Name = ItemsHolderPartName, Type = typeof(Panel))]
    public class DragTabControl : TabControl
    {
        /// <summary>
        /// Template part.
        /// </summary>
        public const string HeaderItemsControlPartName = "PART_HeaderItemsControl";
        /// <summary>
        /// Template part.
        /// </summary>
        public const string ItemsHolderPartName = "PART_ItemsHolder";

        public static readonly DependencyProperty HeaderMemberPathProperty = DependencyProperty.Register(
            "HeaderMemberPath", typeof(string), typeof(DragTabControl), new PropertyMetadata(default(string)));

        public string HeaderMemberPath
        {
            get { return (string)GetValue(HeaderMemberPathProperty); }
            set { SetValue(HeaderMemberPathProperty, value); }
        }
        public static readonly DependencyProperty HeaderItemTemplateProperty = DependencyProperty.Register(
            "HeaderItemTemplate", typeof(DataTemplate), typeof(DragTabControl), new PropertyMetadata(default(DataTemplate)));

        public DataTemplate HeaderItemTemplate
        {
            get { return (DataTemplate)GetValue(HeaderItemTemplateProperty); }
            set { SetValue(HeaderItemTemplateProperty, value); }
        }
        private Panel _itemsHolder;
        private WeakReference _previousSelection;
        private DragItemsControl _dragablzItemsControl;
        public override void OnApplyTemplate()
        {
            _dragablzItemsControl = GetTemplateChild(HeaderItemsControlPartName) as DragItemsControl;

            if (_dragablzItemsControl != null)
            {
                _dragablzItemsControl.ContainerCustomisations = new ContainerCustomisations(null, PrepareChildContainerForItemOverride);
            }

            _itemsHolder = GetTemplateChild(ItemsHolderPartName) as Panel;


            if (SelectedItem == null)
                SetCurrentValue(SelectedItemProperty, Items.OfType<object>().FirstOrDefault());
            UpdateSelectedItem();
            base.OnApplyTemplate();
        }

        protected override void OnSelectionChanged(SelectionChangedEventArgs e)
        {
            if (e.RemovedItems.Count > 0 && e.AddedItems.Count > 0)
                _previousSelection = new WeakReference(e.RemovedItems[0]);
            base.OnSelectionChanged(e);
            UpdateSelectedItem();
            if (_dragablzItemsControl == null) return;
            Func<IList, IEnumerable<DragTabItem>> notTabItems =
                l =>
                    l.Cast<object>()
                        .Where(o => !(o is TabItem))
                        .Select(o => _dragablzItemsControl.ItemContainerGenerator.ContainerFromItem(o))
                        .OfType<DragTabItem>();
            foreach (var addedItem in notTabItems(e.AddedItems))
            {
                addedItem.IsSelected = true;
                addedItem.BringIntoView();
            }
            foreach (var removedItem in notTabItems(e.RemovedItems))
            {
                removedItem.IsSelected = false;
            }

            foreach (var tabItem in e.AddedItems.OfType<TabItem>().Select(t => _dragablzItemsControl.ItemContainerGenerator.ContainerFromItem(t)).OfType<DragTabItem>())
            {
                tabItem.IsSelected = true;
                tabItem.BringIntoView();
            }
            foreach (var tabItem in e.RemovedItems.OfType<TabItem>().Select(t => _dragablzItemsControl.ItemContainerGenerator.ContainerFromItem(t)).OfType<DragTabItem>())
            {
                tabItem.IsSelected = false;
            }
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnItemsChanged(e);
            if (_itemsHolder == null)
            {
                return;
            }
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Reset:
                    _itemsHolder.Children.Clear();

                    if (Items.Count > 0)
                    {
                        SelectedItem = base.Items[0];
                        UpdateSelectedItem();
                    }
                    break;

                case NotifyCollectionChangedAction.Add:
                    foreach (var item in Items)
                    {
                        ((TabItem)item).IsSelected = false;
                    }
                    ((TabItem)e.NewItems[0]).IsSelected = true;
                    SelectedItem = e.NewItems[0];
                    UpdateSelectedItem();
                    break;

                case NotifyCollectionChangedAction.Remove:
                    if (SelectedItem == null)
                        RestorePreviousSelection();
                    foreach (var item in Items)
                    {
                        ((TabItem)item).IsSelected = false;
                    }
                    if (Items.Count > 1 && _previousSelection?.Target != null)
                    {
                        ((TabItem)_previousSelection.Target).IsSelected = true;
                        SelectedItem = _previousSelection.Target;
                        UpdateSelectedItem();
                    }
                    //Console.WriteLine("Remove");
                    break;
                case NotifyCollectionChangedAction.Move:
                    //Console.WriteLine("Move");
                    break;
                case NotifyCollectionChangedAction.Replace:
                    throw new NotImplementedException("Replace not implemented yet");
            }
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new FrameworkElementAutomationPeer(this);
        }
        private void PrepareChildContainerForItemOverride(DependencyObject dependencyObject, object o)
        {
            var dragablzItem = dependencyObject as DragTabItem;
            if (dragablzItem != null && HeaderMemberPath != null)
            {
                var contentBinding = new Binding(HeaderMemberPath) { Source = o };
                dragablzItem.SetBinding(ContentControl.ContentProperty, contentBinding);
                //dragablzItem.UnderlyingContent = o;
            }
        }

        private void UpdateSelectedItem()
        {
            if (_itemsHolder == null)
            {
                return;
            }

            CreateChildContentPresenter(SelectedItem);

            // show the right child
            var selectedContent = GetContent(SelectedItem);
            foreach (ContentPresenter child in _itemsHolder.Children)
            {
                var isSelected = (child.Content == selectedContent);
                child.Visibility = isSelected ? Visibility.Visible : Visibility.Collapsed;
                child.IsEnabled = isSelected;
            }
        }

        private static object GetContent(object item)
        {
            return (item is TabItem) ? ((TabItem)item).Content : item;
        }

        private void CreateChildContentPresenter(object item)
        {
            if (item == null) return;

            var cp = FindChildContentPresenter(item);
            if (cp != null) return;

            // the actual child to be added.  cp.Tag is a reference to the TabItem
            cp = new ContentPresenter
            {
                Content = GetContent(item),
                ContentTemplate = ContentTemplate,
                ContentTemplateSelector = ContentTemplateSelector,
                ContentStringFormat = ContentStringFormat,
                Visibility = Visibility.Collapsed,
            };
            _itemsHolder.Children.Add(cp);
        }

        /// <summary>
        /// Find the CP for the given object.  data could be a TabItem or a piece of data
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        private ContentPresenter FindChildContentPresenter(object data)
        {
            if (data is TabItem)
                data = ((TabItem)data).Content;

            return data == null
                ? null
                : _itemsHolder?.Children.Cast<ContentPresenter>().FirstOrDefault(cp => cp.Content == data);
        }

        private void RestorePreviousSelection()
        {
            var previousSelection = _previousSelection?.Target;
            if (previousSelection != null && Items.Contains(previousSelection))
                SelectedItem = previousSelection;
            else
                SelectedItem = Items.OfType<object>().FirstOrDefault();
        }

    }
}
