using Pegasus.Phone.XF.ViewModels.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace Pegasus.Phone.XF
{
	public partial class TelemetryDetailsView : ContentView
	{
		public TelemetryDetailsView ()
		{
			InitializeComponent ();
		}

        protected override void OnBindingContextChanged()
        {
            base.OnBindingContextChanged();

            var viewModel = (TelemetryDetailsViewModel)this.BindingContext;

            if (viewModel == null)
            {
                return;
            }

            // This is terribly ugly.  However, ListView doesn't render properly
            // when databindings change on Windows and WP, so we're kinda screwed.
            // The grid is a lot more powerful anyways.
            for (int i = 0; i < viewModel.TelemetryItems.Count; i++)
            {
                var item = viewModel.TelemetryItems[i];

                Grid.RowDefinitions.Add(new RowDefinition());

                var name = new Label
                {
                    Text = item.Name,
                    HorizontalOptions = LayoutOptions.Start,
                    LineBreakMode = LineBreakMode.NoWrap
                };
                name.SetValue(Grid.ColumnProperty, 0);
                name.SetValue(Grid.RowProperty, i+1);
                Grid.Children.Add(name);

                var value = new Label
                {
                    HorizontalOptions = LayoutOptions.End,
                    LineBreakMode = LineBreakMode.NoWrap
                };
                value.BindingContext = item;
                value.SetBinding<TelemetryDetailsViewModel.TelemetryItem>(
                    Label.TextProperty,
                    t => t.Value);
                value.SetValue(Grid.ColumnProperty, 1);
                value.SetValue(Grid.RowProperty, i+1);
                Grid.Children.Add(value);
            }
        }
    }
}
