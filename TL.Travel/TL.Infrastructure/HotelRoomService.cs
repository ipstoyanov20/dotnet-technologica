using System.Security.Cryptography;
using SixLabors.ImageSharp;
using SixLabors.ImageSharp.Processing;
using SixLabors.ImageSharp.Formats.Jpeg;
using SixLabors.ImageSharp.Formats.Png;
using Syncfusion.Drawing;
using Syncfusion.Pdf;
using Syncfusion.Pdf.Graphics;
using TL.DataAccess.Models;
using TL.Travel.DataAccess.Base;
using TL.Travel.DomainModels.HotelRoom;
using TL.Travel.Interfaces;

namespace TL.Travel.Infrastructure
{
    public class HotelRoomService : BaseService, IHotelRoomService
    {
        public HotelRoomService(BaseTLTravelDbContext dbContext)
            : base(dbContext)
        {
        }

        public IQueryable<HotelRoomDTO> GetAll(HotelRoomFilters filter)
        {
            var query = Db.HotelRooms.Where(x => x.IsActive);

            if (filter != null && filter.HasAnyFilters())
            {
                if (!string.IsNullOrEmpty(filter.Name))
                {
                    query = query.Where(room => room.Name.ToLower().StartsWith(filter.Name.ToLower()));
                }
            }

            return query.Select(room => new HotelRoomDTO
            {
                Id = room.Id,
                Name = room.Name
            });
        }

        public ViewHotelRoomDTO? GetById(int id)
        {
            var room = Db.HotelRooms.FirstOrDefault(r => r.Id == id && r.IsActive);
            if (room == null) return null;

            return new ViewHotelRoomDTO
            {
                Id = room.Id,
                Name = room.Name,
                Price = room.Price,
                Description = room.Description,
                IsActive = room.IsActive,
                MaxAdults = room.MaxAdults,
                MaxChildren = room.MaxChildren,
                MaxBabies = room.MaxBabies,
                HotelId = room.HotelId
            };
        }

        public byte[] GenerateHotelRoomPhotosPdf(int roomId)
        {
            var room = Db.HotelRooms.FirstOrDefault(r => r.Id == roomId && r.IsActive);
            if (room == null)
                throw new ArgumentException("Hotel room not found");

            var photos = Db.RoomPhotos.Where(x => x.HotelRoomId == roomId && x.IsActive).ToList();
            if (!photos.Any())
                throw new ArgumentException("No photos found for this hotel room");

            using var document = new PdfDocument();

            foreach (var photo in photos)
            {
                var page = document.Pages.Add();
                var graphics = page.Graphics;

                using var imageStream = new MemoryStream(photo.Photo);
                var pdfImage = PdfImage.FromStream(imageStream);

                var pageWidth = page.GetClientSize().Width;
                var pageHeight = page.GetClientSize().Height;

                var imageWidth = pdfImage.Width;
                var imageHeight = pdfImage.Height;

                var widthRatio = pageWidth / imageWidth;
                var heightRatio = pageHeight / imageHeight;
                var ratio = Math.Min(widthRatio, heightRatio);

                var scaledWidth = imageWidth * ratio;
                var scaledHeight = imageHeight * ratio;

                var x = (pageWidth - scaledWidth) / 2;
                var y = (pageHeight - scaledHeight) / 2;

                graphics.DrawImage(pdfImage, (float)x, (float)y, (float)scaledWidth, (float)scaledHeight);
            }

            using var stream = new MemoryStream();
            document.Save(stream);
            return stream.ToArray();
        }

        private string GetImageExtension(byte[] imageBytes)
        {
            using (var ms = new MemoryStream(imageBytes))
            {
                var img = Syncfusion.Drawing.Image.FromStream(ms);
                try
                {
                    if (img.RawFormat.Equals(ImageFormat.Jpeg))
                        return ".jpg";
                    if (img.RawFormat.Equals(ImageFormat.Png))
                        return ".png";
                    if (img.RawFormat.Equals(ImageFormat.Gif))
                        return ".gif";
                    if (img.RawFormat.Equals(ImageFormat.Bmp))
                        return ".bmp";
                    if (img.RawFormat.Equals(ImageFormat.Tiff))
                        return ".tiff";

                    return ".jpg";
                }
                finally
                {
                    if (img is IDisposable disposable)
                    {
                        disposable.Dispose();
                    }
                }
            }
        }

