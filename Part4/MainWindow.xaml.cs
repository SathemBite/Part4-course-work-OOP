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
            Courses.formGroups(dg_groups, dg_students);
        }

        private void b_enrollRandStudent_Click(object sender, RoutedEventArgs e)
        {
            for (int i = 0; i < 50; i++)
            Courses.addRandomStudent();
        }

        private void button_makeTwoWeekStep_Click(object sender, RoutedEventArgs e)
        {
            Courses.makeTwoWeakSteps(dg_groups, dg_students);
        }

        private void dg_students_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Courses.mapStudentOnCourses(dg_students, dg_courses);
        }

        private void dg_groups_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Courses.mapGroupOnStudents(dg_groups, dg_students);
        }

        private void button_ClearTables_Click(object sender, RoutedEventArgs e)
        {
            dg_groups.Items.Clear();
            dg_courses.Items.Clear();
            dg_students.Items.Clear();
        }
    }
}
