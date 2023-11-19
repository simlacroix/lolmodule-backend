namespace LoLModule;

public class LoLApiClient : HttpClient
{
    public LoLApiClient()
    {
        BaseAddress = new Uri(Globals.BaseUrl);
    }
}