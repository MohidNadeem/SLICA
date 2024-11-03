using Microsoft.Extensions.Configuration;
using System.Collections;
using System.Data;
using System.Data.SqlClient;

namespace SLICAWebAPI.Shared
{
    public class Transaction
    {
        

        public static Hashtable DataTransaction(Hashtable parameters, string spName)
        {
            var AppSetting = new ConfigurationBuilder()
               .SetBasePath(Directory.GetCurrentDirectory())
               .AddJsonFile("appsettings.json")
               .Build();

            string connectionString = AppSetting.GetSection("MySettings").GetSection("ConnectionString").Value;

            DataSet resultDataSet = new DataSet();
            Hashtable htResult = new Hashtable();

            try
            {
                using (SqlConnection connection = new SqlConnection(connectionString))
                {
                    using (SqlCommand command = new SqlCommand(spName, connection))
                    {
                        command.CommandType = CommandType.StoredProcedure;

                        foreach (DictionaryEntry entry in parameters)
                        {
                            // Add parameter with datatype handling
                            //SqlParameter parameter = new SqlParameter("@P_" + entry.Key.ToString(), GetSqlDbType(entry.Value));
                            //parameter.Value = entry.Value.ToString();
                            command.Parameters.AddWithValue("@P_" + entry.Key.ToString(), entry.Value);
                        }

                        // Output parameter for message
                        SqlParameter messageParameter = new SqlParameter("@P_Message", SqlDbType.NVarChar, 255);
                        messageParameter.Direction = ParameterDirection.Output;
                        command.Parameters.Add(messageParameter);

                        connection.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(command))
                        {
                            adapter.Fill(resultDataSet);
                        }

                        // Retrieve the output parameter value
                        htResult.Add("ds", resultDataSet);
                        htResult.Add("Message", command.Parameters["@P_Message"].Value.ToString());


                    }
                }
            }
            catch (Exception e)
            {

            }
            return htResult;
        }


        //public static Hashtable SearchVideoTransaction(String VideoName, string spName)
        //{
        //    DataSet resultDataSet = new DataSet();
        //    Hashtable htResult = new Hashtable();

        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            using (SqlCommand command = new SqlCommand(spName, connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;


        //                SqlParameter parameter = new SqlParameter("@P_Name", SqlDbType.NVarChar);
        //                parameter.Value = VideoName;
        //                command.Parameters.Add(parameter);
        //                SqlParameter messageParameter = new SqlParameter("@P_Message", SqlDbType.NVarChar, 255);
        //                messageParameter.Direction = ParameterDirection.Output;
        //                command.Parameters.Add(messageParameter);

        //                connection.Open();

        //                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        //                {
        //                    adapter.Fill(resultDataSet);
        //                }

        //                // Retrieve the output parameter value
        //                htResult.Add("ds", resultDataSet);
        //                htResult.Add("Message", command.Parameters["@P_Message"].Value.ToString());


        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //    return htResult;
        //}
        //public static Hashtable VideosAvailableTransaction(string spName)
        //{
        //    DataSet resultDataSet = new DataSet();
        //    Hashtable htResult = new Hashtable();

        //    try
        //    {
        //        using (SqlConnection connection = new SqlConnection(connectionString))
        //        {
        //            using (SqlCommand command = new SqlCommand(spName, connection))
        //            {
        //                command.CommandType = CommandType.StoredProcedure;
        //                SqlParameter messageParameter = new SqlParameter("@P_Message", SqlDbType.NVarChar, 255);
        //                messageParameter.Direction = ParameterDirection.Output;
        //                command.Parameters.Add(messageParameter);

        //                connection.Open();

        //                using (SqlDataAdapter adapter = new SqlDataAdapter(command))
        //                {
        //                    adapter.Fill(resultDataSet);
        //                }

        //                // Retrieve the output parameter value
        //                htResult.Add("ds", resultDataSet);
        //                htResult.Add("Message", command.Parameters["@P_Message"].Value.ToString());


        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {

        //    }
        //    return htResult;
        //}

        private static SqlDbType GetSqlDbType(object value)
        {
            if (value is DateTime)
            {
                return SqlDbType.DateTime;
            }
            else if (value is int)
            {
                return SqlDbType.Int;
            }
            else if (value is bool)
            {
                return SqlDbType.Bit;
            }
            else if (value is string)
            {
                return SqlDbType.NVarChar;
            }
            else
            {
                // Handle other types as needed
                throw new ArgumentException($"Unsupported parameter type: {value.GetType()}");
            }
        }
    }
}
