using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aper_bot.Database.Model
{
    public class Image: Entity
    {
        public byte[] ImageData { get; set; }

        public Image(byte[] imageData)
        {
            ImageData = imageData;
        }
    }
}
