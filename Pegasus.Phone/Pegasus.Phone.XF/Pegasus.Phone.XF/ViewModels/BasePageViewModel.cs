using System;
using System.ComponentModel;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Pegasus.Phone.XF.ViewModels
{
	public class BasePageViewModel : BaseViewModel
	{
		public BasePageViewModel ()
		{
		}

		private string title = string.Empty;

		/// <summary>
		/// Gets or sets the "Title" property
		/// </summary>
		/// <value>The title.</value>
		public string Title
		{
			get { return title; }
			set { SetProperty (ref title, value); }
		}

		private string subTitle = string.Empty;
		/// <summary>
		/// Gets or sets the "Subtitle" property
		/// </summary>
		public string Subtitle
		{
			get { return subTitle; }
			set { SetProperty (ref subTitle, value);}
		}

		private string icon = null;
		/// <summary>
		/// Gets or sets the "Icon" of the viewmodel
		/// </summary>
		public string Icon
		{
			get { return icon; }
			set { SetProperty (ref icon, value);}
		}

		private bool isBusy;
		/// <summary>
		/// Gets or sets if the view is busy.
		/// </summary>
		public bool IsBusy 
		{
			get { return isBusy; }
			set { SetProperty (ref isBusy, value);}
		}

		private bool canLoadMore = true;
		/// <summary>
		/// Gets or sets if we can load more.
		/// </summary>
		public bool CanLoadMore
		{
			get { return canLoadMore; }
			set { SetProperty (ref canLoadMore, value);}
		}
	}
}

