using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Controls;
using System.Threading;
using System.IO;

namespace Part4.BusinessLogic
{
    class ModelData
    {
        public static int twoWeeksInSeconds = 10;
        public static MainWindow window;
        public static int modelPeriodDuration = 6;
        public static int everyTwoWeekEnnrolledStudents = 20;
        public static DataGrid groups;
        public static DataGrid students;
        public static bool isSuspended;

        public static void initAllVars(int twoWeeksDur, MainWindow wind, int modelDur, int enrollCount, DataGrid dgGroups, DataGrid dgStudents)
        {
            twoWeeksInSeconds = twoWeeksDur * 1000;
            window = wind;
            modelPeriodDuration = 4 *  modelDur;
            everyTwoWeekEnnrolledStudents = enrollCount;
            groups = dgGroups;
            students = dgStudents;
        }

    }

    class Model
    {
        private static Thread thread;
        private static StreamWriter sw;

        private static void runModel()
        {
            while (ModelData.modelPeriodDuration > 0)
            {
                while (ModelData.isSuspended) { }

                DateTime dt = DateTime.Now;
                for (int i = 0; i < ModelData.everyTwoWeekEnnrolledStudents; i++)
                {
                    Courses.addRandomStudent();
                }

                Courses.formGroups();
                Courses.makeTwoWeakSteps();
                ModelData.window.finishPeriodFromAnotherThread();
                ModelData.modelPeriodDuration -= 2;
                int millsSleep = ModelData.twoWeeksInSeconds - (DateTime.Now.Millisecond - dt.Millisecond);

                Thread.Sleep(millsSleep > 0 ? millsSleep : 0);
            }
            ModelData.window.finishModellingActions();
            sw = new StreamWriter("modeling.txt");
            sw.Write(Courses.getStatistics());
            sw.Close();
        }

        public static void run()
        {
            thread = new Thread(runModel);
            thread.Start();
        }

        public static bool isAlive()
        {
            return thread != null ? thread.IsAlive : false;
        }
    }
}
