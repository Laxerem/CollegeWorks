var inputPath = "data.txt";
var outputPath = "result.txt";

var inputString = File.ReadAllText(inputPath);
var valueArray = inputString.Split(' ').Select(int.Parse).ToArray();

File.WriteAllText(outputPath, "");

for (int i = 0; i < valueArray.Length; i++) {
    if (valueArray[i] > 10) {
        File.AppendAllText(outputPath, valueArray[i] + " ");
    }
}

Console.WriteLine("Программа выполнена");