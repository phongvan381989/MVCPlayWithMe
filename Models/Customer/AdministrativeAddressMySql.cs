using MVCPlayWithMe.General;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MVCPlayWithMe.Models
{
    public class AdministrativeAddressMySql
    {
        public async Task<List<AdministrativeAddress>> GetListAdministrativeAddressAsync()
        {
            List<AdministrativeAddress> ls = new List<AdministrativeAddress>();
            using (MySqlConnection conn = new MySqlConnection(MyMySql.connStr))
            {
                try
                {
                    await conn.OpenAsync();
                    MySqlCommand cmd = new MySqlCommand(
                        @"SELECT a.ProvinceId, p.Name, a.SubDistrict FROM tbadministrativeaddress AS a 
                        LEFT JOIN tbaddressprovince AS p ON a.ProvinceId = p.Id ORDER BY a.ProvinceId;", conn);
                    cmd.CommandType = CommandType.Text;
                    using (MySqlDataReader rdr = (MySqlDataReader)await cmd.ExecuteReaderAsync())
                    {
                        int ProvinceIdIndex = rdr.GetOrdinal("ProvinceId");
                        int NameIndex = rdr.GetOrdinal("Name");
                        int SubDistrictIndex = rdr.GetOrdinal("SubDistrict");
                        AdministrativeAddress lastObj = null;
                        int provinceId = -1;
                        while (await rdr.ReadAsync())
                        {
                            provinceId = rdr.GetInt32(ProvinceIdIndex);
                            if (lastObj == null || lastObj.provinceId != provinceId) // Check có phải tỉnh mới
                            {
                                lastObj = new AdministrativeAddress(provinceId, rdr.GetString(NameIndex), rdr.GetString(SubDistrictIndex));
                                ls.Add(lastObj);
                            }
                            else
                            {
                                lastObj.subdistricts.Add(rdr.GetString(SubDistrictIndex));
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    MyLogger.GetInstance().Warn(ex.ToString());
                }
            }
            return ls;
        }
    }
}
