﻿using System.Threading.Tasks;
using Anything.FileSystem;
using Anything.FileSystem.Impl;
using Anything.Preview.Mime.Schema;
using Anything.Preview.Thumbnails;
using Anything.Preview.Thumbnails.Renderers;
using Anything.Utils;
using Microsoft.Extensions.FileProviders;
using NUnit.Framework;

namespace Anything.Tests.Preview.Thumbnails.Renderers
{
    [TestFixture]
    public class ImageFileRendererTests
    {
        [Test]
        public async Task TestRenderImageIcon()
        {
            using var renderContext = new ThumbnailsRenderContext();
            using var fileService = new FileService();
            fileService.AddFileSystem(
                "test",
                new EmbeddedFileSystem(new EmbeddedFileProvider(typeof(ImageFileRendererTests).Assembly)));
            IThumbnailsRenderer renderer = new ImageFileRenderer(fileService);

            async ValueTask<ThumbnailsRenderFileInfo> MakeFileInfo(
                IFileService fs,
                string filename,
                MimeType? mimeType = null)
            {
                var fileHandle = await fs.CreateFileHandle(Url.Parse("file://test/Resources/" + filename));
                return new ThumbnailsRenderFileInfo(
                    fileHandle,
                    await fs.Stat(fileHandle),
                    mimeType ?? MimeType.image_png);
            }

            var renderOption = new ThumbnailsRenderOption();
            renderContext.Resize(512, 512, false);
            await renderer.Render(renderContext, await MakeFileInfo(fileService, "Test Image.png"), renderOption with { Size = 512 });
            await renderContext.SaveTestResult("512w");

            renderContext.Resize(1024, 1024, false);
            await renderer.Render(renderContext, await MakeFileInfo(fileService, "Test Image.png"), renderOption with { Size = 1024 });
            await renderContext.SaveTestResult("1024w");

            renderContext.Resize(256, 256, false);
            await renderer.Render(renderContext, await MakeFileInfo(fileService, "Test Image.png"), renderOption with { Size = 256 });
            await renderContext.SaveTestResult("256w");

            renderContext.Resize(128, 128, false);
            await renderer.Render(renderContext, await MakeFileInfo(fileService, "Test Image.png"), renderOption with { Size = 128 });
            await renderContext.SaveTestResult("128w");

            renderContext.Resize(512, 512, false);
            await renderer.Render(renderContext, await MakeFileInfo(fileService, "20000px.png"), renderOption with { Size = 512 });
            await renderContext.SaveTestResult("Large Pixels");

            await renderer.Render(
                renderContext,
                await MakeFileInfo(fileService, "EXIF Orientation Tag.jpg", MimeType.image_jpeg),
                renderOption with { Size = 512 });
            await renderContext.SaveTestResult("EXIF Orientation Tag");

            await renderer.Render(
                renderContext,
                await MakeFileInfo(fileService, "Grayscale With Alpha.png"),
                renderOption with { Size = 512 });
            await renderContext.SaveTestResult("Grayscale With Alpha");

            await renderer.Render(
                renderContext,
                await MakeFileInfo(fileService, "Grayscale.jpg", MimeType.image_jpeg),
                renderOption with { Size = 512 });
            await renderContext.SaveTestResult("Grayscale");

            await renderer.Render(
                renderContext,
                await MakeFileInfo(fileService, "Pdf Sample.pdf", MimeType.application_pdf),
                renderOption with { Size = 512 });
            await renderContext.SaveTestResult("Pdf Sample");
        }
    }
}
