using Newtonsoft.Json;
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
using System.Net.Http;
using Newtonsoft.Json;
using System.Threading;
using System.Globalization;
using mathiez_benoit_Meteo2.Entity;
using System.Configuration;

namespace mathiez_benoit_Meteo2
{

    public partial class MainWindow : Window
    {
        public List<string> mesVillesList = new List<string>();

        public MainWindow()
        {
            InitializeComponent();
            InitialiserVilles();
            Lab_Auj.Background = Brushes.CornflowerBlue;
            _ = Aujourdhui(); // Utilisation de "_" pour ignorer la tâche retournée par la méthode asynchrone
        }

        public void InitialiserVilles()
        {
            string Mesvilles_ = ConfigurationManager.AppSettings["MesVilles"];
            string[] MesVilles = Mesvilles_.Split(',');
            foreach (string ville in MesVilles)
            {
                mesVillesList.Add(ville);
            }
            Cb_Villes.ItemsSource = mesVillesList;
            int index = mesVillesList.Count - 1;
            Cb_Villes.SelectedItem = mesVillesList[index];
        }
        public async Task<Root> GetMeteo()
        {
            string ville = Cb_Villes.SelectedItem.ToString();
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync($"https://www.prevision-meteo.ch/services/json/{ville}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                Root root = JsonConvert.DeserializeObject<Root>(content);
                return root;
            }
            else
            {
                return null;
            }
        }


        public async Task Aujourdhui()
        {
            Root root = await GetMeteo();
            CurrentCondition current = root.current_condition;
            CityInfo city = root.city_info;
            Lab_Pays.Content = city.country;
            Lab_Jour.Content = current.date;
            Lab_Temp.Content = current.tmp.ToString() + "°";
            Lab_Condition.Content = current.condition;


            string dateString = current.date;
            DateTime date = DateTime.ParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            string nomJourSemaine = date.ToString("dddd", new CultureInfo("fr-FR"));
            string output = nomJourSemaine.First().ToString().ToUpper() + nomJourSemaine.Substring(1).ToLower();
            Lab_JourS.Content = output;

            string icone = current.icon_big;
            BitmapImage image = new BitmapImage(new Uri(icone, UriKind.Absolute));
            Img_CentralImage.Source = image;
            Img_Auj.Source = image;
            FcstDay1 day1 = root.fcst_day_1;
            BitmapImage image2 = new BitmapImage(new Uri(day1.icon, UriKind.Absolute));
            Img_Demain.Source = image2;
            FcstDay2 day2 = root.fcst_day_2;
            BitmapImage image3 = new BitmapImage(new Uri(day2.icon, UriKind.Absolute));
            Img_Apres.Source = image3;


            Button button1 = Lab_Auj;
            button1.Background = Brushes.CornflowerBlue; // Changer la couleur du fond du bouton en gris
            Button button2 = Lab_Apres;
            Button button3 = Lab_Demain;
            button2.Background = Brushes.White;
            button3.Background = Brushes.White;
        }



        public async Task Demain()
        {
            Root root = await GetMeteo();
            FcstDay1 day1 = root.fcst_day_1;
            string temp = day1.tmin.ToString() + "°" + " / " + day1.tmax.ToString() + "°";
            Lab_Temp.Content = temp;
            Lab_Condition.Content = day1.condition;
            Img_CentralImage.Source = new BitmapImage(new Uri(day1.icon_big, UriKind.Absolute));
            Lab_Jour.Content = day1.date;
            string dateString = day1.date;
            DateTime date = DateTime.ParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            string nomJourSemaine = date.ToString("dddd", new CultureInfo("fr-FR"));
            string output = nomJourSemaine.First().ToString().ToUpper() + nomJourSemaine.Substring(1).ToLower();
            Lab_JourS.Content = output;
        }


        public async Task ApresDemain()
        {
            Root root = await GetMeteo();






            FcstDay2 day2 = root.fcst_day_2;
            string temp = day2.tmin.ToString() + "°" + " / " + day2.tmax.ToString() + "°";
            Lab_Temp.Content = temp;
            Lab_Condition.Content = day2.condition;
            Img_CentralImage.Source = new BitmapImage(new Uri(day2.icon_big, UriKind.Absolute));
            Lab_Jour.Content = day2.date;

            string dateString = day2.date;
            DateTime date = DateTime.ParseExact(dateString, "dd.MM.yyyy", CultureInfo.InvariantCulture);
            string nomJourSemaine = date.ToString("dddd", new CultureInfo("fr-FR"));
            string output = nomJourSemaine.First().ToString().ToUpper() + nomJourSemaine.Substring(1).ToLower();
            Lab_JourS.Content = output;
        }

        private void Lab_Demain_Click(object sender, RoutedEventArgs e)
        {
            _ = Demain();
            updateBtnBg((Button)sender);
        }

        private void Lab_Auj_Click(object sender, RoutedEventArgs e)
        {
            _ = Aujourdhui();
            updateBtnBg((Button)sender);
        }

        private void Lab_Apres_Click(object sender, RoutedEventArgs e)
        {
            _ = ApresDemain();
            updateBtnBg((Button)sender);
        }

        public void updateBtnBg(Button button)
        {
            Button button1 = Lab_Auj;
            Button button2 = Lab_Apres;
            Button button3 = Lab_Demain;
            button1.Background = Brushes.White;
            button2.Background = Brushes.White;
            button3.Background = Brushes.White;
            button.Background = Brushes.CornflowerBlue; // Changer la couleur du fond du bouton en gris

        }

        public async Task TesterVille()
        {
            string ville = Tb_VilleRecherche.Text;
            HttpClient httpClient = new HttpClient();
            HttpResponseMessage response = await httpClient.GetAsync($"https://www.prevision-meteo.ch/services/json/{ville}");
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                if (content.Contains("error"))
                {
                    MessageBox.Show("Ville non trouvée, vérifiez l'orthographe");
                }
                else
                {
                    if (Cb_Villes.Items.Contains(ville))
                    {
                        MessageBox.Show("Ville déjà ajoutée");
                    }
                    else
                    {
                        MessageBox.Show("Ville ajoutée");
                        // Charge la configuration actuelle à partir du fichier App.config
                        Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                        // Modifie la valeur de la clé
                        config.AppSettings.Settings["Mesvilles"].Value += $",{ville}";

                        // Enregistre les modifications
                        config.Save(ConfigurationSaveMode.Modified);

                        // Force le rechargement de la configuration
                        ConfigurationManager.RefreshSection("appSettings"); mesVillesList.Add(ville);
                        Cb_Villes.Items.Refresh();                        
                        Cb_Villes.SelectedItem = ville;
                    }
                }
            }
            else
            {
                MessageBox.Show("Ville non trouvée, vérifiez l'orthographe");

            }
        }

        private void Cb_Villes_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            _: Aujourdhui();
        }

        private void Cb_Villes_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            _: Aujourdhui();
        }

        private void Tb_VilleRecherche_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                _ = TesterVille();
            }
        }
    }


   

}