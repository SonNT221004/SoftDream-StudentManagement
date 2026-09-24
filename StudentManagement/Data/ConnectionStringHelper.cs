using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace StudentManagement.Data
{
    public static class ConnectionStringHelper
    {
        /// <summary>
        /// Convert a PostgreSQL URI (postgres:// or postgresql://) into an ADO.NET-style connection string
        /// that Npgsql/NHibernate can consume.
        /// If input is already a key=value connection string, it is returned unchanged.
        /// </summary>
        public static string NormalizePostgresConnectionString(string? conn)
        {
            if (string.IsNullOrWhiteSpace(conn))
                return string.Empty;

            conn = conn.Trim();

            if (!conn.StartsWith("postgres://", StringComparison.OrdinalIgnoreCase) &&
                !conn.StartsWith("postgresql://", StringComparison.OrdinalIgnoreCase))
            {
                return conn;
            }

            var uri = new Uri(conn);
            var userInfo = (uri.UserInfo ?? string.Empty).Split(':', 2);
            var username = userInfo.Length > 0 ? Uri.UnescapeDataString(userInfo[0]) : string.Empty;
            var password = userInfo.Length > 1 ? Uri.UnescapeDataString(userInfo[1]) : string.Empty;
            var host = uri.Host;
            var port = uri.IsDefaultPort ? 5432 : uri.Port;
            var database = uri.AbsolutePath?.TrimStart('/') ?? string.Empty;

            // parse query string into dictionary
            var query = (uri.Query ?? string.Empty).TrimStart('?');
            var queryParams = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            if (!string.IsNullOrEmpty(query))
            {
                var pairs = query.Split('&', StringSplitOptions.RemoveEmptyEntries);
                foreach (var pair in pairs)
                {
                    var idx = pair.IndexOf('=');
                    if (idx >= 0)
                    {
                        var key = Uri.UnescapeDataString(pair.Substring(0, idx));
                        var value = Uri.UnescapeDataString(pair.Substring(idx + 1));
                        queryParams[key] = value;
                    }
                    else
                    {
                        queryParams[Uri.UnescapeDataString(pair)] = string.Empty;
                    }
                }
            }

            queryParams.TryGetValue("sslmode", out var sslModeRaw);

            var sb = new StringBuilder();
            sb.Append($"Host={host};Port={port};Database={database};Username={username};Password={password};");

            if (!string.IsNullOrEmpty(sslModeRaw))
            {
                var mapped = sslModeRaw.Trim().ToLowerInvariant() switch
                {
                    "disable" => "Disable",
                    "allow" => "Allow",
                    "prefer" => "Prefer",
                    "require" => "Require",
                    "verify-ca" or "verify_ca" => "VerifyCA",
                    "verify-full" or "verify_full" => "VerifyFull",
                    var s => CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s)
                };

                sb.Append($"Ssl Mode={mapped};Trust Server Certificate=true;");
            }

            return sb.ToString();
        }
    }
}
