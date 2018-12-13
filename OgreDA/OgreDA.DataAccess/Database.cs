using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OgreDA.DataAccess
{
    public partial class Database
    {
		private bool handleProviderSpecificParameters(DbCommand command)
		{
			if (command is System.Data.OleDb.OleDbCommand)
			{
				replaceParametersWithPlaceHolder(command);
				return true;
			}
			return false;
		}
		private void replaceParametersWithPlaceHolder(DbCommand command)
		{
			string commandText = command.CommandText;
			// must replace named parameters with ? place holders
			foreach (DbParameter param in command.Parameters)
			{
				string paramName = param.ParameterName;
				if (!paramName.StartsWith("@"))
				{
					paramName = "@" + paramName;
				}
				string pattern = paramName + @"\b";
				commandText = System.Text.RegularExpressions.Regex.Replace(commandText, pattern, "?");
			}
			command.CommandText = commandText;
		}
	}
}
