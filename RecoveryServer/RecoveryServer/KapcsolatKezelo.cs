using System;
using System.Collections.Generic;
using System.Text;
using System.Configuration;
using Oracle.ManagedDataAccess.Client;
using System.Security.Cryptography;

namespace Korhaz
{
    public class KapcsolatKezelo
    {
        private string kapcsolatString;
        private OracleConnection kapcsolat;
        public enum JogSzint {Doktor, Asszisztens, Admin, Igazgato }
        static DateTime KoviCheckDate;


        struct TokenInfo
        {

            
            public JogSzint jog;
            public DateTime lejar;

            public TokenInfo(JogSzint _jog, DateTime _lejar)
            {
                
                jog = _jog;
                lejar = _lejar;
            
            }

        }

        private static Dictionary<string, TokenInfo> Tokenek = new Dictionary<string, TokenInfo>();
        public static void Takaritas()
        {
           

            foreach (KeyValuePair<string,TokenInfo> k in Tokenek)
            {
                Tokenek.Remove(k.Key);
            }
        }

       

        public KapcsolatKezelo()
        {

            kapcsolatString = kapcsolatStingDekodolas();
            
            try
            {
                kapcsolat = new OracleConnection(kapcsolatString);
                kapcsolat.Open();
            }
            catch (Exception ef)
            {
                throw;
            }
            

        }


        public void Bezar()
        {

            if (DateTime.Compare(KoviCheckDate, DateTime.Now) < 0)
            {
               // Takaritas();
                KoviCheckDate = DateTime.Now.AddMinutes(10);
            }

            kapcsolat.Close();
            kapcsolat.Dispose();
        }

        private string kapcsolatStingDekodolas()
        {
            // Ezt valahogy titkosítani kellene
            //return ConfigurationManager.ConnectionStrings["Main"].ConnectionString;
            return "Data Source = (DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST = 10.100.220.58)(PORT = 1521))(CONNECT_DATA = (SERVER = DEDICATED)(SERVICE_NAME = scs.swicon.local))); User Id = peti; Password=peti;";



        }


