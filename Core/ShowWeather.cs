using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

namespace Core
{
	public class ShowWeather
	{
		/// <summary>
		/// айди городе и его название
		/// </summary>
		Dictionary<int, string> all_city;
		/// <summary>
		/// имя города+время дня и его погода
		/// </summary>
		Dictionary<string, string> weather_analyze;
		/// <summary>
		/// максимальная и минимальная температура городов
		/// </summary>
		List<int> temp_max = new List<int>();
		List<int> temp_min = new List<int>();
		/// <summary>
		/// разобранный при помощи regex, дата анализа погоды
		/// </summary>
		List<string> day_analyze;
		//нахожу основной корень выражения -2..3 С
		Regex temp_total;
		//вытягиваю из результата выше нужные числа
		Regex regex_temperature_min;
		/// <summary>
		/// regext for take a days
		/// </summary>
		Regex regex_time_days;
		static Random random;

		public void ShowMenu()
		{
			int choice = 0;

			do
			{

				switch (choice)
				{
					case 1: DrowWeather(); break;
					case 2: ShowCurrentCity(); break;
					case 3: WarmestWeather(); break;
					case 4: Console.Clear(); AddCity(); break;
					case 5: Console.Clear(); RemoveCity(); break;
					case 6: Console.Clear(); DummpToXML(); break;
				}
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("===========================GISMETEO WEATHER ANALYZER=========================");
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("1. Show weather in cities");
				Console.WriteLine("2. Show weather in special city");
				Console.WriteLine("3. Show Warmest weather");
				Console.WriteLine("4. Add city");
				Console.WriteLine("5. Remove city");
				Console.WriteLine("6. Dump report to xml");
				Console.WriteLine("7. Exit");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("=============================================================================");
				Console.ResetColor();
				Console.Write("Choice: ");
				int.TryParse(Console.ReadLine(), out choice);

			} while (choice != 7);

		}

		void AddCity()
		{
			//Название города можем указывать какое нам угодно, основная проверка по коду города
			Console.WriteLine("Enter name of citi: ");
			string name = Console.ReadLine();
			Console.WriteLine("Enter cities code: ");
			int code = 0;
			if (int.TryParse(Console.ReadLine(), out code))
			{
				if (all_city.ContainsKey(code))
				{
					Console.WriteLine("This city already exist");
					AddCity();
				}
				try
				{
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load("http://informer.gismeteo.by/rss/" + code + ".xml");
					if (xmlDocument != null)
					{

						XmlNodeList node = xmlDocument.GetElementsByTagName("item");
						XmlNode tempnode = node[node.Count - 1];//вытягиваю последний узел где есть название города
						string text = tempnode["title"].InnerText;//вытянул текст с городом из сабноды сорса
						all_city.Add(code, name);
						Console.WriteLine("All done");

					}
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error: " + ex.Message);
					AddCity();
				}
			}
			else
			{
				Console.WriteLine("Wrong code");
				AddCity();
			}
		}


		void RemoveCity()
		{
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("====================LABEL REMOVE=============================");
			foreach (var item in all_city)
			{
				Console.ForegroundColor = (ConsoleColor)random.Next(0, 16);
				Console.WriteLine("City name: " + item.Value + "; city code: " + item.Key);
			}
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("====================LABEL REMOVE=============================");
			Console.ForegroundColor = (ConsoleColor)random.Next(0, 16);
			Console.WriteLine("Enter cities code(x-return): ");
			int code = 0;
			string s = Console.ReadLine();
			if (int.TryParse(s, out code))
			{
				if (!all_city.ContainsKey(code))
				{
					Console.WriteLine("This city not exist");
					RemoveCity();
				}
				try
				{
					all_city.Remove(code);
				}
				catch (Exception ex)
				{
					Console.WriteLine("Error: " + ex.Message);
					RemoveCity();
				}
			}
			else
			{
				if (s == "x") return;
				Console.WriteLine("Wrong code");
				RemoveCity();
			}
			Console.WriteLine("All done");
		}


		void DrowWeather()
		{
			Console.Clear();
			//очищаю старую информацию и  загружаю актуальную
			if (weather_analyze.Count > 1) weather_analyze.Clear();
			LoadWhether();
			foreach (var s in weather_analyze)
			{
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("=================" + s.Key + "===============");
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(s.Value);
				Console.ForegroundColor = ConsoleColor.Yellow;
				Console.WriteLine("=========================================================");
				Console.ResetColor();
				Console.WriteLine("\n\n\n");
			}

		}


