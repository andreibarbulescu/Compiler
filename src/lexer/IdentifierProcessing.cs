public class IdentifierProcessing{

    
    // Method to Process Identifier
    public static (string identifier, int newPosition) ProcessIdentifier(string line, int startIndex)
    {
        int i = startIndex;
        string id = "" + line[i]; 

        // Loop to capture the rest of the identifier
        while (i + 1 < line.Length && (char.IsLetterOrDigit(line[i + 1]) || line[i + 1] == '_'))
        {
            i++;
            id += line[i];
        }

        return (id, i); 
    }


}