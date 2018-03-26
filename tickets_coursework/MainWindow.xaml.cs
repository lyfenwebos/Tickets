using System;
using System.Collections.Generic;
using System.Data;
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

using MySql.Data.MySqlClient;
using System.Text.RegularExpressions;


namespace tickets_coursework
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public double price;
        public int stage = 2;
        public int stage2 = 2;
        public int chosenMovie;
        public int backStage1 = 1;
        string sale = "";
        public int ticketsAmount;
        public int sessionID;
        DataBase db = new DataBase();
        DataSet ds = new DataSet();

        public MainWindow()
        {
            InitializeComponent();

            amountLabel.Visibility = amount_Box.Visibility = salesGB.Visibility = totalPriceBox.Visibility = totalPriceLabel.Visibility =  Visibility.Hidden;
            tab2.IsEnabled = tab3.IsEnabled = tab4.IsEnabled = false;
            infLabel.Visibility = Visibility.Hidden;

            ds = db.PopulateMovies();
            movie_cb.ItemsSource = ds.Tables[0].DefaultView;
            movie_cb.DisplayMemberPath = ds.Tables[0].Columns["movieName"].ToString();
            movie_cb.SelectedValuePath = ds.Tables[0].Columns["movieID"].ToString();
        }

        private void movie_cb_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            int ID = Convert.ToInt32(movie_cb.SelectedValue);
            priceBox.Text = db.GetPrice(ID).ToString();
            price = Convert.ToInt32(priceBox.Text);
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
        //-------------------------tab1-----------------------------------

        private void amount_Box_TextChanged(object sender, TextChangedEventArgs e)
        {

            try
            {

                if (Convert.ToInt32(amount_Box.Text) <= 30)
                {
                    int amount = Convert.ToInt32(amount_Box.Text);
                    stageButton1.IsEnabled = true;
                    if (amount > 1)
                    {
                        double total = amount * price;
                        priceBox.Text = total.ToString();
                    }
                    else if (amount == 1)
                    {
                        priceBox.Text = price.ToString();
                    }
                }
                else
                {
                    amount_Box.Text = "30";
                    stageButton1.IsEnabled = false;
                }
            }
            catch
            {
                priceBox.Text = price.ToString();
            }
            
        }

        private void stageButton1_Click(object sender, RoutedEventArgs e)
        {
            if (movie_cb.SelectedIndex == -1)
            {
                MessageBox.Show("You haven't choosen movie you would like to watch. Please try again.", "Error", MessageBoxButton.OK, MessageBoxImage.Error, MessageBoxResult.OK);
            }
            else
            {
                switch (stage)
                {
                    case 1:
                        movie_cb.IsEnabled = true;
                        stageButton1.Margin = new Thickness(155, 70, 0, 0);
                        stage++;
                        break;

                    case 2:
                        amountLabel.Visibility = amount_Box.Visibility = Visibility.Visible;
                        movie_cb.IsEnabled = false;
                        chosenMovie = Convert.ToInt32(movie_cb.SelectedValue);
                        stageButton1.Margin = new Thickness(155, 131, 0, 0);
                        stage++;
                        break;

                    case 3:
                        price = Convert.ToDouble(priceBox.Text);
                        ticketsAmount = Convert.ToInt32(amount_Box.Text);
                        amountLabel.Visibility = amount_Box.Visibility = Visibility.Visible;
                        salesGB.Visibility = totalPriceBox.Visibility = totalPriceLabel.Visibility = Visibility.Visible;
                        noSale.IsChecked = true;
                        amount_Box.IsEnabled = false;
                        stageButton1.Margin = new Thickness(155, 241, 0, 0);
                        stage++;
                        break;

                    case 4:
                        Pages.SelectedIndex = 2;
                        Pages.SelectedItem = tab2;
                        tab2.IsSelected = true;
                        tab1.IsEnabled = false;
                        price = Convert.ToDouble(totalPriceBox.Text);
                        ds = db.GetSessionDate(chosenMovie);
                        
                        date_cb.ItemsSource = ds.Tables[0].DefaultView;
                        date_cb.DisplayMemberPath = ds.Tables[0].Columns["session_date"].ToString();
                        date_cb.SelectedValuePath = ds.Tables[0].Columns["sessionID"].ToString();
                        break;
                }
            }
        }

        //----------------------sales------------------------------------  
        


        private void kidsSale_Checked(object sender, RoutedEventArgs e)
        {
            totalPriceBox.Text = Convert.ToString(price - (price * 0.15));
            sale = "Kids 15%";
            
        }

        private void studentsSale_Checked(object sender, RoutedEventArgs e)
        {
            totalPriceBox.Text = Convert.ToString(price - (price * 0.2));
            sale = "Students 20%";
        }

        private void birthdaySale_Checked(object sender, RoutedEventArgs e)
        {
            totalPriceBox.Text = Convert.ToString(price - (price * 0.9));
            sale = "Birthday 90%";
        }

        private void noSale_Checked(object sender, RoutedEventArgs e)
        {
            totalPriceBox.Text = price.ToString();
        }


        //-------------------------tab2-----------------------------------

        private void stageButton2_Click(object sender, RoutedEventArgs e)
        {
            switch (stage2) {
                case 2:
                    sessionID = Convert.ToInt32(date_cb.SelectedValue);
                    ds = db.GetSessionTime(chosenMovie, date_cb.Text, ticketsAmount);
                    time_cb.ItemsSource = ds.Tables[0].DefaultView;
                    time_cb.DisplayMemberPath = ds.Tables[0].Columns["session_time"].ToString();
                    time_cb.SelectedValuePath = ds.Tables[0].Columns["sessionID"].ToString();

                    stageButton2.Margin = new Thickness(219, 45, 0, 0);
                    time_cb.IsEnabled = true;
                    date_cb.IsEnabled = false;

                    stage2++;
                    break;
                case 3:
                    infLabel.Visibility = Visibility.Visible;
                    stageButton2.Margin = new Thickness(220,260, 0, 0);
                    time_cb.IsEnabled = false;

                    if (sale == "")
                    {
                        sessInfo.Text = "Movie name: " + movie_cb.Text + "." + Environment.NewLine + " Amount of tickets: " + amount_Box.Text + ". Date and time of your session: " + date_cb.Text + ", " + time_cb.Text + ". Enjoy";
                    }
                    else
                    {
                        sessInfo.Text = "Movie name: " + movie_cb.Text + "." + Environment.NewLine + " Amount of tickets: " + amount_Box.Text + ". Your sale: " + sale + ". Date and time of your session: " + date_cb.Text + ", " + time_cb.Text + ". Enjoy";
                    }
                    stage2++;
                    break;
                case 4:
                    Pages.SelectedIndex = 3;
                    Pages.SelectedItem = tab3;
                    tab2.IsEnabled = false;
                    tab3.IsSelected = true;
                    totalSumLabel.Content = price;
                    break;  
        }
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            switch (backStage1)
            {
                case 1:
                    time_cb.IsEnabled = true;
                    stageButton2.Margin = new Thickness(260, 95, 0, 0);
                    backStage1++;
                    break;
                case 2:
                    tab2.IsEnabled = false;
                    tab1.IsEnabled = true;
                    tab1.IsSelected = true;
                    break;     
            }

        }

        private void cardNumber_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (cardNumber.Text.StartsWith("4"))
            {
                var uriSource = new Uri(@"Resources/visa.png", UriKind.Relative);
                card.Source = new BitmapImage(uriSource);
                cardNumber.MaxLength = 13;
            }
            else if (cardNumber.Text.StartsWith("5"))
            {
                var uriSource = new Uri(@"Resources/mastercard.png", UriKind.Relative);
                card.Source = new BitmapImage(uriSource);
                cardNumber.MaxLength = 16;
            }
            else
            {
                card.Source = null;
            }
        }

        private void monthBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (monthBox.Text == "MM")
            {
                monthBox.Text = "";
            }
        }

        private void yearBox_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (yearBox.Text == "YY")
            {
                yearBox.Text = "";
            }
        }

        private void monthBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (monthBox.Text.Length == 2 && monthBox.Text!="MM")
            {
                yearBox.Focus();
                yearBox.Text = "";
            }
        }

        private void endButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (cardNumber.Text == "" )
            {
                cardNumber.Background = Brushes.Red;
                MessageBox.Show("You haven't entered card number!", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK);
            } else
            {
                cardNumber.Background = Brushes.WhiteSmoke;
            }
            if ( nameBox.Text == "")
            {
                nameBox.Background = Brushes.Red;
                MessageBox.Show("You haven't entered name!", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK);
            } else
            {
                nameBox.Background = Brushes.WhiteSmoke;
            }
           if (monthBox.Text == "" || yearBox.Text == "")
            {
                monthBox.Background = Brushes.Red;
                yearBox.Background = Brushes.Red;
                MessageBox.Show("You haven't entered the date!", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK);
            } else
            {
                monthBox.Background = Brushes.WhiteSmoke;
                yearBox.Background = Brushes.WhiteSmoke;
            }

            if (email.Text == "")
            {
                email.Background = Brushes.Red;
                MessageBox.Show("You haven't entered your email adress!", "Error", MessageBoxButton.OK, MessageBoxImage.Asterisk, MessageBoxResult.OK);
            }
            else
            {
                email.Background = Brushes.WhiteSmoke;
            }

            Payment pay = new Payment();
            pay.Reservation(sessionID, ticketsAmount);

            UnicCode code = new UnicCode();
            code.GenerateMessage(sessInfo.Text, email.Text, sessionID);

            tab3.IsEnabled = false;
            tab4.IsEnabled = true;
            tab4.IsSelected = true;

        }
    }
}
