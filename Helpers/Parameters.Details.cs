using RtelLibrary.DataBaseObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MisterProtypoParser.Helpers
{
    public partial record Parameters
    {
        public bool UseLDAPS(int detailID) 
        {
            var result = _sysParametersDetails.FirstOrDefault(x => 
            x.AppID == _sysApplicationProcessID && 
            x.DetailID == detailID && 
            x.ParamName.Equals(ParameterNames.UseLDAPS, StringComparison.OrdinalIgnoreCase))?.ParamValue;

            if(string.IsNullOrEmpty(result)) return _useLDAPS;

            return Convert.ToBoolean(result);
        }

        public string LDAPSearcherFilter(int detailID)
        {
            var result = _sysParametersDetails.FirstOrDefault(x =>
            x.AppID == _sysApplicationProcessID &&
            x.DetailID == detailID &&
            x.ParamName.Equals(ParameterNames.LDAPSearcherFilter, StringComparison.OrdinalIgnoreCase))?.ParamValue;

            if (string.IsNullOrEmpty(result)) return _LDAPSearcherFilter;

            return result;
        }

        public string[] ColumnsOfLDAPDataTable(int detailID)
        {
            var result = _sysParametersDetails.FirstOrDefault(x =>
            x.AppID == _sysApplicationProcessID &&
            x.DetailID == detailID &&
            x.ParamName.Equals(ParameterNames.ColumnsOfLDAPDataTable, StringComparison.OrdinalIgnoreCase))?.ParamValue;

            if (string.IsNullOrEmpty(result)) return _columnsOfLDAPDataTable;

            return result?.Split('|', StringSplitOptions.RemoveEmptyEntries);
        }

        public string LDAPSDefaultPort(int detailID)
        {
            var result = _sysParametersDetails.FirstOrDefault(x =>
            x.AppID == _sysApplicationProcessID &&
            x.DetailID == detailID &&
            x.ParamName.Equals(ParameterNames.LDAPSDefaultPort, StringComparison.OrdinalIgnoreCase))?.ParamValue;

            if (string.IsNullOrEmpty(result)) return _LDAPSDefaultPort;

            return result;
        }

        public string LDAPDefaultPort(int detailID)
        {
            var result = _sysParametersDetails.FirstOrDefault(x =>
            x.AppID == _sysApplicationProcessID &&
            x.DetailID == detailID &&
            x.ParamName.Equals(ParameterNames.LDAPDefaultPort, StringComparison.OrdinalIgnoreCase))?.ParamValue;

            if (string.IsNullOrEmpty(result)) return _LDAPDefaultPort;

            return result;
        }
    }
}
