using ClassIntoEnumWithDescriptionsConverter;
using System.Reflection;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using ClassIntoEnumWithDescriptionsConverter.Enums;

Console.WriteLine("Converting string class into new Enum class with descriptions attribute");
Console.WriteLine("Please insert name file with expected extension");
var fileName = Console.ReadLine();

Console.WriteLine("Select conversion type: 1.Fields name, 2. Fields value");

var conversionTypeInput = Console.ReadLine();
DescriptionConversionType conversionType;

switch (conversionTypeInput)
{
    case "1":
        conversionType = DescriptionConversionType.FieldName;
        break;

    case "2":
        conversionType = DescriptionConversionType.FieldValue;
        break;

    default:
        Console.WriteLine("Conversion type not specified. Aborting.");
        return;
}


var stringClassAttributes = new DeviceIcons().GetType().GetFields();

Console.WriteLine("Loading class as textfile");

var enumFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "AllDeviceIcons.cs");

if (File.Exists(enumFilePath))
{
    var fileContent = File.ReadAllText(enumFilePath);
    var lines = File.ReadLines(enumFilePath);

    var stringReader = new StringReader(fileContent);
    var stringBuilder = new StringBuilder();

    bool isMatch = false;
    int replacedAttributes = 0;

    foreach (var attribute in stringClassAttributes)
    {
        isMatch = false;
        stringReader = new StringReader(fileContent);

        while (!isMatch)
        {
            var currentLine = stringReader.ReadLine();

            if (currentLine != null)
            {
                if (Regex.IsMatch(currentLine, $@"\b{attribute.Name}\b"))
                {
                    string newLine = string.Empty;

                    if(conversionType == DescriptionConversionType.FieldName)
                     newLine = currentLine.Replace(attribute.Name, $"[Description(\"{attribute.Name}\")] {Environment.NewLine} {attribute.Name}");
                    else if (conversionType == DescriptionConversionType.FieldValue)
                        newLine = currentLine.Replace(attribute.Name, $"[Description(\"{attribute.GetValue(attribute)}\")] {Environment.NewLine} {attribute.Name}");

                    stringBuilder.Append(newLine).Append(Environment.NewLine);
                    isMatch = true;

                    Console.WriteLine(replacedAttributes++);
                }
            }
            else if (stringReader.Read() == -1)
            {
                isMatch = true;
                if (currentLine != null)
                {
                    currentLine = string.Empty;
                    stringBuilder.Append(currentLine);
                }
            }
        }
    }

    File.WriteAllText(fileName, stringBuilder.ToString());

    Console.WriteLine("Done. Enjoy");
}
else
{
    Console.WriteLine("There's no such a file.");
}