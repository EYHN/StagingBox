﻿using System.Threading.Tasks;
using Anything.Preview.Mime;
using Anything.Preview.Mime.Schema;
using Anything.Utils;
using NUnit.Framework;

namespace Anything.Tests.Preview.Mime
{
    public class MimeTypeServiceTest
    {
        [Test]
        public async Task FeatureTest()
        {
            var service = new MimeTypeService(
                MimeTypeRules.FromJson(
                    "[{\"mime\":\"image/png\",\"extensions\":[\".png\"]},{\"mime\":\"image/jpeg\",\"extensions\":[\".jpg\",\".jpeg\",\".jpe\"]},{\"mime\":\"image/bmp\",\"extensions\":[ \".bmp\"]}]"));

            Assert.AreEqual(MimeType.image_png, await service.GetMimeType(Url.Parse("file://test/a.png"), new MimeTypeOption()));
            Assert.AreEqual(MimeType.image_jpeg, await service.GetMimeType(Url.Parse("file://test/a.jpg"), new MimeTypeOption()));
            Assert.AreEqual(MimeType.image_jpeg, await service.GetMimeType(Url.Parse("file://test/a.jpeg"), new MimeTypeOption()));
            Assert.AreEqual(MimeType.image_jpeg, await service.GetMimeType(Url.Parse("file://test/a.jpe"), new MimeTypeOption()));
            Assert.AreEqual(MimeType.image_bmp, await service.GetMimeType(Url.Parse("file://test/a.bmp"), new MimeTypeOption()));
            Assert.AreEqual(null, await service.GetMimeType(Url.Parse("file://test/a.txt"), new MimeTypeOption()));
        }
    }
}
