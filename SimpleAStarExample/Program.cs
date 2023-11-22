using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Threading.Tasks;

namespace SimpleAStarExample
{
	class Program
	{
		private bool[,] map;
		private SearchParameters searchParameters;

		string[,] values = {
									//{ "0x05", "100.1",     "100.005",             "1",    "200" },
									//{ "-777", "-4555",     "-0.001",              "300",  "-456" },
									//{ "200",  "-34.01112", "1844674407370955161", "-17",  "44" },
									//{ "7",    "222",       "-0.011",              "-10",  "0x22" },
									//{ "1",    "-5",        "-1",                  "5554", "345" }

									{ "345", "0x22",  "44",  "-456",  "200" },
									{ "5554", "-10",  "-17",   "300",  "1" },
									{ "-1",  "-0.011", "1844674407370955161",  "-0.001",  "100.005" },
									{ "-5",  "222",   "-34.01112",   "-4555",  "100.1" },
									{ "1",  "7",  "200",  "-777", "0x05" }
		};

		static void Main(string[] args)
		{
			var program = new Program();
			program.Run();
		}

		public void Run()
		{
			InitializeMap(values);
			PathFinder pathFinder = new PathFinder(searchParameters);
			List<Point> path = pathFinder.FindPath();
			Console.WriteLine("Исходная таблица");
			Console.WriteLine();
			ShowRoute(path);

			//транспонирую таблицу
			string[,] transposed = new string[5, 5];
			for (int i = 0; i < 5; i++)
			{
				for (int j = 0; j < 5; j++)
				{
					transposed[i, j] = values[j, i];
				}
			}

			values = (string[,])transposed.Clone();
			InitializeMap(values);
			pathFinder = new PathFinder(searchParameters);
			path = pathFinder.FindPath();
			Console.WriteLine("Транспонированная таблица");
			Console.WriteLine();
			ShowRoute(path);


			Console.WriteLine("Press any key to exit...");
			Console.ReadKey();
		}

		/// Displays the map and path as a simple grid to the console
		private void ShowRoute(IEnumerable<Point> path)
		{
			List<string> values_coordinates = new List<string>();
			for (int y = 4; y >= 0; y--) // Invert the Y-axis so that coordinate 0,0 is shown in the bottom-left
			{
				for (int x = 4; x >= 0; x--)
				{
					if (this.searchParameters.StartLocation.Equals(new Point(x, y)))
						// Show the start position
						Console.Write(" " + "1".PadLeft(22) + " ");
					else if (this.searchParameters.EndLocation.Equals(new Point(x, y)))
						// Show the end position
						Console.Write(" " + "1".PadLeft(22) + " ");
					else if (this.map[x, y] == false)
						// Show any barriers
						Console.Write(" " + values[x, y].PadLeft(22) + " ");
					else if (path.Where(p => p.X == x && p.Y == y).Any())
					{
						// Show the path in between
						Console.Write(" " + values[x, y].PadLeft(22) + "*");
						values_coordinates.Add(values[x, y].PadLeft(8) + "   X = " + (5-x).ToString() + "; Y = " + (y+1).ToString());
					}

					else
						// Show nodes that aren't part of the path
						Console.Write(" " + values[x, y].PadLeft(22) + " ");
				}

				Console.WriteLine();
				Console.WriteLine();
			}

			foreach (var item in values_coordinates)
			{
				Console.WriteLine(item);
			}
			Console.WriteLine();
			Console.WriteLine();
		}

		private void InitializeMap(string[,] _map)
		{
			bool start_point = true;
			Point startLocation = new Point();
			Point endLocation = new Point();

			map = new bool[5, 5];

			for (int y = 0; y < 5; y++)
			{
				for (int x = 0; x < 5; x++)
				{
					if (_map[x, y].StartsWith("0x"))
					{
						map[x, y] = false;
						continue;
					}
					decimal d;
					if (decimal.TryParse(_map[x, y], NumberStyles.AllowDecimalPoint | NumberStyles.Number, new NumberFormatInfo { NumberDecimalSeparator = "." }, out d))
					{
						if (Decimal.Compare(d, (decimal)-0.01) < 0)
						{
							map[x, y] = true;
						}
						else
						{
							map[x, y] = false;
						}

						if (d == 1 && start_point)
						{
							startLocation = new Point(x, y);
							start_point = false;
						};
						if (d == 1 && !start_point)
						{
							endLocation = new Point(x, y);
						}
						if (d == 1) map[x, y] = true;

					}
					else
					{
						Console.WriteLine("Проверьте формат числа - " + _map[x, y]);
					}
				}
			}
			this.searchParameters = new SearchParameters(startLocation, endLocation, map);
		}
	}
}
