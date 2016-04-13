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

			bool move = args.Length > 2 && args[1] == "move" && !string.IsNullOrEmpty(args[2]);

			string moveDir = move ? args[2] : null;

			foreach (var g in groupsWithDuplicates)
			{
				var duplicates = g.OrderBy(x=>x.Name.Length)
				                      .ThenBy(x=>x.Path.Length)
				                      .Skip(1)
				                      .ToList();
				foreach (var d in duplicates)
				{
					if (move)
					{
						if (!Directory.Exists(moveDir))
						{
							Directory.CreateDirectory(moveDir);
						}

						string destination = Path.Combine(moveDir, d.Name);
						int suffix = 1;
						while (File.Exists(destination))
						{
							destination = Path.Combine(moveDir, string.Format("{0}_{1:00}{2}",Path.GetFileNameWithoutExtension(d.Name),suffix,Path.GetExtension(d.Name)));
							suffix++;
						}
						File.Move(d.Path, destination);
					}
					else if (remove)
					{
						File.Delete(d.Path);
					}

					//also remove any folders that we left empty
					if ((move || remove) && Directory.GetFiles(Path.GetDirectoryName(d.Path)).Count() == 0)
					{
						Directory.Delete(Path.GetDirectoryName(d.Path));
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
	}
}
