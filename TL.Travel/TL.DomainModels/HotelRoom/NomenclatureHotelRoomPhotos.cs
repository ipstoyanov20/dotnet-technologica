using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TL.Travel.DomainModels.HotelRoom
{
    public class HotelRoomImageInfo
    {
        public int ImageIndex { get; set; }
        public string ImageName { get; set; }
        public string ContentType { get; set; }
        public int ImageId { get; set; }
    }
    
    public class NomenclatureHotelRoomPhotos
    {
        public List<byte[]> Photos { get; set; } = new List<byte[]>();
        public List<HotelRoomImageInfo> ImageInfos { get; set; } = new List<HotelRoomImageInfo>();
    }
}
