using System.ComponentModel;
using System.Reflection;
using System.Text;



namespace OtusHomework13
{
    /// <summary>
    /// Класс-сериализатор данных в формат CSV.
    /// </summary>
    public class CsvSerializer<T> where T : class, new()
    {
        private readonly char _delimiter;

        private readonly List<PropertyInfo> _properties;
        private readonly List<FieldInfo> _fields;


        public CsvSerializer(char delimiter = ',')
        {
            _delimiter = delimiter;

            var type = typeof(T);

            var properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance
                 | BindingFlags.GetProperty | BindingFlags.SetProperty);
            _properties = (from p in properties orderby p.Name select p).ToList();


            var fields = type.GetFields(BindingFlags.Public | BindingFlags.Instance);
            _fields = (from f in fields orderby f.Name select f).ToList();
        }


        #region Serialize

        private string GetNamesRow()
        {
            List<string> fieldNames = _fields.Select(f => f.Name).ToList();
            string fieldNamesRow = string.Join(_delimiter.ToString(), fieldNames);

            List<string> propertyNames = _properties.Select(p => p.Name).ToList();
            string propertyNamesRow = string.Join(_delimiter.ToString(), propertyNames);

            var namesRow = string.Join(_delimiter.ToString(), fieldNamesRow, propertyNamesRow);

            return namesRow;
        }



        public List<string> GetFieldValues(T obj)
        {
            var values = new List<string>();

            foreach (var f in _fields)
            {
                string value = f.GetValue(obj).ToString();
                if ((f.FieldType.Name == "String") || (value.Contains(_delimiter)))
                {
                    value = string.Format("\"{0}\"", value);
                }

                values.Add(value);
            }

            return values;
        }


        public List<string> GetPropertyValues(T obj)
        {
            var values = new List<string>();

            foreach (var p in _properties)
            {
                string value = p.GetValue(obj).ToString();

                if ((p.PropertyType.Name == "String") || (value.Contains(_delimiter)))
                {
                    value = string.Format("\"{0}\"", value);
                }

                values.Add(value);
            }

            return values;

        }



        public string Serialize(T obj)
        {
            var sb = new StringBuilder();
            var values = new List<string>();

            try
            {
                string namesRow = GetNamesRow();
                sb.AppendLine(namesRow);

                values.AddRange(GetFieldValues(obj));
                values.AddRange(GetPropertyValues(obj));

                sb.AppendLine(string.Join(_delimiter.ToString(), values.ToArray()));
            }
            catch
            {
                throw;
            }

            string csv = sb.ToString().TrimEnd();

            return csv;
        }



        public string Serialize(Stream stream, T obj)
        {
            var sb = Serialize(obj);

            try
            {
                using (var streamWriter = new StreamWriter(stream, Encoding.UTF8))
                {
                    streamWriter.Write(sb.ToString().Trim());
                }
            }
            catch
            {
                throw;
            }

            return sb.ToString();
        }

        #endregion


        #region Deserialize
        public T Deserialize(string csv)
        {
            string[] rows = csv.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            string stringNames = rows[0];
            string stringValues = rows[1];

            string[] names = stringNames.Split(_delimiter);
            string[] values = stringValues.SplitCsv(_delimiter).ToArray();

            var obj = new T();

            try
            {

                int cnt = 0;
                for (int i = 0; i < _fields.Count; i++)
                {
                    cnt++;
                    var value = values[i].Trim('\"');
                    var name = names[i];

                    var f = _fields.First(a => a.Name == name);
                    if (f != null)
                    {
                        var converter = TypeDescriptor.GetConverter(f.FieldType);
                        var convertedValue = converter.ConvertFrom(value);

                        f.SetValue(obj, convertedValue);
                    }

                }

                for (int i = cnt; i < _properties.Count + cnt; i++)
                {
                    var value = values[i];
                    var name = names[i];

                    value = value.ToString();

                    var p = _properties.First(a => a.Name == name);

                    var converter = TypeDescriptor.GetConverter(p.PropertyType);
                    var convertedValue = converter.ConvertFrom(value);

                    p.SetValue(obj, convertedValue);
                }
            }
            catch
            {
                throw;
            }

            return obj;
        }







        public T Deserialize(Stream stream)
        {
            var sb = new StringBuilder();

            var obj = new T();

            try
            {
                using (var streamReader = new StreamReader(stream))
                {
                    sb.Append(streamReader.ReadLine());
                    sb.Append(streamReader.ReadLine());
                }


                obj = Deserialize(sb.ToString());

            }
            catch
            {
                throw;
            }


            return obj;
        }

        #endregion
    }
}
