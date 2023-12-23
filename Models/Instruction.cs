using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace MvcCosmos.Models
{
    public class Instruction
    {
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; }

        [JsonProperty(PropertyName = "step")]
        public int Step { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; }

        [JsonProperty(PropertyName = "value")]
        public string Value { get; set; }

        public static List<string> ReadDataFromText(Uri fileUri)
        {
            List<string> lines = new List<string>();
            using (HttpClient client = new HttpClient())
            using (StreamReader contents = new StreamReader(client.GetStreamAsync(fileUri).Result))
            {
                while (!contents.EndOfStream)
                {
                    lines.Add(contents.ReadLine());
                }
            };
            return lines;
        }

        public static List<Instruction> CreateInstructionsListFromText(List<string> linesOfText)
        {
            if (linesOfText == null || linesOfText.Count == 0)
            {
                throw new ArgumentNullException(nameof(linesOfText));
            }

            var instructions = new List<Instruction>();
            int step = 1;

            foreach (var line in linesOfText)
            {
                Instruction instruction = new Instruction()
                {
                    Id = null,
                    Step = step,
                    Type = line.Split(null)[0],
                    Value = line.Split(null)[1]
                };
                instructions.Add(instruction);
                step++;
            }

            return instructions;
        }
    }
}
