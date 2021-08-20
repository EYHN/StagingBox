﻿using Anything.Preview.MimeType;
using Anything.Utils;
using NUnit.Framework;

namespace Anything.Tests.Preview.MimeType
{
    public class MimeTypeRulesTests
    {
        [Test]
        public void FeatureTest()
        {
            var rules = MimeTypeRules.FromJson(
                "[{\"mime\":\"image/png\",\"extensions\":[\".png\"]},{\"mime\":\"image/jpeg\",\"extensions\":[\".jpg\",\".jpeg\",\".jpe\"]},{\"mime\":\"image/bmp\",\"extensions\":[ \".bmp\"]}]");

            Assert.AreEqual(Anything.Preview.MimeType.Schema.MimeType.image_png, rules.Match(Url.Parse("file://test/a.png")));
            Assert.AreEqual(Anything.Preview.MimeType.Schema.MimeType.image_jpeg, rules.Match(Url.Parse("file://test/a.jpg")));
            Assert.AreEqual(Anything.Preview.MimeType.Schema.MimeType.image_jpeg, rules.Match(Url.Parse("file://test/a.jpeg")));
            Assert.AreEqual(Anything.Preview.MimeType.Schema.MimeType.image_jpeg, rules.Match(Url.Parse("file://test/a.jpe")));
            Assert.AreEqual(Anything.Preview.MimeType.Schema.MimeType.image_bmp, rules.Match(Url.Parse("file://test/a.bmp")));
            Assert.AreEqual(null, rules.Match(Url.Parse("file://test/a.txt")));

            Assert.Catch(() => MimeTypeRules.FromJson("{a:1}"));
            Assert.Catch(() => MimeTypeRules.FromJson(""));
        }
    }
}
