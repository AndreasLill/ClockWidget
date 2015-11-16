using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

/*
    Author: Andreas Lill
    License: Apache License 2.0

    http://www.apache.org/licenses/LICENSE-2.0
*/
namespace ClockWidget
{
    public partial class EditClock : Window
    {
        private ColorPicker colorPicker;
        private Clock clockWidget;
        private MainWindow mainWindow;

        public EditClock(MainWindow window, Clock clock)
        {
            InitializeComponent();

            mainWindow = window;
            clockWidget = clock;

            InitializeFonts();
            InitializeControls();
        }

        private void InitializeFonts()
        {
            FontFamilyComboBox.SelectedValuePath = "Content";

            // Load all system font families.
            var fonts = Fonts.SystemFontFamilies.OrderBy(font => font.ToString());

            // Create a droplist item for each font family.
            foreach (FontFamily font in fonts)
            {
                ComboBoxItem item = new ComboBoxItem();
                item.Content = font.Source;
                item.FontFamily = font;
                item.FontSize = 16;
                item.Height = 28;
                item.VerticalContentAlignment = VerticalAlignment.Center;
                item.RequestBringIntoView += ComboBoxItem_RequestBringIntoView;

                FontFamilyComboBox.Items.Add(item);
            }
        }

        private void ComboBoxItem_RequestBringIntoView(object sender, RequestBringIntoViewEventArgs e)
        {
            e.Handled = true;
        }

        /// <summary>
        /// Get all font styles from a font and return as an array.
        /// </summary>
        /// <param name="font">The font to use</param>
        /// <returns>The array of font styles</returns>
        private FontStyle[] GetFontStyles(FontFamily font)
        {
            List<FontStyle> list = new List<FontStyle>();
            var typeFaces = font.FamilyTypefaces.OrderBy(f => f.Style.ToString());

            foreach (FamilyTypeface face in typeFaces)
            {
                if (!list.Contains(face.Style))
                {
                    list.Add(face.Style);
                }
            }

            return (FontStyle[])list.ToArray();
        }

        /// <summary>
        /// Get all font weights from a font style and return as array.
        /// </summary>
        /// <param name="font">The font to use</param>
        /// <param name="fontStyle">The font style</param>
        /// <returns>The array of font weights</returns>
        private FontWeight[] GetFontWeights(FontFamily font, FontStyle fontStyle)
        {
            List<FontWeight> list = new List<FontWeight>();
            var typeFaces = font.FamilyTypefaces.OrderBy(f => f.Style.ToString());

            foreach (FamilyTypeface face in typeFaces)
            {
                if (!list.Contains(face.Weight) && String.Equals(face.Style.ToString(), fontStyle.ToString()))
                {
                    list.Add(face.Weight);
                }
            }

            return (FontWeight[])list.ToArray();
        }

        /// <summary>
        /// Updates the font styles and fills the list based on font.
        /// </summary>
        /// <param name="font">The font to use</param>
        private void UpdateFontStyles(FontFamily font)
        {
            if (font != null)
            {
                FontStyleComboBox.ItemsSource = GetFontStyles(font);
                FontStyleComboBox.SelectedItem = FontStyles.Normal;

                if (FontStyleComboBox.SelectedItem == null)
                    FontStyleComboBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Updates the font weights and fills the list based on the font style.
        /// </summary>
        /// <param name="font">The font to use</param>
        /// <param name="style">The font style</param>
        private void UpdateFontWeights(FontFamily font, FontStyle style)
        {
            if (font != null)
            {
                FontWeightComboBox.ItemsSource = GetFontWeights(font, style);
                FontWeightComboBox.SelectedItem = FontWeights.Normal;

                if (FontWeightComboBox.SelectedItem == null)
                    FontWeightComboBox.SelectedIndex = 0;
            }
        }

        private void InitializeControls()
        {
            FontFamilyComboBox.SelectedValue = clockWidget.FontFamily.Source;

            this.UpdateFontStyles(clockWidget.FontFamily);
            this.UpdateFontWeights(clockWidget.FontFamily, clockWidget.FontStyle);

            // Load settings into UI.
            ShowSecondsCheckBox.IsChecked = clockWidget.DisplaySeconds;
            TwelveHourCheckBox.IsChecked = clockWidget.Display12h;
            FontStyleComboBox.SelectedItem = clockWidget.FontStyle;
            FontWeightComboBox.SelectedItem = clockWidget.FontWeight;
            FontSizeTextBox.Text = clockWidget.FontSize.ToString();
            ForegroundColorButton.Background = clockWidget.Foreground;
            OpacitySlider.Value = clockWidget.Opacity * 100f;

            // Create control events.
            ShowSecondsCheckBox.Checked += ShowSecondsCheckBox_Checked;
            ShowSecondsCheckBox.Unchecked += ShowSecondsCheckBox_Unchecked;
            TwelveHourCheckBox.Checked += TwelveHourCheckBox_Checked;
            TwelveHourCheckBox.Unchecked += TwelveHourCheckBox_Unchecked;
            FontFamilyComboBox.SelectionChanged += FontFamilyComboBox_SelectionChanged;
            FontStyleComboBox.SelectionChanged += FontStyleComboBox_SelectionChanged;
            FontWeightComboBox.SelectionChanged += FontWeightComboBox_SelectionChanged;
            FontSizeTextBox.TextChanged += FontSizeTextBox_TextChanged;
            OpacitySlider.ValueChanged += OpacitySlider_ValueChanged;
        }

        private void TwelveHourCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            clockWidget.Display12h = false;

            clockWidget.Update();
            clockWidget.UpdateUI();
        }

        private void TwelveHourCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            clockWidget.Display12h = true;

            clockWidget.Update();
            clockWidget.UpdateUI();
        }

