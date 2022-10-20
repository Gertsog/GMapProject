using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;

namespace GMapProject
{
    public class DbContext
    {
        private string _connectionString;

        public DbContext()
        {
            _connectionString = ConfigurationManager.ConnectionStrings["mssqldb"].ConnectionString;
        }

        //Проверка на существование БД
        public bool DatabaseExists()
        {
            using (var connection = new SqlConnection(_connectionString))
            {
                try
                {
                    connection.Open();
                    return true;
                }
                catch (SqlException)
                {
                    return false;
                }
            }
        }

        //Получение всех маркеров из БД
        public List<MyMarker> GetMarkers()
        {
            var result = new List<MyMarker>();

            var sqlExpression = "SELECT * FROM Markers";
            using (var connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(sqlExpression, connection);
                var reader = command.ExecuteReader();

                if (reader.HasRows)
                {
                    while (reader.Read())
                    {
                        var id = (int)reader.GetValue(0);
                        var lat = (double)reader.GetValue(1);
                        var lng = (double)reader.GetValue(2);
                        var point = new GMap.NET.PointLatLng(lat, lng);
                        result.Add(new MyMarker(id, point));
                    }
                }
            }
            return result;
        }

        //Сохранение маркера в БД
        public int CreateMarker(MyMarker marker)
        {
            string sqlExpression = "INSERT INTO Markers (Latitude, Longitude) " +
                                    "VALUES (@latitude, @lingitude) " +
                                    "SELECT SCOPE_IDENTITY()";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(sqlExpression, connection);
                var lat = new SqlParameter("@latitude", marker.Position.Lat);
                var lng = new SqlParameter("@lingitude", marker.Position.Lng);
                command.Parameters.Add(lat);
                command.Parameters.Add(lng);

                return Convert.ToInt32(command.ExecuteScalar());
            }
        }

        //Обновление маркера в БД
        public void UpdateMarker(MyMarker marker)
        {
            string sqlExpression = "UPDATE Markers " +
                                    "SET Latitude=@latitude, Longitude=@lingitude " +
                                    "WHERE id=@id";
            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();
                var command = new SqlCommand(sqlExpression, connection);
                var lat = new SqlParameter("@latitude", marker.Position.Lat);
                var lng = new SqlParameter("@lingitude", marker.Position.Lng);
                var id = new SqlParameter("@id", marker.Id);
                command.Parameters.Add(lat);
                command.Parameters.Add(lng);
                command.Parameters.Add(id);
                command.ExecuteNonQuery();
            }
        }

    }
}
