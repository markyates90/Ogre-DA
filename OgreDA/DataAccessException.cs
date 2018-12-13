using System;
using System.Collections.Generic;
using System.Text;

namespace OgreDA.DataAccess
{
	/// <summary>
	/// There was an exception thrown by custom code accessing data from a data source
	/// usually used to wrap lower level framework exceptions
	/// </summary>
	public class DataAccessException : Exception
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="DataAccessException"/> class.
		/// </summary>
		/// <param name="message">The message that describes the error.</param>
		public DataAccessException(string message) : base(message)
		{
		}
		/// <summary>
		/// Initializes a new instance of the <see cref="DataAccessException"/> class.
		/// </summary>
		/// <param name="message">The error message that explains the reason for the exception.</param>
		/// <param name="innerException">The exception that is the cause of the current exception, or a null reference (Nothing in Visual Basic) if no inner exception is specified.</param>
		public DataAccessException(string message, Exception innerException) : base(message, innerException)
		{
		}
		public DataAccessException(string message, params object[] args) : base(string.Format(message, args))
		{
		}
	}
}
