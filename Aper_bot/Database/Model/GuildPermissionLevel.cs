using Aper_bot.Modules.Commands;

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Database.Model
{
    public class GuildPermissionLevel : Entity
    {
        
        public int GuildID { get; set; }
        
        public string RoleID { get; set; }

        public PermissionLevels PermissionLevel { get; set; }

        public GuildPermissionLevel(string roleID, PermissionLevels permissionLevel)
        {
            RoleID = roleID;
            PermissionLevel = permissionLevel;
        }
    }
}
