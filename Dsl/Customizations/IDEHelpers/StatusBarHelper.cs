using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.VisualStudio.Shell.Interop;
using EnvDTE;

namespace consist.RapidEntity.Customizations.IDEHelpers
{
    public static class StatusBarHelper
    {
        static IVsStatusbar statusBar = null;
        static uint cookie = 0;
        static uint counter = 0;
        const uint PROGREES_TOTAL = 100;        

        public static void Initialise(ClassDiagram classDiagram)
        {
            cookie = 0;
            counter = 0;

            if (statusBar.IsNull())
            {
                statusBar = (IVsStatusbar)classDiagram.GetService(typeof(SVsStatusbar));
            }
        }

        public static void InitialiseProgressBar()
        {            
            statusBar.Progress(ref cookie, 1, "", 0, 0);
        }

        public static void IncrementProgress(uint increment, string label)
        {
            counter += increment;

            if (counter < PROGREES_TOTAL)
            {
                statusBar.Progress(ref cookie, 1, label, increment, PROGREES_TOTAL);
                System.Threading.Thread.Sleep(10);
            }
        }

        public static void ClearProgress()
        {
            counter = 0;
            statusBar.Progress(ref cookie, 0, "", 0, 0);
        }
    }
}
