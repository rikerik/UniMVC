using Newtonsoft.Json;

namespace MunkasokBeolvasas.Models
{
    // A dolgozó modell osztály, amely leírja egy munkás jellemzőit
    public class Worker
    {
        // Az egyedi azonosító, ami az adatbázisban tárolódik
        public int Id { get; set; }
        // A dolgozó vezetékneve, amely a JSON-ben a "Név" kulcshoz tartozik
        [JsonProperty("Név")]
        public string LastName { get; set; }
        // A dolgozó életkora, amely a JSON-ben az "Életkor" kulcshoz tartozik

        [JsonProperty("Életkor")]
        public int Age { get; set; }
        // A dolgozó fizetése, amely a JSON-ben a "Bér" kulcshoz tartozik

        [JsonProperty("Bér")]
        public decimal Salary { get; set; }
        // A dolgozó munkaviszonyának időtartama, amely a JSON-ben a "Munkaviszony" kulcshoz tartozik

        [JsonProperty("Munkaviszony")]
        public int EmploymentDuration { get; set; }
    }
}