        private void ShowSecondsCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            clockWidget.DisplaySeconds = false;

            clockWidget.Update();
            clockWidget.UpdateUI();
        }

        private void ShowSecondsCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            clockWidget.DisplaySeconds = true;

            clockWidget.Update();
            clockWidget.UpdateUI();
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamily fontFamily = new FontFamily(FontFamilyComboBox.SelectedValue.ToString());

            if (fontFamily != null)
            {
                this.UpdateFontStyles(fontFamily);
                this.UpdateFontWeights(fontFamily, (FontStyle) FontStyleComboBox.SelectedItem);

                clockWidget.FontFamily = fontFamily;
                clockWidget.FontStyle = (FontStyle)FontStyleComboBox.SelectedItem;
                clockWidget.FontWeight = (FontWeight)FontWeightComboBox.SelectedItem;
            }
        }

        private void FontStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamily fontFamily = new FontFamily(FontFamilyComboBox.SelectedValue.ToString());

            if (fontFamily != null)
            {
                this.UpdateFontWeights(fontFamily, (FontStyle) FontStyleComboBox.SelectedItem);

                clockWidget.FontStyle = (FontStyle)FontStyleComboBox.SelectedItem;
            }
        }

        private void FontWeightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamily fontFamily = new FontFamily(FontFamilyComboBox.SelectedValue.ToString());

            if (fontFamily != null)
            {
                if (FontWeightComboBox.SelectedItem == null)
                    FontWeightComboBox.SelectedIndex = 0;

                clockWidget.FontWeight = (FontWeight)FontWeightComboBox.SelectedItem;
            }
        }

        private void FontSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            // Force use of numbers only using regex.
            if (!Regex.IsMatch(textBox.Text, "^[0-9]+$"))
            {
                textBox.Text = Helper.FilterByRegex(textBox.Text, "^[0-9]+$");
                textBox.CaretIndex = textBox.Text.Length;
                return;
            }

            if (!String.IsNullOrEmpty(textBox.Text))
            {
                double fontSize = Double.Parse(textBox.Text);
                clockWidget.FontSize = Helper.Clamp(fontSize, 1, 5000); ;
            }
        }

        private void ForegroundColorButton_Click(object sender, RoutedEventArgs e)
        {
            if (colorPicker != null)
                colorPicker.Close();

            colorPicker = new ColorPicker(ForegroundColorButton.Background);
            colorPicker.Apply += ColorPicker_ApplyForeground;
            colorPicker.Show();
        }

        private void ColorPicker_ApplyForeground(object sender, EventArgs e)
        {
            clockWidget.Foreground = colorPicker.GetBrush();
            ForegroundColorButton.Background = colorPicker.GetBrush();
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double opacity = OpacitySlider.Value / 100f;
            OpacityLabel.Content = Math.Round(opacity * 100) + "%";

            clockWidget.Opacity = opacity;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mainWindow.SaveData();
        }
    }
}
