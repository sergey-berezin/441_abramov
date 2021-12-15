using System;
using Microsoft.EntityFrameworkCore;
using Entities;

namespace DataStorage
{

    public class ImagesLibraryContext : DbContext
    { 
        public DbSet<ImageInfo> ImagesInfo { get; set; }

        public DbSet<ImageInfoDetails> ImagesInfoDetails { get; set; }

        public DbSet<RecognizedObject> RecognizedObjects { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder o)
        {
            var dir = System.IO.Directory.GetCurrentDirectory();
            var db_path = dir.Substring(0, dir.LastIndexOf("RecognitionServer")) + @"\DataStorage\images_library.db";
            o/*.UseLazyLoadingProxies()*/.UseSqlite($"Data Source={db_path}");            
        }
    }
}
