using HMIApp.Components.CSVReader;
using HMIApp.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Drawing;
using System.Windows.Forms;
using HMIApp.Components.UserAdministration;
using HMIApp.Data;
using Microsoft.EntityFrameworkCore;
using Humanizer;
using Microsoft.CodeAnalysis.CSharp.Syntax;


namespace HMIApp
{
    public partial class Form1 : Form
    {

        App App = new App();
        UserAdministration Users = new UserAdministration();
        DataBase DataBase = new DataBase();
        //Services do dependency injection
        ServiceCollection services = new ServiceCollection();
        ServiceProvider serviceProvider;

        //konstruktor Form1
        public Form1()
        {
            InitializeComponent();
            _Form1 = this;
            //zakomentowane do robienia apki bez plc
            //App.RunInitPLC();
            //App.ReadAlarmsFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_2.csv");
            //App.ReadIOFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_3.csv");

            DataBase.Run();
            #region zakomentowane services ktore bylo wczesniej
            ////Services do dependency injection
            //var services = new ServiceCollection();
            //services.AddSingleton<iDataBase, DataBase>();
            ////ZArejestrowanie DBContextu - connection string wrzucic pozniej w jakis plik konfiguracyjny
            //services.AddDbContext<HMIAppDBContext>(options => options
            //.UseSqlServer(Data.ConnectionString));
            //var serviceProvider = services.BuildServiceProvider();
            //var database = serviceProvider.GetService<iDataBase>();
            //database.SaveToDataBase();
            //database.ReadFromDataBase();
            # endregion koniec komentarza services

            //Services do dependency injection
            services.AddSingleton<iDataBase, DataBase>();
            //ZArejestrowanie DBContextu - Stworzenie połączenia do bazy danych i service providera
            services.AddDbContext<HMIAppDBContext>(options => options
            .UseSqlServer(DataBase.ConnectionString));
            serviceProvider = services.BuildServiceProvider();
            ReadFromDbWhenAppIsStarting();
            Users.Run();

            OdczytDB.Enabled = true;
            listBox1.DrawItem += new System.Windows.Forms.DrawItemEventHandler(App.listBox1_DrawItem);
            label63.Text = "";
            Users.EnabledObjects();

            CommaReplaceDotTextBox(this);

        }
        //statyczna zmienna typu Form1 zeby dostac sie z poziomu innej klasy do obiektow wewnatrz Form1
        public static Form1 _Form1;


        //METODA DO ODCZYTU DANYCH Z BAZY przy starcie aplikacji
        public void ReadFromDbWhenAppIsStarting()
        {
            var database = serviceProvider.GetService<iDataBase>();
            database.SelectFromDbToComboBox();
            //Najpierw odczyt z combobox zeby potem moc odczytac z bazy danych pierwszy element na starcie
            if (comboBox5.Items.Count > 0)
            {
                database.SelectFromDataBase(comboBox5.Items[0].ToString());
            }
            else
            {
                // MessageBox.Show("Nie znaleziono referencji");
            }

        }

        //Zapis 
        private void button1_Click(object sender, EventArgs e)
        {
            App.WriteToDB(DB666Tag0.Checked.ToString(), DB666Tag0.Tag.ToString());
            App.WriteToDB(DB666Tag1.Checked.ToString(), DB666Tag1.Tag.ToString());
            App.WriteToDB(DB666Tag8.Checked.ToString(), DB666Tag8.Tag.ToString());
            App.WriteToDB(DB666Tag9.Checked.ToString(), DB666Tag9.Tag.ToString());
            App.WriteToDB(DB666Tag10.Checked.ToString(), DB666Tag10.Tag.ToString());
            App.WriteToDB(DB666Tag11.Checked.ToString(), DB666Tag11.Tag.ToString());
            App.WriteToDB(DB666Tag12.Checked.ToString(), DB666Tag12.Tag.ToString());
            App.WriteToDB(DB666Tag13.Checked.ToString(), DB666Tag13.Tag.ToString());
            App.WriteToDB(DB666Tag2.Text, DB666Tag2.Tag.ToString());
            App.WriteToDB(DB666Tag4.Text, DB666Tag4.Tag.ToString());
            App.WriteToDB(DB666Tag6.Text, DB666Tag6.Tag.ToString());
            App.WriteToDB(DB666Tag16.Text, DB666Tag16.Tag.ToString());
            App.WriteToDB(DB666Tag17.Text, DB666Tag17.Tag.ToString());
            App.WriteToDB(DB666Tag3.Text, DB666Tag3.Tag.ToString());
            App.WriteToDB(DB666Tag5.Text, DB666Tag5.Tag.ToString());
            App.WriteToDB(DB666Tag15.Text, DB666Tag15.Tag.ToString());
            App.WriteToDB(DB666Tag7.Text, DB666Tag7.Tag.ToString());
            App.WriteToDB(DB666Tag14.Text, DB666Tag14.Tag.ToString());
            App.WriteToDB(DB666Tag18.Text, DB666Tag18.Tag.ToString());
            App.WriteToDB(DB666Tag19.Text, DB666Tag19.Tag.ToString());
            App.WriteToDB(DB666Tag20.Text, DB666Tag20.Tag.ToString());
            App.WriteToDB(DB666Tag21.Text, DB666Tag21.Tag.ToString());
            App.WriteToDB(DB666Tag22.Text, DB666Tag22.Tag.ToString());

            var database = serviceProvider.GetService<iDataBase>();
            database.UpdateDb(comboBox5.SelectedItem.ToString());
        }

