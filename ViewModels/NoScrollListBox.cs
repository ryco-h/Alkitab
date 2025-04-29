
using System;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Styling;

namespace Alkitab.Views
{
    public class NoScrollListBox : ListBox, IStyleable
    {
        // This tells Avalonia to apply ListBox styles/templates to this subclass
        Type IStyleable.StyleKey => typeof(ListBox);

        protected override void OnGotFocus(GotFocusEventArgs e)
        {
            // suppress auto-scroll on programmatic focus, but still allow mouse/tab focus
            if (e.NavigationMethod == NavigationMethod.Unspecified)
                return;
            base.OnGotFocus(e);
        }
        
        public NoScrollListBox()
        {
            SelectionChanged += OnSelectionChangedInternal;
            AddHandler(RequestBringIntoViewEvent, OnRequestBringIntoView, RoutingStrategies.Tunnel);
        }

        private void OnSelectionChangedInternal(object? sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.AddedItems)
            {
                var index = Items?.Cast<object>().ToList().IndexOf(item) ?? -1;
                if (index >= 0)
                {
                    var container = ItemContainerGenerator.ContainerFromIndex(index) as Control;
                    if (container != null)
                    {
                        container.AddHandler(RequestBringIntoViewEvent, SuppressBringIntoView, RoutingStrategies.Tunnel);
                    }
                }
            }
        }

        private void SuppressBringIntoView(object? sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }
        
        private void OnRequestBringIntoView(object? sender, RequestBringIntoViewEventArgs e)
        {
            // Suppress any automatic scrolling behavior
            e.Handled = true;
        }

    }
}
