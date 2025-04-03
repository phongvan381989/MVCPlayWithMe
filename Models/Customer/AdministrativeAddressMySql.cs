using MVCPlayWithMe.General;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace MVCPlayWithMe.Models
{
    public class AdministrativeAddressMySql : BasicMySql
    {
        private string pro;
        private string dis;
        private string subdis;

        private void Convert(List<AdministrativeAddress> ls, MySqlDataReader rdr)
        {
            pro = MyMySql.GetString(rdr, "Province");
            dis = MyMySql.GetString(rdr, "District");
            subdis = MyMySql.GetString(rdr, "SubDistrict");
            if(ls.Count() == 0)
            {
                ls.Add(new AdministrativeAddress(pro, dis, subdis));
            }
            else
            {
                AdministrativeAddress obj = ls[ls.Count() - 1];
                if(obj.province != pro) // Check có phải tỉnh mới
                {
                    ls.Add(new AdministrativeAddress(pro, dis, subdis));
                }
                else
                {
                    if(obj.districts[obj.districts.Count() - 1] != dis)// Check có phải huyện mới
                    {
                        obj.districts.Add(dis);

                        List<string> lsubdis = new List<string>();
                        lsubdis.Add(subdis);
                        obj.subdistricts.Add(lsubdis);
                    }
                    else{
                        obj.subdistricts[obj.subdistricts.Count() - 1].Add(subdis);
                    }
                }
            }
        }
        public List<AdministrativeAddress> GetListAdministrativeAddress()
        {
            List<AdministrativeAddress> ls = new List<AdministrativeAddress>();
            MySqlConnection conn = new MySqlConnection(MyMySql.connStr);
            try
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand("SELECT * FROM webplaywithme.tbadministrativeaddress ORDER BY Province, District, SubDistrict;", conn);
                cmd.CommandType = CommandType.Text;

                MySqlDataReader rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    Convert(ls, rdr);
                }
                rdr.Close();

            }
            catch (Exception ex)
            {
                
                MyLogger.GetInstance().Warn(ex.ToString());
            }

            conn.Close();
            return ls;
        }
    }
}