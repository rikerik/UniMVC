namespace MunkasokBeolvasas.Models
{
    using Newtonsoft.Json;
    using System.Collections.Generic;
    using System.IO;
    // A dolgozók repository osztálya, amely a dolgozók beolvasását végzi JSON-ból
    public class WorkerRepository
    {
        // JSON-ból olvassa be a dolgozók adatait a MemoryStream-ból
        public static List<Worker> ReadFromJson(MemoryStream stream)
        {
            // Az adatfolyam pozíciójának az elejére állítása olvasás előtt
            stream.Position = 0;

            using (StreamReader reader = new StreamReader(stream))
            {
                // Teljes JSON beolvasása a streamből
                string json = reader.ReadToEnd();
                // JSON deszérializálása Worker objektumok listájává
                List<Worker> workers = JsonConvert.DeserializeObject<List<Worker>>(json);
                return workers;
            }
        }
    }
}
