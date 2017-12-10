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
        }
    }
}
