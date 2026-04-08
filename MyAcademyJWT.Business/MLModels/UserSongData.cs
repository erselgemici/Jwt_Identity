using Microsoft.ML.Data;

namespace MyAcademyJWT.Business.MLModels
{
    public class UserSongData
    {
        [LoadColumn(0)]
        public float UserId { get; set; }

        [LoadColumn(1)]
        public float SongId { get; set; }

        // Şarkının kaç kez dinlendiği (Çok dinlenen = Çok sevilen)
        [LoadColumn(2)]
        public float Label { get; set; }
    }
}
