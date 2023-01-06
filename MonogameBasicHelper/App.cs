using MonogameBasicHelper.ContainerService;
using MonogameBasicHelper.MVVM;
using MonogameBasicHelper.Test;
using MonogameBasicHelper.ViewGenerator;
using SharpDX.Direct3D11;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace MonogameBasicHelper
{
    internal class App
    {
        public interface ITest : IViewModel
        {
            public IParam Adapter { get; set; }
            public string Name { get; set; }

            void Run();

        }

        public interface IParam : IAdapter
        {
            void Run();
        }

        public class TestAdapter : IParam
        {
            static int counter;

            public TestAdapter()
            {
                counter++;
            }

            public void Run()
            {
                Console.WriteLine($"#{counter}");
            }
        }

        public class Test : ITest
        {
            public IParam Adapter { get; set; }
            public string Name { get; set; }

            public Test(IParam test)
            {
                this.Adapter = test;
                counter++;
            }

            static int counter;
 

            public void Run()
            {
                Console.WriteLine($"#{counter}");
            }
        }


        public static void Main(string[] args)
        {
            //TestViewModel testViewModel = new TestViewModel();

            //Form frm = new Form();
            //frm.Size = new System.Drawing.Size(500, 500);
            //var cntrl = AnnotationBasedPropertyViewGenerator<TestViewModel>.Create(testViewModel);

            //cntrl.Padding = new Padding(10);
            //cntrl.ForeColor = Color.Red;
            //cntrl.Font = new Font("Arial", 12, FontStyle.Bold);
            //cntrl.Initialize();

            //frm.Controls.Add(cntrl);

            //testViewModel.N = 161;

            //Application.Run(frm); 


            MonogameDepdencyInjection service = MonogameDepdencyInjection.Services;
            service.AddAdapter<IParam, TestAdapter>();
            service.AddViewModel<ITest, Test>();

            var s = service.GetViewModel<ITest>();
            s.Run();
            s.Adapter.Run();

            Console.WriteLine();

            var s1 = service.GetViewModel<ITest>();
            s1.Run();
            s1.Adapter.Run();

            Console.WriteLine();

            var s2 = service.New<Test>();
            s2.Run();
            s2.Adapter.Run();

            Console.WriteLine();



            var s3 = service.New<Test>();
            s2.Run();
            s2.Adapter.Run();

            Console.WriteLine();



            var s4 = service.New<Test>();
            s2.Run();
            s2.Adapter.Run();

            Console.WriteLine();


            var s5 = service.GetViewModel<ITest>();
            s1.Run();
            s1.Adapter.Run();

            Console.WriteLine();
        }
    }
}