		/// <summary>
		/// метод только для загрузки данных
		/// </summary>
		void LoadWhether()
		{
			XmlDocument xmlDocument = new XmlDocument();
			foreach (var item in all_city)
			{
				try
				{
					xmlDocument.Load("http://informer.gismeteo.by/rss/" + item.Key + ".xml");
					foreach (XmlNode weth in xmlDocument.GetElementsByTagName("item"))
					{
						weather_analyze.Add(weth["title"].InnerText, weth["description"].InnerText);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
			}
		}

		void ShowCurrentCity()
		{
			Console.Clear();
			int index;
			int count = 1;
			int st_index = 0;
			foreach (var item in all_city)
			{
				Console.ForegroundColor = (ConsoleColor)random.Next(0, 16);
				Console.WriteLine(count + ". " + item.Value);
				count++;
			}
			Console.ForegroundColor = (ConsoleColor)random.Next(0, 16);
			Console.Write("Choice city(index): ");
			if (int.TryParse(Console.ReadLine(), out index))
			{
				Console.Clear();
				foreach (var item in all_city)
				{
					if (st_index + 1 == index)
					{
						try
						{
							//загружаю всегда новую актуальную информацию
							XmlDocument xmlDocument = new XmlDocument();
							xmlDocument.Load("http://informer.gismeteo.by/rss/" + item.Key + ".xml");
							XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("item");
							foreach (XmlNode item1 in xmlNodeList)
							{
								Console.ForegroundColor = ConsoleColor.Yellow;
								Console.WriteLine("=================" + item1["title"].InnerText + "===============");
								Console.ForegroundColor = ConsoleColor.Green;
								Console.WriteLine(item1["description"].InnerText);
								Console.ForegroundColor = ConsoleColor.Yellow;
								Console.WriteLine("=====================================");
								Console.ResetColor();
								Console.WriteLine("\n\n\n");
							}
						}
						catch (Exception ex)
						{
							Console.WriteLine("Error: " + ex.Message);
							ShowCurrentCity();
						}
						break;
					}
					st_index++;
				}
			}
			else Console.WriteLine("Check input");

		}

		/// <summary>
		/// метод находит самую теплую погоду на данный момент
		/// </summary>
		void WarmestWeather()
		{

			Console.Clear();
			int max_s = 0;
			int min_s = 0;
			Console.ForegroundColor = ConsoleColor.Cyan;
			Console.WriteLine("=====================ПОКАЗАТЕЛИ ТЕМПЕРАТУРЫ НА ДАННЫЙ МОМЕНТ=============================");			
			try
			{
				foreach (var item in all_city)
				{	
					//загружаю нову информацию, и вытягиваю последнюю ноду item, в нем хранится текущая температура, на основании нее и нахожу самое теплое время на данный момент
					XmlDocument xmlDocument = new XmlDocument();
					xmlDocument.Load("http://informer.gismeteo.by/rss/" + item.Key + ".xml");
					//получаю узел item
					XmlNodeList xmlNodeList = xmlDocument.GetElementsByTagName("item");
					//вытягиваю из узла последний саб узел, который показывает актуальную на данный момент темп
					XmlNode xmlNode = xmlNodeList[xmlNodeList.Count - 1];
					//вытащили корень показателей температуры, 5..8С
					MatchCollection matchCollection = temp_total.Matches(xmlNode["description"].InnerText);
					//расспарсил цифры из выражения выше, по отдельности
					MatchCollection matchCollection1 = regex_temperature_min.Matches(matchCollection[0].Value);
					//сохраняю данные в переменные
					max_s = int.Parse(matchCollection1[1].Value);
					min_s = int.Parse(matchCollection1[0].Value);
					temp_max.Add(max_s);
					temp_min.Add(min_s);
					//записываю время снятие показаний
					MatchCollection match = regex_time_days.Matches(xmlNode["title"].InnerText);
					string b = "";
					for (int i = 0; i < match.Count; i++)
					{
						b += match[i].Value;

					}
					b = b.Trim();
					day_analyze.Add(b);
					Console.WriteLine("Город: " + item.Value + "; Текущая низкая температура: " + temp_min.Last() + "; Текущая высокая температура: " + temp_max.Last());				
				}
				Console.ForegroundColor = ConsoleColor.Cyan;
				Console.WriteLine("=========================================================================================");
				int max = 0;
				int min = temp_min[0];
				int index_min = 0;
				int index_max = 0;
				for (int i = 0; i < temp_max.Count; i++)
				{
					if (temp_max[i] > max)
					{
						max = temp_max[i];
						index_max = i;
					}
					//эта проверка для тех случаев, когда у нас максимальная темпа может быть тоже отрицательной, и самый холодный день будет там где отрицательное число выше
					if (temp_max[i] < min)
					{
						min = temp_max[i];
						index_min = i;
					}

					if (temp_min[i] < min)
					{
						min = temp_min[i];
						index_min = i;
					}
				}
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine("Самая теплая погода в городе: " + all_city.ElementAt(index_max).Value + " и температура в этом городе: " + max + " C");
				Console.WriteLine("Самая холодная погода в городе: " + all_city.ElementAt(index_min).Value + " и температура в этом городе: " + min + " C");
				Console.ResetColor();
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
		}




		void Save()
		{
			//только создание, нужно для того , чтобы всегда перезаписывать файл с 0, если к примеру мы удаляем города.....
			FileStream fileStream = new FileStream("City.txt", FileMode.Create, FileAccess.Write);
			BinaryWriter binaryWriter = new BinaryWriter(fileStream, UnicodeEncoding.Unicode);
			foreach (var item in all_city)
			{
				binaryWriter.Write(item.Key);
				binaryWriter.Write(item.Value);
			}
			binaryWriter.Close();
		}


		void DummpToXML()
		{

			if (day_analyze.Count > 0)
			{
				XmlWriter xml = null;
				DateTime dateTime = new DateTime();
				try
				{

					dateTime = DateTime.Now;
					FileStream fileStream = new FileStream("Stat-" + dateTime.ToShortDateString() + ".xml", FileMode.Create, FileAccess.Write);
					XmlWriterSettings settings = new XmlWriterSettings();
					settings.Indent = true;
					settings.Encoding = Encoding.UTF8;
					settings.CloseOutput = true;
					xml = XmlWriter.Create(fileStream, settings);
					xml.WriteStartDocument();
					xml.WriteStartElement("Weathers");
					for (int i = 0; i < all_city.Count; i++)
					{
						xml.WriteStartElement("weather");
						xml.WriteAttributeString("city", all_city.ElementAt(i).Value);
						xml.WriteAttributeString("id", all_city.ElementAt(i).Key.ToString());
						xml.WriteElementString("day", day_analyze[i]);
						xml.WriteElementString("temp_min", temp_min[i].ToString());
						xml.WriteElementString("temp_max", temp_max[i].ToString());
						xml.WriteEndElement();
					}
					xml.WriteEndElement();
					xml.WriteEndDocument();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				finally
				{
					Console.WriteLine("all done");
					xml.Close();
					if (File.Exists("Stat-" + dateTime.ToShortDateString() + ".xml")) Process.Start("Stat-" + dateTime.ToShortDateString() + ".xml");
				}
			}
			else
			{
				Console.WriteLine("no info to save, collect menu 3");
			}
		}



		void Load()
		{
			all_city = new Dictionary<int, string>();
			if (File.Exists("City.txt"))
			{
				BinaryReader binaryReader = null;
				try
				{
					FileStream fileStream = new FileStream("City.txt", FileMode.Open, FileAccess.Read);
					binaryReader = new BinaryReader(fileStream, UTF8Encoding.Unicode);
					while (binaryReader.PeekChar() != -1) all_city.Add(binaryReader.ReadInt32(), binaryReader.ReadString());
					binaryReader.Close();
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.Message);
				}
				finally
				{
					binaryReader.Close();
				}
			}
			else
			{
				//если городов нет, делаю дефолтные
				all_city.Add(33345, "Киев");
				all_city.Add(35188, "Астана");
				all_city.Add(38880, "Ашхабад");
				all_city.Add(27612, "Москва");
				all_city.Add(26422, "Рига");
			}
		}

		/// <summary>
		/// класс полностью закрытый, свойств не делаю
		/// </summary>
		public ShowWeather()
		{
			//выбираю корень выражения, и в посл. рег. выр. распаршиваю еще раз
			temp_total = new Regex(@"\d+..\d+\sС|-\d+..-\d+\sС|-\d+..\d+\sС|\d+..-\d+\sС", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
			regex_time_days = new Regex(@"\s\w*", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
			regex_temperature_min = new Regex(@"-\d+|\d+", RegexOptions.IgnoreCase | RegexOptions.Compiled | RegexOptions.IgnorePatternWhitespace);
			List<int> temp_max = new List<int>();
			List<int> temp_min = new List<int>();
			Load();
			weather_analyze = new Dictionary<string, string>();
			random = new Random();
			day_analyze = new List<string>();
		}
		~ShowWeather()
		{
			Save();
		}


	}
}
