using System;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClockWidget
{
    public class LabelWidget : Label
    {
        private const string LINEBREAK = "\\n";

        private const string H12 = "{hh}";
        private const string H24 = "{HH}";
        private const string MIN = "{mm}";
        private const string SEC = "{ss}";
        private const string AMPM = "{tt}";

        private const string YEAR_FULL = "{yyyy}";
        private const string YEAR_SHORT = "{yy}";
        private const string MONTH_FULL = "{MMMM}";
        private const string MONTH_SHORT = "{MMM}";
        private const string MONTH_NUM = "{MM}";
        private const string MONTH_SHORT_NUM = "{M}";
        private const string DAY_FULL = "{dddd}";
        private const string DAY_SHORT = "{ddd}";
        private const string DAY_NUM = "{dd}";
        private const string DAY_SHORT_NUM = "{d}";

        private const string CPU_USAGE = "{CPU}";
        private const string MEMORY_USAGE = "{MEMORY}";

        private Point location;
        private CultureInfo culture;

        private string parsedText;

        public string WidgetText = "";

        public LabelWidget()
        {
            culture = new CultureInfo("en-US");

            // Defaults
            this.WidgetText = "Label";
            this.Content = GetParsedText();
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
        private string GetParsedText()
        {
            string text = WidgetText;

            if (text.Contains(LINEBREAK)) text = text.Replace(LINEBREAK, Environment.NewLine);

            if (text.Contains(H24)) text = text.Replace(H24, DateTime.Now.ToString("HH", culture));
            if (text.Contains(H12)) text = text.Replace(H12, DateTime.Now.ToString("%h", culture));
            if (text.Contains(MIN)) text = text.Replace(MIN, DateTime.Now.ToString("mm", culture));
            if (text.Contains(SEC)) text = text.Replace(SEC, DateTime.Now.ToString("ss", culture));
            if (text.Contains(AMPM)) text = text.Replace(AMPM, DateTime.Now.ToString("tt", culture));

            if (text.Contains(YEAR_FULL)) text = text.Replace(YEAR_FULL, DateTime.Now.ToString("yyyy", culture));
            if (text.Contains(YEAR_SHORT)) text = text.Replace(YEAR_SHORT, DateTime.Now.ToString("yy", culture));
            if (text.Contains(MONTH_FULL)) text = text.Replace(MONTH_FULL, DateTime.Now.ToString("MMMM", culture));
            if (text.Contains(MONTH_SHORT)) text = text.Replace(MONTH_SHORT, DateTime.Now.ToString("MMM", culture));
            if (text.Contains(MONTH_NUM)) text = text.Replace(MONTH_NUM, DateTime.Now.ToString("MM", culture));
            if (text.Contains(MONTH_SHORT_NUM)) text = text.Replace(MONTH_SHORT_NUM, DateTime.Now.ToString("M", culture));
            if (text.Contains(DAY_FULL)) text = text.Replace(DAY_FULL, DateTime.Now.ToString("dddd", culture));
            if (text.Contains(DAY_SHORT)) text = text.Replace(DAY_SHORT, DateTime.Now.ToString("ddd", culture));
            if (text.Contains(DAY_NUM)) text = text.Replace(DAY_NUM, DateTime.Now.ToString("dd", culture));
            if (text.Contains(DAY_SHORT_NUM)) text = text.Replace(DAY_SHORT_NUM, DateTime.Now.ToString("d", culture));

            return text;
        }

        /// <summary>
        /// Update the widget.
        /// </summary>
        public void Update()
        {
            parsedText = GetParsedText();
        }

        /// <summary>
        /// Update the widget UI.
        /// </summary>
        public void UpdateUI()
        {
            this.Content = parsedText;
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
