using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;

namespace DiscAcessDeny
{
    static class Program
    {
        /// <summary>
        /// Главная точка входа для приложения.
        /// </summary>
        [STAThread]
        static void Main()
        {
            System.Threading.Mutex nmp = new System.Threading.Mutex(true, "{7fffef53-97fb-40a4-9f68-4f9b58cfc2d6}", out bool runned);
            if (runned)
            {
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new Main_form());
            }
            else
            {
                MessageBox.Show("Невозможно запустить второй экземпляр программы!", "Ошибка");
            }
        }
    }
}
