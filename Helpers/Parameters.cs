using System;
using System.Collections.Generic;
using System.Linq;
using RtelLibrary.DataBaseObjects;
using RtelLibrary.Enums;
using RtelLibrary.TableModels;

namespace MisterTeamsUsersParserParser.Helpers
{
    public partial record Parameters
    {
        //Private properties
        private IEnumerable<SysParametersDetails> _sysParametersDetails;
        private readonly int _sysApplicationProcessID;        
        
        //Public properties
        public IEnumerable<SysParameters> SysParameters { get; private set; }

        public Parameters(IEnumerable<SysParameters> sysParameters, IEnumerable<SysParametersDetails> sysParametersDetails, SysApplicationProcess sysApplicationProcess)
        {
            SysParameters = sysParameters;
            _sysParametersDetails = sysParametersDetails;
            _sysApplicationProcessID = (int)sysApplicationProcess;
            InitializeProperties(sysParameters);
        }

        public void UpdateProperties(IEnumerable<SysParameters> sysParameters, IEnumerable<SysParametersDetails> sysParametersDetails)
        {
            SysParameters = sysParameters;
            _sysParametersDetails = sysParametersDetails;
            InitializeProperties(sysParameters);
        }
        
        private void InitializeProperties(IEnumerable<SysParameters> sysParameters)
        {/*
            _useLDAPS = Convert.ToBoolean(
                sysParameters.FirstOrDefault(x => x.AppID == _sysApplicationProcessID && x.ParamName.Equals(ParameterNames.UseLDAPS, StringComparison.OrdinalIgnoreCase))?.ParamValue ??
                sysParameters.FirstOrDefault(x => x.AppID == (int)Program.sysApplication && x.ParamName.Equals(ParameterNames.UseLDAPS, StringComparison.OrdinalIgnoreCase))?.ParamValue
                );
            _LDAPSDefaultPort = sysParameters.FirstOrDefault(x => x.AppID == _sysApplicationProcessID && x.ParamName.Equals(ParameterNames.LDAPSDefaultPort, StringComparison.OrdinalIgnoreCase))?.ParamValue ??
                sysParameters.FirstOrDefault(x => x.AppID == (int)Program.sysApplication && x.ParamName.Equals(ParameterNames.LDAPSDefaultPort, StringComparison.OrdinalIgnoreCase))?.ParamValue;
            _LDAPDefaultPort = sysParameters.FirstOrDefault(x => x.AppID == _sysApplicationProcessID && x.ParamName.Equals(ParameterNames.LDAPDefaultPort, StringComparison.OrdinalIgnoreCase))?.ParamValue ??
                sysParameters.FirstOrDefault(x => x.AppID == (int)Program.sysApplication && x.ParamName.Equals(ParameterNames.LDAPDefaultPort, StringComparison.OrdinalIgnoreCase))?.ParamValue;
            _LDAPSearcherFilter = sysParameters.FirstOrDefault(x => x.AppID == _sysApplicationProcessID && x.ParamName.Equals(ParameterNames.LDAPSearcherFilter, StringComparison.OrdinalIgnoreCase))?.ParamValue ??
                sysParameters.FirstOrDefault(x => x.AppID == (int)Program.sysApplication && x.ParamName.Equals(ParameterNames.LDAPSearcherFilter, StringComparison.OrdinalIgnoreCase))?.ParamValue;
            _columnsOfLDAPDataTable = sysParameters.FirstOrDefault(x => x.AppID == _sysApplicationProcessID && x.ParamName.Equals(ParameterNames.ColumnsOfLDAPDataTable, StringComparison.OrdinalIgnoreCase))?.ParamValue?.Split('|', StringSplitOptions.RemoveEmptyEntries) ??
                sysParameters.FirstOrDefault(x => x.AppID == (int)Program.sysApplication && x.ParamName.Equals(ParameterNames.ColumnsOfLDAPDataTable, StringComparison.OrdinalIgnoreCase))?.ParamValue?.Split('|', StringSplitOptions.RemoveEmptyEntries);
            AutoGrowLDAPData = Convert.ToBoolean(
                sysParameters.FirstOrDefault(x => x.AppID == _sysApplicationProcessID && x.ParamName.Equals(ParameterNames.AutoGrowLDAPData, StringComparison.OrdinalIgnoreCase))?.ParamValue ??
                sysParameters.FirstOrDefault(x => x.AppID == (int)Program.sysApplication && x.ParamName.Equals(ParameterNames.AutoGrowLDAPData, StringComparison.OrdinalIgnoreCase))?.ParamValue
                );
            LDAP_DataTableNVarcharDefaultSize = Convert.ToInt32(
                sysParameters.FirstOrDefault(x => x.AppID == _sysApplicationProcessID && x.ParamName.Equals(ParameterNames.LDAP_DataTableNVarcharDefaultSize, StringComparison.OrdinalIgnoreCase))?.ParamValue ??
                sysParameters.FirstOrDefault(x => x.AppID == (int)Program.sysApplication && x.ParamName.Equals(ParameterNames.LDAP_DataTableNVarcharDefaultSize, StringComparison.OrdinalIgnoreCase))?.ParamValue
                );
            */
        }
        }
}