        private string ValidateAndProcessImage(byte[] imageBytes, out byte[] processedImageBytes)
        {
            processedImageBytes = null;

            if (imageBytes == null || imageBytes.Length == 0)
                return "Image data is empty";

            var contentType = GetContentType(imageBytes);
            if (contentType != "image/png" && contentType != "image/jpeg")
                return "Only PNG and JPEG formats are allowed";

            try
            {
                using var image = SixLabors.ImageSharp.Image.Load(imageBytes);

                if (image.Width > 1920 || image.Height > 1080)
                {
                    processedImageBytes = ResizeImageToFullHD(imageBytes, contentType);
                }
                else
                {
                    processedImageBytes = imageBytes;
                }

                return null;
            }
            catch (Exception ex)
            {
                return $"Invalid image format: {ex.Message}";
            }
        }

        private byte[] ResizeImageToFullHD(byte[] originalImageBytes, string contentType)
        {
            const int maxWidth = 1920;
            const int maxHeight = 1080;

            using var image = SixLabors.ImageSharp.Image.Load(originalImageBytes);

            var ratioX = (double)maxWidth / image.Width;
            var ratioY = (double)maxHeight / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);

            image.Mutate(x => x.Resize(newWidth, newHeight));

            using var outputStream = new MemoryStream();

            if (contentType == "image/png")
            {
                image.Save(outputStream, new PngEncoder());
            }
            else
            {
                image.Save(outputStream, new JpegEncoder { Quality = 90 });
            }

            return outputStream.ToArray();
        }

        private string CalculateImageHash(byte[] imageBytes)
        {
            using var sha256 = SHA256.Create();
            var hash = sha256.ComputeHash(imageBytes);
            return Convert.ToBase64String(hash);
        }

        public ViewHotelRoomDTO? AddEdit(UpdateHotelRoomDTO room, int id = 0)
        {
            HotelRoom hotelroom;
            if (id == 0)
            {
                hotelroom = new HotelRoom
                {
                    Name = room.Name,
                    Price = room.Price,
                    Description = room.Description,
                    IsActive = room.IsActive,
                    MaxAdults = room.MaxAdults,
                    MaxChildren = room.MaxChildren,
                    MaxBabies = room.MaxBabies,
                    HotelId = room.HotelId
                };

                Db.HotelRooms.Add(hotelroom);
            }
            else
            {
                hotelroom = Db.HotelRooms.FirstOrDefault(r => r.Id == id && r.IsActive);

                hotelroom.Name = room.Name;
                hotelroom.Price = room.Price;
                hotelroom.Description = room.Description;
                hotelroom.IsActive = room.IsActive;
                hotelroom.MaxAdults = room.MaxAdults;
                hotelroom.MaxChildren = room.MaxChildren;
                hotelroom.MaxBabies = room.MaxBabies;
            }

            Db.SaveChanges();
            if (room.RoomPhoto != null && room.RoomPhoto.Count > 0)
            {
                var validatedPhotos = new List<(byte[] photoBytes, string hash)>();

                foreach (var file in room.RoomPhoto)
                {
                    if (file.Length > 0)
                    {
                        byte[] photoBytes;
                        using (var memoryStream = new MemoryStream())
                        {
                            file.CopyTo(memoryStream);
                            photoBytes = memoryStream.ToArray();
                        }

                        var validationError = ValidateAndProcessImage(photoBytes, out byte[] processedImageBytes);
                        if (validationError != null)
                            throw new ArgumentException($"Image validation failed: {validationError}");

                        var imageHash = CalculateImageHash(processedImageBytes);
                        validatedPhotos.Add((processedImageBytes, imageHash));
                    }
                }

                var requestPhotoHashes = validatedPhotos.Select(p => p.hash).ToList();
                var existingRoomPhotos = Db.RoomPhotos.Where(x => x.HotelRoomId == hotelroom.Id).ToList();
                var dbPhotoHashes = existingRoomPhotos.Select(p => CalculateImageHash(p.Photo)).ToList();

                var toBeAdded = requestPhotoHashes.Except(dbPhotoHashes).ToList();
                foreach (var photoHash in toBeAdded)
                {
                    var photoData = validatedPhotos.First(p => p.hash == photoHash);

                    var newPhoto = new RoomPhoto
                    {
                        HotelRoomId = hotelroom.Id,
                        Photo = photoData.photoBytes,
                        IsActive = true
                    };

                    Db.RoomPhotos.Add(newPhoto);
                }

                var toBeDeleted = dbPhotoHashes.Except(requestPhotoHashes).ToList();
                foreach (var photoHash in toBeDeleted)
                {
                    var toRemove = existingRoomPhotos.FirstOrDefault(p => CalculateImageHash(p.Photo) == photoHash);
                    if (toRemove != null)
                        Db.RoomPhotos.Remove(toRemove);
                }

                Db.SaveChanges();
            }

            if (room.RoomExtras != null && room.RoomExtras.Count > 0)
            {
                var requestRoomExtraIds = room.RoomExtras;
                var existingRoomExtras = Db.HotelRoomExtras
                    .Where(x => x.HotelRoomId == hotelroom.Id)
                    .ToList();

                var dbRoomExtraKeys = existingRoomExtras.Select(e => (e.HotelRoomId, e.ExtrasId)).ToList();
                var requestRoomExtraKeys = requestRoomExtraIds.Select(extraId => (hotelroom.Id, extraId)).ToList();

                var toBeAdded = requestRoomExtraKeys.Except(dbRoomExtraKeys).ToList();
                foreach (var (roomId, extraId) in toBeAdded)
                {
                    if (Db.Extras.Any(x => x.Id == extraId))
                    {
                        Db.HotelRoomExtras.Add(new HotelRoomExtra
                        {
                            HotelRoomId = roomId,
                            ExtrasId = extraId,
                            IsActive = true
                        });
                    }
                }

                var toBeDeleted = dbRoomExtraKeys.Except(requestRoomExtraKeys).ToList();
                foreach (var (roomId, extraId) in toBeDeleted)
                {
                    var toRemove = existingRoomExtras.FirstOrDefault(x => x.ExtrasId == extraId);
                    if (toRemove != null)
                        Db.HotelRoomExtras.Remove(toRemove);
                }

                var toBeUpdated = dbRoomExtraKeys.Intersect(requestRoomExtraKeys).ToList();
                foreach (var (roomId, extraId) in toBeUpdated)
                {
                    var toUpdate = existingRoomExtras.FirstOrDefault(x => x.ExtrasId == extraId);
                    if (toUpdate != null)
                    {
                        toUpdate.IsActive = true;
                    }
                }

                Db.SaveChanges();
            }

            return new ViewHotelRoomDTO
            {
                Id = hotelroom.Id,
                Name = hotelroom.Name,
                Price = hotelroom.Price,
                Description = hotelroom.Description,
                IsActive = hotelroom.IsActive,
                MaxAdults = hotelroom.MaxAdults,
                MaxChildren = hotelroom.MaxChildren,
                MaxBabies = hotelroom.MaxBabies,
                HotelId = hotelroom.HotelId
            };
        }

