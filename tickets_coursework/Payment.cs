using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using MySql.Data.MySqlClient;

namespace tickets_coursework
{
    class Payment
    {
        public  void Reservation(int id, int places)
        {

            string connectionString = "SERVER=exordium.cloud;DATABASE=Tickets_DB;UID=Cation;PASSWORD=olja090398";
            MySqlConnection myconn = new MySqlConnection(connectionString);
            MySqlCommand cmd;
            myconn.Open();
            try
            {
                cmd = myconn.CreateCommand();
                cmd.CommandText = "SELECT Seats FROM Sessions WHERE sessionID= @id";
                cmd.Parameters.AddWithValue("@id", id);
                int seats = Convert.ToInt32(cmd.ExecuteScalar());
                seats -= places;
                cmd.CommandText = "UPDATE Sessions SET Seats = @seats WHERE sessionID = @id";
                cmd.Parameters.AddWithValue("@seats", seats);
                cmd.ExecuteNonQuery();

            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                if (myconn.State == System.Data.ConnectionState.Open)
                {
                    myconn.Close();
                }
            }


        }

    }
}
