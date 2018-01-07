using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.IO;
using Part4.BusinessLogic.AdaptersToDataGrid;
using Part4.BusinessLogic;

namespace Part4
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            Student_Data newSt = new Student_Data();
            newSt.ShowDialog();
            if (newSt.nSt != null)
            {
                Courses.addStudent(newSt.nSt);
            }
            
            
        }

        private void button_formGroups_Click(object sender, RoutedEventArgs e)
        {
            Courses.formGroups();
            Courses.showAllGroupsAndStudents(dg_groups, dg_students);
        }

        private void b_enrollRandStudent_Click(object sender, RoutedEventArgs e)
        {
            int studentAddition;
            if (!int.TryParse(tb_enrollingCount.Text, out studentAddition)) { studentAddition = 30; }
            for (int i = 0; i < studentAddition; i++)
                Courses.addRandomStudent();
        }

        private void button_makeTwoWeekStep_Click(object sender, RoutedEventArgs e)
        {
            Courses.makeTwoWeakSteps();
            finishTwoWeekPeriod();
        }

        private void dg_students_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Courses.mapStudentOnCourses(dg_students, dg_courses);
        }

        private void dg_groups_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Courses.mapGroupOnStudents(dg_groups, dg_students);
            cb_studentParameters_SelectionChanged(null, null);
        }

        private void button_ClearTables_Click(object sender, RoutedEventArgs e)
        {
            dg_groups.Items.Clear();
            dg_courses.Items.Clear();
            dg_students.Items.Clear();
        }

        private void cb_groupLang_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (cb_groupLang.SelectedIndex == 0)
            {
                Courses.showAllGroupsInTable(dg_groups);
            }
            else
            {
                Courses.selectTheSpecGroups((Language)cb_groupLang.SelectedItem, dg_groups);
            }
        }

        private void cb_studentParameters_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (IsLoaded)
            {
                if (cb_studentType.SelectedIndex == 1)
                {
                    if (dg_groups.SelectedItem != null)
                    {
                        Courses.mapGroupOnStudents(dg_groups, dg_students);
                    }
                }
                else
                {
                    if (cb_studetnLang.SelectedIndex == 0)
                    {
                        Courses.showAllStudentsInTable(dg_students);
                    }
                    else
                    {
                        Courses.selectLangSpecStudents((Language)cb_studetnLang.SelectedItem, dg_students);
                    }

                }

                if (cb_studentAge.SelectedIndex != 0)
                {
                    Courses.selectByAgeStudents(dg_students, cb_studentAge.SelectedIndex);
                }
            }
        }

        public void finishTwoWeekPeriod()
        {
            calendar_curDate.DisplayDate = calendar_curDate.DisplayDate.AddDays(14);
            calendar_curDate.SelectedDate = calendar_curDate.DisplayDate;
            Courses.showAllGroupsAndStudents(dg_groups, dg_students);
        }

        public void finishPeriodFromAnotherThread()
        {
            Dispatcher.Invoke(new Action(finishTwoWeekPeriod));
        }

        private void button_runModelling_process_Click(object sender, RoutedEventArgs e)
        {

            int twoWeekDur;
            int modelDur;
            int studentAddition;
            if (!int.TryParse(tb_periodDur.Text, out twoWeekDur)) { twoWeekDur = 10; }
            if (!int.TryParse(tb_modelDur.Text, out modelDur)) { modelDur = 6; }
            if (!int.TryParse(tb_enrollingCount.Text, out studentAddition)) { studentAddition = 30; }

            ModelData.initAllVars(twoWeekDur, this, modelDur, studentAddition, dg_groups, dg_students);
            
            if (Model.isAlive())
            {
                if (ModelData.isSuspended)
                {
                    ModelData.isSuspended = false;
                    button_runModelling_process.Content = "Pause process";
                }
                else
                {
                    ModelData.isSuspended = true;
                    button_runModelling_process.Content = "Resume process";
                }

            }
            else
            {
                Model.run();
                button_runModelling_process.Content = "Pause process";
            } 
        }

        public void finishModellingActions()
        {
            Dispatcher.Invoke(new Action(() => button_runModelling_process.Content = "Run modeling process"));
        }
    }
}
