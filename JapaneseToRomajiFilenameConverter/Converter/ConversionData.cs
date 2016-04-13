using System;

namespace JapaneseToRomajiFileConverter.Converter {

    [Serializable]
    public class ConversionData {

        public string FilePath { get; private set; }
        public string Title { get; private set; }
        public string Album { get; private set; }
        public string[] Performers { get; private set; }
        public string[] AlbumArtists { get; private set; }

        public ConversionData(string filePath,
                              string title,
                              string album,
                              string[] performers,
                              string[] albumArtists) {
            FilePath = filePath;
            Title = title;
            Album = album;
            Performers = performers;
            AlbumArtists = albumArtists;
        }

        protected bool Equals(ConversionData other)
        {
            return string.Equals(FilePath, other.FilePath) &&
                   string.Equals(Title, other.Title) &&
                   string.Equals(Album, other.Album) &&
                   Equals(Performers, other.Performers) &&
                   Equals(AlbumArtists, other.AlbumArtists);
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((ConversionData) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                var hashCode = (FilePath != null ? FilePath.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Title != null ? Title.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Album != null ? Album.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (Performers != null ? Performers.GetHashCode() : 0);
                hashCode = (hashCode*397) ^ (AlbumArtists != null ? AlbumArtists.GetHashCode() : 0);
                return hashCode;
            }
        }
    }
}
