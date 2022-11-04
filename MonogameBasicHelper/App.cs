using MonogameBasicHelper.Test;
using MonogameBasicHelper.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonogameBasicHelper
{
    internal class App
    {
        public static void Main(string[] args)
        {
            TestViewModel testViewModel = new TestViewModel();
            var uiMapper = UiViewModelControlMapper<TestViewModel>.New(testViewModel);

            Form frm = new Form();
            Panel panel = new Panel();
            panel.AutoScroll = true;
            panel.Size = new System.Drawing.Size(frm.Size.Width - 20, frm.Size. Height - 40);
            frm.Controls.Add(panel);

            uiMapper.GenerateUi(panel);

            Application.Run(frm); 
        }
    }
}
