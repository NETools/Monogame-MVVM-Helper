using MonogameBasicHelper.Attributes;
using MonogameBasicHelper.Concrete.Converters;
using MonogameBasicHelper.Events;
using MonogameBasicHelper.MVVM;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Windows.Forms;
 
namespace MonogameBasicHelper.ViewGenerator
{
    public enum TagType
    {
        DescriptionLabel,
        Parent,
        Child,
        Plain
    }

    internal class SubAnnotationBasedContentGenerator<T> : Panel where T: IPropertyChanged
    {

        private static Dictionary<Type, Type> _predefinedConverterTypes = new Dictionary<Type, Type>()
        {
            {typeof(double), typeof(DoubleConverter) }
        };

        private Dictionary<Type, Action<UiElement, PropertyInfo>> _actions = new Dictionary<Type, Action<UiElement, PropertyInfo>>();

        private int _currentHorizontalPosition, _currentVerticalPosition;
   
        public SubAnnotationBasedContentGenerator(AnnotationBasedPropertyViewGenerator<T> parent)
        {
            this.Parent = parent;
            this.Padding = parent.Padding;
            _actions.Add(typeof(TextBoxElement), (a, b) => HandleTextBoxElement((TextBoxElement)a, b));
            _actions.Add(typeof(ButtonElement), (a, b) => HandleButtonElement((ButtonElement)a, b));
            _actions.Add(typeof(TrackBarElement), (a, b) => HandleTrackBarElement((TrackBarElement)a, b));

            this.AutoSize = true;
            this.AutoScroll = true;

            _currentHorizontalPosition = Padding.Left;
            _currentVerticalPosition = Padding.Top;

            this.Location = new Point(Padding.Left, Padding.Top);
        }

        public void AddContent(UiElement uiElement, PropertyInfo propertyInfo)
        {
            _actions[uiElement.GetType()](uiElement, propertyInfo);
        }

        private void HandleTextBoxElement(TextBoxElement uiElement, PropertyInfo propertyInfo)
        {
            var label = GetLabel(propertyInfo.Name);
            var measuredSize = Helper.MeasureStringSize(label);
            label.Size = new Size((int)measuredSize.Width + 1, (int)measuredSize.Height);
            label.Tag = TagType.DescriptionLabel;

            MoveCursorHorizontal((int)measuredSize.Width + Padding.Right);
            
            var textBox = GetTextBox(uiElement.Width, (int)measuredSize.Height, propertyInfo.Name);
            textBox.Tag = TagType.Plain;

            var defaultConverter = _predefinedConverterTypes[propertyInfo.PropertyType];

            if (uiElement.ValueConverterType == null)
                uiElement.Converter = (IConverter)Activator.CreateInstance(defaultConverter);
            else uiElement.Converter = (IConverter)Activator.CreateInstance(uiElement.ValueConverterType);

            textBox.Text = propertyInfo.GetValue(((AnnotationBasedPropertyViewGenerator<T>)Parent).AnnotatedViewModel).ToString();

            textBox.Leave += (a, b) =>
            {
                var value = uiElement.Converter.Convert(textBox.Text);
                if (value != null)
                {
                    ((AnnotationBasedPropertyViewGenerator<T>)Parent).BeginChange();
                    propertyInfo.SetValue(((AnnotationBasedPropertyViewGenerator<T>)Parent).AnnotatedViewModel, value);
                    ((AnnotationBasedPropertyViewGenerator<T>)Parent).EndChange();
                }
            };

            MoveCursorVertical((int)measuredSize.Height + Padding.Bottom);

            this.Controls.AddRange(new Control[] { label, textBox });
            ((AnnotationBasedPropertyViewGenerator<T>)Parent).AdjustHorizontalPosition(_currentHorizontalPosition);

            ResetHorizontal();
        }

        private void HandleButtonElement(ButtonElement uiElement, PropertyInfo propertyInfo)
        {

        }

