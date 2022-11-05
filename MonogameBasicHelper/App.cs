using MonogameBasicHelper.Test;
using MonogameBasicHelper.ViewGenerator;
using System.Drawing;
using System.Windows.Forms;

namespace MonogameBasicHelper
{
    internal class App
    {
        public static void Main(string[] args)
        {
            TestViewModel testViewModel = new TestViewModel();

            Form frm = new Form();
            frm.Size = new System.Drawing.Size(500, 500);
            var cntrl = AnnotationBasedPropertyViewGenerator<TestViewModel>.Create(testViewModel);
            
            cntrl.Padding = new Padding(10);
            cntrl.ForeColor = Color.Red;
            cntrl.Font = new Font("Arial", 12, FontStyle.Bold);
            cntrl.Initialize();

            frm.Controls.Add(cntrl);

            testViewModel.N = 161;

            Application.Run(frm); 
        }
    }
}
