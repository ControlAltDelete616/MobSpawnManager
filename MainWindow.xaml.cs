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
using System.Configuration;
using System.Data.SqlClient;
using System.Data;

namespace WPFDB_Server
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection sqlConnection;
        public MainWindow()
        {
            InitializeComponent();

            string connectionString = ConfigurationManager.ConnectionStrings["WPFDB_Server.Properties.Settings.SqlServerDataBaseConnectionString"].ConnectionString;
            sqlConnection = new SqlConnection(connectionString);

            ShowLocation();

            ShowBestiarium();

        }

        public void ShowLocation()
        {

            try
            {

                //Query um alle Daten von der Tabelle zu erhalten
                string query = "select * from Zoo";
                //Ist wie ein Interface das ermöglicht, SQL elemente als C# Sharp Objekte zu nutzen
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable locationTable = new DataTable();
                    sqlDataAdapter.Fill(locationTable);

                    //Welche Informationen der Tabelle in unserem DataTable soll in unsere ListBox angezeigt werden
                    ListLocation.DisplayMemberPath = "Location";
                    //Welcher Wert soll gegeben werden wenn unsere Items von der ListBox ausgewählt wird
                    ListLocation.SelectedValuePath = "Id";
                    //DataTable in der ListBox ausgeben und anzeigen lassen
                    ListLocation.ItemsSource = locationTable.DefaultView;

                }

            }
            catch (Exception e)
            {

                MessageBox.Show("Gelöscht!");
            }


        }
        public void ShowMonster()
        {
            if(ListLocation.SelectedValue == null)
            {
                return;
            }
            try
            {

                string query = "select * from Animal a inner join ZooAnimal za on a.Id = za.AnimalID where za.ZooID = @ZooId";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

                using (sqlDataAdapter)
                {

                    sqlCommand.Parameters.AddWithValue("@ZooId", ListLocation.SelectedValue);
                    DataTable monsterTable = new DataTable();
                    sqlDataAdapter.Fill(monsterTable);

                    listMob.DisplayMemberPath = "Name";
                    listMob.SelectedValuePath = "Id";
                    listMob.ItemsSource = monsterTable.DefaultView;

                }

            }
            catch (Exception e)
            {

                MessageBox.Show("bei ShowMonster ist etwas nicht richtig!");
            }


        }


        public void ShowBestiarium()
        {

            try
            {

                string query = "select * from Animal";
                SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(query, sqlConnection);

                using (sqlDataAdapter)
                {
                    DataTable bestiariumTable = new DataTable();
                    sqlDataAdapter.Fill(bestiariumTable);

                    bestiariumList.DisplayMemberPath = "Name";
                    bestiariumList.SelectedValuePath = "Id";
                    bestiariumList.ItemsSource = bestiariumTable.DefaultView;

                }

            }
            catch (Exception e)
            {

                MessageBox.Show("Gelöscht!");
            }
        }

            private void ListLocation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowMonster();
            ShowSelectedLocationinTextBox();
            
        }

        private void deleteLocation_Click(object sender, RoutedEventArgs e)
        {
            //Location löschen Methode
            try
            {
                string query = "delete from Zoo where id = @ZooId";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooId", ListLocation.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Beim löschen der Location ist ein Fehler aufgetreten");

            }
            finally
            {
                sqlConnection.Close();
                ShowLocation();
            }

        }

        private void removeMonster_Click(object sender, RoutedEventArgs e)
        {         

            try
            {

                string query = "DELETE FROM ZooAnimal WHERE ZooID = @ZooID AND AnimalID = @AnimalID";

                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);



                sqlConnection.Open();

                sqlCommand.Parameters.AddWithValue("@ZooId", ListLocation.SelectedValue);

                sqlCommand.Parameters.AddWithValue("@AnimalId", listMob.SelectedValue);

                sqlCommand.ExecuteScalar();

            }
            catch (Exception erafz)
            {

                MessageBox.Show("Beim löschen des Monsters aus der Location ist etwas schief gegangen!");

            }
            finally
            {

                sqlConnection.Close();

                ShowMonster();

            }
            }

        private void addLocation_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Zoo values (@Location)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception)
            {

                MessageBox.Show("Fehler beim hinzufügen!");
            }
            finally
            {
                sqlConnection.Close();
                ShowLocation();
            }

        }

        private void addMonster_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "insert into Animal values (@Name)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception)
            {

                MessageBox.Show("Fehler beim hinzufügen eines Monsters zum Bestiarium!");
            }
            finally
            {
                sqlConnection.Close();
                ShowBestiarium();
            }
        }


        private void ShowSelectedMonsterinTextBox()
        {
            string query = "select Name from Animal where Id = @AnimalID";
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);

            using (sqlDataAdapter)
            {
                sqlCommand.Parameters.AddWithValue("@AnimalID", bestiariumList.SelectedValue);
                DataTable ZooDataTable = new DataTable();
                sqlDataAdapter.Fill(ZooDataTable);

                myTextBox.Text = ZooDataTable.Rows[0]["Name"].ToString();
            }

        }


        private void ShowSelectedLocationinTextBox()
        {
            string query = "select location from Zoo where Id = @ZooId";
            SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
            SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
            
            using (sqlDataAdapter)
            {
                sqlCommand.Parameters.AddWithValue("@ZooId", ListLocation.SelectedValue);
                DataTable ZooDataTable = new DataTable();
                sqlDataAdapter.Fill(ZooDataTable);

                myTextBox.Text = ZooDataTable.Rows[0]["Location"].ToString();
            }

        }

        private void updateLocation_Click(object sender, RoutedEventArgs e)
        {
            if (ListLocation.SelectedValue == null || myTextBox.Text == null)
            {
                return;
            }
                try
                {

                    string query = "update Zoo Set Location = @Location where Id = @ZooId";
                    SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                    sqlConnection.Open();
                    sqlCommand.Parameters.AddWithValue("@ZooId", ListLocation.SelectedValue);
                    sqlCommand.Parameters.AddWithValue("@Location", myTextBox.Text);
                    sqlCommand.ExecuteScalar();

                }
                catch (Exception)
                {
                    MessageBox.Show("Fehler beim aktualisieren der Location");

                }
                finally
                {
                    sqlConnection.Close();
                    ShowLocation();

                }
        }


        private void updateMonster_Click(object sender, RoutedEventArgs e)
        {
            if (bestiariumList.SelectedValue == null || myTextBox.Text == null)
            {
                return;
            }
            try
            {

                string query = "update Animal Set Name = @Name where Id = @AnimalID";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalID", bestiariumList.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@Name", myTextBox.Text);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception)
            {
                MessageBox.Show("Fehler beim aktualisieren der Monster");

            }
            finally
            {
                sqlConnection.Close();
                ShowBestiarium();

            }
        }

        private void addMonsterToLocation_Click(object sender, RoutedEventArgs e)
        {
            if(ListLocation.SelectedValue == null || bestiariumList.SelectedValue == null)
            {
                return;
            }
            try
            {

                string query = "insert into ZooAnimal values (@ZooID, @AnimalID)";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@ZooID", ListLocation.SelectedValue);
                sqlCommand.Parameters.AddWithValue("@AnimalID", bestiariumList.SelectedValue);
                sqlCommand.ExecuteScalar();

            }
            catch (Exception)
            {
                MessageBox.Show("Fehler beim hinzufügen eines Monsters in eine Location");

            }
            finally
            {
                sqlConnection.Close();
                ShowMonster();

            }

        }

        private void deleteMonster_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                string query = "delete from Animal where id = @AnimalID";
                SqlCommand sqlCommand = new SqlCommand(query, sqlConnection);
                sqlConnection.Open();
                sqlCommand.Parameters.AddWithValue("@AnimalID", bestiariumList.SelectedValue);
                sqlCommand.ExecuteScalar();
            }
            catch (Exception )
            {
                MessageBox.Show("Beim löschen des Monsters ist etwas schief gelauffen");

            }
            finally
            {
                sqlConnection.Close();
                ShowBestiarium();
                ShowMonster();
            }
        }

        private void bestiariumList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ShowSelectedMonsterinTextBox();
        }
    }
}
