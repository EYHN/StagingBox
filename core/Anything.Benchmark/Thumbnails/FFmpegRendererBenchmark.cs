using System.Threading.Tasks;
using Anything.FileSystem;
using Anything.FileSystem.Impl;
using Anything.Preview.Mime.Schema;
using Anything.Preview.Thumbnails;
using Anything.Preview.Thumbnails.Renderers;
using Anything.Utils;
using Anything.Utils.Logging;
using BenchmarkDotNet.Attributes;

namespace Anything.Benchmark.Thumbnails
{
    [SimpleJob]
    [RPlotExporter]
    [MemoryDiagnoser]
    public class FFmpegRendererBenchmark : Disposable
    {
        private FileHandle _exampleMp4FileHandle = null!;
        private FileService _fileService = null!;
        private ThumbnailsRenderContext _renderContext = null!;
        private FFmpegRenderer _renderer = null!;
        private ThumbnailsRenderOption _renderOption = null!;

        [GlobalSetup]
        public async Task Setup()
        {
#pragma warning disable IDISP003
            _renderContext = new ThumbnailsRenderContext();
            _fileService = new FileService(Logger.Slient);
#pragma warning restore IDISP003
            _fileService.AddFileSystem(
                "test",
                new MemoryFileSystem());
            var root = await _fileService.CreateFileHandle(Url.Parse("file://test/"));
            _exampleMp4FileHandle = await _fileService.CreateFile(
                root,
                "example.mp4",
                Resources.ReadEmbeddedFile(typeof(FFmpegRendererBenchmark).Assembly, "/Resources/example.mp4"));
            _renderer = new FFmpegRenderer(_fileService);
            _renderOption = new ThumbnailsRenderOption { Size = 512 };
        }

        [Benchmark]
        public async Task FFmpeg()
        {
            await _renderer.Render(
                _renderContext,
                new ThumbnailsRenderFileInfo(_exampleMp4FileHandle, await _fileService.Stat(_exampleMp4FileHandle), MimeType.video_mp4),
                _renderOption);
        }

        protected override void DisposeManaged()
        {
            base.DisposeManaged();

            _renderContext.Dispose();
            _fileService.Dispose();
        }
    }
}
