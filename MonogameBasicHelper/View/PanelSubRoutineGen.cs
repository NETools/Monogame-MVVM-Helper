using MonogameBasicHelper.Attributes;
using MonogameBasicHelperDLL;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using TrackBar = System.Windows.Forms.TrackBar;

namespace MonogameBasicHelper.View
{
    internal class PanelSubRoutineGen<T>
    {
        private struct _AttributeData
        {
            public PropertyInfo PropertyInfo;
            public UiElement UiElementData;
        }

        private int _positionY;
        private int _padding = 10;

        private List<_AttributeData> _properties = new List<_AttributeData>();
        private Dictionary<Type, Action<Panel, _AttributeData>> _typeActionMapping = new Dictionary<Type, Action<Panel, _AttributeData>>();

        private T _loadedViewModel;
        public PanelSubRoutineGen(T loadedViewModel, int padding)
        {
            this._loadedViewModel = loadedViewModel;
            _padding = padding;

            this._typeActionMapping.Add(typeof(TextBoxElement),
                    (panel, propertyInfo) =>
                    {
                        var textBoxAttribute = (TextBoxElement)propertyInfo.UiElementData;

                        if (string.IsNullOrEmpty(textBoxAttribute.LabelText))
                            textBoxAttribute.LabelText = propertyInfo.PropertyInfo.Name;

                        var label = GetLabel(textBoxAttribute.LabelText);

                        var fontWidth = (int)Math.Ceiling(Helper.MeasureStringSize(label).Width) + _padding * 3;
                        var fontHeight = label.Font.Height + _padding * 2;

                        var txtBox = GetTextBox(propertyInfo.PropertyInfo, fontHeight, fontWidth);
                        panel.Controls.AddRange(new Control[] { label, txtBox });

                        ExpandVertically(fontHeight);
                    });

            this._typeActionMapping.Add(typeof(ScrollBarElement), (panel, propertyInfo) =>
            {
                var textBoxAttribute = (ScrollBarElement)propertyInfo.UiElementData;

                if (string.IsNullOrEmpty(textBoxAttribute.LabelText))
                    textBoxAttribute.LabelText = propertyInfo.PropertyInfo.Name;

                var label = GetLabel(textBoxAttribute.LabelText);

                var fontWidth = (int)Math.Ceiling(Helper.MeasureStringSize(label).Width) + _padding * 3;
                var fontHeight = label.Font.Height + _padding * 2;

                var trackBar = GetTrackBar(textBoxAttribute.Min, textBoxAttribute.Max, textBoxAttribute.Start, fontWidth);

                ExpandVertically(fontHeight + trackBar.Height);

                var label2 = GetLabel(trackBar.Value + "", trackBar.Width, -fontHeight);
                fontHeight = label2.Font.Height + _padding * 2;

                trackBar.ValueChanged += (a, b) =>
                {
                    label2.Text = trackBar.Value + "";
                    propertyInfo.PropertyInfo.SetValue(_loadedViewModel, trackBar.Value);
                };

                panel.Controls.AddRange(new Control[] { label, trackBar, label2 });
            });

            this._typeActionMapping.Add(typeof(ButtonElement), (panel, propertyInfo) =>
            {
                //var textBoxAttribute = (ButtonElement)propertyInfo.UiElementData;

                //if (string.IsNullOrEmpty(textBoxAttribute.LabelText))
                //    textBoxAttribute.LabelText = propertyInfo.PropertyInfo.Name;

                //var label = GetLabel(textBoxAttribute.LabelText);

                //var fontWidth = (int)Math.Ceiling(Helper.MeasureStringSize(label).Width) + _padding * 3;
                //var fontHeight = label.Font.Height + _padding * 2;

                //var txtBox = GetTextBox(propertyInfo.PropertyInfo, fontHeight, fontWidth, _positionY);
                //panel.Controls.AddRange(new Control[] { label, txtBox });

                //ExpandVertically(fontHeight);
            });
        }

        private void ExpandVertically(int dy)
        {
            _positionY += dy;
        }

        public void AddProperty(PropertyInfo propertyInfo, UiElement attribute)
        {
            _properties.Add(new _AttributeData()
            {
                PropertyInfo = propertyInfo,
                UiElementData = attribute
            });
        }

        private TrackBar GetTrackBar(int min, int max, int start, int posX)
        {
            TrackBar trackBar = new TrackBar();
            trackBar.Minimum = min;
            trackBar.Maximum = max;
            trackBar.Value = start;

            trackBar.Size = new Size((max - min) * 5 , trackBar.Size.Height);
            trackBar.Location = new Point(posX + _padding, _positionY + trackBar.Height / 2);

            return trackBar;

        }

        private Label GetLabel(string text, int posX = 0, int posY = 0)
        {
            Label label = new Label();
            label.Text = text;
            label.Padding = new Padding(_padding);
            label.Location = new System.Drawing.Point(_padding + posX, _positionY + _padding + posY);
            label.AutoSize = true;
            label.Font = new Font("Arial", 12, FontStyle.Bold);
            label.ForeColor = Color.FromArgb(130, 130, 134);

            return label;
        }

        private TextBox GetTextBox(PropertyInfo propertyInfo, int height, int posX)
        {
            TextBox textBox = new TextBox();
            textBox.Size = new Size(100, height);
            textBox.Location = new Point(posX + _padding, _positionY + _padding * 2);

            textBox.Leave += (a, b) =>
            {
                if (double.TryParse(textBox.Text, out double rslt))
                    propertyInfo.SetValue(_loadedViewModel, rslt); // todo
            };

            return textBox;
        }

        public Control GetControl(int lastY)
        {
            Panel panel = new Panel();
            panel.AutoSize = true;
            panel.AutoScroll = true;
            panel.BackColor = Color.FromArgb(30, 30, 32);
            panel.Location = new Point(panel.Location.X + _padding, lastY);
            panel.BorderStyle = BorderStyle.Fixed3D;

            foreach (var property in _properties)
            {
                _typeActionMapping[property.UiElementData.GetType()](panel, property);
            }

            return panel;
        }

    }
}
