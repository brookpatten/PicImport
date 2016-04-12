using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PicImport
{
	public class MainClass
	{
		public static void Main(string[] args)
		{
			if (args.Length != 2)
			{
				Console.WriteLine("Usage: PicImport.exe <input path> <output path>");
			}
			else
			{
				DirectoryInfo input = new DirectoryInfo(args[0]);
				string output = args[1];
				var paths = GetFilePaths(input);
				Console.WriteLine("Found " + paths.Count + " total files");
				var createDates = GetCreateDates(paths);
				var outputPaths = GetOutputPaths(createDates, output);
				Console.WriteLine("Found " + outputPaths.Count + " files to import");
				CopyFiles(outputPaths);
				Console.WriteLine("Done");
			}
		}

		private static List<string> GetFilePaths(DirectoryInfo input)
		{
			var paths = new List<string>();

			var files = input.GetFiles();
			paths.AddRange(files.Select(x => x.FullName));

			foreach(var dir in input.GetDirectories())
			{
				paths.AddRange(GetFilePaths(dir));
			}
			return paths;
		}

		private static List<Tuple<string, DateTime>> GetCreateDates(List<string> inputPaths)
		{
			var result = new List<Tuple<string, DateTime>>();
			foreach (var input in inputPaths)
			{
				var fi = new FileInfo(input);
				result.Add(new Tuple<string, DateTime>(input, fi.CreationTimeUtc));
			}
			return result;
		}

		private static List<Tuple<string, string>> GetOutputPaths(List<Tuple<string, DateTime>> inputs, string outputPath)
		{
			var result = new List<Tuple<string, string>>();
			foreach (var input in inputs)
			{
				var destination = GetOutputpath(input.Item1, input.Item2, outputPath);
				if (!string.IsNullOrEmpty(destination))
				{
					result.Add(new Tuple<string, string>(input.Item1, destination));
				}
			}
			return result;
		}

		private static void CopyFiles(List<Tuple<string, string>> items)
		{
			Parallel.ForEach(items, item =>
			 {
				 try
				 {
					 if (!Directory.Exists(Path.GetDirectoryName(item.Item2)))
					 {
						 Directory.CreateDirectory(Path.GetDirectoryName(item.Item2));
					 }
					 File.Copy(item.Item1, item.Item2);
					 Console.WriteLine("Copied "+item.Item1 + " to " + item.Item2);
					 Console.ReadLine();
				 }
				 catch (Exception ex)
				 {
					 System.Console.WriteLine("Failed to copy " + item.Item1 + " to " + item.Item2);
				 }
			 });
		}

		private static string GetOutputpath(string inputPath, DateTime createdAt, string output)
		{
			var fileName = Path.GetFileName(inputPath);
			var destination = Path.Combine(output, string.Format("{0:0000}/{1:00}/{2:00}/{3}", createdAt.Year, createdAt.Month, createdAt.Day, fileName));

			//if the file already exists, see if it's the same
			if (File.Exists(destination))
			{
				var input = new FileInfo(inputPath);
				var dest = new FileInfo(destination);

				if (input.Length == dest.Length)
				{
					//we already have it, don't need to do anything
					//yes, just beause it's the same length doesn't mean it's the same file
					//but I like to live dangerously
					return null;
				}
				else
				{
					int suffix = 1;
					destination = Path.Combine(output, string.Format("{0:0000}/{1:00}/{2:00}/{3}_{4}{5}", createdAt.Year, createdAt.Month, createdAt.Day, Path.GetFileNameWithoutExtension(fileName), suffix, Path.GetExtension(fileName)));
					while (File.Exists(destination))
					{
						suffix++;
					}
					return destination;
				}
			}
			else 
			{
				return destination;
			}
		}
	}
}
