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
using System.Data;
using System.Data.SqlClient;
using System.Collections.ObjectModel;
using System.Data.Common;
using System.Text.RegularExpressions;

namespace SQL_Interface_Test
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// Cock and balls
    public partial class MainWindow : Window
    {
        public static string tableString = "obj2";
        int nextID;
        public static string consoleMessage;
        public static string consolePrev;

        public MainWindow()
        {
            InitializeComponent();
            _classes.clsDB.CurrentConnection.OpenConnection();
            Refresh();

            InitTable();
            //b();

            //_classes.clsDB.CurrentConnection.CloseConnection();
        }

        public void ChangeTbl(string tblStr)
        {
            tableString = tblStr;
        }

        public void ChangeDB(string dbStr)
        {
            _classes.clsDB.nm_DB_Current = dbStr;
        }


        public void b()
        {
            DataTable table = null;

            try
            {
                table = _classes.clsDB.Get_DataTable(tableString);
            }
            catch (Exception e)
            {
                _classes.clsDB.Execute_SQL("CREATE TABLE " + tableString + " (ID int, str varchar(100));");
                table = _classes.clsDB.Get_DataTable(tableString);
            }

            Grunk grunk = new Grunk();
            grunk.grunkint = 4;
            grunk.grunkstr = "three";

            try
            {
                _classes.clsDB.Get_DataTable(tableString);
            }
            catch
            {
                _classes.clsDB.Execute_SQL("CREATE TABLE " + tableString + " (ID int, str varchar(100));");
            }
            finally
            {
                _classes.clsDB.Execute_SQL("INSERT INTO " + tableString + " VALUES (" + grunk.grunkint + ", " + "'" + grunk.grunkstr + "'" + ");");
            }
        }

        public class Grunk
        {
            public int grunkint;
            public string grunkstr;
        }

        private void InitTable()
        {
            DataTable table = null;

            try
            {
                table = _classes.clsDB.Get_DataTable(tableString);

                if (table == null)
                    throw new Exception();
            }
            catch (Exception e)
            {
                _classes.clsDB.Execute_SQL("CREATE TABLE " + tableString + " (ID int, str varchar(100))");
                table = _classes.clsDB.Get_DataTable("SELECT * FROM " + tableString);
            }
        }

        private void Submit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                _classes.clsDB.Execute_SQL("INSERT INTO " + tableString + " VALUES (" + nextID + ", '" + Name_Box.Text + "');");
            }
            catch (Exception f)
            {
                UpdateMessage(f.Message);
            }

            Refresh();
        }

        public void Refresh()
        {
            Name_Box.Text = null;

            try
            {
                if (_classes.clsDB.Get_DataTable(tableString) == null)
                    throw new Exception();
            }
            catch (Exception e)
            {
                _classes.clsDB.Execute_SQL("CREATE TABLE " + tableString + " (ID int, str varchar(100));");
            }

            nextID = _classes.clsDB.Get_Last_ID(tableString) + 1;

            ID_Label.Content = nextID.ToString();

            DB_Selection.Text = _classes.clsDB.nm_DB_Current;

            Table_Selection.Text = tableString;

            Console_Output.Content = consoleMessage == consolePrev ? "" : consoleMessage;
        }

        /// TODO:
        /// Handle error when invalid DB name used - Exception in Get_DB_Connection();
        /// System tables - retrieve tables
        /// AutoIncrement ID

        private void DB_Change_Click(object sender, RoutedEventArgs e)
        {
            ChangeDB(DB_Selection.Text);
            Refresh();
        }

        private void Table_Change_Click(object sender, RoutedEventArgs e)
        {
            Regex regex = new Regex(@"^[\p{L}_][\p{L}\p{N}@$#_]{0,127}$");

            if (regex.Match(Table_Selection.Text) == null)
            {
                try
                {
                    ChangeTbl(Table_Selection.Text);
              
                    //throw new Exception("ERROR: Table could not be changed!");
                }
                catch (Exception f)
                {
                    UpdateMessage(f.Message);
                }
            }
            else
            {
                UpdateMessage("Invalid Table Name!");
            }

            Refresh();
        }

        public static void UpdateMessage(string message)
        {
            consolePrev = consoleMessage;
            consoleMessage = message;
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ChangeServer(Server_Selection.SelectedItem.ToString());
            Refresh();
        }
        public void ChangeServer(string server)
        {
            const string prefix = "System.Windows.Controls.ComboBoxItem: ";

            switch (server)
            {
                case prefix + "MySQL":
                    {
                        _classes.clsDB.CurrentConnection.connectionString = _classes.clsDB.PathDB_MySQL.connectionString;
                        break;
                    }
                case prefix + "Local":
                    {
                        _classes.clsDB.CurrentConnection.connectionString = _classes.clsDB.PathDB_Local.connectionString;
                        break;
                    }
                case prefix + "VS":
                    {
                        _classes.clsDB.CurrentConnection.connectionString = _classes.clsDB.PathDB_VS.connectionString;
                        break;
                    }
                default:
                    {
                        _classes.clsDB.CurrentConnection.connectionString = null;
                        break;
                    }
            }
        }
    }
}