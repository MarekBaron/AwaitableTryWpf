using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace AwaitableTryWpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private EventAwaiter _eventAwaiter;

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("button1_Click: Before await");
            _eventAwaiter = new EventAwaiter();
            var result = await _eventAwaiter;
            Debug.WriteLine($"button1_Click: After await: {result} ::");
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine("button2_Click: Before set");
            _eventAwaiter.TextValue = TextBox.Text;
            Debug.WriteLine("button2_Click: After set");
        }
    }

    public class EventAwaiter : INotifyCompletion
    {
        //----------------------
        public EventAwaiter GetAwaiter()
        {
            EnqueueCheck();
            return this;
        }

        private void EnqueueCheck()
        {
            Dispatcher.CurrentDispatcher.BeginInvoke(new Action(() =>
            {
                if (!IsCompleted)
                {
                    EnqueueCheck();
                }
                else
                {
                    Dispatcher.CurrentDispatcher.BeginInvoke(_continuation, DispatcherPriority.Input);
                }
            }), DispatcherPriority.Input);
        }

        //----------------------

        public string TextValue { get; set; }

        private Action _continuation;

        public void OnCompleted(Action aContinuation)
        {
            _continuation = aContinuation;
        }

        public bool IsCompleted => !string.IsNullOrEmpty(TextValue);
        
        public string GetResult()
        {
            Debug.WriteLine("GetResult");
            return TextValue;
        }
    }

}
