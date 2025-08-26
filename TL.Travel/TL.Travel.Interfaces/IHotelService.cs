using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TL.Travel.DomainModels.Hotel;
using TL.Travel.DomainModels.Operators;

namespace TL.Travel.Interfaces
{
    public interface IHotelService : IService
    {
        IQueryable<NomenclatureDTO> GetAllOperators();
        IQueryable<NomenclatureDTO> GetAllDestinations();
        IQueryable<NomenclatureDTO> GetAllExtras();

        IQueryable<HotelVM> GetAll();

        IQueryable<NomenclatureHotelPhotos> DownloadHotelPhoto();

        byte[] GenerateHotelPhotosPdf(int hotelId);


        List<HotelImageInfo> GetHotelImagesList(int hotelId);

        byte[] GetHotelPhoto(int imageId, out string contentType, out string fileName);

        HotelVM GetById(int id);

        HotelVM AddEdit(HotelUM body, int id = 0);

        bool Delete(int id);
    }
}
