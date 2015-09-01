using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ClockWidget
{
    public partial class EditWidget : Window
    {
        private ColorPicker colorPicker;
        private LabelWidget widget;
        private MainWindow mainWindow;

        public EditWidget(MainWindow window, LabelWidget labelWidget)
        {
            InitializeComponent();

            mainWindow = window;
            widget = labelWidget;

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
            ContentTextBox.Text = widget.WidgetText;
            FontFamilyComboBox.SelectedValue = widget.FontFamily.Source;

            this.UpdateFontStyles(widget.FontFamily);
            this.UpdateFontWeights(widget.FontFamily, widget.FontStyle);

            FontStyleComboBox.SelectedItem = widget.FontStyle;
            FontWeightComboBox.SelectedItem = widget.FontWeight;
            FontSizeTextBox.Text = widget.FontSize.ToString();
            ForegroundColorButton.Background = widget.Foreground;
            OpacitySlider.Value = widget.Opacity * 100f;

            ContentTextBox.TextChanged += ContentTextBox_TextChanged;
            FontFamilyComboBox.SelectionChanged += FontFamilyComboBox_SelectionChanged;
            FontStyleComboBox.SelectionChanged += FontStyleComboBox_SelectionChanged;
            FontWeightComboBox.SelectionChanged += FontWeightComboBox_SelectionChanged;
            FontSizeTextBox.TextChanged += FontSizeTextBox_TextChanged;
            OpacitySlider.ValueChanged += OpacitySlider_ValueChanged;
        }

        private void ContentTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            widget.WidgetText = ContentTextBox.Text;
            widget.Update();
            widget.UpdateUI();
        }

        private void FontFamilyComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamily fontFamily = new FontFamily(FontFamilyComboBox.SelectedValue.ToString());

            if (fontFamily != null)
            {
                this.UpdateFontStyles(fontFamily);
                this.UpdateFontWeights(fontFamily, (FontStyle) FontStyleComboBox.SelectedItem);

                widget.FontFamily = fontFamily;
                widget.FontStyle = (FontStyle)FontStyleComboBox.SelectedItem;
                widget.FontWeight = (FontWeight)FontWeightComboBox.SelectedItem;
            }
        }

        private void FontStyleComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamily fontFamily = new FontFamily(FontFamilyComboBox.SelectedValue.ToString());

            if (fontFamily != null)
            {
                this.UpdateFontWeights(fontFamily, (FontStyle) FontStyleComboBox.SelectedItem);

                widget.FontStyle = (FontStyle)FontStyleComboBox.SelectedItem;
            }
        }

        private void FontWeightComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            FontFamily fontFamily = new FontFamily(FontFamilyComboBox.SelectedValue.ToString());

            if (fontFamily != null)
            {
                if (FontWeightComboBox.SelectedItem == null)
                    FontWeightComboBox.SelectedIndex = 0;

                widget.FontWeight = (FontWeight)FontWeightComboBox.SelectedItem;
            }
        }

        private void FontSizeTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = sender as TextBox;

            if (!Regex.IsMatch(textBox.Text, "^[0-9]+$"))
            {
                textBox.Text = Helper.FilterByRegex(textBox.Text, "^[0-9]+$");
                textBox.CaretIndex = textBox.Text.Length;
                return;
            }

            if (!String.IsNullOrEmpty(textBox.Text))
            {
                double fontSize = Double.Parse(textBox.Text);
                widget.FontSize = Helper.Clamp(fontSize, 1, 5000); ;
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
            widget.Foreground = colorPicker.GetBrush();
            ForegroundColorButton.Background = colorPicker.GetBrush();
        }

        private void OpacitySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            double opacity = OpacitySlider.Value / 100f;
            OpacityLabel.Content = Math.Round(opacity * 100) + "%";

            widget.Opacity = opacity;
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            mainWindow.SaveData();
        }
    }
}
