using System;
using System.Data.Common;
using System.IO;

namespace OgreDA.DataAccess
{
	public static class DbDataReaderExtensions
	{
		public static string? SafeGetString(this DbDataReader reader, int ordinal)
		{
			return reader.IsDBNull(ordinal) ? null : reader.GetString(ordinal);
		}
		public static string? SafeGetString(this DbDataReader reader, string columnName)
		{
			int ordinal = reader.GetOrdinal(columnName);
			return reader.SafeGetString(ordinal);
		}
		public static object? SafeGetValue(this DbDataReader reader, int ordinal)
		{
			return reader.IsDBNull(ordinal) ? null : reader.GetValue(ordinal);
		}
		public static string? GetString(this DbDataReader reader, string name)
		{
			int ordinal = reader.GetOrdinal(name);
			return reader.IsDBNull(ordinal) ? null : reader.SafeGetString(ordinal);
		}
		public static int SafeGetInt32(this DbDataReader reader, int ordinal, int defaultVal = 0)
		{
			if (reader.IsDBNull(ordinal))
				return defaultVal;

			try
			{
				return reader.GetInt32(ordinal);
			}
			catch (InvalidCastException)
			{
				int value = defaultVal;
				int.TryParse(reader.GetValue(ordinal).ToString(), out value);
				return value;
			}
			catch (Exception)
			{
				return defaultVal;
			}
		}
		public static int GetInt32(this DbDataReader reader, string name, int defaultVal = 0)
		{
			int ordinal = reader.GetOrdinal(name);
			return reader.SafeGetInt32(ordinal, defaultVal);
		}
		public static Guid SafeGetGuid(this DbDataReader reader, string columnName)
		{
			int ordinal = reader.GetOrdinal(columnName);
			return reader.SafeGetGuid(ordinal);
		}
		public static Guid SafeGetGuid(this DbDataReader reader, int ordinal)
		{
			if (reader.IsDBNull(ordinal))
				return Guid.Empty;

			if (maySupportGuid(reader))
			{
				try
				{
					return reader.GetGuid(ordinal);
				}
				catch
				{
					// Guid not supported
				}
			}

			try
			{
				byte[] bytes = new byte[16];
				reader.GetBytes(ordinal, 0, bytes, 0, 16);
				return new Guid(bytes);
			}
			catch (Exception e)
			{
				throw new DataAccessException("Could not convert the data in the column to a Guid.", e);
			}
		}

		public static Guid GetGuid(this DbDataReader reader, string name)
		{
			int ordinal = reader.GetOrdinal(name);
			return reader.SafeGetGuid(ordinal);
		}
		public static byte[]? GetBinary(this DbDataReader reader, int ordinal)
		{
			if (reader.IsDBNull(ordinal))
				return null;

			try
			{
				MemoryStream stream = new MemoryStream();
				reader.GetStream(ordinal).CopyTo(stream);
				return stream.ToArray();
			}
			catch (Exception e)
			{
				throw new DataAccessException("Could not read column into a byte array.", e);
			}
		}
		public static byte[]? GetBinary(this DbDataReader reader, string name)
		{
			int ordinal = reader.GetOrdinal(name);
			return reader.GetBinary(ordinal);
		}

		private static bool maySupportGuid(DbDataReader reader)
		{
			if (reader.GetType().FullName!.Contains("Oracle"))
				return false;
			return true;
		}
	}
}
