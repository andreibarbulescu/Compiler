public class FileReader{

    private string _path;

    public FileReader(string path)
    {
        this._path = path;
    }

    public void readFile(){

        try
        {
            using (StreamReader reader = new(_path))
            {
                string line;
                // Read and display lines from the file until the end of the file is reached
                while ((line = reader.ReadLine()) != null)
                {
                    Console.WriteLine(line);
                }
            }
        }
        catch (IOException)
        {
            Console.WriteLine("An Exception has occured");

        }

    }
}