        private void HandleTrackBarElement (TrackBarElement uiElement, PropertyInfo propertyInfo)
        {
            var label = GetLabel(propertyInfo.Name);
            var measuredSize = Helper.MeasureStringSize(label);
            label.Size = new Size((int)measuredSize.Width + 1, (int)measuredSize.Height);
            label.Tag = TagType.DescriptionLabel;

            MoveCursorHorizontal((int)measuredSize.Width + Padding.Right);

            var trackBar = GetTrackBar(uiElement.Min, uiElement.Max, uiElement.Width, uiElement.TickFrequency, propertyInfo.Name);
            trackBar.Tag = TagType.Parent;

            MoveCursorVertical((int)trackBar.Height);
            ((AnnotationBasedPropertyViewGenerator<T>)Parent).AdjustHorizontalPosition(_currentHorizontalPosition);

            MoveCursorHorizontal(trackBar.Width);
            var trackBarValueLabel = GetLabel("1");
            measuredSize = Helper.MeasureStringSize(trackBarValueLabel);
            trackBarValueLabel.AutoSize = true;
            trackBarValueLabel.Tag = TagType.Child;

            var defaultConverter = _predefinedConverterTypes[propertyInfo.PropertyType];
            
            if (uiElement.ValueConverterType == null)
                uiElement.Converter = (IConverter)Activator.CreateInstance(defaultConverter);
            else uiElement.Converter = (IConverter)Activator.CreateInstance(uiElement.ValueConverterType);

            if (uiElement.ValueToTextConverterType == null)
                uiElement.ValueToTextConverter = new DefaultValueToTextConverter();
            else uiElement.ValueToTextConverter = (IValueTextConverter)Activator.CreateInstance(uiElement.ValueToTextConverterType);

            trackBar.ValueChanged += (a, b) =>
            {
                trackBarValueLabel.Text = uiElement.ValueToTextConverter.Convert(trackBar.Value);

                var value = uiElement.Converter.Convert(trackBar.Value);
                if (value != null)
                {
                    ((AnnotationBasedPropertyViewGenerator<T>)Parent).BeginChange();
                    propertyInfo.SetValue(((AnnotationBasedPropertyViewGenerator<T>)Parent).AnnotatedViewModel, value);
                    ((AnnotationBasedPropertyViewGenerator<T>)Parent).EndChange();
                }
            };

            this.Controls.AddRange(new Control[] { label,trackBar, trackBarValueLabel });
            trackBar.Value = (int)uiElement.Converter.ConvertBack(propertyInfo.GetValue(((AnnotationBasedPropertyViewGenerator<T>)Parent).AnnotatedViewModel));

            MoveCursorVertical((int)measuredSize.Height + Padding.Bottom);

            ResetHorizontal();
        }

        private Label GetLabel(string text)
        {
            Label label = new Label();
            label.Parent = this;
            label.Text = text;
            label.BackColor = Color.Transparent;
            label.Font = Parent.Font;
            label.ForeColor = Parent.ForeColor;
            label.Location = new System.Drawing.Point(_currentHorizontalPosition, _currentVerticalPosition);
            return label;
        }

        private TextBox GetTextBox(int width, int height, string name = "")
        {
            TextBox textBox = new TextBox();
            textBox.Parent = this;
            textBox.Size = new System.Drawing.Size(width, height);
            textBox.Font = Parent.Font;
            textBox.Location = new System.Drawing.Point(_currentHorizontalPosition, _currentVerticalPosition);
            textBox.Name = name;
            return textBox;
        }

        private TrackBar GetTrackBar(int min, int max, int width, int tickFrequency, string name = "")
        {
            TrackBar trackBar = new TrackBar();
            trackBar.Minimum = min;
            trackBar.Maximum = max;
            trackBar.Size = new Size(width, trackBar.Size.Height);
            trackBar.Location = new Point(_currentHorizontalPosition, _currentVerticalPosition);
            trackBar.TickFrequency = tickFrequency;
            trackBar.Name = name;
            return trackBar;
        }

        private void MoveCursorHorizontal(int dx)
        {
            _currentHorizontalPosition += dx;
        }

        private void MoveCursorVertical(int dy)
        {
            _currentVerticalPosition += dy;
        }

        public void ResetHorizontal()
        {
            _currentHorizontalPosition = Padding.Left;
        }

    }
}