        public void InsertDolgozo(Dolgozo dolg, string token)
        {

            if (!Tokenek.ContainsKey(token) || Tokenek[token].jog == JogSzint.Doktor || Tokenek[token].jog == JogSzint.Asszisztens)
            {
                return;
            }
            else if (DateTime.Compare(Tokenek[token].lejar, DateTime.Now) < 0)
            {
                Tokenek.Remove(token);
                return;
            }

            

            try
            {
                OracleCommand cmd = new OracleCommand(dolg.InserteAlakitas(), kapcsolat);
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public void InsertBeteg(Beteg beteg, string token)
        {

            if (!Tokenek.ContainsKey(token) || Tokenek[token].jog == JogSzint.Doktor)
            {
                return;
            }
            else if (DateTime.Compare(Tokenek[token].lejar, DateTime.Now) < 0)
            {
                Tokenek.Remove(token);
                return;
            }


            try
            {
                OracleCommand cmd = new OracleCommand(beteg.InserteAlakitas(), kapcsolat);
                cmd.ExecuteNonQuery();
                
            }
            catch (Exception )
            {
                
                throw;
            }
        }


        public List<string> GetAllKezeles()
        {

            List<string> kezelesek = new List<string>();
            OracleCommand cmd = new OracleCommand("SELECT peti.kezeles_tipus.nev FROM peti.kezeles_tipus", kapcsolat);
            OracleDataReader reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                kezelesek.Add(reader["nev"].ToString());
                while (reader.Read())
                {

                    kezelesek.Add(reader["nev"].ToString());

                }
                return kezelesek;

            }
            else
            {
                kezelesek.Add("hiba");
                return kezelesek;
            }


        }

        public bool InsertKezeles(Kezeles kez, string token)
        {


            if (!Tokenek.ContainsKey(token))
            {
                return false;
            }
            else if (DateTime.Compare(Tokenek[token].lejar, DateTime.Now) < 0)
            {
                Tokenek.Remove(token);
                return false;
            }






            try
            {
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "kezeles_regisztralas";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = kapcsolat;
                cmd.Parameters.Add(new OracleParameter("@beteg_nev", kez.kezelt_beteg));
                cmd.Parameters.Add(new OracleParameter("@dolgozo_nev", kez.kezelo_orvos));
                cmd.Parameters.Add(new OracleParameter("@kezeles_tipus_nev", kez.kezeles_tipus));
                cmd.Parameters.Add(new OracleParameter("@ido", kez.ido));
                cmd.Parameters.Add(new OracleParameter("@kezdes", kez.kezdes.ToString("dd/MMM/yy")));
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (OracleException ef)
            {
                if (ef.Number == 1403)
                {
                    return false;
                }
                throw;
            }

            

        }

        public List<string> FilterBeteg(string s)
        {

            OracleCommand cmd = new OracleCommand("SELECT beteg.nev FROM beteg WHERE beteg.nev LIKE :keresett", kapcsolat);
           
            s = s + "%";
            cmd.Parameters.Add(new OracleParameter("keresett",  s));
            


            
            OracleDataReader r = cmd.ExecuteReader();
            List<string> megfelelt = new List<string>();
            while (r.Read())
            {
                megfelelt.Add(r["nev"].ToString());
            }
            return megfelelt;


        }

        public List<string> FilterOrvos(string s)
        {

            OracleCommand cmd = new OracleCommand("SELECT dolgozo.nev FROM dolgozo WHERE dolgozo.nev LIKE :keresett", kapcsolat);

            s = s + "%";
            cmd.Parameters.Add(new OracleParameter("keresett", s));




            OracleDataReader r = cmd.ExecuteReader();
            List<string> megfelelt = new List<string>();
            while (r.Read())
            {
                megfelelt.Add(r["nev"].ToString());
            }
            return megfelelt;


        }


        public List<Elozmeny> ElozmenyKerdezes(int tipus, string beteg, string dolgozo, string token)
        {
            if (!Tokenek.ContainsKey(token))
            {
                return null;
            }
            else if (DateTime.Compare(Tokenek[token].lejar, DateTime.Now) < 0)
            {
                Tokenek.Remove(token);
                return null;
            }



            List<Elozmeny> elozmenyek = new List<Elozmeny>();
            string parancsSzoveg = @"SELECT peti.beteg.nev, peti.kezeles.kezdes, peti.dolgozo.nev, peti.kezeles_tipus.nev FROM peti.beteg 
                                    INNER JOIN peti.kezeles ON peti.beteg.idbeteg = peti.kezeles.idbeteg
                                    INNER JOIN peti.dolgozo ON peti.dolgozo.iddolgozo = peti.kezeles.iddolgozo
                                    INNER JOIN peti.kezeles_tipus ON peti.kezeles_tipus.idkezeles_tipus = peti.kezeles.idkezeles_tipus";
                                   
          
            if (tipus == 1)
            {
                parancsSzoveg += " WHERE peti.beteg.nev = :beteg_nev AND  peti.dolgozo.nev = :dolgozo_nev";
                OracleCommand cmd = new OracleCommand(parancsSzoveg, kapcsolat);
                cmd.Parameters.Add(new OracleParameter("beteg_nev", beteg));
                cmd.Parameters.Add(new OracleParameter("dolgoz-nev", dolgozo));
                OracleDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    elozmenyek.Add(new Elozmeny(r[0].ToString(), r[1].ToString(), r[2].ToString(), r[3].ToString()));
                }

                return elozmenyek;

            }
            else if (tipus == 2)
            {
                parancsSzoveg += " WHERE peti.beteg.nev = :beteg_nev";
                OracleCommand cmd = new OracleCommand(parancsSzoveg, kapcsolat);
                cmd.Parameters.Add(new OracleParameter("beteg_nev", beteg));
                OracleDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    elozmenyek.Add(new Elozmeny(r[0].ToString(), r[1].ToString().Split(' ')[0], r[2].ToString(), r[3].ToString()));
                }

                return elozmenyek;
            }
            else
            {
                parancsSzoveg += " WHERE peti.dolgozo.nev = :dolgozo_nev";
                OracleCommand cmd = new OracleCommand(parancsSzoveg, kapcsolat);
                cmd.Parameters.Add(new OracleParameter("dolgoz-nev", dolgozo));
                OracleDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    elozmenyek.Add(new Elozmeny(r[0].ToString(), r[1].ToString().Split(' ')[0], r[2].ToString(), r[3].ToString()));
                }

                return elozmenyek;
            }


        }

        public string Login(string felhasznalo, string jelszo)
        {


            MD5 hashAlgo = MD5.Create();

            byte[] byteStream = hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(jelszo));


            StringBuilder sw = new StringBuilder();

            for (int i = 0; i < byteStream.Length; i++)
            {
                sw.Append(byteStream[i].ToString("x2"));
            }

            string hash = sw.ToString();

