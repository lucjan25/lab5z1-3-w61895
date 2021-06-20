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
using System.Reflection;
using System.Xml.Serialization;
using Microsoft.Win32;

namespace lab5z1
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public List<Student> ListaStudentow { get; set; }
        public MainWindow()
        {
            ListaStudentow = new List<Student>()
            {
                new Student(){ imie="Jan", nazwisko="Kowalski", NrIndeksu=1234, wydzial="KIS"},
                new Student(){ imie="Anna", nazwisko="Nowak", NrIndeksu=4321, wydzial="KIS"},
                new Student(){ imie="Michał", nazwisko="Jacek", NrIndeksu=34567, wydzial="KIS"}
            };

            InitializeComponent();

            dgStudents.Columns.Add(new DataGridTextColumn() { Header = "Imię", Binding = new Binding("imie") });
            dgStudents.Columns.Add(new DataGridTextColumn() { Header = "Nazwisko", Binding = new Binding("nazwisko") });
            dgStudents.Columns.Add(new DataGridTextColumn() { Header = "Nr indeksu", Binding = new Binding("NrIndeksu") });
            dgStudents.Columns.Add(new DataGridTextColumn() { Header = "Wydział", Binding = new Binding("wydzial") });

            dgStudents.AutoGenerateColumns = false;
            dgStudents.ItemsSource = ListaStudentow;
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new StudentWindow();
            if (dialog.ShowDialog() == true)
            {
                ListaStudentow.Add(dialog.student);
                dgStudents.Items.Refresh();
            }
        }

        private void btnRemove_Click(object sender, RoutedEventArgs e)
        {
            if (dgStudents.SelectedItem is Student)
                ListaStudentow.Remove((Student)dgStudents.SelectedItem);
            dgStudents.Items.Refresh();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "data.txt";
                saveFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == true)
                    using (StreamWriter sw = new StreamWriter(saveFileDialog.FileName))
                        foreach (Student student in ListaStudentow)
                        {
                            Save<Student>(student, sw);
                        };
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception occured", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Zapis do txt udany.", "Komunikat", MessageBoxButton.OK);

        }

        private void btnLoad_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.FileName = "data.txt";
                openFileDialog.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    ListaStudentow.Clear();
                    using (StreamReader sr = new StreamReader(openFileDialog.FileName))
                        while (!sr.EndOfStream)
                        {
                            Student student = Load<Student>(sr);
                            ListaStudentow.Add(student);
                        }
                    dgStudents.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception occured", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Odczyt z txt udany.", "Komunikat", MessageBoxButton.OK);
        }

        public void Save<T>(T ob, StreamWriter sw)
        {
            Type t = ob.GetType();
            sw.WriteLine($"[[{t.FullName}]]");
            foreach (var p in t.GetProperties())
            {
                sw.WriteLine($"[{p.Name}]");
                sw.WriteLine(p.GetValue(ob));
            }
            sw.WriteLine($"[[]]");
        }

        public T Load<T>(StreamReader sr) where T : new()
        {
            T ob = default(T);
            Type tob = null;
            PropertyInfo property = null;

            while (!sr.EndOfStream)
            {
                var ln = sr.ReadLine();
                if (ln == "[[]]")
                    return ob;
                else if (ln.StartsWith("[["))
                {
                    tob = Type.GetType(ln.Trim('[', ']'));
                    if (typeof(T).IsAssignableFrom(tob))
                        ob = (T)Activator.CreateInstance(tob);
                }
                else if (ln.StartsWith("[") && ob != null)
                    property = tob.GetProperty(ln.Trim('[', ']'));
                else if (ob != null && property != null)
                    property.SetValue(ob, Convert.ChangeType(ln, property.PropertyType));
            }
            return default(T);
        }

        private void btnSaveToXml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.FileName = "data.xml";
                saveFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

                if (saveFileDialog.ShowDialog() == true)
                {
                    FileStream fs = new FileStream(saveFileDialog.FileName, FileMode.Create);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Student>));
                    serializer.Serialize(fs, ListaStudentow);
                    fs.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception occured", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Zapis do Xml udany.", "Komunikat", MessageBoxButton.OK);
        }

        private void btnLoadFromXml_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                List<Student> ls;
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.FileName = "data.xml";
                openFileDialog.Filter = "XML files (*.xml)|*.xml|All files (*.*)|*.*";

                if (openFileDialog.ShowDialog() == true)
                {
                    FileStream fs = new FileStream(openFileDialog.FileName, FileMode.Open);
                    XmlSerializer serializer = new XmlSerializer(typeof(List<Student>));
                    ls = (List<Student>)serializer.Deserialize(fs);
                    fs.Close();
                    ListaStudentow.Clear();
                    foreach (Student student in ls)
                    {
                        ListaStudentow.Add(student);
                    }
                    dgStudents.Items.Refresh();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Exception occured", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            MessageBox.Show("Odczyt z Xml udany.", "Komunikat", MessageBoxButton.OK);
        }
    }
}
