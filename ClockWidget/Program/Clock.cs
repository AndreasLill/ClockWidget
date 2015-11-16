using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClockWidget
{
    public class Clock : Label
    {
        private const string HOUR_24 = "HH";
        private const string HOUR_12 = "h";
        private const string MINUTES = "mm";
        private const string SECONDS = "ss";
        private const string AMPM = "tt";

        private Point location;
        private CultureInfo culture;
        private string parsedTime;

        public string TimeFormat;
        public bool Display12h;
        public bool DisplaySeconds;

        public Clock()
        {
            // Defaults
            culture = new CultureInfo("en-US");
            Display12h = false;
            DisplaySeconds = false;
            TimeFormat = "HH:mm";

            this.Content = DateTime.Now.ToString(TimeFormat, culture);
            this.Foreground = Brushes.White;
            this.Background = Brushes.Transparent;
            this.FontFamily = new FontFamily("Calibri");
            this.FontStyle = FontStyles.Normal;
            this.FontWeight = FontWeights.Normal;
            this.FontSize = 72;
        }

        /// <summary>
        /// Parse text and replace time codes with time values.
        /// </summary>
        /// <returns>The parsed text</returns>
        private string ParseTime()
        {
            string hour = HOUR_24;

            if (Display12h)
            {
                hour = HOUR_12;
            }

            TimeFormat = hour + ":" + MINUTES;

            if (DisplaySeconds)
            {
                TimeFormat += ":" + SECONDS;
            }

            if (Display12h)
            {
                TimeFormat += " " + AMPM;
            }

            return DateTime.Now.ToString(TimeFormat, culture);
        }

        /// <summary>
        /// Update the widget.
        /// </summary>
        public void Update()
        {
            parsedTime = ParseTime();
        }

        /// <summary>
        /// Update the widget UI.
        /// </summary>
        public void UpdateUI()
        {
            this.Content = parsedTime;
        }

        /// <summary>
        /// Get the widget location data.
        /// </summary>
        /// <returns>The location in panel</returns>
        public Point GetLocation()
        {
            return location;
        }

        /// <summary>
        /// Set the location of the widget.
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public void SetLocation(double x, double y)
        {
            location = new Point(x, y);
        }
    }
}
