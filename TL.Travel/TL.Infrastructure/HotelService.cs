using System.Drawing;
using System.IO.Compression;
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
using TL.Travel.DomainModels.Hotel;
using TL.Travel.DomainModels.Operators;
using TL.Travel.Interfaces;

namespace TL.Travel.Infrastructure
{
    public class HotelService : BaseService, IHotelService
    {
        private BaseTLTravelDbContext dbContext;
        private IOperatorService opratorService;
        public HotelService(BaseTLTravelDbContext dbContext, IOperatorService opratorService) : base(dbContext)
        {
            this.opratorService = opratorService;
        }

        public IQueryable<HotelVM> GetAll()
        {
            var query = Db.Hotels.Where(x => x.IsActive);
            return query.Join(Db.Locations, h => h.LocationId, l => l.Id, (h, l) => new HotelVM
            {
                Id = h.Id,
                Name = h.Name,
                Stars = h.Stars,
                Contacts = h.Contacts,
                IsTemporaryClosed = h.IsTemporaryClosed,
                DestinationName = l.Name
            });
        }

        public IQueryable<NomenclatureHotelPhotos> DownloadHotelPhoto()
        {
            throw new NotImplementedException();
        }

        public byte[] GenerateHotelPhotosPdf(int hotelId)
        {
            var hotel = Db.Hotels.FirstOrDefault(h => h.Id == hotelId && h.IsActive);
            if (hotel == null)
                throw new ArgumentException("Hotel not found");

            var photos = Db.HotelPhotos.Where(x => x.HotelId == hotelId && x.IsActive).ToList();
            if (!photos.Any())
                throw new ArgumentException("No photos found for this hotel");

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

        public IQueryable<NomenclatureDTO> GetAllOperators()
        {
            var res = Db.Agents.Where(x => x.IsActive);

            return res.Select(x => new NomenclatureDTO()
            {
                Id = x.Id,
                Name = x.Name
            });
        }

        public IQueryable<NomenclatureDTO> GetAllDestinations()
        {
            var res = Db.Locations.Where(x => x.IsActive);
            return res.Select(x => new NomenclatureDTO()
            {
                Id = x.Id,
                Name = x.Name
            });
        }

        public IQueryable<NomenclatureDTO> GetAllExtras()
        {
            var res = Db.Extras.Where(x => x.IsActive);
            return res.Select(x => new NomenclatureDTO()
            {
                Id = x.Id,
                Name = x.Name
            });
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

        public HotelVM AddEdit(HotelUM body, int id = 0)
        {
            if (body == null)
                throw new ArgumentNullException(nameof(body), "Hotel data is required.");

            Hotel res;

            if (id > 0)
            {
                res = Db.Hotels.Find(id);
                if (res == null)
                    throw new Exception("Hotel not found.");

                res.Name = body.Name;
                res.Stars = body.Stars;
                res.Contacts = body.Contacts;
                res.IsTemporaryClosed = body.IsTemporaryClosed;
            }
            else
            {
                res = new Hotel
                {
                    Name = body.Name,
                    Stars = body.Stars,
                    Contacts = body.Contacts,
                    IsTemporaryClosed = body.IsTemporaryClosed,
                    IsActive = true,
                    PartnerId = body.PartnerId.Value,
                    LocationId = body.LocationId.Value
                };
                Db.Hotels.Add(res);
            }

            Db.SaveChanges();
            if (body.Extras != null && body.Extras.Count > 0)
            {
                var requestExtraIds = body.Extras;
                var existingExtras = Db.HotelExtras
                    .Where(x => x.HotelId == res.Id)
                    .ToList();

                var dbExtraIds = existingExtras.Select(e => e.ExtrasId).ToList();

                var toBeAdded = requestExtraIds.Except(dbExtraIds).ToList();
                foreach (var extraId in toBeAdded)
                {
                    if (Db.Extras.Any(x => x.Id == extraId))
                    {
                        Db.HotelExtras.Add(new HotelExtra
                        {
                            HotelId = res.Id,
                            ExtrasId = extraId,
                            IsActive = true
                        });
                    }
                }

                var toBeDeleted = dbExtraIds.Except(requestExtraIds).ToList();
                foreach (var extraId in toBeDeleted)
                {
                    var toRemove = existingExtras.FirstOrDefault(x => x.ExtrasId == extraId);
                    if (toRemove != null)
                        Db.HotelExtras.Remove(toRemove);
                }

                var toBeUpdated = dbExtraIds.Intersect(requestExtraIds).ToList();
                foreach (var extraId in toBeUpdated)
                {
                    var toUpdate = existingExtras.FirstOrDefault(x => x.ExtrasId == extraId);
                    if (toUpdate != null)
                    {
                        toUpdate.IsActive = true;
                    }
                }

                Db.SaveChanges();
            }

            if (body.FeedingsTypeId != null && body.FeedingsTypeId.Count > 0)
            {
                var requestFeedingTypeIds = body.FeedingsTypeId;
                var existingFeedingTypes = Db.HotelFeedingTypes
                    .Where(x => x.HotelId == res.Id)
                    .ToList();

                var dbFeedingTypeIds = existingFeedingTypes.Select(e => e.FeedingTypeId).ToList();

                var toBeAdded = requestFeedingTypeIds.Except(dbFeedingTypeIds).ToList();
                foreach (var feedingTypeId in toBeAdded)
                {
                    if (Db.FeedingTypes.Any(x => x.Id == feedingTypeId))
                    {
                        Db.HotelFeedingTypes.Add(new HotelFeedingType
                        {
                            HotelId = res.Id,
                            FeedingTypeId = feedingTypeId,
                            IsActive = true
                        });
                    }
                }

                var toBeDeleted = dbFeedingTypeIds.Except(requestFeedingTypeIds).ToList();
                foreach (var feedingTypeId in toBeDeleted)
                {
                    var toRemove = existingFeedingTypes.FirstOrDefault(x => x.FeedingTypeId == feedingTypeId);
                    if (toRemove != null)
                        Db.HotelFeedingTypes.Remove(toRemove);
                }

                var toBeUpdated = dbFeedingTypeIds.Intersect(requestFeedingTypeIds).ToList();
                foreach (var feedingTypeId in toBeUpdated)
                {
                    var toUpdate = existingFeedingTypes.FirstOrDefault(x => x.FeedingTypeId == feedingTypeId);
                    if (toUpdate != null)
                    {
                        toUpdate.IsActive = true;
                    }
                }

                Db.SaveChanges();
            }

            if (body.Photo != null && body.Photo.Count > 0)
            {
                var validatedPhotos = new List<(byte[] photoBytes, string hash)>();

                foreach (var file in body.Photo)
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
                var existingPhotos = Db.HotelPhotos.Where(x => x.HotelId == res.Id).ToList();
                var dbPhotoHashes = existingPhotos.Select(p => CalculateImageHash(p.Photo)).ToList();

                var toBeAdded = requestPhotoHashes.Except(dbPhotoHashes).ToList();
                foreach (var photoHash in toBeAdded)
                {
                    var photoData = validatedPhotos.First(p => p.hash == photoHash);

                    var newPhoto = new HotelPhoto
                    {
                        HotelId = res.Id,
                        Photo = photoData.photoBytes,
                        IsActive = true
                    };

                    Db.HotelPhotos.Add(newPhoto);
                }

                var toBeDeleted = dbPhotoHashes.Except(requestPhotoHashes).ToList();
                foreach (var photoHash in toBeDeleted)
                {
                    var toRemove = existingPhotos.FirstOrDefault(p => CalculateImageHash(p.Photo) == photoHash);
                    if (toRemove != null)
                        Db.HotelPhotos.Remove(toRemove);
                }

                Db.SaveChanges();
            }

            var loc = Db.Hotels.Where(x => x.IsActive)
            .Join(Db.Locations, h => h.LocationId, l => l.Id, (h, l) => new { h, l }).FirstOrDefault();
            return new HotelVM
            {
                Id = res.Id,
                Name = res.Name,
                Stars = res.Stars,
                Contacts = res.Contacts,
                IsTemporaryClosed = res.IsTemporaryClosed,
                DestinationName = loc.l.Name
            };
        }

        public HotelVM? GetById(int id)
        {
            var hotel = Db.Hotels.FirstOrDefault(h => h.Id == id && h.IsActive);
            var res = Db.Hotels.Where(x => x.IsActive)
            .Join(Db.Locations, h => h.LocationId, l => l.Id, (h, l) => new { h, l }).FirstOrDefault();
            if (hotel == null) return null;
            return new HotelVM
            {
                Id = hotel.Id,
                Name = hotel.Name,
                Stars = hotel.Stars,
                Contacts = hotel.Contacts,
                DestinationName = res.l.Name,
                IsTemporaryClosed = hotel.IsTemporaryClosed,
            };
        }

        public bool Delete(int id)
        {
            var hotel = Db.Hotels.FirstOrDefault(h => h.Id == id && h.IsActive);
            if (hotel == null) return false;
            SoftDeleteRowInDatabase<Hotel>(hotel);

            Db.SaveChanges();
            return true;
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

        public List<HotelImageInfo> GetHotelImagesList(int hotelId)
        {
            var hotel = Db.Hotels.FirstOrDefault(h => h.Id == hotelId && h.IsActive);
            if (hotel == null)
                throw new ArgumentException("Hotel not found");

            var photos = Db.HotelPhotos.Where(x => x.HotelId == hotelId && x.IsActive).ToList();

            var imageInfos = new List<HotelImageInfo>();
            for (int i = 0; i < photos.Count; i++)
            {
                var photo = photos[i];
                var contentType = GetContentType(photo.Photo);
                var extension = GetImageExtension(photo.Photo);

                imageInfos.Add(new HotelImageInfo
                {
                    ImageIndex = i,
                    ImageName = $"hotel_{hotelId}_photo_{i + 1}{extension}",
                    ContentType = contentType,
                    ImageId = photo.Id
                });
            }

            return imageInfos;
        }

        public byte[] GetHotelPhoto(int imageId, out string contentType, out string fileName)
        {


            var photo = Db.HotelPhotos.FirstOrDefault(x => x.Id == imageId && x.IsActive);
            if (photo == null)
                throw new ArgumentException("Photo not found");

            contentType = GetContentType(photo.Photo);
            var extension = GetImageExtension(photo.Photo);
            fileName = $"hotel_photo_{imageId}{extension}";

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
