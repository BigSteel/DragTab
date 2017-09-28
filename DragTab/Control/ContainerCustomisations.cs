using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace DragTab.Control
{
    internal class ContainerCustomisations
    {
        private readonly Func<DragTabItem> _getContainerForItemOverride;
        private readonly Action<DependencyObject, object> _prepareContainerForItemOverride;
        private readonly Action<DependencyObject, object> _clearingContainerForItemOverride;

        public ContainerCustomisations(Func<DragTabItem> getContainerForItemOverride = null, Action<DependencyObject, object> prepareContainerForItemOverride = null, Action<DependencyObject, object> clearingContainerForItemOverride = null)
        {
            _getContainerForItemOverride = getContainerForItemOverride;
            _prepareContainerForItemOverride = prepareContainerForItemOverride;
            _clearingContainerForItemOverride = clearingContainerForItemOverride;
        }

        public Func<DragTabItem> GetContainerForItemOverride
        {
            get { return _getContainerForItemOverride; }
        }

        public Action<DependencyObject, object> PrepareContainerForItemOverride
        {
            get { return _prepareContainerForItemOverride; }
        }

        public Action<DependencyObject, object> ClearingContainerForItemOverride
        {
            get { return _clearingContainerForItemOverride; }
        }
    }
}
