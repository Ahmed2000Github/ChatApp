
namespace ChatAppShared
{
    public class ProgressableStreamContent : HttpContent
    {
        private const int _defaultBufferSize = 4096;
        private readonly HttpContent _content;
        private readonly Func<int,Task> _progress;
        private readonly int _bufferSize;
        private readonly CancellationToken _cancellationToken;

        public ProgressableStreamContent(HttpContent content, Func<int, Task> progress, CancellationToken cancellationToken = default, int bufferSize = _defaultBufferSize)
        {
            this._content = content ?? throw new ArgumentNullException(nameof(content));
            this._progress = progress ?? throw new ArgumentNullException(nameof(progress));
            this._bufferSize = bufferSize;
            _cancellationToken = cancellationToken;
            foreach (var header in content.Headers)
            {
                Headers.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

        protected override async Task SerializeToStreamAsync(Stream stream, System.Net.TransportContext context)
        {
            var buffer = new byte[_bufferSize];
            TryComputeLength(out long size);
            var uploaded = 0L;
            using (var inputStream = await _content.ReadAsStreamAsync())
            {
                var length = inputStream.Length;
                int read;
                while ((read = await inputStream.ReadAsync(buffer, 0, buffer.Length)) > 0)
                {
                    _cancellationToken.ThrowIfCancellationRequested();
                    await stream.WriteAsync(buffer, 0, read);
                    uploaded += read;
                    await Task.Delay(1);
                    await _progress((int)(((double)uploaded / length)*100) );
                }
            }
        }

        protected override bool TryComputeLength(out long length)
        {
            length = _content.Headers.ContentLength ?? -1;
            return length != -1;
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _content.Dispose();
            }
            base.Dispose(disposing);
        }
    }

}
