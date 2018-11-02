using System.ComponentModel.DataAnnotations;

namespace quizer_backend.Data.Entities {
    public class UserSettings {
        [Required] public string UserId { get; set; }
        [Required, Range(0, 10)] public uint ReoccurrencesIfBad { get; set; } = 1;
        [Required, Range(1, 10)] public uint ReoccurrencesOnStart { get; set; } = 2;
        [Required, Range(0, 10)] public uint MaxReoccurrences { get; set; } = 10;
    }
}
