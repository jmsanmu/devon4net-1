using Devon4Net.Infrastructure.AnsibleTower.Dto.Common;
using System;
using System.Collections.Generic;
using System.Text;

namespace Devon4Net.Infrastructure.AnsibleTower.Dto.Credentials
{
    public class GetCredentialsResponseDto
    {

            public int id { get; set; }
            public string type { get; set; }
            public string url { get; set; }
            public RelatedCredentials related { get; set; }
            public Summary_Fields summary_fields { get; set; }
            public DateTime created { get; set; }
            public DateTime modified { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public int organization { get; set; }
            public int credential_type { get; set; }
            public Dictionary<string, string> inputs { get; set; }
        }

        public class RelatedCredentials
    {
            public string created_by { get; set; }
            public string modified_by { get; set; }
            public string organization { get; set; }
            public string activity_stream { get; set; }
            public string access_list { get; set; }
            public string object_roles { get; set; }
            public string owner_users { get; set; }
            public string owner_teams { get; set; }
            public string copy { get; set; }
            public string input_sources { get; set; }
            public string credential_type { get; set; }
        }

        public class Summary_Fields
        {
            public Organization organization { get; set; }
            public object host { get; set; }
            public object project { get; set; }
            public Created_By created_by { get; set; }
            public Modified_By modified_by { get; set; }
            public Dictionary<string, RoleItems> object_roles { get; set; }
            public User_Capabilities user_capabilities { get; set; }
            public Owner[] owners { get; set; }
        }

        public class User_Capabilities
        {
            public bool edit { get; set; }
            public bool delete { get; set; }
            public bool copy { get; set; }
            public bool use { get; set; }
        }

        public class Owner
        {
            public int id { get; set; }
            public string type { get; set; }
            public string name { get; set; }
            public string description { get; set; }
            public string url { get; set; }
        }
}
