using csharp_api;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Group_8_Project
{
    class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        /// 

        apipull api = new apipull();

        static async Task Main(string[] args)
        {
            Program program = new Program();
            await program.api.GetResponse();       
            

            Application.SetHighDpiMode(HighDpiMode.SystemAware);
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());

        }


        //[STAThread]
        //static void Main()
        //{
        //    Application.SetHighDpiMode(HighDpiMode.SystemAware);
        //    Application.EnableVisualStyles();
        //    Application.SetCompatibleTextRenderingDefault(false);
        //    Application.Run(new Form1());

        //}
    }
}
