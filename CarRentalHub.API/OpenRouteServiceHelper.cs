using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;

public static class OpenRouteServiceHelper
{
    private static readonly string ApiKey = "enter-your-api-key";
    private static readonly HttpClient client = new HttpClient();

    public static async Task<double> GetDistanceInKmAsync(string from, string to)
    {
        var fromCoords = await GeocodeAsync(from);
        var toCoords = await GeocodeAsync(to);
        if (fromCoords == null || toCoords == null)
            return 0;

        var url = $"https://api.openrouteservice.org/v2/directions/driving-car?api_key={ApiKey}&start={fromCoords[1]},{fromCoords[0]}&end={toCoords[1]},{toCoords[0]}";
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode) return 0;
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var distance = root.GetProperty("features")[0].GetProperty("properties").GetProperty("segments")[0].GetProperty("distance").GetDouble();
        return distance / 1000.0;
    }

    private static async Task<double[]> GeocodeAsync(string location)
    {
        var url = $"https://api.openrouteservice.org/geocode/search?api_key={ApiKey}&text={System.Net.WebUtility.UrlEncode(location)}";
        var response = await client.GetAsync(url);
        if (!response.IsSuccessStatusCode) return null;
        var json = await response.Content.ReadAsStringAsync();
        using var doc = JsonDocument.Parse(json);
        var root = doc.RootElement;
        var features = root.GetProperty("features");
        if (features.GetArrayLength() == 0) return null;
        var coords = features[0].GetProperty("geometry").GetProperty("coordinates");
        return new double[] { coords[1].GetDouble(), coords[0].GetDouble() };
    }
} 
