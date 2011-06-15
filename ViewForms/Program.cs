using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using GameEngine;
using System.Threading;

namespace ViewForms
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new View());
        }
    }

    //public class Starter : ApplicationContext
    //{
    //    Thread thread;
    //    View view;
    //    Game game;

    //    public Starter()
    //    {
            
    //        view = new View();
    //        view.ViewClosing += new EventHandler(view_ViewClosing);
    //        game = new Game(view);
    //        view.SetGame(game);
    //        view.Show();

    //        ThreadStart start = new ThreadStart(game.Start);
    //        thread = new Thread(start);
    //        thread.Start();
    //    }

    //    void view_ViewClosing(object sender, EventArgs e)
    //    {
    //        thread.Abort();
    //    }
    //}

}
