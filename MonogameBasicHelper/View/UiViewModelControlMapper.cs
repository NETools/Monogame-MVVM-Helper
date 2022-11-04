using MonogameBasicHelper.Attributes;
using MonogameBasicHelperDLL;
using MonogameBasicHelperDLL.MVVM;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonogameBasicHelper.View
{
    public class UiViewModelControlMapper<T> where T : IViewModel
    {
        private Dictionary<string, PanelSubRoutineGen<T>> _subroutines = new Dictionary<string, PanelSubRoutineGen<T>>();
        private T _loadedViewModel;

        public int Padding { get; set; } = 10;
        public Color BackgroundColor { get; set; } = Color.FromArgb(30, 30, 34);

        private UiViewModelControlMapper(T loadedViewModel)
        {
            _loadedViewModel = loadedViewModel;
        }
        public static UiViewModelControlMapper<T> New(T viewModel) 
        {
            return new UiViewModelControlMapper<T>(viewModel);
        }

        private void AddSubRoutines( Type attributeType)
        {
            foreach (var property in Helper.GetPropertiesWithAttribute(typeof(T), attributeType))
            {
                var attribute = (UiElement)property.GetCustomAttributes(typeof(UiElement), true)[0];
                var groupName = attribute.GroupName;

                if (!_subroutines.ContainsKey(groupName))
                    _subroutines[groupName] = new PanelSubRoutineGen<T>(_loadedViewModel, Padding);
                _subroutines[groupName].AddProperty(property, attribute);
            }
        }

        public void GenerateUi(Control frmInstance) 
        {
            AddSubRoutines(typeof(UiElement));

            AddSubRoutines(typeof(ButtonElement));
            AddSubRoutines(typeof(TextBoxElement));
            AddSubRoutines(typeof(ScrollBarElement));

            Panel parent = new Panel();
            parent.AutoScroll = true;
            parent.BackColor = BackgroundColor;
            parent.AutoSize = true;

            int accumulatorY = 0;
            _subroutines.Values.ToList().ForEach(p =>
            {
                var cntrl = p.GetControl(accumulatorY);
                parent.Controls.Add(cntrl);
                accumulatorY += cntrl.Size.Height + Padding;
            });

            frmInstance.Controls.Add(parent);
        }

    }
}
