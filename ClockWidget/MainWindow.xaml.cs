using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;

using Application = System.Windows.Application;
using ContextMenu = System.Windows.Controls.ContextMenu;
using MenuItem = System.Windows.Controls.MenuItem;
using MouseEventArgs = System.Windows.Input.MouseEventArgs;

namespace ClockWidget
{
    public partial class MainWindow : Window
    {
        private readonly BackgroundWorker updateWorker;
        private NotifyIcon notifyIcon;
        private ContextMenu widgetMenu;

        private List<LabelWidget> widgetList;

        private Point moveFrom;
        private bool isLocked;
        private bool isMoving;
        private int extendedStyle;

        public MainWindow()
        {
            InitializeComponent();
            InitializeMenu();
            InitializeWidgets();

            Width = SystemParameters.WorkArea.Width;
            Height = SystemParameters.WorkArea.Height;

            // Create a new background worker to update all widgets.
            updateWorker = new BackgroundWorker();
            updateWorker.WorkerSupportsCancellation = true;
            updateWorker.WorkerReportsProgress = true;
            updateWorker.DoWork += updateWorker_DoWork;
            updateWorker.ProgressChanged += updateWorker_ProgressChanged;

            updateWorker.RunWorkerAsync();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            extendedStyle = WindowUtility.GetExtendedStyle(this);

            isLocked = FileIO.LoadWidgetLock();

            if (isLocked)
            {
                LockWindow();
            }
            else
            {
                UnlockWindow();
            }
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            WindowUtility.SendWindowToBack(this);
        }

        /// <summary>
        /// Create all menu items.
        /// </summary>
        private void InitializeMenu()
        {
            notifyIcon = new NotifyIcon();

            notifyIcon.ContextMenu = new System.Windows.Forms.ContextMenu();
            notifyIcon.Icon = System.Drawing.SystemIcons.Application;
            notifyIcon.Visible = true;

            notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Lock", Menu_Lock));
            notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("-"));
            notifyIcon.ContextMenu.MenuItems.Add(new System.Windows.Forms.MenuItem("Exit", Menu_Exit));

            ContextMenu = new ContextMenu();
            widgetMenu = new ContextMenu();

            MenuItem menuItem;

            menuItem = new MenuItem();
            menuItem.Header = "Edit";
            menuItem.Click += Menu_EditWidget;
            widgetMenu.Items.Add(menuItem);

            widgetMenu.Items.Add(new Separator());

            menuItem = new MenuItem();
            menuItem.Header = "Remove";
            menuItem.Click += Menu_RemoveWidget;
            widgetMenu.Items.Add(menuItem);

            menuItem = new MenuItem();
            menuItem.Header = "New Label";
            menuItem.Click += Menu_NewLabel;
            this.ContextMenu.Items.Add(menuItem);
        }

        /// <summary>
        /// Initialize all widgets.
        /// </summary>
        private void InitializeWidgets()
        {
            widgetList = FileIO.LoadWidgets();

            foreach (LabelWidget widget in widgetList)
            {
                this.AddWidget(widget);
            }
        }

        /// <summary>
        /// Add a new widget to the window.
        /// </summary>
        /// <param name="widget">The widget to add</param>
        private void AddWidget(LabelWidget widget)
        {
            widget.MouseMove += Widget_MouseMove;
            widget.MouseUp += Widget_MouseUp;
            widget.MouseDown += Widget_MouseDown;
            widget.ContextMenu = widgetMenu;

            Panel.Children.Add(widget);

            Canvas.SetLeft(widget, widget.GetLocation().X);
            Canvas.SetTop(widget, widget.GetLocation().Y);
        }

        private void Menu_NewLabel(object sender, RoutedEventArgs e)
        {
            LabelWidget widget = new LabelWidget();

            widget.MouseMove += Widget_MouseMove;
            widget.MouseUp += Widget_MouseUp;
            widget.MouseDown += Widget_MouseDown;

            widget.ContextMenu = widgetMenu;

            widgetList.Add(widget);
            Panel.Children.Add(widget);

            Point mousePos = Mouse.GetPosition(Panel);
            widget.SetLocation(mousePos.X, mousePos.Y);

            Canvas.SetLeft(widget, widget.GetLocation().X);
            Canvas.SetTop(widget, widget.GetLocation().Y);

            SaveData();
        }

