using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TL.Travel.DomainModels.Hotel
{
    public class HotelImageInfo
    {
        public int ImageIndex { get; set; }
        public string ImageName { get; set; }
        public string ContentType { get; set; }
        public int ImageId { get; set; }
    }
    
    public class NomenclatureHotelPhotos
    {
        public List<byte[]> Photos { get; set; } = new List<byte[]>();
        public List<HotelImageInfo> ImageInfos { get; set; } = new List<HotelImageInfo>();
    }
}
