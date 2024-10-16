using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;
using System.Windows.Threading;

namespace DrawingNameComposer.CustomControls
{
	/// <summary>
	/// Interaction logic for StatusMessagePopup.xaml
	/// </summary>
	public partial class StatusMessagePopup : UserControl
	{
		public StatusMessagePopup()
		{
			InitializeComponent();
			_fadeInAnimation = new DoubleAnimation
			{
				From = 0.0,
				To = 1.0,
				Duration = new Duration(TimeSpan.FromSeconds(2))
			};
			_fadeInStoryBoard = new Storyboard();
			_fadeInStoryBoard.Children.Add(_fadeInAnimation);
			Storyboard.SetTargetName(_fadeInAnimation, "root");
			Storyboard.SetTargetProperty(_fadeInAnimation, new PropertyPath(OpacityProperty));
			this.Visibility = Visibility.Collapsed;
			_fadeOutAnimationSlow = new DoubleAnimation
			{
				From = 1.0,
				To = 0.0,
				Duration = new Duration(TimeSpan.FromSeconds(5))
			};
			_fadeOutSlowStoryBoard = new Storyboard();
			_fadeOutSlowStoryBoard.Children.Add(_fadeOutAnimationSlow);
			Storyboard.SetTargetName(_fadeOutAnimationSlow, "root");
			Storyboard.SetTargetProperty(_fadeOutAnimationSlow, new PropertyPath(OpacityProperty));
			_timer = new DispatcherTimer
			{
				Interval = TimeSpan.FromMilliseconds(2000)
			};
			_timer.Tick += _timer_Tick;
			this.Opacity = 0;
		}

		private async void _timer_Tick(object sender, EventArgs e)
		{
			_fadeOutSlowStoryBoard.Begin(this);
			_timer.Stop();
			await Task.Delay(5000);
			this.Visibility = Visibility.Collapsed;
		}

		private DispatcherTimer _timer;
		private bool _isFadingOut;



		public static readonly DependencyProperty MessageProperty =
		  DependencyProperty.Register("Message", typeof(string), typeof(StatusMessagePopup), new PropertyMetadata(string.Empty, OnMessageChanged));
		public static readonly DependencyProperty ProgressValueProperty =
		  DependencyProperty.Register("ProgressValue", typeof(int), typeof(StatusMessagePopup), new PropertyMetadata(0, OnProgress));

		private static void OnProgress(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var uc = d as StatusMessagePopup;
			uc.ProgressBar.Value = (int)e.NewValue;
			if (uc.ProgressValue == 100)
			{
				uc._timer.Start();
			}

		}

		private readonly DoubleAnimation _fadeInAnimation;
		private readonly Storyboard _fadeInStoryBoard;
		private readonly DoubleAnimation _fadeOutAnimationSlow;
		private readonly Storyboard _fadeOutSlowStoryBoard;

		private static async void OnMessageChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
		{
			var uc = d as StatusMessagePopup;
			if (uc is not null && !string.IsNullOrEmpty(uc.Message))
			{
				uc._timer.Stop();
				if (uc.Opacity == 0)
				{
					uc._fadeInStoryBoard.Begin(uc);
					uc.Visibility = Visibility.Visible;
				}
				else
				{
					while (uc._isFadingOut)
					{
						await Task.Delay(500);
					}
					if (uc.Opacity == 0)
					{
						uc._fadeInStoryBoard.Begin(uc);
					}
				}
			}

		}

		public string Message
		{
			get { return (string)GetValue(MessageProperty); }
			set { SetValue(MessageProperty, value); }
		}

		public int ProgressValue
		{
			get { return (int)GetValue(ProgressValueProperty); }
			set { SetValue(ProgressValueProperty, value); }
		}
		private void Button_Click(object sender, RoutedEventArgs e)
		{
			_isFadingOut = true;
			var myDoubleAnimation = new DoubleAnimation
			{
				From = 1.0,
				To = 0.0,
				Duration = new Duration(TimeSpan.FromSeconds(0.5))
			};
			var myStoryboard = new Storyboard();
			myStoryboard.Children.Add(myDoubleAnimation);
			Storyboard.SetTargetName(myDoubleAnimation, "root");
			Storyboard.SetTargetProperty(myDoubleAnimation, new PropertyPath(OpacityProperty));
			myStoryboard.Begin(this);
			_isFadingOut = false;
		}
	}
}
