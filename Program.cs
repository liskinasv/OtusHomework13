// See https://aka.ms/new-console-template for more information

using Newtonsoft.Json;
using OtusHomework13;
using System.Diagnostics;


Console.WriteLine("Сериализуемый класс: class F { int i1, i2, i3, i4, i5; double d2; string cats; DateTime date; }\n");

F obj = F.Get();

const int iterations = 1000;



Console.WriteLine("мой рефлекшен:");

CsvSerializer<F> serializer = new CsvSerializer<F>(';');

string csv1 = Serialize<F>(iterations, serializer.Serialize, obj);
DeSerialize<F>(iterations, serializer.Deserialize, csv1);



Console.WriteLine();



Console.WriteLine("стандартный механизм (NewtonsoftJson):");

string csv = Serialize<F>(iterations, JsonConvert.SerializeObject, obj);
DeSerialize<object>(iterations, JsonConvert.DeserializeObject, csv);





Console.ReadKey();





string Serialize<T>(int iterations, Func<T, string> serialize, T obj)
{
    Stopwatch stopwatch = Stopwatch.StartNew();
    
    for (int i = 0; i < iterations - 1; i++)
    {
        serialize(obj);
    }

    string csv = serialize(obj);
    
    stopwatch.Stop();
    TimeSpan elapsedCsv = stopwatch.Elapsed;

    stopwatch.Restart();
    Console.WriteLine(csv);
    stopwatch.Stop();

    TimeSpan elapsedConsole = stopwatch.Elapsed;

    Console.WriteLine($"Количество итераций цикла: {iterations}. Время сериализации: {elapsedCsv.TotalMilliseconds} мс");
    Console.WriteLine($"Время вывода в консоль: {elapsedConsole.TotalMilliseconds} мс");

    return csv;
}



void DeSerialize<T>(int iterations, Func<string, T> deserialize, string csv)
{
    Stopwatch stopwatch = Stopwatch.StartNew();

    for (int i = 0; i < iterations - 1; i++)
    {
        deserialize(csv);
    }

    T obj = deserialize(csv);

    stopwatch.Stop();
    TimeSpan elapsed = stopwatch.Elapsed;

    Console.WriteLine(obj.ToString());
    Console.WriteLine($"Время десериализации: {elapsed.TotalMilliseconds} мс");
}