        //Timer co 100ms do oczytywania danych
        private void timer1_Tick(object sender, EventArgs e)
        {
            //zakomentowane do robienia apki bez plc
            //App.ReadAlarmsFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_2.csv");
            //App.ReadIOFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_3.csv");

            //aktualizacja daty i godziny
            this.Text = DateTime.Now.ToString();
            label57.Text = this.Text;

            //przepisywanie numeru referencji i klienta do wyswietlenia dla operatora
            DB666Tag16PassedValue.Text = DB666Tag16.Text;
            DB666Tag17PassedValue.Text = DB666Tag17.Text;

        }

        //Cofniecie zmian dokonanych w karcie Dane - Modyfikowalne
        private void button2_Click(object sender, EventArgs e)
        {
            App.ReadActualValueFromDB("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_1.csv");
        }

        //Zamkniecie aplikacji
        private void button3_Click(object sender, EventArgs e)
        {
            App.ClosePLCConnection();
            Close();
        }

        //Metoda do zmiany koloru buttona
        int index = 0;
        private void ChangeColorOfButton(Control button)
        {
            if (index == 0)
            {
                button.BackColor = Color.LightGreen;
                index += 1;
            }
            else
            {
                button.BackColor = Color.Transparent;
                index = 0;
            }
        }

        //Testowy przycisk z karty Manual do wyslania komendy
        private void button4_Click(object sender, EventArgs e)
        {
            App.WriteToDB("15", button4.Tag.ToString(), 1);
            ChangeColorOfButton(button4);
        }

        //Testowy przycisk z karty Manual do wyslania komendy
        private void button5_Click(object sender, EventArgs e)
        {
            App.WriteToDB("11", button5.Tag.ToString(), 1);
            ChangeColorOfButton(button5);
        }

        //Przycisk wyzwalajacy zapis uzytkownika
        private void button6_Click(object sender, EventArgs e)
        {
            Users.SaveToXML();
        }

        //Wyczyszczenie statusu karty Użytkownicy
        private void timer2_Tick(object sender, EventArgs e)
        {
            listBox2.Items.Clear();
        }

        //Wyswietlenie uzytkownikow z bazy
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            textBox15.Text = comboBox2.SelectedItem.ToString();
            Users.DisplayValuesFromXML(Users.LoadFromXML("document.xml"), textBox15.Text);

        }

        //Przycisk wyzwalający edycje użytkownika
        private void button7_Click(object sender, EventArgs e)
        {
            Users.EditXML();

        }

        //Przycisk zastepujacy event przyłożenia karty RFID d oczytnika
        private void button9_Click_1(object sender, EventArgs e)
        {
            Users.FindUserinXML();
            Users.EnabledObjects();
        }

        //Wylogowanie uzytkownika po uplywie czasu
        private void timer3_Tick(object sender, EventArgs e)
        {
            Users.ClearUserFromDisplay();
            TimeoutWylogowania.Enabled = false;
            Users.EnabledObjects();
        }

        //Obsluga odliczania czasu do wylogowania
        private void timer4_Tick(object sender, EventArgs e)
        {
            if (TimeoutWylogowania.Enabled)
            {
                Users.Interval -= 1;
                label13.Text = Users.Interval.ToString();
            }
            else
            {
                OdliczaSekunde.Enabled = false;
                Users.Interval = 100000 / 1000;
                label13.Text = Users.Interval.ToString();
            }
        }

        // PRZYCISK WYZWALAJACY ZAPIS
        private void button13_Click(object sender, EventArgs e)
        {
            var database = serviceProvider.GetService<iDataBase>();
            database.InsertToDataBase();
        }
        //Wyrzucenie referencji po rozwinieciu comboboxa
        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            var database = serviceProvider.GetService<iDataBase>();
            database.SelectFromDataBase(comboBox5.Text);
        }
        //Usuwanie wybranej referencji
        private void button2_Click_1(object sender, EventArgs e)
        {
            var database = serviceProvider.GetService<iDataBase>();
            if (comboBox5.Text != null && comboBox5.Text != "")
            {
                database.Delete(comboBox5.Text);
                App.ClearAllValueInForm1("D:\\Projekty C#\\HMIApp\\HMIApp\\HMIApp\\Resources\\Files\\tags_zone_0.csv");
            }
        }

        //Zamiana kropki na przecinek
        private void CommaReplaceDotTextBox(Control control)
        {
            foreach (Control ctrl in control.Controls)
            {
                if (ctrl is TextBox)
                {
                    TextBox textBox = (TextBox)ctrl;
                    textBox.KeyPress += TextBox_KeyPress;
                }
                else if (ctrl.HasChildren)
                {
                    CommaReplaceDotTextBox(ctrl);
                }
            }
        }

        private void TextBox_KeyPress(object sender, KeyPressEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            // Jeśli naciśnięto kropkę, zamień na przecinek
            if (e.KeyChar == '.')
            {
                e.KeyChar = ',';
            }
        }

        private void label81_Click(object sender, EventArgs e)
        {

        }

        private void tabPage5_Click(object sender, EventArgs e)
        {

        }

        private void label41_Click(object sender, EventArgs e)
        {

        }
    }
}