        public bool Delete(int id)
        {
            var room = Db.HotelRooms.FirstOrDefault(r => r.Id == id && r.IsActive);
            if (room == null) return false;

            room.IsActive = false;
            Db.SaveChanges();
            return true;
        }

        public List<HotelRoomImageInfo> GetHotelRoomImagesList(int roomId)
        {
            var room = Db.HotelRooms.FirstOrDefault(r => r.Id == roomId && r.IsActive);
            if (room == null)
                throw new ArgumentException("Hotel room not found");

            var photos = Db.RoomPhotos.Where(x => x.HotelRoomId == roomId && x.IsActive).ToList();

            var imageInfos = new List<HotelRoomImageInfo>();
            for (int i = 0; i < photos.Count; i++)
            {
                var photo = photos[i];
                var contentType = GetContentType(photo.Photo);
                var extension = GetImageExtension(photo.Photo);

                imageInfos.Add(new HotelRoomImageInfo
                {
                    ImageIndex = i,
                    ImageName = $"room_{roomId}_photo_{i + 1}{extension}",
                    ContentType = contentType,
                    ImageId = photo.Id
                });
            }

            return imageInfos;
        }

        public byte[] GetRoomPhoto(int imageId, out string contentType, out string fileName)
        {

            var photo = Db.RoomPhotos.FirstOrDefault(x => x.Id == imageId && x.IsActive);
            if (photo == null)
                throw new ArgumentException("Photo not found");

            contentType = GetContentType(photo.Photo);
            var extension = GetImageExtension(photo.Photo);
            fileName = $"room_photo_{imageId}{extension}";

            return photo.Photo;
        }

        private string GetContentType(byte[] imageBytes)
        {
            if (imageBytes.Length >= 2)
            {
                if (imageBytes[0] == 0xFF && imageBytes[1] == 0xD8)
                    return "image/jpeg";
                if (imageBytes[0] == 0x89 && imageBytes[1] == 0x50)
                    return "image/png";
                if (imageBytes[0] == 0x47 && imageBytes[1] == 0x49)
                    return "image/gif";
                if (imageBytes[0] == 0x42 && imageBytes[1] == 0x4D)
                    return "image/bmp";
            }
            return "image/jpeg";
        }
    }
}