            OracleCommand cmd = new OracleCommand("SELECT dolgozo_login.pass, dolgozo_login.szint FROM peti.dolgozo_login INNER JOIN dolgozo ON dolgozo.iddolgozo = dolgozo_login.iddolgozo WHERE dolgozo.nev = :felhasznalo", kapcsolat);
            cmd.Parameters.Add(new OracleParameter("felhasznalo", felhasznalo));
            OracleDataReader rd = cmd.ExecuteReader();
            if (rd.Read())
            {
                if (rd["pass"].ToString() == hash)
                {
                    Random r = new Random();
                    string token = felhasznalo + "_" + rd["szint"].ToString() + "_" + DateTime.Now.AddMinutes(10) + "_" + (char)r.Next(98, 120) + (char)r.Next(98, 120) + (char)r.Next(98, 120) + (char)r.Next(98, 120);
                    Tokenek.Add(token,new TokenInfo((JogSzint)int.Parse(rd["szint"].ToString()), DateTime.Now.AddMinutes(10)));
                    return token;
                }
                else
                {
                    return "-1";
                }

            }
            else
            {
                return "-2";
            }


        }


        public bool Reg(string felhasznalo, string jelszo, int szint)
        {
            
            MD5 hashAlgo = MD5.Create();

            byte[] byteStream = hashAlgo.ComputeHash(Encoding.UTF8.GetBytes(jelszo));

          
            StringBuilder sw = new StringBuilder();


            for (int i = 0; i < byteStream.Length; i++)
            {
                sw.Append(byteStream[i].ToString("x2"));
            }
           
            try
            {
                string hash = sw.ToString();
                OracleCommand cmd = new OracleCommand();
                cmd.CommandText = "dolgozo_login_register";
                cmd.CommandType = System.Data.CommandType.StoredProcedure;
                cmd.Connection = kapcsolat;
                cmd.Parameters.Add(new OracleParameter("@login_nev", felhasznalo));
                cmd.Parameters.Add(new OracleParameter("@login_pass", hash));
                cmd.Parameters.Add(new OracleParameter("@szint", szint));
                cmd.ExecuteNonQuery();
            }
            catch (Exception)
            {

                throw;
            }
            return true;
        }


        public bool InsertAsszisztensKapcsolat(string AsszisztensNev, string DoktorNev)
        {

            try
            {

                OracleCommand cmd = new OracleCommand("INSERT INTO peti.asszisztens_kapcsolat VALUES((SELECT peti.dolgozo.iddolgozo FROM peti.dolgozo WHERE peti.dolgozo.nev = :asszisztens),(SELECT peti.dolgozo.iddolgozo FROM peti.dolgozo WHERE peti.dolgozo.nev = :doktor))", kapcsolat);
                cmd.Parameters.Add(new OracleParameter("asszisztens", AsszisztensNev));
                cmd.Parameters.Add(new OracleParameter("doktor", DoktorNev));
                int siker = cmd.ExecuteNonQuery();
                if (siker == 1)
                {
                    return true;
                }

                else
                {
                    return false;
                }

            }
            catch (Exception)
            {

                throw;
            }

            
        }

        public string GetAsszisztensKapcsolat(string AsszisztensNev)
        {

            string cmdStrng = "SELECT peti.dolgozo.nev FROM peti.dolgozo INNER JOIN peti.asszisztens_kapcsolat ON peti.dolgozo.iddolgozo = peti.asszisztens_kapcsolat.idorvos WHERE (SELECT peti.asszisztens_kapcsolat.idasszisztens FROM peti.asszisztens_kapcsolat INNER JOIN peti.dolgozo ON peti.dolgozo.iddolgozo = peti.asszisztens_kapcsolat.idasszisztens WHERE peti.dolgozo.nev = :asszisztens)  = peti.asszisztens_kapcsolat.idasszisztens";
            try
            {

                OracleCommand cmd = new OracleCommand("SELECT peti.dolgozo.nev FROM peti.dolgozo INNER JOIN peti.asszisztens_kapcsolat ON peti.dolgozo.iddolgozo = peti.asszisztens_kapcsolat.idorvos WHERE (SELECT peti.asszisztens_kapcsolat.idasszisztens FROM peti.asszisztens_kapcsolat INNER JOIN peti.dolgozo ON peti.dolgozo.iddolgozo = peti.asszisztens_kapcsolat.idasszisztens WHERE peti.dolgozo.nev = :asszisztens)  = peti.asszisztens_kapcsolat.idasszisztens", kapcsolat);
                cmd.Parameters.Add(new OracleParameter("asszisztens", AsszisztensNev));
                OracleDataReader rd = cmd.ExecuteReader();
                if (rd.Read())
                {
                    return rd["nev"].ToString();
                }
                else
                {
                    return "-1";
                }
            }
            catch (Exception)
            {

                throw;
            }

        }


    }
}
