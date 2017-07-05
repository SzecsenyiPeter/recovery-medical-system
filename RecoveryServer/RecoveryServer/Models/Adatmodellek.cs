using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Korhaz
{

    public class Dolgozo {


        public string nev;
        public string cim;
        public string telefonszam;
        public string email_cim;
        public int fizetes;

        public Dolgozo(string _nev, string _cim, string _telefonszam, string _email_cim, int _fizetes) {

            nev = _nev;
            cim = _cim;
            telefonszam = _telefonszam;
            email_cim = _email_cim;
            fizetes = _fizetes;

        }

        public string InserteAlakitas()
        {

            string querystring = "INSERT INTO dolgozo VALUES(beteg_ai.nextval, '" + nev + "', '" + cim + "', '" + telefonszam + "', '" + email_cim + "', '" + fizetes + "')";
            return querystring;

        }

        
    }

    public class Beteg {

        public string nev;
        public string cim;
        public string telefonszam;
        

        public Beteg(string _nev, string _cim, string _telefonszam)
        {

            nev = _nev;
            cim = _cim;
            telefonszam = _telefonszam;
           
        }

        public string InserteAlakitas()
        {

            string querystring = "INSERT INTO beteg VALUES( dolgozo_ai.nextval, '" + nev + "', '" + cim + "', '" + telefonszam  + "')";
            return querystring;

        }

    }

    public class Kezeles {

        public string kezeles_tipus;
        public string kezelt_beteg;
        public string kezelo_orvos;
        public int ido;
        public DateTime kezdes;

        public Kezeles(string _kezeles_tipus, string _kezelt_beteg,  string _kezelo_orvos,  int _ido,  DateTime _kezdes)
        {

            kezeles_tipus = _kezeles_tipus;
            kezelt_beteg = _kezelt_beteg;
            kezelo_orvos = _kezelo_orvos;
            ido = _ido;
            kezdes = _kezdes;

        }

        

    }

    public class Kezeles_Tipus
    {

        public string nev;
        public int ar;

        public Kezeles_Tipus(string _nev, int _ar) {

            ar = _ar;
            nev = _nev;
        }


    }

    public class Elozmeny
    {

        public string betegnev;
        public string orvosNev;
        public string kezelesNev;
        public string kezdes;

        public Elozmeny(string _betegNev, string _kezdes, string _orvosNev, string _kezelesNev )
        {
            betegnev = _betegNev;
            orvosNev = _orvosNev;
            kezelesNev = _kezelesNev;
            kezdes = _kezdes;

        }

    }

    public class UserInfo
    {

        public string User { set; get; }
        public string PassworldHash { set; get; }

    }
    

}
