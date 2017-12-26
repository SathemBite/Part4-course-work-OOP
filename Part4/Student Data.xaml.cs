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
using System.Text.RegularExpressions;

namespace Part4
{


    

    /// <summary>
    /// Логика взаимодействия для Student_Data.xaml
    /// </summary>
    public partial class Student_Data : Window
    {
        public Student nSt;
        private Dictionary<Language, CourseInf> courses;
        public Student_Data()
        {
            InitializeComponent();
            courses = new Dictionary<Language, CourseInf>();
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
            if (courses.ContainsKey((Language)language_Box.SelectedValue))
            {
                MessageBox.Show("Course with the specified language is exist!!!");
                return;
            }

            Language lang = (Language)language_Box.SelectedValue;
            Level level = (Level)level_Box.SelectedValue;
            Intensity inten = (Intensity)intensity_Box.SelectedValue;
            LinkedList<DayOfWeek> visD = determineVisitingDays();
            bool isGroup = isGroupClasses();

            if (!isGroup)
            {
                courses.Add(lang,
                new CourseInf(
                inten,
                level,
                visD,
                Courses.getTwoWeekCost(lang, inten, isGroupClasses(), visD.Count),
                isGroup,
                false));
            }
            else
            {
                if (Courses.isCanFormGroup(visD, lang, level, inten))
                {
                    courses.Add(lang,
                        new CourseInf(
                            inten,
                            level,
                            visD,
                            Courses.getTwoWeekCost(lang, inten, isGroupClasses(), visD.Count),
                            isGroup,
                            true));
                }
                else
                {
                    MessageBoxResult mbRes = MessageBox
                        .Show("Group with the specified parameters don't exist.\n       We can enroll you on individual classes.",
                        "Confirm individual classes",
                        MessageBoxButton.YesNo);
                    if (mbRes == MessageBoxResult.Yes)
                    {
                        isGroup = !isGroup;
                        courses.Add(lang,
                        new CourseInf(
                            inten,
                            level,
                            visD,
                            Courses.getTwoWeekCost(lang, inten, isGroupClasses(), visD.Count),
                            isGroup,
                            true));
                    }
                    else
                    {
                        return;
                    }
                }
            }

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
                + (isGroup ? "Group" : "Individual")
                + "-"
                + String.Join(" ", determineVisitingDays()));
        }

        private void deleteSkill_But_Click(object sender, RoutedEventArgs e)
        {
            courses.Remove((Language)language_Box.SelectedValue);
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
            String name = getText(NameBox);
            String age = getText(AgeBox);
            Regex nameReg = new Regex(@"^[A-Za-z\\-\\ ]+?$");
            Regex ageReg = new Regex(@"^([7-9])|([1-9]\d{0,2}\w?)$");

            if (!nameReg.IsMatch(name))
            {
                MessageBox.Show("Name must include only character symbols!");
                return;
            }
            if (!ageReg.IsMatch(age))
            {
                MessageBox.Show("Age must be grand then 7 years and include 1 - 3 Integer digit!");
                return;
            }

            if (courses.Count == 0)
            {
                MessageBox.Show("You didn't choose any course, please choose one or more");
                return;
            }


            nSt = new Student(
                new TextRange(NameBox.Document.ContentStart, NameBox.Document.ContentEnd).Text,
                int.Parse(new TextRange(AgeBox.Document.ContentStart, AgeBox.Document.ContentEnd).Text),
                courses);
            Close();
        }

        private LinkedList<DayOfWeek> determineVisitingDays()
        {
            LinkedList<DayOfWeek> visDays = new LinkedList<DayOfWeek>();
            if (tabs_groupAndIndividual.SelectedIndex == 0)
            {
                if ((bool)visiting_days1_radiobtn.IsChecked)
                {
                    visDays.AddLast(DayOfWeek.MON);
                    visDays.AddLast(DayOfWeek.WED);
                    visDays.AddLast(DayOfWeek.FRI);
                }
                else
                {
                    visDays.AddLast(DayOfWeek.TUE);
                    visDays.AddLast(DayOfWeek.THU);
                }
            }
            else
            {
                CheckBox[] checkBoxes = { chB_mon, chB_tue, chB_wed, chB_thu, chB_fri};
                foreach (CheckBox chB in checkBoxes)
                {
                    if ((bool)chB.IsChecked)
                    {
                        visDays.AddLast((DayOfWeek)Enum.Parse(typeof(DayOfWeek), (string)chB.Content));
                    }
                }
            }

            return visDays;
        }

        private bool isGroupClasses()
        {
            if ((tabs_groupAndIndividual.SelectedItem as TabItem).Header.Equals("Individual"))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private void button_cancelEnrolling_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
