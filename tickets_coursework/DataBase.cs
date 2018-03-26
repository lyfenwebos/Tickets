using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MySql.Data.MySqlClient;

namespace tickets_coursework
{
    public class DataBase
    {
        public string connectionString = Properties.Settings.Default.connectionDB;

        public int GetPrice(int ID)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            conn.Open();
            try
            {
                cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT moviePrice FROM Movies WHERE movieID =@ID";
                cmd.Parameters.AddWithValue("@ID", ID);
                int temp = Convert.ToInt32(cmd.ExecuteScalar());
                conn.Close();
                return temp;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }
        public DataSet PopulateMovies()
        {

            MySqlConnection conn = new MySqlConnection(Properties.Settings.Default.connectionDB);
            try
            {
                MySqlDataAdapter da = new MySqlDataAdapter("Select movieID,movieName FROM Movies", conn);
                DataSet ds = new DataSet();
                da.Fill(ds, "Movies");
                return ds;
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public DataSet GetSessionDate(int movieID)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlDataAdapter da;
            DataSet ds;

            da = new MySqlDataAdapter("SELECT sessionID, DATE_FORMAT(session_date,'%d-%m-%y') as session_date FROM Sessions WHERE movieName =@movieID Group By session_date", conn);
            da.SelectCommand.Parameters.AddWithValue("@movieID", movieID);
            ds = new DataSet();
            da.Fill(ds, "Sessions");
            return ds;
        }

        public DataSet GetSessionTime(int movieID, string textDate, int ticketsAmount)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlDataAdapter da;
            DataSet ds;

            da = new MySqlDataAdapter("SELECT sessionID, DATE_FORMAT(session_date,'%d-%m-%y') as session_date, TIME_FORMAT(session_time,'%H:%i') as session_time, Seats FROM Sessions WHERE movieName =@movieID AND DATE_FORMAT(session_date,'%d-%m-%y') = @Date AND Seats >= @ticketsAmount", conn);
            da.SelectCommand.Parameters.AddWithValue("@movieID", movieID);
            da.SelectCommand.Parameters.AddWithValue("@Date", textDate);
            da.SelectCommand.Parameters.AddWithValue("@ticketsAmount", ticketsAmount);
            ds = new DataSet();
            da.Fill(ds, "Sessions");
            return ds;
        }

        public void InsertCodeDB(int sessionID, string code)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            conn.Open();
            try
            {
                cmd = conn.CreateCommand();
                cmd.CommandText = "INSERT INTO unicCode(sessionID, generatedCode) VALUES (@sessionID, @code)";
                cmd.Parameters.AddWithValue("@sessionID", sessionID);
                cmd.Parameters.AddWithValue("@code", code);
                cmd.ExecuteNonQuery();
                conn.Close();
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public List<string> GetCodes(int sessionID)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            conn.Open();
            try
            {
                cmd = conn.CreateCommand();
                cmd.CommandText = "select generatedCode from unicCode where sessionID = @sessionID";
                cmd.Parameters.AddWithValue("@sessionID", sessionID);
                List<string> Codes = new List<string>();
                using (MySqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        Codes.Add(reader.GetString(0));
                    }
                }
                conn.Close();
                return Codes;
                    

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

        public void DeleteCode(string code)
        {
            MySqlConnection conn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            conn.Open();
            try
            {
                cmd = conn.CreateCommand();
                cmd.CommandText = "delete from unicCode where generatedCode = @code";
                cmd.Parameters.AddWithValue("@code", code);
                cmd.ExecuteNonQuery();
                conn.Close();

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (conn.State == ConnectionState.Open)
                {
                    conn.Close();
                }
            }
        }

    }
}
