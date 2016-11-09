using System.IO;

namespace GameWork.Core.IO
{
	public class CSVReader
	{
		private readonly StreamReader _reader;

		public CSVReader(Stream stream)
		{
			_reader = new StreamReader(stream);
		}  	

		public string[] ReadRow()
		{	
			var line = _reader.ReadLine();
			var cells = CSV.ParseRow(line);
			return cells;
		}

		public int Peek()
		{
			return _reader.Peek();
		}
	}
}