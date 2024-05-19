using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
 
class Program
{
    static void Main(string[] args)
    {
        JObject Arguments = new JObject();
 
        for (int i = 0; i < args.Length; i++)
        {
            if (args[i].StartsWith("-"))
            {
                string value = "";
                for (int j = i + 1; j < args.Length; j++)
                {
                    if (args[j].StartsWith("-")) { break; }
                    value += args[j];
                }
                Arguments[args[i].Substring(1, args[i].Length - 1).ToLower()] = value;
            }
        }
 
        foreach (var Argument in Arguments)
        {
            Console.WriteLine(Argument.Key + " : " + Argument.Value);
        }
 
        string input = (string)Arguments["input"];
        string output = (string)Arguments["output"];
        char separator = ((string)Arguments["separator"]).ToCharArray()[0];
 
        // Lecture du fichier CSV
        string[] csvLines = File.ReadAllLines(input);
 
        // Récupération des en-têtes de colonnes
        string[] headers = csvLines[0].Split(separator);
 
        // Création d'une liste pour stocker les objets JSON
        List<JObject> jsonObjects = new List<JObject>();
 
        // Parcours des lignes du fichier CSV (à partir de la deuxième ligne)
        for (int i = 1; i < csvLines.Length; i++)
        {
            string[] values = ParseCsvLine(csvLines[i], separator);
 
            // Création d'un objet JSON pour chaque ligne
            JObject jsonObject = new JObject();
 
            // Parcours des colonnes
            for (int j = 0; j < headers.Length && j < values.Length; j++)
            {
                jsonObject[headers[j]] = values[j];
            }
 
            // Ajout de l'objet JSON à la liste
            jsonObjects.Add(jsonObject);
        }
 
        // Conversion de la liste d'objets JSON en une chaîne JSON
        string json = JsonConvert.SerializeObject(jsonObjects, Formatting.Indented);
 
        // Affichage du résultat
        Console.WriteLine(json);
        File.WriteAllText(output, json);
    }
 
    static string[] ParseCsvLine(string line, char separator)
    {
        // Liste pour stocker les valeurs analysées
        List<string> values = new List<string>();
 
        // Indicateur pour suivre si nous sommes actuellement à l'intérieur d'une valeur entre guillemets
        bool inQuotes = false;
 
        // Chaîne pour stocker la valeur en cours de traitement
        string currentValue = "";
 
        // Parcours de chaque caractère dans la ligne CSV
        for (int i = 0; i < line.Length; i++)
        {
            char c = line[i];
            if (c == '"')
            {
                if (inQuotes && i + 1 < line.Length && line[i + 1] == '"')
                {
                    // Si le caractère actuel est un guillemet et le suivant est également un guillemet,
                    // cela signifie que ce guillemet fait partie de la valeur
                    currentValue += '"';
                    i++; // Skip the next character
                }
                else
                {
                    // Sinon, basculer l'indicateur inQuotes
                    inQuotes = !inQuotes;
                }
            }
            else if (c == separator && !inQuotes)
            {
                // Si le caractère est le séparateur et n'est pas à l'intérieur de guillemets,
                // ajouter la valeur actuelle à la liste et réinitialiser la valeur actuelle
                values.Add(currentValue.Trim());
                currentValue = "";
            }
            else
            {
                // Ajouter le caractère à la valeur actuelle
                currentValue += c;
            }
        }
 
        // Ajouter la dernière valeur à la liste
        values.Add(currentValue.Trim());
 
        return values.ToArray();
    }
}
