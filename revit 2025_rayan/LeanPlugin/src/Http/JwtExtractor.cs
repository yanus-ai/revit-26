using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace YANUSConnector.Http
{
    public class JwtExtractor
    {
        public static string ExtractJwt(string url)
        {
            // Check if the URL contains the "jwt=" parameter
            if (url.Contains("token="))
            {
                // Create a Uri object
                Uri uri = new Uri(url);

                // Get the query part of the URL
                string query = uri.Query;

                // Parse the query string
                var queryParameters = HttpUtility.ParseQueryString(query);

                // Check if "jwt" parameter exists and return its value
                if (!string.IsNullOrEmpty(queryParameters["token"]))
                {
                    return queryParameters["token"];
                }
            }

            // Return null or an empty string if "token=" is not found
            return null;
        }
    }
}
