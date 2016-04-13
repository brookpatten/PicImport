using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;

namespace PicDeduplicate
{
	class MainClass
	{
		public static void Main(string[] args)
		{
			var files = new List<PictureFile>();
			if (args.Any())
			{
				var path = args[0];
				var directory = new DirectoryInfo(path);
				files = GetFilePaths(directory);
			}

			Console.WriteLine(files.Count + " files found");

			var grouped = files.GroupBy(x => new { x.Name, x.Length });

			Console.WriteLine(grouped.Count() + " unique files");

			var groupsWithDuplicates = grouped.Where(x => x.Count() > 1);

			Console.WriteLine(groupsWithDuplicates.Sum(x=>x.Count()-1)+" to be removed");

			bool remove = args.Length > 1 && args[1] == "remove";

			foreach (var g in groupsWithDuplicates)
			{
				var duplicates = g.OrderBy(x=>x.Name.Length)
				                      .ThenBy(x=>x.Path.Length)
				                      .Skip(1)
				                      .ToList();
				foreach (var d in duplicates)
				{
					if (remove)
					{
						File.Delete(d.Path);
					}
					Console.WriteLine(d.Path);
				}
			}
		}

		private static List<PictureFile> GetFilePaths(DirectoryInfo input)
		{
			var paths = new List<PictureFile>();

			var files = input.GetFiles();
			foreach (var file in files)
			{
				paths.Add(new PictureFile()
				{
					Path = file.FullName,
					Name = file.Name.ToLower(),
					Length = file.Length
				});
			}

			foreach(var dir in input.GetDirectories())
			{
				paths.AddRange(GetFilePaths(dir));
			}
			return paths;
		}
	}

	class PictureFile
	{
		public string Name { get; set; }
		public string Path { get; set; }
		public long Length { get; set; }
		public string Hash { get; set; }
	}
}
