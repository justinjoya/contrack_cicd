using Npgsql;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace Contrack
{
    public enum DatabaseCollection
    {
        Byntra = 0,
        Contrack = 1,
    }
    public class SqlDB : CustomException, IDisposable
    {
        public NpgsqlConnection Conn;
        public void Dispose()
        {
            Conn.Close();
        }
        public SqlDB(DatabaseCollection Database = DatabaseCollection.Byntra)
        {
            try
            {
                Conn = new NpgsqlConnection(GetDbInfo(Database));

                if (Conn.State != ConnectionState.Open)
                    Conn.Open();
            }
            catch (Exception ex)
            {
                RecordException(ex);
                //Log.WriteLog("Error : " + ex.ToString());
            }
        }
        public DataTable GetDataTable(string Sql)
        {
            DataTable tbl = new DataTable();
            try
            {
                if (Conn.State != ConnectionState.Open)
                    Conn.Open();
                //NpgsqlCommand Command = new NpgsqlCommand(Sql, Conn);
                //Command.CommandType = cmdtype;
                NpgsqlDataAdapter Da = new NpgsqlDataAdapter(Sql, Conn);
                Da.Fill(tbl);
            }
            catch (Exception ex)
            {
                RecordException(ex);
                //Log.WriteLog("Qry : " + Sql + ", Error : " + ex.ToString());
                //tbl = Common.ResultToDT("0", Sql + "=>" + ex.ToString());
            }
            return tbl;
        }

        public Result ExecuteProcedure(string query)
        {
            Result result = Common.SuccessMessage("Success");

            try
            {
                Conn.Notice += (sender, e) => result = Common.ErrorMessage(e.Notice.MessageText);

                if (Conn.State != ConnectionState.Open)
                    Conn.Open();

                NpgsqlCommand Command = new NpgsqlCommand(query, Conn);
                //Command.CommandType = cmdtype;
                Command.ExecuteNonQuery();

            }
            catch (NpgsqlException ex)
            {
                RecordException(ex);
                result = Common.ErrorMessage(ex.Message);
            }
            return result;
        }

        public int Execute(string Sql) //, CommandType cmdtype = CommandType.Text
        {
            int RowsAffected = 0;
            try
            {
                if (Conn.State != ConnectionState.Open)
                    Conn.Open();
                NpgsqlCommand Command = new NpgsqlCommand(Sql, Conn);
                //Command.CommandType = cmdtype;
                RowsAffected = Command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                RecordException(ex);
                //Log.WriteLog("Qry : " + Sql + ", Error : " + ex.ToString());
            }
            return RowsAffected;
        }

        public object GetValue(string Sql)
        {
            object obj = null;
            try
            {
                if (Conn.State != ConnectionState.Open)
                    Conn.Open();
                NpgsqlCommand Command = new NpgsqlCommand(Sql, Conn);
                //Command.CommandType = cmdtype;
                obj = Command.ExecuteScalar();
            }
            catch (Exception ex)
            {
                RecordException(ex);
                //Log.WriteLog("Qry : " + Sql + ", Error : " + ex.ToString());
            }
            return obj;
        }

        private static string GetDbInfo(DatabaseCollection db)
        {
            switch (db)
            {
                case DatabaseCollection.Byntra: return ConfigurationManager.ConnectionStrings["DBConnection"].ToString();
                case DatabaseCollection.Contrack: return ConfigurationManager.ConnectionStrings["DBConnectionConTrack"].ToString();
                default: return ConfigurationManager.ConnectionStrings["DBConnection"].ToString();
            }
        }
    }
}