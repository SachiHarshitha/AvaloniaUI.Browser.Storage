using System.Text.Json;

namespace AvaloniaUI.Browser.Storage.Models
{
    public class FileWithMetadata
    {
        public string Key { get; set; } = default!;
        public string FileName { get; set; } = default!;
        public string MimeType { get; set; } = default!;
        public double Size { get; set; }
        public string CreatedAt { get; set; } = default!;
        public string LastModifiedAt { get; set; } = default!;
        public string FileFormat { get; set; } = default!;
        public byte[] Data { get; set; } = default!;

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }
    }
}