
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace JSONClassConverter
{
    public class IJSONConverter
    {

        private static string jsonPath = "NULL";

        public static void Main(string[] args)
        {
            if (args.Length != 0) jsonPath = args[0];
            else GetFileInput();

            string txt = File.ReadAllText(jsonPath);

            List<string> lines = new();

            try
            {
                JObject obj = JsonConvert.DeserializeObject<JObject>(txt);

                foreach (JProperty tok in obj.Properties())
                {
                    Console.WriteLine(tok);
                    switch (tok.Value.Type)
                    {
                        case JTokenType.String:
                            lines.Add($"public string {tok.Path} " + "{ get; set; }");
                            break;
                        case JTokenType.Integer:
                            lines.Add($"public int {tok.Path} " + "{ get; set; }");
                            break;
                        case JTokenType.Boolean:
                            lines.Add($"public bool {tok.Path} " + "{ get; set; }");
                            break;
                        case JTokenType.Date:
                            lines.Add($"public DateTime {tok.Path} " + "{ get; set; }");
                            break;
                        case JTokenType.Float:
                            lines.Add($"public float {tok.Path} " + "{ get; set; }");
                            break;
                        case JTokenType.Array:
                            string firstValueType = "object";
                            if ((tok.Value).Count() > 0 && tok.Value[0].Type != JTokenType.Object)
                            {
                                firstValueType = ((JValue)tok.Value[0]).Value.GetType().Name.ToLower();
                            }
                            lines.Add($"public {firstValueType}[] {tok.Path} " + "{ get; set; }" + $" /* THIS ARRAY TYPE MIGHT NOT BE CORRECT, CHECK WITH THE PROVIDED VALUES: {tok.Value} */");
                            break;
                        case JTokenType.Null:
                            lines.Add($"public object {tok.Path} " + "{ get; set; } " + $" /* THIS TYPE COULD NOT BE DETERMINED BECAUSE THIS VALUE WAS NULL */");
                            break;
                        default:
                            lines.Add($"public {tok.Value.Type.ToString().ToLower()} {tok.Path} " + "{ get; set; } " + $" /* THIS VALUE WAS NOT FOUND, FIX THIS BASED ON THE PROVIDED VALUES: {tok.Value} */");
                            break;

                    }
                }

                File.WriteAllLines("GeneratedJsonClass.cs", lines.ToArray());

                FileInfo genFile = new("GeneratedJsonClass.cs");

                if (!genFile.Exists)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"ERROR: Something went wrong with the creation of the 'GeneratedJsonClass.cs' file!");
                    Console.WriteLine("Press enter to exit...");
                    Console.ReadLine();
                    return;
                }

                Console.WriteLine($"GeneratedJsonClass.cs file created at {genFile.FullName}!");
                Console.WriteLine("Done!");
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"ERROR: {ex.Message}");
                Console.WriteLine("Press enter to exit...");
                Console.ReadLine();
            }



        }

        public static void GetFileInput()
        {
            Console.WriteLine("Json file path: ");
            jsonPath = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(jsonPath) || !File.Exists(jsonPath))
            {
                GetFileInput();
            }
        }
    }
}
