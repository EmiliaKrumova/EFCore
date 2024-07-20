namespace MusicHub
{
    using System;
    using System.Diagnostics;
    using System.Text;
    using Data;
    using Initializer;
    using MusicHub.Data.Models;

    public class StartUp
    {
        public static void Main()
        {
            MusicHubDbContext context =
                new MusicHubDbContext();

            // DbInitializer.ResetDatabase(context);
            //  Console.WriteLine(ExportAlbumsInfo(context, 4));

            Console.WriteLine(ExportSongsAboveDuration(context,4));

            //ExportAlbumsInfo(context, 9);


            //Test your solutions here
        }


        public static string ExportAlbumsInfo(MusicHubDbContext context, int producerId)
        {
            var albums = context.Albums
                 .Where(a => a.ProducerId == producerId)
                 .Select(a => new
                 {
                     a.Name,
                     ReleaseDate = a.ReleaseDate.ToString("MM/dd/yyyy"),
                     ProducerName = a.Producer.Name,
                     AlbumTotalPrice = a.Songs.Sum(s => s.Price),
                     Songs = a.Songs.Select(s => new
                     {
                        SongName =  s.Name,
                        SongPrice = s.Price,
                        SongWritterName = s.Writer.Name,

                     }).OrderByDescending(s => s.SongName)
                     .ThenBy(s=>s.SongWritterName)
                     .ToList()
                 }).OrderByDescending(a=>a.AlbumTotalPrice)
                 .ToList();

            StringBuilder sb = new StringBuilder();

            foreach (var album in albums)
            {
                sb.AppendLine($"-AlbumName: {album.Name}");
                sb.AppendLine($"-ReleaseDate: {album.ReleaseDate}");
                sb.AppendLine($"-ProducerName: {album.ProducerName}");
                sb.AppendLine($"-Songs:");

                int counter = 1;
                foreach (var song in album.Songs)
                {
                    ;
                    sb.AppendLine($"---#{counter}");
                    counter++;
                    sb.AppendLine($"---SongName: {song.SongName}");
                    sb.AppendLine($"---Price: {song.SongPrice:F2}");
                    sb.AppendLine($"---Writer: {song.SongWritterName}");
                }
                sb.AppendLine($"-AlbumPrice: {album.AlbumTotalPrice:F2}");
           

            }
            return sb.ToString().TrimEnd();
        }
        //For each Album, get the Name, ReleaseDate in format the "MM/dd/yyyy", ProducerName, the Album Songs with each Song Name, Price (formatted to the second digit) and the Song WriterName. Sort the Songs by Song Name (descending) and by Writer (ascending). At the end export the Total Album Price with exactly two digits after the decimal place. Sort the Albums by their Total Price (descending).

        public static string ExportSongsAboveDuration(MusicHubDbContext context, int duration)
        {

            TimeSpan durationNeeded = TimeSpan.FromSeconds(duration);
            var songs = context.Songs.
                Where(s=>s.Duration > durationNeeded)
                .Select(s=> new
                {
                   SongName = s.Name,
                   SongPerformers =  s.SongPerformers
                        .Select(sp=>new
                        {
                         PerformerFullName =  sp.Performer.FirstName+" " +sp.Performer.LastName
                         })
                        .OrderBy(sp => sp.PerformerFullName)
                        .ToList(),
                    WriterName = s.Writer.Name,
                    AlbumProcucer = s.Album.Producer.Name,
                    SongDuration = s.Duration
                })
                .OrderBy(s=>s.SongName)
                .ThenBy(s=>s.WriterName)
                .ToList();
            int counter = 0;
            StringBuilder sb = new StringBuilder();
            foreach(var song in songs)
            {
                counter++;
                sb.AppendLine($"-Song #{counter}");
                sb.AppendLine($"---SongName: {song.SongName}");
                sb.AppendLine($"---Writer: {song.WriterName}");

                if(song.SongPerformers.Any())
                {
                    foreach(var p in song.SongPerformers)
                    {
                        sb.AppendLine($"---Performer: {p.PerformerFullName}");
                    }
                }
                sb.AppendLine($"---AlbumProducer: {song.AlbumProcucer}");
                sb.AppendLine($"---Duration: {song.SongDuration:c}");               


            }
            return sb.ToString().TrimEnd();
        }
        //For each Song, export its Name, Performer Full Name, Writer Name, Album Producer and Duration (in format("c")). Sort the Songs by their Name (ascending), and then by Writer (ascending).
        //If a Song has more than one Performer, export all performers and sort them (ascending, alphabetically).
        //If there are no Performers for a given song, don't print the "---Performer" line at all.

    }
}
