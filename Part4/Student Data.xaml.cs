using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Part4
{

    public struct SkillProps
    {
        public SkillProps(KeyValuePair<Level, Intensity> skill, ClassTime classTime, VisitingDays visitingDays)
        {
            this.skill = skill;
            this.classTime = classTime;
            this.visitingDays = visitingDays;
        }
        KeyValuePair<Level, Intensity> skill;
        ClassTime classTime;
        VisitingDays visitingDays;
    }

    /// <summary>
    /// Логика взаимодействия для Student_Data.xaml
    /// </summary>
    public partial class Student_Data : Window
    {
        public Student nSt;
        private Dictionary<Language, SkillProps> skills;
        public Student_Data()
        {
            InitializeComponent();
            skills = new Dictionary<Language, SkillProps>();
            textBox_FocusChagedHandler(NameBox, false, "Enter your name here...");
            textBox_FocusChagedHandler(AgeBox, false, "Enter your age here...");
            skills_TextBox.Document.Blocks.Clear();
        }

        private void NameBox_LostFocus(object sender, RoutedEventArgs e)
        {
            textBox_FocusChagedHandler(sender, false, "Enter your name here...");
        }

        private void textBox_FocusChagedHandler(Object sender, bool isFocusGetting, string str)
        {
            RichTextBox tb = (RichTextBox)sender;
            if (isFocusGetting)
            {
                if (getText(tb) == str)
                {
                    tb.Document.Blocks.Clear();
                    changeTextColor((RichTextBox)sender, Colors.Black);
                }
            }
            else
            {
                if (getText(tb).Equals(string.Empty))
                {
                    tb.Document.Blocks.Clear();
                    tb.AppendText(str);
                    changeTextColor((RichTextBox)sender, Colors.LightGray);
                }
            }
        }

        private string getText(RichTextBox rTB)
        {
            return new TextRange(rTB.Document.ContentStart, rTB.Document.ContentEnd).Text.Trim(' ', '\n', '\r');
        }

        private void changeTextColor(RichTextBox rTB, Color color)
        {
            new TextRange(rTB.Document.ContentStart, rTB.Document.ContentEnd).ApplyPropertyValue(TextElement.ForegroundProperty, new SolidColorBrush(color));
        }

        private void addSkillBut_Click(object sender, RoutedEventArgs e)
        {
            if (skills.ContainsKey((Language)language_Box.SelectedValue))
            {
                MessageBox.Show("Skill with the specified language is exist!!!");
                return;
            }
            skills.Add(
                (Language)language_Box.SelectedValue, 
                new SkillProps(
                    new KeyValuePair<Level, Intensity>((Level)level_Box.SelectedValue, (Intensity)intensity_Box.SelectedValue),
                    determineClassTime(),
                    determineVisitingDays()));


            if (skills_TextBox.Document.Blocks.LastBlock != null)
            {
                skills_TextBox.Selection.ApplyPropertyValue(Paragraph.MarginProperty, new Thickness(0));
                skills_TextBox.AppendText("\n");
            }
            skills_TextBox.AppendText(
                language_Box.SelectedValue 
                + "-" 
                + level_Box.SelectedValue 
                + "-" 
                + intensity_Box.SelectedValue
                + "-"
                + determineVisitingDays()
                + "-"
                + determineClassTime());
        }

        private void deleteSkill_But_Click(object sender, RoutedEventArgs e)
        {

            skills_TextBox.Document.Blocks.Remove(skills_TextBox.Document.Blocks.LastBlock);
        }

        private void NameBox_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox_FocusChagedHandler(sender, true, "Enter your name here...");
        }

        private void AgeBox_GotFocus(object sender, RoutedEventArgs e)
        {
            textBox_FocusChagedHandler(sender, true, "Enter your age here...");
        }

        private void AgeBox_LostFocus(object sender, RoutedEventArgs e)
        {
            textBox_FocusChagedHandler(sender, false, "Enter your age here...");
        }

        private void enrollStudent_Click(object sender, RoutedEventArgs e)
        {
            nSt = new Student(
                new TextRange(NameBox.Document.ContentStart, NameBox.Document.ContentEnd).Text,
                int.Parse(new TextRange(NameBox.Document.ContentStart, NameBox.Document.ContentEnd).Text),
                skills,
                determineClassTime(),
                determineVisitingDays());
        }

        private VisitingDays determineVisitingDays()
        {
            if ((bool)visiting_days2_radiobtn.IsChecked)
                return VisitingDays.TUE_THU_SAT;
            else
                return VisitingDays.MON_WED_FRI;
        }

        private ClassTime determineClassTime()
        {
            if ((bool)afternoon_course_radiobtn.IsChecked)
            {
                return ClassTime.AFTERNOON;
            }
            else
            {
                if ((bool)evening_course_radiobtn.IsChecked)
                {
                    return ClassTime.EVENING;
                }
                else
                {
                    return ClassTime.MORNING;
                }
            } 
        }
    }
}
