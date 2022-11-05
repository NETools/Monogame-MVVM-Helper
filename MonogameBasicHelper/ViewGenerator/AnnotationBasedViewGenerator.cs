using MonogameBasicHelper.Attributes;
using MonogameBasicHelper.Events;
using MonogameBasicHelper.MVVM;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;

namespace MonogameBasicHelper.ViewGenerator
{
    public class AnnotationBasedPropertyViewGenerator<T> : Panel where T : IPropertyChanged
    {

        private int _currentMaximumHorizontal;
        private bool _changedBegin;

        public T AnnotatedViewModel { get; private set; }
        public Control TargetControl { get; private set; }

        private Dictionary<string, SubAnnotationBasedContentGenerator<T>> 
            _contentGenerators = new Dictionary<string, SubAnnotationBasedContentGenerator<T>>();

        private AnnotationBasedPropertyViewGenerator(T annotatedViewModel)
        {
            AnnotatedViewModel = annotatedViewModel;
            this.AutoSize = true;
            this.Dock = DockStyle.Fill;
            this.AutoScroll = true;
            this.BorderStyle = BorderStyle.Fixed3D;

            annotatedViewModel.PropertyChanged += AnnotatedViewModel_PropertyChanged;

        }

        internal void BeginChange()
        {
            _changedBegin = true;
        }

        internal void EndChange()
        {
            _changedBegin = false;
        }

        private void AnnotatedViewModel_PropertyChanged(string name)
        {
            if (_changedBegin) return;

            var relevantControl = Controls.Find(name, true)[0];
            if (relevantControl is TextBox)
            {
                ((TextBox)relevantControl).Text = AnnotatedViewModel.GetType().GetProperty(name).GetValue(AnnotatedViewModel).ToString();
            }
            else if (relevantControl is TrackBar)
            {
                ((TrackBar)relevantControl).Value = int.Parse(AnnotatedViewModel.GetType().GetProperty(name).GetValue(AnnotatedViewModel) + ""); // not fine implemented yet.
            }
        }

        public static AnnotationBasedPropertyViewGenerator<T> Create(T viewModel)
        {
            return new AnnotationBasedPropertyViewGenerator<T>(viewModel);
        }

        public void Initialize()
        {
            var currentViewModelType = typeof(T);
            var properties = currentViewModelType.GetProperties();

            foreach (var property in properties)
            {
                var annotations = property.GetCustomAttributes(true);
                var currentUiElement = (UiElement)annotations.FirstOrDefault(p => p.GetType().BaseType.Equals(typeof(UiElement)));
                if (currentUiElement == null) continue;
                if (!_contentGenerators.ContainsKey(currentUiElement.GroupName))
                    _contentGenerators.Add(currentUiElement.GroupName, new SubAnnotationBasedContentGenerator<T>(this));
                _contentGenerators[currentUiElement.GroupName].AddContent(currentUiElement, property);
            }

            int currentVerticalPosition = Padding.Top;
            _contentGenerators.Values.ToList().ForEach(p =>
            {
                p.Location = new Point(p.Location.X, currentVerticalPosition);
                p.BorderStyle = BorderStyle.Fixed3D;
                this.Controls.Add(p);
                currentVerticalPosition += p.Size.Height + Padding.Bottom;
            });

            this.Location = new Point(this.Location.X + Padding.Left, this.Location.Y + Padding.Top);

            CompleteAlign();
        }

        internal void AdjustHorizontalPosition(int horizontal)
        {
            _currentMaximumHorizontal = Math.Max(horizontal, _currentMaximumHorizontal);
        }

        private void CompleteAlign()
        {
            foreach (Control panel in this.Controls)
            {
                Point parentLocation = new Point();
                foreach (Control control in panel.Controls)
                {
                    if(!control.Tag.Equals(TagType.DescriptionLabel))
                    {
                        if (control.Tag.Equals(TagType.Parent) || control.Tag.Equals(TagType.Plain))
                        {
                            parentLocation = control.Location;
                            var dx = _currentMaximumHorizontal - control.Location.X;
                            control.Location = new Point(control.Location.X + dx, control.Location.Y);
                        }
                        else if (control.Tag.Equals(TagType.Child))
                        {
                            var dx = _currentMaximumHorizontal - parentLocation.X;
                            control.Location = new Point(control.Location.X + dx, control.Location.Y);
                        }
                    }
                }
            }
        }
    }
}
