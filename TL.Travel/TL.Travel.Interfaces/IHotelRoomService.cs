using TL.Travel.DomainModels.HotelRoom;
namespace TL.Travel.Interfaces
{
    public interface IHotelRoomService : IService
    {
        IQueryable<HotelRoomDTO> GetAll(HotelRoomFilters filter);

        ViewHotelRoomDTO? GetById(int id);
        ViewHotelRoomDTO? AddEdit(UpdateHotelRoomDTO room, int id = 0);

        bool Delete(int id);

        byte[] GenerateHotelRoomPhotosPdf(int roomId);


        List<HotelRoomImageInfo> GetHotelRoomImagesList(int roomId);

        byte[] GetRoomPhoto(int imageId, out string contentType, out string fileName);
    }
}