        private void Menu_RemoveWidget(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item != null)
            {
                var menu = item.Parent as ContextMenu;

                if (menu != null)
                {
                    var widget = menu.PlacementTarget as LabelWidget;

                    if (widget != null)
                    {
                        Panel.Children.Remove(widget as UIElement);
                        widgetList.Remove(widget);

                        SaveData();
                    }
                }
            }

            WindowUtility.SendWindowToBack(this);
        }

        private void Menu_EditWidget(object sender, RoutedEventArgs e)
        {
            var item = sender as MenuItem;

            if (item != null)
            {
                var menu = item.Parent as ContextMenu;

                if (menu != null)
                {
                    var widget = menu.PlacementTarget as LabelWidget;

                    if (widget != null)
                    {
                        new EditWidget(this, widget).Show();
                    }
                }
            }

            WindowUtility.SendWindowToBack(this);
        }

        private void Menu_Lock(object sender, EventArgs e)
        {
            if (!isLocked)
                LockWindow();
            else
                UnlockWindow();

            SaveData();
        }

        /// <summary>
        /// Lock the window and make it transparent.
        /// </summary>
        private void LockWindow()
        {
            isLocked = true;
            notifyIcon.ContextMenu.MenuItems[0].Text = "Unlock";

            WindowUtility.SetWindowTransparent(this, extendedStyle);

            this.Background = new SolidColorBrush(Color.FromArgb(0, 255, 255, 255));
            this.PanelBorder.BorderBrush = Brushes.Transparent;
        }

        /// <summary>
        /// Unlock the window and make it return to normal.
        /// </summary>
        private void UnlockWindow()
        {
            isLocked = false;
            notifyIcon.ContextMenu.MenuItems[0].Text = "Lock";

            WindowUtility.SetWindowNormal(this, extendedStyle);

            this.Background = new SolidColorBrush(Color.FromArgb(1, 255, 255, 255));
            this.PanelBorder.BorderBrush = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
        }

        private void Menu_Exit(object sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            updateWorker.CancelAsync();
            SaveData();
        }

        private void Widget_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!isLocked && e.LeftButton == MouseButtonState.Pressed && Keyboard.IsKeyDown(Key.LeftShift))
            {
                var element = sender as UIElement;

                moveFrom = e.GetPosition(element);
                Mouse.Capture(element);

                isMoving = true;
            }
        }

        private void Widget_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Released)
            {
                var element = sender as UIElement;

                Mouse.Capture(null);
                isMoving = false;

                SaveData();
            }
        }

        private void Widget_MouseMove(object sender, MouseEventArgs e)
        {
            if (!isLocked && isMoving && e.LeftButton == MouseButtonState.Pressed && Keyboard.IsKeyDown(Key.LeftShift))
            {
                var element = sender as UIElement;

                Point currentPos = e.GetPosition(Panel);

                element.SetValue(Canvas.LeftProperty, currentPos.X - moveFrom.X);
                element.SetValue(Canvas.TopProperty, currentPos.Y - moveFrom.Y);

                LabelWidget widget = element as LabelWidget;

                widget.SetLocation(currentPos.X - moveFrom.X, currentPos.Y - moveFrom.Y);
            }
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            WindowUtility.SendWindowToBack(this);
        }

        public void SaveData()
        {
            FileIO.SaveWidgets(widgetList, isLocked);
        }

        private void updateWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            while (!updateWorker.CancellationPending)
            {
                foreach (LabelWidget widget in widgetList)
                {
                    widget.Update();
                }

                updateWorker.ReportProgress(0);
                Thread.Sleep(1000);
            }
        }

        private void updateWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            foreach (LabelWidget widget in widgetList)
            {
                 widget.UpdateUI();
            }
        }
    }
}
